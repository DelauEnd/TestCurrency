using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCurrency.Authentication;
using TestCurrency.Data;
using TestCurrency.Handlers;

namespace TestCurrency.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : Controller//my apiKey: 549f67dd2bb6aa79160f
    {
        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<Currency>>> Get(string apiKey)
        {
            var curList = await CurrencyHandler.AsyncGetCurrencyList(apiKey);
            if (curList == null)
                return BadRequest();
            return await CurrencyHandler.AsyncGetCurrencyList(apiKey);
        }

        [Authorize(Roles = nameof(UserRoles.USER) + "," + nameof(UserRoles.ADMIN))]
        [HttpGet("{title}")]
        public async Task<ActionResult<Currency>> Get(string title,string apiKey)
        {
            var curList = await CurrencyHandler.AsyncGetCurrencyList(apiKey);
            if (curList == null)
                return BadRequest();
            Currency cur = curList.FirstOrDefault(x => x.id.ToLower() == title.ToLower());
            if (cur == null)
                return NotFound();
            return new ObjectResult(cur);
        }


    }
}
