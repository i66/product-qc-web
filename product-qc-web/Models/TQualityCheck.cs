using System;
using System.Collections.Generic;

namespace product_qc_web.Models
{
    public partial class TQualityCheck
    {
        public decimal WorkOrderNum { get; set; }
        public decimal MachineNum { get; set; }
        public DateTime QcFinishedTime { get; set; }

        public virtual TManufacture TManufacture { get; set; }
    }
}
