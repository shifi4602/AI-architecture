using DTO_s;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        IOrderService _iOrderService;
        public OrdersController(IOrderService iOrderService)
        {
            _iOrderService = iOrderService;
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersDTO>> GetOrderById(int id)
        {
            OrdersDTO orderDTO = await _iOrderService.GetOrderById(id);
            if (orderDTO != null)
            {
                return Ok(orderDTO);
            }
            else
                return NoContent();
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<OrdersDTO>> Post([FromBody] OrdersDTO orderDto)
        {
            OrdersDTO order1 = await _iOrderService.AddNewOrder(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { order1.OrderId }, order1);
        }
    }
}
