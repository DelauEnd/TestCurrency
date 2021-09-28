using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCurrency.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestCurrency.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        ProductsDBContext db;
        public ProductsController(ProductsDBContext context)
        {
            db = context;
            if (!db.Products.Any())
            {
                var cat = new Categories { Title = "Кат1" };
                db.Categories.Add(cat);
                db.SaveChanges();
                db.Products.Add(new Products { Title = "Товар1", Cost = 0.99f, CategoryId=1});
                db.SaveChanges();
            } 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> Get()
        {
            return await db.Products.Include(x=>x.Category).ToListAsync();
        }
        
        [HttpGet("{id}")] // GET api/prod/
        public async Task<ActionResult<Products>> Get(int id)
        {
            Products prod = await db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductsId == id);
            if (prod == null)
                return NotFound();
            return new ObjectResult(prod);
        }

        
        [HttpPost] // POST api/prod/
        public async Task<ActionResult<Products>> Post(Products prod)
        {
            if (prod == null)
            {
                return BadRequest();
            }

            db.Products.Add(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }

        // PUT api/prod/
        [HttpPut]
        public async Task<ActionResult<Products>> Put(Products prod)
        {
            if (prod == null)
            {
                return BadRequest();
            }
            if (!db.Products.Any(x => x.ProductsId == prod.ProductsId))
            {
                return NotFound();
            }

            db.Update(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> Delete(int id)
        {
            Products prod = db.Products.FirstOrDefault(x => x.ProductsId == id);
            if (prod == null)
            {
                return NotFound();
            }
            db.Products.Remove(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }
    }
}
