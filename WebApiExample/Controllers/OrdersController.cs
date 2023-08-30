using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApiExample.Data;
using WebApiExample.Models;

namespace WebApiExample.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController :ControllerBase {

        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context) {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders() {
            if(_context.Orders == null) {
                return NotFound();
            }
            return await _context.Orders.Include(x => x.Customer).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id) {
            if(_context.Orders == null) {
                return NotFound();
            }
            var order = await _context.Orders
                                    .Include(x => x.Customer)
                                    .Include(x => x.Orderlines)!
                                    .ThenInclude(x => x.Item)
                                    .SingleOrDefaultAsync(x => x.Id == id);

            if(order == null) {
                return NotFound();
            }

            return order;
        }

        [HttpGet("ok")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersWithStatusOk() {
            if(_context.Orders == null) {
                return NotFound();
            }
            //return await _context.Orders
            //                        .Where(x => x.Status == "OK")
            //                        .Include(x => x.Customer)
            //                        .ToListAsync();

            return await (from o in _context.Orders
                          where o.Status == "OK"
                          select o)
                          .ToListAsync();
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order) {
            if(id != order.Id) {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch(DbUpdateConcurrencyException) {
                if(!OrderExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("ok/{id}")]
        public async Task<IActionResult> SetOrderStatusToOk(int id, Order order) {
            order.Status = "OK";
            return await PutOrder(id, order);
        }
        [HttpPut("backorder/{id}")]
        public async Task<IActionResult> SetOrderStatusToBackorder(int id, Order order) {
            order.Status = "BACKORDER";
            return await PutOrder(id, order);
        }
        [HttpPut("closed/{id}")]
        public async Task<IActionResult> SetOrderStatusToClosed(int id, Order order) {
            order.Status = "CLOSED";
            return await PutOrder(id, order);
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order) {
            if(_context.Orders == null) {
                return Problem("Entity set 'AppDbContext.Orders'  is null.");
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id) {
            if(_context.Orders == null) {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if(order == null) {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id) {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
