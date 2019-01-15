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
    public class TDeliveriesController : Controller
    {
        private readonly HexsaveContext _context;

        public TDeliveriesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: TDeliveries
        public async Task<IActionResult> Index(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                productName = "大主機";

            decimal productCode = (from s in _context.TProduct
                                   where s.ProductName == productName
                                   select s.ProductCode).FirstOrDefault();

            ViewData["ProductName"] = productName;

            var data = (from m in _context.TManufacture
                        join d in _context.TDelivery on new { m.WorkOrderNum, m.MachineNum } equals new { d.WorkOrderNum, d.MachineNum }
                        join q in _context.TQualityCheck on new { m.WorkOrderNum, m.MachineNum } equals new { q.WorkOrderNum, q.MachineNum }
                        where m.ProductCode == productCode
                        select new TDelivery() {
                            QcFinishedTime = q.QcFinishedTime,
                            WorkOrderNum = d.WorkOrderNum,
                            MachineNum = d.MachineNum,
                            DeliveryDestination = d.DeliveryDestination,
                            ExchangeReturnMalfunctionNote = d.ExchangeReturnMalfunctionNote,
                            TManufacture = m
                        });

            return View(await data.ToListAsync());
        }

        // GET: TDeliveries/Details/5
        public async Task<IActionResult> Details(decimal? workOrderNum, decimal? machineNum)
        {
            if (workOrderNum == null || machineNum == null)
            {
                return NotFound();
            }

            var tDelivery = await _context.TDelivery
                .Include(t => t.TManufacture)
                .FirstOrDefaultAsync( m =>  m.WorkOrderNum == workOrderNum && m.MachineNum == machineNum);
            if (tDelivery == null)
            {
                return NotFound();
            }

            return View(tDelivery);
        }

        // GET: TDeliveries/Edit/5
        public async Task<IActionResult> Edit(decimal? workOrderNum, decimal? machineNum)
        {
            if (workOrderNum == null || machineNum == null)
            {
                return NotFound();
            }

            var tDelivery = await _context.TDelivery.FindAsync(workOrderNum, machineNum);
            if (tDelivery == null)
            {
                return NotFound();
            }
            ViewData["WorkOrderNum"] = new SelectList(_context.TManufacture, "WorkOrderNum", "WorkOrderNum", tDelivery.WorkOrderNum);
            return View(tDelivery);
        }

        // POST: TDeliveries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("WorkOrderNum,MachineNum,DeliveryDestination,ExchangeReturnMalfunctionNote")] TDelivery tDelivery)
        {
            if (id != tDelivery.WorkOrderNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tDelivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TDeliveryExists(tDelivery.WorkOrderNum))
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
            ViewData["WorkOrderNum"] = new SelectList(_context.TManufacture, "WorkOrderNum", "WorkOrderNum", tDelivery.WorkOrderNum);
            return View(tDelivery);
        }

        // GET: TDeliveries/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            return NotFound();
        }

        // POST: TDeliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            return NotFound();
        }

        private bool TDeliveryExists(decimal id)
        {
            return _context.TDelivery.Any(e => e.WorkOrderNum == id);
        }
    }
}
