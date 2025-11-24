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
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public AdminCategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CategoryAdminDTO>>> GetAllCategories()
        {
            try
            {
                var categories = _categoryRepository.GetAll();
                var categoryDTOs = categories.Select(c => new CategoryAdminDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdateAt = c.UpdateAt,
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

                return Ok(new ApiResponse<IEnumerable<CategoryAdminDTO>>
                {
                    Success = true,
                    Message = "Все категории успешно получены",
                    Data = categoryDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<CategoryAdminDTO>> CreateCategory([FromBody] CreateCategoryDTO createCategoryDTO)
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

                var category = new Category
                {
                    Name = createCategoryDTO.Name,
                    Description = createCategoryDTO.Description,
                    Items = new List<Item>(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };

                var createdCategory = _categoryRepository.Create(category);

                var categoryDTO = new CategoryAdminDTO
                {
                    Id = createdCategory.Id,
                    Name = createdCategory.Name,
                    Description = createdCategory.Description,
                    IsActive = createdCategory.IsActive,
                    CreatedAt = createdCategory.CreatedAt,
                    UpdateAt = createdCategory.UpdateAt,
                    Items = new List<ItemUserDTO>()
                };

                return Ok(new ApiResponse<CategoryAdminDTO>
                {
                    Success = true,
                    Message = "Категория успешно создана",
                    Data = categoryDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка создания категории", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(UserRoles.Admin, UserRoles.Manager)]
        public ActionResult<ApiResponse<CategoryAdminDTO>> UpdateCategory(int id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
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

                if (!_categoryRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Категория не найдена" });
                }

                var existingCategory = _categoryRepository.GetById(id);
                if (existingCategory == null)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Категория не найдена" });
                }

                existingCategory.Name = updateCategoryDTO.Name;
                existingCategory.Description = updateCategoryDTO.Description;
                existingCategory.UpdateAt = DateTime.UtcNow;

                var updatedCategory = _categoryRepository.Update(existingCategory);

                var categoryDTO = new CategoryAdminDTO
                {
                    Id = updatedCategory.Id,
                    Name = updatedCategory.Name,
                    Description = updatedCategory.Description,
                    IsActive = updatedCategory.IsActive,
                    CreatedAt = updatedCategory.CreatedAt,
                    UpdateAt = updatedCategory.UpdateAt,
                    Items = updatedCategory.Items.Select(i => new ItemUserDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Category = new CategorySimpleDTO
                        {
                            Id = updatedCategory.Id,
                            Name = updatedCategory.Name,
                            Description = updatedCategory.Description
                        }
                    }).ToList()
                };

                return Ok(new ApiResponse<CategoryAdminDTO>
                {
                    Success = true,
                    Message = "Категория успешно обновлена",
                    Data = categoryDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка обновления категории", Details = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<bool>> DeleteCategory(int id)
        {
            try
            {
                if (!_categoryRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Категория не найдена" });
                }

                var result = _categoryRepository.Delete(id);

                if (result)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "Категория успешно удалена",
                        Data = true
                    });
                }

                return StatusCode(500, new ErrorResponse { Error = "Ошибка", Details = "Не удалось удалить категорию" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }
    }
}