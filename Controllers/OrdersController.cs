using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewOrdersApi.DataModel;

namespace NewOrdersApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ECommerceContext _context;

        public OrdersController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
       

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> placeOrder([Bind("Id,UserId,AddressLine1,AddressLine2,City,PostalCode,Country,Mobile,MailId,ContactPerson")] UserAddress address)
        {
            int userId = 1000;
            try
            {
                var cart = await _context.Carts.Where(o => o.UserId == userId).ToListAsync();
                int total = (int)(from c in cart where c.UserId == userId select c).Sum(x => x.SubTotal).Value;

                Order order = new Order();
                order.UserId = 1000;
                order.TotalAmount = total;
                order.PaymentType = "COD";

                _context.Add(order);
                await _context.SaveChangesAsync();

                var orderInfo = _context.Orders.Where(o => o.UserId == userId).FirstOrDefault();
                foreach (var c in cart)
                {

                    OrderItem oi = new OrderItem();
                    oi.OrderId = orderInfo.OrderId;
                    oi.ProductId = c.ProductId;
                    oi.Product = _context.Products.Where(p => p.ProductId == c.ProductId).FirstOrDefault();
                    oi.SubTotal = c.SubTotal;
                    oi.Quantity = c.Quantity;
                    _context.Add(oi);
                    await _context.SaveChangesAsync();

                }
                address.UserId = 1000;
                _context.Add(address);
                await _context.SaveChangesAsync();

                if (cart != null)
                {
                    _context.Carts.RemoveRange(cart);
                    _context.SaveChanges();
                    return Ok();

                }



                return Ok();
            }
            catch (Exception e)
            {
                return Ok();
            }
        }
        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
