using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCurrency.Authentication;
using TestCurrency.Data;
using TestCurrency.Handlers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestCurrency.Controllers
{
    [Authorize]
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

        [Authorize(Roles = UserRoles.user + "," + UserRoles.admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> Get()
        {
            return await db.Products.Include(x=>x.Category).ToListAsync();
        }

        [Authorize(Roles = UserRoles.user + "," + UserRoles.admin)]
        [HttpGet]
        [Route("search/{title}")]
        public async Task<ActionResult<IEnumerable<Products>>> Search(string title)
        {
            var prodList = await db.Products.Include(x => x.Category).Where(x => x.Title.ToLower().Contains(title.ToLower())).ToListAsync();
            if (prodList.Count == 0)
                return NotFound();
            return prodList;
        }

        [Authorize(Roles = UserRoles.user + "," + UserRoles.admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> Get(int id)
        {
            Products prod = await db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductsId == id);
            if (prod == null)
                return NotFound();
            return new ObjectResult(prod);
        }

        [Authorize(Roles = UserRoles.user + "," + UserRoles.admin)]
        [HttpGet]
        [Route("{id}/convert/{currency}")]//my apiKey: 549f67dd2bb6aa79160f
        public async Task<ActionResult<ConvertedProducts>> convert(int id, string currency, string apiKey)
        {
            const string oldCurrency = "BYN";

            Products prod = await db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductsId == id);
            if (prod == null)
                return NotFound();

            var convProd = await CurrencyHandler.AsyncConvertPrice(prod, oldCurrency, currency, apiKey);
            if (convProd == null)
                return BadRequest();
            return convProd;
        }

        [Authorize(Roles = UserRoles.admin)]
        [HttpPost]
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

        [Authorize(Roles = UserRoles.admin)]
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

        [Authorize(Roles = UserRoles.admin)]
        [HttpPatch]
        [Route("priceupdate/{title}")]
        public async Task<ActionResult<Categories>> Patch(int id, float Cost)
        {
            Products prod = await db.Products.FirstOrDefaultAsync(x => x.ProductsId == id);
            if (prod == null)
            {
                return NotFound();
            }

            prod.Cost = Cost;

            db.Update(prod);
            await db.SaveChangesAsync();
            return Ok(prod);
        }

        [Authorize(Roles = UserRoles.admin)]
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
