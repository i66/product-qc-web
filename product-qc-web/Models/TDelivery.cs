using System;
using System.Collections.Generic;

namespace product_qc_web.Models
{
    public partial class TDelivery
    {
        public decimal WorkOrderNum { get; set; }
        public decimal MachineNum { get; set; }
        public string DeliveryDestination { get; set; }
        public string ExchangeReturnMalfunctionNote { get; set; }
        public DateTime LastModifiedTime { get; set; }

        public virtual TManufacture TManufacture { get; set; }
    }
}
