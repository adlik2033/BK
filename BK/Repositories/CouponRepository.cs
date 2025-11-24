using BK.Models;
using Microsoft.EntityFrameworkCore;

namespace BK.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly BKDbContext _context;
        public CouponRepository(BKDbContext bKDbContext)
        {
            _context = bKDbContext;
        }
        public Coupon Create(Coupon entity)
        {
            _context.Coupons.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            var coupon = GetById(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            return _context.Coupons.Any(x => x.Id == id);
        }

        public IEnumerable<Coupon> GetAll()
        {
            return _context.Coupons
                .Include(c => c.Items.Where(i => i.IsActive))
                .ToList();
        }

        public Coupon GetById(int id)
        {
            var coupon = _context.Coupons
                .Include(c => c.Items.Where(i => i.IsActive))
                .FirstOrDefault(x => x.Id == id);
            return coupon;
        }

        public Coupon Update(Coupon entity)
        {
            _context.Coupons.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public Coupon GetByCode(string code)
        {
            return _context.Coupons.FirstOrDefault(x => x.Code == code);
        }

        public IEnumerable<Coupon> GetActiveCoupons()
        {
            var now = DateTime.UtcNow;
            return _context.Coupons
                .Include(c => c.Items.Where(i => i.IsActive))
                .Where(x => x.IsActive &&
                           x.ValidFrom <= now &&
                           x.ValidUntil >= now &&
                           x.UsageCount < x.UsageLimit)
                .ToList();
        }
        public Coupon GetByIdWithItems(int id)
        {
            return _context.Coupons
                .Include(c => c.Items.Where(i => i.IsActive))
                .FirstOrDefault(x => x.Id == id);
        }

        public bool AddItemsToCoupon(int couponId, List<int> itemIds)
        {
            try
            {
                var coupon = _context.Coupons
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.Id == couponId);

                if (coupon == null) return false;

                var itemsToAdd = _context.Items
                    .Where(i => itemIds.Contains(i.Id) && i.IsActive)
                    .ToList();

                foreach (var item in itemsToAdd)
                {
                    if (!coupon.Items.Any(i => i.Id == item.Id))
                    {
                        coupon.Items.Add(item);
                    }
                }

                coupon.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveAllItemsFromCoupon(int couponId)
        {
            try
            {
                var coupon = _context.Coupons
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.Id == couponId);

                if (coupon == null) return false;

                coupon.Items.Clear();
                coupon.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveItemFromCoupon(int couponId, int itemId)
        {
            try
            {
                var coupon = _context.Coupons
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.Id == couponId);

                if (coupon == null) return false;

                var itemToRemove = coupon.Items.FirstOrDefault(i => i.Id == itemId);
                if (itemToRemove != null)
                {
                    coupon.Items.Remove(itemToRemove);
                    coupon.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}