using BK.Models;
using Microsoft.EntityFrameworkCore;

namespace BK.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BKDbContext _context;
        public OrderRepository(BKDbContext bKDbContext)
        {
            _context = bKDbContext;
        }
        public Order Create(Order entity)
        {
            _context.Orders.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            var order = GetById(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            return _context.Orders.Any(x => x.Id == id);
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.Items.Where(i => i.IsActive))
                .Include(o => o.Coupons.Where(c => c.IsActive))
                .Include(o => o.User)
                .ThenInclude(u => u.Role)
                .ToList();
        }

        public Order GetById(int id)
        {
            var order = _context.Orders
                .Include(o => o.Items.Where(i => i.IsActive))
                .Include(o => o.Coupons.Where(c => c.IsActive))
                .Include(o => o.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefault(x => x.Id == id);
            return order;
        }

        public Order Update(Order entity)
        {
            _context.Orders.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<Order> GetUserOrders(int userId)
        {
            return _context.Orders
                .Include(o => o.Items.Where(i => i.IsActive))
                .Include(o => o.Coupons.Where(c => c.IsActive))
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public IEnumerable<Order> GetOrdersByStatus(string status)
        {
            return _context.Orders
                .Include(o => o.Items.Where(i => i.IsActive))
                .Include(o => o.Coupons.Where(c => c.IsActive))
                .Include(o => o.User)
                .ThenInclude(u => u.Role)
                .Where(o => o.Status == status)
                .ToList();
        }

        public Order CreateOrder(Order order, List<int> itemIds, List<int> couponIds)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var lastOrder = _context.Orders.OrderByDescending(o => o.OrderNumber).FirstOrDefault();
                order.OrderNumber = lastOrder?.OrderNumber + 1 ?? 1000;

                var items = _context.Items.Where(i => itemIds.Contains(i.Id) && i.IsActive).ToList();
                order.Items = items;

                var coupons = _context.Coupons.Where(c => couponIds.Contains(c.Id) && c.IsActive).ToList();
                order.Coupons = coupons;

                order.TotalAmount = items.Sum(i => i.Price) - coupons.Sum(c => c.DiscountValue);

                _context.Orders.Add(order);
                _context.SaveChanges();

                transaction.Commit();
                return order;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}