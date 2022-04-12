using System.Collections.Generic;

namespace Count4U.Model.Services
{
    public interface IOrdersRepository
    {
        IList<Order> GetOrders();
    }
}