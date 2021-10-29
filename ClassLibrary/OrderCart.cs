using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class OrderCart
    {
        public List<FoodItem> items { get; set; }

        public OrderCart()
        {
            items = new List<FoodItem>();
        }

        public OrderCart(FoodItem foodItem) : this()
        {
            this.AddItem(foodItem);
        }

        public OrderCart AddItem(FoodItem foodItem)
        {
            FoodItem existingFoodItem = this.items.FirstOrDefault(item => item.Name == foodItem.Name);
            if (existingFoodItem == null)
            {
                this.items.Add(foodItem);
            }

            return this;
        }

        public OrderCart RemoveItem(string foodItemName)
        {
            FoodItem existingFoodItem = this.items.FirstOrDefault(item => item.Name == foodItemName);
            if (existingFoodItem != null)
            {
                this.items.Remove(existingFoodItem);
            }

            return this;
        }

        public IEnumerable<FoodItem> GetItems()
        {
            return this.items.AsEnumerable<FoodItem>();
        }
    }
}
