using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exam;
using Exam.Models;

namespace Exam.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleItem>>> GetScheduleItem()
        {
            return await _context.ScheduleItem.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleItem>> GetScheduleItem(int id)
        {
            var scheduleItem = await _context.ScheduleItem.FindAsync(id);

            if (scheduleItem == null)
            {
                return NotFound();
            }

            return scheduleItem;
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScheduleItem(int id, ScheduleItem scheduleItem)
        {
            if (id != scheduleItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(scheduleItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       
        [HttpPost]
        public async Task<ActionResult<ScheduleItem>> PostScheduleItem(ScheduleItem scheduleItem)
        {
            _context.ScheduleItem.Add(scheduleItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScheduleItem", new { id = scheduleItem.Id }, scheduleItem);
        }

        // DELETE: api/Schedule/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ScheduleItem>> DeleteScheduleItem(int id)
        {
            var scheduleItem = await _context.ScheduleItem.FindAsync(id);
            if (scheduleItem == null)
            {
                return NotFound();
            }

            _context.ScheduleItem.Remove(scheduleItem);
            await _context.SaveChangesAsync();

            return scheduleItem;
        }

        private bool ScheduleItemExists(int id)
        {
            return _context.ScheduleItem.Any(e => e.Id == id);
        }
    }
}
