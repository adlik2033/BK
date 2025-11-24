using BK.Models;
using Microsoft.EntityFrameworkCore;

namespace BK.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BKDbContext _context;

        public CategoryRepository(BKDbContext bKDbContext)
        {
            _context = bKDbContext;
        }

        public Category Create(Category entity)
        {
            _context.Categories.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            var category = GetById(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(x => x.Id == id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories
                .Include(c => c.Items.Where(i => i.IsActive))
                .ToList();
        }

        public Category GetById(int id)
        {
            return _context.Categories
                .Include(c => c.Items.Where(i => i.IsActive))
                .FirstOrDefault(x => x.Id == id);
        }

        public Category Update(Category entity)
        {
            _context.Categories.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<Category> GetActiveCategories()
        {
            return _context.Categories
                .Include(c => c.Items.Where(i => i.IsActive))
                .Where(c => c.IsActive)
                .ToList();
        }
    }
}