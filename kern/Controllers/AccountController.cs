using kern.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kern.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        public IActionResult Managment()
        {
            ViewData["Message"] = "Управление пользователями";

            // Вызов класса безопасности
            Security security = new Security();

            return View(security);
        }

        [HttpGet]
        public ActionResult AjaxListUsers(string searchText)
        {
            // Вызов класса безопасности
            Security security = new Security();

            // Ввод текста для поиска
            security.SearchContext = searchText == null ? "" : searchText;

            return PartialView(security);
        }

        public IActionResult EditUser(string domainLogin)
        {
            // Вызов класса безопасности
            Security security = new Security();

            // Присваеваем доменный логин пользователя
            security.DomainLogin = domainLogin;

            // Заполняем свойства класса данными из Домена
            security.GetDomainInfo();

            return PartialView(security);
        }

        [HttpPost]
        public JsonResult AddDellRoleUser(string domainLogin, string roleName)
        {
            // Вызов класса безопасности
            Security security = new Security();

            // Присваеваем доменный логин и изменяемую роль пользователя
            security.DomainLogin = domainLogin;
            security.RoleName = roleName;

            // Меняем роль
            security.SetOrRemoveRole();

            return Json(1);
        }
    }
}