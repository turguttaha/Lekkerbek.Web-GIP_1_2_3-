using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Controllers
{
    public class TimeSlotsController : Controller
    {
        private readonly LekkerbekContext _context;

        public TimeSlotsController(LekkerbekContext context)
        {
            _context = context;
        }

        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
              return View(await _context.TimeSlot.ToListAsync());
        }

        // GET: TimeSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TimeSlot == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // GET: TimeSlots/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TimeSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsAvailable,StartTimeSlot,EndTimeSlot,Chef1Id,Chef2Id")] TimeSlot timeSlot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timeSlot);
        }

        // GET: TimeSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TimeSlot == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            return View(timeSlot);
        }

        // POST: TimeSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsAvailable,StartTimeSlot,EndTimeSlot,Chef1Id,Chef2Id")] TimeSlot timeSlot)
        {
            if (id != timeSlot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeSlotExists(timeSlot.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(timeSlot);
        }

        // GET: TimeSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TimeSlot == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // POST: TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TimeSlot == null)
            {
                return Problem("Entity set 'LekkerbekContext.TimeSlot'  is null.");
            }
            var timeSlot = await _context.TimeSlot.FindAsync(id);
            if (timeSlot != null)
            {
                _context.TimeSlot.Remove(timeSlot);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeSlotExists(int id)
        {
          return _context.TimeSlot.Any(e => e.Id == id);
        }
    }
}
