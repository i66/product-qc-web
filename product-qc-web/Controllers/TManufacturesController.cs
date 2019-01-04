using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using product_qc_web.Models;

namespace product_qc_web.Controllers
{
    public class TManufacturesController : Controller
    {
        private readonly HexsaveContext _context;

        public TManufacturesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: TManufactures
        public async Task<IActionResult> Index()
        {
            var hexsaveContext = _context.TManufacture.Include(t => t.ProductCodeNavigation);
            return View(await hexsaveContext.ToListAsync());
        }

        // GET: TManufactures/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tManufacture = await _context.TManufacture
                .Include(t => t.ProductCodeNavigation)
                .FirstOrDefaultAsync(m => m.WorkOrderNum == id);
            if (tManufacture == null)
            {
                return NotFound();
            }

            return View(tManufacture);
        }

        // GET: TManufactures/Create
        public IActionResult Create()
        {
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName");
            return View();
        }

        // POST: TManufactures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductCode,WorkOrderNum,MachineNum")] TManufacture tManufacture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tManufacture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName", tManufacture.ProductCode);
            return View(tManufacture);
        }

        // GET: TManufactures/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tManufacture = await _context.TManufacture.FindAsync(id);
            if (tManufacture == null)
            {
                return NotFound();
            }
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName", tManufacture.ProductCode);
            return View(tManufacture);
        }

        // POST: TManufactures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("ProductCode,WorkOrderNum,MachineNum")] TManufacture tManufacture)
        {
            if (id != tManufacture.WorkOrderNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tManufacture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TManufactureExists(tManufacture.WorkOrderNum))
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
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName", tManufacture.ProductCode);
            return View(tManufacture);
        }

        // GET: TManufactures/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tManufacture = await _context.TManufacture
                .Include(t => t.ProductCodeNavigation)
                .FirstOrDefaultAsync(m => m.WorkOrderNum == id);
            if (tManufacture == null)
            {
                return NotFound();
            }

            return View(tManufacture);
        }

        // POST: TManufactures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var tManufacture = await _context.TManufacture.FindAsync(id);
            _context.TManufacture.Remove(tManufacture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TManufactureExists(decimal id)
        {
            return _context.TManufacture.Any(e => e.WorkOrderNum == id);
        }
    }
}
