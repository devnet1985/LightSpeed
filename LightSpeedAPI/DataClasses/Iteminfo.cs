using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI_AWS.DataClasses
{
    //public class Iteminfo
    //{
    //}
    public class Attribut
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class ItemPrice
    {
        public string amount { get; set; }
        public string useTypeID { get; set; }
        public string useType { get; set; }
    }

    public class Prices
    {
        public List<ItemPrice> ItemPrice { get; set; }
    }

    public class Item
    {
        public string itemID { get; set; }
        public string systemSku { get; set; }
        public string defaultCost { get; set; }
        public string avgCost { get; set; }
        public string discountable { get; set; }
        public string tax { get; set; }
        public string archived { get; set; }
        public string itemType { get; set; }
        public string serialized { get; set; }
        public string description { get; set; }
        public string modelYear { get; set; }
        public string upc { get; set; }
        public string ean { get; set; }
        public string customSku { get; set; }
        public string manufacturerSku { get; set; }
        public DateTime createTime { get; set; }
        public DateTime timeStamp { get; set; }
        public string publishToEcom { get; set; }
        public string categoryID { get; set; }
        public string taxClassID { get; set; }
        public string departmentID { get; set; }
        public string itemMatrixID { get; set; }
        public string manufacturerID { get; set; }
        public string seasonID { get; set; }
        public string defaultVendorID { get; set; }
        public Prices Prices { get; set; }
        public string descriptionItemMatrix { get; set; }
        public string itemAttributeSetID { get; set; }
        public string name { get; set; }
        public string attributeName1 { get; set; }
        public string attributeName2 { get; set; }
        public string attribute1Values { get; set; }
        public string attribute2Values { get; set; }
        public string BrandName { get; set; }
        public string reorderPoint { get; set; }
        public string reorderLevel { get; set; }
        public string shopID { get; set; }
        public string qoh { get; set; }
        public string shop_0_reorderLevel { get; set; }
        public string shop_0_qoh { get; set; }
        public string shop_0_reorderPoint { get; set; }
        public string shop_1_qoh { get; set; }
        public string shop_1_reorderLevel { get; set; }
        public string shop_1_reorderPoint { get; set; }
        public string shop_2_qoh { get; set; }
        public string shop_2_reorderLevel { get; set; }
        public string shop_2_reorderPoint { get; set; }
        public string shop_3_qoh { get; set; }
        public string shop_3_reorderLevel { get; set; }
        public string shop_3_reorderPoint { get; set; }
        public string shop_4_qoh { get; set; }
        public string shop_4_reorderLevel { get; set; }
        public string shop_4_reorderPoint { get; set; }
        public string shop_5_qoh { get; set; }
        public string shop_5_reorderLevel { get; set; }
        public string shop_5_reorderPoint { get; set; }
        public string shop_6_qoh { get; set; }
        public string shop_6_reorderLevel { get; set; }
        public string shop_6_reorderPoint { get; set; }
        public string shop_7_qoh { get; set; }
        public string shop_7_reorderLevel { get; set; }
        public string shop_7_reorderPoint { get; set; }
        public string shop_8_qoh { get; set; }
        public string shop_8_reorderLevel { get; set; }
        public string shop_8_reorderPoint { get; set; }
        public string shop_9_qoh { get; set; }
        public string shop_9_reorderLevel { get; set; }
        public string shop_9_reorderPoint { get; set; }
        //public string shop_10_qoh { get; set; }
        //public string shop_10_reorderLevel { get; set; }
        //public string shop_10_reorderPoint { get; set; }
    }

    public class Iteminfo
    {
     public Attribut __invalid_name__attributes { get; set; }
     public List<Item> Item { get; set; }
    }


    public class itemsingleinfromation
    {
        public Attribut __invalid_name__attributes { get; set; }
        public Item Item { get; set; }
    }


}