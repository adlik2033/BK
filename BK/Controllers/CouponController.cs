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

        // GET: api/coupon
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
                _logger.LogError(ex, "Error occurred while getting all coupons");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/coupon/{id}
        [HttpGet("{id}")]
        public ActionResult<CouponDTO> GetCouponById(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var coupon = _couponRepository.GetById(id);
                var couponDTO = _mapper.Map<CouponDTO>(coupon);
                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/coupon/{id}/with-items
        [HttpGet("{id}/with-items")]
        public ActionResult<CouponWithItemsDTO> GetCouponWithItems(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                if (coupon == null)
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);
                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting coupon with items with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/coupon
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
                coupon.items = new List<Item>(); // Инициализируем пустой список

                var createdCoupon = _couponRepository.Create(coupon);
                var couponDTO = _mapper.Map<CouponDTO>(createdCoupon);

                return CreatedAtAction(nameof(GetCouponById), new { id = couponDTO.Id }, couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating coupon");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/coupon/{id}
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
                    return NotFound($"Coupon with ID {id} not found");
                }

                var existingCoupon = _couponRepository.GetById(id);
                if (existingCoupon == null)
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                // Validate business rules
                var validationResult = ValidateCouponUpdate(updateCouponDTO, existingCoupon);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.ErrorMessage);
                }

                // Update properties using mapper
                _mapper.Map(updateCouponDTO, existingCoupon);
                existingCoupon.UpdatedAt = DateTime.UtcNow;

                // Check if coupon should be deactivated based on usage
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
                _logger.LogError(ex, "Error occurred while updating coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/coupon/{id}/items
        [HttpPost("{id}/items")]
        public ActionResult<CouponWithItemsDTO> AddItemsToCoupon(int id, [FromBody] AddItemsToCouponDTO addItemsDTO)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var result = _couponRepository.AddItemsToCoupon(id, addItemsDTO.ItemIds);
                if (!result)
                {
                    return BadRequest("Failed to add items to coupon");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding items to coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/coupon/{id}/items
        [HttpDelete("{id}/items")]
        public ActionResult<CouponWithItemsDTO> RemoveAllItemsFromCoupon(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var result = _couponRepository.RemoveAllItemsFromCoupon(id);
                if (!result)
                {
                    return BadRequest("Failed to remove items from coupon");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing items from coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/coupon/{id}/items/{itemId}
        [HttpDelete("{id}/items/{itemId}")]
        public ActionResult<CouponWithItemsDTO> RemoveItemFromCoupon(int id, int itemId)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound($"Coupon with ID {id} not found");
                }

                var result = _couponRepository.RemoveItemFromCoupon(id, itemId);
                if (!result)
                {
                    return BadRequest("Failed to remove item from coupon");
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = _mapper.Map<CouponWithItemsDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item from coupon with ID {CouponId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/coupon/{id}
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

        // Остальные методы (deactivate, activate, apply, etc.) остаются без изменений
        // PATCH: api/coupon/{id}/deactivate
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

        // GET: api/coupon/active
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

        // Helper methods for validation (остаются без изменений)
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

    // Helper classes (остаются без изменений)
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static ValidationResult Valid() => new ValidationResult { IsValid = true };
        public static ValidationResult Invalid(string message) => new ValidationResult { IsValid = false, ErrorMessage = message };
    }
}