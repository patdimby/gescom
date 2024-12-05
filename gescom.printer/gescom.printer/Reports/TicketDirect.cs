using gescom.data.Models;
using System;

namespace gescom.printer.Reports
{
    public partial class TicketDirect : DevExpress.XtraReports.UI.XtraReport
    {
        public TicketDirect()
        {
            InitializeComponent();
        }

        public TicketDirect(OperationElem item)
        {
            InitializeComponent();
            Init(item);
            DataSource = item.Members;
            count.Text = item.Count;
            quantite.DataBindings.Add("Text", item.Members, "Quantite");
            codevente.DataBindings.Add("Text", item.Members, "LongCode");
            nom.DataBindings.Add("Text", item.Members, "Refce");
        }

        private void Init(OperationElem item)
        {
            num.Text = StdCalcul.DoubleToSpaceFormat(item.Id);
            vendeur.Text = Environment.MachineName;
            temps.Text = DateTime.Now.ToString();
            operateur.Text = item.Operateur.ToUpper();
        }
    }
}