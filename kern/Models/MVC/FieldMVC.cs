using kern.Models.DataBase;
using PostgresApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kern.Models.MVC
{
    public class FieldMVC
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Месторождение")]
        [Required(ErrorMessage = "Не указано месторождение")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Принимает от 5 до 30 символов")]
        public string NameField { get; set; }

        [Display(Name = "Залежь")]
        [Required(ErrorMessage ="Не указана залежь")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Принимает от 5 до 30 символов")]
        public string Deposit { get; set; }

        [Display(Name = "Год открытия")]
        [Required(ErrorMessage = "Не указан год открытия")]
        [Range(1920, 2050, ErrorMessage = "Принимает значения от 1920 до 2050")]
        public int YearOpening { get; set; }

        [Display(Name = "Год начала разработки")]
        [Required(ErrorMessage = "Не указан год начала разработки")]
        [Range(1920, 2050, ErrorMessage = "Принимает значения от 1920 до 2050")]
        public int YearStartDevelopment { get; set; }

        // Извлечь залежи из БД
        public List<BaseField> GetListFields()
        {
            // Возвращаемый список
            List<BaseField> returnList = new List<BaseField>();

            // Инициализация БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем пользователя по доменному логину
                returnList = db.BaseFields.Where(f => f.NameField.ToUpper().Contains(NameField.ToUpper()) || 
                                                      f.Deposit.ToUpper().Contains(Deposit.ToUpper()) ||
                                                      NameField == "" ||
                                                      Deposit == "").ToList();

                // Возвращаем список
                return returnList;
            }
        }

        // Создание нового или изменение существующей залежи
        public void ModifyField()
        {
            // Работа с БД
            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем залежь по Id
                var fields = db.BaseFields.Where(u => u.Id == Id).ToList();

                // Если такой залежи не существует, то созадем нового
                if (fields.Count == 0)
                {
                    // Создание нового класса залежи БД
                    BaseField bf = new BaseField();
                    bf.NameField = NameField;
                    bf.Deposit = Deposit;
                    bf.YearOpening = YearOpening;
                    bf.YearStartDevelopment = YearStartDevelopment;
                    // Добавление в БД
                    db.BaseFields.Add(bf);
                    // Сохранение в БД
                    db.SaveChanges();
                } else
                {
                    // Находим залежь
                    var bf = fields.First();
                    // Изменяем свойства
                    bf.NameField = NameField;
                    bf.Deposit = Deposit;
                    bf.YearOpening = YearOpening;
                    bf.YearStartDevelopment = YearStartDevelopment;
                    // Сохранение в БД
                    db.SaveChanges();
                }
            }
        }
    }
}
