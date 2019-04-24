using System.Linq;
using System;
using System.DirectoryServices.AccountManagement;
using PostgresApp;
using kern.Models.DataBase;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace kern.Models.Authentication
{
    public class Security
    {
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Класс для синхронизации доменнных данных пользователей с БД, раздачи прав.
        // Необходимо для дальнейшего расширения его прав и логирования его действий.
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // Доменный логин
        public string DomainLogin { get; set; }

        // ФИО пользователя из домена
        public string FullName { get; set; }

        // Адрес электронной почты из домена
        public string EmailAddress { get; set; }

        // Домен пользователя
        public string Domain { get; set; }

        // Название роли, необходим при добавлении или удалении дайнной роли у пользователя
        public string RoleName { get; set; }

        // Добавлена для поиска в различных запросах по условию содержания
        public string SearchContext { get; set; }

        // Заполняем свойства класса данными из домена
        public void GetDomainInfo()
        {
            // проверки
            PersonException();

            // Разбиваем доменный логин на домен и логин
            string[] nameArr = DomainLogin.Split(new char[] { '\\' });

            // Заполняем домен пользователя
            Domain = nameArr[0];

            // Инкапсуляция сервера для выполнения операций по пользователям
            var principalContext = new PrincipalContext(ContextType.Domain, nameArr[0]);

            // Ищем пользователя
            var principal = UserPrincipal.FindByIdentity(principalContext, DomainLogin);

            // Извлекаем его полное имя и email адрес
            FullName = principal.Name;
            EmailAddress = principal.EmailAddress;
        }

        // Запись доменных данных пользователя в БД
        public void InsertUser()
        {
            // Заполнить свойства класса
            GetDomainInfo();

            // Работа с БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем пользователя по доменному логину
                var usersDb = db.AccauntUsers.Where(u => u.Login == DomainLogin.Trim()).ToList();

                // Если такого пользователя не существует, то созадем нового
                if (usersDb.Count == 0)
                {
                    // Создание нового класса пользователя БД
                    AccauntUser accauntUser = new AccauntUser();
                    accauntUser.Login = DomainLogin;
                    accauntUser.UserName = FullName;
                    accauntUser.Email = EmailAddress;
                    accauntUser.CreateDate = DateTime.Now;
                    accauntUser.IsValid = true;
                    // Добавление в БД
                    db.AccauntUsers.Add(accauntUser);
                    // Сохранение в БД
                    db.SaveChanges();
                }
            }
        }

        // Извлечь всех пользователей из БД
        public List<AccauntUser> GetListUsers()
        {
            // Инкапсуляция возвращаемого списка
            List<AccauntUser> returnList = new List<AccauntUser>();

            // Инициализация БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Извлекаем список пользователей, для поиска применяем фильтр на свойства Логин, ФИО и адрес электронной почты
                returnList = db.AccauntUsers.Where(u => u.Login.ToUpper().Contains(SearchContext.ToUpper()) ||
                                                        u.UserName.ToUpper().Contains(SearchContext.ToUpper()) ||
                                                        u.Email.ToUpper().Contains(SearchContext.ToUpper()) ||
                                                        SearchContext == ""
                ).ToList();
            }

            return returnList;
        }

        // Извлечь ПРИСВОЕННЫЕ роли конкретного пользователя
        public List<AccauntRole> GetUserRoles()
        {
            // проверки
            PersonException();

            // Инициализация БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем пользователя по доменному логину
                AccauntUser user = db.AccauntUsers.Where(u => u.Login == DomainLogin.Trim() && 
                                                              u.IsValid == true).ToList().First();

                // Роли пользователя
                var serv = (from r in db.AccauntRoles
                            join ur in db.AccauntUsersRoles on r.IdRole equals ur.IdRole
                            where ur.IdUser == user
                            select r).ToList();

                return serv;
            }
        }

        // Роль с пометкой существования данной роли у пользователя
        public class RoleUser
        {
            public int IdRole { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public bool flagRole { get; set; }
        }

        // Извлечь все существующие роли в БД с пометкой ПРИСВОЕНИЯ или УДАЛЕНИЯ данной роли у конкретного пользователя
        public List<RoleUser> GetAllRoleUser()
        {
            // проверки
            PersonException();

            List<RoleUser> returnList = new List<RoleUser>();

            // Инициализация БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем пользователя по доменному логину
                AccauntUser user = db.AccauntUsers.Where(u => u.Login == DomainLogin.Trim() &&
                                                              u.IsValid == true).ToList().First();

                // Left Join Для того, чтобы извлечь все роли + пометки о присвоении роли
                var serv = (from r in db.AccauntRoles
                            join ur in db.AccauntUsersRoles.Where(ur => ur.IdUser == user) on r.IdRole equals ur.IdRole into ps
                            from x in ps.DefaultIfEmpty()
                            orderby r.IdRole
                            select new {
                                r.IdRole,
                                r.RoleName,
                                r.Description,
                                flagRole = (x == null ? false : true)
                            }
                            ).ToList();

                foreach(var ur in serv)
                {
                    RoleUser roleUser = new RoleUser();
                    roleUser.IdRole = ur.IdRole;
                    roleUser.RoleName = ur.RoleName;
                    roleUser.Description = ur.Description;
                    roleUser.flagRole = ur.flagRole;

                    returnList.Add(roleUser);
                }

                return returnList;
            }
        }

        // УСТАНОВИТЬ-УДАЛИТЬ роль пользователя
        public void SetOrRemoveRole()
        {
            // Проверки
            PersonException2();

            // Извлекаем пользователя и роль из БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем пользователя по доменному логину
                AccauntUser user = db.AccauntUsers.Where(u => u.Login == DomainLogin.Trim() &&
                                                              u.IsValid == true).ToList().First();

                // Ищем роль в справочнике ролей
                AccauntRole role = db.AccauntRoles.Where(r => r.RoleName == RoleName.Trim() &&
                                                              r.isValid == true).ToList().First();

                // Ищем присвоенные роли этого пользователя
                List<AccauntUsersRole> userRole = db.AccauntUsersRoles.Where(ur => ur.IdUser == user &&
                                                                                   ur.IdRole == role.IdRole).ToList();

                // Если роль присвоена, то удаляем, иначе присваеваем
                if (userRole.Count() > 0)
                {
                    db.AccauntUsersRoles.Attach(userRole.First());
                    db.AccauntUsersRoles.Remove(userRole.First());
                    db.SaveChanges();
                }
                else
                {
                    // Создание нового класса роли пользователя БД
                    AccauntUsersRole accauntUserRole = new AccauntUsersRole();
                    accauntUserRole.IdUser = user;
                    accauntUserRole.IdRole = role.IdRole;
                    accauntUserRole.CreateDate = DateTime.Now;

                    // Добавление в БД
                    db.AccauntUsersRoles.Add(accauntUserRole);

                    // Сохранение в БД
                    db.SaveChanges();
                }
            }
        }

        // Проверки
        // Требование обязательного заполнения DomainLogin
        private void PersonException()
        {
            if (DomainLogin == "" || DomainLogin is null)
            {
                throw new Exception("Необходимо заполнить поле DomainLogin");
            }
        }

        // Требование обязательного заполнения DomainLogin и RoleName
        private void PersonException2()
        {
            if (DomainLogin == "" || DomainLogin is null || RoleName == "" || RoleName is null)
            {
                throw new Exception("Необходимо заполнить поле DomainLogin и RoleName");
            }
        }
    }
}
