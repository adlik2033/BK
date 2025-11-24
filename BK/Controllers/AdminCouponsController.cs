using Microsoft.AspNetCore.Mvc;
using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using BK.Attributes;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [AuthorizeRoles(UserRoles.Admin, UserRoles.Manager)]
    public class AdminCouponsController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;

        public AdminCouponsController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CouponAdminDTO>>> GetAllCoupons()
        {
            try
            {
                var coupons = _couponRepository.GetAll();
                var couponDTOs = coupons.Select(c => new CouponAdminDTO
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue,
                    ValidFrom = c.ValidFrom,
                    ValidUntil = c.ValidUntil,
                    UsageLimit = c.UsageLimit,
                    UsageCount = c.UsageCount,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Items = c.Items.Select(i => new ItemUserDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Category = new CategorySimpleDTO
                        {
                            Id = i.Category.Id,
                            Name = i.Category.Name,
                            Description = i.Category.Description
                        }
                    }).ToList()
                }).ToList();

                return Ok(new ApiResponse<IEnumerable<CouponAdminDTO>>
                {
                    Success = true,
                    Message = "Все купоны успешно получены",
                    Data = couponDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<CouponAdminDTO>> CreateCoupon([FromBody] CreateCouponDTO createCouponDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Ошибка валидации",
                        Details = "Проверьте правильность введенных данных"
                    });
                }

                var validationResult = ValidateCoupon(createCouponDTO);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка валидации", Details = validationResult.ErrorMessage });
                }

                // Проверка уникальности кода
                var existingCoupon = _couponRepository.GetByCode(createCouponDTO.Code);
                if (existingCoupon != null)
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка", Details = "Купон с таким кодом уже существует" });
                }

                var coupon = new Coupon
                {
                    Code = createCouponDTO.Code,
                    Description = createCouponDTO.Description,
                    DiscountType = createCouponDTO.DiscountType,
                    DiscountValue = createCouponDTO.DiscountValue,
                    ValidFrom = createCouponDTO.ValidFrom,
                    ValidUntil = createCouponDTO.ValidUntil,
                    UsageLimit = createCouponDTO.UsageLimit,
                    UsageCount = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Items = new List<Item>()
                };

                var createdCoupon = _couponRepository.Create(coupon);

                var couponDTO = new CouponAdminDTO
                {
                    Id = createdCoupon.Id,
                    Code = createdCoupon.Code,
                    Description = createdCoupon.Description,
                    DiscountType = createdCoupon.DiscountType,
                    DiscountValue = createdCoupon.DiscountValue,
                    ValidFrom = createdCoupon.ValidFrom,
                    ValidUntil = createdCoupon.ValidUntil,
                    UsageLimit = createdCoupon.UsageLimit,
                    UsageCount = createdCoupon.UsageCount,
                    IsActive = createdCoupon.IsActive,
                    CreatedAt = createdCoupon.CreatedAt,
                    UpdatedAt = createdCoupon.UpdatedAt,
                    Items = new List<ItemUserDTO>()
                };

                return Ok(new ApiResponse<CouponAdminDTO>
                {
                    Success = true,
                    Message = "Купон успешно создан",
                    Data = couponDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка создания купона", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<CouponAdminDTO>> UpdateCoupon(int id, [FromBody] UpdateCouponDTO updateCouponDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Ошибка валидации",
                        Details = "Проверьте правильность введенных данных"
                    });
                }

                if (!_couponRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                var existingCoupon = _couponRepository.GetById(id);
                if (existingCoupon == null)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                // Проверка уникальности кода исключая текущий купон
                var existingCodeCoupon = _couponRepository.GetByCode(updateCouponDTO.Code);
                if (existingCodeCoupon != null && existingCodeCoupon.Id != id)
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка", Details = "Купон с таким кодом уже существует" });
                }

                var validationResult = ValidateCouponUpdate(updateCouponDTO, existingCoupon);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка валидации", Details = validationResult.ErrorMessage });
                }

                existingCoupon.Code = updateCouponDTO.Code;
                existingCoupon.Description = updateCouponDTO.Description;
                existingCoupon.DiscountType = updateCouponDTO.DiscountType;
                existingCoupon.DiscountValue = updateCouponDTO.DiscountValue;
                existingCoupon.ValidFrom = updateCouponDTO.ValidFrom;
                existingCoupon.ValidUntil = updateCouponDTO.ValidUntil;
                existingCoupon.UsageCount = updateCouponDTO.UsageCount;
                existingCoupon.UpdatedAt = DateTime.UtcNow;

                if (existingCoupon.UsageCount >= existingCoupon.UsageLimit)
                {
                    existingCoupon.IsActive = false;
                }

                var updatedCoupon = _couponRepository.Update(existingCoupon);

                var couponDTO = new CouponAdminDTO
                {
                    Id = updatedCoupon.Id,
                    Code = updatedCoupon.Code,
                    Description = updatedCoupon.Description,
                    DiscountType = updatedCoupon.DiscountType,
                    DiscountValue = updatedCoupon.DiscountValue,
                    ValidFrom = updatedCoupon.ValidFrom,
                    ValidUntil = updatedCoupon.ValidUntil,
                    UsageLimit = updatedCoupon.UsageLimit,
                    UsageCount = updatedCoupon.UsageCount,
                    IsActive = updatedCoupon.IsActive,
                    CreatedAt = updatedCoupon.CreatedAt,
                    UpdatedAt = updatedCoupon.UpdatedAt,
                    Items = updatedCoupon.Items.Select(i => new ItemUserDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Category = new CategorySimpleDTO
                        {
                            Id = i.Category.Id,
                            Name = i.Category.Name,
                            Description = i.Category.Description
                        }
                    }).ToList()
                };

                return Ok(new ApiResponse<CouponAdminDTO>
                {
                    Success = true,
                    Message = "Купон успешно обновлен",
                    Data = couponDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка обновления купона", Details = ex.Message });
            }
        }

        [HttpPost("{id}/items")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<CouponAdminDTO>> AddItemsToCoupon(int id, [FromBody] AddItemsToCouponDTO addItemsDTO)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                var result = _couponRepository.AddItemsToCoupon(id, addItemsDTO.ItemIds);
                if (!result)
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка", Details = "Ошибка добавления товаров к купону" });
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = new CouponAdminDTO
                {
                    Id = coupon.Id,
                    Code = coupon.Code,
                    Description = coupon.Description,
                    DiscountType = coupon.DiscountType,
                    DiscountValue = coupon.DiscountValue,
                    ValidFrom = coupon.ValidFrom,
                    ValidUntil = coupon.ValidUntil,
                    UsageLimit = coupon.UsageLimit,
                    UsageCount = coupon.UsageCount,
                    IsActive = coupon.IsActive,
                    CreatedAt = coupon.CreatedAt,
                    UpdatedAt = coupon.UpdatedAt,
                    Items = coupon.Items.Select(i => new ItemUserDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Category = new CategorySimpleDTO
                        {
                            Id = i.Category.Id,
                            Name = i.Category.Name,
                            Description = i.Category.Description
                        }
                    }).ToList()
                };

                return Ok(new ApiResponse<CouponAdminDTO>
                {
                    Success = true,
                    Message = "Товары успешно добавлены к купону",
                    Data = couponDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка добавления товаров", Details = ex.Message });
            }
        }

        [HttpDelete("{id}/items")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<CouponAdminDTO>> RemoveAllItemsFromCoupon(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                var result = _couponRepository.RemoveAllItemsFromCoupon(id);
                if (!result)
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка", Details = "Ошибка удаления товаров из купона" });
                }

                var coupon = _couponRepository.GetByIdWithItems(id);
                var couponDTO = new CouponAdminDTO
                {
                    Id = coupon.Id,
                    Code = coupon.Code,
                    Description = coupon.Description,
                    DiscountType = coupon.DiscountType,
                    DiscountValue = coupon.DiscountValue,
                    ValidFrom = coupon.ValidFrom,
                    ValidUntil = coupon.ValidUntil,
                    UsageLimit = coupon.UsageLimit,
                    UsageCount = coupon.UsageCount,
                    IsActive = coupon.IsActive,
                    CreatedAt = coupon.CreatedAt,
                    UpdatedAt = coupon.UpdatedAt,
                    Items = new List<ItemUserDTO>()
                };

                return Ok(new ApiResponse<CouponAdminDTO>
                {
                    Success = true,
                    Message = "Все товары успешно удалены из купона",
                    Data = couponDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка удаления товаров", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<bool>> DeleteCoupon(int id)
        {
            try
            {
                if (!_couponRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Купон не найден" });
                }

                var result = _couponRepository.Delete(id);
                if (result)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "Купон успешно удален",
                        Data = true
                    });
                }

                return StatusCode(500, new ErrorResponse { Error = "Ошибка", Details = "Не удалось удалить купон" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        private ValidationResult ValidateCoupon(CreateCouponDTO coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
            {
                return ValidationResult.Invalid("Код купона обязателен");
            }

            if (coupon.ValidUntil <= coupon.ValidFrom)
            {
                return ValidationResult.Invalid("Дата окончания должна быть позже даты начала");
            }

            if (coupon.UsageLimit <= 0)
            {
                return ValidationResult.Invalid("Лимит использований должен быть больше 0");
            }

            if (coupon.DiscountValue <= 0)
            {
                return ValidationResult.Invalid("Значение скидки должно быть положительным");
            }

            var validDiscountTypes = new[] { "Процентная", "Бесплатная доставка" };
            if (!validDiscountTypes.Contains(coupon.DiscountType))
            {
                return ValidationResult.Invalid($"Тип скидки должен быть одним из: {string.Join(", ", validDiscountTypes)}");
            }

            if (coupon.DiscountType == "Percentage" && coupon.DiscountValue > 100)
            {
                return ValidationResult.Invalid("Процентная скидка не может превышать 100%");
            }

            return ValidationResult.Valid();
        }

        private ValidationResult ValidateCouponUpdate(UpdateCouponDTO coupon, Coupon existingCoupon)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
            {
                return ValidationResult.Invalid("Код купона обязателен");
            }

            if (coupon.ValidUntil <= coupon.ValidFrom)
            {
                return ValidationResult.Invalid("Дата окончания должна быть позже даты начала");
            }

            if (coupon.UsageCount < 0)
            {
                return ValidationResult.Invalid("Количество использований не может быть отрицательным");
            }

            if (coupon.UsageCount > existingCoupon.UsageLimit)
            {
                return ValidationResult.Invalid("Количество использований не может превышать лимит");
            }

            if (coupon.DiscountValue <= 0)
            {
                return ValidationResult.Invalid("Значение скидки должно быть положительным");
            }

            var validDiscountTypes = new[] { "Percentage", "Fixed", "FreeShipping" };
            if (!validDiscountTypes.Contains(coupon.DiscountType))
            {
                return ValidationResult.Invalid($"Тип скидки должен быть одним из: {string.Join(", ", validDiscountTypes)}");
            }

            if (coupon.DiscountType == "Percentage" && coupon.DiscountValue > 100)
            {
                return ValidationResult.Invalid("Процентная скидка не может превышать 100%");
            }

            return ValidationResult.Valid();
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static ValidationResult Valid() => new ValidationResult { IsValid = true };
        public static ValidationResult Invalid(string message) => new ValidationResult { IsValid = false, ErrorMessage = message };
    }
}