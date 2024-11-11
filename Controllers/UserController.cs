using Calculator.Models;
using Calculator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Calculator.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;


        public UserController(ApplicationDbContext _context)
        {
            this.context = _context;

        }
        public User GetUserByEmail(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email == email);
        }
        private string GenerateJwtToken(string userEmail)
        {
            // Retrieve user details from the database based on the userEmail
            // User user = GetUserByEmail(userEmail);
            User user = GetUserByEmail(userEmail);
            
            if (user == null)
            {
                // Handle the case where the user is not found
                throw new Exception("User not found");
            }
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
                new Claim("Id",user.Id.ToString()),
                new Claim("Name",user.Name.ToString()),


            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_1_top_2_secret_3_key_4_1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

       
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            
            try
            {
                
                var user = await context.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.Password == request.Password);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid login attempt." });
                }

                var token = GenerateJwtToken(user.Email);

                Response.Cookies.Append("token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
                Response.Cookies.Append("Name", user.Name, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                // Return a JSON response instead of redirecting
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /* public IActionResult Login()
         {
             //  ClaimsPrincipal claimUser = HttpContext.User;
             ////  throw new Exception($"{claimUser.Identity.IsAuthenticated}");
             //  // Vérifiez si l'utilisateur est authentifié
             //  if (claimUser.Identity.IsAuthenticated)
             //  {
             //      throw new Exception($"{claimUser.Identity.IsAuthenticated}");
             //      return RedirectToAction("Index", "Home");
             //  }

             return View();
         }*/
        private bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("my_1_top_2_secret_3_key_4_1234567890"); // Utilisez la même clé que pour la génération du token

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Optionnel : supprimez le délai d'expiration
                }, out SecurityToken validatedToken);

                return true; // Token est valide
            }
            catch
            {
                return false; // Token n'est pas valide
            }
        }


        public async Task<IActionResult> Login()
        {
            /*var token = HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
           
            if (!string.IsNullOrEmpty(token))
            {
                // Validez le token ici (similaire à ce qui a été montré précédemment)
                if (IsTokenValid(token)) // Méthode simple pour vérifier la validité
                {
                    return RedirectToAction("Index", "Home");
                }
            }*/
            var userExists = await context.Users.AnyAsync(); // AnyAsync renvoie true si au moins un utilisateur existe

            // Passer cette information à la vue
            

            var user = new UserDto
            {
                existaccount = userExists
            };

            return View(user);
        }

        /* 
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

                 return RedirectToAction("Article", "Article");
             }
             ViewData["ValidateMessage"] = "L'utilisateur n'a pas été trouvé";
             return View();
         }
       /*
       [HttpPost]
       public async Task<IActionResult> Login(UserLogin userDto)
       {
           if (!ModelState.IsValid)
           {
               var errors = ModelState.Values
       .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
       .ToList();

               return BadRequest(new { message = "Données invalides", errors });
           }
           // Rechercher l'utilisateur dans la base de données
           var user = await context.Users
               .SingleOrDefaultAsync(u => u.Email == userDto.Email);

           if (user == null)
           {
               return BadRequest(new { message = "Utilisateur non trouvé ou mot de passe incorrect" });
           }
           if (user.Password != userDto.Password) // Remplacez par une vérification sécurisée
           {
               return BadRequest(new { message = "Mot de passe incorrect" });
           }

           // Créer les claims
           var claims = new List<Claim>
   {
       new Claim(ClaimTypes.Name, user.Name),
       new Claim(ClaimTypes.NameIdentifier, userDto.Email),
   };

           // Créer le token JWT
           var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey123456789-YourSecretKey12345678"));
           var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
           var expiration = DateTime.UtcNow.AddMinutes(30);

           var token = new JwtSecurityToken(
               issuer: "YourIssuer",
               audience: "YourAudience",
               expires: expiration,
               signingCredentials: creds,
               claims: claims
           );
           TempData["token"] = token;

           //return Ok(new
           // {
           //     token = new JwtSecurityTokenHandler().WriteToken(token),
           //     expiration
           // });


           // Redirection vers une autre action ou page
           return RedirectToAction("Index", "Home");
       }
       */


        [HttpPost]
        public IActionResult SignUp(UserDto accountDto)
        {


          //  if (!ModelState.IsValid)
          //  {
          //      var errorMessages = ModelState
          //.Where(ms => ms.Value.Errors.Count > 0)
          //.Select(ms => new
          //{
          //    Key = ms.Key,
          //    Errors = ms.Value.Errors.Select(e => e.ErrorMessage)
          //})
          //.SelectMany(e => e.Errors)
          //.Aggregate((current, next) => current + " | " + next);


          //      var exceptionMessage = $"Validation failed: {errorMessages}";
          //      throw new Exception(exceptionMessage);

          //      return View();
          //  }

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
        private string GetNameFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("my_1_top_2_secret_3_key_4_1234567890"); // Utilisez la même clé que pour la génération du token

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Supprimez le délai d'expiration
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;

                // Utilisez ClaimTypes.Name pour récupérer la claim "Name"
                var nameClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "Id");
                string name = nameClaim?.Value; // Récupérez la valeur

                return name; // Retournez le nom
            }
            catch (Exception ex)
            {
                // Optionnel : loggez l'exception pour le débogage
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null; // Token n'est pas valide
            }
        }

        public IActionResult Profile()
        {
            

            var token = HttpContext.Request.Cookies["token"];
            var Id = GetNameFromToken(token);

            if (IsTokenValid(token)) {
                var user = context.Users.FirstOrDefault(user => (user.Id).ToString() == Id);
               // var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
           // var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
          // var user = context.Users.FirstOrDefault(u => u.Email == userEmail);
            int totalArticles = context.Articles.Count(a => a.UserId == user.Id);
            int totalComponents = context.Components.Count(a => a.UserId == user.Id);
            var model = new UserDto
            {
                Name = user.Name,
                Email = user.Email,
                Password = "aaaaaaaaaa", // Placeholder
                KeepLoggedIn = false,
                TotalArticles = totalArticles,
                TotalComponents = totalComponents,
                token=token

            };

            return View(model);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        


        public IActionResult EditProfile()

        {
            var token = HttpContext.Request.Cookies["token"];
           
            var Id = GetNameFromToken(token);

            if (!IsTokenValid(token))
            {
                return RedirectToAction("Login", "User"); // Rediriger vers la page de connexion si non authentifié
            }
            else
            {
                var userName = context.Users.FirstOrDefault(c => c.Id.ToString() == Id)?.Name;
                var userEmail = context.Users.FirstOrDefault(c => c.Id.ToString() == Id)?.Email;

                var model = new UserDto
                {
                    Name = userName,
                    Email = userEmail,
                    token = token
                    
                };

                return View(model);
            }



           
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfil([FromBody]  UserDto userDto)
        {
            var token = HttpContext.Request.Cookies["token"];

            var Id = GetNameFromToken(token);
            if (!IsTokenValid(token))
            {
                return RedirectToAction("Login", "User"); // Rediriger vers la page de connexion si non authentifié
            }
            
                //var userName = context.Users.FirstOrDefault(c => c.Id.ToString() == Id)?.Name;
                var userEmail = context.Users.FirstOrDefault(c => c.Id.ToString() == Id)?.Email;
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
            //return RedirectToAction("Profile", "User");
            
            //var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
           // var user = context.Users.FirstOrDefault(o => o.Email == userEmail);

           
        }


        public IActionResult EditPassword()
        {
            var token = HttpContext.Request.Cookies["token"];

            var Id = GetNameFromToken(token);

            if (!IsTokenValid(token))
            {
                return RedirectToAction("Login", "User"); // Rediriger vers la page de connexion si non authentifié
            }
            else
            {

                var model = new UserDto
                {
                   
                    token = token

                };

                return View(model);
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPass([FromBody] UserDto userDto)
        {
            var token = HttpContext.Request.Cookies["token"];

            var Id = GetNameFromToken(token);
            if (!IsTokenValid(token))
            {
                return RedirectToAction("Login", "User"); // Rediriger vers la page de connexion si non authentifié
            }
            var userEmail = context.Users.FirstOrDefault(c => c.Id.ToString() == Id)?.Email;
            var user = context.Users.FirstOrDefault(o => o.Email == userEmail);
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
