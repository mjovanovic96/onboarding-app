using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary;
using System.Fabric;

namespace OrderCartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderCartsController : Controller
    {
        public static Guid Guid = new Guid();
        private readonly IReliableStateManager StateManager;
        private readonly OrderCartService OrderCartService;
        const string OrderCart = "orderCart";

        public OrderCartsController(IReliableStateManager stateManager, OrderCartService orderCartService)
        {
            this.StateManager = stateManager;
            this.OrderCartService = orderCartService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return this.Json(await this.getCart());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FoodItem foodItem)
        {
            await this.addToCart(foodItem);

            return this.Json(await this.getCart());
        }            

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            await this.removeFromCart(name);

            return this.Json(await this.getCart());
        }

        private async Task<OrderCart> getCart()
        {
            IReliableDictionary<string, OrderCart> orderCartDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);

            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                string cartId = this.getId(Request);
                int cartsCount = 0;
                if (cartId == null)
                {
                    cartId = this.generateNewCartId();
                }

                Response.Cookies.Append(
                    "id",
                    cartId,
                    new Microsoft.AspNetCore.Http.CookieOptions()
                    {
                        Path = "/",
                        Expires = DateTime.Now.AddDays(3)
                    });
                /*var result = await orderCartDictionary.TryGetValueAsync(transaction, cartId);
                if (result.HasValue) return result.Value;
                else
                {
                    newCart = new OrderCart();
                    await orderCartDictionary.AddAsync(transaction, cartId, newCart);
                    cartsCount++;
                    this.OrderCartService.setCountCartsMetric(cartsCount);
                    return newCart;
                }*/
                var result = await orderCartDictionary.GetOrAddAsync(transaction, cartId, new OrderCart());
                cartsCount = (int)(await orderCartDictionary.GetCountAsync(transaction));
                this.OrderCartService.setCountCartsMetric(cartsCount);
                await transaction.CommitAsync();
                return result;
            }
        }

        private string getId(HttpRequest request)
        {
            return Request.Cookies["id"];
        }

        private string generateNewCartId()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task addToCart(FoodItem foodItem)
        {
            IReliableDictionary<string, OrderCart> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);

            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                await foodDictionary.AddOrUpdateAsync(transaction, this.getId(Request), new OrderCart(foodItem), (key, value) => value.AddItem(foodItem));
                await transaction.CommitAsync();
            }
        }

        private async Task removeFromCart(string name)
        {
            IReliableDictionary<string, OrderCart> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, OrderCart>>(OrderCart);
            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                OrderCart cart = await this.getCart();
                await foodDictionary.SetAsync(transaction, this.getId(Request), cart.RemoveItem(name));
                await transaction.CommitAsync();
            }
        }
    }
}
