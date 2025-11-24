using AutoMapper;
using BK.Models;
using BK.Models.DTO;
using BK.Repositories;
using Microsoft.AspNetCore.Mvc;
using BK.Attributes;
using System.Security.Claims;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public OrderController(IOrderRepository orderRepository, IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [AuthorizeRoles(UserRoles.User, UserRoles.Manager, UserRoles.Admin)]
        public ActionResult<ApiResponse<object>> GetOrders()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                object ordersDTO;

                if (userRole == UserRoles.User)
                {
                    // Пользователь видит только свои заказы
                    var orders = _orderRepository.GetUserOrders(userId);
                    ordersDTO = _mapper.Map<IEnumerable<OrderUserDTO>>(orders);
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Ваши заказы успешно получены",
                        Data = ordersDTO
                    });
                }
                else if (userRole == UserRoles.Manager)
                {
                    // Менеджер видит все заказы с деталями
                    var orders = _orderRepository.GetAll();
                    ordersDTO = _mapper.Map<IEnumerable<OrderManagerDTO>>(orders);
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Все заказы успешно получены",
                        Data = ordersDTO
                    });
                }
                else // Admin
                {
                    // Админ видит полную информацию о заказах
                    var orders = _orderRepository.GetAll();
                    ordersDTO = _mapper.Map<IEnumerable<OrderAdminDTO>>(orders);
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Все заказы успешно получены",
                        Data = ordersDTO
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AuthorizeRoles(UserRoles.User, UserRoles.Manager, UserRoles.Admin)]
        public ActionResult<ApiResponse<object>> GetOrder(int id)
        {
            try
            {
                var order = _orderRepository.GetById(id);
                if (order == null)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Заказ не найден" });
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Проверка прав доступа
                if (userRole == UserRoles.User && order.UserId != userId)
                {
                    return Forbid();
                }

                object orderDTO;

                if (userRole == UserRoles.User)
                {
                    orderDTO = _mapper.Map<OrderUserDTO>(order);
                }
                else if (userRole == UserRoles.Manager)
                {
                    orderDTO = _mapper.Map<OrderManagerDTO>(order);
                }
                else // Admin
                {
                    orderDTO = _mapper.Map<OrderAdminDTO>(order);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Заказ успешно получен",
                    Data = orderDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizeRoles(UserRoles.User, UserRoles.Manager, UserRoles.Admin)]
        public ActionResult<ApiResponse<object>> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
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

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var order = new Order
                {
                    UserId = userId,
                    Status = "Создан",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdOrder = _orderRepository.CreateOrder(order, createOrderDTO.ItemIds, createOrderDTO.CouponIds);

                object orderDTO;
                if (userRole == UserRoles.User)
                {
                    orderDTO = _mapper.Map<OrderUserDTO>(createdOrder);
                }
                else if (userRole == UserRoles.Manager)
                {
                    orderDTO = _mapper.Map<OrderManagerDTO>(createdOrder);
                }
                else
                {
                    orderDTO = _mapper.Map<OrderAdminDTO>(createdOrder);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Заказ успешно создан",
                    Data = orderDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка создания заказа", Details = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Admin)]
        public ActionResult<ApiResponse<object>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO updateStatusDTO)
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

                var order = _orderRepository.GetById(id);
                if (order == null)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Заказ не найден" });
                }

                order.Status = updateStatusDTO.Status;
                order.UpdatedAt = DateTime.UtcNow;

                var updatedOrder = _orderRepository.Update(order);

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                object orderDTO;

                if (userRole == UserRoles.Manager)
                {
                    orderDTO = _mapper.Map<OrderManagerDTO>(updatedOrder);
                }
                else
                {
                    orderDTO = _mapper.Map<OrderAdminDTO>(updatedOrder);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Статус заказа успешно обновлен",
                    Data = orderDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка обновления статуса", Details = ex.Message });
            }
        }

        [HttpGet("user/my-orders")]
        [AuthorizeRoles(UserRoles.User)]
        public ActionResult<ApiResponse<IEnumerable<OrderUserDTO>>> GetMyOrders()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var orders = _orderRepository.GetUserOrders(userId);
                var ordersDTO = _mapper.Map<IEnumerable<OrderUserDTO>>(orders);

                return Ok(new ApiResponse<IEnumerable<OrderUserDTO>>
                {
                    Success = true,
                    Message = "Ваши заказы успешно получены",
                    Data = ordersDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Admin)]
        public ActionResult<ApiResponse<object>> GetOrdersByStatus(string status)
        {
            try
            {
                var orders = _orderRepository.GetOrdersByStatus(status);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                object ordersDTO;
                if (userRole == UserRoles.Manager)
                {
                    ordersDTO = _mapper.Map<IEnumerable<OrderManagerDTO>>(orders);
                }
                else
                {
                    ordersDTO = _mapper.Map<IEnumerable<OrderAdminDTO>>(orders);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Заказы со статусом '{status}' успешно получены",
                    Data = ordersDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [AuthorizeRoles(UserRoles.Admin)]
        public ActionResult<ApiResponse<bool>> DeleteOrder(int id)
        {
            try
            {
                if (!_orderRepository.Exists(id))
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Заказ не найден" });
                }

                var result = _orderRepository.Delete(id);
                if (result)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "Заказ успешно удален",
                        Data = true
                    });
                }

                return StatusCode(500, new ErrorResponse { Error = "Ошибка", Details = "Не удалось удалить заказ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "Ошибка сервера", Details = ex.Message });
            }
        }

        [HttpPut("{id}/cancel")]
        [AuthorizeRoles(UserRoles.User, UserRoles.Manager, UserRoles.Admin)]
        public ActionResult<ApiResponse<object>> CancelOrder(int id)
        {
            try
            {
                var order = _orderRepository.GetById(id);
                if (order == null)
                {
                    return NotFound(new ErrorResponse { Error = "Ошибка", Details = "Заказ не найден" });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Проверка прав: пользователь может отменять только свои заказы
                if (userRole == UserRoles.User && order.UserId != userId)
                {
                    return Forbid();
                }

                order.Status = "Отменен";
                order.UpdatedAt = DateTime.UtcNow;

                var updatedOrder = _orderRepository.Update(order);

                object orderDTO;
                if (userRole == UserRoles.User)
                {
                    orderDTO = _mapper.Map<OrderUserDTO>(updatedOrder);
                }
                else if (userRole == UserRoles.Manager)
                {
                    orderDTO = _mapper.Map<OrderManagerDTO>(updatedOrder);
                }
                else
                {
                    orderDTO = _mapper.Map<OrderAdminDTO>(updatedOrder);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Заказ успешно отменен",
                    Data = orderDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка отмены заказа", Details = ex.Message });
            }
        }
    }
}