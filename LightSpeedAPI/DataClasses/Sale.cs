using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI.DataClasses
{
    public class Salereload
    {
        public string saleID { get; set; }
        public DateTime timeStamp { get; set; }
        public string discountPercent { get; set; }
        public string completed { get; set; }
        public string archived { get; set; }
        public string voided { get; set; }
        public string enablePromotions { get; set; }
        public string isTaxInclusive { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public DateTime completeTime { get; set; }
        public string referenceNumber { get; set; }
        public string referenceNumberSource { get; set; }
        public string tax1Rate { get; set; }
        public string tax2Rate { get; set; }
        public string change { get; set; }
        public string receiptPreference { get; set; }
        public string displayableSubtotal { get; set; }
        public string ticketNumber { get; set; }
        public string calcDiscount { get; set; }
        public string calcTotal { get; set; }
        public string calcSubtotal { get; set; }
        public string calcTaxable { get; set; }
        public string calcNonTaxable { get; set; }
        public string calcAvgCost { get; set; }
        public string calcFIFOCost { get; set; }
        public string calcTax1 { get; set; }
        public string calcTax2 { get; set; }
        public string calcPayments { get; set; }
        public string total { get; set; }
        public string totalDue { get; set; }
        public string displayableTotal { get; set; }
        public string balance { get; set; }
        public string customerID { get; set; }
        public string discountID { get; set; }
        public string employeeID { get; set; }
        public string quoteID { get; set; }
        public string registerID { get; set; }
        public string shipToID { get; set; }
        public string shopID { get; set; }
        public string taxCategoryID { get; set; }
        public string taxTotal { get; set; }
    }
    public class Attributes1
    {
        public string count { get; set; }
    }
    public class SaleObject
    {
        public Attributes __invalid_name__attributes { get; set; }
        public Salereload Sale { get; set; }
    }
}