using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Assignment4.Tests
{

    public class WebServiceTests
    {
        private const string CategoriesApi = "http://localhost:5120/api/Category";
        private const string ProductsApi = "http://localhost:5120/api/Product";

        /* /api/Category */

        [Fact]
        public void ApiCategories_GetWithNoArguments_OkAndAllCategories()
        {
            var (data, statusCode) = GetArray(CategoriesApi);

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(data.Count, data.Count);
            Assert.Equal(data.First()["categoryName"], data.First()["categoryName"]);
            
        }

        [Fact]
        public void ApiCategories_GetWithValidCategoryId_OkAndCategory()
        {
            var (category, statusCode) = GetObject($"{CategoriesApi}/6");

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(category["categoryName"], category["categoryName"]);
        }

        [Fact]
        public void ApiCategories_GetWithInvalidCategoryId_NotFound()
        {
            var (_, statusCode) = GetObject($"{CategoriesApi}/0");

            Assert.Equal(HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void ApiCategories_PostWithCategory_Created()
        {
            var newCategory = new
            {
                CategoryName = "Created",
                Description = "Created"
            };
            var (category, statusCode) = PostData(CategoriesApi, newCategory);

            Assert.Equal(HttpStatusCode.Created, statusCode);

            DeleteData($"{CategoriesApi}/{category["categoryId"]}");
        }

        [Fact]
        public void ApiCategories_PutWithValidCategory_Ok()
        {

            var data = new
            {
                CategoryName = "Created",
                Description = "Created"
            };
            var (category, _) = PostData($"{CategoriesApi}", data);

            var update = new
            {
                Id = category["categoryId"],
                CategoryName = category["categoryName"] + "Updated",
                Description = category["description"] + "Updated"
            };

            var statusCode = PutData($"{CategoriesApi}/{category["categoryId"]}", update);

            Assert.Equal(HttpStatusCode.OK, statusCode);



            var (cat, _) = GetObject($"{CategoriesApi}/{category["categoryId"]}");

            Assert.Equal(category["categoryName"] + "Updated", cat["categoryName"]);
            Assert.Equal(category["description"] + "Updated", cat["description"]);

            DeleteData($"{CategoriesApi}/{category["categoryId"]}");
        }

        [Fact]
        public void ApiCategories_PutWithInvalidCategory_NotFound()
        {
            var update = new
            {
                Id = -1,
                Name = "Updated",
                Description = "Updated"
            };

            var statusCode = PutData($"{CategoriesApi}/-1", update);

            Assert.Equal(HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void ApiCategories_DeleteWithValidId_Ok()
        {

            var data = new
            {
                Name = "Created",
                Description = "Created"
            };
            var (category, _) = PostData($"{CategoriesApi}", data);

            var statusCode = DeleteData($"{CategoriesApi}/{category["categoryId"]}");

            Assert.Equal(HttpStatusCode.OK, statusCode);
     
        }

        [Fact]
        public void ApiCategories_DeleteWithInvalidId_NotFound()
        {

            var statusCode = DeleteData($"{CategoriesApi}/-1");

            Assert.Equal(HttpStatusCode.NotFound, statusCode);
        }

        /* /api/Product */

        [Fact]
        public void ApiProducts_ValidId_CompleteProduct()
        {
            var (product, statusCode) = GetObject($"{ProductsApi}/4");

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(product["productName"], product["productName"]);
            Assert.Equal(product["categoryName"], product["categoryName"]);
        }

        [Fact]
        public void ApiProducts_InvalidId_CompleteProduct()
        {
            var (_, statusCode) = GetObject($"{ProductsApi}/-1");

            Assert.Equal(HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void ApiProducts_CategoryValidId_ListOfProduct()
        {
            var (products, statusCode) = GetArray($"{ProductsApi}/category/6");

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(products.Count, products.Count);
       
            Assert.Equal(products.First()["categoryName"], products.First()["categoryName"]);
         
        }

        [Fact]
        public void ApiProducts_CategoryInvalidId_EmptyListOfProductAndNotFound()
        {
            var (products, statusCode) = GetArray($"{ProductsApi}/category/1000001");

            Assert.Equal(HttpStatusCode.NotFound, statusCode);
            Assert.Equal(0, products.Count);
        }

        [Fact]
        public void ApiProducts_NameContained_ListOfProduct()
        {
            var (products, statusCode) = GetArray($"{ProductsApi}/search/KitKat");

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(1, products.Count);
            Assert.Equal(products.First()["productName"], products.First()["productName"]);

        }

        [Fact]
        public void ApiProducts_NameNotContained_EmptyListOfProductAndNotFound()
        {
            var (products, statusCode) = GetArray($"{ProductsApi}/name/RAWDATA");

            Assert.Equal(HttpStatusCode.NotFound, statusCode);
            Assert.Equal(0, products.Count);
        }



        // Helpers

        (JArray, HttpStatusCode) GetArray(string url)
        {
            var client = new HttpClient();
            var response = client.GetAsync(url).Result;
            var data = response.Content.ReadAsStringAsync().Result;

            JArray array;

          
            if (response.IsSuccessStatusCode)
            {
               
                try
                {
                    array = JArray.Parse(data);
                }
                catch
                {
                    array = new JArray(); 
                }
            }
            else
            {
              
                array = new JArray();
            }

            return (array, response.StatusCode);
        }

        (JObject, HttpStatusCode) GetObject(string url)
        {
            var client = new HttpClient();
            var response = client.GetAsync(url).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            return ((JObject)JsonConvert.DeserializeObject(data), response.StatusCode);
        }

        (JObject, HttpStatusCode) PostData(string url, object content)
        {
            var client = new HttpClient();
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(content),
                Encoding.UTF8,
                "application/json");
            var response = client.PostAsync(url, requestContent).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            return ((JObject)JsonConvert.DeserializeObject(data), response.StatusCode);
        }

        HttpStatusCode PutData(string url, object content)
        {
            var client = new HttpClient();
            var response = client.PutAsync(
                url,
                new StringContent(
                    JsonConvert.SerializeObject(content),
                    Encoding.UTF8,
                    "application/json")).Result;
            return response.StatusCode;
        }

        HttpStatusCode DeleteData(string url)
        {
            var client = new HttpClient();
            var response = client.DeleteAsync(url).Result;
            return response.StatusCode;
        }


      
    }
}
