using BK.Models;
using Microsoft.EntityFrameworkCore;

namespace BK.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly BKDbContext _context;
        public ItemRepository(BKDbContext bKDbContext)
        {
            _context = bKDbContext;
        }
        public Item Create(Item entity)
        {
            _context.Items.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            return _context.Items.Any(x => x.Id == id);
        }

        public IEnumerable<Item> GetAll()
        {
            return _context.Items.ToList();
        }

        public Item GetById(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            return item;
        }

        public Item Update(Item entity)
        {
            _context.Items.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
