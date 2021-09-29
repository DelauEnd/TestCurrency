using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCurrency.Data;

namespace TestCurrency.Handlers
{
    public class CurrencyHandler
    {
        public static async Task<List<Currency>> AsyncGetCurrencyList(string apiKey)
        {
            RequestHandler rh = new RequestHandler();
            return await Task.Run(() => rh.GetCurrencyList((apiKey)));
        }

        public static async Task<ConvertedProducts> AsyncConvertPrice(Products prod, string from, string to, string apiKey)
        {
            RequestHandler rh = new RequestHandler();
            float multiplier = await Task.Run(() => rh.GetCurrencyValue(from,to,apiKey));
            if (multiplier == 0)
                return null;
            return new ConvertedProducts 
            {
                ProductsId = prod.ProductsId,
                Title = prod.Title,
                OldCurrency = from,
                Cost = prod.Cost,
                Currency = to,
                ConvertedCost = prod.Cost * multiplier                  
            };
        }
    }
}
