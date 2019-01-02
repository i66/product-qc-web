using System;
using System.Collections.Generic;

namespace product_qc_web.Models
{
    public partial class TManufacture
    {
        public decimal ProductCode { get; set; }
        public decimal WorkOrderNum { get; set; }
        public decimal MachineNum { get; set; }

        public virtual TProduct ProductCodeNavigation { get; set; }
        public virtual TDelivery TDelivery { get; set; }
        public virtual TQualityCheck TQualityCheck { get; set; }
    }
}
