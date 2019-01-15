using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace product_qc_web.Models
{
    [ModelMetadataType(typeof(MetadataTDelivery))]
    public partial class TDelivery
    {
        [NotMapped]
        public string ProductName { get; set; }

        [NotMapped]
        public DateTime QcFinishedTime { get; set; }
    }

    public class MetadataTDelivery
    {
        [Display(Name = "成品別")]
        public string ProductName { get; set; }

        [Display(Name = "完成日期")]
        public DateTime QcFinishedTime { get; set; }

        [Display(Name = "工單號碼")]
        public decimal WorkOrderNum { get; set; }

        [Display(Name = "編號")]
        public decimal MachineNum { get; set; }

        [Display(Name = "出貨案場")]
        public string DeliveryDestination { get; set; }

        [Display(Name = "換貨/退貨/故障紀錄")]
        public string ExchangeReturnMalfunctionNote { get; set; }

    }
}
