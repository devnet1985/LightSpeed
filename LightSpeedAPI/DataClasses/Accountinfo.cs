using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightSpeedAPI_AWS
{
    public class Attributes
    {
        public string count { get; set; }
    }

    public class Attributes2
    {
        public string href { get; set; }
    }

    public class Link
    {
        public Attributes2 __invalid_name__attributes { get; set; }
    }

    public class Account
    {
        public string accountID { get; set; }
        public string name { get; set; }
        public Link link { get; set; }
    }

    public class Accountinfo
    {
        public Attributes __invalid_name__attributes { get; set; }
        public Account Account { get; set; }
    }
}


    



