using TrocaChaves.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Web.Routing;

namespace TrocaChaves.Controllers
{
    public class ConfiguracaoController : Controller
    {
        Configuracoes model = new Configuracoes();

        public ActionResult Index(string sistema)
        {
            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            List<Configuracoes> lstProject = new List<Configuracoes>();

            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/Xml/Configuracoes.xml"));

            if (ds.Tables.Count > 0)
            {

                DataView dvPrograms;
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "ServidorId";

                foreach (DataRowView dr in dvPrograms)
                {
                    Configuracoes model = new Configuracoes
                    {
                        ServidorId = Convert.ToString(dr[0]),

                        ServidorSystemId = Convert.ToString(dr[1]),

                        ServerPath = Convert.ToString(dr[2]),

                        BillingPath = Convert.ToString(dr[3]),

                        InteractionPath = Convert.ToString(dr[8]),

                        OrderPath = Convert.ToString(dr[13]),

                        SimpleProposalPath = Convert.ToString(dr[18]),

                        StatusCaptcha = Convert.ToInt32(dr[23])

                    };

                    if (model.ServidorSystemId.ToUpper() == "ICARE")
                        lstProject.Add(model);
                }
                if (lstProject.Count > 0)
                {
                    return View(lstProject);
                }
            }

            return View();

        }

        #region SpeedWeb
        public ActionResult IndexSpw(string sistema)
        {
            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            List<Configuracoes> lstProject = new List<Configuracoes>();

            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/Xml/Configuracoes.xml"));

            if (ds.Tables.Count > 0)
            {
                DataView dvPrograms;
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "ServidorId";

                foreach (DataRowView dr in dvPrograms)
                {
                    Configuracoes model = new Configuracoes
                    {
                        ServidorId = Convert.ToString(dr[0]),

                        ServidorSystemId = Convert.ToString(dr[1]),

                        ServerPath = Convert.ToString(dr[2]),

                        BillingPath = Convert.ToString(dr[3]),

                        InteractionPath = Convert.ToString(dr[8]),

                        OrderPath = Convert.ToString(dr[13]),

                        SimpleProposalPath = Convert.ToString(dr[18]),

                        StatusCaptcha = Convert.ToInt32(dr[23])

                    };

                    if (model.ServidorSystemId.ToUpper() == "SPW")
                        lstProject.Add(model);
                }
                if (lstProject.Count > 0)
                {
                    return View(lstProject);
                }
            }

            return View();

        }
        #endregion

        public ActionResult Adicionar(string sistema)
        {
            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Adicionar(Configuracoes mdl, string sistema)
        {
            mdl.BillingPath = @"\ICareCustomerBillingUI";
            mdl.OrderPath = @"\ICareOrderManagementUI";
            mdl.InteractionPath = @"\ICareCustomerInteractionUI";
            mdl.SimpleProposalPath = @"\SimpleProposal";

            if (mdl.ServidorId != null)
            {

                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

                var items = (from item in xmlDoc.Descendants("Servers")
                             select item).ToList();

                XElement selected = items.Elements("Server").Where(i => (string)i.Element("ServidorId") == mdl.ServidorId).FirstOrDefault();


                if (selected != null)
                    selected.Remove();

                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));
                xmlDoc.Element("Servers").Add(new XElement("Server",
                                                                new XElement("ServidorId", mdl.ServidorId),
                                                                new XElement("ServidorSystemId", sistema),
                                                                new XElement("ServerPath", mdl.ServerPath),

                                                                new XElement("BillingPath", mdl.BillingPath),
                                                                new XElement("BillingPciExecuteOn", mdl.BillingPciExecuteOn),
                                                                new XElement("BillingPciExecuteOff", mdl.BillingPciExecuteOff),
                                                                new XElement("BillingPciSecurityOn", mdl.BillingPciSecurityOn),
                                                                new XElement("BillingPciSecurityOff", mdl.BillingPciSecurityOff),

                                                                new XElement("InteractionPath", mdl.InteractionPath),
                                                                new XElement("InteractionPciExecuteOn", mdl.BillingPciExecuteOn),
                                                                new XElement("InteractionPciExecuteOff", mdl.BillingPciExecuteOff),
                                                                new XElement("InteractionPciSecurityOn", mdl.BillingPciSecurityOn),
                                                                new XElement("InteractionPciSecurityOff", mdl.BillingPciSecurityOff),

                                                                new XElement("OrderPath", mdl.OrderPath),
                                                                new XElement("OrderPciExecuteOn", mdl.BillingPciExecuteOn),
                                                                new XElement("OrderPciExecuteOff", mdl.BillingPciExecuteOff),
                                                                new XElement("OrderPciSecurityOn", mdl.BillingPciSecurityOn),
                                                                new XElement("OrderPciSecurityOff", mdl.BillingPciSecurityOff),

                                                                new XElement("SimpleProposalPath", mdl.SimpleProposalPath),
                                                                new XElement("SimpleProposalPciExecuteOn", mdl.BillingPciExecuteOn),
                                                                new XElement("SimpleProposalPciExecuteOff", mdl.BillingPciExecuteOff),
                                                                new XElement("SimpleProposalPciJSPathOn", mdl.BillingPciSecurityOn),
                                                                new XElement("SimpleProposalPciJSPathOff", mdl.BillingPciSecurityOff),

                                                                new XElement("StatusCaptcha", 0)
                                                           )
                                            );
                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));

