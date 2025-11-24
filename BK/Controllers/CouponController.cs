using Microsoft.AspNetCore.Mvc;
using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using AutoMapper;
using BK.Attributes;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponController(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CouponUserDTO>>> GetAllCoupons()
        {
            try
            {
                var coupons = _couponRepository.GetActiveCoupons();
                var couponDTOs = _mapper.Map<IEnumerable<CouponUserDTO>>(coupons);
                return Ok(new ApiResponse<IEnumerable<CouponUserDTO>>
                {
                    Success = true,
                    Message = "Купоны успешно получены",
                    Data = couponDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<CouponUserDTO>> GetCouponById(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                var coupon = _couponRepository.GetById(id);
                if (coupon == null || !coupon.IsActive)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                var couponDTO = _mapper.Map<CouponUserDTO>(coupon);
                return Ok(new ApiResponse<CouponUserDTO>
                {
                    Success = true,
                    Message = "Купон успешно получен",
                    Data = couponDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }
    }
}