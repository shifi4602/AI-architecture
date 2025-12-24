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
        public async Task<ActionResult<OrdersDTO>> GetById(int id)
        {
            OrdersDTO orderResult = await _iOrderService.GetOrderById(id);
            if (orderResult == null)
                return NoContent();
            return Ok(orderResult);
        }


        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<OrdersDTO>> Post([FromBody] OrdersDTO orderDto)
        {
            OrdersDTO orderResult = await _iOrderService.AddNewOrder(orderDto);
            return CreatedAtAction(nameof(GetById), new { id = orderResult.OrderId }, orderResult);
        }
    }
}
