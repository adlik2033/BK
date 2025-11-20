using Microsoft.AspNetCore.Mvc;
using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponController> _logger;

        public CouponController(ICouponRepository couponRepository, IMapper mapper, ILogger<CouponController> logger)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CouponDTO>> GetAllCoupons()
        {
            try
            {
                var coupons = _couponRepository.GetAll();
                var couponDTOs = _mapper.Map<IEnumerable<CouponDTO>>(coupons);
                return Ok(couponDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения купонов");
                return StatusCode(500, "Ошибка сервера");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CouponDTO> GetCouponById(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Купон с таким ID не существует!");
                }

                var coupon = _couponRepository.GetById(id);
                var couponDTO = _mapper.Map<CouponDTO>(coupon);
                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Такой купон не найден {id}", id);
                return StatusCode(500, "Ошибка сервера");
            }
        }

        [HttpGet("{id}/with-items")]
        public ActionResult<CouponWithItemsDTO> GetCouponWithItems(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound("Такой купон не найден");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                if (coupon == null)
                {
                    return NotFound("Такой купон не найден");
                }

                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);
                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения айтемов с купоном {id}", id);
                return StatusCode(500, "Ошибка сервера");
            }
        }
        [HttpPost]
        public ActionResult<CouponDTO> CreateCoupon([FromBody] CreateCouponDTO createCouponDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate business rules
                var validationResult = ValidateCoupon(createCouponDTO);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.ErrorMessage);
                }

                var coupon = _mapper.Map<Coupon>(createCouponDTO);
                coupon.IsActive = true;
                coupon.CreatedAt = DateTime.UtcNow;
                coupon.UpdatedAt = DateTime.UtcNow;
                coupon.items = new List<Item>(); 

                var createdCoupon = _couponRepository.Create(coupon);
                var couponDTO = _mapper.Map<CouponDTO>(createdCoupon);

                return CreatedAtAction(nameof(GetCouponById), new { id = couponDTO.Id }, couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания купона");
                return StatusCode(500, "Ошибка сервера");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<CouponDTO> UpdateCoupon(int id, [FromBody] UpdateCouponDTO updateCouponDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Купон с {id} не найден");
                }

                var existingCoupon = _couponRepository.GetById(id);
                if (existingCoupon == null)
                {
                    return NotFound($"Купон с {id} не найден");
                }

                var validationResult = ValidateCouponUpdate(updateCouponDTO, existingCoupon);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.ErrorMessage);
                }

                _mapper.Map(updateCouponDTO, existingCoupon);
                existingCoupon.UpdatedAt = DateTime.UtcNow;

                if (existingCoupon.UsageCount >= existingCoupon.UsageLimit)
                {
                    existingCoupon.IsActive = false;
                }

                var updatedCoupon = _couponRepository.Update(existingCoupon);
                var couponDTO = _mapper.Map<CouponDTO>(updatedCoupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления купона с {id}", id);
                return StatusCode(500, "Ошибка сервера");
            }
        }


        [HttpPost("{id}/items")]
        public ActionResult<CouponWithItemsDTO> AddItemsToCoupon(int id, [FromBody] AddItemsToCouponDTO addItemsDTO)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Купон с {id} не найден");
                }

                var result = _couponRepository.AddItemsToCoupon(id, addItemsDTO.ItemIds);
                if (!result)
                {
                    return BadRequest("Ошибка добавления купона");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения айтемов с купоном {id}", id);
                return StatusCode(500, "Ошибка сервера");
            }
        }

        [HttpDelete("{id}/items")]
        public ActionResult<CouponWithItemsDTO> RemoveAllItemsFromCoupon(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Купон с {id} не найден");
                }

                var result = _couponRepository.RemoveAllItemsFromCoupon(id);
                if (!result)
                {
                    return BadRequest("Ошибка удаления купона!");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления айтемов с купона {id}", id);
                return StatusCode(500, "Ошибка сервера");
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        public ActionResult<CouponWithItemsDTO> RemoveItemFromCoupon(int id, int itemId)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Такой купон не найден {id}");
                }

                var result = _couponRepository.RemoveItemFromCoupon(id, itemId);
                if (!result)
                {
                    return BadRequest("Ошибка удаления купона");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления айтемов с купона {CouponId}", id);
                return StatusCode(500, "Ошибка сервера");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteCoupon(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var result = _couponRepository.Delete(id);
                if (result)
                {
                    return NoContent();
                }

                return StatusCode(500, "Failed to delete coupon");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/deactivate")]
        public ActionResult<CouponDTO> DeactivateCoupon(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var coupon = _couponRepository.GetById(id);
                if (coupon == null)
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                coupon.IsActive = false;
                coupon.UpdatedAt = DateTime.UtcNow;

                var updatedCoupon = _couponRepository.Update(coupon);
                var couponDTO = _mapper.Map<CouponDTO>(updatedCoupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("active")]
        public ActionResult<IEnumerable<CouponDTO>> GetActiveCoupons()
        {
            try
            {
                var activeCoupons = _couponRepository.GetActiveCoupons();
                var couponDTOs = _mapper.Map<IEnumerable<CouponDTO>>(activeCoupons);
                return Ok(couponDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active coupons");
                return StatusCode(500, "Internal server error");
            }
        }

        private ValidationResult ValidateCoupon(CreateCouponDTO coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
            {
                return ValidationResult.Invalid("Code is required");
            }

            if (coupon.ValidUntil <= coupon.ValidFrom)
            {
                return ValidationResult.Invalid("ValidUntil must be after ValidFrom");
            }

            if (coupon.UsageLimit <= 0)
            {
                return ValidationResult.Invalid("UsageLimit must be greater than 0");
            }

            if (coupon.UsageCount < 0)
            {
                return ValidationResult.Invalid("UsageCount cannot be negative");
            }

            if (coupon.UsageCount > coupon.UsageLimit)
            {
                return ValidationResult.Invalid("UsageCount cannot exceed UsageLimit");
            }

            if (coupon.DiscountValue <= 0)
            {
                return ValidationResult.Invalid("DiscountValue must be positive");
            }

            var validDiscountTypes = new[] { "Percentage", "Fixed", "FreeShipping" };
            if (!validDiscountTypes.Contains(coupon.DiscountType))
            {
                return ValidationResult.Invalid($"DiscountType must be one of: {string.Join(", ", validDiscountTypes)}");
            }

            if (coupon.DiscountType == "Percentage" && coupon.DiscountValue > 100)
            {
                return ValidationResult.Invalid("Percentage discount cannot exceed 100%");
            }

            return ValidationResult.Valid();
        }

        private ValidationResult ValidateCouponUpdate(UpdateCouponDTO coupon, Coupon existingCoupon)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
            {
                return ValidationResult.Invalid("Code is required");
            }

            if (coupon.ValidUntil <= coupon.ValidFrom)
            {
                return ValidationResult.Invalid("ValidUntil must be after ValidFrom");
            }

            if (coupon.UsageCount < 0)
            {
                return ValidationResult.Invalid("UsageCount cannot be negative");
            }

            if (coupon.UsageCount > existingCoupon.UsageLimit)
            {
                return ValidationResult.Invalid("UsageCount cannot exceed UsageLimit");
            }

            if (coupon.DiscountValue <= 0)
            {
                return ValidationResult.Invalid("DiscountValue must be positive");
            }

            var validDiscountTypes = new[] { "Percentage", "Fixed", "FreeShipping" };
            if (!validDiscountTypes.Contains(coupon.DiscountType))
            {
                return ValidationResult.Invalid($"DiscountType must be one of: {string.Join(", ", validDiscountTypes)}");
            }

            if (coupon.DiscountType == "Percentage" && coupon.DiscountValue > 100)
            {
                return ValidationResult.Invalid("Percentage discount cannot exceed 100%");
            }

            return ValidationResult.Valid();
        }

        // ... остальные методы
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static ValidationResult Valid() => new ValidationResult { IsValid = true };
        public static ValidationResult Invalid(string message) => new ValidationResult { IsValid = false, ErrorMessage = message };
    }
}