using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BidMVC.Models;

namespace BidMVC.Controllers
{
    public class ProductController : Controller
    {
        Bid_MVC_DBEntities db = new Bid_MVC_DBEntities();
        BidingInfo bidingInfo = new BidingInfo();
        //
        // GET: /Product/
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Product product, HttpPostedFileBase Image1, HttpPostedFileBase Image2, HttpPostedFileBase Image3)
        {
            string path1 = Path.Combine(Server.MapPath("/Images"), Path.GetFileName(Image1.FileName));
            Image1.SaveAs(path1);
            string path2 = Path.Combine(Server.MapPath("/Images"), Path.GetFileName(Image2.FileName));
            Image2.SaveAs(path2);
            string path3 = Path.Combine(Server.MapPath("/Images"), Path.GetFileName(Image3.FileName));
            Image3.SaveAs(path3);
            string img1 = "~/Images/" + Path.GetFileName(Image1.FileName);
            string img2 = "~/Images/" + Path.GetFileName(Image2.FileName);
            string img3 = "~/Images/" + Path.GetFileName(Image3.FileName);
            string ext = Path.GetExtension(Image1.FileName);
            product.Image1 = img1;
            product.Image2 = img2;
            product.Image3 = img3;
            var email = Session["Email"].ToString();
            product.SellerId = db.UserInfoes.Where(x => x.Email == email).Select(x => x.Id).ToList().LastOrDefault();
            db.Products.Add(product);
            db.SaveChanges();

            return View();
        }
        [HttpGet]
        public ActionResult ListViewAllProducts()
        {
            var productList = db.Products.Select(x => x).ToList();
            // ViewBag.ProductList = productList;
            return View(productList);
        }

        [HttpGet]
        public ActionResult GridViewAllProducts()
        {
            var productList = db.Products.Select(x => x).ToList();
            return View(productList);
        }

        [HttpGet]
        public ActionResult ProductSelector(string productCode)
        {
            var product = db.Products.Where(x => x.ProductCode == productCode).Select(x => x).ToList().LastOrDefault();
            ViewBag.Products = product;
            return View(product);
        }

        [HttpGet]
        public ActionResult SearchProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchProduct(string productName)
        {
            var product = db.Products.Where(x => x.ProductName == productName).Select(x => x).ToList().LastOrDefault();
            ViewBag.Products = product;
            return View();
        }

        //        [HttpGet]
        public void BidProduct(string productCode, string biddingMoney)
        {
            if (!string.IsNullOrEmpty(Session["Email"] as string))  //have value
            {
                var email = Session["Email"].ToString();
                var bidderId = db.UserInfoes.Where(x => x.Email == email).Select(x => x.Id).ToList().LastOrDefault();
                var productId =
                    db.Products.Where(x => x.ProductCode == productCode).Select(x => x.Id).ToList().LastOrDefault();
                try
                {

                    bidingInfo.BidderId = bidderId;
                    bidingInfo.ProductId = productId;
                    bidingInfo.BidDateTime = DateTime.Now;
                    bidingInfo.BiddingMoney = Convert.ToDouble(biddingMoney);

                    db.BidingInfoes.Add(bidingInfo);
                    db.SaveChanges();

                    string message = "Bidded Successfully!!!";
                    string url = "/Product/GridViewAllProducts";
                    string script = "window.onload = function(){ alert('";
                    script += message;
                    script += "');";
                    script += "window.location = '";
                    script += url;
                    script += "'; }";
                    //ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
                    Response.Write("<script>" + script + "</script>");
                }
                catch (Exception)
                {
                    string message = "Enter valid amount!!!";
                    string url = "/Product/GridViewAllProducts";
                    string script = "window.onload = function(){ alert('";
                    script += message;
                    script += "');";
                    script += "window.location = '";
                    script += url;
                    script += "'; }";
                    //ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
                    Response.Write("<script>" + script + "</script>");
                }
            }
            else
            {
                string message = "You have to login!!!";
                string url = "/Product/GridViewAllProducts";
                string script = "window.onload = function(){ alert('";
                script += message;
                script += "');";
                script += "window.location = '";
                script += url;
                script += "'; }";
                //ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
                Response.Write("<script>" + script + "</script>");
            }


        }
    }
}