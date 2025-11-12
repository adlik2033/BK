using BK.Models;

namespace BK.Repositories
{
    public interface IItemRepository
    {
        IEnumerable<Item> GetAll();
        Item GetById(int id);
        IEnumerable<Item> GetByCategoryId(int categoryId);
        Item Create(Item entity);
        Item Update(Item entity);
        bool Delete(int id);
        bool Exists(int id);
    }
}