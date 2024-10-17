using Calculator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Calculator.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Calculator.Controllers
{
    public class RawMaterialController : Controller
    {
        private readonly ApplicationDbContext context;

        public RawMaterialController(ApplicationDbContext _context)
        {
            this.context = _context;
        }
        public IActionResult RawMaterial()
        {
            //var articles = context.Articles.Include(a => a.ArticleComponents).ToList();
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = context.Users.FirstOrDefault(u => u.Email == userEmail);


            var rawMaterials = context.Components
               // .Include(a => a.ArticleComponents)
               // .Where(a => a.UserId == user.Id) // Filtrer par UserId
                .ToList();

            var rawmat = new ListComponentDto
            {
                Components = rawMaterials
            };

            

            return View(rawmat);
        }

        

        // GET: RawMaterial/Create
        public IActionResult AddRawMaterial()
        {
            return View(); // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
        }

         [HttpPost]
    public IActionResult AddRawMaterial(RawMaterialDto rawMaterialDto)
    {
       
      var existingComponent = context.Components.FirstOrDefault(comp => comp.Name == rawMaterialDto.Name);
        if (existingComponent == null)
        {
            var component = new Component
            {
                Name = rawMaterialDto.Name,
                Unit = rawMaterialDto.Unit,
            };
       

        context.Components.Add(component);
        context.SaveChanges();
           
        }
       

        
          
        return RedirectToAction("RawMaterial", "RawMaterial");
    }

  
        public ActionResult EditRawMaterial(int id, string Name, string Unit)
        {
            var component = context.Components.FirstOrDefault(comp => comp.Id == id);
            component.Name = Name;
            component.Unit = Unit;
            context.Components.Update(component);
            context.SaveChanges();
            return RedirectToAction("RawMaterial", "RawMaterial");
        }

       



        // GET: RawMaterial/Delete/5
        public ActionResult DeleteRawMaterial(int rawmaterialId)
        {
          
                var rawMaterial = context.Components.FirstOrDefault(o => o.Id == rawmaterialId);

                context.Components.Remove(rawMaterial);
                context.SaveChanges();
                return RedirectToAction("RawMaterial", "RawMaterial");
            
            
        }

      
    }
}
