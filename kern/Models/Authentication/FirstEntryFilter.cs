using Microsoft.AspNetCore.Mvc.Filters;
using System;


namespace kern.Models.Authentication
{
    public class FirstEntryFilter : Attribute, IActionFilter
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Фильтр создания записи в базе данных о пользователе при первичном входе.
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Извлекаем доменный логин пользователя
            string domainLogin = context.HttpContext.User.Identity.Name;

            // Вызов класса безопасности
            Security security = new Security() { DomainLogin = domainLogin };

            // Запись пользователя
            security.InsertUser();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
