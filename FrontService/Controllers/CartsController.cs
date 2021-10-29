using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.ServiceFabric.Services.Client;
using ClassLibrary;

namespace FrontService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CartsController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;

        public CartsController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext statelessServiceContext)
        {
            this.fabricClient = fabricClient;
            this.httpClient = httpClient;
            this.serviceContext = statelessServiceContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Uri serviceName = FrontService.GetOrderCartServiceName(this.serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);
            ServicePartitionList partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            foreach (Partition partition in partitions)
            {
                string proxyUrl =
                    $"{proxyAddress}/api/OrderCarts?PartitionKey={((Int64RangePartitionInformation)partition.PartitionInformation).LowKey}&PartitionKind=Int64Range";

                using (HttpResponseMessage response = await this.httpClient.GetAsync(proxyUrl))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return this.StatusCode((int)response.StatusCode);
                    }

                    HttpContent content = response.Content;
                    string result = content.ReadAsStringAsync().Result;
                    OrderCart orderCart = JsonConvert.DeserializeObject<OrderCart>(result);
                    return this.Json(orderCart);
                }
            }

            return this.Json(new OrderCart());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FoodItem item)
        {
            StringContent postContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await this.httpClient.PostAsync(await this.ResolveOrderCartAddress() + "/api/OrderCarts", postContent);
            return new OkResult();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            HttpResponseMessage response = await this.httpClient.DeleteAsync(await this.ResolveOrderCartAddress() + "/api/OrderCarts/" + name);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return this.StatusCode((int)response.StatusCode);
            return new OkResult();
        }

        private async Task<string> ResolveOrderCartAddress()
        {
            var partitionResolver = ServicePartitionResolver.GetDefault();
            var resolveResults = await partitionResolver.ResolveAsync(
                                                            new Uri("fabric:/FoodOrderApp/OrderCartService"), 
                                                            new ServicePartitionKey(1), 
                                                            new System.Threading.CancellationToken());
            var endpoint = resolveResults.GetEndpoint();
            var endpointObject = JsonConvert.DeserializeObject<JObject>(endpoint.Address);
            var addressString = ((JObject)endpointObject.Property("Endpoints").Value)[""].Value<string>();

            return addressString;
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }

        private long GetPartitionKey(string name)
        {
            return Char.ToUpper(name.First()) - 'A';
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
