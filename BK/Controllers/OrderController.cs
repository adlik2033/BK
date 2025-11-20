using AutoMapper;
using BK.Models;
using BK.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IOrderRepository _orderRepository;

        public OrderController(IOrderRepository order, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = order;
        }
    }
}
