using System;
using System.Collections.Generic;

namespace product_qc_web.Models
{
    public partial class TProduct
    {
        public TProduct()
        {
            TManufacture = new HashSet<TManufacture>();
        }

        public string ProductName { get; set; }
        public decimal ProductCode { get; set; }

        public virtual ICollection<TManufacture> TManufacture { get; set; }
    }
}
