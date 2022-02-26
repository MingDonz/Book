using HtmlAgilityPack;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ITableEntity = Azure.Data.Tables.ITableEntity;
using static Program;
using TableEntity = Azure.Data.Tables.TableEntity;

namespace Crawl
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            var storageUri = "http://127.0.0.1:10002/devstoreaccount1";
            var accountName = "devstoreaccount1";
            var storageAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
            string tableName = "CuaHieu";
            var Unit = new List<Unit>();
            var url = "https://concung.com/tim-sieu-thi.html";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var script = htmlDoc.DocumentNode
                .Descendants().First(n => n.Id == "main-content")
                .Descendants().First(n => n.Name == "script").InnerText;
            string variName = "var storeall= ";
            int from = script.IndexOf(variName) + variName.Length;
            int to = script.IndexOf(";", from);
            string kq = script.Substring(from, to - from);
            var units = JsonConvert.DeserializeObject<ICollection<Unit>>(kq);
            ///------------------------------------/////
            foreach (var unit in units)
            {
                //Id Store
                var id = unit.id_store;
                Console.WriteLine($"Id store: {id}");
                //Address
                var address = unit.Address;
                Console.WriteLine($"Address: {address}");
                //District
                var district = unit.district_name;
                Console.WriteLine($"District: {district}");
                ////Name
                var name = unit.Name;
                Console.WriteLine($"Name: {name}");
                //Province
                var province = unit.province_name;
                Console.WriteLine($"Province Name: {province}");
                //Ward
                var ward = unit.ward_name;
                Console.WriteLine($"Ward Name: {ward}");
                //Latitude
                var latitude = unit.Latitude;
                Console.WriteLine($"Latitude: {latitude}");
                //Longitude
                var longitude = unit.Longitude;
                Console.WriteLine($"Longitude: {longitude} \n");


                Unit.Add(new Unit()
                {
                    Address = address,
                    Name = name,
                    Latitude = latitude,
                    Longitude = longitude,
                    district_name = district,
                    province_name = province,
                    ward_name = ward,
                    id_store = id
                });
            }
            if(Unit.Any())
            {
                var serviceClient = new TableServiceClient(
                 new Uri(storageUri),
                 new TableSharedKeyCredential(accountName, storageAccountKey));
                var tableClient = serviceClient.GetTableClient(tableName);
                TableItem table = serviceClient.CreateTableIfNotExists(tableName);

                var query = new TableQuery<Unit>();

                var entity = Unit.Select(x => new Unit
                {
                    PartitionKey = x.Name,
                    RowKey = Guid.NewGuid().ToString(),
                    id_store=x.id_store,
                    district_name = x.district_name,
                    province_name = x.province_name,
                    ward_name=x.ward_name,
                    Latitude=x.Latitude,
                    Longitude=x.Longitude
                });
                TableBatchOperation
            }
            //-----------------------------------------//
            
        }
       




        public class Unit : TableEntity
        {

            public string Address { get; set; }
            public string Name { get; set; }
            public string district_name { get; set; }
            public string province_name { get; set; }
            public string ward_name { get; set; }
            public double id_store { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }

            public Unit() { }

            public Unit(string address,string name,string distric, string province,string ward,double id,double longitude,double latitude)
            {
                Address = name;
                Name = address;
                district_name = distric;
                province_name = province;
                ward_name = ward;
                id_store = id;
                Longitude = longitude;
                Latitude = latitude;
            }

            
        }
    }
}
