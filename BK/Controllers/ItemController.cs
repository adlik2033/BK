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

        // GET: api/items
        [HttpGet]
        public ActionResult<IEnumerable<ItemDTO>> GetItems()
        {
            var items = _itemRepository.GetAll();
            var itemDTOs = items.Select(item => new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description, 
                Price = item.Price,
                CategoryId = item.CategoryId,
                Category = new CategorySimpleDTO
                {
                    Id = item.Category.Id,
                    Name = item.Category.Name,
                    Description = item.Category.Description
                }
            }).ToList();

            return Ok(itemDTOs);
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItem(int id)
        {
            var item = _itemRepository.GetById(id);

            if (item == null)
            {
                return NotFound();
            }

            var itemDTO = new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId,
                Category = new CategorySimpleDTO
                {
                    Id = item.Category.Id,
                    Name = item.Category.Name,
                    Description = item.Category.Description
                }
            };

            return Ok(itemDTO);
        }

        [HttpPost]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO createItemDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем существование категории
            if (!_categoryRepository.Exists(createItemDTO.CategoryId))
            {
                return BadRequest($"Category with ID {createItemDTO.CategoryId} does not exist.");
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

            var itemDTO = new ItemDTO
            {
                Id = createdItem.Id,
                Name = createdItem.Name,
                Description = createdItem.Description,
                Price = createdItem.Price,
                CategoryId = createdItem.CategoryId,
                Category = new CategorySimpleDTO
                {
                    Id = createdItem.Category.Id,
                    Name = createdItem.Category.Name,
                    Description = createdItem.Category.Description
                }
            };

            return CreatedAtAction(nameof(GetItem), new { id = itemDTO.Id }, itemDTO);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItem(int id, UpdateItemDTO updateItemDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_itemRepository.Exists(id))
            {
                return NotFound();
            }

            // Проверяем существование категории
            if (!_categoryRepository.Exists(updateItemDTO.CategoryId))
            {
                return BadRequest($"Category with ID {updateItemDTO.CategoryId} does not exist.");
            }

            var existingItem = _itemRepository.GetById(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDTO.Name;
            existingItem.Description = updateItemDTO.Description;
            existingItem.Price = updateItemDTO.Price;
            existingItem.CategoryId = updateItemDTO.CategoryId;
            existingItem.UpdateAt = DateTime.UtcNow;

            _itemRepository.Update(existingItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            if (!_itemRepository.Exists(id))
            {
                return NotFound();
            }

            var result = _itemRepository.Delete(id);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}