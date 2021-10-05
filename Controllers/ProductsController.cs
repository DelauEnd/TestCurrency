using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestCurrency.Authentication;
using TestCurrency.Data;
using TestCurrency.Handlers;

namespace TestCurrency.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private ProductsDBContext db;
        private readonly IConfiguration config;

        public ProductsController(ProductsDBContext context, IConfiguration config)
        {
            this.config = config;
            db = context;
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> Get()
        {
            if (!db.Products.Any())
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Products table is empty" });
            return await db.Products.Include(x=>x.Category).ToListAsync();
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet]
        [Route("search/{title}")]
        public async Task<ActionResult<IEnumerable<Products>>> Search(string title)
        {
            var prodList = await db.Products.Where(x => x.Title.ToLower().Contains(title.ToLower())).ToListAsync();
            if (prodList.Count == 0)
                return NotFound();
            return prodList;
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> Get(int id)
        {
            Products prod = await db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductsId == id);
            if (prod == null)
                return NotFound();
            return new ObjectResult(prod);
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet]
        [Route("{id}/convert/{currency}")]
        public async Task<ActionResult<ConvertedProducts>> convert(int id, string currency)
        {
            const string baseCurrency = "BYN";

            Products prod = await db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductsId == id);
            if (prod == null)
                return NotFound();
            
            string apiKey = CurrencyHandler.GetCurrencyApiKey(config);

            var convProd = await CurrencyHandler.AsyncConvertPrice(prod, baseCurrency, currency, apiKey);
            if (convProd == null)
                return BadRequest();
            return convProd;
        }

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
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

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
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

            var newProd = db.Products.FirstOrDefault(x=>x.ProductsId == prod.ProductsId);
            newProd.CategoryId = prod.CategoryId;
            newProd.Title = prod.Title;
            newProd.Cost = prod.Cost;

            await db.SaveChangesAsync();
            return Ok(prod);
        }

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
        [HttpPatch]
        [Route("priceupdate/{id}")]
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

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
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
