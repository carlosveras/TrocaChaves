using TrocaChaves.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System;
using System.IO;

namespace TrocaChaves.Controllers
{
    public class HomeController : Controller
    {
        Configuracoes model = new Configuracoes();

        public ActionResult Index()
        {

            if (Session["Xlogado"] == null)
            {
                return RedirectToAction("Xlogar", "XLogin");
            }

            var configuracoes = GetAllServers();
            //ViewBag.ServidorId = new SelectList(configuracoes.ListServers, "ServidorId", "BillingPath", model.ServidorId);

            return View(configuracoes);
        }

        private Configuracoes GetAllServers()
        {
            Configuracoes configuracoes = new Configuracoes()
            {
                ListServersIcare = new List<SelectListItem>(),
                ListServersSPW = new List<SelectListItem>()
            };

            XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

            //var itens = xmlDoc.Element("Servers").Elements("Server").ToList();
            //foreach (var item in itens)
            //{
            //    configuracoes.ListServers.Add(new SelectListItem()
            //    {
            //        Text = item.Element("ServidorId").Value,
            //        Value = item.Element("BillingPath").Value,
            //    });
            //}

            var srvIcare = xmlDoc.Descendants("Server").Where(i => (string)i.Element("ServidorSystemId") == "ICARE").ToList();

            foreach (var item in srvIcare)
            {
                configuracoes.ListServersIcare.Add(new SelectListItem()
                {
                    Text = item.Element("ServidorId").Value,
                    Value = item.Element("BillingPath").Value,
                });
            }

            var srvSpw = xmlDoc.Descendants("Server").Where(i => (string)i.Element("ServidorSystemId") == "SPW").ToList();

            foreach (var item in srvSpw)
            {
                configuracoes.ListServersSPW.Add(new SelectListItem()
                {
                    Text = item.Element("ServidorId").Value,
                    Value = item.Element("BillingPath").Value,
                });
            }

            return configuracoes;
        }

        public PartialViewResult ConfiguracoesAtuais()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult ConfiguracoesAtuais(string ServidorId)
        {
            if (!string.IsNullOrEmpty(ServidorId))
            {
                var servidorId = ServidorId;
                model = GetDetailsById(servidorId);
            }

            return PartialView("ConfiguracoesAtuais", model);
        }

        public Configuracoes GetDetailsById(string ServidorId)
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

                model.OrderPath = selected.Element("OrderPath").Value;

                model.OrderPciExecuteOn = selected.Element("OrderPciExecuteOn").Value;
                model.OrderPciExecuteOff = selected.Element("OrderPciExecuteOff").Value;
                model.OrderPciSecurityOn = selected.Element("OrderPciSecurityOn").Value;
                model.OrderPciSecurityOff = selected.Element("OrderPciSecurityOff").Value;

                model.InteractionPath = selected.Element("InteractionPath").Value;

                model.InteractionPciExecuteOn = selected.Element("InteractionPciExecuteOn").Value;
                model.InteractionPciExecuteOff = selected.Element("InteractionPciExecuteOff").Value;
                model.InteractionPciSecurityOn = selected.Element("InteractionPciSecurityOn").Value;
                model.InteractionPciSecurityOff = selected.Element("InteractionPciSecurityOff").Value;

                if (model.ServidorSystemId == "SPW")
                {
                    model.SimpleProposalPath = selected.Element("SimpleProposalPath").Value;

                    model.SimpleProposaPcilExecuteOn = selected.Element("SimpleProposalPciExecuteOn").Value;
                    model.SimpleProposalPciExecuteOff = selected.Element("SimpleProposalPciExecuteOff").Value;
                    model.SimpleProposalPciJSPathOn = selected.Element("SimpleProposalPciJSPathOn").Value;
                    model.SimpleProposalPciJSPathOff = selected.Element("SimpleProposalPciJSPathOff").Value;
                }
                GetDetailsByPath(model);

            };

