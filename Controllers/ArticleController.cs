using Calculator.Migrations;
using Calculator.Models;
using Calculator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

public class ArticleController : Controller
{
    private readonly ApplicationDbContext context;

    public ArticleController(ApplicationDbContext _context)
    {
        this.context = _context;
    }

    public IActionResult Article()
    {
       //var articles = context.Articles.Include(a => a.ArticleComponents).ToList();
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = context.Users.FirstOrDefault(u => u.Email == userEmail);

        
            var articles = context.Articles
                .Include(a => a.ArticleComponents)
                .Where(a => a.UserId == user.Id) // Filtrer par UserId
                .ToList();

        var components = context.Components.ToList();

        // Gérer le cas où l'utilisateur n'est pas trouvé (peut-être rediriger vers la page de connexion)

        var comp = new ComponentDto
        {
            articles = articles,
            Name = "",
            components = components

        };
        
        return View(comp);
    }

    public IActionResult Create()
    {
        return View(); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }
    public IActionResult EditArticle(int id)
    {
        var article = context.Articles.Include(a =>a.ArticleComponents).ToList().FirstOrDefault(article => article.Id == id);
        // var Components = context.ArticleComponents.Select(c=> new ComponentDto
        // {
        //     Id= c.ComponentId,

        // });

        var name = article.Name;
        var articleDto = new ArticleDto
        {
            Nameart = name,
            Id = id
            


        };
        return View(articleDto); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }

    [HttpPost]
    public IActionResult EditArticle(int Id, string Nameart)
    {
        var article = context.Articles.Include(a => a.ArticleComponents).ToList().FirstOrDefault(article => article.Id == Id);
        
        article.Name = Nameart;
        context.Articles.Update(article);
        context.SaveChanges();

        
        return RedirectToAction("Article","Article"); 
    }
    [HttpPost]
    public IActionResult Create(string Nameart)
    {
        if (!ModelState.IsValid)
        {
            return View(); // Passez l'articleDto à la vue Create
        }

        // Récupérer l'ID de l'utilisateur à partir des claims
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = context.Users.FirstOrDefault(u => u.Email == userEmail);

        var article = new Article
        {
            Name = Nameart,
            UserId = user?.Id ?? 0, // Assurez-vous que l'ID de l'utilisateur est bien assigné
            CreationDate = DateTime.Now,
        };

        context.Articles.Add(article);
        context.SaveChanges();

        return RedirectToAction("Article", "Article");
    }

    public IActionResult EditComponent(int id, int Quantity, decimal Price)
    {
        var component = context.ArticleComponents.Include(c=>c.Component).ToList().FirstOrDefault(artcomp => artcomp.ComponentId == id);

        component.Quantity = Quantity;
        component.Price = Price;
        context.ArticleComponents.Update(component);
        context.SaveChanges();
        //var componentDto = new ArticleDetailsViewModel
        //{
        //    Id = id,
        // ComponentName = component.Component.Name,
        // Price = component.Price,
        // Quantity = component.Quantity,

        //};
        return RedirectToAction("Details", "Article", new { id = component.ArticleId }); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }
 
  

    [HttpPost]
    public IActionResult Addcomponent(ComponentDto componentDto, int articleId)
    {
        //var compoentId = 0;
        var existingComponent = context.Components.FirstOrDefault(comp => comp.Name == componentDto.Name);
       

        // Récupérer l'article de la base de données
        var article = context.Articles
            .Include(a => a.ArticleComponents)
            .FirstOrDefault(a => a.Id == articleId);
        var ArtComp = new ArticleComponent
        {
            ArticleId = articleId,
            ComponentId = existingComponent.Id,
            Quantity = componentDto.Quantity,
            Price = componentDto.Price
        };
        article.ArticleComponents.Add(ArtComp);
        context.SaveChanges();

        return RedirectToAction("Article", "Article");
    }
    public IActionResult AddComponent()
    {
        return View(); // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
    }

  

    public IActionResult Details(int id)
    {
        var articleComponents = context.ArticleComponents.Where(c => c.ArticleId == id).Include(c=>c.Component).Include(c => c.Article).ToList() ;
        var articleName = context.Articles.FirstOrDefault(c => c.Id == id).Name;
        
        var components = articleComponents.Select(c => new ComponentDto
     {
        Id=c.Component.Id,
         Name = c.Component.Name,  // Accéder au nom du composant à travers l'inclusion
         Quantity = c.Quantity,              // Inclure d'autres propriétés si nécessaire
         Price = c.Price,
         Tva =c.tva,
         totalpriceTTC = c.totalpriceTTC,
        })
     .ToList();

        var articleDetailsViewModel = new ArticleDetailsViewModel
        {
            ArticleName = articleName,
            ArticleId = id,
            Components = components

        };
        return View(articleDetailsViewModel); 
    }
    public IActionResult DeleteArticle()
    {
        return View(); // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
    }
    [HttpPost]
    public IActionResult DeleteArticle( int articleId, string type )
    {   if (type == "article")
        {
            var article = context.Articles.FirstOrDefault(o => o.Id == articleId);
            
            context.Articles.Remove(article);
            context.SaveChanges();
            return RedirectToAction("Article", "Article");
        }
        if (type == "component")
        {
            var component = context.ArticleComponents.FirstOrDefault(o => o.ComponentId == articleId);

             context.ArticleComponents.Remove(component);
             context.SaveChanges();
            return RedirectToAction("Details", "Article", new { id = component.ArticleId });
        }
        else {
            return View(); 
        }
        
            
        
        
    }

   

    [HttpPost]
    public IActionResult UpdateTva([FromBody] TvaUpdateModelDto model)
    {
        Console.WriteLine("Méthode UpdateTva appelée");
        Console.WriteLine("Données reçues : " + JsonSerializer.Serialize(model)); // Nécessite using System.Text.Json;

        if (ModelState.IsValid)
        {
            var component = context.ArticleComponents
                .FirstOrDefault(c => c.ArticleId == model.articleId && c.ComponentId == model.componentId);

            if (component == null)
            {
                Console.WriteLine($"Composant non trouvé pour ArticleId={model.articleId}, ComponentId={model.componentId}");
                return NotFound(new { success = false, message = "Component not found." });
            }

            try
            {
                component.tva = model.Tva;
                component.totalpriceTTC = model.totalpriceTTC;
                context.Update(component);
                context.SaveChanges();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la mise à jour de la TVA: " + ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        Console.WriteLine("ModelState non valide : " + JsonSerializer.Serialize(ModelState));
        return BadRequest(new { success = false, errors = ModelState });
    }



    public IActionResult Statistic() 
    { 
        return View(); 
    }
}
