using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace SmashZone
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            // Home
            routes.MapPageRoute("RouteHome", "Home", "~/Pages/User/Default.aspx");

            // Public pages
            routes.MapPageRoute("RouteAbout", "About", "~/Pages/User/About.aspx");
            routes.MapPageRoute("RouteContact", "Contact", "~/Pages/User/Contact.aspx");
            routes.MapPageRoute("RouteFaq", "FAQ", "~/Pages/User/Faq.aspx");

            // Auth / account
            routes.MapPageRoute("RouteLogin", "Login", "~/Pages/User/Login.aspx");
            routes.MapPageRoute("RouteSignUp", "SignUp", "~/Pages/User/signUp.aspx");
            routes.MapPageRoute("Route2FA", "2FA", "~/Pages/User/2fa.aspx");
            routes.MapPageRoute("RouteAccount", "Account", "~/Pages/User/accountDetails.aspx");

            // Shop / product listing
            routes.MapPageRoute("RouteBadmintonProducts", "Badminton", "~/Pages/User/badmintonProducts.aspx");
            routes.MapPageRoute("RouteTennisProducts", "Tennis", "~/Pages/User/tennisProducts.aspx");
            routes.MapPageRoute("RouteSquashProducts", "Squash", "~/Pages/User/squashProducts.aspx");

            // Product details (querystring version)
            // Example: /Product?id=12
            routes.MapPageRoute("RouteProductDetails", "Product", "~/Pages/User/productDetails.aspx");

            // Cart / checkout
            routes.MapPageRoute("RouteCart", "Cart", "~/Pages/User/Cart.aspx");
            routes.MapPageRoute("RouteCheckout", "Checkout", "~/Pages/User/checkout.aspx");
            routes.MapPageRoute("RouteCheckoutSuccess", "CheckoutSuccess", "~/Pages/User/checkoutSuccess.aspx");

            // Search
            // Example: /Search?q=racket
            routes.MapPageRoute("RouteSearch", "Search", "~/Pages/User/Search.aspx");

            // Transaction history
            routes.MapPageRoute("RouteTransactions", "Transactions", "~/Pages/User/transactionHistory.aspx");
        }


    }


}