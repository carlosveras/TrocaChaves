using System.Web.Mvc;
using System;
using System.IO;
using System.Web.Optimization;
using System.Web.Routing;

namespace TrocaChaves
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BundleTable.EnableOptimizations = true;
        }

        protected void Application_EndRequest()
        {
            var strError = Context.AllErrors;

            using (StreamWriter w = new StreamWriter(Server.MapPath("~/Logs/log.txt"), true))
            {
                w.Write("\r\nLog Entry : ");
                if (strError != null)
                {
                    w.WriteLine(strError.ToString());
                    w.WriteLine(new String('-', 40));
                }

            }

        }
    }
}
