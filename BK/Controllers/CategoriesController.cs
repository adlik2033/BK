using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CategoryUserDTO>>> GetCategories()
        {
            try
            {
                var categories = _categoryRepository.GetActiveCategories();
                var categoryDTOs = categories.Select(c => new CategoryUserDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Items = c.Items.Select(i => new ItemUserDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Category = new CategorySimpleDTO
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description
                        }
                    }).ToList()
                }).ToList();

                return Ok(new ApiResponse<IEnumerable<CategoryUserDTO>>
                {
                    Success = true,
                    Message = "Категории успешно получены",
                    Data = categoryDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<CategoryUserDTO>> GetCategory(int id)
        {
            try
            {
                var category = _categoryRepository.GetById(id);

                if (category == null || !category.IsActive)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Категория не найдена" });
                }

                var categoryDTO = new CategoryUserDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Items = category.Items.Select(i => new ItemUserDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Category = new CategorySimpleDTO
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Description = category.Description
                        }
                    }).ToList()
                };

                return Ok(new ApiResponse<CategoryUserDTO>
                {
                    Success = true,
                    Message = "Категория успешно получена",
                    Data = categoryDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }
    }
}