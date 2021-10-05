using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCurrency.Data;
using Microsoft.AspNetCore.Authorization;
using TestCurrency.Authentication;
using Microsoft.AspNetCore.Http;

namespace TestCurrency.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private ProductsDBContext db;

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
        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> Get()
        {
            if (!db.Categories.Any())
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Categories table is empty" });
            return await db.Categories.Include(x=>x.Products).ToListAsync();
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> Get(int id)
        {
            Categories categ = await db.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.CategoriesId == id);
            if (categ == null)
                return NotFound();
            return new ObjectResult(categ);
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet]
        [Route("search/{title}")]
        public async Task<ActionResult<IEnumerable<Categories>>> Search(string title)
        {
            var catList = await db.Categories.Where(x => x.Title.ToLower().Contains(title.ToLower())).ToListAsync();
            if (catList.Count == 0)
                return NotFound();
            return catList;
        }

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
        [HttpPost]
        public async Task<ActionResult<Categories>> Post(Categories categ)
        {
            if (categ == null)
            {
                return BadRequest();
            }

            db.Categories.Add(categ);
            await db.SaveChangesAsync();
            return Ok(categ);
        }

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
        [HttpPut]
        public async Task<ActionResult<Categories>> Put(Categories categ)
        {
            if (categ == null)
            {
                return BadRequest();
            }
            if (!db.Categories.Any(x => x.CategoriesId == categ.CategoriesId))
            {
                return NotFound();
            }

            await db.SaveChangesAsync();
            return Ok(categ);
        }

        [Authorize(Roles = nameof(UserRoles.ADMIN))]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Categories>> Delete(int id)
        {
            Categories categ = db.Categories.FirstOrDefault(x => x.CategoriesId == id);
            if (categ == null)
            {
                return NotFound();
            }
            db.Categories.Remove(categ);
            await db.SaveChangesAsync();
            return Ok(categ);
        }
    }
}
