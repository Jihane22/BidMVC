using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BidMVC.Models;
using CrossPlatformLibrary.Geolocation;
namespace BidMVC.Controllers
{
    public class UserController : Controller
    {
        Bid_MVC_DBEntities db = new Bid_MVC_DBEntities();
        SoldInfo soldInfo = new SoldInfo();
        //
        // GET: /User/
        [HttpGet]
        public ActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserRegister(UserInfo userInfo)
        {
            ModelState.Clear();
            if (db.UserInfoes.Any(x => x.UserName == userInfo.UserName || x.Email == userInfo.Email))
            {
                Response.Write("<script>alert('This username or email already exist!!!');</script>");
            }
            else
            {
                db.UserInfoes.Add(userInfo);
                db.SaveChanges();
                Response.Write("<script>alert('User info saved successfully!!!');</script>");
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(UserInfo userInfo)
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

            // Do not suppress prompt, and wait 1000 milliseconds to start.
            watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));

            GeoCoordinate coord = watcher.Position.Location;

            if (coord.IsUnknown != true)
            {
                Console.WriteLine("Lat: {0}, Long: {1}",
                    coord.Latitude,
                    coord.Longitude);
            }
            else
            {
                Console.WriteLine("Unknown latitude and longitude.");
            }

            if (db.UserInfoes.Any(x => x.Email == userInfo.Email && x.Password == userInfo.Password))
            {
                Session["Email"] = userInfo.Email;

                Response.Write("<script>alert('Login successful!!!');</script>");

                //Response.Redirect("~/Product/AddProduct");

                return RedirectToAction("AddProduct", "Product");
            }
            else
            {
                Response.Write("<script>alert('Please check your username and password!!!');</script>");
                return UserLogin();

            }

        }
        public ActionResult UserLogout()
        {
            Session["Email"] = null;
            return RedirectToAction("GridViewAllProducts", "Product");
        }

        [HttpGet]
        public ActionResult UserProduct()
        {
            var email = Session["Email"].ToString();
            var myProductList = db.Products.Where(x => x.UserInfo.Email == email).Select(x => x).ToList();
            var soldProductId = db.SoldInfoes.Select(x => x.ProductId).ToList();

            //var currentProductListTry = myProductList.Where(r => soldProductId.Contains(r.Id)).Select(x => x).ToList();    //list of same Id
            var currentProductList = myProductList.Where(x => soldProductId.All(i => x.Id != i)).ToList();                   //list of not same Id

            return View(currentProductList);
        }

        [HttpGet]
        public ActionResult UserSoldProduct()
        {
            var email = Session["Email"].ToString();
            var myProductList = db.Products.Where(x => x.UserInfo.Email == email).Select(x => x).ToList();
            var soldProductId = db.SoldInfoes.Select(x => x.ProductId).ToList();

            var soldProductList = myProductList.Where(r => soldProductId.Contains(r.Id)).Select(x => x).ToList();              //list of same Id
            //var currentProductList = myProductList.Where(x => soldProductId.All(i => x.Id != i)).ToList();                   //list of not same Id

            return View(soldProductList);
        }

        [HttpGet]
        public ActionResult AllBiderList(int? productId)
        {
            Session["ProductId"] = productId;
            var bidderInfoList = db.BidingInfoes.Where(x => x.ProductId == productId).OrderByDescending(x => x.BiddingMoney).Select(x => x.UserInfo).ToList();

            ViewBag.BidderInfoList = bidderInfoList;
            return View(bidderInfoList);
        }

        public ActionResult SoldNow(int? bidderId, int? productId)
        {
            soldInfo.ProductId = productId;
            soldInfo.BuyerId = bidderId;
            db.SoldInfoes.Add(soldInfo);
            db.SaveChanges();
            return View();
        }
    }


}