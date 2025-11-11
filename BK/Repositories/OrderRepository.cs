using BK.Models;

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
            return _context.Orders.ToList();
        }

        public Order GetById(int id)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == id);
            return order;
        }

        public Order Update(Order entity)
        {
            _context.Orders.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
