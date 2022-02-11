using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TrocaChaves.Models
{
    public class Configuracoes
    {
        [Column("Id do Servidor")]
        [DisplayName("Nome Servidor")]
        public string ServidorId { get; set; }

        [Column("Id do Servidor")]
        [DisplayName("Nome Servidor")]
        public string ServidorSystemId { get; set; }

        [DisplayName("Caminho Servidor")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string ServerPath { get; set; }

        #region Billing
        [DisplayName("Caminho Billing")]
        public string BillingPath { get; set; }

        public string BillingPciExecuteAtual { get; set; }

        public string BillingPciSecurityAtual { get; set; }

        [DisplayName("SkytefHTML")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string BillingPciExecuteOn { get; set; }

        [DisplayName("SkytefHTML")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string BillingPciExecuteOff { get; set; }

        [DisplayName("SkytefJS")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string BillingPciSecurityOn { get; set; }

        [DisplayName("SkytefJS")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string BillingPciSecurityOff { get; set; }

        #endregion

        #region Interaction
        [DisplayName("Caminho Interaction")]
        public string InteractionPath { get; set; }

        public string InteractionPciExecuteAtual { get; set; }

        public string InteractionPciSecurityAtual { get; set; }

        public string InteractionPciExecuteOn { get; set; }

        public string InteractionPciExecuteOff { get; set; }

        public string InteractionPciSecurityOn { get; set; }

        public string InteractionPciSecurityOff { get; set; }

        #endregion

        #region Order
        [DisplayName("Caminho Order")]
        public string OrderPath { get; set; }

        public string OrderPciExecuteAtual { get; set; }

        public string OrderPciSecurityAtual { get; set; }

        public string OrderPciExecuteOn { get; set; }

        public string OrderPciExecuteOff { get; set; }

        public string OrderPciSecurityOn { get; set; }

        public string OrderPciSecurityOff { get; set; }

        #endregion

        #region SimpleProposal
        [DisplayName("Caminho SimpleProposal")]
        public string SimpleProposalPath { get; set; }

        public string SimpleProposalPciExecuteAtual { get; set; }

        public string SimpleProposalPciJSPathAtual { get; set; }

        [DisplayName("SkytefHTML")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string SimpleProposaPcilExecuteOn { get; set; }

        [DisplayName("SkytefHTML")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string SimpleProposalPciExecuteOff { get; set; }

        [DisplayName("SkytefJS")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string SimpleProposalPciJSPathOn { get; set; }

        [DisplayName("SkytefJS")]
        [Required(ErrorMessage = "Campo obrigatorio")]
        public string SimpleProposalPciJSPathOff { get; set; }

        #endregion

        [DisplayName("Status")]
        public int StatusCaptcha { get; set; }

        public bool IsEdit { get; set; }

        public string Luin { get; set; }

        public string IsAuth { get; set; }

        public string IName { get; set; }

        public string Uhadd { get; set; }

        public List<SelectListItem> ListServersIcare { get; set; }

        public List<SelectListItem> ListServersSPW { get; set; }

    }
}