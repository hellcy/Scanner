using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Scanner.Models;
using System.Web.Http.WebHost;
using System.Web.SessionState;

namespace Scanner
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //  GlobalConfiguration.Configure((WebApiConfig.RegisterRoutes);
            RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            Global.ConnStr = ConfigurationManager.ConnectionStrings["GramLineConn"].ToString();

        }



        public static void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            route.RouteHandler = new MyHttpControllerRouteHandler();
        }







        //public override void Init()
        //{
        //    var asa = "asas";
        //}
        //public override void Dispose()
        //{
        //    var asa = "asas";
        //}
        //public void Application_OnEnd()
        //{
        //    var asa = "asas";
        //}
        //public void Application_Error()
        //{
        //    var asa = "asas";
        //}

        // public void Application_EndRequest(
        //   object sender,
        //   EventArgs e
        //)
        //{
        //    var asa = "asas";
        //}

        //•Application_BeginRequest
        //•Application_AuthenticateRequest
        //•Application_AuthorizeRequest
        //•Application_ResolveRequestCache
        //•Application_AcquireRequestState
        //•Application_PreRequestHandlerExecute
        //•Application_PreSendRequestHeaders
        //•Application_PreSendRequestContent
        //•<<code is executed>>
        //•Application_PostRequestHandlerExecute
        //•Application_ReleaseRequestState
        //•Application_UpdateRequestCache
        //•Application_EndRequest
        //protected void Application_Error(Object sender, EventArgs e)
        //{
        //    var tt = "ss";
        //}


        //protected void Application_Disposed(object sender, EventArgs e)
        //{

        //    var tt = "ss";
        //}



        protected void Session_End(object sender, EventArgs e)
        {
            //try
            //{
            //    string cookieName = FormsAuthentication.FormsCookieName; //Find cookie name
            //    HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName]; //Get the cookie by it's name
            //    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decrypt it
            //    string UserName = ticket.Name; //You have the UserName!


            //    using (var context = new DbContext(Global.ConnStr))
            //    {                 
            //        var sql = "exec dbo.proc_Logout @UserName";
            //        context.Database.ExecuteSqlCommand(sql, UserName);
            //    }
            //}
            //catch (Exception ex) {
            //}

            //FormsAuthentication.SignOut();
        }

    }

    public class MyHttpControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData) : base(routeData)
        {
        }
    }
    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    }



}