            return model;
        }

        private void GetDetailsByPath(Configuracoes model)
        {
            var filePathB = model.ServerPath + model.BillingPath + @"\Web.config";
            var filePathO = model.ServerPath + model.OrderPath + @"\Web.config";
            var filePathI = model.ServerPath + model.InteractionPath + @"\Web.config";
            var filePathS = model.ServerPath + model.SimpleProposalPath + @"\Web.config";

            string pciExec = string.Empty;
            string pciSec = string.Empty;

            if (model.ServidorSystemId == "ICARE")
            {

                #region Billing
                try
                {
                    ReadConfigFile(filePathB, out pciExec, out pciSec);
                    model.BillingPciExecuteAtual = pciExec;
                    model.BillingPciSecurityAtual = pciSec;
                }
                catch (Exception)
                {
                    string msgError = string.Empty;
                    var billingDir = model.ServerPath + model.BillingPath;

                    try
                    {
                        Directory.SetCurrentDirectory(billingDir);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        msgError = "Diretorio inexistente --> " + billingDir;
                    }
                    catch (UnauthorizedAccessException Un)
                    {
                        msgError = "Acesso negado ao diretorio --> " + billingDir + "----" + Un.Message;
                    }
                    catch (Exception e)
                    {
                        msgError = billingDir + e.ToString();
                    }

                    if (string.IsNullOrEmpty(msgError))
                    {
                        try
                        {
                            FileStream fs = null;
                            string line = null;

                            fs = new FileStream(filePathB, FileMode.Open);
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                line = reader.ReadLine();
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            msgError = "Arquivo não encontrado --> " + filePathB;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            msgError = "Acesso negado ao arquivo --> " + filePathB;
                        }
                    }

                    GravarLog(msgError, model);
                }
                #endregion

                #region Order
                try
                {
                    ReadConfigFile(filePathO, out pciExec, out pciSec);
                    model.OrderPciExecuteAtual = pciExec;
                    model.OrderPciSecurityAtual = pciSec;
                }
                catch (Exception)
                {
                    string msgError = string.Empty;
                    var OrderDir = model.ServerPath + model.OrderPath;

                    try
                    {

                        Directory.SetCurrentDirectory(OrderDir);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        msgError = "Diretorio inexistente --> " + OrderDir;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        msgError = "Acesso negado ao diretorio --> " + OrderDir;
                    }
                    catch (Exception e)
                    {
                        msgError = OrderDir + e.ToString();
                    }

                    if (string.IsNullOrEmpty(msgError))
                    {
                        try
                        {
                            FileStream fs = null;
                            string line = null;

                            fs = new FileStream(filePathO, FileMode.Open);
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                line = reader.ReadLine();
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            msgError = "Arquivo não encontrado --> " + filePathO;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            msgError = "Acesso negado ao arquivo --> " + filePathO;
                        }
                    }

                    GravarLog(msgError, model);
                }
                #endregion

                #region Interaction
                try
                {
                    ReadConfigFile(filePathI, out pciExec, out pciSec);
                    model.InteractionPciExecuteAtual = pciExec;
                    model.InteractionPciSecurityAtual = pciSec;
                }
                catch (Exception)
                {
                    string msgError = string.Empty;
                    var InteractionDir = model.ServerPath + model.InteractionPath;

                    try
                    {

                        Directory.SetCurrentDirectory(InteractionDir);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        msgError = "Diretorio inexistente --> " + InteractionDir;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        msgError = "Acesso negado ao diretorio --> " + InteractionDir;
                    }
                    catch (Exception e)
                    {
                        msgError = InteractionDir + e.ToString();
                    }

                    if (string.IsNullOrEmpty(msgError))
                    {
                        try
                        {
                            FileStream fs = null;
                            string line = null;

                            fs = new FileStream(filePathI, FileMode.Open);
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                line = reader.ReadLine();
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            msgError = "Arquivo não encontrado --> " + filePathI;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            msgError = "Acesso negado ao arquivo --> " + filePathI;
                        }
                    }

                    GravarLog(msgError, model);
                }
                #endregion

            }
            else
            {

                #region SimpleProposal
                try
                {
                    ReadConfigFile(filePathS, out pciExec, out pciSec);
                    model.SimpleProposalPciExecuteAtual = pciExec;
                    model.SimpleProposalPciJSPathAtual = pciSec;
                }
                catch (Exception)
                {
                    string msgError = string.Empty;
                    var SimpleProposalDir = model.ServerPath + model.SimpleProposalPath;

                    try
                    {

                        Directory.SetCurrentDirectory(SimpleProposalDir);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        msgError = "Diretorio inexistente --> " + SimpleProposalDir;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        msgError = "Acesso negado ao diretorio --> " + SimpleProposalDir;
                    }
                    catch (Exception e)
                    {
                        msgError = SimpleProposalDir + e.ToString();
                    }


                    if (string.IsNullOrEmpty(msgError))
                    {
                        try
                        {
                            FileStream fs = null;
                            string line = null;

                            fs = new FileStream(filePathS, FileMode.Open);
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                line = reader.ReadLine();
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            msgError = "Arquivo não encontrado --> " + filePathS;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            msgError = "Acesso negado ao arquivo --> " + filePathS;
                        }
                    }

                    GravarLog(msgError, model);
                }
                #endregion
            }

        }

        private void ReadConfigFile(String path, out string pciExecutePath, out string pciSecurityPath)
        {
            var _map = new ExeConfigurationFileMap { ExeConfigFilename = path };
            Configuration configI = ConfigurationManager.OpenMappedExeConfiguration(_map, ConfigurationUserLevel.None);

            pciExecutePath = configI.AppSettings.Settings["PciExecutePath"].Value;
            if (model.ServidorSystemId == "ICARE")
            {
                pciSecurityPath = configI.AppSettings.Settings["PciSecurityPath"].Value;
            }
            else
                pciSecurityPath = configI.AppSettings.Settings["PciJSPath"].Value;
        }


        //private void ReadConfigFileSpw(String path, out string pciExecutePath, out string pciSecurityPath)
        //{
        //    var _map = new ExeConfigurationFileMap { ExeConfigFilename = path };
        //    Configuration configI = ConfigurationManager.OpenMappedExeConfiguration(_map, ConfigurationUserLevel.None);

        //    pciExecutePath = configI.AppSettings.Settings["PciExecutePath"].Value;
        //    pciSecurityPath = configI.AppSettings.Settings["PciJSPath"].Value;
        //}

        [HttpGet]
        public PartialViewResult LigaCaptcha(string ServidorId)
        {
            if (!string.IsNullOrEmpty(ServidorId))
            {
                var servidorId = ServidorId;
                model = GetDetailsById(ServidorId);
                model = LigaChaves(model);
            }

            return PartialView("ConfiguracoesAtuais", model);
        }

        private Configuracoes LigaChaves(Configuracoes model)
        {
            string caminhoBilling = model.ServerPath + model.BillingPath + @"\Web.config";
            string caminhoInteraction = model.ServerPath + model.InteractionPath + @"\Web.config";
            string caminhoOrder = model.ServerPath + model.OrderPath + @"\Web.config";
            string caminhoProposal = model.ServerPath + model.SimpleProposalPath + @"\Web.config";

            if (model.ServidorSystemId == "ICARE")
            {
                if (caminhoBilling != null || !string.IsNullOrEmpty(caminhoBilling))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoBilling);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();

                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciSecurityPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.BillingPciExecuteOn);
                        keyPciSecurity.SetAttributeValue("value", model.BillingPciSecurityOn);

                        xmlDoc.Save(caminhoBilling);

                        model.BillingPciExecuteAtual = model.BillingPciExecuteOn;
                        model.BillingPciSecurityAtual = model.BillingPciSecurityOn;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        string msgError = "Acesso negado ao arquivo ao tentar ligar a chave--> " + caminhoBilling;

                        GravarLog(msgError, model);
                    }
                }

                if (caminhoInteraction != null || !string.IsNullOrEmpty(caminhoInteraction))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoInteraction);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();

                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciSecurityPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.InteractionPciExecuteOn);
                        keyPciSecurity.SetAttributeValue("value", model.InteractionPciSecurityOn);

                        xmlDoc.Save(caminhoInteraction);

                        model.InteractionPciExecuteAtual = model.InteractionPciExecuteOn;
                        model.InteractionPciSecurityAtual = model.InteractionPciSecurityOn;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        string msgError = "Acesso negado ao arquivo ao tentar ligar a chave--> " + caminhoInteraction;

                        GravarLog(msgError, model);
                    }
                }

                if (caminhoOrder != null || !string.IsNullOrEmpty(caminhoOrder))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoOrder);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();

                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciSecurityPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.OrderPciExecuteOn);
                        keyPciSecurity.SetAttributeValue("value", model.OrderPciSecurityOn);

                        xmlDoc.Save(caminhoOrder);

                        model.OrderPciExecuteAtual = model.OrderPciExecuteOn;
                        model.OrderPciSecurityAtual = model.OrderPciSecurityOn;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        string msgError = "Acesso negado ao arquivo ao tentar ligar a chave--> " + caminhoOrder;

                        GravarLog(msgError, model);
                    }
                }
            }
            else
            {
                if (caminhoProposal != null || !string.IsNullOrEmpty(caminhoProposal))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoProposal);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();

                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciJSPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.SimpleProposaPcilExecuteOn);
                        keyPciSecurity.SetAttributeValue("value", model.SimpleProposalPciJSPathOn);

                        xmlDoc.Save(caminhoProposal);

                        model.SimpleProposalPciExecuteAtual = model.SimpleProposaPcilExecuteOn;
                        model.SimpleProposalPciJSPathAtual = model.SimpleProposalPciJSPathOn;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        string msgError = "Acesso negado ao arquivo ao tentar ligar a chave--> " + caminhoOrder;

                        GravarLog(msgError, model);
                    }
                }
            }

            AlteraConfiguracoesOn(model);

            return model;
        }

        private void AlteraConfiguracoesOn(Configuracoes model)
        {
            if (model.ServidorId != null)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

                var items = (from item in xmlDoc.Descendants("Servers")
                             select item).ToList();

                XElement selected = items.Elements("Server").Where(i => (string)i.Element("ServidorId") == model.ServidorId).FirstOrDefault();

                if (selected != null)
                    selected.Remove();

                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));
                xmlDoc.Element("Servers").Add(new XElement("Server",
                                                                new XElement("ServidorId", model.ServidorId),
                                                                new XElement("ServidorSystemId", model.ServidorSystemId),
                                                                new XElement("ServerPath", model.ServerPath),

                                                                new XElement("BillingPath", model.BillingPath),
                                                                new XElement("BillingPciExecuteOn", model.BillingPciExecuteOn),
                                                                new XElement("BillingPciExecuteOff", model.BillingPciExecuteOff),
                                                                new XElement("BillingPciSecurityOn", model.BillingPciSecurityOn),
                                                                new XElement("BillingPciSecurityOff", model.BillingPciSecurityOff),

                                                                new XElement("InteractionPath", model.InteractionPath),
                                                                new XElement("InteractionPciExecuteOn", model.InteractionPciExecuteOn),
                                                                new XElement("InteractionPciExecuteOff", model.InteractionPciExecuteOff),
                                                                new XElement("InteractionPciSecurityOn", model.InteractionPciSecurityOn),
                                                                new XElement("InteractionPciSecurityOff", model.InteractionPciSecurityOff),

                                                                new XElement("OrderPath", model.OrderPath),
                                                                new XElement("OrderPciExecuteOn", model.OrderPciExecuteOn),
                                                                new XElement("OrderPciExecuteOff", model.OrderPciExecuteOff),
                                                                new XElement("OrderPciSecurityOn", model.OrderPciSecurityOn),
                                                                new XElement("OrderPciSecurityOff", model.OrderPciSecurityOff),

                                                                new XElement("SimpleProposalPath", model.SimpleProposalPath),
                                                                new XElement("SimpleProposalPciExecuteOn", model.BillingPciExecuteOn),
                                                                new XElement("SimpleProposalPciExecuteOff", model.BillingPciExecuteOff),
                                                                new XElement("SimpleProposalPciJSPathOn", model.BillingPciSecurityOn),
                                                                new XElement("SimpleProposalPciJSPathOff", model.BillingPciSecurityOff),

                                                                new XElement("StatusCaptcha", 1)
                                                           )
                                            );
                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));
            }
        }

        [HttpGet]
        public PartialViewResult DesligaCaptcha(string ServidorId)
        {
            if (!string.IsNullOrEmpty(ServidorId))
            {
                var servidorId = ServidorId;
                model = GetDetailsById(ServidorId);
                model = DesligaChaves(model);
            }

            return PartialView("ConfiguracoesAtuais", model);
        }

        private Configuracoes DesligaChaves(Configuracoes model)
        {
            string caminhoBilling = model.ServerPath + model.BillingPath + @"\Web.config";
            string caminhoInteraction = model.ServerPath + model.InteractionPath + @"\Web.config";
            string caminhoOrder = model.ServerPath + model.OrderPath + @"\Web.config";
            string caminhoSimpleProposal = model.ServerPath + model.SimpleProposalPath + @"\Web.config";

            string userName = HttpContext.User.Identity.Name;

            if (model.ServidorSystemId == "ICARE")
            {
                #region Billing
                if (caminhoBilling != null || !string.IsNullOrEmpty(caminhoBilling))
                {
                    try
                    {

                        XDocument xmlDoc = XDocument.Load(caminhoBilling);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();

                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciSecurityPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.BillingPciExecuteOff);
                        keyPciSecurity.SetAttributeValue("value", model.BillingPciSecurityOff);

                        xmlDoc.Save(caminhoBilling);

                        model.BillingPciExecuteAtual = model.BillingPciExecuteOff;
                        model.BillingPciSecurityAtual = model.BillingPciSecurityOff;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        GravarLog(string.Format(@"Acesso negado ao arquivo ao tentar desligar a chave--> {0}", caminhoBilling), model);
                    }
                }
                #endregion

                #region Interaction
                if (caminhoInteraction != null || !string.IsNullOrEmpty(caminhoInteraction))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoInteraction);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();
                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciSecurityPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.InteractionPciExecuteOff);
                        keyPciSecurity.SetAttributeValue("value", model.InteractionPciSecurityOff);

                        xmlDoc.Save(caminhoInteraction);

                        model.InteractionPciExecuteAtual = model.InteractionPciExecuteOff;
                        model.InteractionPciSecurityAtual = model.InteractionPciSecurityOff;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        GravarLog(string.Format(@"Acesso negado ao arquivo ao tentar desligar a chave--> {0}", caminhoInteraction), model);
                    }
                }
                #endregion

                #region Order
                if (caminhoOrder != null || !string.IsNullOrEmpty(caminhoOrder))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoOrder);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();
                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciSecurityPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.OrderPciExecuteOff);
                        keyPciSecurity.SetAttributeValue("value", model.OrderPciSecurityOff);

                        xmlDoc.Save(caminhoOrder);

                        model.OrderPciExecuteAtual = model.OrderPciExecuteOff;
                        model.OrderPciSecurityAtual = model.OrderPciSecurityOff;

                    }
                    catch (UnauthorizedAccessException)
                    {
                        GravarLog(string.Format(@"Acesso negado ao arquivo ao tentar desligar a chave--> {0}", caminhoOrder), model);
                    }
                }
                #endregion
            }
            else
            {
                #region SimpleProposal
                if (caminhoSimpleProposal != null || !string.IsNullOrEmpty(caminhoSimpleProposal))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(caminhoSimpleProposal);

                        XElement keyPciExecute = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciExecutePath").FirstOrDefault();

                        XElement keyPciSecurity = xmlDoc.Descendants("appSettings").Elements("add").Where(c => (string)c.Attribute("key").Value == "PciJSPath").FirstOrDefault();

                        keyPciExecute.SetAttributeValue("value", model.SimpleProposalPciExecuteOff);
                        keyPciSecurity.SetAttributeValue("value", model.SimpleProposalPciJSPathOff);

                        xmlDoc.Save(caminhoSimpleProposal);

                        model.SimpleProposalPciExecuteAtual = model.SimpleProposalPciExecuteOff;
                        model.SimpleProposalPciJSPathAtual = model.SimpleProposalPciJSPathOff;

                    }
                    catch (UnauthorizedAccessException)
                    {
                        GravarLog(string.Format(@"Acesso negado ao arquivo ao tentar desligar a chave--> {0}", caminhoSimpleProposal), model);
                    }
                }
                #endregion
            }

            AlteraConfiguracoesOff(model);
            return model;
        }

        private void GravarLog(string msgError, Configuracoes model)
        {
            var IName = model.IName;
            var IsAuth = model.IsAuth;
            var Luin = model.Luin;
            var Uhadd = model.Uhadd;

            var usrId = Environment.UserName;
            var principalId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            using (StreamWriter w = new StreamWriter(Server.MapPath("~/Logs/log.txt"), true))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), HttpContext.User.Identity.Name, usrId, principalId, IName, IsAuth, Luin, Uhadd, msgError.ToString());
                w.WriteLine(new String('-', 40));
            }
        }

        private void AlteraConfiguracoesOff(Configuracoes model)
        {
            if (model.ServidorId != null)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/Configuracoes.xml"));

                var items = (from item in xmlDoc.Descendants("Servers")
                             select item).ToList();

                XElement selected = items.Elements("Server").Where(i => (string)i.Element("ServidorId") == model.ServidorId).FirstOrDefault();

                if (selected != null)
                    selected.Remove();

                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));
                xmlDoc.Element("Servers").Add(new XElement("Server",
                                                                new XElement("ServidorId", model.ServidorId),
                                                                new XElement("ServidorSystemId", model.ServidorSystemId),
                                                                new XElement("ServerPath", model.ServerPath),
                                                                new XElement("BillingPath", model.BillingPath),
                                                                new XElement("BillingPciExecuteOn", model.BillingPciExecuteOn),
                                                                new XElement("BillingPciExecuteOff", model.BillingPciExecuteOff),
                                                                new XElement("BillingPciSecurityOn", model.BillingPciSecurityOn),
                                                                new XElement("BillingPciSecurityOff", model.BillingPciSecurityOff),

                                                                new XElement("InteractionPath", model.InteractionPath),
                                                                new XElement("InteractionPciExecuteOn", model.InteractionPciExecuteOn),
                                                                new XElement("InteractionPciExecuteOff", model.InteractionPciExecuteOff),
                                                                new XElement("InteractionPciSecurityOn", model.InteractionPciSecurityOn),
                                                                new XElement("InteractionPciSecurityOff", model.InteractionPciSecurityOff),

                                                                new XElement("OrderPath", model.OrderPath),
                                                                new XElement("OrderPciExecuteOn", model.OrderPciExecuteOn),
                                                                new XElement("OrderPciExecuteOff", model.OrderPciExecuteOff),
                                                                new XElement("OrderPciSecurityOn", model.OrderPciSecurityOn),
                                                                new XElement("OrderPciSecurityOff", model.OrderPciSecurityOff),

                                                                new XElement("SimpleProposalPath", model.SimpleProposalPath),
                                                                new XElement("SimpleProposalPciExecuteOn", model.BillingPciExecuteOn),
                                                                new XElement("SimpleProposalPciExecuteOff", model.BillingPciExecuteOff),
                                                                new XElement("SimpleProposalPciJSPathOn", model.BillingPciSecurityOn),
                                                                new XElement("SimpleProposalPciJSPathOff", model.BillingPciSecurityOff),

                                                                new XElement("StatusCaptcha", 0)
                                                           )
                                            );
                xmlDoc.Save(Server.MapPath("~/Xml/Configuracoes.xml"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (Session["Xlogado"] != null)
            {
                Session["Xlogado"] = null;
                return RedirectToAction("XLogar", "Xlogin");
            }

            return View();
        }

    }
}