using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI_AWS.DataClasses
{

    public class Attribute
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class taxCategory
    {
        public string taxCategoryID { get; set; }
        public string isTaxInclusive { get; set; }
        public string tax1Name { get; set; }
        public string tax2Name { get; set; }
        public string tax1Rate { get; set; }
        public string tax2Rate { get; set; }
        public DateTime timeStamp { get; set; }
    }

    public class Saleline
    {
        public string saleLineID { get; set; }
        public DateTime createTime { get; set; }
        public DateTime timeStamp { get; set; }
        public string unitQuantity { get; set; }
        public string unitPrice { get; set; }
        public string normalUnitPrice { get; set; }
        public string discountAmount { get; set; }
        public string discountPercent { get; set; }
        public string avgCost { get; set; }
        public string fifoCost { get; set; }
        public string tax { get; set; }
        public string tax1Rate { get; set; }
        public string tax2Rate { get; set; }
        public string isLayaway { get; set; }
        public string isWorkorder { get; set; }
        public string isSpecialOrder { get; set; }
        public string displayableSubtotal { get; set; }
        public string displayableUnitPrice { get; set; }
        public string calcLineDiscount { get; set; }
        public string calcTransactionDiscount { get; set; }
        public string calcTotal { get; set; }
        public string calcSubtotal { get; set; }
        public string calcTax1 { get; set; }
        public string calcTax2 { get; set; }
        public string taxClassID { get; set; }
        public string customerID { get; set; }
        public string discountID { get; set; }
        public string employeeID { get; set; }
        public string itemID { get; set; }
        public string noteID { get; set; }
        public string parentSaleLineID { get; set; }
        public string shopID { get; set; }
        public string taxCategoryID { get; set; }
        public string saleID { get; set; }
        public taxCategory TaxCategory { get; set; }
    }

    public class SaleLineinfo
    {
        public Attribute __invalid_name__attributes { get; set; }
        public List<Saleline> SaleLine { get; set; }
    }


    public class Salelininfromation
    {
        public Attribute __invalid_name__attributes { get; set; }
        public Saleline SaleLine { get; set; }
    }

}