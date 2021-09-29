using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCurrency.Data
{
    //Класс для получения ответа со списком валют
    public class Currency
    {
        public string currencyName { get; set; }
        public string currencySymbol { get; set; }
        public string id { get; set; }
    }

    #region Классы для сериализации ответа от валютного апи(курсы валют)
    public class Root
    {
        public Results results { get; set; }
    }

    public class Results
    {
        public Converted cur { get; set; }
    }

    public class Converted
    {
        public float val { get; set; }
        public string to { get; set; }
        public string fr { get; set; }
    }
    #endregion
}
