﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace product_qc_web.Controllers
{
    public class TDeliveriesController : Controller
    {
        private const int TAB_PAGE_MAX_DATA = 10;
        private readonly HexsaveContext _context;
        private const int DESTINATION_WORD_LENGTH = 50;
        private const int ERM_WORD_LENGTH = 255;
        private const string DEFAULT_PRODUCT_NAME = "大主機";
        private const int DEFAULT_PAGE = 1;

        public TDeliveriesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: TDeliveries
        public async Task<IActionResult> Index(string productName, int? page)
        {
            if (string.IsNullOrWhiteSpace(productName))
                productName = DEFAULT_PRODUCT_NAME;

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
                            QcFinishedTime = q.QcFinishedTime,
                            WorkOrderNum = d.WorkOrderNum,
                            MachineNum = d.MachineNum,
                            DeliveryDestination = d.DeliveryDestination,
                            ExchangeReturnMalfunctionNote = d.ExchangeReturnMalfunctionNote,
                            TManufacture = m
                        });

            ViewData["PageNumber"] = (goodPage - 1) * TAB_PAGE_MAX_DATA;
            return View(await data.ToPagedListAsync(goodPage, TAB_PAGE_MAX_DATA));
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
        public async Task<IActionResult> Edit(string productName, int? page, decimal? workOrderNum, decimal? machineNum)
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
                                 QcFinishedTime = q.QcFinishedTime,
                                 WorkOrderNum = d.WorkOrderNum,
                                 MachineNum = d.MachineNum,
                                 DeliveryDestination = d.DeliveryDestination,
                                 ExchangeReturnMalfunctionNote = d.ExchangeReturnMalfunctionNote,
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

                _context.Update(tDelivery);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { productName, page });

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
