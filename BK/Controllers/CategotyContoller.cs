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
        public ActionResult<IEnumerable<CategoryDTO>> GetCategories()
        {
            var categories = _categoryRepository.GetAll();
            var categoryDTOs = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Items = c.Items.Select(i => new ItemSimpleDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price
                }).ToList()
            }).ToList();

            return Ok(categoryDTOs);
        }

        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetCategory(int id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Items = category.Items.Select(i => new ItemSimpleDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price
                }).ToList()
            };

            return Ok(categoryDTO);
        }

        [HttpGet("active")]
        public ActionResult<IEnumerable<CategoryDTO>> GetActiveCategories()
        {
            var categories = _categoryRepository.GetAll().Where(c => c.IsActive);
            var categoryDTOs = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Items = c.Items.Select(i => new ItemSimpleDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price
                }).ToList()
            }).ToList();

            return Ok(categoryDTOs);
        }

        [HttpPost]
        public ActionResult<CategoryDTO> CreateCategory(CreateCategoryDTO createCategoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            var categoryDTO = new CategoryDTO
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                Description = createdCategory.Description,
                Items = new List<ItemSimpleDTO>()
            };

            return CreatedAtAction(nameof(GetCategory), new { id = categoryDTO.Id }, categoryDTO);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, CategorySimpleDTO categorySimpleDTO)
        {
            if (id != categorySimpleDTO.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.Exists(id))
            {
                return NotFound();
            }

            var existingCategory = _categoryRepository.GetById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = categorySimpleDTO.Name;
            existingCategory.Description = categorySimpleDTO.Description;
            existingCategory.UpdateAt = DateTime.UtcNow;

            _categoryRepository.Update(existingCategory);

            return NoContent();
        }

        [HttpPatch("{id}/toggle")]
        public IActionResult ToggleCategory(int id)
        {
            if (!_categoryRepository.Exists(id))
            {
                return NotFound();
            }

            var existingCategory = _categoryRepository.GetById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.IsActive = !existingCategory.IsActive;
            existingCategory.UpdateAt = DateTime.UtcNow;

            _categoryRepository.Update(existingCategory);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            if (!_categoryRepository.Exists(id))
            {
                return NotFound();
            }

            var result = _categoryRepository.Delete(id);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}