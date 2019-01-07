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
            ViewData["CurrentServerTime"] = DateTime.Today.ToString("s");
            return View();
        }

        // POST: TManufactures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TManufacture tManufacture)
        {
            if (ModelState.IsValid)
            {
                saveChange(tManufacture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName", tManufacture.ProductCode);
            return View(tManufacture);
        }

        private List<int> parsingMachineNum(string machineNumList)
        {
            if (string.IsNullOrWhiteSpace(machineNumList))
                return new List<int>();
            string[] parserResult = machineNumList.Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<int> result = new List<int>();
            foreach (string numData in parserResult)
            {
                int temp;
                if (!int.TryParse(numData.Trim(),out temp))
                    continue;
                if (result.Contains(temp))
                    continue;
                result.Add(temp);
            }
            return result;
        }

        private void saveChange(TManufacture tManufacture)
        {
            List<int> machinieNumList = parsingMachineNum(tManufacture.MachineNumList);
            foreach (int machineNum in machinieNumList)
            {
                TManufacture manufacture = new TManufacture();
                manufacture.ProductCode = tManufacture.ProductCode;
                manufacture.WorkOrderNum = tManufacture.WorkOrderNum;
                manufacture.MachineNum = machineNum;
                _context.Add(manufacture);

                TQualityCheck qualityCheck = new TQualityCheck();
                qualityCheck.TManufacture = manufacture;
                qualityCheck.QcFinishedTime = tManufacture.QcFinishedTime;
                _context.Add<TQualityCheck>(qualityCheck);

                TDelivery delivery = new TDelivery();
                delivery.TManufacture = manufacture;
                _context.Add<TDelivery>(delivery);
            }
        }

        private bool TManufactureExists(decimal id)
        {
            return _context.TManufacture.Any(e => e.WorkOrderNum == id);
        }
    }
}
