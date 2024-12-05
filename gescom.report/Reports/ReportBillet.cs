using gescom.data.Models;

namespace gescom.report.Reports
{
    public partial class ReportBillet : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportBillet()
        {
            InitializeComponent();
        }

        public ReportBillet(CashModel model)
        {
            InitializeComponent();
            lineNum.Text = model.Nom;
            NUMERO.Text = StdCalcul.DoubleToSpaceFormat(model.Id);
            VALEUR.Text = StdCalcul.DoubleToSpaceFormat(model.Montant);
        }
    }
}