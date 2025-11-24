using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ItemsController(IItemRepository itemRepository, ICategoryRepository categoryRepository)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ItemUserDTO>>> GetItems()
        {
            try
            {
                var items = _itemRepository.GetAll();
                var itemDTOs = items.Select(item => new ItemUserDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Category = new CategorySimpleDTO
                    {
                        Id = item.Category.Id,
                        Name = item.Category.Name,
                        Description = item.Category.Description
                    }
                }).ToList();

                return Ok(new ApiResponse<IEnumerable<ItemUserDTO>>
                {
                    Success = true,
                    Message = "Товары успешно получены",
                    Data = itemDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<ItemUserDTO>> GetItem(int id)
        {
            try
            {
                var item = _itemRepository.GetById(id);

                if (item == null || !item.IsActive)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Товар не найден" });
                }

                var itemDTO = new ItemUserDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Category = new CategorySimpleDTO
                    {
                        Id = item.Category.Id,
                        Name = item.Category.Name,
                        Description = item.Category.Description
                    }
                };

                return Ok(new ApiResponse<ItemUserDTO>
                {
                    Success = true,
                    Message = "Товар успешно получен",
                    Data = itemDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }
    }
}