using DevExpress.XtraEditors;
using gescom.data.Models;
using System.Data;
using System.IO;

namespace gescom.backup
{
    public partial class FrmBackup : XtraForm
    {
        public FrmBackup()
        {
            InitializeComponent();
            DataHelpers.SaveGeneral();
            DataHelpers.SaveLocal();
           // SetFile();
        }
/*
        protected void SetFile()
        {
            var ftpClient = new FtpClient { Host = "ASPNET", UserName = "ftpuser", Password = "Pa$$w0rd", Port = 21 };            
            //  localDirectory= "" => local file is application directory
            string localDirectory = Directory.GetCurrentDirectory();
            const string remoteDirectory = "";
            const string remoteFilename = "Data.xml";
            const string localFilenameDownload = "Data.xml";
            ftpClient.Download(localDirectory, localFilenameDownload, remoteDirectory, remoteFilename);
            var file = new FileInfo("Data.xml");
            if (file.Exists) { Text += file.LastWriteTime.ToString("F"); }
        }
        */

        protected void SetData()
        {
            var oDs = new DataSet();
            oDs.ReadXml("Tout.xml");
            grille.DataSource = oDs.Tables[0];
        }

        private void FrmBackup_Activated(object sender, System.EventArgs e)
        {
            var file = new FileInfo("Tout.xml");
            if (file.Exists)
            {
                Text += file.LastWriteTime.ToString("F");
                SetData();
                return;
            }
            Text = @"IMPOSSIBLE DE LIRE LE FICHIER";
        }
    }
}