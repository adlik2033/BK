using BK.Models;

namespace BK.Repositories
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Coupon GetByCode(string code);
        IEnumerable<Coupon> GetActiveCoupons();
        Coupon GetByIdWithItems(int id);
        bool AddItemsToCoupon(int couponId, List<int> itemIds);
        bool RemoveAllItemsFromCoupon(int couponId);
        bool RemoveItemFromCoupon(int couponId, int itemId);
    }
}