using FaunditShippingAddition.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FaunditShippingAddition.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        public string GenerateLabel(int ItemID) //Original GenerateLabel(int ItemID)
        {
            //var item = Item.GetItemByID(ItemID);

            //// Get Hotel
            //var hotel = Hotel.GetHotelByHotelID(item.HotelID);

            //// Get Country Code
            //var consigneeCountry = Country.GetCountryFromID(item.CountryID);
            //var hotelCountry = Country.GetCountryFromID(hotel.CountryID);

            //string shipmentType = "";

            //if (hotelCountry.ID == consigneeCountry.ID)
            //    shipmentType = "N";
            //else if (consigneeCountry.Region.ToUpper() == "EU" && hotelCountry.Region.ToUpper() == "EU")
            //    shipmentType = "U";
            //else
            //    shipmentType = "P";

            //string duty = "";

            //if (consigneeCountry.Region.ToUpper() != "EU")
            //    duty = "Y";
            //else
            //    duty = "N";

            //string fullName = item.FirstName + " " + item.LastName;
            //if (fullName.Length > 35)
            //    fullName = fullName.Substring(0, 34);

            //string companyName = "";
            //if (item.CompanyName == null)
            //    companyName = item.FirstName;
            //else if (item.CompanyName == "")
            //    companyName = item.FirstName;
            //else
            //    companyName = item.CompanyName;



            // Create DHL Label from item data --> replaced with test data
            var dhl = new Models.DHL.ShippingLabel
            {
                ItemName = "Item test",
                ItemNumber = "111",

                // Account
                SiteID = "******", // These values have been removed, so the request does not work
                Password = "******",// These values have been removed, so the request does not work
                ShipperAccountNumber = "******",// These values have been removed, so the request does not work
                ShipperID = "******",// These values have been removed, so the request does not work
                RegisteredAccount = "******", // These values have been removed, so the request does not work

                // Consignee
                Consignee_Address1 = "Gabriele-Tergit-Promenade",
                Consignee_AddressNumber = "19",
                Consignee_Address2 = "Scandic Potsdamer Platz",
                Consignee_City = "Berlin",

                Consignee_CompanyName = "Faundit",

                Consignee_Contact_PersonName = "Casper Larsen",
                Consignee_Contact_PhoneNumber = "12345678",
                Consignee_CountryCode = "DE",
                Consignee_CountryName = "Germany",
                Consignee_PostalCode = "10963",

                // Shipper
                ShipperName = "Casper",
                Shipper_Address1 = "Leipziger Str.",
                Shipper_AddressNumber = "106-111",
                Shipper_Address2 = "NH Hotel",
                Shipper_City = "Berlin",
                Shipper_CompanyName = "NH Collection Berlin Mitte",
                Shipper_CountryCode = "DE",
                Shipper_CountryName = "Germany",
                Shipper_PhoneNumber = "12345",
                Shipper_PostalCode = "10117",
                Weight = 2,
                ShipmentType = "N",
                Duty = "N"

                //Extras (add to hotel model etc)
                //AWBNO
                //Shipper_Pickup_Place = hotel.PickupPlace
            };

            string awbNumber = "";
            var pdfBytes = Models.DHL.ShippingLabel.CreateLabel(dhl, out awbNumber);

            // Send email on invalid label
            if (pdfBytes.Length != 0)

                    //Mail.SendMail(item, MailType.Label);


            //Save Shipping label data
            //item.AirwayBillNumber = awbNumber;

            //System.IO.File.WriteAllBytes("C:\\Temp\\myDHL_Label.pdf", pdfBytes);
            //Item.UpdateItemWithShippingLabelAndReciepts(ItemID, 0, pdfBytes, null, null, awbNumber);




            return "<div style=\"text-align: center;\"><br /><br /><br /><br /><br /><br /><h3>New label succesfully created</h3><br /><br /><h4><a href='/Item/Create/?id='+'ItemID'>Go to item details</a></h4></div>";
        }
    }
}