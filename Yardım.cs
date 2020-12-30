using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MalkocBilisim.DB;
using MalkocBilisim.Classes;

namespace MalkocBilisim
{
    public partial class Yardım : Form
    {
        MKocdbEntityDataContext dc = new MKocdbEntityDataContext();
        public Yardım()
        {
            InitializeComponent();
        }
       

        public Version AssemblyVersion
        {
            get
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }
        
        private void Yardım_Load(object sender, EventArgs e)
        {
            var guncelle = (from get in dc.Guncellemes orderby get.GuncellemeID descending select get).First();
            var resultDetay = from get in dc.GuncellemeDetayTablos orderby get.GuncellemeDetayID descending where get.GuncellemeID == guncelle.GuncellemeID select get;
            
            foreach (var item in resultDetay)
            {
                lstGuncelleme.Items.Add(item.GuncellemeBilgisi);
                lbTarihGuncelleme.Text = Convert.ToDateTime(item.Tarih).ToShortDateString();
            }
            lbVersion.Text = ApplicationDeployment.IsNetworkDeployed == true ? AssemblyVersion.ToString(4) : "v";
        }
    }
}
