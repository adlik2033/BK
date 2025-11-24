using BK.Models;

namespace BK.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetUserOrders(int userId);
        IEnumerable<Order> GetOrdersByStatus(string status);
        Order CreateOrder(Order order, List<int> itemIds, List<int> couponIds);
    }
}