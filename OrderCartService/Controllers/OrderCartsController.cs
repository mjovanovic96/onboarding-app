using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading.Tasks;
using ClassLibrary;

namespace OrderCartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderCartsController : Controller
    {
        private readonly IReliableStateManager StateManager;
        private readonly OrderCartService OrderCartService;
        const string OrderCart = "orderCart";

        public OrderCartsController(IReliableStateManager stateManager, OrderCartService orderCartService)
        {
            this.StateManager = stateManager;
            this.OrderCartService = orderCartService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return this.Json(await this.getCart(id));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, [FromBody] FoodItem foodItem)
        {
            await this.addToCart(id, foodItem);

            return this.Json(await this.getCart(id));
        }            

        [HttpDelete("{id}/{name}")]
        public async Task<IActionResult> Delete(string id, string name)
        {
            await this.removeFromCart(id, name);

            return this.Json(await this.getCart(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            IReliableDictionary<string, OrderCart> orderCartDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);
            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                await orderCartDictionary.TryRemoveAsync(transaction, id);
                int cartsCount = (int)(await orderCartDictionary.GetCountAsync(transaction));
                this.OrderCartService.setCountCartsMetric(cartsCount);
                await transaction.CommitAsync();
                return this.Json(orderCartDictionary);
            }
        }

        [HttpGet("NumberOfCarts")]
        public async Task<IActionResult> GetNumberOfCarts()
        {
            IReliableDictionary<string, OrderCart> orderCartDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);
            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                int cartsCount = (int)(await orderCartDictionary.GetCountAsync(transaction));
                return this.Json(cartsCount);
            }
        }

        private async Task<OrderCart> getCart(string id)
        {
            IReliableDictionary<string, OrderCart> orderCartDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);

            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                int cartsCount = 0;
                var result = await orderCartDictionary.GetOrAddAsync(transaction, id, new OrderCart());

                cartsCount = (int)(await orderCartDictionary.GetCountAsync(transaction));
                this.OrderCartService.setCountCartsMetric(cartsCount);

                await transaction.CommitAsync();

                return result;
            }
        }


        private async Task addToCart(string id, FoodItem foodItem)
        {
            IReliableDictionary<string, OrderCart> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);

            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                await foodDictionary.AddOrUpdateAsync(transaction, id, new OrderCart(foodItem), (key, value) => value.AddItem(foodItem));
                await transaction.CommitAsync();
            }
        }

        private async Task removeFromCart(string id, string name)
        {
            IReliableDictionary<string, OrderCart> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);
            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                OrderCart cart = await this.getCart(id);
                await foodDictionary.SetAsync(transaction, id, cart.RemoveItem(name));
                await transaction.CommitAsync();
            }
        }
    }
}
