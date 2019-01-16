using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace product_qc_web.Models
{
    [ModelMetadataType(typeof(MetadataTDelivery))]
    public partial class TDelivery
    {
        [NotMapped]
        public DateTime QcFinishedTime { get; set; }

        [NotMapped]
        public string ProductName { get; set; }

        [NotMapped]
        public int Page { get; set; }
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
        [RegularExpression(@"^.{0,50}$", ErrorMessage = "字數(含標點符號)最多為50字")]
        public string DeliveryDestination { get; set; }

        [Display(Name = "狀態紀錄")]
        [RegularExpression(@"^.{0,255}$", ErrorMessage = "字數(含標點符號)最多為255字")]
        public string ExchangeReturnMalfunctionNote { get; set; }

    }
}
