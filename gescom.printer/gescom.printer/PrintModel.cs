using DevExpress.XtraReports.UI;
using gescom.data.Models;
using gescom.printer.Drafts;
using gescom.printer.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gescom.printer
{
    public static class PrintHelpers
    {
        public static void PrintNewTicket(OperationElem item)
        {
            DiaryHelpers.UpdateInitialPrint(item.Id);
            DateHelpers.UpdateBon(item.Id);
            var rpt = new TicketDirect(item);
            rpt.somme.Text = item.Montant2;
            rpt.count.Text = item.Count;
            var ptr = new Ticket(item);
            ptr.count.Text = item.Count;         
            rpt.Print("\\\\WIN-HOST.photo104.mg\\XP-80C");
            ptr.Print("\\\\THINK.photo104.mg\\80 Printer(1)");
        }

        public static void PrintTiroir()
        {
            var t = new TiroirModel();
            var rpt = new Recu(t);
            //rpt.Print("Xprinter XP-360B (Copy 3)");
        }

        public static void PrintBon(long id)
        {
            var report = new XtraBonCommande(id);
            report.Print();
            ReparationHelpers.ClearBonTable(id);
            DiaryHelpers.UpdateInitialPrint(id);
            DateHelpers.UpdateBon(id);
        }

        public static void PrintReceipt(CasherIn c)
        {
            ApiModel.SetCasherList(c);
            var model = new PrintModel(c);
            model.ImprimerRecu();
            CashHelpers.UpdatePay(c.Id, c.Obtenu);
            ReparationHelpers.AddFastPrint(c.Id);
            ReparationHelpers.ClearCashTable(c.Id);
        }

        public static void ImprimerCommande(long id)
        {
            var report = new XtraCommandes(id);
            report.ShowPreview();
        }

        public static void ImprimerPrime(List<PersonnelItem> liste, DateTime debut, DateTime fin)
        {
            var report = new ReportBonus(liste, debut, fin);
            report.ShowPreview();
        }
    }

    public class PrintModel
    {
        private List<PrintHelp> _recuList;
        public CasherIn _c;
        public TicketDirect Ticket { get; set; }
        public TicketDirect Bon { get; set; }

        public PrintModel()
        {
            _recuList = new List<PrintHelp>();
            SetList();
        }

        public PrintModel(CasherIn c)
        {
            _c = new CasherIn();
            _c = c;
            _c.Set();
        }

        public PrintModel(OperationElem item)
        {
            Bon = new TicketDirect(item);
            Bon.somme.Text = item.Montant2;
            Bon.count.Text = item.Count;
            Bon.nomPro.Text = "BON DE COMMANDE";
            Ticket = new TicketDirect(item);
            Ticket.somme.Text = item.Montant2;
            Ticket.count.Text = item.Count;
            Ticket.nomPro.Text = "TICKET DE LIVRAISON";
            Ticket.message.Text = "Cherchez les marchandises.";
        }

        public void ImprimerRecu()
        {
            var rpt = new Recu(_c);
            //rpt.Print("Xprinter XP-360B (Copy 3)");
            rpt.Print("Epson TM-T70 Receipt");
            DiaryHelpers.UpdateErrorPrint(_c.Id);
            DateHelpers.Delete(_c.Id);
            ReparationHelpers.RemoveFastPrint(_c.Id);
            ReparationHelpers.ClearCashTable(_c.Id);
        }

        protected void ImprimerAutres(long id, PrintItem item)
        {
            if ((item.Groupe == 6) || (item.Groupe >= 10) || (item.Groupe == 2))
            {
                return;
            }
            var model = CashHelpers.GetModel(id);
            var rpt = new XtraDocument(model);
            rpt.Print();
            if (item.Groupe == 9)
            {
                CashHelpers.UpdateAmountInvoice(id, 0);
                return;
            }
            DateHelpers.Delete(id);
        }

        private void ImprimerTicket()
        {
            string name = ApiModel.GetPrinterName(1);
            Ticket.Print(name);
        }

        private void ImprimerBon()
        {
            string name = ApiModel.GetPrinterName(1);
            Bon.Print(name);
        }

        private void SetList()
        {
            _recuList = ReparationHelpers.GetFastPrintHelps().ToList();
        }
    }
}