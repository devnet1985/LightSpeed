using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI_AWS.DataClasses
{



    public class Attributes_itemmatrix
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class ItemPrice_itemmatrix
    {
        public string amount { get; set; }
        public string useTypeID { get; set; }
        public string useType { get; set; }
    }

    public class Prices_itemmatrix
    {
        public List<ItemPrice_itemmatrix> ItemPrice { get; set; }
    }

    public class ItemMatrix
    {
        public string itemMatrixID { get; set; }
        public string description { get; set; }
        public string tax { get; set; }
        public string defaultCost { get; set; }
        public string itemType { get; set; }
        public string serialized { get; set; }
        public string modelYear { get; set; }
        public string archived { get; set; }
        public DateTime timeStamp { get; set; }
        public string itemAttributeSetID { get; set; }
        public string manufacturerID { get; set; }
        public string categoryID { get; set; }
        public string defaultVendorID { get; set; }
        public string taxClassID { get; set; }
        public string seasonID { get; set; }
        public string departmentID { get; set; }
        public Prices_itemmatrix Prices { get; set; }
        public object attribute1Values { get; set; }
        public object attribute2Values { get; set; }
 
    }
    public class ItemMatrixinfo
    {
        public Attributes_itemmatrix __invalid_name__attributes { get; set; }
        public ItemMatrix ItemMatrix { get; set; }
    }
}