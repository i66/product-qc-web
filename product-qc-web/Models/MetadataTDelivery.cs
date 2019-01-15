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
        public DateTime QcFinishedTime { get; set; }
    }

    public class MetadataTDelivery
    {
        [Display(Name = "完成時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public DateTime QcFinishedTime { get; set; }

        [Display(Name = "工單號碼")]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = false)]
        public decimal WorkOrderNum { get; set; }

        [Display(Name = "編號")]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = false)]
        public decimal MachineNum { get; set; }

        [Display(Name = "出貨案場")]
        public string DeliveryDestination { get; set; }

        [Display(Name = "狀態紀錄")]
        public string ExchangeReturnMalfunctionNote { get; set; }

    }
}