                if (sistema.ToUpper() == "ICARE")
                    return RedirectToAction("Index", "Configuracao");
                else
                    return RedirectToAction("IndexSpw", "Configuracao");
            }
            else
            {
                var result = new HttpNotFoundResult();
                return result;
            }
        }

        public ActionResult Alterar(string ServidorId, string Sistema)
        {
            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            if (!string.IsNullOrEmpty(ServidorId))
            {
                model.BillingPath = @"\ICareCustomerBillingUI";
                model.OrderPath = @"\ICareOrderManagementUI";
                model.InteractionPath = @"\ICareCustomerInteractionUI";
                model.SimpleProposalPath = @"\SimpleProposal";

                var servidorId = ServidorId;

                GetDetailsById(servidorId);
                model.IsEdit = true;

                ViewBag.Sistema = Sistema;
                return View(model);
            }
            else
            {
                var result = new HttpNotFoundResult();
                return result;
            }
        }

        [HttpPost]
        public ActionResult Alterar(Configuracoes xmlConfig, string sistema)
        {

            if (xmlConfig.ServidorId != null)
            {

                xmlConfig.BillingPath = @"\ICareCustomerBillingUI";
                xmlConfig.OrderPath = @"\ICareOrderManagementUI";
                xmlConfig.InteractionPath = @"\ICareCustomerInteractionUI";
                xmlConfig.SimpleProposalPath = @"\SimpleProposal";

                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

                var items = (from item in xmlDoc.Descendants("Servers")
                             select item).ToList();

                XElement selected = items.Elements("Server").Where(i => (string)i.Element("ServidorId") == xmlConfig.ServidorId).FirstOrDefault();

                if (selected != null)
                    selected.Remove();

                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));
                xmlDoc.Element("Servers").Add(new XElement("Server",
                                                                new XElement("ServidorId", xmlConfig.ServidorId),
                                                                new XElement("ServidorSystemId", sistema),
                                                                new XElement("ServerPath", xmlConfig.ServerPath),

                                                                new XElement("BillingPath", xmlConfig.BillingPath),
                                                                new XElement("BillingPciExecuteOn", xmlConfig.BillingPciExecuteOn),
                                                                new XElement("BillingPciExecuteOff", xmlConfig.BillingPciExecuteOff),
                                                                new XElement("BillingPciSecurityOn", xmlConfig.BillingPciSecurityOn),
                                                                new XElement("BillingPciSecurityOff", xmlConfig.BillingPciSecurityOff),

                                                                new XElement("InteractionPath", xmlConfig.InteractionPath),
                                                                new XElement("InteractionPciExecuteOn", xmlConfig.BillingPciExecuteOn),
                                                                new XElement("InteractionPciExecuteOff", xmlConfig.BillingPciExecuteOff),
                                                                new XElement("InteractionPciSecurityOn", xmlConfig.BillingPciSecurityOn),
                                                                new XElement("InteractionPciSecurityOff", xmlConfig.BillingPciSecurityOff),

                                                                new XElement("OrderPath", xmlConfig.OrderPath),
                                                                new XElement("OrderPciExecuteOn", xmlConfig.BillingPciExecuteOn),
                                                                new XElement("OrderPciExecuteOff", xmlConfig.BillingPciExecuteOff),
                                                                new XElement("OrderPciSecurityOn", xmlConfig.BillingPciSecurityOn),
                                                                new XElement("OrderPciSecurityOff", xmlConfig.BillingPciSecurityOff),

                                                                new XElement("SimpleProposalPath", xmlConfig.SimpleProposalPath),
                                                                new XElement("SimpleProposalPciExecuteOn", xmlConfig.BillingPciExecuteOn),
                                                                new XElement("SimpleProposalPciExecuteOff", xmlConfig.BillingPciExecuteOff),
                                                                new XElement("SimpleProposalPciJSPathOn", xmlConfig.BillingPciSecurityOn),
                                                                new XElement("SimpleProposalPciJSPathOff", xmlConfig.BillingPciSecurityOff),

                                                                new XElement("StatusCaptcha", 0)
                                                           )
                                            );
                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));

                if (sistema.ToUpper() == "ICARE")
                    return RedirectToAction("Index", "Configuracao");
                else
                    return RedirectToAction("IndexSpw", "Configuracao");
            }
            else
            {
                var result = new HttpNotFoundResult();
                return result;
            }


        }

        public ActionResult Excluir(string ServidorId, string Sistema)
        {
            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            if (!string.IsNullOrEmpty(ServidorId))
            {
                var servidorId = ServidorId;

                GetDetailsById(servidorId);
                model.IsEdit = true;
                ViewBag.Sistema = Sistema;
                return View(model);
            }
            else
            {
                var result = new HttpNotFoundResult();
                return result;
            }
        }

        [HttpPost]
        public ActionResult Excluir(Configuracoes xmlConfig, string sistema)
        {
            if (xmlConfig.ServidorId != null)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

                var items = (from item in xmlDoc.Descendants("Servers")
                             select item).ToList();

                XElement selected = items.Elements("Server").Where(i => (string)i.Element("ServidorId") == xmlConfig.ServidorId).FirstOrDefault();

                if (selected != null)
                    selected.Remove();

                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));


                if (sistema.ToUpper() == "ICARE")
                    return RedirectToAction("Index", "Configuracao");
                else
                    return RedirectToAction("IndexSpw", "Configuracao");

            }
            else
            {
                var result = new HttpNotFoundResult();
                return result;
            }
        }

        public ActionResult Detalhes(string ServidorId, string Sistema)
        {
            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            if (!string.IsNullOrEmpty(ServidorId))
            {
                var servidorId = ServidorId;

                GetDetailsById(servidorId);
                model.IsEdit = true;

                ViewBag.Sistema = Sistema;

                return View(model);
            }
            else
            {
                var result = new HttpNotFoundResult();
                return result;
            }
        }

        public void GetDetailsById(string ServidorId)
        {
            XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

            var items = (from item in xmlDoc.Descendants("Servers")
                         select item).ToList();

            XElement selected = items.Elements("Server").Where(i => (string)i.Element("ServidorId") == ServidorId).FirstOrDefault();

            if (selected != null)
            {
                model.ServidorId = selected.Element("ServidorId").Value;
                model.ServerPath = selected.Element("ServerPath").Value;
                model.ServidorSystemId = selected.Element("ServidorSystemId").Value;

                model.BillingPath = selected.Element("BillingPath").Value;

                model.BillingPciExecuteOn = selected.Element("BillingPciExecuteOn").Value;
                model.BillingPciExecuteOff = selected.Element("BillingPciExecuteOff").Value;
                model.BillingPciSecurityOn = selected.Element("BillingPciSecurityOn").Value;
                model.BillingPciSecurityOff = selected.Element("BillingPciSecurityOff").Value;

                model.InteractionPath = selected.Element("InteractionPath").Value;

                model.InteractionPciExecuteOn = selected.Element("InteractionPciExecuteOn").Value;
                model.InteractionPciExecuteOff = selected.Element("InteractionPciExecuteOff").Value;
                model.InteractionPciSecurityOn = selected.Element("InteractionPciSecurityOn").Value;
                model.InteractionPciSecurityOff = selected.Element("InteractionPciSecurityOff").Value;

                model.OrderPath = selected.Element("OrderPath").Value;

                model.OrderPciExecuteOn = selected.Element("OrderPciExecuteOn").Value;
                model.OrderPciExecuteOff = selected.Element("OrderPciExecuteOff").Value;
                model.OrderPciSecurityOn = selected.Element("OrderPciSecurityOn").Value;
                model.OrderPciSecurityOff = selected.Element("OrderPciSecurityOff").Value;

                model.SimpleProposaPcilExecuteOn = selected.Element("SimpleProposalPciExecuteOn").Value;
                model.SimpleProposalPciExecuteOff = selected.Element("SimpleProposalPciExecuteOff").Value;
                model.SimpleProposalPciJSPathOn = selected.Element("SimpleProposalPciJSPathOn").Value;
                model.SimpleProposalPciJSPathOff = selected.Element("SimpleProposalPciJSPathOff").Value;

            };
        }
    }
}