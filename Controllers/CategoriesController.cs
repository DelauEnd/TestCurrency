using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCurrency.Data;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestCurrency.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        ProductsDBContext db;
        public CategoriesController(ProductsDBContext context)
        {
            db = context;
            if (!db.Products.Any())
            {
                var cat = new Categories { Title = "Кат1" };
                db.Categories.Add(cat);
                db.SaveChanges();
                db.Products.Add(new Products { Title = "Товар1", Cost = 0.99f, CategoryId = 1 });
                db.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> Get()
        {
            return await db.Categories.Include(x=>x.Products).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> Get(int id)
        {
            Categories prod = await db.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.CategoriesId == id);
            if (prod == null)
                return NotFound();
            return new ObjectResult(prod);
        }

        [HttpGet]
        [Route("search/{title}")]
        public async Task<ActionResult<IEnumerable<Categories>>> Search(string title)
        {
            var catList = await db.Categories.Include(x => x.Products).Where(x => x.Title.ToLower().Contains(title.ToLower())).ToListAsync();
            if (catList.Count == 0)
                return NotFound();
            return catList;
        }

        [HttpPost]
        public async Task<ActionResult<Categories>> Post(Categories prod)
        {
            if (prod == null)
            {
                return BadRequest();
            }

            db.Categories.Add(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }

        [HttpPut]
        public async Task<ActionResult<Categories>> Put(Categories prod)
        {
            if (prod == null)
            {
                return BadRequest();
            }
            if (!db.Categories.Any(x => x.CategoriesId == prod.CategoriesId))
            {
                return NotFound();
            }

            db.Update(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Categories>> Delete(int id)
        {
            Categories prod = db.Categories.FirstOrDefault(x => x.CategoriesId == id);
            if (prod == null)
            {
                return NotFound();
            }
            db.Categories.Remove(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }
    }
}
