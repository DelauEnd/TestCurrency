using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCurrency.Data;
using TestCurrency.Handlers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestCurrency.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : Controller//my apiKey: 549f67dd2bb6aa79160f
    {
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<Currency>>> Get(string apiKey)
        {
            var curList = await CurrencyHandler.AsyncGetCurrencyList(apiKey);
            if (curList == null)
                return BadRequest();
            return await CurrencyHandler.AsyncGetCurrencyList(apiKey);
        }

        [HttpGet("{title}")]
        public async Task<ActionResult<Currency>> Get(string title,string apiKey)
        {
            var curList = await CurrencyHandler.AsyncGetCurrencyList(apiKey);
            if (curList == null)
                return BadRequest();
            Currency cur = curList.FirstOrDefault(x => x.id == title);
            if (cur == null)
                return NotFound();
            return new ObjectResult(cur);
        }


    }
}
