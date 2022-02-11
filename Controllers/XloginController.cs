using TrocaChaves.Models;
using System.Web.Mvc;

namespace TrocaChaves.Controllers
{
    public class XloginController : Controller
    {
      
        public ActionResult Xlogar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Xlogar(XLogin xlogin)
        {

            if (xlogin.Login == "admin" && xlogin.Password == "123")
            {
                Session["Xlogado"] = xlogin.Login;
                return RedirectToAction("Index", "Home");
            }
            return View();

        }

    }
}