using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace FaunditShippingAddition.Models.DHL
{
    public class ShippingLabel
    {

        public string SiteID { get; set; } 
        public string Password { get; set; } 
        public string ShipperAccountNumber { get; set; } 
        public string Consignee_CompanyName { get; set; }
        public string Consignee_Address1 { get; set; }
        public string Consignee_Address2 { get; set; }
        public string Consignee_AddressNumber { get; set; }
        public string Consignee_City { get; set; }
        public string Consignee_PostalCode { get; set; }
        public string Consignee_CountryCode { get; set; }
        public string Consignee_CountryName { get; set; }
        public string Consignee_Contact_PersonName { get; set; }
        public string Consignee_Contact_PhoneNumber { get; set; }
        public string ItemNumber { get; set; }
        public int Weight { get; set; }
        public string ItemName { get; set; }
        public string ShipperID { get; set; } 
        public string ShipperName { get; set; }
        public string RegisteredAccount { get; set; } 
        public string Shipper_CompanyName { get; set; }
        public string Shipper_Address1 { get; set; }
        public string Shipper_Address2 { get; set; }
        public string Shipper_AddressNumber { get; set; }
        public string Shipper_City { get; set; }
        public string Shipper_PostalCode { get; set; }
        public string Shipper_CountryCode { get; set; }
        public string Shipper_CountryName { get; set; }
        public string Shipper_PhoneNumber { get; set; }
        public string ShipmentType { get; set; }
        public string Duty { get; set; }
        private static string GetLabelRequestXML(ShippingLabel label)
        {
            var xml = "";
           
                xml = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Models/DHL/ShipmentLabel.xml"));
            
            xml = xml.Replace("%%SITEID%%", label.SiteID);
            xml = xml.Replace("%%PASSWORD%%", label.Password);
            xml = xml.Replace("%%SHIPPERACCOUNTNUMBER%%", label.ShipperAccountNumber);
            xml = xml.Replace("%%CONSIGNEE_COMPANYNAME%%", label.Consignee_CompanyName);
            xml = xml.Replace("%%CONSIGNEE_ADDRESS1%%", label.Consignee_Address1 + " " + label.Consignee_AddressNumber);
            xml = xml.Replace("%%CONSIGNEE_ADDRESS2%%", label.Consignee_Address2);
            xml = xml.Replace("%%CONSIGNEE_CITY%%", label.Consignee_City);
            xml = xml.Replace("%%CONSIGNEE_POSTALCODE%%", label.Consignee_PostalCode);
            xml = xml.Replace("%%CONSIGNEE_COUNTRYCODE%%", label.Consignee_CountryCode);
            xml = xml.Replace("%%CONSIGNEE_COUNTRYNAME%%", label.Consignee_CountryName);
            xml = xml.Replace("%%CONSIGNEE_CONTACT_PERSONNAME%%", label.Consignee_Contact_PersonName);
            xml = xml.Replace("%%CONSIGNEE_CONTACT_PHONENUMBER%%", label.Consignee_Contact_PhoneNumber);
            xml = xml.Replace("%%ITEMNUMBER%%", label.ItemNumber);
            xml = xml.Replace("%%WEIGHT%%", label.Weight.ToString());
            xml = xml.Replace("%%PAYMENT_DATE%%", DateTime.Now.ToString("yyyy-MM-dd"));
            xml = xml.Replace("%%ITEM_NAME%%", label.ItemName);
            xml = xml.Replace("%%SHIPPERID%%", label.ShipperID);
            xml = xml.Replace("%%SHIPPERNAME%%", label.ShipperName);
            xml = xml.Replace("%%REGISTEREDACCOUNT%%", label.RegisteredAccount);
            xml = xml.Replace("%%SHIPPER_ADDRESS1%%", label.Shipper_Address1 + " " + label.Shipper_AddressNumber);
            xml = xml.Replace("%%SHIPPER_ADDRESS2%%", label.Shipper_Address2);
            xml = xml.Replace("%%SHIPPER_CITY%%", label.Shipper_City);
            xml = xml.Replace("%%SHIPPER_POSTALCODE%%", label.Shipper_PostalCode);
            xml = xml.Replace("%%SHIPPER_COUNTRYCODE%%", label.Shipper_CountryCode);
            xml = xml.Replace("%%SHIPPER_COUNTRYNAME%%", label.Shipper_CountryName);
            xml = xml.Replace("%%SHIPPER_PHONENUMBER%%", label.Shipper_PhoneNumber);
            xml = xml.Replace("%%SHIPMENT_TYPE%%", label.ShipmentType);
            xml = xml.Replace("%%DUTY%%", label.Duty);
            xml = xml.Replace("%%INSURENCE_VALUE%%", DHL.ShippingLabel.GetInsurenceValue(label.SelectedInsurenceValue));
            xml = xml.Replace("%%CUSTOM_VALUE%%", DHL.ShippingLabel.GetCustomValue(label.SelectedInsurenceValue));
            xml = xml.Replace("%%SPECIAL_SERVICE_TYPE%%", DHL.ShippingLabel.GetShipmentType(label.SelectedInsurenceValue));


            return xml;
        }

        private static string GetInsurenceValue(int selectedInsurenceValue)
        {
            if (selectedInsurenceValue == 1)
            {
                return "0";
            }
            else if (selectedInsurenceValue == 2)
            {
                return "7500";
            }
            else
            {
                return "20000";
            }
        }

        private static string GetCustomValue(int selectedInsurenceValue)
        {
            if (selectedInsurenceValue == 1)
            {
                return "100";
            }
            else if (selectedInsurenceValue == 2)
            {
                return "7500";
            }
            else
            {
                return "20000";
            }
        }

        private static string GetShipmentType(int selectedInsurenceValue)
        {
            if (selectedInsurenceValue == 1)
            {
                return "";
            }
            else
            {
                return "II";
            }
        }

        public int SelectedInsurenceValue { get; set; }
        //public int InsurenceCurrency { get; set; }

        public static byte[] CreateLabel(ShippingLabel dhl, out string awbNumber)
        {
            var shipmentLabel = GetLabelRequestXML(dhl);

            //ShipmentResponse shimentResponse = SendRequest(shipmentLabel);
            var labelData = SendRequest(shipmentLabel, out awbNumber);

            if (labelData.StartsWith("ERROR"))
            {
                //Mail.SendMailOnLabelIssue(dhl, labelData);
                return new byte[] { };
            }
            return Convert.FromBase64String(labelData);
        }

        private static string SendRequest(string reqLabelXml, out string awbNumber)
        {
            ShipmentResponse shipmentResponse = new ShipmentResponse();
            awbNumber = "";

            try
            {
                // Create a request for the URL. 
                // PI End point
                WebRequest request = WebRequest.Create("http://xmlpitest-ea.dhl.com/XMLShippingServlet"); //originally: WebRequest request = WebRequest.Create(Common.DHLEndpoint);

                // If required by the server, set the credentials.
                // request.Credentials = CredentialCache.DefaultCredentials;

                // Wrap the request stream with a text-based writer
                request.Method = "POST";        // Post method
                request.ContentType = "text/xml";

                var stream = request.GetRequestStream();
                TextWriter writer = new StreamWriter(stream);

                writer.Write(reqLabelXml);
                writer.Close();

                // Get the response.
                WebResponse response = request.GetResponse();

                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();



                // Clean up the streams and the response.
                reader.Close();
                response.Close();


                // Display the content.
                Debug.WriteLine(responseFromServer);


                string labelData = "";
                string errorMessage = "";

                //var soapReader = new XmlSerializer(typeof(BookPickupRequestEA));
                //var ns = new XmlSerializerNamespaces();
                //ns.Add("res", "http://www.dhl.com");
                var serializer = new XmlSerializer(typeof(ShipmentResponse));

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseFromServer);

                XmlReader rdr = XmlReader.Create(new System.IO.StringReader(responseFromServer));
                while (rdr.Read())
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                    {
                        if (rdr.LocalName == "OutputImage")
                        {
                            labelData = rdr.ReadInnerXml();
                        }
                        else if (rdr.LocalName == "ConditionData")
                        {
                            labelData = "ERROR: " + rdr.ReadInnerXml();
                        }
                        else if (rdr.LocalName == "AirwayBillNumber")
                        {
                            awbNumber = rdr.ReadInnerXml();
                        }
                    }
                }

                //using (var xmlReader = new StringReader(responseFromServer))
                //{

                //    shipmentResponse = (ShipmentResponse)serializer.Deserialize(xmlReader);
                //}


                return labelData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                //shipmentResponse.LabelImage.OutputFormat = "ERROR!";
                //shipmentResponse.LabelImage.OutputImage = "ERROR!";
                return "ERROR: " + ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }


        public string GetLabelResponseXML()
        {
            return System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Models/DHL/ShipmentResponse.xml"));
        }
    }

    [Serializable]
    [XmlRoot("res:ShipmentResponse", Namespace = "http://www.dhl.com")]
    public class ShipmentResponse
    {
        [XmlElement("AirwayBillNumber")]
        public string AirwayBillNumber { get; set; }
        [XmlElement("LabelImage")]
        public string LabelImage { get; set; }
        //public  LabelImage LabelImage { get; set; }

    }
}
