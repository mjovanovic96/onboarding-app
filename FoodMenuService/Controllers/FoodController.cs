using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ClassLibrary;

namespace FoodMenuService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : Controller
    {
        private readonly IReliableStateManager StateManager;
        const string FoodMenu = "foodMenu";

        public FoodController(IReliableStateManager stateManager)
        {
            this.StateManager = stateManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return this.Json(await this.getMenu());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FoodItem item)
        {
            await this.addToMenu(item);

            return this.Json(await this.getMenu());
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            await this.removeFromMenu(name);

            return this.Json(await this.getMenu());
        }

        private async Task<FoodMenu> getMenu()
        {
            IReliableDictionary<string, FoodMenu> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, FoodMenu>>(FoodMenu);

            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                var result = await foodDictionary.TryGetValueAsync(transaction, FoodMenu);
                if (result.HasValue) return result.Value;
                else return new FoodMenu();
            }
        }
        private async Task addToMenu(FoodItem item)
        {
            IReliableDictionary<string, FoodMenu> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, FoodMenu>>(FoodMenu);
            
            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                await foodDictionary.AddOrUpdateAsync(transaction, FoodMenu, new FoodMenu(item), (key, value) => value.AddItem(item));
                await transaction.CommitAsync();
            }
        }
        private async Task removeFromMenu(string name)
        {
            IReliableDictionary<string, FoodMenu> foodDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, FoodMenu>>(FoodMenu);
            using (ITransaction transaction = this.StateManager.CreateTransaction())
            {
                FoodMenu menu = await this.getMenu();
                await foodDictionary.SetAsync(transaction, FoodMenu, menu.RemoveItem(name));               
                await transaction.CommitAsync();
            }
        }
    }
}
