using System;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Text;
using NPOI.SS.Util;

namespace saigonbook
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://saigonbooks.com.vn/shop?nph=966&fbclid=IwAR3MFZlSoE02Fq2WdWoWRn2Mn7Mdb7DEa_LjAM_vT-2LoxATUbaEJbLGGWw";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc = web.Load(url);

            var ProductHtml = doc.DocumentNode.Descendants("ul").Where(node => node.GetAttributeValue("class", "").Equals("products row")).ToList();

            var  ProductItems = ProductHtml[0].Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("inner")).ToList();
            foreach (var node in ProductItems)
            {
                /////Book Names
                var bookName = node.Descendants("div").Where(n => n.GetAttributeValue("class", "").Contains("product-name")).FirstOrDefault().InnerText.Trim('\n', '\t', '\r', ' ').ToString();
                Console.WriteLine($"Book Name : {bookName}");

                ///Images
                var img = node.Descendants("img").Where(n => n.GetAttributeValue("class", "").Contains("img img-responsive")).FirstOrDefault().Attributes["src"].Value.ToString();
                Console.WriteLine($"Image : https://saigonbooks.com.vn{img}");

                //CurrencyPrice & CurrencyValue
                var currencyPrice = node.Descendants("span").Where(n => n.GetAttributeValue("class", "").Contains("oe_currency_value")).FirstOrDefault().InnerText.Trim('\n', '\t', '\r', ' ').ToString();
                Console.WriteLine($"Currency Price :{currencyPrice}");

                //Json convert
                List<Book> book = new List<Book>();
                string result1 = bookName.Substring(0);
                var books = JsonConvert.DeserializeObject<Book>(result1);

                XSSFWorkbook wb = new XSSFWorkbook();
                //Create Sheet
                ISheet sheet = wb.CreateSheet();
                //Create Row
                var row0 = sheet.CreateRow(0);
                row0.CreateCell(0); // tạo ra cell trc khi merge
                CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
                sheet.AddMergedRegion(cellMerge);
                row0.GetCell(0).SetCellValue("Book data");
                //Ghi row
                var row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("Name");
                row1.CreateCell(1).SetCellValue("Img(Url)");
                row1.CreateCell(2).SetCellValue("Price");

                // bắt đầu duyệt mảng và ghi tiếp tục
                int rowIndex = 2;
                foreach (var item in book)
                {
                    // tao row mới
                    var newRow = sheet.CreateRow(rowIndex);

                    // set giá trị
                    newRow.CreateCell(0).SetCellValue(item.Name);
                    newRow.CreateCell(1).SetCellValue(item.Src);
                    newRow.CreateCell(2).SetCellValue(item.Price);

                    // tăng index
                    rowIndex++;
                }
                // xong hết thì save file lại
                FileStream fs = new FileStream(@"C:\Users\Minh\Desktop\CaoDB", FileMode.CreateNew);
                wb.Write(fs);
            }
            
            
        }
        public class Book 
        {
            public string Name { get; set; }
            public string Src { get; set; }
            public double Price { get; set; }
        }
    }
}
