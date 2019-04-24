using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kern.Models.Authentication;
using kern.Models.MVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace kern.Controllers
{
    [Authorize(Roles = "Load")]
    public class DataBankController : Controller
    {
        public IActionResult Field()
        {
            FieldMVC field = new FieldMVC();

            return View(field);
        }

        [HttpGet]
        public ActionResult AjaxListFields(string nameField, string deposit)
        {
            // Вызов класса месторождений
            FieldMVC field = new FieldMVC();

            // Ввод текста для поиска по названию месторождения
            field.NameField = nameField == null ? "" : nameField;

            // Ввод текста для поиска по названию залежи 
            field.Deposit = deposit == null ? "" : deposit;

            return PartialView(field);
        }

        public IActionResult EditField(int idField)
        {
            // Вызов класса залежи
            FieldMVC field = new FieldMVC();

            // Присваеваем уникальный идентификатор
            field.Id = idField;

            return PartialView("_EditField",field);
        }

        [HttpPost]
        public JsonResult RegisterField(FieldMVC field)
        {
            // Повторная проверка введенных данных на случай мошенничества
            if (ModelState.IsValid) {
                // Извлекаем ИД для того, чтобы показать была изменена старая запись или создана новая
                var id = field.Id;

                // Создаем новую/изменяем старую запись залежи
                field.ModifyField();

                // Вывод на экран в зависимости от того, новая эта запись или старая
                return Json(id == 0 ? "Новая залежь успешно добавлена." : "Залежь успешно изменена.");
            }
            else {
                return Json("Произошла ошибка. Обратитесь к администратору.");
            }
                
        }

        public IActionResult Create(FieldMVC field)
        {
            return View(field);
        }

        [HttpPost]
        public IActionResult RegisterInput(FieldMVC field)
        {
            if (ModelState.IsValid)
                return Content($"{field.NameField} - {field.Deposit}");
            else
                return Content($"{field.Deposit} - {field.NameField}");
        }


    }
}