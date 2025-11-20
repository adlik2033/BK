using BK.Models;
using Microsoft.EntityFrameworkCore;

namespace BK.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly BKDbContext _context;

        public ItemRepository(BKDbContext context)
        {
            _context = context;
        }

        public Item Create(Item entity)
        {
            if (entity.CategoryId > 0 && entity.Category == null)
            {
                entity.Category = _context.Categories.Find(entity.CategoryId);
            }

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
            return _context.Items
                .Include(i => i.Category)
                .Where(i => i.IsActive)
                .ToList();
        }

        public Item GetById(int id)
        {
            return _context.Items
                .Include(i => i.Category)
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Item> GetByCategoryId(int categoryId)
        {
            return _context.Items
                .Include(i => i.Category)
                .Where(x => x.CategoryId == categoryId && x.IsActive)
                .ToList();
        }

        public Item Update(Item entity)
        {
            if (entity.CategoryId > 0 && entity.Category == null)
            {
                entity.Category = _context.Categories.Find(entity.CategoryId);
            }

            _context.Items.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}