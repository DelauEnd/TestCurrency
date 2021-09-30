using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TestCurrency.Data;

namespace TestCurrency.Handlers
{
    public class RequestHandler
    {
        WebClient web;
        public RequestHandler()
        {
            web = new WebClient();
            web.Encoding = Encoding.UTF8;
            web.UseDefaultCredentials = true;
            web.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 90.0.4430.41 YaBrowser / 21.5.0.579 Yowser / 2.5 Safari / 537.36");
        }

        public List<Currency> GetCurrencyList(string apiKey)
        {
            var jsonString = web.DownloadString($"https://free.currconv.com/api/v7/currencies?compact=ultra&apiKey={apiKey}");
            var data = JObject.Parse(jsonString)["results"].ToArray();
            return data.Select(item => item.First.ToObject<Currency>()).ToList();
        }

        public float GetCurrencyValue(string from, string to, string apiKey)
        {
            var jsonString = web.DownloadString($"https://free.currconv.com/api/v7/convert?compact=y&q={from}_{to}&apiKey={apiKey}");
            var res = JObject.Parse(jsonString).First;
            if (res == null)
                return 0;
            return (float)res.First["val"].ToObject<double>();
        }
    }
}
