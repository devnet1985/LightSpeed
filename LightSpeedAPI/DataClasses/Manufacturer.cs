using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LightSpeedAPI.DataClasses
{

    public class Attributes
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class Manufacturer
    {
        public string manufacturerID { get; set; }
        public string name { get; set; }
        public DateTime createTime { get; set; }
        public DateTime timeStamp { get; set; }
    }
    public class ManufacturerObject
    {
      [DataMember(Name = "_invalid_name__@attributes")]
      public Attributes attributes { get; set; }
      public Manufacturer Manufacturer { get; set; }
     }
}