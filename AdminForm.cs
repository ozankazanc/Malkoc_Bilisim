using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MalkocBilisim.DB;

namespace MalkocBilisim
{
    public partial class AdminForm : Form
    {
        MKocdbEntityDataContext dc = new MKocdbEntityDataContext();
        
        public AdminForm()
        {
            InitializeComponent();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {

      

        }

        private void btnGuncelleEkle_Click(object sender, EventArgs e)
        {
            string version = txVersion.Text;
            string bilgi = txBilgi.Text;
            int guncelleid;

            var result = from get in dc.Guncellemes where get.Version == version select get;

            if (result.Count() == 0)
            {
                Guncelleme guncelle = new Guncelleme
                {
                    Version = version,
                    Checked = false
                };
                dc.Guncellemes.InsertOnSubmit(guncelle);
            }
            else
            {
                Guncelleme guncelle = dc.Guncellemes.FirstOrDefault(x => x.Version == version);
                guncelle.Checked = false;
            }
            dc.SubmitChanges();
            
            guncelleid = (from get in dc.Guncellemes orderby get.GuncellemeID descending select get.GuncellemeID).First();

            GuncellemeDetayTablo detay = new GuncellemeDetayTablo
            {
                GuncellemeBilgisi = bilgi,
                GuncellemeID = guncelleid,
                Tarih = DateTime.Now
            };
            dc.GuncellemeDetayTablos.InsertOnSubmit(detay);
            dc.SubmitChanges();
        }
    }
}
