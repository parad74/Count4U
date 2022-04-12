using System.Collections.Generic;

namespace Count4U.Model.Services
{
    public class OrdersRepository : IOrdersRepository
    {
        public IList<Order> GetOrders()
        {
            return new List<Order>
                       {
                           new Order {Product = "Product1"},
                           new Order {Product = "Product2"},
                           new Order {Product = "Product3"}
                       };
        }
    }
}