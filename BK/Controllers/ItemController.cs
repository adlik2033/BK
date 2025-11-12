using AutoMapper;
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
        private readonly IMapper _mapper;

        public ItemsController(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        // GET: api/items
        [HttpGet]
        public ActionResult<IEnumerable<ItemDTO>> GetItems()
        {
            var items = _itemRepository.GetAll();
            var itemDTOs = _mapper.Map<IEnumerable<ItemDTO>>(items);
            return Ok(itemDTOs);
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItem(int id)
        {
            var item = _itemRepository.GetById(id);

            if (item == null)
            {
                return NotFound();
            }

            var itemDTO = _mapper.Map<ItemDTO>(item);
            return Ok(itemDTO);
        }

        // GET: api/items/category/5
        [HttpGet("category/{categoryId}")]
        public ActionResult<IEnumerable<ItemDTO>> GetItemsByCategory(int categoryId)
        {
            var items = _itemRepository.GetByCategoryId(categoryId);
            var itemDTOs = _mapper.Map<IEnumerable<ItemDTO>>(items);
            return Ok(itemDTOs);
        }

        // POST: api/items
        [HttpPost]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO createItemDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = _mapper.Map<Item>(createItemDTO);
            var createdItem = _itemRepository.Create(item);
            var itemDTO = _mapper.Map<ItemDTO>(createdItem);

            return CreatedAtAction(nameof(GetItem), new { id = itemDTO.Id }, itemDTO);
        }

        // PUT: api/items/5
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

            ArgumentNullException.ThrowIfNull(updateItemDTO);
            var existingItem = _itemRepository.GetById(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            _mapper.Map(updateItemDTO, existingItem);
            _itemRepository.Update(existingItem);

            return NoContent();
        }

        // DELETE: api/items/5
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