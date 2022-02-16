using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
public class Program
{
    public static void Main()
    {
        var url = "https://concung.com/tim-sieu-thi.html";
        var web = new HtmlWeb();
        var doc = web.Load(url);
        var script = doc.DocumentNode.Descendants()
                             .FirstOrDefault(n => n.Id == "main-content").Descendants()
                             .FirstOrDefault(n => n.Name == "script").InnerText;
        string variableName = "var storeall= ";
        int pFrom = script.IndexOf(variableName) + variableName.Length;
        int pTo = script.IndexOf(";", pFrom);
        string result = script.Substring(pFrom, pTo - pFrom);
        var units = JsonConvert.DeserializeObject<ICollection<Unit>>(result);
        Console.WriteLine(result);

    }
    public class Unit
    {
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Name { get; set; }
    }
}