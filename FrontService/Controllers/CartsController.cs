using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;
using System.Fabric.Query;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ClassLibrary;
using Microsoft.AspNetCore.Http;

namespace FrontService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CartsController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;
        private readonly FrontService frontService;

        public static Guid Guid = new Guid();

        public CartsController(HttpClient httpClient, FabricClient fabricClient, StatelessServiceContext statelessServiceContext, FrontService frontService)
        {
            this.fabricClient = fabricClient;
            this.httpClient = httpClient;
            this.serviceContext = statelessServiceContext;
            this.frontService = frontService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            /*string cookie;
            uint cartId;
            uint partitionId;
            Partition partition;
            Uri serviceName = FrontService.GetOrderCartServiceName(this.serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);
            ServicePartitionList partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            if(partitions.Count == 0) 
            {
                return this.Json(new OrderCart());
            }

            cookie = this.getCookie(Request);
            if (cookie == null)
            {
                cartId = this.generateNewCartId();
                partitionId = cartId % (uint)partitions.Count;
                cookie = cartId + "_" + partitionId;
                Response.Cookies.Append(
                    "id",
                    cookie,
                    new CookieOptions
                    {
                        Path = "localhost",
                        IsEssential = true,
                        Expires = DateTime.Now.AddDays(7)
                    });
            }
            else
            {
                cartId = uint.Parse(cookie.Split("_")[0]);
                partitionId = uint.Parse(cookie.Split("_")[1]);
            }

            partition = partitions[(int)partitionId];
            string proxyUrl = $"{proxyAddress}/api/OrderCarts/{cartId}?PartitionKey={((NamedPartitionInformation)partition.PartitionInformation)}&PartitionKind=NamedPartition";*/
            string proxyUrl = await this.GetProxyUrl(this.fabricClient);
            if(proxyUrl == "")
            {
                return this.Json(new OrderCart());
            }

            using (HttpResponseMessage response = await this.httpClient.GetAsync(proxyUrl))
            {
                this.increaseRequestsMetric();

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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FoodItem item)
        {
            string proxyUrl = await this.GetProxyUrl(this.fabricClient);
            if (proxyUrl == "")
            {
                return new BadRequestResult();
            }

            this.increaseRequestsMetric();
            StringContent postContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await this.httpClient.PostAsync(proxyUrl, postContent);

            return new OkResult();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            string proxyUrl = await this.GetProxyUrl(this.fabricClient, name);
            if (proxyUrl == "")
            {
                return new BadRequestResult();
            }

            this.increaseRequestsMetric();
            HttpResponseMessage response = await this.httpClient.DeleteAsync(proxyUrl);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return this.StatusCode((int)response.StatusCode);
            return new OkResult();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            string proxyUrl = await this.GetProxyUrl(this.fabricClient);
            if (proxyUrl == "")
            {
                return new BadRequestResult();
            }

            this.increaseRequestsMetric();
            HttpResponseMessage response = await this.httpClient.DeleteAsync(proxyUrl);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return this.StatusCode((int)response.StatusCode);
            return new OkResult();
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }

        private async Task<string> GetProxyUrl(FabricClient fabricClient)
        {
            string cookie;
            uint cartId;
            uint partitionId;
            Partition partition;
            Uri serviceName = FrontService.GetOrderCartServiceName(this.serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);
            ServicePartitionList partitions = await fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            if (partitions.Count == 0)
            {
                return "";
            }

            cookie = this.getCookie(Request);
            if (cookie == null)
            {
                cartId = this.generateNewCartId();
                partitionId = cartId % (uint)partitions.Count;
                cookie = cartId + "_" + partitionId;
                Response.Cookies.Append(
                    "id",
                    cookie,
                    new CookieOptions
                    {
                        Path = "localhost",
                        IsEssential = true,
                        Expires = DateTime.Now.AddDays(7)
                    });
            }
            else
            {
                cartId = uint.Parse(cookie.Split("_")[0]);
                partitionId = uint.Parse(cookie.Split("_")[1]);
            }

            partition = partitions[(int)partitionId];
            string proxyUrl = $"{proxyAddress}/api/OrderCarts/{cartId}?PartitionKey={partitionId}&PartitionKind=Named";

            return proxyUrl;
        }

        private async Task<string> GetProxyUrl(FabricClient fabricClient, string extraParameter)
        {
            string cookie;
            uint cartId;
            uint partitionId;
            Partition partition;
            Uri serviceName = FrontService.GetOrderCartServiceName(this.serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);
            ServicePartitionList partitions = await fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            if (partitions.Count == 0)
            {
                return "";
            }

            cookie = this.getCookie(Request);
            if (cookie == null)
            {
                cartId = this.generateNewCartId();
                partitionId = cartId % (uint)partitions.Count;
                cookie = cartId + "_" + partitionId;
                Response.Cookies.Append(
                    "id",
                    cookie,
                    new CookieOptions
                    {
                        Path = "localhost",
                        IsEssential = true,
                        Expires = DateTime.Now.AddDays(7)
                    });
            }
            else
            {
                cartId = uint.Parse(cookie.Split("_")[0]);
                partitionId = uint.Parse(cookie.Split("_")[1]);
            }

            partition = partitions[(int)partitionId];
            string proxyUrl = $"{proxyAddress}/api/OrderCarts/{cartId}/{extraParameter}?PartitionKey={partitionId}&PartitionKind=Named";

            return proxyUrl;
        }

        public IActionResult Index()
        {
            return View();
        }

        private uint generateNewCartId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
        }

        private string getCookie(HttpRequest request)
        {
            return Request.Cookies["id"];
        }

        private void increaseRequestsMetric()
        {
            FrontService.RequestsCount++;
            this.frontService.setRequestsCartsMetric(FrontService.RequestsCount);
        }
    }
}
