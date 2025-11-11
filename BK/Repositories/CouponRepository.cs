using BK.Models;

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
            return _context.Coupons.ToList();
        }

        public Coupon GetById(int id)
        {
            var coupon = _context.Coupons.FirstOrDefault(x => x.Id == id);
            return coupon;
        }

        public Coupon Update(Coupon entity)
        {
            _context.Coupons.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
