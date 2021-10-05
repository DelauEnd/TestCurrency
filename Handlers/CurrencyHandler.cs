using Microsoft.Extensions.Configuration;
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
            if (apiKey == "" || apiKey == null)
                return null;
            RequestHandler rh = new RequestHandler();
            return await Task.Run(() => rh.GetCurrencyList((apiKey)));
        }

        public static async Task<ConvertedProducts> AsyncConvertPrice(Products prod, string from, string to, string apiKey)
        {
            if (apiKey == "" || apiKey == null)
                return null;
            RequestHandler rh = new RequestHandler();
            float multiplier = await Task.Run(() => rh.GetCurrencyValue(from,to,apiKey));
            if (multiplier == 0)
                return null;
            return new ConvertedProducts 
            {
                ProductsId = prod.ProductsId,
                Title = prod.Title,
                BaseCurrency = from,
                Cost = prod.Cost,
                Currency = to,
                ConvertedCost = prod.Cost * multiplier                  
            };
        }

        public static string GetCurrencyApiKey(IConfiguration config)
        {
            return config["CurrencyApi:ApiKey"];
        }
    }
}
