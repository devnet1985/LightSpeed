using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI_AWS.DataClasses
{
    public class Attributes
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class Shop
    {
        public string shopID { get; set; }
        public string name { get; set; }
        public string serviceRate { get; set; }
        public string timeZone { get; set; }
        public string taxLabor { get; set; }
        public string labelTitle { get; set; }
        public string labelMsrp { get; set; }
        public string archived { get; set; }
        public DateTime timeStamp { get; set; }
        public string companyRegistrationNumber { get; set; }
        public string vatNumber { get; set; }
        public string zebraBrowserPrint { get; set; }
        public string contactID { get; set; }
        public string taxCategoryID { get; set; }
        public string receiptSetupID { get; set; }
        public string ccGatewayID { get; set; }
        public string gatewayConfigID { get; set; }
        public string priceLevelID { get; set; }
    }

    public class Shopinfo
    {
        public Attributes __invalid_name__attributes { get; set; }
    public List<Shop> Shop { get; set; }
}





//public class Shopinfo
//    {
//        public attributes __invalid_name__attributes { get; set; }
//        public List<Shop> Shop { get; set; }
//    }
}