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
                return RedirectToAction(nameof(Create));
            }
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName", tManufacture.ProductCode);
            return View(tManufacture);
        }

        private bool TManufactureExists(decimal id)
        {
            return _context.TManufacture.Any(e => e.WorkOrderNum == id);
        }
    }
}
