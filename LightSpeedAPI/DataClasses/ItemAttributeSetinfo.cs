using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI_AWS.DataClasses
{
    
    public class Attributes_ItemAttributeSetinfo
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class ItemAttributeSet
    {
        public string itemAttributeSetID { get; set; }
        public string name { get; set; }
        public string attributeName1 { get; set; }
        public string attributeName2 { get; set; }
        public string attributeName3 { get; set; }
        public string system { get; set; }
        public string archived { get; set; }
    }

    public class ItemAttributeSetinfo
    {
        public Attributes_ItemAttributeSetinfo __invalid_name__attributes { get; set; }
        public ItemAttributeSet ItemAttributeSet { get; set; }
    }



}