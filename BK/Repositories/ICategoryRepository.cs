using BK.Models;

namespace BK.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category GetById(int id);
        Category Create(Category entity);
        Category Update(Category entity);
        bool Delete(int id);
        bool Exists(int id);
        IEnumerable<Category> GetActiveCategories();
    }
}