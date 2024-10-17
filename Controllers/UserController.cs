using Calculator.Models;
using Calculator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Calculator.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;


        public UserController(ApplicationDbContext _context)
        {
            this.context = _context;

        }
        public IActionResult Login()
        {
            ClaimsPrincipal ClaimUser = HttpContext.User;
            if (ClaimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            var user = await context.Users
               .SingleOrDefaultAsync(u => u.Email == userDto.Email && u.Password == userDto.Password);

            if (user != null)

            {
                
                List<Claim> claims = new List<Claim>()
                {
                     new Claim(ClaimTypes.Name, user.Name),
                    //  new Claim(ClaimTypes.Email, userDto.Email),
                    new Claim(ClaimTypes.NameIdentifier, userDto.Email),
                   // new Claim(ClaimTypes.Email, userDto.Email),

                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = userDto.KeepLoggedIn
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
               //var principal = new ClaimsPrincipal(claimsIdentity);
               // var claimsList = principal.Claims.ToList();
                return RedirectToAction("Article", "Article");
            }
            ViewData["ValidateMessage"] = "L'utilisateur n'a pas été trouvé";
            return View();
        }



        [HttpPost]
        public IActionResult SignUp(UserDto accountDto)
        {


            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
          .Where(ms => ms.Value.Errors.Count > 0)
          .Select(ms => new
          {
              Key = ms.Key,
              Errors = ms.Value.Errors.Select(e => e.ErrorMessage)
          })
          .SelectMany(e => e.Errors)
          .Aggregate((current, next) => current + " | " + next);


                var exceptionMessage = $"Validation failed: {errorMessages}";
                throw new Exception(exceptionMessage);

                return View();
            }

            // Vérifiez si l'adresse e-mail existe déjà
            var existingUser = context.Users
                .SingleOrDefault(u => u.Email == accountDto.Email);

            if (existingUser != null)
            {
                // Si l'utilisateur existe déjà, renvoyez un message d'erreur
                ViewData["ValidationMessage"] = "Cette adresse e-mail est déjà utilisée.";
                return RedirectToAction("Login", "User");
            }
            var account = new User
            {
                Name = accountDto.Name,
                Email = accountDto.Email,
                Password = accountDto.Password,

            };

            context.Users.Add(account);
            context.SaveChanges();


            return RedirectToAction("Login", "User");

        }

        public IActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Rediriger vers la page de connexion si non authentifié
            }



           var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
           var user = context.Users.FirstOrDefault(u => u.Email == userEmail);
            int totalArticles = context.Articles.Count(a => a.UserId == user.Id);
            int totalComponents = context.Components.Count();
            var model = new UserDto
            {
                Name = userName,
                Email = userEmail,
                Password = "aaaaaaaaaa", // Placeholder
                KeepLoggedIn = false,
                TotalArticles = totalArticles,
                TotalComponents = totalComponents,

            };

            return View(model);
        }
        


        public IActionResult EditProfile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Rediriger vers la page de connexion si non authentifié
            }



            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            var model = new UserDto
            {
                Name = userName,
                Email = userEmail,
            
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserDto userDto)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = context.Users.FirstOrDefault(o => o.Email == userEmail);

            if (user == null)
            {
                return NotFound(); // Ou gérer l'erreur comme vous le souhaitez
            }

            var pass = user.Password;

            if (userDto.Password == pass)
            {
                // Mettre à jour les informations de l'utilisateur
                user.Email = userDto.Email;
                user.Name = userDto.Name;
                context.Users.Update(user);
                await context.SaveChangesAsync();

                // Mettre à jour les revendications d'authentification
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Email),
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = userDto.KeepLoggedIn // Assurez-vous que KeepLoggedIn est bien défini dans UserDto
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                // return RedirectToAction("Profile", "User");
                return Json(new { success = true }); // Réponse JSON indiquant succès
            }
            else
            {
                // Ajoutez un message d'erreur à ViewData et renvoyez la vue avec le modèle
                ViewData["ErrorMessage"] = "Le mot de passe est incorrect.";
                return Json(new { success = false, errorMessage = "Le mot de passe est incorrect." });

                // return View(userDto); // Renvoie le modèle à la vue pour afficher les erreurs
            }
            return RedirectToAction("Profile", "User");
        }


        public IActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditPassword(UserDto userDto)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = context.Users.FirstOrDefault(o => o.Email == userEmail);
            if (user == null)
            {
                throw new Exception($"{user.Email}");
            }
            if (user == null)
            {
                return NotFound(); // Ou gérer l'erreur comme vous le souhaitez
            }

            var pass = user.Password;
            if (userDto.newPassword == userDto.confPassword)
            {
                if (userDto.Password == pass)
                {
                    // Mettre à jour les informations de l'utilisateur
                    user.Password =userDto.newPassword ;
                    
                    context.Users.Update(user);
                    await context.SaveChangesAsync();


                    return Json(new { success = true }); // Réponse JSON indiquant succès
                }
                else
                {
                    return Json(new { success = false, errorMessagePass = "Le mot de passe est incorrect.", errorMessageConf="" });

                }
            }
            else
            {
                if (userDto.Password != pass)
                {
                    return Json(new { success = false, errorMessagePass = "Le mot de passe est incorrect.", errorMessageConf = "Les mots de passe ne correspondent pas. Veuillez vérifier et réessayer." });

                }
                else
                {
                    return Json(new { success = false, errorMessagePass="", errorMessageConf = "Les mots de passe ne correspondent pas. Veuillez vérifier et réessayer." });

                }


            }
            
        }



    }
}
