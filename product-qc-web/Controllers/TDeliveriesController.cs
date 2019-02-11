using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace product_qc_web.Controllers
{
    [Authorize]
    public class TDeliveriesController : Controller
    {
        private readonly HexsaveContext _context;
        private const int TAB_PAGE_MAX_DATA = 10;
        private const int DESTINATION_WORD_LENGTH = 50;
        private const int ERM_WORD_LENGTH = 255;
        private const string DEFAULT_PRODUCT_NAME = "大主機";
        private const int DEFAULT_PAGE = 1;

        public TDeliveriesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: TDeliveries
        public async Task<IActionResult> Index(string productName, DeliveryField sortField, DeliveryField lastSortField, bool isAsc, int? page)
        {
            if (string.IsNullOrWhiteSpace(productName))
                productName = DEFAULT_PRODUCT_NAME;

            Func<TDelivery, Object> sortClause = sortingCondition(sortField);
            bool isOrderAsc = getOrder(sortField, lastSortField, isAsc);

            decimal productCode = (from s in _context.TProduct
                                   where s.ProductName == productName
                                   select s.ProductCode).FirstOrDefault();

            int goodPage = page ?? DEFAULT_PAGE;
            var data = (from m in _context.TManufacture
                        join d in _context.TDelivery on new { m.WorkOrderNum, m.MachineNum } equals new { d.WorkOrderNum, d.MachineNum }
                        join q in _context.TQualityCheck on new { m.WorkOrderNum, m.MachineNum } equals new { q.WorkOrderNum, q.MachineNum }
                        where m.ProductCode == productCode
                        select new TDelivery() {
                            ProductName = productName,
                            Page = goodPage,
                            SortField = sortField,
                            IsAsc = isOrderAsc,
                            QcFinishedTime = q.QcFinishedTime,
                            WorkOrderNum = d.WorkOrderNum,
                            MachineNum = d.MachineNum,
                            DeliveryDestination = d.DeliveryDestination,
                            ExchangeReturnMalfunctionNote = d.ExchangeReturnMalfunctionNote,
                            LastModifiedTime = d.LastModifiedTime,
                            TManufacture = m
                        });

            List<TDelivery> sortingData;
            if (isOrderAsc)
                sortingData = data.OrderBy(sortClause).ToList();
            else
                sortingData = data.OrderByDescending(sortClause).ToList();

            ViewData["PageNumber"] = (goodPage - 1) * TAB_PAGE_MAX_DATA;
            return View(await sortingData.ToPagedListAsync(goodPage, TAB_PAGE_MAX_DATA));
        }

        private bool getOrder(DeliveryField sortField, DeliveryField lastSortField, bool isAsc)
        {
            if (DeliveryField.none == sortField && DeliveryField.none == lastSortField)
                return false;

            if (sortField == lastSortField)
                return !isAsc;
            else
                return isAsc;

        }

        private Func<TDelivery, Object> sortingCondition(DeliveryField sortField)
        {
            Func<TDelivery, Object> orderClause;

            switch (sortField)
            {
                case DeliveryField.QcFinishedTime:
                    orderClause = TDelivery => TDelivery.QcFinishedTime;
                    break;
                case DeliveryField.WorkOrderNum:
                    orderClause = TDelivery => TDelivery.WorkOrderNum;
                    break;
                case DeliveryField.DeliveryDestination:
                    orderClause = TDelivery => TDelivery.DeliveryDestination;
                    break;
                case DeliveryField.none:
                default:
                    orderClause = TDelivery => TDelivery.QcFinishedTime;
                    break;
            }

            return orderClause;
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
        public async Task<IActionResult> Edit(string productName, DeliveryField sortField, bool isAsc, int? page, decimal? workOrderNum, decimal? machineNum)
        {
            if (workOrderNum == null || machineNum == null)
                return NotFound();

            int goodPage = page ?? DEFAULT_PAGE;

            var tDelivery = (from d in _context.TDelivery
                             join q in _context.TQualityCheck on new { d.WorkOrderNum, d.MachineNum } equals new { q.WorkOrderNum, q.MachineNum }
                             where d.WorkOrderNum == workOrderNum && d.MachineNum == machineNum
                             select new TDelivery()
                             {
                                 ProductName = productName,
                                 Page = goodPage,
                                 SortField = sortField,
                                 IsAsc = isAsc,
                                 QcFinishedTime = q.QcFinishedTime,
                                 WorkOrderNum = d.WorkOrderNum,
                                 MachineNum = d.MachineNum,
                                 DeliveryDestination = d.DeliveryDestination,
                                 ExchangeReturnMalfunctionNote = d.ExchangeReturnMalfunctionNote,
                                 LastModifiedTime = d.LastModifiedTime,
                                 TManufacture = null
                             }).FirstOrDefaultAsync();

            if (tDelivery == null)
            {
                return NotFound();
            }

            return View(await tDelivery);
        }

        // POST: TDeliveries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TDelivery tDelivery)
        {
            int page = tDelivery.Page == 0 ? DEFAULT_PAGE : tDelivery.Page;
            int strCountOfDestination = tDelivery.DeliveryDestination == null ? 0 : tDelivery.DeliveryDestination.Length;
            int strCountERM = tDelivery.ExchangeReturnMalfunctionNote == null ? 0 : tDelivery.ExchangeReturnMalfunctionNote.Length;
            string productName = string.IsNullOrWhiteSpace(tDelivery.ProductName) ? DEFAULT_PRODUCT_NAME : tDelivery.ProductName;

            if (tDelivery.WorkOrderNum < 1 || tDelivery.MachineNum < 1)
                return errorResponse(tDelivery, "伺服器端無工單號碼或編號!!");

            if (strCountOfDestination > DESTINATION_WORD_LENGTH)
                return errorResponse(tDelivery, "出貨案場字數須少於" + DESTINATION_WORD_LENGTH + "字!!");

            if (strCountERM > ERM_WORD_LENGTH)
                return errorResponse(tDelivery, "換貨/退貨/故障紀錄字數須少於" + ERM_WORD_LENGTH + "字!!");

            try
            {
                if (!ModelState.IsValid)
                    return errorResponse(tDelivery, "資料有錯誤！！");

                tDelivery.LastModifiedTime = DateTime.Now;
                 _context.Update(tDelivery);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { productName, sortField = tDelivery.SortField, isAsc = tDelivery.IsAsc, page });

            }
            catch(Exception ex)
            {
                return errorResponse(tDelivery, "伺服器端出現可怕錯誤！" + ex.ToString());
            }

        }

        private IActionResult errorResponse(TDelivery tDelivery, string errorMsg)
        {
            ViewData["ErrorMsg"] = "匯入失敗：" + errorMsg;
            return View(tDelivery);
        }

        // GET: TDeliveries/Delete/5
        public IActionResult Delete(decimal? id)
        {
            return NotFound();
        }

        // POST: TDeliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(decimal id)
        {
            return NotFound();
        }

        private bool TDeliveryExists(decimal id)
        {
            return _context.TDelivery.Any(e => e.WorkOrderNum == id);
        }
    }
}
