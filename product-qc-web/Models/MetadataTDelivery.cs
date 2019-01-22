using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace product_qc_web.Models
{
    public enum DeliveryField { none = 0, QcFinishedTime, WorkOrderNum, DeliveryDestination };

    [ModelMetadataType(typeof(MetadataTDelivery))]
    public partial class TDelivery
    {
        private const int MAX_DELIVERY_DESTINATION_SHOW_LENGTH = 10;
        private const int DELIVERY_DESTINATION__LEFT_RESERVED = 6;
        private const int DELIVERY_DESTINATION_RIGHT_RESERVED = 3;

        private const int MAX_NOTE_SHOW_LENGTH = 20;
        private const int NOTE_LEFT_RESERVED = 18;

        private const string NEGLECT_TOKEN = "...";
        private string shortDeliveryDestinationStr(string deliveryDestination)
        {
            if (string.IsNullOrWhiteSpace(deliveryDestination) || 
                deliveryDestination.Length < MAX_DELIVERY_DESTINATION_SHOW_LENGTH)
                return deliveryDestination;

            string result = deliveryDestination.Substring(0, DELIVERY_DESTINATION__LEFT_RESERVED) + NEGLECT_TOKEN
                + deliveryDestination.Substring(deliveryDestination.Length - DELIVERY_DESTINATION_RIGHT_RESERVED);
            return result;
        }

        private string shortNoteStr(string exchangeReturnMalfunctionNote)
        {
            if (string.IsNullOrWhiteSpace(exchangeReturnMalfunctionNote) ||
                exchangeReturnMalfunctionNote.Length < MAX_NOTE_SHOW_LENGTH)
                return exchangeReturnMalfunctionNote;

            string result = exchangeReturnMalfunctionNote.Substring(0, NOTE_LEFT_RESERVED) + NEGLECT_TOKEN;
            return result;
        }

        [NotMapped]
        public string ShortDeliveryDestination
        {
            get { return shortDeliveryDestinationStr(DeliveryDestination); }
        }
        
        [NotMapped]
        public string ShortExchangeReturnMalfunctionNote
        {
            get { return shortNoteStr(ExchangeReturnMalfunctionNote); }
        }

        [NotMapped]
        public DateTime QcFinishedTime { get; set; }

        [NotMapped]
        public string ProductName { get; set; }

        [NotMapped]
        public int Page { get; set; }

        [NotMapped]
        public DeliveryField SortField { get; set; }

        [NotMapped]
        public bool IsAsc { get; set; }
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

        [Display(Name = "異動時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public DateTime LastModifiedTime { get; set; }

        [Display(Name = "狀態紀錄")]
        [RegularExpression(@"^.{0,255}$", ErrorMessage = "字數(含標點符號)最多為255字")]
        public string ExchangeReturnMalfunctionNote { get; set; }

    }
}
