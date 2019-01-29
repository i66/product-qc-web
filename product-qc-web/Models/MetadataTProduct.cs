using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace product_qc_web.Models
{
    [ModelMetadataType(typeof(MetadataTProduct))]
    public partial class TProduct
    {
    }

    public class MetadataTProduct
    {
        [Display(Name = "成品")]
        public string ProductName { get; set; }
    }
}
