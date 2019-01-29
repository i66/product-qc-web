using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace product_qc_web.Models
{
    [ModelMetadataType(typeof(MetadataTManufacture))]
    public partial class TManufacture
    {
        [NotMapped]
        public DateTime QcFinishedTime { get; set; }

        [NotMapped]
        public string MachineNumList { get; set; }
    }

    public class MetadataTManufacture
    {
        [Display(Name = "成品別")]
        public decimal ProductCode { get; set; }

        [Required]
        [Display(Name = "工單號碼")]
        [RegularExpression(@"^[0|1|2|3|4]\d{11}$", ErrorMessage = "工單為12碼數字且首碼為0、1、2、3或4。")]
        public decimal WorkOrderNum { get; set; }

        [Required]
        [Display(Name = "編號")]
        public decimal MachineNum { get; set; }

        [Required]
        [Display(Name = "完成時間")]
        public DateTime QcFinishedTime { get; set; }

        [Required]
        [Display(Name = "編號清單")]
        [RegularExpression(@"^(\d{1,3},\s*)*(\d{1,3})$", ErrorMessage = "請以逗點分隔各編號，且編號最多三碼。")]
        public string MachineNumList { get; set; }
    }
}
