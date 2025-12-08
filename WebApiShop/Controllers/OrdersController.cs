using Enteties;
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
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            Order order = await _iOrderService.GetOrderById(id);
            if (order != null)
            {
                return Ok(order);
            }
            else
                return NoContent();
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<Order>> Post([FromBody] Order order)
        {
            Order order1 = await _iOrderService.AddNewOrder(order);
            return CreatedAtAction(nameof(GetOrderById), new { order1.OrderId }, order1);
        }
    }
}
