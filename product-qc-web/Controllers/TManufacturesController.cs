using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            ViewData["SuccessCount"] = TempData["SuccessCount"];
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName");
            ViewData["CurrentServerTime"] = DateTime.Today.ToString("s");
            return View();
        }

        // POST: TManufactures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TManufacture tManufacture)
        {
            try
            {
                if (!ModelState.IsValid)
                    return errorResponse(tManufacture, "資料有錯誤！！");

                List<int> machinieNumList = parsingMachineNum(tManufacture.MachineNumList);
                string errorMsg;
                if (!checkDataExist(machinieNumList, tManufacture.WorkOrderNum, out errorMsg))
                    return errorResponse(tManufacture, errorMsg);

                saveChange(tManufacture, machinieNumList);
                await _context.SaveChangesAsync();

                TempData["SuccessCount"] = machinieNumList.Count();
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                return errorResponse(tManufacture, "伺服器端出現可怕錯誤！" + ex.ToString());
            }
        }

        private IActionResult errorResponse(TManufacture tManufacture, string errorMsg)
        {
            ViewData["ErrorMsg"] = "匯入失敗：" + errorMsg;
            ViewData["ProductCode"] = new SelectList(_context.TProduct, "ProductCode", "ProductName", tManufacture.ProductCode);
            ViewData["CurrentServerTime"] = tManufacture.QcFinishedTime.ToString("s");
            return View(tManufacture);
        }

        private bool checkDataExist(List<int> machineNumList, decimal workOrderNum, out string errorMsg)
        {
            errorMsg = string.Empty;
            foreach (int machineNum in machineNumList)
            {
                bool isDataExist = _context.TManufacture.
                    Where(x => x.MachineNum == machineNum 
                    && x.WorkOrderNum == workOrderNum).Any();
                if (!isDataExist)
                    continue;
                errorMsg = string.Format("編號{1} 已經存在於資料庫了！",
                    workOrderNum, machineNum);
                return false;
            }
            return true;
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

        private void saveChange(TManufacture tManufacture, List<int> machinieNumList)
        {
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
