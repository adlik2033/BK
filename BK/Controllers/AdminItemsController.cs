using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using Microsoft.AspNetCore.Mvc;
using BK.Attributes;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [AuthorizeRoles(UserRoles.Admin, UserRoles.Manager)]
    public class AdminItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AdminItemsController(IItemRepository itemRepository, ICategoryRepository categoryRepository)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ItemAdminDTO>>> GetAllItems()
        {
            try
            {
                var items = _itemRepository.GetAll();
                var itemDTOs = items.Select(item => new ItemAdminDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    IsActive = item.IsActive,
                    CreatedAt = item.CreatedAt,
                    UpdateAt = item.UpdateAt,
                    CategoryId = item.CategoryId,
                    Category = new CategorySimpleDTO
                    {
                        Id = item.Category.Id,
                        Name = item.Category.Name,
                        Description = item.Category.Description
                    }
                }).ToList();

                return Ok(new ApiResponse<IEnumerable<ItemAdminDTO>>
                {
                    Success = true,
                    Message = "Все товары успешно получены",
                    Data = itemDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizeRoles(UserRoles.Admin, UserRoles.Manager)]
        public ActionResult<ApiResponse<ItemAdminDTO>> CreateItem([FromBody] CreateItemDTO createItemDTO)
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

                if (!_categoryRepository.Exists(createItemDTO.CategoryId))
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка", Details = "Категория не существует" });
                }

                var item = new Item
                {
                    Name = createItemDTO.Name,
                    Description = createItemDTO.Description,
                    Price = createItemDTO.Price,
                    CategoryId = createItemDTO.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };

                var createdItem = _itemRepository.Create(item);

                var itemDTO = new ItemAdminDTO
                {
                    Id = createdItem.Id,
                    Name = createdItem.Name,
                    Description = createdItem.Description,
                    Price = createdItem.Price,
                    IsActive = createdItem.IsActive,
                    CreatedAt = createdItem.CreatedAt,
                    UpdateAt = createdItem.UpdateAt,
                    CategoryId = createdItem.CategoryId,
                    Category = new CategorySimpleDTO
                    {
                        Id = createdItem.Category.Id,
                        Name = createdItem.Category.Name,
                        Description = createdItem.Category.Description
                    }
                };

                return Ok(new ApiResponse<ItemAdminDTO>
                {
                    Success = true,
                    Message = "Товар успешно создан",
                    Data = itemDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка создания товара", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(UserRoles.Admin, UserRoles.Manager)]
        public ActionResult<ApiResponse<ItemAdminDTO>> UpdateItem(int id, [FromBody] UpdateItemDTO updateItemDTO)
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

                if (!_itemRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Товар не найден" });
                }

                if (!_categoryRepository.Exists(updateItemDTO.CategoryId))
                {
                    return BadRequest(new ErrorResponse { Error = "Ошибка", Details = "Категория не существует" });
                }

                var existingItem = _itemRepository.GetById(id);
                if (existingItem == null)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Товар не найден" });
                }

                existingItem.Name = updateItemDTO.Name;
                existingItem.Description = updateItemDTO.Description;
                existingItem.Price = updateItemDTO.Price;
                existingItem.CategoryId = updateItemDTO.CategoryId;
                existingItem.UpdateAt = DateTime.UtcNow;

                var updatedItem = _itemRepository.Update(existingItem);

                var itemDTO = new ItemAdminDTO
                {
                    Id = updatedItem.Id,
                    Name = updatedItem.Name,
                    Description = updatedItem.Description,
                    Price = updatedItem.Price,
                    IsActive = updatedItem.IsActive,
                    CreatedAt = updatedItem.CreatedAt,
                    UpdateAt = updatedItem.UpdateAt,
                    CategoryId = updatedItem.CategoryId,
                    Category = new CategorySimpleDTO
                    {
                        Id = updatedItem.Category.Id,
                        Name = updatedItem.Category.Name,
                        Description = updatedItem.Category.Description
                    }
                };

                return Ok(new ApiResponse<ItemAdminDTO>
                {
                    Success = true,
                    Message = "Товар успешно обновлен",
                    Data = itemDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка обновления товара", Details = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<bool>> DeleteItem(int id)
        {
            try
            {
                if (!_itemRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Товар не найден" });
                }

                var result = _itemRepository.Delete(id);

                if (result)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "Товар успешно удален",
                        Data = true
                    });
                }

                return StatusCode(500, new ErrorResponse { Error = "Ошибка", Details = "Не удалось удалить товар" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }
    }
}