using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using kern.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PostgresApp;
using kern.Models.DataBase;
using System.DirectoryServices.AccountManagement;
using System.Text.RegularExpressions;
using kern.Models.Authentication;

namespace kern.Controllers
{
    public class HomeController : Controller
    {
        [FirstEntryFilter]
        public IActionResult Index()
        {
            ViewData["Message"] = "Домашняя страница";

            string userName = HttpContext.User.Identity.Name;

            string userName2 = User.Identity.Name;

            Regex re = new Regex(@"\w+/\/\d+");
            Match m = re.Match(userName);
            string[] nameArr = userName.Split(new char[] { '\\' });

            var context = new PrincipalContext(ContextType.Domain, "NIPI");
            var principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            var identityName = User.Identity.Name;
            var firstName = principal.GivenName;
            var lastName = principal.Surname;
            var middleName = principal.MiddleName;
            var emailAddress = principal.EmailAddress;

            return View();
        }

        [FirstEntryFilter]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [FirstEntryFilter]
        [Authorize(Roles = "Admin")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Test()
        {
            ViewData["Message"] = "Test page.";

            return View();
        }

        public IActionResult Test2()
        {
            ViewData["Message"] = "Test page.";

            return View();
        }

        public void Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimTypes.Role, "admin")
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {
            if (User.FindFirst("Authenticated") == null)
                return RedirectToAction("Login", new { returnUrl });

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Извлекаем доменный логин пользователя
            string domainLogin = User.Identity.Name;

            // Вызов класса безопасности
            Security security = new Security() { DomainLogin = domainLogin };

            // Извлекаем роли пользователя из БД
            var userRoles = security.GetUserRoles();

            // Авторизация на основе Claims
            var claims = new List<Claim>();

            // Добавляем логин пользователя в Claims
            claims.Add(new Claim(ClaimTypes.Name, domainLogin, ClaimValueTypes.String));

            // Добавление ролей пользователя в Claims
            foreach (AccauntRole ur in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, ur.RoleName, ClaimValueTypes.String));
            }

            // Реализация удостоверения на основе утверждений
            var userIdentity = new ClaimsIdentity("SuperSecureLogin");

            // Добавление claims в удостоверение
            userIdentity.AddClaims(claims);

            // Предоставление коллекции учетных данных
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // Создаем зашифрованный файл cookie и добавляем его в текущий ответ
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return RedirectToLocal(returnUrl);
        }

        [AllowAnonymous]
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
