using LightSpeedAPI_AWS.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using System.Data;
using LightSpeedAPI_AWS;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Timers;
using System.Globalization;
using Newtonsoft.Json;
using LightSpeedAPI.DataClasses;

namespace LightSpeedAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }


        [HttpGet]
        [Route("api/LightSpeed/InvoiceItemsInsert")]
        public IEnumerable<string> InvoiceItems()
        {
            var Result = AccesToken();
            RefreshToken refobj = new RefreshToken();
            refobj = new JavaScriptSerializer().Deserialize<RefreshToken>(Result);
            var accountinfo = AccountInformation(refobj.access_token);
            Accountinfo accobj = new Accountinfo();
            accobj = new JavaScriptSerializer().Deserialize<Accountinfo>(accountinfo);
            //Sales Info insertion
            var salesinfo = SalesInformation(refobj.access_token, accobj.Account.accountID);
            dynamic respose = JsonConvert.DeserializeObject(salesinfo);
            int count = Convert.ToInt32(respose["@attributes"]["count"]);

            if (count > 1)
            {
                Salesinfo slsobj = new Salesinfo();
                slsobj = new JavaScriptSerializer().Deserialize<Salesinfo>(salesinfo);
                if (slsobj.Sale != null)
                {
                    InsertInvoiceItems(slsobj);
                }
            }
            else
            {
                salessingle slsobj = new salessingle();
                slsobj = new JavaScriptSerializer().Deserialize<salessingle>(salesinfo);
                if (slsobj.Sale != null)
                {
                    InsertInvoiceItemssingle(slsobj);
                }
            }
            return new string[] { "Inseted Succesfully" };

            //var db = new CustomerEntities();
            //var customers = db.Customers.ToList();
            //return Ok(customers);
        }


        [HttpGet]
        [Route("api/LightSpeed/ShopItemsInsert")]
        public IEnumerable<string> ShopItems()
        {

            var Result = AccesToken();
            RefreshToken refobj = new RefreshToken();
            refobj = new JavaScriptSerializer().Deserialize<RefreshToken>(Result);
            var accountinfo = AccountInformation(refobj.access_token);
            Accountinfo accobj = new Accountinfo();
            accobj = new JavaScriptSerializer().Deserialize<Accountinfo>(accountinfo);
            //Sales Info insertion
            var shopinfo = ShopInformation(refobj.access_token, accobj.Account.accountID);
            Shopinfo shpobj = new Shopinfo();
            shpobj = new JavaScriptSerializer().Deserialize<Shopinfo>(shopinfo);
            InsertShopItems(shpobj);
            return new string[] { "Inseted Succesfully" };

            //var db = new CustomerEntities();
            //var customers = db.Customers.ToList();
            //return Ok(customers);
        }

        [HttpGet]
        [Route("api/LightSpeed/SalesLineItemsInsert")]
        public IEnumerable<string> SalesLineItems()
        {
            var Result = AccesToken();
            RefreshToken refobj = new RefreshToken();
            refobj = new JavaScriptSerializer().Deserialize<RefreshToken>(Result);
            var accountinfo = AccountInformation(refobj.access_token);
            Accountinfo accobj = new Accountinfo();
            accobj = new JavaScriptSerializer().Deserialize<Accountinfo>(accountinfo);
            var shoplineinfo = SaleLineInformation(refobj.access_token, accobj.Account.accountID);

            dynamic response = JsonConvert.DeserializeObject(shoplineinfo);

            int scount = Convert.ToInt32(response["@attributes"]["count"]);

            if (scount > 1)
            {
                SaleLineinfo shplineobj = new SaleLineinfo();
                shplineobj = new JavaScriptSerializer().Deserialize<SaleLineinfo>(shoplineinfo);
                if (shplineobj.SaleLine != null)
                {
                    for (int count = 0; count < shplineobj.SaleLine.Count; count++)
                    {
                        var Getcaluamt = GetCalctotal(refobj.access_token, accobj.Account.accountID, shplineobj.SaleLine[count].saleID);
                        shplineobj.SaleLine[count].calcTotal = Getcaluamt.Sale.calcTotal;
                    }
                }
                if (shplineobj.SaleLine != null)
                {
                    InsertSalesLineItems(shplineobj);
                }
            }
            else
            {
                Salelininfromation sallin = new Salelininfromation();
                sallin = new JavaScriptSerializer().Deserialize<Salelininfromation>(shoplineinfo);
                if (sallin.SaleLine != null)
                {
                    var Getcaluamt = GetCalctotal(refobj.access_token, accobj.Account.accountID, sallin.SaleLine.saleID);
                    sallin.SaleLine.calcTotal = Getcaluamt.Sale.calcTotal;
                }
                if (sallin.SaleLine != null)
                {
                    InsertSalesLineSingleItems(sallin);
                }
            }
            return new string[] { "Inseted Succesfully" };
            //var db = new CustomerEntities();
            //var customers = db.Customers.ToList();
            //return Ok(customers);
        }

        [HttpGet]
        [Route("api/Customer/InventoryItemsInsert")]
        public IEnumerable<string> InventoryItems()
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = dt.AddMinutes(-30);
            string CurrentDate = dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            string pastDate = dt1.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            // DateTime d3 = DateTime.Parse(dt1.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);
            int index1 = CurrentDate.IndexOf("+");
            int index2 = CurrentDate.Length - 1;
            int indeslastchr = CurrentDate.Length - 1;
            CurrentDate = CurrentDate.Substring(0, CurrentDate.IndexOf("+"));
            pastDate = pastDate.Substring(0, pastDate.IndexOf("+"));
            var Result = AccesToken();
            RefreshToken refobj = new RefreshToken();
            refobj = new JavaScriptSerializer().Deserialize<RefreshToken>(Result);
            var accountinfo = AccountInformation(refobj.access_token);
            Accountinfo accobj = new Accountinfo();
            accobj = new JavaScriptSerializer().Deserialize<Accountinfo>(accountinfo);
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + accobj.Account.accountID + "/Item.json?createTime=><," + pastDate + "," + CurrentDate);
            //    var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Item.json");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + refobj.access_token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    Iteminfo itemobj = new Iteminfo();
                    dynamic deserialized = JsonConvert.DeserializeObject(jsonResponseText);
                    int scount = Convert.ToInt32(deserialized["@attributes"]["count"]);
                    if (scount > 1)
                    {
                        var iteminfo = ItemInformation(refobj.access_token, accobj.Account.accountID);
                        if (iteminfo.Item != null)
                        {
                            InsertInventoryItems(iteminfo);
                        }
                    }
                    else
                    {
                        var iteminfosingle = ItemInformationsingle(refobj.access_token, accobj.Account.accountID);
                        if (iteminfosingle.Item != null)
                        {
                            InsertInventorySingleItems(iteminfosingle);
                        }
                    }
                }
            }
            //var shoplineinfo = SaleLineInformation(refobj.access_token, accobj.Account.accountID);
            return new string[] { "Inseted Succesfully" };
            //var db = new CustomerEntities();
            //var customers = db.Customers.ToList();
            //return Ok(customers);
        }


        private static void LightSwitchApiCalling()
        {
            var Result = AccesToken();
            RefreshToken refobj = new RefreshToken();
            refobj = new JavaScriptSerializer().Deserialize<RefreshToken>(Result);
            var accountinfo = AccountInformation(refobj.access_token);
            Accountinfo accobj = new Accountinfo();
            accobj = new JavaScriptSerializer().Deserialize<Accountinfo>(accountinfo);
            //Sales Info insertion
            var salesinfo = SalesInformation(refobj.access_token, accobj.Account.accountID);
            Salesinfo slsobj = new Salesinfo();
            slsobj = new JavaScriptSerializer().Deserialize<Salesinfo>(salesinfo);
            InsertInvoiceItems(slsobj);


            Task.Delay(50000);
            //Shop Insertion
            var shopinfo = ShopInformation(refobj.access_token, accobj.Account.accountID);
            Shopinfo shpobj = new Shopinfo();
            shpobj = new JavaScriptSerializer().Deserialize<Shopinfo>(shopinfo);
            InsertShopItems(shpobj);


            Task.Delay(50000);

            //Sales Lineinfo insertion
            var shoplineinfo = SaleLineInformation(refobj.access_token, accobj.Account.accountID);
            SaleLineinfo shplineobj = new SaleLineinfo();
            shplineobj = new JavaScriptSerializer().Deserialize<SaleLineinfo>(shoplineinfo);
            InsertSalesLineItems(shplineobj);

            Task.Delay(50000);

            var iteminfo = ItemInformation(refobj.access_token, accobj.Account.accountID);
            InsertInventoryItems(iteminfo);
        }

        private static DataSet GetInventoryItems()
        {
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            try
            {
                cnn.Open();
                string query = "select * from INVENTORY";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = cnn;
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                cnn.Close();
            }
            catch (Exception ex)
            {

            }

            return ds;
        }

        private static DataSet GetShopItems()
        {
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            try
            {
                cnn.Open();
                string query = "select * from SHOP";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = cnn;
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                cnn.Close();
            }
            catch (Exception ex)
            {
                //   return null;
                // MessageBox.Show("Can not open connection ! ");
            }

            return ds;
        }

        private static DataSet GetInvoiceItems()
        {
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            try
            {
                cnn.Open();
                string query = "select * from INVOICES";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = cnn;
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                cnn.Close();
            }
            catch (Exception ex)
            {
                //   return null;
                // MessageBox.Show("Can not open connection ! ");
            }

            return ds;
        }

        private static DataSet GetSalesLineItems()
        {
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            try
            {
                cnn.Open();
                string query = "select * from INVOICE_ITEM";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = cnn;
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                cnn.Close();

            }
            catch (Exception ex)
            {
                //   return null;
                // MessageBox.Show("Can not open connection ! ");
            }

            return ds;
        }

        private static string AccountInformation(string Token)
        {
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account.json");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    return jsonResponseText;

                }
            }
        }

        private static string AccesToken()
        {
            string URL = "https://cloud.lightspeedapp.com/oauth/access_token.php";
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(URL);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            Stream reqStream = webRequest.GetRequestStream();
            string postData = "refresh_token=fa257998516c36027d0fb93b9453c8dd722cfb3f&client_id=23eea8ec9d7195a76c989794df18643b114ab8d3b1942b6ba3cc94a3ea4a8218&client_secret=a02cd99c7525093c40a6364c7a4d6ecfe131f1fb9df3c7cb4a8a597845d8c463&grant_type=refresh_token";
            byte[] postArray = Encoding.ASCII.GetBytes(postData);
            reqStream.Write(postArray, 0, postArray.Length);
            reqStream.Close();
            StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream());
            var Result = sr.ReadToEnd();
            return Result;
        }

        private static string SalesInformation(string Token, string AccountId)
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = dt.AddMinutes(-30);
            string CurrentDate = dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            string pastDate = dt1.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            int index1 = CurrentDate.IndexOf("+");
            int index2 = CurrentDate.Length - 1;
            int indeslastchr = CurrentDate.Length - 1;
            CurrentDate = CurrentDate.Substring(0, CurrentDate.IndexOf("+"));
            pastDate = pastDate.Substring(0, pastDate.IndexOf("+"));
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Sale.json?completeTime=><," + pastDate + "," + CurrentDate);
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    return jsonResponseText;

                }
            }
        }

        private static string ShopInformation(string Token, string AccountId)
        {
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Shop.json");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    return jsonResponseText;
                }
            }
        }

        private static string SaleLineInformation(string Token, string AccountId)
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = dt.AddMinutes(-30);
            string CurrentDate = dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            string pastDate = dt1.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            // DateTime d3 = DateTime.Parse(dt1.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);
            int index1 = CurrentDate.IndexOf("+");
            int index2 = CurrentDate.Length - 1;
            int indeslastchr = CurrentDate.Length - 1;
            CurrentDate = CurrentDate.Substring(0, CurrentDate.IndexOf("+"));
            pastDate = pastDate.Substring(0, pastDate.IndexOf("+"));
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/SaleLine.json?createTime=><," + pastDate + "," + CurrentDate);
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    return jsonResponseText;
                }
            }
        }

        public SaleObject GetCalctotal(string Token, string AccountId, string saleLineID)
        {

            if (saleLineID != null && saleLineID != "0")
            {

                var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Sale/" + saleLineID + ".json");
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + Token);
                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        SaleObject itemobj = new SaleObject();
                        itemobj = new JavaScriptSerializer().Deserialize<SaleObject>(jsonResponseText);
                        return itemobj;
                    }
                }
            }
            else
            {
                SaleObject itemobj = new SaleObject();
                Salereload reload = new Salereload();
                reload.calcTotal = "";
                itemobj.Sale = reload;
                return itemobj;
            }

        }
        private static string ItemInformationRespose(string Token, string AccountId)
        {
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Shop.json");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    return jsonResponseText;
                }
            }
        }

        private static itemsingleinfromation ItemInformationsingle(string Token, string AccountId)
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = dt.AddMinutes(-30);
            string CurrentDate = dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            string pastDate = dt1.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            // DateTime d3 = DateTime.Parse(dt1.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);
            int index1 = CurrentDate.IndexOf("+");
            int index2 = CurrentDate.Length - 1;
            int indeslastchr = CurrentDate.Length - 1;
            CurrentDate = CurrentDate.Substring(0, CurrentDate.IndexOf("+"));
            pastDate = pastDate.Substring(0, pastDate.IndexOf("+"));
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Item.json?createTime=><," + pastDate + "," + CurrentDate);

            //    var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Item.json");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    itemsingleinfromation itemobj = new itemsingleinfromation();

                    dynamic deserialized = JsonConvert.DeserializeObject(jsonResponseText);
                    int scount = Convert.ToInt32(deserialized["@attributes"]["count"]);
                    if (scount > 0)
                    {
                        itemobj = JsonConvert.DeserializeObject<itemsingleinfromation>(jsonResponseText);
                        var C = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)deserialized).First).Count;
                        if (itemobj.Item != null)
                        {

                            var getmatrixinfo = ItemmatrixInformation(Token, AccountId, itemobj.Item.itemMatrixID);
                            if (getmatrixinfo.ItemMatrix != null)
                            {
                                itemobj.Item.descriptionItemMatrix = getmatrixinfo.ItemMatrix.description;
                                itemobj.Item.itemAttributeSetID = getmatrixinfo.ItemMatrix.itemAttributeSetID;
                                string Attribute1 = string.Empty;
                                string Attribute2 = string.Empty;
                                try
                                {
                                    Attribute1 = getmatrixinfo.ItemMatrix.attribute1Values.ToString().TrimStart('{').TrimStart('[').TrimEnd(']').TrimEnd('}').Replace("\r\n", "").Replace('"', ' ').Replace(" ", "").Trim();
                                }
                                catch (Exception)
                                {
                                }
                                try
                                {
                                    Attribute2 = getmatrixinfo.ItemMatrix.attribute2Values.ToString().TrimStart('{').TrimStart('[').TrimEnd(']').TrimEnd('}').Replace("\r\n", "").Replace('"', ' ').Replace(" ", "").Trim();
                                }
                                catch (Exception)
                                {
                                }
                                itemobj.Item.attribute1Values = Attribute1;
                                itemobj.Item.attribute2Values = Attribute2;
                            }
                            var getitemreload = Iteminfromation(Token, AccountId, itemobj.Item.itemID);
                            var getmanufacturerinfo = Manufacturersetinfromation(Token, AccountId, itemobj.Item.manufacturerID);
                            if (getitemreload.Item != null)
                            {

                                itemobj.Item.shop_0_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[0].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[0].reorderLevel;
                                itemobj.Item.shop_0_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[0].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[0].reorderPoint;
                                itemobj.Item.shop_0_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[0].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[0].qoh;

                                itemobj.Item.shop_1_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[1].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[1].reorderLevel;
                                itemobj.Item.shop_1_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[1].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[1].reorderPoint;
                                itemobj.Item.shop_1_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[1].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[1].qoh;

                                itemobj.Item.shop_2_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[2].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[2].reorderLevel;
                                itemobj.Item.shop_2_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[2].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[2].reorderPoint;
                                itemobj.Item.shop_2_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[2].shopID + "_" + */getitemreload.Item.ItemShops.ItemShop[2].qoh;

                                itemobj.Item.shop_3_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[3].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[3].reorderLevel;
                                itemobj.Item.shop_3_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[3].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[3].reorderPoint;
                                itemobj.Item.shop_3_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[3].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[3].qoh;

                                itemobj.Item.shop_4_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[4].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[4].reorderLevel;
                                itemobj.Item.shop_4_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[4].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[4].reorderPoint;
                                itemobj.Item.shop_4_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[4].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[4].qoh;

                                itemobj.Item.shop_5_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[5].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[5].reorderLevel;
                                itemobj.Item.shop_5_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[5].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[5].reorderPoint;
                                itemobj.Item.shop_5_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[5].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[5].qoh;

                                itemobj.Item.shop_6_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[6].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[6].reorderLevel;
                                itemobj.Item.shop_6_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[6].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[6].reorderPoint;
                                itemobj.Item.shop_6_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[6].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[6].qoh;

                                itemobj.Item.shop_7_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[7].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[7].reorderLevel;
                                itemobj.Item.shop_7_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[7].shopID + "_" + */getitemreload.Item.ItemShops.ItemShop[7].reorderPoint;
                                itemobj.Item.shop_7_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[7].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[7].qoh;


                                itemobj.Item.shop_8_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[8].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[8].reorderLevel;
                                itemobj.Item.shop_8_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[8].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[8].reorderPoint;
                                itemobj.Item.shop_8_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[8].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[8].qoh;



                                itemobj.Item.shop_9_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[9].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[9].reorderLevel;
                                itemobj.Item.shop_9_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[9].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[9].reorderPoint;
                                itemobj.Item.shop_9_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[9].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[9].qoh;

                                foreach (var item in getitemreload.Item.ItemShops.ItemShop)
                                {
                                    itemobj.Item.reorderLevel += "shop_" + item.shopID + "_" + item.reorderLevel + ',';
                                    itemobj.Item.reorderPoint += "shop_" + item.shopID + "_" + item.reorderPoint + ',';
                                    itemobj.Item.shopID += "shop_" + item.shopID + "_" + item.shopID + ',';
                                    itemobj.Item.qoh += "shop_" + item.shopID + "_" + item.qoh + ',';
                                }
                            }
                            if (getmanufacturerinfo.Manufacturer != null)
                            {
                                itemobj.Item.BrandName = getmanufacturerinfo.Manufacturer.name;
                            }
                            var getitemattributeinfo = ItemAttributeSetInformation(Token, AccountId, itemobj.Item.itemAttributeSetID);
                            if (getitemattributeinfo.ItemAttributeSet != null)
                            {
                                itemobj.Item.descriptionItemMatrix = getitemattributeinfo.ItemAttributeSet.name;
                                itemobj.Item.attributeName1 = getitemattributeinfo.ItemAttributeSet.attributeName1;
                                itemobj.Item.attributeName2 = getitemattributeinfo.ItemAttributeSet.attributeName2;
                            }

                        }
                        return itemobj;
                    }

                    return itemobj;
                }
            }
        }

        private static Iteminfo ItemInformation(string Token, string AccountId)
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = dt.AddMinutes(-30);
            string CurrentDate = dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            string pastDate = dt1.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            // DateTime d3 = DateTime.Parse(dt1.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);
            int index1 = CurrentDate.IndexOf("+");
            int index2 = CurrentDate.Length - 1;
            int indeslastchr = CurrentDate.Length - 1;
            CurrentDate = CurrentDate.Substring(0, CurrentDate.IndexOf("+"));
            pastDate = pastDate.Substring(0, pastDate.IndexOf("+"));
            var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Item.json?createTime=><," + pastDate + "," + CurrentDate);

            //    var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Item.json");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + Token);
            using (System.Net.WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    dynamic jsonResponseText = streamReader.ReadToEnd();
                    Iteminfo itemobj = new Iteminfo();
                    dynamic deserialized = JsonConvert.DeserializeObject(jsonResponseText);
                    itemobj = JsonConvert.DeserializeObject<Iteminfo>(jsonResponseText);
                    var C = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)deserialized).First).Count;
                    if (itemobj.Item != null)
                    {
                        for (int count = 0; count < itemobj.Item.Count; count++)
                        {
                            var getmatrixinfo = ItemmatrixInformation(Token, AccountId, itemobj.Item[count].itemMatrixID);
                            if (getmatrixinfo.ItemMatrix != null)
                            {
                                itemobj.Item[count].descriptionItemMatrix = getmatrixinfo.ItemMatrix.description;
                                itemobj.Item[count].itemAttributeSetID = getmatrixinfo.ItemMatrix.itemAttributeSetID;
                                string Attribute1 = string.Empty;
                                string Attribute2 = string.Empty;
                                try
                                {
                                    Attribute1 = getmatrixinfo.ItemMatrix.attribute1Values.ToString().TrimStart('{').TrimStart('[').TrimEnd(']').TrimEnd('}').Replace("\r\n", "").Replace('"', ' ').Replace(" ", "").Trim();
                                }
                                catch (Exception)
                                {
                                }
                                try
                                {
                                    Attribute2 = getmatrixinfo.ItemMatrix.attribute2Values.ToString().TrimStart('{').TrimStart('[').TrimEnd(']').TrimEnd('}').Replace("\r\n", "").Replace('"', ' ').Replace(" ", "").Trim();
                                }
                                catch (Exception)
                                {
                                }
                                itemobj.Item[count].attribute1Values = Attribute1;
                                itemobj.Item[count].attribute2Values = Attribute2;
                            }
                            var getitemreload = Iteminfromation(Token, AccountId, itemobj.Item[count].itemID);
                            var getmanufacturerinfo = Manufacturersetinfromation(Token, AccountId, itemobj.Item[count].manufacturerID);
                            if (getitemreload.Item != null)
                            {

                                itemobj.Item[count].shop_0_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[0].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[0].reorderLevel;
                                itemobj.Item[count].shop_0_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[0].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[0].reorderPoint;
                                itemobj.Item[count].shop_0_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[0].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[0].qoh;

                                itemobj.Item[count].shop_1_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[1].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[1].reorderLevel;
                                itemobj.Item[count].shop_1_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[1].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[1].reorderPoint;
                                itemobj.Item[count].shop_1_qoh =/* "shop_" + getitemreload.Item.ItemShops.ItemShop[1].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[1].qoh;

                                itemobj.Item[count].shop_2_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[2].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[2].reorderLevel;
                                itemobj.Item[count].shop_2_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[2].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[2].reorderPoint;
                                itemobj.Item[count].shop_2_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[2].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[2].qoh;

                                itemobj.Item[count].shop_3_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[3].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[3].reorderLevel;
                                itemobj.Item[count].shop_3_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[3].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[3].reorderPoint;
                                itemobj.Item[count].shop_3_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[3].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[3].qoh;

                                itemobj.Item[count].shop_4_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[4].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[4].reorderLevel;
                                itemobj.Item[count].shop_4_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[4].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[4].reorderPoint;
                                itemobj.Item[count].shop_4_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[4].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[4].qoh;

                                itemobj.Item[count].shop_5_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[5].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[5].reorderLevel;
                                itemobj.Item[count].shop_5_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[5].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[5].reorderPoint;
                                itemobj.Item[count].shop_5_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[5].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[5].qoh;

                                itemobj.Item[count].shop_6_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[6].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[6].reorderLevel;
                                itemobj.Item[count].shop_6_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[6].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[6].reorderPoint;
                                itemobj.Item[count].shop_6_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[6].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[6].qoh;

                                itemobj.Item[count].shop_7_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[7].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[7].reorderLevel;
                                itemobj.Item[count].shop_7_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[7].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[7].reorderPoint;
                                itemobj.Item[count].shop_7_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[7].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[7].qoh;


                                itemobj.Item[count].shop_8_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[8].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[8].reorderLevel;
                                itemobj.Item[count].shop_8_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[8].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[8].reorderPoint;
                                itemobj.Item[count].shop_8_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[8].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[8].qoh;



                                itemobj.Item[count].shop_9_reorderLevel = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[9].shopID + "_" + */getitemreload.Item.ItemShops.ItemShop[9].reorderLevel;
                                itemobj.Item[count].shop_9_reorderPoint = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[9].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[9].reorderPoint;
                                itemobj.Item[count].shop_9_qoh = /*"shop_" + getitemreload.Item.ItemShops.ItemShop[9].shopID + "_" +*/ getitemreload.Item.ItemShops.ItemShop[9].qoh;

                                foreach (var item in getitemreload.Item.ItemShops.ItemShop)
                                {
                                    itemobj.Item[count].reorderLevel += "shop_" + item.shopID + "_" + item.reorderLevel + ',';
                                    itemobj.Item[count].reorderPoint += "shop_" + item.shopID + "_" + item.reorderPoint + ',';
                                    itemobj.Item[count].shopID += "shop_" + item.shopID + "_" + item.shopID + ',';
                                    itemobj.Item[count].qoh += "shop_" + item.shopID + "_" + item.qoh + ',';
                                }
                            }
                            if (getmanufacturerinfo.Manufacturer != null)
                            {
                                itemobj.Item[count].BrandName = getmanufacturerinfo.Manufacturer.name;
                            }
                            var getitemattributeinfo = ItemAttributeSetInformation(Token, AccountId, itemobj.Item[count].itemAttributeSetID);
                            if (getitemattributeinfo.ItemAttributeSet != null)
                            {
                                itemobj.Item[count].descriptionItemMatrix = getitemattributeinfo.ItemAttributeSet.name;
                                itemobj.Item[count].attributeName1 = getitemattributeinfo.ItemAttributeSet.attributeName1;
                                itemobj.Item[count].attributeName2 = getitemattributeinfo.ItemAttributeSet.attributeName2;
                            }
                        }
                    }
                    return itemobj;
                }
            }
        }

        public static ItemReload Iteminfromation(string Token, string AccountId, string itemID)
        {
            if (itemID != null)
            {
                var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Item.json?load_relations=%5B%22ItemShops%22%5D&itemID=" + itemID);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + Token);
                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        ItemReload itemobj = new ItemReload();
                        itemobj = new JavaScriptSerializer().Deserialize<ItemReload>(jsonResponseText);
                        return itemobj;
                    }
                }
            }
            else
            {
                ItemReload itemobj = new ItemReload();
                List<ItemShop> obj = new List<ItemShop>();
                ItemShop man = new ItemShop();
                man.reorderLevel = null;
                man.reorderPoint = null;
                obj.Add(man);
                itemobj.Item.ItemShops.ItemShop = obj;
                return itemobj;
            }

        }


        private static ItemMatrixinfo ItemmatrixInformation(string Token, string AccountId, string itemMatrixID)
        {

            if (itemMatrixID != null && itemMatrixID != "")
            {
                var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/ItemMatrix.json?itemMatrixID=" + itemMatrixID);

                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + Token);
                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        ItemMatrixinfo itemobj = new ItemMatrixinfo();
                        //itemobj = new JavaScriptSerializer().Deserialize<ItemMatrixinfo>(jsonResponseText);
                        itemobj = JsonConvert.DeserializeObject<ItemMatrixinfo>(jsonResponseText);

                        return itemobj;
                    }
                }
            }
            return null;
        }

        private static ItemAttributeSetinfo ItemAttributeSetInformation(string Token, string AccountId, string itemAttriubuteID)
        {
            if (itemAttriubuteID != null)
            {
                var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/ItemAttributeSet.json?itemAttributeSetID=" + itemAttriubuteID);

                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + Token);
                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        ItemAttributeSetinfo itemobj = new ItemAttributeSetinfo();
                        itemobj = new JavaScriptSerializer().Deserialize<ItemAttributeSetinfo>(jsonResponseText);
                        return itemobj;
                    }
                }
            }
            else
            {
                ItemAttributeSetinfo itemobj = new ItemAttributeSetinfo();
                itemobj.ItemAttributeSet = null;
                return itemobj;
            }
        }


        public static ManufacturerObject Manufacturersetinfromation(string Token, string AccountId, string manufacturerID)
        {
            if (manufacturerID != null)
            {
                var request = System.Net.HttpWebRequest.Create("https://api.lightspeedapp.com/API/Account/" + AccountId + "/Manufacturer.json?manufacturerID=" + manufacturerID);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + Token);
                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        ManufacturerObject itemobj = new ManufacturerObject();
                        // itemobj = JsonConvert.DeserializeObject<ManufacturerObject>(jsonResponseText);
                        itemobj = new JavaScriptSerializer().Deserialize<ManufacturerObject>(jsonResponseText);
                        return itemobj;
                    }
                }
            }
            else
            {
                ManufacturerObject itemobj = new ManufacturerObject();
                Manufacturer man = new Manufacturer();
                man.name = null;
                itemobj.Manufacturer = man;
                return itemobj;
            }
        }

        private static void InsertInvoiceItemssingle(salessingle salaesitems)
        {

            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            var saleID = salaesitems.Sale.saleID;

            if (saleID != "")
            {
                string query1 = "select SALE_ID,EMPLOYEE_ID,CUSTOMER_ID,STORE_ID,DISCOUNT_PERC from INVOICES where SALE_ID=" + salaesitems.Sale.saleID;

                MySqlCommand cmd1 = new MySqlCommand(query1);
                cmd1.Connection = cnn;
                DataSet dsn = new DataSet();
                MySqlDataAdapter adp1 = new MySqlDataAdapter();
                adp1.SelectCommand = cmd1;
                adp1.Fill(dsn);
                var ccoun = dsn.Tables[0].Rows.Count;
                if (ccoun == 0)
                {
                    try
                    {
                        cnn.Open();
                        string query = "Insert into INVOICES(SALE_ID,TIME_STAMP,EMPLOYEE_ID,CUSTOMER_ID,STORE_ID,DISCOUNT_PERC) Values (@saleID,@timeStamp,@employeeID,@customerID,@shopID,@Discountper)";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = query;
                        Int32 employeeID;
                        Int32 customerID;
                        Int32 shopID;
                        Decimal Discountper;
                        DateTime datetimevalue;
                        if (salaesitems.Sale.employeeID == "")
                        {
                            employeeID = 0;
                        }
                        else
                        {
                            employeeID = Convert.ToInt32(salaesitems.Sale.employeeID);
                        }


                        if (salaesitems.Sale.customerID == "")
                        {
                            customerID = 0;
                        }
                        else
                        {
                            customerID = Convert.ToInt32(salaesitems.Sale.customerID);
                        }

                        if (salaesitems.Sale.shopID == "")
                        {
                            shopID = 0;
                        }
                        else
                        {
                            shopID = Convert.ToInt32(salaesitems.Sale.shopID);
                        }


                        if (salaesitems.Sale.discountPercent == "")
                        {
                            Discountper = 0;
                        }
                        else
                        {
                            Discountper = Convert.ToDecimal(salaesitems.Sale.discountPercent);
                        }


                        if (salaesitems.Sale.discountPercent == "")
                        {
                            Discountper = 0;
                        }
                        else
                        {
                            Discountper = Convert.ToDecimal(salaesitems.Sale.discountPercent);
                        }


                        if (salaesitems.Sale.timeStamp == null)
                        {
                            datetimevalue = Convert.ToDateTime(0);
                        }
                        else
                        {
                            datetimevalue = Convert.ToDateTime(salaesitems.Sale.timeStamp);
                        }

                        cmd.Parameters.AddWithValue("saleID", saleID);
                        cmd.Parameters.AddWithValue("timeStamp", datetimevalue);
                        cmd.Parameters.AddWithValue("employeeID", employeeID);
                        cmd.Parameters.AddWithValue("customerID", customerID);
                        cmd.Parameters.AddWithValue("shopID", shopID);
                        cmd.Parameters.AddWithValue("Discountper", Discountper);
                        cmd.Connection = cnn;
                        var a = cmd.ExecuteNonQuery();
                        //cnn.Close();

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    cnn.Open();
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = cnn;
                    Int32 employeeID;
                    Int32 customerID;
                    Int32 shopID;
                    Decimal Discountper;
                    DateTime datetimevalue;
                    if (salaesitems.Sale.employeeID == "")
                    {
                        employeeID = 0;
                    }
                    else
                    {
                        employeeID = Convert.ToInt32(salaesitems.Sale.employeeID);
                    }


                    if (salaesitems.Sale.customerID == "")
                    {
                        customerID = 0;
                    }
                    else
                    {
                        customerID = Convert.ToInt32(salaesitems.Sale.customerID);
                    }

                    if (salaesitems.Sale.shopID == "")
                    {
                        shopID = 0;
                    }
                    else
                    {
                        shopID = Convert.ToInt32(salaesitems.Sale.shopID);
                    }


                    if (salaesitems.Sale.discountPercent == "")
                    {
                        Discountper = 0;
                    }
                    else
                    {
                        Discountper = Convert.ToDecimal(salaesitems.Sale.discountPercent);
                    }


                    if (salaesitems.Sale.discountPercent == "")
                    {
                        Discountper = 0;
                    }
                    else
                    {
                        Discountper = Convert.ToDecimal(salaesitems.Sale.discountPercent);
                    }
                    if (salaesitems.Sale.timeStamp == null)
                    {
                        datetimevalue = Convert.ToDateTime(0);
                    }
                    else
                    {
                        datetimevalue = Convert.ToDateTime(salaesitems.Sale.timeStamp);
                    }
                    string updateQuery = "UPDATE INVOICES SET EMPLOYEE_ID = @employeeID , CUSTOMER_ID = @customerID, STORE_ID =@shopID , DISCOUNT_PERC = @Discountper , TIME_STAMP = @datetimevalue  WHERE SALE_ID=" + Convert.ToInt64(salaesitems.Sale.saleID);
                    cmd2.CommandText = updateQuery;
                    cmd2.Parameters.AddWithValue("@employeeID", employeeID);
                    cmd2.Parameters.AddWithValue("@customerID", customerID);
                    cmd2.Parameters.AddWithValue("@shopID", shopID);
                    cmd2.Parameters.AddWithValue("@Discountper", Discountper);
                    cmd2.Parameters.AddWithValue("@datetimevalue", datetimevalue);
                    cmd2.ExecuteNonQuery();
                }
                cnn.Close();
            }

        }





        private static void InsertInvoiceItems(Salesinfo salaesitems)
        {
            for (Int32 Rowscount = 0; Rowscount <= salaesitems.Sale.Count() - 1; Rowscount++)
            {
                string connetionString = null;
                MySqlConnection cnn;
                connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
                cnn = new MySqlConnection(connetionString);
                DataSet ds = new DataSet();
                var saleID = salaesitems.Sale[Rowscount].saleID;

                if (saleID != "")
                {
                    string query1 = "select SALE_ID,EMPLOYEE_ID,CUSTOMER_ID,STORE_ID,DISCOUNT_PERC from INVOICES where SALE_ID=" + salaesitems.Sale[Rowscount].saleID;

                    MySqlCommand cmd1 = new MySqlCommand(query1);
                    cmd1.Connection = cnn;
                    DataSet dsn = new DataSet();
                    MySqlDataAdapter adp1 = new MySqlDataAdapter();
                    adp1.SelectCommand = cmd1;
                    adp1.Fill(dsn);
                    var ccoun = dsn.Tables[0].Rows.Count;
                    if (ccoun == 0)
                    {
                        try
                        {
                            cnn.Open();
                            string query = "Insert into INVOICES(SALE_ID,TIME_STAMP,EMPLOYEE_ID,CUSTOMER_ID,STORE_ID,DISCOUNT_PERC) Values (@saleID,@timeStamp,@employeeID,@customerID,@shopID,@Discountper)";
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.CommandText = query;
                            Int32 employeeID;
                            Int32 customerID;
                            Int32 shopID;
                            Decimal Discountper;
                            DateTime datetimevalue;
                            if (salaesitems.Sale[Rowscount].employeeID == "")
                            {
                                employeeID = 0;
                            }
                            else
                            {
                                employeeID = Convert.ToInt32(salaesitems.Sale[Rowscount].employeeID);
                            }


                            if (salaesitems.Sale[Rowscount].customerID == "")
                            {
                                customerID = 0;
                            }
                            else
                            {
                                customerID = Convert.ToInt32(salaesitems.Sale[Rowscount].customerID);
                            }

                            if (salaesitems.Sale[Rowscount].shopID == "")
                            {
                                shopID = 0;
                            }
                            else
                            {
                                shopID = Convert.ToInt32(salaesitems.Sale[Rowscount].shopID);
                            }


                            if (salaesitems.Sale[Rowscount].discountPercent == "")
                            {
                                Discountper = 0;
                            }
                            else
                            {
                                Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
                            }


                            if (salaesitems.Sale[Rowscount].discountPercent == "")
                            {
                                Discountper = 0;
                            }
                            else
                            {
                                Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
                            }


                            if (salaesitems.Sale[Rowscount].timeStamp == null)
                            {
                                datetimevalue = Convert.ToDateTime(0);
                            }
                            else
                            {
                                datetimevalue = Convert.ToDateTime(salaesitems.Sale[Rowscount].timeStamp);
                            }

                            cmd.Parameters.AddWithValue("saleID", saleID);
                            cmd.Parameters.AddWithValue("timeStamp", datetimevalue);
                            cmd.Parameters.AddWithValue("employeeID", employeeID);
                            cmd.Parameters.AddWithValue("customerID", customerID);
                            cmd.Parameters.AddWithValue("shopID", shopID);
                            cmd.Parameters.AddWithValue("Discountper", Discountper);
                            cmd.Connection = cnn;
                            var a = cmd.ExecuteNonQuery();
                            //cnn.Close();

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        cnn.Open();
                        MySqlCommand cmd2 = new MySqlCommand();
                        cmd2.Connection = cnn;
                        Int32 employeeID;
                        Int32 customerID;
                        Int32 shopID;
                        Decimal Discountper;
                        DateTime datetimevalue;
                        if (salaesitems.Sale[Rowscount].employeeID == "")
                        {
                            employeeID = 0;
                        }
                        else
                        {
                            employeeID = Convert.ToInt32(salaesitems.Sale[Rowscount].employeeID);
                        }


                        if (salaesitems.Sale[Rowscount].customerID == "")
                        {
                            customerID = 0;
                        }
                        else
                        {
                            customerID = Convert.ToInt32(salaesitems.Sale[Rowscount].customerID);
                        }

                        if (salaesitems.Sale[Rowscount].shopID == "")
                        {
                            shopID = 0;
                        }
                        else
                        {
                            shopID = Convert.ToInt32(salaesitems.Sale[Rowscount].shopID);
                        }


                        if (salaesitems.Sale[Rowscount].discountPercent == "")
                        {
                            Discountper = 0;
                        }
                        else
                        {
                            Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
                        }


                        if (salaesitems.Sale[Rowscount].discountPercent == "")
                        {
                            Discountper = 0;
                        }
                        else
                        {
                            Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
                        }
                        if (salaesitems.Sale[Rowscount].timeStamp == null)
                        {
                            datetimevalue = Convert.ToDateTime(0);
                        }
                        else
                        {
                            datetimevalue = Convert.ToDateTime(salaesitems.Sale[Rowscount].timeStamp);
                        }
                        string updateQuery = "UPDATE INVOICES SET EMPLOYEE_ID = @employeeID , CUSTOMER_ID = @customerID, STORE_ID =@shopID , DISCOUNT_PERC = @Discountper , TIME_STAMP = @datetimevalue  WHERE SALE_ID=" + Convert.ToInt64(salaesitems.Sale[Rowscount].saleID);
                        cmd2.CommandText = updateQuery;
                        cmd2.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd2.Parameters.AddWithValue("@customerID", customerID);
                        cmd2.Parameters.AddWithValue("@shopID", shopID);
                        cmd2.Parameters.AddWithValue("@Discountper", Discountper);
                        cmd2.Parameters.AddWithValue("@datetimevalue", datetimevalue);
                        cmd2.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
            }
        }




        //private static void InsertInvoiceItems(Salesinfo salaesitems)
        //{
        //    for (Int32 Rowscount = 0; Rowscount <= salaesitems.Sale.Count() - 1; Rowscount++)
        //    {
        //        string connetionString = null;
        //        MySqlConnection cnn;
        //        connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
        //        cnn = new MySqlConnection(connetionString);
        //        DataSet ds = new DataSet();
        //        var saleID = salaesitems.Sale[Rowscount].saleID;

        //        if (saleID != "")
        //        {
        //            string query1 = "select SALE_ID,EMPLOYEE_ID,CUSTOMER_ID,STORE_ID,DISCOUNT_PERC from INVOICES where SALE_ID=" + salaesitems.Sale[Rowscount].saleID;

        //            MySqlCommand cmd1 = new MySqlCommand(query1);
        //            cmd1.Connection = cnn;
        //            DataSet dsn = new DataSet();
        //            MySqlDataAdapter adp1 = new MySqlDataAdapter();
        //            adp1.SelectCommand = cmd1;
        //            adp1.Fill(dsn);
        //            var ccoun = dsn.Tables[0].Rows.Count;
        //            if (ccoun == 0)
        //            {
        //                try
        //                {
        //                    cnn.Open();
        //                    string query = "Insert into INVOICES(SALE_ID,TIME_STAMP,EMPLOYEE_ID,CUSTOMER_ID,STORE_ID,DISCOUNT_PERC) Values (@saleID,@timeStamp,@employeeID,@customerID,@shopID,@Discountper)";
        //                    MySqlCommand cmd = new MySqlCommand();
        //                    cmd.CommandText = query;
        //                    Int32 employeeID;
        //                    Int32 customerID;
        //                    Int32 shopID;
        //                    Decimal Discountper;
        //                    DateTime datetimevalue;
        //                    if (salaesitems.Sale[Rowscount].employeeID == "")
        //                    {
        //                        employeeID = 0;
        //                    }
        //                    else
        //                    {
        //                        employeeID = Convert.ToInt32(salaesitems.Sale[Rowscount].employeeID);
        //                    }


        //                    if (salaesitems.Sale[Rowscount].customerID == "")
        //                    {
        //                        customerID = 0;
        //                    }
        //                    else
        //                    {
        //                        customerID = Convert.ToInt32(salaesitems.Sale[Rowscount].customerID);
        //                    }

        //                    if (salaesitems.Sale[Rowscount].shopID == "")
        //                    {
        //                        shopID = 0;
        //                    }
        //                    else
        //                    {
        //                        shopID = Convert.ToInt32(salaesitems.Sale[Rowscount].shopID);
        //                    }


        //                    if (salaesitems.Sale[Rowscount].discountPercent == "")
        //                    {
        //                        Discountper = 0;
        //                    }
        //                    else
        //                    {
        //                        Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
        //                    }


        //                    if (salaesitems.Sale[Rowscount].discountPercent == "")
        //                    {
        //                        Discountper = 0;
        //                    }
        //                    else
        //                    {
        //                        Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
        //                    }


        //                    if (salaesitems.Sale[Rowscount].timeStamp == null)
        //                    {
        //                        datetimevalue = Convert.ToDateTime(0);
        //                    }
        //                    else
        //                    {
        //                        datetimevalue = Convert.ToDateTime(salaesitems.Sale[Rowscount].timeStamp);
        //                    }




        //                    cmd.Parameters.AddWithValue("saleID", saleID);
        //                    cmd.Parameters.AddWithValue("timeStamp", datetimevalue);
        //                    cmd.Parameters.AddWithValue("employeeID", employeeID);
        //                    cmd.Parameters.AddWithValue("customerID", customerID);
        //                    cmd.Parameters.AddWithValue("shopID", shopID);
        //                    cmd.Parameters.AddWithValue("Discountper", Discountper);
        //                    cmd.Connection = cnn;
        //                    var a = cmd.ExecuteNonQuery();
        //                    //cnn.Close();

        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }
        //            else
        //            {
        //                cnn.Open();
        //                MySqlCommand cmd2 = new MySqlCommand();
        //                cmd2.Connection = cnn;
        //                Int32 employeeID;
        //                Int32 customerID;
        //                Int32 shopID;
        //                Decimal Discountper;
        //                DateTime datetimevalue;
        //                if (salaesitems.Sale[Rowscount].employeeID == "")
        //                {
        //                    employeeID = 0;
        //                }
        //                else
        //                {
        //                    employeeID = Convert.ToInt32(salaesitems.Sale[Rowscount].employeeID);
        //                }


        //                if (salaesitems.Sale[Rowscount].customerID == "")
        //                {
        //                    customerID = 0;
        //                }
        //                else
        //                {
        //                    customerID = Convert.ToInt32(salaesitems.Sale[Rowscount].customerID);
        //                }

        //                if (salaesitems.Sale[Rowscount].shopID == "")
        //                {
        //                    shopID = 0;
        //                }
        //                else
        //                {
        //                    shopID = Convert.ToInt32(salaesitems.Sale[Rowscount].shopID);
        //                }


        //                if (salaesitems.Sale[Rowscount].discountPercent == "")
        //                {
        //                    Discountper = 0;
        //                }
        //                else
        //                {
        //                    Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
        //                }


        //                if (salaesitems.Sale[Rowscount].discountPercent == "")
        //                {
        //                    Discountper = 0;
        //                }
        //                else
        //                {
        //                    Discountper = Convert.ToDecimal(salaesitems.Sale[Rowscount].discountPercent);
        //                }


        //                if (salaesitems.Sale[Rowscount].timeStamp == null)
        //                {
        //                    datetimevalue = Convert.ToDateTime(0);
        //                }
        //                else
        //                {
        //                    datetimevalue = Convert.ToDateTime(salaesitems.Sale[Rowscount].timeStamp);
        //                }



        //                string updateQuery = "UPDATE INVOICES SET EMPLOYEE_ID = @employeeID , CUSTOMER_ID = @customerID, STORE_ID =@shopID , DISCOUNT_PERC = @Discountper , TIME_STAMP = @datetimevalue  WHERE SALE_ID=" + Convert.ToInt64(salaesitems.Sale[Rowscount].saleID);
        //                cmd2.CommandText = updateQuery;
        //                cmd2.Parameters.AddWithValue("@employeeID", employeeID);
        //                cmd2.Parameters.AddWithValue("@customerID", customerID);
        //                cmd2.Parameters.AddWithValue("@shopID", shopID);
        //                cmd2.Parameters.AddWithValue("@Discountper", Discountper);
        //                cmd2.Parameters.AddWithValue("@datetimevalue", datetimevalue);

        //                cmd2.ExecuteNonQuery();

        //            }
        //            cnn.Close();
        //        }



        //    }

        //}

        private static void InsertShopItems(Shopinfo shopitems)
        {
            for (Int32 Rowscount = 0; Rowscount <= shopitems.Shop.Count() - 1; Rowscount++)
            {
                string connetionString = null;
                MySqlConnection cnn;
                connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
                cnn = new MySqlConnection(connetionString);
                DataSet ds = new DataSet();
                var ShopID = shopitems.Shop[Rowscount].shopID;
                if (ShopID != "")
                {
                    cnn.Open();
                    string query1 = "select * from SHOP where STORE_ID=" + shopitems.Shop[Rowscount].shopID;
                    MySqlCommand cmd1 = new MySqlCommand(query1);
                    cmd1.Connection = cnn;
                    DataSet dsn = new DataSet();
                    MySqlDataAdapter adp1 = new MySqlDataAdapter();
                    adp1.SelectCommand = cmd1;
                    //var a = adp1;
                    adp1.Fill(dsn);
                    var ccoun = dsn.Tables[0].Rows.Count;
                    if (ccoun == 0)
                    {
                        try
                        {
                            string query = "Insert into SHOP(STORE_ID,STORE_NAME) Values (@STORE_ID,@STORE_NAME)";
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("STORE_ID", Convert.ToInt32(shopitems.Shop[Rowscount].shopID));
                            cmd.Parameters.AddWithValue("STORE_NAME", shopitems.Shop[Rowscount].name);
                            cmd.Connection = cnn;
                            var a = cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    else
                    {
                        MySqlCommand cmd2 = new MySqlCommand();
                        cmd2.Connection = cnn;
                        string updateQuery = "UPDATE SHOP SET STORE_NAME = @ShopName WHERE STORE_ID=@StoreId";
                        cmd2.CommandText = updateQuery;
                        cmd2.Parameters.AddWithValue("@ShopName", shopitems.Shop[Rowscount].name.ToString());
                        cmd2.Parameters.AddWithValue("@StoreId", Convert.ToInt64(shopitems.Shop[Rowscount].shopID));
                        cmd2.ExecuteNonQuery();
                    }
                }
                cnn.Close();
            }
        }

        private static void InsertSalesLineItems(SaleLineinfo shopitems)
        {
            for (Int32 Rowscount = 0; Rowscount <= shopitems.SaleLine.Count() - 1; Rowscount++)
            {
                string connetionString = null;
                MySqlConnection cnn;
                connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
                cnn = new MySqlConnection(connetionString);
                DataSet ds = new DataSet();
                var saleslineid = shopitems.SaleLine[Rowscount].saleLineID;
                if (saleslineid != "")
                {
                    cnn.Open();
                    string query1 = "select * from INVOICE_ITEM where saleLineID=" + shopitems.SaleLine[Rowscount].saleLineID;
                    MySqlCommand cmd1 = new MySqlCommand(query1);
                    cmd1.Connection = cnn;
                    DataSet dsn = new DataSet();
                    MySqlDataAdapter adp1 = new MySqlDataAdapter();
                    adp1.SelectCommand = cmd1;
                    //var a = adp1;
                    adp1.Fill(dsn);
                    var ccoun = dsn.Tables[0].Rows.Count;

                    if (ccoun == 0)
                    {
                        try
                        {
                            string query = "Insert into INVOICE_ITEM(saleLineID,SALELINE_NO,LSITEM_ID,SALE_ID,QTY,ORIG_PRICE,DISPLAY_PRICE,TAX_AMT,COST,DISCOUNT_PERC ,calcTotal) Values (@saleLineID,@SALELINE_NO,@LSITEM_ID,@SALE_ID,@QTY,@ORIG_PRICE,@DISPLAY_PRICE,@TAX_AMT,@COST,@DISCOUNT_PERC,@calcTotal)";
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.CommandText = query;

                            if (shopitems.SaleLine[Rowscount].saleLineID != "" && shopitems.SaleLine[Rowscount].saleLineID != null)
                                cmd.Parameters.AddWithValue("SALELINE_NO", Convert.ToInt32(shopitems.SaleLine[Rowscount].saleLineID));
                            else
                                cmd.Parameters.AddWithValue("SALELINE_NO", "");

                            if (shopitems.SaleLine[Rowscount].itemID != "0" && shopitems.SaleLine[Rowscount].itemID != null)
                                cmd.Parameters.AddWithValue("LSITEM_ID", Convert.ToInt32(shopitems.SaleLine[Rowscount].itemID));
                            else
                                cmd.Parameters.AddWithValue("LSITEM_ID", null);
                            if (shopitems.SaleLine[Rowscount].saleID != "" && shopitems.SaleLine[Rowscount].saleID != null)
                                cmd.Parameters.AddWithValue("SALE_ID", Convert.ToInt32(shopitems.SaleLine[Rowscount].saleID));
                            else
                                cmd.Parameters.AddWithValue("SALE_ID", "");


                            if (shopitems.SaleLine[Rowscount].unitQuantity != "" && shopitems.SaleLine[Rowscount].unitQuantity != null)
                                cmd.Parameters.AddWithValue("QTY", Convert.ToDecimal(shopitems.SaleLine[Rowscount].unitQuantity));
                            else
                                cmd.Parameters.AddWithValue("QTY", "");


                            if (shopitems.SaleLine[Rowscount].normalUnitPrice != "" && shopitems.SaleLine[Rowscount].normalUnitPrice != null)
                                cmd.Parameters.AddWithValue("ORIG_PRICE", Convert.ToDecimal(shopitems.SaleLine[Rowscount].normalUnitPrice));
                            else
                                cmd.Parameters.AddWithValue("ORIG_PRICE", "");


                            if (shopitems.SaleLine[Rowscount].displayableUnitPrice != "" && shopitems.SaleLine[Rowscount].displayableUnitPrice != null)
                                cmd.Parameters.AddWithValue("DISPLAY_PRICE", Convert.ToDecimal(shopitems.SaleLine[Rowscount].displayableUnitPrice));
                            else
                                cmd.Parameters.AddWithValue("DISPLAY_PRICE", "");
                            if (shopitems.SaleLine[Rowscount].calcTax1 != "" && shopitems.SaleLine[Rowscount].calcTax1 != null)
                                cmd.Parameters.AddWithValue("TAX_AMT", Convert.ToDecimal(shopitems.SaleLine[Rowscount].calcTax1));
                            else
                                cmd.Parameters.AddWithValue("TAX_AMT", "");

                            if (shopitems.SaleLine[Rowscount].calcTotal != "" && shopitems.SaleLine[Rowscount].calcTotal != null)
                                cmd.Parameters.AddWithValue("calcTotal", Convert.ToDecimal(shopitems.SaleLine[Rowscount].calcTotal));
                            else
                                cmd.Parameters.AddWithValue("calcTotal", "");

                            if (shopitems.SaleLine[Rowscount].calcTax1 != "" && shopitems.SaleLine[Rowscount].avgCost != null)
                                cmd.Parameters.AddWithValue("COST", Convert.ToDecimal(shopitems.SaleLine[Rowscount].avgCost));
                            else
                                cmd.Parameters.AddWithValue("COST", "");


                            if (shopitems.SaleLine[Rowscount].discountPercent != "" && shopitems.SaleLine[Rowscount].discountPercent != null)
                                cmd.Parameters.AddWithValue("DISCOUNT_PERC", Convert.ToDecimal(shopitems.SaleLine[Rowscount].discountPercent));
                            else
                                cmd.Parameters.AddWithValue("DISCOUNT_PERC", "");

                            if (shopitems.SaleLine[Rowscount].saleLineID != "" && shopitems.SaleLine[Rowscount].saleLineID != null)
                                cmd.Parameters.AddWithValue("saleLineID", Convert.ToInt32(shopitems.SaleLine[Rowscount].saleLineID));
                            else
                                cmd.Parameters.AddWithValue("saleLineID", "");


                            cmd.Connection = cnn;
                            var a = cmd.ExecuteNonQuery();
                            cnn.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            string query = "Insert into INVOICE_ITEM(saleLineID,SALELINE_NO,LSITEM_ID,SALE_ID,QTY,ORIG_PRICE,DISPLAY_PRICE,TAX_AMT,COST,DISCOUNT_PERC,calcTotal) Values (@saleLineID,@SALELINE_NO,@LSITEM_ID,@SALE_ID,@QTY,@ORIG_PRICE,@DISPLAY_PRICE,@TAX_AMT,@COST,@DISCOUNT_PERC,@calcTotal)";
                            MySqlCommand cmd2 = new MySqlCommand();
                            cmd2.Connection = cnn;
                            Int32 LSITEM_ID;
                            Int32 SALE_ID;
                            Decimal QTY;
                            Decimal ORIG_PRICE;
                            Decimal DISPLAY_PRICE;
                            Decimal TAX_AMT;
                            Decimal COST;
                            Decimal DISCOUNT_PERC;
                            Decimal calcTotal;
                            var a = Convert.ToInt64(shopitems.SaleLine[Rowscount].saleLineID);

                            if (shopitems.SaleLine[Rowscount].itemID == "")
                            {
                                LSITEM_ID = 0;
                            }
                            else
                            {
                                LSITEM_ID = Convert.ToInt32(shopitems.SaleLine[Rowscount].itemID);
                            }


                            if (shopitems.SaleLine[Rowscount].saleID == "")
                            {
                                SALE_ID = 0;
                            }
                            else
                            {
                                SALE_ID = Convert.ToInt32(shopitems.SaleLine[Rowscount].saleID);
                            }

                            if (shopitems.SaleLine[Rowscount].unitQuantity == "")
                            {
                                QTY = 0;
                            }
                            else
                            {
                                QTY = Convert.ToDecimal(shopitems.SaleLine[Rowscount].unitQuantity);
                            }

                            if (shopitems.SaleLine[Rowscount].calcTax1 == "")
                            {
                                TAX_AMT = 0;
                            }
                            else
                            {
                                TAX_AMT = Convert.ToDecimal(shopitems.SaleLine[Rowscount].calcTax1);
                            }

                            if (shopitems.SaleLine[Rowscount].avgCost == "")
                            {
                                COST = 0;
                            }
                            else
                            {
                                COST = Convert.ToDecimal(shopitems.SaleLine[Rowscount].avgCost);
                            }

                            if (shopitems.SaleLine[Rowscount].discountPercent == "")
                            {
                                DISCOUNT_PERC = 0;
                            }
                            else
                            {
                                DISCOUNT_PERC = Convert.ToDecimal(shopitems.SaleLine[Rowscount].discountPercent);
                            }

                            if (shopitems.SaleLine[Rowscount].displayableUnitPrice == "")
                            {
                                DISPLAY_PRICE = 0;
                            }
                            else
                            {
                                DISPLAY_PRICE = Convert.ToDecimal(shopitems.SaleLine[Rowscount].displayableUnitPrice);
                            }


                            if (shopitems.SaleLine[Rowscount].normalUnitPrice == "")
                            {
                                ORIG_PRICE = 0;
                            }
                            else
                            {
                                ORIG_PRICE = Convert.ToDecimal(shopitems.SaleLine[Rowscount].displayableUnitPrice);
                            }

                            if (shopitems.SaleLine[Rowscount].calcTotal == "")
                            {
                                calcTotal = 0;
                            }
                            else
                            {
                                calcTotal = Convert.ToDecimal(shopitems.SaleLine[Rowscount].calcTotal);
                            }
                            string updateQuery = "UPDATE INVOICE_ITEM SET LSITEM_ID = @LSITEM_ID, SALE_ID = @SALELINE_NO , QTY = @QTY, ORIG_PRICE = @ORIG_PRICE, DISPLAY_PRICE = @DISPLAY_PRICE, TAX_AMT = @TAX_AMT, COST = @COST, DISCOUNT_PERC= @DISCOUNT_PERC ,calcTotal=@calcTotal  WHERE saleLineID=" + Convert.ToInt64(shopitems.SaleLine[Rowscount].saleLineID);
                            cmd2.Parameters.AddWithValue("@LSITEM_ID", LSITEM_ID);
                            cmd2.Parameters.AddWithValue("@SALELINE_NO", SALE_ID);
                            cmd2.Parameters.AddWithValue("@QTY", QTY);
                            cmd2.Parameters.AddWithValue("@ORIG_PRICE", ORIG_PRICE);
                            cmd2.Parameters.AddWithValue("@DISPLAY_PRICE", DISPLAY_PRICE);
                            cmd2.Parameters.AddWithValue("@TAX_AMT", TAX_AMT);
                            cmd2.Parameters.AddWithValue("@COST", COST);
                            cmd2.Parameters.AddWithValue("@DISCOUNT_PERC", DISCOUNT_PERC);
                            cmd2.Parameters.AddWithValue("@calcTotal", calcTotal);
                            cmd2.CommandText = updateQuery;
                            cmd2.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                        }

                    }
                    cnn.Close();
                }
            }
        }



        private static void InsertSalesLineSingleItems(Salelininfromation shopitems)
        {

            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            var saleslineid = shopitems.SaleLine.saleLineID;
            if (saleslineid != "")
            {
                cnn.Open();
                string query1 = "select * from INVOICE_ITEM where saleLineID=" + shopitems.SaleLine.saleLineID;
                MySqlCommand cmd1 = new MySqlCommand(query1);
                cmd1.Connection = cnn;
                DataSet dsn = new DataSet();
                MySqlDataAdapter adp1 = new MySqlDataAdapter();
                adp1.SelectCommand = cmd1;
                //var a = adp1;
                adp1.Fill(dsn);
                var ccoun = dsn.Tables[0].Rows.Count;

                if (ccoun == 0)
                {
                    try
                    {
                        string query = "Insert into INVOICE_ITEM(saleLineID,SALELINE_NO,LSITEM_ID,SALE_ID,QTY,ORIG_PRICE,DISPLAY_PRICE,TAX_AMT,COST,DISCOUNT_PERC ,calcTotal) Values (@saleLineID,@SALELINE_NO,@LSITEM_ID,@SALE_ID,@QTY,@ORIG_PRICE,@DISPLAY_PRICE,@TAX_AMT,@COST,@DISCOUNT_PERC,@calcTotal)";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = query;

                        if (shopitems.SaleLine.saleLineID != "" && shopitems.SaleLine.saleLineID != null)
                            cmd.Parameters.AddWithValue("SALELINE_NO", Convert.ToInt32(shopitems.SaleLine.saleLineID));
                        else
                            cmd.Parameters.AddWithValue("SALELINE_NO", "");

                        if (shopitems.SaleLine.itemID != "0" && shopitems.SaleLine.itemID != null)
                            cmd.Parameters.AddWithValue("LSITEM_ID", Convert.ToInt32(shopitems.SaleLine.itemID));
                        else
                            cmd.Parameters.AddWithValue("LSITEM_ID", null);
                        if (shopitems.SaleLine.saleID != "" && shopitems.SaleLine.saleID != null)
                            cmd.Parameters.AddWithValue("SALE_ID", Convert.ToInt32(shopitems.SaleLine.saleID));
                        else
                            cmd.Parameters.AddWithValue("SALE_ID", "");


                        if (shopitems.SaleLine.unitQuantity != "" && shopitems.SaleLine.unitQuantity != null)
                            cmd.Parameters.AddWithValue("QTY", Convert.ToDecimal(shopitems.SaleLine.unitQuantity));
                        else
                            cmd.Parameters.AddWithValue("QTY", "");


                        if (shopitems.SaleLine.normalUnitPrice != "" && shopitems.SaleLine.normalUnitPrice != null)
                            cmd.Parameters.AddWithValue("ORIG_PRICE", Convert.ToDecimal(shopitems.SaleLine.normalUnitPrice));
                        else
                            cmd.Parameters.AddWithValue("ORIG_PRICE", "");


                        if (shopitems.SaleLine.displayableUnitPrice != "" && shopitems.SaleLine.displayableUnitPrice != null)
                            cmd.Parameters.AddWithValue("DISPLAY_PRICE", Convert.ToDecimal(shopitems.SaleLine.displayableUnitPrice));
                        else
                            cmd.Parameters.AddWithValue("DISPLAY_PRICE", "");
                        if (shopitems.SaleLine.calcTax1 != "" && shopitems.SaleLine.calcTax1 != null)
                            cmd.Parameters.AddWithValue("TAX_AMT", Convert.ToDecimal(shopitems.SaleLine.calcTax1));
                        else
                            cmd.Parameters.AddWithValue("TAX_AMT", "");

                        if (shopitems.SaleLine.calcTotal != "" && shopitems.SaleLine.calcTotal != null)
                            cmd.Parameters.AddWithValue("calcTotal", Convert.ToDecimal(shopitems.SaleLine.calcTotal));
                        else
                            cmd.Parameters.AddWithValue("calcTotal", "");

                        if (shopitems.SaleLine.calcTax1 != "" && shopitems.SaleLine.avgCost != null)
                            cmd.Parameters.AddWithValue("COST", Convert.ToDecimal(shopitems.SaleLine.avgCost));
                        else
                            cmd.Parameters.AddWithValue("COST", "");


                        if (shopitems.SaleLine.discountPercent != "" && shopitems.SaleLine.discountPercent != null)
                            cmd.Parameters.AddWithValue("DISCOUNT_PERC", Convert.ToDecimal(shopitems.SaleLine.discountPercent));
                        else
                            cmd.Parameters.AddWithValue("DISCOUNT_PERC", "");

                        if (shopitems.SaleLine.saleLineID != "" && shopitems.SaleLine.saleLineID != null)
                            cmd.Parameters.AddWithValue("saleLineID", Convert.ToInt32(shopitems.SaleLine.saleLineID));
                        else
                            cmd.Parameters.AddWithValue("saleLineID", "");


                        cmd.Connection = cnn;
                        var a = cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    try
                    {
                        string query = "Insert into INVOICE_ITEM(saleLineID,SALELINE_NO,LSITEM_ID,SALE_ID,QTY,ORIG_PRICE,DISPLAY_PRICE,TAX_AMT,COST,DISCOUNT_PERC,calcTotal) Values (@saleLineID,@SALELINE_NO,@LSITEM_ID,@SALE_ID,@QTY,@ORIG_PRICE,@DISPLAY_PRICE,@TAX_AMT,@COST,@DISCOUNT_PERC,@calcTotal)";
                        MySqlCommand cmd2 = new MySqlCommand();
                        cmd2.Connection = cnn;
                        Int32 LSITEM_ID;
                        Int32 SALE_ID;
                        Decimal QTY;
                        Decimal ORIG_PRICE;
                        Decimal DISPLAY_PRICE;
                        Decimal TAX_AMT;
                        Decimal COST;
                        Decimal DISCOUNT_PERC;
                        Decimal calcTotal;
                        var a = Convert.ToInt64(shopitems.SaleLine.saleLineID);

                        if (shopitems.SaleLine.itemID == "")
                        {
                            LSITEM_ID = 0;
                        }
                        else
                        {
                            LSITEM_ID = Convert.ToInt32(shopitems.SaleLine.itemID);
                        }


                        if (shopitems.SaleLine.saleID == "")
                        {
                            SALE_ID = 0;
                        }
                        else
                        {
                            SALE_ID = Convert.ToInt32(shopitems.SaleLine.saleID);
                        }

                        if (shopitems.SaleLine.unitQuantity == "")
                        {
                            QTY = 0;
                        }
                        else
                        {
                            QTY = Convert.ToDecimal(shopitems.SaleLine.unitQuantity);
                        }

                        if (shopitems.SaleLine.calcTax1 == "")
                        {
                            TAX_AMT = 0;
                        }
                        else
                        {
                            TAX_AMT = Convert.ToDecimal(shopitems.SaleLine.calcTax1);
                        }

                        if (shopitems.SaleLine.avgCost == "")
                        {
                            COST = 0;
                        }
                        else
                        {
                            COST = Convert.ToDecimal(shopitems.SaleLine.avgCost);
                        }

                        if (shopitems.SaleLine.discountPercent == "")
                        {
                            DISCOUNT_PERC = 0;
                        }
                        else
                        {
                            DISCOUNT_PERC = Convert.ToDecimal(shopitems.SaleLine.discountPercent);
                        }

                        if (shopitems.SaleLine.displayableUnitPrice == "")
                        {
                            DISPLAY_PRICE = 0;
                        }
                        else
                        {
                            DISPLAY_PRICE = Convert.ToDecimal(shopitems.SaleLine.displayableUnitPrice);
                        }


                        if (shopitems.SaleLine.normalUnitPrice == "")
                        {
                            ORIG_PRICE = 0;
                        }
                        else
                        {
                            ORIG_PRICE = Convert.ToDecimal(shopitems.SaleLine.displayableUnitPrice);
                        }

                        if (shopitems.SaleLine.calcTotal == "")
                        {
                            calcTotal = 0;
                        }
                        else
                        {
                            calcTotal = Convert.ToDecimal(shopitems.SaleLine.calcTotal);
                        }
                        string updateQuery = "UPDATE INVOICE_ITEM SET LSITEM_ID = @LSITEM_ID, SALE_ID = @SALELINE_NO , QTY = @QTY, ORIG_PRICE = @ORIG_PRICE, DISPLAY_PRICE = @DISPLAY_PRICE, TAX_AMT = @TAX_AMT, COST = @COST, DISCOUNT_PERC= @DISCOUNT_PERC ,calcTotal=@calcTotal  WHERE saleLineID=" + Convert.ToInt64(shopitems.SaleLine.saleLineID);
                        cmd2.Parameters.AddWithValue("@LSITEM_ID", LSITEM_ID);
                        cmd2.Parameters.AddWithValue("@SALELINE_NO", SALE_ID);
                        cmd2.Parameters.AddWithValue("@QTY", QTY);
                        cmd2.Parameters.AddWithValue("@ORIG_PRICE", ORIG_PRICE);
                        cmd2.Parameters.AddWithValue("@DISPLAY_PRICE", DISPLAY_PRICE);
                        cmd2.Parameters.AddWithValue("@TAX_AMT", TAX_AMT);
                        cmd2.Parameters.AddWithValue("@COST", COST);
                        cmd2.Parameters.AddWithValue("@DISCOUNT_PERC", DISCOUNT_PERC);
                        cmd2.Parameters.AddWithValue("@calcTotal", calcTotal);
                        cmd2.CommandText = updateQuery;
                        cmd2.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }

                }
                cnn.Close();
            }

        }


        private static void InsertInventorySingleItems(itemsingleinfromation shopitems)
        {


            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
            cnn = new MySqlConnection(connetionString);
            DataSet ds = new DataSet();
            var itemid = shopitems.Item.itemID;
            if (itemid != "")
            {
                try
                {
                    cnn.Open();
                    string query1 = "select * from INVENTORY where LSITEM_ID=" + shopitems.Item.itemID;
                    MySqlCommand cmd1 = new MySqlCommand(query1);
                    cmd1.Connection = cnn;
                    DataSet dsn = new DataSet();
                    MySqlDataAdapter adp1 = new MySqlDataAdapter();
                    adp1.SelectCommand = cmd1;
                    //var a = adp1;
                    adp1.Fill(dsn);
                    var ccoun = dsn.Tables[0].Rows.Count;
                    if (ccoun == 0)
                    {
                        string query = "Insert into INVENTORY(CreatedDate,LSITEM_ID,Category,attribute1 ,attribute2,UPC,ALU,ITEM_SID,BRAND,MATRIX_ID,COST,ITEM_NAME,ITEM_ATT_SET_ID,ITEM_ATT_SET_NAME,ITEM_ATT_1,ITEM_ATT_2,Price,MSRP,systemSku,BrandName ,reorderLevel,reorderPoint,shopID,qoh,shop_0_qoh,shop_0_reorderLevel,shop_0_reorderPoint,shop_1_qoh,shop_1_reorderLevel,shop_1_reorderPoint, shop_2_qoh,shop_2_reorderLevel,shop_2_reorderPoint,shop_3_qoh,shop_3_reorderLevel,shop_3_reorderPoint,shop_4_qoh,shop_4_reorderLevel,shop_4_reorderPoint,shop_5_qoh,shop_5_reorderLevel,shop_5_reorderPoint,shop_6_qoh,shop_6_reorderLevel,shop_6_reorderPoint,shop_7_qoh,shop_7_reorderLevel,shop_7_reorderPoint,shop_8_qoh,shop_8_reorderLevel,shop_8_reorderPoint,shop_9_qoh,shop_9_reorderLevel,shop_9_reorderPoint) Values (@CreatedDate,@LSITEM_ID,@Category,@attribute1,@attribute2,@UPC,@ALU,@ITEM_SID,@BRAND,@MATRIX_ID,@COST,@ITEM_NAME,@ITEM_ATT_SET_ID,@ITEM_ATT_SET_NAME,@ITEM_ATT_1,@ITEM_ATT_2,@Price,@MSRP,@systemSku,@BrandName,@reorderLevel,@reorderPoint,@shopID,@qoh,shop_0_qoh,shop_0_reorderLevel,shop_0_reorderPoint,@shop_1_qoh,@shop_1_reorderLevel,@shop_1_reorderPoint,@shop_2_qoh,@shop_2_reorderLevel,@shop_2_reorderPoint,@shop_3_qoh,@shop_3_reorderLevel,@shop_3_reorderPoint,@shop_4_qoh,@shop_4_reorderLevel,@shop_4_reorderPoint,@shop_5_qoh,@shop_5_reorderLevel,@shop_5_reorderPoint,@shop_6_qoh,@shop_6_reorderLevel,@shop_6_reorderPoint,@shop_7_qoh,@shop_7_reorderLevel,@shop_7_reorderPoint,@shop_8_qoh,@shop_8_reorderLevel,@shop_8_reorderPoint,@shop_9_qoh,@shop_9_reorderLevel,@shop_9_reorderPoint)";
                        // string query = "Insert into INVENTORY(CreatedDate,LSITEM_ID,CATEGORY,UPC,ALU,ITEM_SID,BRAND,MATRIX_ID,COST,ITEM_NAME,ITEM_ATT_SET_ID,ITEM_ATT_SET_NAME,ITEM_ATT_1,ITEM_ATT_2,Price,MSRP,MATRIX_ID) Values (@CreatedDate,@LSITEM_ID,@CATEGORY,@UPC,@ALU,@ITEM_SID,@BRAND,@MATRIX_ID,@COST,@ITEM_NAME,@ITEM_ATT_SET_ID,@ITEM_ATT_SET_NAME,@ITEM_ATT_1,@ITEM_ATT_2,@Price,@MSRP,@MATRIX_ID)";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("LSITEM_ID", Convert.ToInt32(shopitems.Item.itemID));
                        //if ((shopitems.Item[Rowscount].itemMatrixID).ToString() != "")
                        //    cmd.Parameters.AddWithValue("MATRIX_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemMatrixID));
                        //else
                        //    cmd.Parameters.AddWithValue("MATRIX_ID", "");
                        if ((shopitems.Item.createTime).ToString() != "")
                            cmd.Parameters.AddWithValue("CreatedDate", Convert.ToDateTime(shopitems.Item.createTime));
                        else
                            cmd.Parameters.AddWithValue("CreatedDate", "");
                        if (shopitems.Item.categoryID != "")
                            cmd.Parameters.AddWithValue("CATEGORY", shopitems.Item.categoryID.ToString());
                        else
                            cmd.Parameters.AddWithValue("CATEGORY", "");
                        if (shopitems.Item.upc != "")
                            cmd.Parameters.AddWithValue("UPC", Convert.ToDouble(shopitems.Item.upc));
                        else
                            cmd.Parameters.AddWithValue("UPC", "");
                        if (shopitems.Item.customSku != null && shopitems.Item.customSku != "")
                            cmd.Parameters.AddWithValue("ALU", shopitems.Item.customSku.ToString());
                        else
                            cmd.Parameters.AddWithValue("ALU", "");
                        if (shopitems.Item.manufacturerSku != null && shopitems.Item.manufacturerSku != "")
                            cmd.Parameters.AddWithValue("ITEM_SID", shopitems.Item.manufacturerSku);
                        else
                            cmd.Parameters.AddWithValue("ITEM_SID", 0);

                        if (shopitems.Item.manufacturerID != null && shopitems.Item.manufacturerID != "")
                            cmd.Parameters.AddWithValue("BRAND", (shopitems.Item.manufacturerID).ToString());
                        else
                            cmd.Parameters.AddWithValue("BRAND", "");

                        if (shopitems.Item.itemMatrixID != null && shopitems.Item.itemMatrixID != "")
                            cmd.Parameters.AddWithValue("MATRIX_ID", Convert.ToInt32(shopitems.Item.itemMatrixID));
                        else
                            cmd.Parameters.AddWithValue("MATRIX_ID", "");
                        if (shopitems.Item.defaultCost != null && shopitems.Item.defaultCost != "")
                            cmd.Parameters.AddWithValue("COST", Convert.ToDecimal(shopitems.Item.defaultCost));
                        else
                            cmd.Parameters.AddWithValue("COST", "");

                        if (shopitems.Item.Prices.ItemPrice.Count != 0)
                        {
                            cmd.Parameters.AddWithValue("Price", Convert.ToDecimal(shopitems.Item.Prices.ItemPrice[0].amount));
                        }
                        else
                            cmd.Parameters.AddWithValue("Price", "");
                        if (shopitems.Item.Prices.ItemPrice.Count != 0)
                        {
                            cmd.Parameters.AddWithValue("MSRP", Convert.ToDecimal(shopitems.Item.Prices.ItemPrice[1].amount));
                        }
                        else
                            cmd.Parameters.AddWithValue("MSRP", "");

                        //cmd.Parameters.AddWithValue("PRICE", Convert.ToDecimal(shopitems.Item[0].co));
                        if (shopitems.Item.description != null && shopitems.Item.description != "")
                            cmd.Parameters.AddWithValue("ITEM_NAME", (shopitems.Item.description).ToString());
                        else
                            cmd.Parameters.AddWithValue("ITEM_NAME", "");

                        if (shopitems.Item.itemAttributeSetID != null && shopitems.Item.itemAttributeSetID != "")
                            cmd.Parameters.AddWithValue("ITEM_ATT_SET_ID", Convert.ToInt32(shopitems.Item.itemAttributeSetID));
                        else
                            cmd.Parameters.AddWithValue("ITEM_ATT_SET_ID", "");

                        if (shopitems.Item.name != null && shopitems.Item.name != "")
                            cmd.Parameters.AddWithValue("ITEM_ATT_SET_NAME", (shopitems.Item.name).ToString());
                        else
                            cmd.Parameters.AddWithValue("ITEM_ATT_SET_NAME", "");

                        if (shopitems.Item.attributeName1 != null && shopitems.Item.attributeName1 != "")
                            cmd.Parameters.AddWithValue("ITEM_ATT_1", (shopitems.Item.attributeName1).ToString());
                        else
                            cmd.Parameters.AddWithValue("ITEM_ATT_1", "");

                        if (shopitems.Item.attributeName2 != null && shopitems.Item.attributeName2 != "")
                            cmd.Parameters.AddWithValue("ITEM_ATT_2", (shopitems.Item.attributeName2).ToString());
                        else
                            cmd.Parameters.AddWithValue("ITEM_ATT_2", "");

                        if (shopitems.Item.attribute1Values != null && shopitems.Item.attribute1Values != "")
                            cmd.Parameters.AddWithValue("attribute1", (shopitems.Item.attribute1Values).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("attribute1", "");
                        }
                        if (shopitems.Item.attribute2Values != null && shopitems.Item.attribute2Values != "")
                            cmd.Parameters.AddWithValue("attribute2", (shopitems.Item.attribute2Values).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("attribute2", "");
                        }
                        if (shopitems.Item.systemSku != null && shopitems.Item.systemSku != "")
                            cmd.Parameters.AddWithValue("systemSku", (shopitems.Item.systemSku).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("systemSku", "");
                        }

                        if (shopitems.Item.BrandName != null && shopitems.Item.BrandName != "")
                            cmd.Parameters.AddWithValue("BrandName", (shopitems.Item.BrandName).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("BrandName", "");
                        }

                        if (shopitems.Item.reorderLevel != null && shopitems.Item.reorderLevel != "")
                            cmd.Parameters.AddWithValue("reorderLevel", (shopitems.Item.reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("reorderLevel", "");
                        }
                        if (shopitems.Item.reorderPoint != null && shopitems.Item.reorderPoint != "")
                            cmd.Parameters.AddWithValue("reorderPoint", (shopitems.Item.reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("reorderPoint", "");
                        }
                        if (shopitems.Item.shopID != null && shopitems.Item.shopID != "")
                            cmd.Parameters.AddWithValue("shopID", (shopitems.Item.shopID).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shopID", "");
                        }
                        if (shopitems.Item.qoh != null && shopitems.Item.qoh != "")
                            cmd.Parameters.AddWithValue("qoh", (shopitems.Item.qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("qoh", "");
                        }


                        if (shopitems.Item.shop_0_qoh != null && shopitems.Item.shop_0_qoh != "")
                        {
                            cmd.Parameters.AddWithValue("shop_0_qoh", (shopitems.Item.shop_0_qoh).ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_0_qoh", "");
                        }

                        if (shopitems.Item.shop_1_qoh != null && shopitems.Item.shop_1_qoh != "")
                        {
                            cmd.Parameters.AddWithValue("shop_1_qoh", (shopitems.Item.shop_1_qoh).ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_1_qoh", "");
                        }


                        if (shopitems.Item.shop_2_qoh != null && shopitems.Item.shop_2_qoh != "")
                            cmd.Parameters.AddWithValue("shop_2_qoh", (shopitems.Item.shop_2_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_2_qoh", "");
                        }

                        if (shopitems.Item.shop_3_qoh != null && shopitems.Item.shop_3_qoh != "")
                            cmd.Parameters.AddWithValue("shop_3_qoh", (shopitems.Item.shop_3_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_3_qoh", "");
                        }

                        if (shopitems.Item.shop_4_qoh != null && shopitems.Item.shop_4_qoh != "")
                            cmd.Parameters.AddWithValue("shop_4_qoh", (shopitems.Item.shop_4_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_4_qoh", "");
                        }

                        if (shopitems.Item.shop_5_qoh != null && shopitems.Item.shop_5_qoh != "")
                            cmd.Parameters.AddWithValue("shop_5_qoh", (shopitems.Item.shop_5_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_5_qoh", "");
                        }

                        if (shopitems.Item.shop_6_qoh != null && shopitems.Item.shop_6_qoh != "")
                            cmd.Parameters.AddWithValue("shop_6_qoh", (shopitems.Item.shop_6_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_6_qoh", "");
                        }

                        if (shopitems.Item.shop_7_qoh != null && shopitems.Item.shop_7_qoh != "")
                            cmd.Parameters.AddWithValue("shop_7_qoh", (shopitems.Item.shop_7_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_7_qoh", "");
                        }
                        if (shopitems.Item.shop_8_qoh != null && shopitems.Item.shop_8_qoh != "")
                            cmd.Parameters.AddWithValue("shop_8_qoh", (shopitems.Item.shop_8_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_8_qoh", "");
                        }
                        if (shopitems.Item.shop_9_qoh != null && shopitems.Item.shop_9_qoh != "")
                            cmd.Parameters.AddWithValue("shop_9_qoh", (shopitems.Item.shop_9_qoh).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_9_qoh", "");
                        }

                        if (shopitems.Item.shop_0_reorderLevel != null && shopitems.Item.shop_0_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_0_reorderLevel", (shopitems.Item.shop_0_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_0_reorderLevel", "");
                        }

                        if (shopitems.Item.shop_1_reorderLevel != null && shopitems.Item.shop_1_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_1_reorderLevel", (shopitems.Item.shop_1_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_1_reorderLevel", "");
                        }
                        if (shopitems.Item.shop_2_reorderLevel != null && shopitems.Item.shop_2_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_2_reorderLevel", (shopitems.Item.shop_2_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_2_reorderLevel", "");
                        }
                        if (shopitems.Item.shop_3_reorderLevel != null && shopitems.Item.shop_3_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_3_reorderLevel", (shopitems.Item.shop_3_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_3_reorderLevel", "");
                        }

                        if (shopitems.Item.shop_4_reorderLevel != null && shopitems.Item.shop_4_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_4_reorderLevel", (shopitems.Item.shop_4_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_4_reorderLevel", "");
                        }

                        if (shopitems.Item.shop_5_reorderLevel != null && shopitems.Item.shop_5_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_5_reorderLevel", (shopitems.Item.shop_5_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_5_reorderLevel", "");
                        }
                        if (shopitems.Item.shop_6_reorderLevel != null && shopitems.Item.shop_6_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_6_reorderLevel", (shopitems.Item.shop_6_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_6_reorderLevel", "");
                        }
                        if (shopitems.Item.shop_7_reorderLevel != null && shopitems.Item.shop_7_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_7_reorderLevel", (shopitems.Item.shop_7_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_7_reorderLevel", "");
                        }
                        if (shopitems.Item.shop_8_reorderLevel != null && shopitems.Item.shop_8_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_8_reorderLevel", (shopitems.Item.shop_8_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_8_reorderLevel", "");
                        }

                        if (shopitems.Item.shop_9_reorderLevel != null && shopitems.Item.shop_9_reorderLevel != "")
                            cmd.Parameters.AddWithValue("shop_9_reorderLevel", (shopitems.Item.shop_9_reorderLevel).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_9_reorderLevel", "");
                        }

                        if (shopitems.Item.shop_0_reorderPoint != null && shopitems.Item.shop_0_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_0_reorderPoint", (shopitems.Item.shop_0_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_0_reorderPoint", "");
                        }

                        if (shopitems.Item.shop_1_reorderPoint != null && shopitems.Item.shop_1_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_1_reorderPoint", (shopitems.Item.shop_1_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_1_reorderPoint", "");
                        }
                        if (shopitems.Item.shop_2_reorderPoint != null && shopitems.Item.shop_2_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_2_reorderPoint", (shopitems.Item.shop_2_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_2_reorderPoint", "");
                        }
                        if (shopitems.Item.shop_3_reorderPoint != null && shopitems.Item.shop_3_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_3_reorderPoint", (shopitems.Item.shop_3_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_3_reorderPoint", "");
                        }

                        if (shopitems.Item.shop_4_reorderPoint != null && shopitems.Item.shop_4_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_4_reorderPoint", (shopitems.Item.shop_4_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_4_reorderPoint", "");
                        }
                        if (shopitems.Item.shop_5_reorderPoint != null && shopitems.Item.shop_5_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_5_reorderPoint", (shopitems.Item.shop_5_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_5_reorderPoint", "");
                        }

                        if (shopitems.Item.shop_6_reorderPoint != null && shopitems.Item.shop_6_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_6_reorderPoint", (shopitems.Item.shop_6_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_6_reorderPoint", "");
                        }

                        if (shopitems.Item.shop_7_reorderPoint != null && shopitems.Item.shop_7_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_7_reorderPoint", (shopitems.Item.shop_7_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_7_reorderPoint", "");
                        }

                        if (shopitems.Item.shop_8_reorderPoint != null && shopitems.Item.shop_8_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_8_reorderPoint", (shopitems.Item.shop_8_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_8_reorderPoint", "");
                        }

                        if (shopitems.Item.shop_9_reorderPoint != null && shopitems.Item.shop_9_reorderPoint != "")
                            cmd.Parameters.AddWithValue("shop_9_reorderPoint", (shopitems.Item.shop_9_reorderPoint).ToString());
                        else
                        {
                            cmd.Parameters.AddWithValue("shop_9_reorderPoint", "");
                        }




                        //if (shopitems.Item[Rowscount].manufacturerSku != null && shopitems.Item[Rowscount].manufacturerSku != "")
                        //    cmd.Parameters.AddWithValue("ITEM_SID", (shopitems.Item[Rowscount].manufacturerSku).ToString());
                        //else
                        //    cmd.Parameters.AddWithValue("ITEM_SID", "");

                        cmd.Connection = cnn;
                        cmd.ExecuteNonQuery();
                    }

                    else
                    {
                        MySqlCommand cmd2 = new MySqlCommand();
                        cmd2.Connection = cnn;
                        Double upc;
                        Int32 LSITEM_ID;
                        Int32 MATRIX_ID;
                        Decimal Cost;
                        Int32 ITEM_ATT_SET_ID;
                        string ALU;
                        string Brand;
                        string ITEM_NAME;
                        string ITEM_ATT_1;
                        string ITEM_ATT_2;
                        string ITEM_ATT_SET_NAME;
                        Decimal Price;
                        decimal MSRP;
                        string manufacturerSKU;
                        DateTime CreatedDate;
                        string attribute1;
                        string attribute2;
                        string systemSku;
                        string BrandName;
                        string reorderLevel;
                        string reorderPoint;
                        string shopID;
                        string qoh;
                        string Category;
                        string shop_0_qoh;
                        string shop_1_qoh;
                        string shop_2_qoh;
                        string shop_3_qoh;
                        string shop_4_qoh;
                        string shop_5_qoh;
                        string shop_6_qoh;
                        string shop_7_qoh;
                        string shop_8_qoh;
                        string shop_9_qoh;
                        string shop_0_reorderLevel;
                        string shop_1_reorderLevel;
                        string shop_2_reorderLevel;
                        string shop_3_reorderLevel;
                        string shop_4_reorderLevel;
                        string shop_5_reorderLevel;
                        string shop_6_reorderLevel;
                        string shop_7_reorderLevel;
                        string shop_8_reorderLevel;
                        string shop_9_reorderLevel;
                        string shop_0_reorderPoint;
                        string shop_1_reorderPoint;
                        string shop_2_reorderPoint;
                        string shop_3_reorderPoint;
                        string shop_4_reorderPoint;
                        string shop_5_reorderPoint;
                        string shop_6_reorderPoint;
                        string shop_7_reorderPoint;
                        string shop_8_reorderPoint;
                        string shop_9_reorderPoint;

                        //Int32 MATRIX_ID;
                        //if ((shopitems.Item[Rowscount].itemMatrixID).ToString() != "")
                        //    cmd.Parameters.AddWithValue("MATRIX_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemMatrixID));
                        //else
                        //    cmd.Parameters.AddWithValue("MATRIX_ID", "");
                        //   var a = Convert.ToInt64(shopitems.Item[Rowscount].);

                        CreatedDate = shopitems.Item.createTime;
                        if (shopitems.Item.manufacturerSku == "")
                        {
                            manufacturerSKU = "0";
                        }
                        else
                        {
                            manufacturerSKU = shopitems.Item.manufacturerSku;
                        }

                        if (shopitems.Item.itemID == "")
                        {
                            LSITEM_ID = 0;
                        }
                        else
                        {
                            LSITEM_ID = Convert.ToInt32(shopitems.Item.itemID);
                        }


                        if (shopitems.Item.upc == "")
                        {
                            upc = 0;
                        }
                        else
                        {
                            upc = Convert.ToDouble(shopitems.Item.upc);
                        }

                        if (shopitems.Item.itemMatrixID == "")
                        {
                            MATRIX_ID = 0;
                        }
                        else
                        {
                            MATRIX_ID = Convert.ToInt32(shopitems.Item.itemMatrixID);
                        }

                        if (shopitems.Item.defaultCost == "")
                        {
                            Cost = 0;
                        }
                        else
                        {
                            Cost = Convert.ToDecimal(shopitems.Item.defaultCost);
                        }

                        if (shopitems.Item.itemAttributeSetID == "")
                        {
                            ITEM_ATT_SET_ID = 0;
                        }
                        else
                        {
                            ITEM_ATT_SET_ID = Convert.ToInt32(shopitems.Item.itemAttributeSetID);
                        }
                        if (shopitems.Item.customSku == "" && shopitems.Item.customSku == null)
                        {
                            ALU = "";
                        }
                        else
                        {
                            ALU = shopitems.Item.customSku;
                        }


                        if (shopitems.Item.manufacturerID == "" && shopitems.Item.manufacturerID == null)
                        {
                            Brand = "";
                        }
                        else
                        {
                            Brand = shopitems.Item.manufacturerID.ToString();
                        }

                        if (shopitems.Item.description == "")
                        {
                            ITEM_NAME = "";
                        }
                        else
                        {
                            ITEM_NAME = shopitems.Item.description;
                        }

                        if (shopitems.Item.attributeName1 == "")
                        {
                            ITEM_ATT_1 = "";
                        }
                        else
                        {
                            ITEM_ATT_1 = shopitems.Item.attributeName1;
                        }

                        if (shopitems.Item.attributeName2 == "" && shopitems.Item.attributeName2 == null)
                        {
                            ITEM_ATT_2 = "";
                        }
                        else
                        {
                            ITEM_ATT_2 = shopitems.Item.attributeName2;
                        }

                        if (shopitems.Item.name == "" && shopitems.Item.name == null)
                        {
                            ITEM_ATT_SET_NAME = "";
                        }
                        else
                        {
                            ITEM_ATT_SET_NAME = shopitems.Item.name;
                        }

                        if (shopitems.Item.Prices.ItemPrice.Count != 0)
                        {
                            Price = Convert.ToDecimal(shopitems.Item.Prices.ItemPrice[0].amount);
                        }
                        else
                        {
                            Price = 0;
                        }
                        if (shopitems.Item.Prices.ItemPrice.Count != 0)
                        {
                            MSRP = Convert.ToDecimal(shopitems.Item.Prices.ItemPrice[1].amount);
                        }
                        else
                        {
                            MSRP = 0;
                        }
                        if (shopitems.Item.attribute1Values == "" || shopitems.Item.attribute1Values == null)
                        {
                            attribute1 = "";
                        }
                        else
                        {
                            attribute1 = shopitems.Item.attribute1Values.ToString();
                        }
                        if (shopitems.Item.attribute2Values == "" || shopitems.Item.attribute2Values == null)
                        {
                            attribute2 = "";
                        }
                        else
                        {
                            attribute2 = shopitems.Item.attribute2Values.ToString();
                        }


                        if (shopitems.Item.systemSku == null && shopitems.Item.systemSku == "")
                        {
                            systemSku = "";
                        }
                        else
                        {
                            systemSku = shopitems.Item.systemSku.ToString();
                        }

                        if (shopitems.Item.BrandName == null && shopitems.Item.BrandName == "")
                        {
                            BrandName = "";
                        }
                        else
                        {
                            BrandName = shopitems.Item.BrandName.ToString();
                        }

                        if (shopitems.Item.shop_0_qoh == null && shopitems.Item.shop_0_qoh == "")
                        {
                            shop_0_qoh = "";
                        }
                        else
                        {
                            shop_0_qoh = shopitems.Item.shop_0_qoh.ToString();
                        }


                        if (shopitems.Item.shop_1_qoh == null && shopitems.Item.shop_1_qoh == "")
                        {
                            shop_1_qoh = "";
                        }
                        else
                        {
                            shop_1_qoh = shopitems.Item.shop_1_qoh.ToString();
                        }

                        if (shopitems.Item.shop_2_qoh == null && shopitems.Item.shop_2_qoh == "")
                        {
                            shop_2_qoh = "";
                        }
                        else
                        {
                            shop_2_qoh = shopitems.Item.shop_2_qoh.ToString();
                        }


                        if (shopitems.Item.shop_3_qoh == null && shopitems.Item.shop_3_qoh == "")
                        {
                            shop_3_qoh = "";
                        }
                        else
                        {
                            shop_3_qoh = shopitems.Item.shop_3_qoh.ToString();
                        }

                        if (shopitems.Item.shop_4_qoh == null && shopitems.Item.shop_4_qoh == "")
                        {
                            shop_4_qoh = "";
                        }
                        else
                        {
                            shop_4_qoh = shopitems.Item.shop_4_qoh.ToString();
                        }
                        if (shopitems.Item.shop_5_qoh == null && shopitems.Item.shop_5_qoh == "")
                        {
                            shop_5_qoh = "";
                        }
                        else
                        {
                            shop_5_qoh = shopitems.Item.shop_5_qoh.ToString();
                        }

                        if (shopitems.Item.shop_6_qoh == null && shopitems.Item.shop_6_qoh == "")
                        {
                            shop_6_qoh = "";
                        }
                        else
                        {
                            shop_6_qoh = shopitems.Item.shop_6_qoh.ToString();
                        }

                        if (shopitems.Item.shop_7_qoh == null && shopitems.Item.shop_7_qoh == "")
                        {
                            shop_7_qoh = "";
                        }
                        else
                        {
                            shop_7_qoh = shopitems.Item.shop_7_qoh.ToString();
                        }

                        if (shopitems.Item.shop_8_qoh == null && shopitems.Item.shop_8_qoh == "")
                        {
                            shop_8_qoh = "";
                        }
                        else
                        {
                            shop_8_qoh = shopitems.Item.shop_8_qoh.ToString();
                        }

                        if (shopitems.Item.shop_9_qoh == null && shopitems.Item.shop_9_qoh == "")
                        {
                            shop_9_qoh = "";
                        }
                        else
                        {
                            shop_9_qoh = shopitems.Item.shop_9_qoh.ToString();
                        }

                        if (shopitems.Item.shop_0_reorderLevel == null && shopitems.Item.shop_0_reorderLevel == "")
                        {
                            shop_0_reorderLevel = "";
                        }
                        else
                        {
                            shop_0_reorderLevel = shopitems.Item.shop_0_reorderLevel.ToString();
                        }

                        if (shopitems.Item.shop_1_reorderLevel == null && shopitems.Item.shop_1_reorderLevel == "")
                        {
                            shop_1_reorderLevel = "";
                        }
                        else
                        {
                            shop_1_reorderLevel = shopitems.Item.shop_1_reorderLevel.ToString();
                        }
                        if (shopitems.Item.shop_2_reorderLevel == null && shopitems.Item.shop_2_reorderLevel == "")
                        {
                            shop_2_reorderLevel = "";
                        }
                        else
                        {
                            shop_2_reorderLevel = shopitems.Item.shop_2_reorderLevel.ToString();
                        }

                        if (shopitems.Item.shop_3_reorderLevel == null && shopitems.Item.shop_3_reorderLevel == "")
                        {
                            shop_3_reorderLevel = "";
                        }
                        else
                        {
                            shop_3_reorderLevel = shopitems.Item.shop_3_reorderLevel.ToString();
                        }


                        if (shopitems.Item.shop_4_reorderLevel == null && shopitems.Item.shop_4_reorderLevel == "")
                        {
                            shop_4_reorderLevel = "";
                        }
                        else
                        {
                            shop_4_reorderLevel = shopitems.Item.shop_4_reorderLevel.ToString();
                        }


                        if (shopitems.Item.shop_5_reorderLevel == null && shopitems.Item.shop_5_reorderLevel == "")
                        {
                            shop_5_reorderLevel = "";
                        }
                        else
                        {
                            shop_5_reorderLevel = shopitems.Item.shop_5_reorderLevel.ToString();
                        }

                        if (shopitems.Item.shop_6_reorderLevel == null && shopitems.Item.shop_6_reorderLevel == "")
                        {
                            shop_6_reorderLevel = "";
                        }
                        else
                        {
                            shop_6_reorderLevel = shopitems.Item.shop_6_reorderLevel.ToString();
                        }
                        if (shopitems.Item.shop_7_reorderLevel == null && shopitems.Item.shop_7_reorderLevel == "")
                        {
                            shop_7_reorderLevel = "";
                        }
                        else
                        {
                            shop_7_reorderLevel = shopitems.Item.shop_7_reorderLevel.ToString();
                        }
                        if (shopitems.Item.shop_8_reorderLevel == null && shopitems.Item.shop_8_reorderLevel == "")
                        {
                            shop_8_reorderLevel = "";
                        }
                        else
                        {
                            shop_8_reorderLevel = shopitems.Item.shop_8_reorderLevel.ToString();
                        }
                        if (shopitems.Item.shop_9_reorderLevel == null && shopitems.Item.shop_9_reorderLevel == "")
                        {
                            shop_9_reorderLevel = "";
                        }
                        else
                        {
                            shop_9_reorderLevel = shopitems.Item.shop_9_reorderLevel.ToString();
                        }


                        if (shopitems.Item.shop_0_reorderPoint == null && shopitems.Item.shop_0_reorderPoint == "")
                        {
                            shop_0_reorderPoint = "";
                        }
                        else
                        {
                            shop_0_reorderPoint = shopitems.Item.shop_0_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_1_reorderPoint == null && shopitems.Item.shop_1_reorderPoint == "")
                        {
                            shop_1_reorderPoint = "";
                        }
                        else
                        {
                            shop_1_reorderPoint = shopitems.Item.shop_1_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_2_reorderPoint == null && shopitems.Item.shop_2_reorderPoint == "")
                        {
                            shop_2_reorderPoint = "";
                        }
                        else
                        {
                            shop_2_reorderPoint = shopitems.Item.shop_2_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_3_reorderPoint == null && shopitems.Item.shop_3_reorderPoint == "")
                        {
                            shop_3_reorderPoint = "";
                        }
                        else
                        {
                            shop_3_reorderPoint = shopitems.Item.shop_3_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_4_reorderPoint == null && shopitems.Item.shop_4_reorderPoint == "")
                        {
                            shop_4_reorderPoint = "";
                        }
                        else
                        {
                            shop_4_reorderPoint = shopitems.Item.shop_4_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_5_reorderPoint == null && shopitems.Item.shop_5_reorderPoint == "")
                        {
                            shop_5_reorderPoint = "";
                        }
                        else
                        {
                            shop_5_reorderPoint = shopitems.Item.shop_5_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_6_reorderPoint == null && shopitems.Item.shop_6_reorderPoint == "")
                        {
                            shop_6_reorderPoint = "";
                        }
                        else
                        {
                            shop_6_reorderPoint = shopitems.Item.shop_6_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_7_reorderPoint == null && shopitems.Item.shop_7_reorderPoint == "")
                        {
                            shop_7_reorderPoint = "";
                        }
                        else
                        {
                            shop_7_reorderPoint = shopitems.Item.shop_7_reorderPoint.ToString();
                        }
                        if (shopitems.Item.shop_8_reorderPoint == null && shopitems.Item.shop_8_reorderPoint == "")
                        {
                            shop_8_reorderPoint = "";
                        }
                        else
                        {
                            shop_8_reorderPoint = shopitems.Item.shop_8_reorderPoint.ToString();
                        }

                        if (shopitems.Item.shop_9_reorderPoint == null && shopitems.Item.shop_9_reorderPoint == "")
                        {
                            shop_9_reorderPoint = "";
                        }
                        else
                        {
                            shop_9_reorderPoint = shopitems.Item.shop_9_reorderPoint.ToString();
                        }


                        if (shopitems.Item.reorderLevel == null && shopitems.Item.reorderLevel == "")
                        {
                            reorderLevel = "";
                        }
                        else
                        {
                            reorderLevel = shopitems.Item.reorderLevel.ToString();
                        }
                        if (shopitems.Item.reorderPoint == null && shopitems.Item.reorderPoint == "")
                        {
                            reorderPoint = "";
                        }
                        else
                        {
                            reorderPoint = shopitems.Item.reorderPoint.ToString();
                        }
                        if (shopitems.Item.shopID == null && shopitems.Item.shopID == "")
                        {
                            shopID = "";
                        }
                        else
                        {
                            shopID = shopitems.Item.shopID.ToString();
                        }
                        if (shopitems.Item.qoh == null && shopitems.Item.qoh == "")
                            qoh = "";
                        else
                        {
                            qoh = shopitems.Item.qoh.ToString();
                        }
                        //if (shopitems.Item[Rowscount].description == "")
                        //{
                        //    ITEM_ATT_SET_NAME = "";
                        //}
                        //else
                        //{
                        //    ITEM_ATT_SET_NAME = shopitems.Item[Rowscount].description.ToString();
                        //}
                        // string updateQuery = "UPDATE INVENTORY SET LSITEM_ID = '" + LSITEM_ID + "', UPC = '" + upc + "', ALU = '" + shopitems.Item[Rowscount].customSku.ToString() + "' , BRAND = '" + shopitems.Item[Rowscount].manufacturerID.ToString() + "', MATRIX_ID = '" + MATRIX_ID + "', COST = '" + Cost + "', ITEM_NAME = '" + shopitems.Item[Rowscount].description + "', ITEM_ATT_SET_ID = '" + ITEM_ATT_SET_ID + "', ITEM_ATT_SET_NAME = '" + shopitems.Item[Rowscount].name + "', ITEM_ATT_1 = '" + shopitems.Item[Rowscount].attributeName1.ToString() + "', ITEM_ATT_2 = '" + (shopitems.Item[Rowscount].attributeName2).ToString() + "' WHERE ITEM_SID="+ Convert.ToInt64(shopitems.Item[Rowscount].manufacturerSku);
                        string updateQuery = "UPDATE INVENTORY SET CreatedDate= @CreatedDate, ITEM_SID=@manufacturerSKU , UPC =@upc, ALU = @ALU , BRAND = @Brand, MATRIX_ID = @MATRIX_ID , COST = @Cost , ITEM_NAME = @ITEM_NAME, ITEM_ATT_SET_ID = @ITEM_ATT_SET_ID , ITEM_ATT_SET_NAME = @ITEM_ATT_SET_NAME ,Price=@Price, ITEM_ATT_1 = @ITEM_ATT_1, ITEM_ATT_2 = @ITEM_ATT_2,MSRP=@MSRP, qoh=@qoh,shopID=@shopID, reorderPoint=@reorderPoint ,reorderLevel=@reorderLevel,BrandName=@BrandName,systemSku=@systemSku,attribute1=@attribute1,attribute2=@attribute2,shop_0_reorderLevel=@shop_0_reorderLevel ,shop_1_reorderLevel=@shop_1_reorderLevel,shop_2_reorderLevel=@shop_2_reorderLevel ,shop_3_reorderLevel=@shop_3_reorderLevel,shop_4_reorderLevel=@shop_4_reorderLevel,shop_5_reorderLevel=@shop_5_reorderLevel,shop_6_reorderLevel=@shop_6_reorderLevel,shop_7_reorderLevel=@shop_7_reorderLevel,shop_8_reorderLevel=@shop_8_reorderLevel,shop_9_reorderLevel=@shop_9_reorderLevel,shop_0_qoh=@shop_0_qoh,shop_1_qoh=@shop_1_qoh ,shop_2_qoh=@shop_2_qoh,shop_3_qoh=@shop_3_qoh,shop_4_qoh=@shop_4_qoh,shop_5_qoh=@shop_5_qoh,shop_6_qoh=@shop_6_qoh,shop_7_qoh=@shop_7_qoh,shop_8_qoh=@shop_8_qoh,shop_9_qoh=@shop_9_qoh,shop_0_reorderPoint=@shop_0_reorderPoint,shop_1_reorderPoint=@shop_1_reorderPoint,shop_2_reorderPoint=@shop_2_reorderPoint,shop_3_reorderPoint=@shop_3_reorderPoint,shop_4_reorderPoint=@shop_4_reorderPoint,shop_5_reorderPoint=@shop_5_reorderPoint,shop_6_reorderPoint=@shop_6_reorderPoint,shop_7_reorderPoint=@shop_7_reorderPoint,shop_8_reorderPoint=@shop_8_reorderPoint,shop_9_reorderPoint=@shop_9_reorderPoint  WHERE LSITEM_ID=" + LSITEM_ID;
                        cmd2.Parameters.AddWithValue("@manufacturerSKU", manufacturerSKU);
                        cmd2.Parameters.AddWithValue("@upc", upc);
                        cmd2.Parameters.AddWithValue("@ALU", ALU);
                        cmd2.Parameters.AddWithValue("@Brand", Brand);
                        cmd2.Parameters.AddWithValue("@MATRIX_ID", MATRIX_ID);
                        cmd2.Parameters.AddWithValue("@Cost", Cost);
                        cmd2.Parameters.AddWithValue("@ITEM_NAME", ITEM_NAME);
                        cmd2.Parameters.AddWithValue("@ITEM_ATT_SET_ID", ITEM_ATT_SET_ID);
                        cmd2.Parameters.AddWithValue("@ITEM_ATT_1", ITEM_ATT_1);
                        cmd2.Parameters.AddWithValue("@ITEM_ATT_2", ITEM_ATT_2);
                        cmd2.Parameters.AddWithValue("@ITEM_ATT_SET_NAME", ITEM_ATT_SET_NAME);
                        cmd2.Parameters.AddWithValue("@Price", Price);
                        cmd2.Parameters.AddWithValue("@MSRP", MSRP);
                        cmd2.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd2.Parameters.AddWithValue("@attribute1", attribute1);
                        cmd2.Parameters.AddWithValue("@attribute2", attribute2);
                        cmd2.Parameters.AddWithValue("@systemSku", systemSku);
                        cmd2.Parameters.AddWithValue("@BrandName", BrandName);
                        cmd2.Parameters.AddWithValue("@reorderLevel", reorderLevel);
                        cmd2.Parameters.AddWithValue("@reorderPoint", reorderPoint);
                        cmd2.Parameters.AddWithValue("@qoh", qoh);
                        cmd2.Parameters.AddWithValue("@shopID", shopID);

                        cmd2.Parameters.AddWithValue("@shop_0_qoh", shop_0_qoh);
                        cmd2.Parameters.AddWithValue("@shop_0_reorderPoint", shop_0_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_0_reorderLevel", shop_0_reorderLevel);

                        cmd2.Parameters.AddWithValue("@shop_1_qoh", shop_1_qoh);
                        cmd2.Parameters.AddWithValue("@shop_1_reorderLevel", shop_1_reorderLevel);
                        cmd2.Parameters.AddWithValue("@shop_1_reorderPoint", shop_1_reorderPoint);

                        cmd2.Parameters.AddWithValue("@shop_2_qoh", shop_2_qoh);
                        cmd2.Parameters.AddWithValue("@shop_2_reorderPoint", shop_2_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_2_reorderLevel", shop_2_reorderLevel);


                        cmd2.Parameters.AddWithValue("@shop_3_qoh", shop_3_qoh);
                        cmd2.Parameters.AddWithValue("@shop_3_reorderPoint", shop_3_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_3_reorderLevel", shop_3_reorderLevel);

                        cmd2.Parameters.AddWithValue("@shop_4_qoh", shop_4_qoh);
                        cmd2.Parameters.AddWithValue("@shop_4_reorderPoint", shop_4_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_4_reorderLevel", shop_4_reorderLevel);

                        cmd2.Parameters.AddWithValue("@shop_5_qoh", shop_5_qoh);
                        cmd2.Parameters.AddWithValue("@shop_5_reorderPoint", shop_5_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_5_reorderLevel", shop_5_reorderLevel);

                        cmd2.Parameters.AddWithValue("@shop_6_qoh", shop_6_qoh);
                        cmd2.Parameters.AddWithValue("@shop_6_reorderPoint", shop_6_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_6_reorderLevel", shop_6_reorderLevel);

                        cmd2.Parameters.AddWithValue("@shop_7_qoh", shop_7_qoh);
                        cmd2.Parameters.AddWithValue("@shop_7_reorderPoint", shop_7_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_7_reorderLevel", shop_7_reorderLevel);


                        cmd2.Parameters.AddWithValue("@shop_8_qoh", shop_8_qoh);
                        cmd2.Parameters.AddWithValue("@shop_8_reorderPoint", shop_8_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_8_reorderLevel", shop_8_reorderLevel);

                        cmd2.Parameters.AddWithValue("@shop_9_qoh", shop_9_qoh);
                        cmd2.Parameters.AddWithValue("@shop_9_reorderPoint", shop_9_reorderPoint);
                        cmd2.Parameters.AddWithValue("@shop_9_reorderLevel", shop_9_reorderLevel);


                        cmd2.CommandText = updateQuery;
                        cmd2.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
                catch (Exception ex)
                {
                }
            }
            else
            {

            }
        }

        private static void InsertInventoryItems(Iteminfo shopitems)
        {

            for (Int32 Rowscount = 0; Rowscount <= shopitems.Item.Count() - 1; Rowscount++)
            {
                string connetionString = null;
                MySqlConnection cnn;
                connetionString = "Server = rproods.cluster-c2vodxkdsl4p.us-east-1.rds.amazonaws.com; Port = 3306; Database = RPROODS; Uid = reportuser; Pwd = fuykA4LH; ";
                cnn = new MySqlConnection(connetionString);
                DataSet ds = new DataSet();
                var itemid = shopitems.Item[Rowscount].itemID;
                if (itemid != "")
                {
                    try
                    {
                        cnn.Open();
                        string query1 = "select * from INVENTORY where LSITEM_ID=" + shopitems.Item[Rowscount].itemID;
                        MySqlCommand cmd1 = new MySqlCommand(query1);
                        cmd1.Connection = cnn;
                        DataSet dsn = new DataSet();
                        MySqlDataAdapter adp1 = new MySqlDataAdapter();
                        adp1.SelectCommand = cmd1;
                        //var a = adp1;
                        adp1.Fill(dsn);
                        var ccoun = dsn.Tables[0].Rows.Count;
                        if (ccoun == 0)
                        {
                            string query = "Insert into INVENTORY(CreatedDate,LSITEM_ID,Category,attribute1 ,attribute2,UPC,ALU,ITEM_SID,BRAND,MATRIX_ID,COST,ITEM_NAME,ITEM_ATT_SET_ID,ITEM_ATT_SET_NAME,ITEM_ATT_1,ITEM_ATT_2,Price,MSRP,systemSku,BrandName ,reorderLevel,reorderPoint,shopID,qoh,shop_0_qoh,shop_0_reorderLevel,shop_0_reorderPoint ,shop_1_qoh,shop_1_reorderLevel,shop_1_reorderPoint, shop_2_qoh,shop_2_reorderLevel,shop_2_reorderPoint,shop_3_qoh,shop_3_reorderLevel,shop_3_reorderPoint,shop_4_qoh,shop_4_reorderLevel,shop_4_reorderPoint,shop_5_qoh,shop_5_reorderLevel,shop_5_reorderPoint,shop_6_qoh,shop_6_reorderLevel,shop_6_reorderPoint,shop_7_qoh,shop_7_reorderLevel,shop_7_reorderPoint,shop_8_qoh,shop_8_reorderLevel,shop_8_reorderPoint,shop_9_qoh,shop_9_reorderLevel,shop_9_reorderPoint) Values (@CreatedDate,@LSITEM_ID,@Category,@attribute1,@attribute2,@UPC,@ALU,@ITEM_SID,@BRAND,@MATRIX_ID,@COST,@ITEM_NAME,@ITEM_ATT_SET_ID,@ITEM_ATT_SET_NAME,@ITEM_ATT_1,@ITEM_ATT_2,@Price,@MSRP,@systemSku,@BrandName,@reorderLevel,@reorderPoint,@shopID,@qoh,shop_0_qoh,shop_0_reorderLevel,shop_0_reorderPoint,@shop_1_qoh,@shop_1_reorderLevel,@shop_1_reorderPoint,@shop_2_qoh,@shop_2_reorderLevel,@shop_2_reorderPoint,@shop_3_qoh,@shop_3_reorderLevel,@shop_3_reorderPoint,@shop_4_qoh,@shop_4_reorderLevel,@shop_4_reorderPoint,@shop_5_qoh,@shop_5_reorderLevel,@shop_5_reorderPoint,@shop_6_qoh,@shop_6_reorderLevel,@shop_6_reorderPoint,@shop_7_qoh,@shop_7_reorderLevel,@shop_7_reorderPoint,@shop_8_qoh,@shop_8_reorderLevel,@shop_8_reorderPoint,@shop_9_qoh,@shop_9_reorderLevel,@shop_9_reorderPoint)";
                            // string query = "Insert into INVENTORY(CreatedDate,LSITEM_ID,CATEGORY,UPC,ALU,ITEM_SID,BRAND,MATRIX_ID,COST,ITEM_NAME,ITEM_ATT_SET_ID,ITEM_ATT_SET_NAME,ITEM_ATT_1,ITEM_ATT_2,Price,MSRP,MATRIX_ID) Values (@CreatedDate,@LSITEM_ID,@CATEGORY,@UPC,@ALU,@ITEM_SID,@BRAND,@MATRIX_ID,@COST,@ITEM_NAME,@ITEM_ATT_SET_ID,@ITEM_ATT_SET_NAME,@ITEM_ATT_1,@ITEM_ATT_2,@Price,@MSRP,@MATRIX_ID)";
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("LSITEM_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemID));
                            //if ((shopitems.Item[Rowscount].itemMatrixID).ToString() != "")
                            //    cmd.Parameters.AddWithValue("MATRIX_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemMatrixID));
                            //else
                            //    cmd.Parameters.AddWithValue("MATRIX_ID", "");
                            if ((shopitems.Item[Rowscount].createTime).ToString() != "")
                                cmd.Parameters.AddWithValue("CreatedDate", Convert.ToDateTime(shopitems.Item[Rowscount].createTime));
                            else
                                cmd.Parameters.AddWithValue("CreatedDate", "");
                            if (shopitems.Item[Rowscount].categoryID != "")
                                cmd.Parameters.AddWithValue("CATEGORY", shopitems.Item[Rowscount].categoryID.ToString());
                            else
                                cmd.Parameters.AddWithValue("CATEGORY", "");
                            if (shopitems.Item[Rowscount].upc != "")
                                cmd.Parameters.AddWithValue("UPC", Convert.ToDouble(shopitems.Item[Rowscount].upc));
                            else
                                cmd.Parameters.AddWithValue("UPC", "");
                            if (shopitems.Item[Rowscount].customSku != null && shopitems.Item[Rowscount].customSku != "")
                                cmd.Parameters.AddWithValue("ALU", shopitems.Item[Rowscount].customSku.ToString());
                            else
                                cmd.Parameters.AddWithValue("ALU", "");
                            if (shopitems.Item[Rowscount].manufacturerSku != null && shopitems.Item[Rowscount].manufacturerSku != "")
                                cmd.Parameters.AddWithValue("ITEM_SID", shopitems.Item[Rowscount].manufacturerSku);
                            else
                                cmd.Parameters.AddWithValue("ITEM_SID", 0);

                            if (shopitems.Item[Rowscount].manufacturerID != null && shopitems.Item[Rowscount].manufacturerID != "")
                                cmd.Parameters.AddWithValue("BRAND", (shopitems.Item[Rowscount].manufacturerID).ToString());
                            else
                                cmd.Parameters.AddWithValue("BRAND", "");

                            if (shopitems.Item[Rowscount].itemMatrixID != null && shopitems.Item[Rowscount].itemMatrixID != "")
                                cmd.Parameters.AddWithValue("MATRIX_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemMatrixID));
                            else
                                cmd.Parameters.AddWithValue("MATRIX_ID", "");
                            if (shopitems.Item[Rowscount].defaultCost != null && shopitems.Item[Rowscount].defaultCost != "")
                                cmd.Parameters.AddWithValue("COST", Convert.ToDecimal(shopitems.Item[Rowscount].defaultCost));
                            else
                                cmd.Parameters.AddWithValue("COST", "");

                            if (shopitems.Item[Rowscount].Prices.ItemPrice.Count != 0)
                            {
                                cmd.Parameters.AddWithValue("Price", Convert.ToDecimal(shopitems.Item[Rowscount].Prices.ItemPrice[0].amount));
                            }
                            else
                                cmd.Parameters.AddWithValue("Price", "");
                            if (shopitems.Item[Rowscount].Prices.ItemPrice.Count != 0)
                            {
                                cmd.Parameters.AddWithValue("MSRP", Convert.ToDecimal(shopitems.Item[Rowscount].Prices.ItemPrice[1].amount));
                            }
                            else
                                cmd.Parameters.AddWithValue("MSRP", "");

                            //cmd.Parameters.AddWithValue("PRICE", Convert.ToDecimal(shopitems.Item[0].co));
                            if (shopitems.Item[Rowscount].description != null && shopitems.Item[Rowscount].description != "")
                                cmd.Parameters.AddWithValue("ITEM_NAME", (shopitems.Item[Rowscount].description).ToString());
                            else
                                cmd.Parameters.AddWithValue("ITEM_NAME", "");

                            if (shopitems.Item[Rowscount].itemAttributeSetID != null && shopitems.Item[Rowscount].itemAttributeSetID != "")
                                cmd.Parameters.AddWithValue("ITEM_ATT_SET_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemAttributeSetID));
                            else
                                cmd.Parameters.AddWithValue("ITEM_ATT_SET_ID", "");

                            if (shopitems.Item[Rowscount].name != null && shopitems.Item[Rowscount].name != "")
                                cmd.Parameters.AddWithValue("ITEM_ATT_SET_NAME", (shopitems.Item[Rowscount].name).ToString());
                            else
                                cmd.Parameters.AddWithValue("ITEM_ATT_SET_NAME", "");

                            if (shopitems.Item[Rowscount].attributeName1 != null && shopitems.Item[Rowscount].attributeName1 != "")
                                cmd.Parameters.AddWithValue("ITEM_ATT_1", (shopitems.Item[Rowscount].attributeName1).ToString());
                            else
                                cmd.Parameters.AddWithValue("ITEM_ATT_1", "");

                            if (shopitems.Item[Rowscount].attributeName2 != null && shopitems.Item[Rowscount].attributeName2 != "")
                                cmd.Parameters.AddWithValue("ITEM_ATT_2", (shopitems.Item[Rowscount].attributeName2).ToString());
                            else
                                cmd.Parameters.AddWithValue("ITEM_ATT_2", "");

                            if (shopitems.Item[Rowscount].attribute1Values != null && shopitems.Item[Rowscount].attribute1Values != "")
                                cmd.Parameters.AddWithValue("attribute1", (shopitems.Item[Rowscount].attribute1Values).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("attribute1", "");
                            }
                            if (shopitems.Item[Rowscount].attribute2Values != null && shopitems.Item[Rowscount].attribute2Values != "")
                                cmd.Parameters.AddWithValue("attribute2", (shopitems.Item[Rowscount].attribute2Values).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("attribute2", "");
                            }
                            if (shopitems.Item[Rowscount].systemSku != null && shopitems.Item[Rowscount].systemSku != "")
                                cmd.Parameters.AddWithValue("systemSku", (shopitems.Item[Rowscount].systemSku).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("systemSku", "");
                            }

                            if (shopitems.Item[Rowscount].BrandName != null && shopitems.Item[Rowscount].BrandName != "")
                                cmd.Parameters.AddWithValue("BrandName", (shopitems.Item[Rowscount].BrandName).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("BrandName", "");
                            }

                            if (shopitems.Item[Rowscount].reorderLevel != null && shopitems.Item[Rowscount].reorderLevel != "")
                                cmd.Parameters.AddWithValue("reorderLevel", (shopitems.Item[Rowscount].reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].reorderPoint != null && shopitems.Item[Rowscount].reorderPoint != "")
                                cmd.Parameters.AddWithValue("reorderPoint", (shopitems.Item[Rowscount].reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("reorderPoint", "");
                            }
                            if (shopitems.Item[Rowscount].shopID != null && shopitems.Item[Rowscount].shopID != "")
                                cmd.Parameters.AddWithValue("shopID", (shopitems.Item[Rowscount].shopID).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shopID", "");
                            }
                            if (shopitems.Item[Rowscount].qoh != null && shopitems.Item[Rowscount].qoh != "")
                                cmd.Parameters.AddWithValue("qoh", (shopitems.Item[Rowscount].qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_0_qoh != null && shopitems.Item[Rowscount].shop_0_qoh != "")
                                cmd.Parameters.AddWithValue("shop_0_qoh", (shopitems.Item[Rowscount].shop_0_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_0_qoh", "");
                            }


                            if (shopitems.Item[Rowscount].shop_1_qoh != null && shopitems.Item[Rowscount].shop_1_qoh != "")
                                cmd.Parameters.AddWithValue("shop_1_qoh", (shopitems.Item[Rowscount].shop_1_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_1_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_2_qoh != null && shopitems.Item[Rowscount].shop_2_qoh != "")
                                cmd.Parameters.AddWithValue("shop_2_qoh", (shopitems.Item[Rowscount].shop_2_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_2_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_3_qoh != null && shopitems.Item[Rowscount].shop_3_qoh != "")
                                cmd.Parameters.AddWithValue("shop_3_qoh", (shopitems.Item[Rowscount].shop_3_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_3_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_4_qoh != null && shopitems.Item[Rowscount].shop_4_qoh != "")
                                cmd.Parameters.AddWithValue("shop_4_qoh", (shopitems.Item[Rowscount].shop_4_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_4_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_5_qoh != null && shopitems.Item[Rowscount].shop_5_qoh != "")
                                cmd.Parameters.AddWithValue("shop_5_qoh", (shopitems.Item[Rowscount].shop_5_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_5_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_6_qoh != null && shopitems.Item[Rowscount].shop_6_qoh != "")
                                cmd.Parameters.AddWithValue("shop_6_qoh", (shopitems.Item[Rowscount].shop_6_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_6_qoh", "");
                            }
                            if (shopitems.Item[Rowscount].shop_7_qoh != null && shopitems.Item[Rowscount].shop_7_qoh != "")
                                cmd.Parameters.AddWithValue("shop_7_qoh", (shopitems.Item[Rowscount].shop_7_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_7_qoh", "");
                            }
                            if (shopitems.Item[Rowscount].shop_8_qoh != null && shopitems.Item[Rowscount].shop_8_qoh != "")
                                cmd.Parameters.AddWithValue("shop_8_qoh", (shopitems.Item[Rowscount].shop_8_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_8_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_9_qoh != null && shopitems.Item[Rowscount].shop_9_qoh != "")
                                cmd.Parameters.AddWithValue("shop_9_qoh", (shopitems.Item[Rowscount].shop_9_qoh).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_9_qoh", "");
                            }

                            if (shopitems.Item[Rowscount].shop_0_reorderLevel != null && shopitems.Item[Rowscount].shop_0_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_0_reorderLevel", (shopitems.Item[Rowscount].shop_0_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_0_reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].shop_1_reorderLevel != null && shopitems.Item[Rowscount].shop_1_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_1_reorderLevel", (shopitems.Item[Rowscount].shop_1_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_1_reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].shop_2_reorderLevel != null && shopitems.Item[Rowscount].shop_2_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_2_reorderLevel", (shopitems.Item[Rowscount].shop_2_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_2_reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].shop_3_reorderLevel != null && shopitems.Item[Rowscount].shop_3_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_3_reorderLevel", (shopitems.Item[Rowscount].shop_3_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_3_reorderLevel", "");
                            }

                            if (shopitems.Item[Rowscount].shop_4_reorderLevel != null && shopitems.Item[Rowscount].shop_4_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_4_reorderLevel", (shopitems.Item[Rowscount].shop_4_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_4_reorderLevel", "");
                            }

                            if (shopitems.Item[Rowscount].shop_5_reorderLevel != null && shopitems.Item[Rowscount].shop_5_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_5_reorderLevel", (shopitems.Item[Rowscount].shop_5_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_5_reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].shop_6_reorderLevel != null && shopitems.Item[Rowscount].shop_6_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_6_reorderLevel", (shopitems.Item[Rowscount].shop_6_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_6_reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].shop_7_reorderLevel != null && shopitems.Item[Rowscount].shop_7_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_7_reorderLevel", (shopitems.Item[Rowscount].shop_7_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_7_reorderLevel", "");
                            }
                            if (shopitems.Item[Rowscount].shop_8_reorderLevel != null && shopitems.Item[Rowscount].shop_8_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_8_reorderLevel", (shopitems.Item[Rowscount].shop_8_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_8_reorderLevel", "");
                            }

                            if (shopitems.Item[Rowscount].shop_9_reorderLevel != null && shopitems.Item[Rowscount].shop_9_reorderLevel != "")
                                cmd.Parameters.AddWithValue("shop_9_reorderLevel", (shopitems.Item[Rowscount].shop_9_reorderLevel).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_9_reorderLevel", "");
                            }

                            if (shopitems.Item[Rowscount].shop_0_reorderPoint != null && shopitems.Item[Rowscount].shop_0_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_0_reorderPoint", (shopitems.Item[Rowscount].shop_0_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_0_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_1_reorderPoint != null && shopitems.Item[Rowscount].shop_1_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_1_reorderPoint", (shopitems.Item[Rowscount].shop_1_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_1_reorderPoint", "");
                            }
                            if (shopitems.Item[Rowscount].shop_2_reorderPoint != null && shopitems.Item[Rowscount].shop_2_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_2_reorderPoint", (shopitems.Item[Rowscount].shop_2_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_2_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_3_reorderPoint != null && shopitems.Item[Rowscount].shop_3_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_3_reorderPoint", (shopitems.Item[Rowscount].shop_3_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_3_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_4_reorderPoint != null && shopitems.Item[Rowscount].shop_4_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_4_reorderPoint", (shopitems.Item[Rowscount].shop_4_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_4_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_5_reorderPoint != null && shopitems.Item[Rowscount].shop_5_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_5_reorderPoint", (shopitems.Item[Rowscount].shop_5_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_5_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_6_reorderPoint != null && shopitems.Item[Rowscount].shop_6_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_6_reorderPoint", (shopitems.Item[Rowscount].shop_6_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_6_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_7_reorderPoint != null && shopitems.Item[Rowscount].shop_7_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_7_reorderPoint", (shopitems.Item[Rowscount].shop_7_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_7_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_8_reorderPoint != null && shopitems.Item[Rowscount].shop_8_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_8_reorderPoint", (shopitems.Item[Rowscount].shop_8_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_8_reorderPoint", "");
                            }

                            if (shopitems.Item[Rowscount].shop_9_reorderPoint != null && shopitems.Item[Rowscount].shop_9_reorderPoint != "")
                                cmd.Parameters.AddWithValue("shop_9_reorderPoint", (shopitems.Item[Rowscount].shop_9_reorderPoint).ToString());
                            else
                            {
                                cmd.Parameters.AddWithValue("shop_9_reorderPoint", "");
                            }




                            //if (shopitems.Item[Rowscount].manufacturerSku != null && shopitems.Item[Rowscount].manufacturerSku != "")
                            //    cmd.Parameters.AddWithValue("ITEM_SID", (shopitems.Item[Rowscount].manufacturerSku).ToString());
                            //else
                            //    cmd.Parameters.AddWithValue("ITEM_SID", "");

                            cmd.Connection = cnn;
                            cmd.ExecuteNonQuery();
                        }

                        else
                        {
                            MySqlCommand cmd2 = new MySqlCommand();
                            cmd2.Connection = cnn;
                            Double upc;
                            Int32 LSITEM_ID;
                            Int32 MATRIX_ID;
                            Decimal Cost;
                            Int32 ITEM_ATT_SET_ID;
                            string ALU;
                            string Brand;
                            string ITEM_NAME;
                            string ITEM_ATT_1;
                            string ITEM_ATT_2;
                            string ITEM_ATT_SET_NAME;
                            Decimal Price;
                            decimal MSRP;
                            string manufacturerSKU;
                            DateTime CreatedDate;
                            string attribute1;
                            string attribute2;
                            string systemSku;
                            string BrandName;
                            string reorderLevel;
                            string reorderPoint;
                            string shopID;
                            string qoh;
                            string Category;
                            string shop_0_qoh;
                            string shop_1_qoh;
                            string shop_2_qoh;
                            string shop_3_qoh;
                            string shop_4_qoh;
                            string shop_5_qoh;
                            string shop_6_qoh;
                            string shop_7_qoh;
                            string shop_8_qoh;
                            string shop_9_qoh;
                            string shop_0_reorderLevel;
                            string shop_1_reorderLevel;
                            string shop_2_reorderLevel;
                            string shop_3_reorderLevel;
                            string shop_4_reorderLevel;
                            string shop_5_reorderLevel;
                            string shop_6_reorderLevel;
                            string shop_7_reorderLevel;
                            string shop_8_reorderLevel;
                            string shop_9_reorderLevel;
                            string shop_0_reorderPoint;
                            string shop_1_reorderPoint;
                            string shop_2_reorderPoint;
                            string shop_3_reorderPoint;
                            string shop_4_reorderPoint;
                            string shop_5_reorderPoint;
                            string shop_6_reorderPoint;
                            string shop_7_reorderPoint;
                            string shop_8_reorderPoint;
                            string shop_9_reorderPoint;




                            //Int32 MATRIX_ID;
                            //if ((shopitems.Item[Rowscount].itemMatrixID).ToString() != "")
                            //    cmd.Parameters.AddWithValue("MATRIX_ID", Convert.ToInt32(shopitems.Item[Rowscount].itemMatrixID));
                            //else
                            //    cmd.Parameters.AddWithValue("MATRIX_ID", "");
                            //   var a = Convert.ToInt64(shopitems.Item[Rowscount].);

                            CreatedDate = shopitems.Item[Rowscount].createTime;
                            if (shopitems.Item[Rowscount].manufacturerSku == "")
                            {
                                manufacturerSKU = "0";
                            }
                            else
                            {
                                manufacturerSKU = shopitems.Item[Rowscount].manufacturerSku;
                            }

                            if (shopitems.Item[Rowscount].itemID == "")
                            {
                                LSITEM_ID = 0;
                            }
                            else
                            {
                                LSITEM_ID = Convert.ToInt32(shopitems.Item[Rowscount].itemID);
                            }


                            if (shopitems.Item[Rowscount].upc == "")
                            {
                                upc = 0;
                            }
                            else
                            {
                                upc = Convert.ToDouble(shopitems.Item[Rowscount].upc);
                            }

                            if (shopitems.Item[Rowscount].itemMatrixID == "")
                            {
                                MATRIX_ID = 0;
                            }
                            else
                            {
                                MATRIX_ID = Convert.ToInt32(shopitems.Item[Rowscount].itemMatrixID);
                            }

                            if (shopitems.Item[Rowscount].defaultCost == "")
                            {
                                Cost = 0;
                            }
                            else
                            {
                                Cost = Convert.ToDecimal(shopitems.Item[Rowscount].defaultCost);
                            }

                            if (shopitems.Item[Rowscount].itemAttributeSetID == "")
                            {
                                ITEM_ATT_SET_ID = 0;
                            }
                            else
                            {
                                ITEM_ATT_SET_ID = Convert.ToInt32(shopitems.Item[Rowscount].itemAttributeSetID);
                            }
                            if (shopitems.Item[Rowscount].customSku == "" && shopitems.Item[Rowscount].customSku == null)
                            {
                                ALU = "";
                            }
                            else
                            {
                                ALU = shopitems.Item[Rowscount].customSku;
                            }


                            if (shopitems.Item[Rowscount].manufacturerID == "" && shopitems.Item[Rowscount].manufacturerID == null)
                            {
                                Brand = "";
                            }
                            else
                            {
                                Brand = shopitems.Item[Rowscount].manufacturerID.ToString();
                            }

                            if (shopitems.Item[Rowscount].description == "")
                            {
                                ITEM_NAME = "";
                            }
                            else
                            {
                                ITEM_NAME = shopitems.Item[Rowscount].description;
                            }

                            if (shopitems.Item[Rowscount].attributeName1 == "")
                            {
                                ITEM_ATT_1 = "";
                            }
                            else
                            {
                                ITEM_ATT_1 = shopitems.Item[Rowscount].attributeName1;
                            }

                            if (shopitems.Item[Rowscount].attributeName2 == "" && shopitems.Item[Rowscount].attributeName2 == null)
                            {
                                ITEM_ATT_2 = "";
                            }
                            else
                            {
                                ITEM_ATT_2 = shopitems.Item[Rowscount].attributeName2;
                            }

                            if (shopitems.Item[Rowscount].name == "" && shopitems.Item[Rowscount].name == null)
                            {
                                ITEM_ATT_SET_NAME = "";
                            }
                            else
                            {
                                ITEM_ATT_SET_NAME = shopitems.Item[Rowscount].name;
                            }

                            if (shopitems.Item[Rowscount].Prices.ItemPrice.Count != 0)
                            {
                                Price = Convert.ToDecimal(shopitems.Item[Rowscount].Prices.ItemPrice[0].amount);
                            }
                            else
                            {
                                Price = 0;
                            }
                            if (shopitems.Item[Rowscount].Prices.ItemPrice.Count != 0)
                            {
                                MSRP = Convert.ToDecimal(shopitems.Item[Rowscount].Prices.ItemPrice[1].amount);
                            }
                            else
                            {
                                MSRP = 0;
                            }
                            if (shopitems.Item[Rowscount].attribute1Values == "" || shopitems.Item[Rowscount].attribute1Values == null)
                            {
                                attribute1 = "";
                            }
                            else
                            {
                                attribute1 = shopitems.Item[Rowscount].attribute1Values.ToString();
                            }
                            if (shopitems.Item[Rowscount].attribute2Values == "" || shopitems.Item[Rowscount].attribute2Values == null)
                            {
                                attribute2 = "";
                            }
                            else
                            {
                                attribute2 = shopitems.Item[Rowscount].attribute2Values.ToString();
                            }


                            if (shopitems.Item[Rowscount].systemSku == null && shopitems.Item[Rowscount].systemSku == "")
                            {
                                systemSku = "";
                            }
                            else
                            {
                                systemSku = shopitems.Item[Rowscount].systemSku.ToString();
                            }

                            if (shopitems.Item[Rowscount].BrandName == null || shopitems.Item[Rowscount].BrandName == "")
                            {
                                BrandName = "";
                            }
                            else
                            {
                                BrandName = shopitems.Item[Rowscount].BrandName.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_0_qoh == null && shopitems.Item[Rowscount].shop_0_qoh == "")
                            {
                                shop_0_qoh = "";
                            }
                            else
                            {
                                shop_0_qoh = shopitems.Item[Rowscount].shop_0_qoh.ToString();
                            }


                            if (shopitems.Item[Rowscount].shop_1_qoh == null && shopitems.Item[Rowscount].shop_1_qoh == "")
                            {
                                shop_1_qoh = "";
                            }
                            else
                            {
                                shop_1_qoh = shopitems.Item[Rowscount].shop_1_qoh.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_2_qoh == null && shopitems.Item[Rowscount].shop_2_qoh == "")
                            {
                                shop_2_qoh = "";
                            }
                            else
                            {
                                shop_2_qoh = shopitems.Item[Rowscount].shop_2_qoh.ToString();
                            }


                            if (shopitems.Item[Rowscount].shop_3_qoh == null && shopitems.Item[Rowscount].shop_3_qoh == "")
                            {
                                shop_3_qoh = "";
                            }
                            else
                            {
                                shop_3_qoh = shopitems.Item[Rowscount].shop_3_qoh.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_4_qoh == null && shopitems.Item[Rowscount].shop_4_qoh == "")
                            {
                                shop_4_qoh = "";
                            }
                            else
                            {
                                shop_4_qoh = shopitems.Item[Rowscount].shop_4_qoh.ToString();
                            }
                            if (shopitems.Item[Rowscount].shop_5_qoh == null && shopitems.Item[Rowscount].shop_5_qoh == "")
                            {
                                shop_5_qoh = "";
                            }
                            else
                            {
                                shop_5_qoh = shopitems.Item[Rowscount].shop_5_qoh.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_6_qoh == null && shopitems.Item[Rowscount].shop_6_qoh == "")
                            {
                                shop_6_qoh = "";
                            }
                            else
                            {
                                shop_6_qoh = shopitems.Item[Rowscount].shop_6_qoh.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_7_qoh == null && shopitems.Item[Rowscount].shop_7_qoh == "")
                            {
                                shop_7_qoh = "";
                            }
                            else
                            {
                                shop_7_qoh = shopitems.Item[Rowscount].shop_7_qoh.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_8_qoh == null && shopitems.Item[Rowscount].shop_8_qoh == "")
                            {
                                shop_8_qoh = "";
                            }
                            else
                            {
                                shop_8_qoh = shopitems.Item[Rowscount].shop_8_qoh.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_9_qoh == null && shopitems.Item[Rowscount].shop_9_qoh == "")
                            {
                                shop_9_qoh = "";
                            }
                            else
                            {
                                shop_9_qoh = shopitems.Item[Rowscount].shop_9_qoh.ToString();
                            }


                            if (shopitems.Item[Rowscount].shop_0_reorderLevel == null && shopitems.Item[Rowscount].shop_0_reorderLevel == "")
                            {
                                shop_0_reorderLevel = "";
                            }
                            else
                            {
                                shop_0_reorderLevel = shopitems.Item[Rowscount].shop_0_reorderLevel.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_1_reorderLevel == null && shopitems.Item[Rowscount].shop_1_reorderLevel == "")
                            {
                                shop_1_reorderLevel = "";
                            }
                            else
                            {
                                shop_1_reorderLevel = shopitems.Item[Rowscount].shop_1_reorderLevel.ToString();
                            }
                            if (shopitems.Item[Rowscount].shop_2_reorderLevel == null && shopitems.Item[Rowscount].shop_2_reorderLevel == "")
                            {
                                shop_2_reorderLevel = "";
                            }
                            else
                            {
                                shop_2_reorderLevel = shopitems.Item[Rowscount].shop_2_reorderLevel.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_3_reorderLevel == null && shopitems.Item[Rowscount].shop_3_reorderLevel == "")
                            {
                                shop_3_reorderLevel = "";
                            }
                            else
                            {
                                shop_3_reorderLevel = shopitems.Item[Rowscount].shop_3_reorderLevel.ToString();
                            }


                            if (shopitems.Item[Rowscount].shop_4_reorderLevel == null && shopitems.Item[Rowscount].shop_4_reorderLevel == "")
                            {
                                shop_4_reorderLevel = "";
                            }
                            else
                            {
                                shop_4_reorderLevel = shopitems.Item[Rowscount].shop_4_reorderLevel.ToString();
                            }


                            if (shopitems.Item[Rowscount].shop_5_reorderLevel == null && shopitems.Item[Rowscount].shop_5_reorderLevel == "")
                            {
                                shop_5_reorderLevel = "";
                            }
                            else
                            {
                                shop_5_reorderLevel = shopitems.Item[Rowscount].shop_5_reorderLevel.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_6_reorderLevel == null && shopitems.Item[Rowscount].shop_6_reorderLevel == "")
                            {
                                shop_6_reorderLevel = "";
                            }
                            else
                            {
                                shop_6_reorderLevel = shopitems.Item[Rowscount].shop_6_reorderLevel.ToString();
                            }
                            if (shopitems.Item[Rowscount].shop_7_reorderLevel == null && shopitems.Item[Rowscount].shop_7_reorderLevel == "")
                            {
                                shop_7_reorderLevel = "";
                            }
                            else
                            {
                                shop_7_reorderLevel = shopitems.Item[Rowscount].shop_7_reorderLevel.ToString();
                            }
                            if (shopitems.Item[Rowscount].shop_8_reorderLevel == null && shopitems.Item[Rowscount].shop_8_reorderLevel == "")
                            {
                                shop_8_reorderLevel = "";
                            }
                            else
                            {
                                shop_8_reorderLevel = shopitems.Item[Rowscount].shop_8_reorderLevel.ToString();
                            }
                            if (shopitems.Item[Rowscount].shop_9_reorderLevel == null && shopitems.Item[Rowscount].shop_9_reorderLevel == "")
                            {
                                shop_9_reorderLevel = "";
                            }
                            else
                            {
                                shop_9_reorderLevel = shopitems.Item[Rowscount].shop_9_reorderLevel.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_0_reorderPoint == null && shopitems.Item[Rowscount].shop_0_reorderPoint == "")
                            {
                                shop_0_reorderPoint = "";
                            }
                            else
                            {
                                shop_0_reorderPoint = shopitems.Item[Rowscount].shop_0_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_1_reorderPoint == null && shopitems.Item[Rowscount].shop_1_reorderPoint == "")
                            {
                                shop_1_reorderPoint = "";
                            }
                            else
                            {
                                shop_1_reorderPoint = shopitems.Item[Rowscount].shop_1_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_2_reorderPoint == null && shopitems.Item[Rowscount].shop_2_reorderPoint == "")
                            {
                                shop_2_reorderPoint = "";
                            }
                            else
                            {
                                shop_2_reorderPoint = shopitems.Item[Rowscount].shop_2_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_3_reorderPoint == null && shopitems.Item[Rowscount].shop_3_reorderPoint == "")
                            {
                                shop_3_reorderPoint = "";
                            }
                            else
                            {
                                shop_3_reorderPoint = shopitems.Item[Rowscount].shop_3_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_4_reorderPoint == null && shopitems.Item[Rowscount].shop_4_reorderPoint == "")
                            {
                                shop_4_reorderPoint = "";
                            }
                            else
                            {
                                shop_4_reorderPoint = shopitems.Item[Rowscount].shop_4_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_5_reorderPoint == null && shopitems.Item[Rowscount].shop_5_reorderPoint == "")
                            {
                                shop_5_reorderPoint = "";
                            }
                            else
                            {
                                shop_5_reorderPoint = shopitems.Item[Rowscount].shop_5_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_6_reorderPoint == null && shopitems.Item[Rowscount].shop_6_reorderPoint == "")
                            {
                                shop_6_reorderPoint = "";
                            }
                            else
                            {
                                shop_6_reorderPoint = shopitems.Item[Rowscount].shop_6_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_7_reorderPoint == null && shopitems.Item[Rowscount].shop_7_reorderPoint == "")
                            {
                                shop_7_reorderPoint = "";
                            }
                            else
                            {
                                shop_7_reorderPoint = shopitems.Item[Rowscount].shop_7_reorderPoint.ToString();
                            }
                            if (shopitems.Item[Rowscount].shop_8_reorderPoint == null && shopitems.Item[Rowscount].shop_8_reorderPoint == "")
                            {
                                shop_8_reorderPoint = "";
                            }
                            else
                            {
                                shop_8_reorderPoint = shopitems.Item[Rowscount].shop_8_reorderPoint.ToString();
                            }

                            if (shopitems.Item[Rowscount].shop_9_reorderPoint == null && shopitems.Item[Rowscount].shop_9_reorderPoint == "")
                            {
                                shop_9_reorderPoint = "";
                            }
                            else
                            {
                                shop_9_reorderPoint = shopitems.Item[Rowscount].shop_9_reorderPoint.ToString();
                            }



                            if (shopitems.Item[Rowscount].reorderLevel == null && shopitems.Item[Rowscount].reorderLevel == "")
                            {
                                reorderLevel = "";
                            }
                            else
                            {
                                reorderLevel = shopitems.Item[Rowscount].reorderLevel.ToString();
                            }
                            if (shopitems.Item[Rowscount].reorderPoint == null && shopitems.Item[Rowscount].reorderPoint == "")
                            {
                                reorderPoint = "";
                            }
                            else
                            {
                                reorderPoint = shopitems.Item[Rowscount].reorderPoint.ToString();
                            }
                            if (shopitems.Item[Rowscount].shopID == null && shopitems.Item[Rowscount].shopID == "")
                            {
                                shopID = "";
                            }
                            else
                            {
                                shopID = shopitems.Item[Rowscount].shopID.ToString();
                            }
                            if (shopitems.Item[Rowscount].qoh == null && shopitems.Item[Rowscount].qoh == "")
                                qoh = "";
                            else
                            {
                                qoh = shopitems.Item[Rowscount].qoh.ToString();
                            }
                            //if (shopitems.Item[Rowscount].description == "")
                            //{
                            //    ITEM_ATT_SET_NAME = "";
                            //}
                            //else
                            //{
                            //    ITEM_ATT_SET_NAME = shopitems.Item[Rowscount].description.ToString();
                            //}
                            // string updateQuery = "UPDATE INVENTORY SET LSITEM_ID = '" + LSITEM_ID + "', UPC = '" + upc + "', ALU = '" + shopitems.Item[Rowscount].customSku.ToString() + "' , BRAND = '" + shopitems.Item[Rowscount].manufacturerID.ToString() + "', MATRIX_ID = '" + MATRIX_ID + "', COST = '" + Cost + "', ITEM_NAME = '" + shopitems.Item[Rowscount].description + "', ITEM_ATT_SET_ID = '" + ITEM_ATT_SET_ID + "', ITEM_ATT_SET_NAME = '" + shopitems.Item[Rowscount].name + "', ITEM_ATT_1 = '" + shopitems.Item[Rowscount].attributeName1.ToString() + "', ITEM_ATT_2 = '" + (shopitems.Item[Rowscount].attributeName2).ToString() + "' WHERE ITEM_SID="+ Convert.ToInt64(shopitems.Item[Rowscount].manufacturerSku);
                            string updateQuery = "UPDATE INVENTORY SET CreatedDate= @CreatedDate, ITEM_SID=@manufacturerSKU , UPC =@upc, ALU = @ALU , BRAND = @Brand, MATRIX_ID = @MATRIX_ID , COST = @Cost , ITEM_NAME = @ITEM_NAME, ITEM_ATT_SET_ID = @ITEM_ATT_SET_ID , ITEM_ATT_SET_NAME = @ITEM_ATT_SET_NAME ,Price=@Price, ITEM_ATT_1 = @ITEM_ATT_1, ITEM_ATT_2 = @ITEM_ATT_2,MSRP=@MSRP, qoh=@qoh,shopID=@shopID, reorderPoint=@reorderPoint ,reorderLevel=@reorderLevel,BrandName=@BrandName,systemSku=@systemSku,attribute1=@attribute1,attribute2=@attribute2 ,shop_1_reorderLevel=@shop_1_reorderLevel,shop_2_reorderLevel=@shop_2_reorderLevel ,shop_3_reorderLevel=@shop_3_reorderLevel,shop_4_reorderLevel=@shop_4_reorderLevel,shop_5_reorderLevel=@shop_5_reorderLevel,shop_6_reorderLevel=@shop_6_reorderLevel,shop_7_reorderLevel=@shop_7_reorderLevel,shop_8_reorderLevel=@shop_8_reorderLevel,shop_9_reorderLevel=@shop_9_reorderLevel,shop_0_reorderLevel=@shop_0_reorderLevel ,shop_1_qoh=@shop_1_qoh ,shop_2_qoh=@shop_2_qoh,shop_3_qoh=@shop_3_qoh,shop_4_qoh=@shop_4_qoh,shop_5_qoh=@shop_5_qoh,shop_6_qoh=@shop_6_qoh,shop_7_qoh=@shop_7_qoh,shop_8_qoh=@shop_8_qoh,shop_9_qoh=@shop_9_qoh,shop_0_qoh=@shop_0_qoh,shop_1_reorderPoint=@shop_1_reorderPoint,shop_2_reorderPoint=@shop_2_reorderPoint,shop_3_reorderPoint=@shop_3_reorderPoint,shop_4_reorderPoint=@shop_4_reorderPoint,shop_5_reorderPoint=@shop_5_reorderPoint,shop_6_reorderPoint=@shop_6_reorderPoint,shop_7_reorderPoint=@shop_7_reorderPoint,shop_8_reorderPoint=@shop_8_reorderPoint,shop_9_reorderPoint=@shop_9_reorderPoint,shop_0_reorderPoint=@shop_0_reorderPoint  WHERE LSITEM_ID=" + LSITEM_ID;
                            cmd2.Parameters.AddWithValue("@manufacturerSKU", manufacturerSKU);
                            cmd2.Parameters.AddWithValue("@upc", upc);
                            cmd2.Parameters.AddWithValue("@ALU", ALU);
                            cmd2.Parameters.AddWithValue("@Brand", Brand);
                            cmd2.Parameters.AddWithValue("@MATRIX_ID", MATRIX_ID);
                            cmd2.Parameters.AddWithValue("@Cost", Cost);
                            cmd2.Parameters.AddWithValue("@ITEM_NAME", ITEM_NAME);
                            cmd2.Parameters.AddWithValue("@ITEM_ATT_SET_ID", ITEM_ATT_SET_ID);
                            cmd2.Parameters.AddWithValue("@ITEM_ATT_1", ITEM_ATT_1);
                            cmd2.Parameters.AddWithValue("@ITEM_ATT_2", ITEM_ATT_2);
                            cmd2.Parameters.AddWithValue("@ITEM_ATT_SET_NAME", ITEM_ATT_SET_NAME);
                            cmd2.Parameters.AddWithValue("@Price", Price);
                            cmd2.Parameters.AddWithValue("@MSRP", MSRP);
                            cmd2.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                            cmd2.Parameters.AddWithValue("@attribute1", attribute1);
                            cmd2.Parameters.AddWithValue("@attribute2", attribute2);
                            cmd2.Parameters.AddWithValue("@systemSku", systemSku);
                            cmd2.Parameters.AddWithValue("@BrandName", BrandName);
                            cmd2.Parameters.AddWithValue("@reorderLevel", reorderLevel);
                            cmd2.Parameters.AddWithValue("@reorderPoint", reorderPoint);
                            cmd2.Parameters.AddWithValue("@qoh", qoh);
                            cmd2.Parameters.AddWithValue("@shopID", shopID);

                            cmd2.Parameters.AddWithValue("@shop_0_qoh", shop_0_qoh);
                            cmd2.Parameters.AddWithValue("@shop_0_reorderPoint", shop_0_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_0_reorderLevel", shop_0_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_1_qoh", shop_1_qoh);
                            cmd2.Parameters.AddWithValue("@shop_1_reorderLevel", shop_1_reorderLevel);
                            cmd2.Parameters.AddWithValue("@shop_1_reorderPoint", shop_1_reorderPoint);

                            cmd2.Parameters.AddWithValue("@shop_2_qoh", shop_2_qoh);
                            cmd2.Parameters.AddWithValue("@shop_2_reorderPoint", shop_2_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_2_reorderLevel", shop_2_reorderLevel);


                            cmd2.Parameters.AddWithValue("@shop_3_qoh", shop_3_qoh);
                            cmd2.Parameters.AddWithValue("@shop_3_reorderPoint", shop_3_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_3_reorderLevel", shop_3_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_4_qoh", shop_4_qoh);
                            cmd2.Parameters.AddWithValue("@shop_4_reorderPoint", shop_4_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_4_reorderLevel", shop_4_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_5_qoh", shop_5_qoh);
                            cmd2.Parameters.AddWithValue("@shop_5_reorderPoint", shop_5_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_5_reorderLevel", shop_5_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_6_qoh", shop_6_qoh);
                            cmd2.Parameters.AddWithValue("@shop_6_reorderPoint", shop_6_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_6_reorderLevel", shop_6_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_7_qoh", shop_7_qoh);
                            cmd2.Parameters.AddWithValue("@shop_7_reorderPoint", shop_7_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_7_reorderLevel", shop_7_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_8_qoh", shop_8_qoh);
                            cmd2.Parameters.AddWithValue("@shop_8_reorderPoint", shop_8_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_8_reorderLevel", shop_8_reorderLevel);

                            cmd2.Parameters.AddWithValue("@shop_9_qoh", shop_9_qoh);
                            cmd2.Parameters.AddWithValue("@shop_9_reorderPoint", shop_9_reorderPoint);
                            cmd2.Parameters.AddWithValue("@shop_9_reorderLevel", shop_9_reorderLevel);


                            cmd2.CommandText = updateQuery;
                            cmd2.ExecuteNonQuery();
                        }
                        cnn.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                else
                {
                }
            }
        }

        void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            LightSwitchApiCalling();

        }

        protected void btnSync_Click(object sender, EventArgs e)
        {
            LightSwitchApiCalling();
        }
    }
}
