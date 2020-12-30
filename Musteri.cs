using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MalkocBilisim.Classes;
using MalkocBilisim.DB;

namespace MalkocBilisim
{
    public partial class Musteri : DevExpress.XtraEditors.XtraUserControl
    {
        private static Musteri _instance;
       int MusteriIDGonder = 0;
        public static Musteri Instance
        {
            get
            {
                if (_instance == null)
                { _instance = new Musteri(); }

                return _instance;
            }
        }
        class ModelDetay
        {
            public int OdemeID { get; set; }
            public String Kategori { get; set; }
            public String OdemeDurumu { get; set; }
            public DateTime Tarih { get; set; }
        }
        public Musteri()
        {
            InitializeComponent();
            ShowRec(Models.MusteriTablos);
        }
        public void ShowRec(IQueryable<MusteriTablo> t)
        {
            gcMusteri.DataSource = t;
            ShowRecDetail(Convert.ToInt32(gvMusteri.GetFocusedRowCellValue(clMusteriID)));
        }
        public void ShowRecDetail(int MusteriID)
        {
            var result = from get in Models.OdemeTablos
                         where get.MusteriID == MusteriID
                         select new ModelDetay
                         {
                             Kategori = get.TeknikID != null ? "Teknik Servis" : (get.SatisTablo.SatisDurumu == false ? "Teklif" : "Satış"),
                             OdemeDurumu = get.SatisTablo.SatisDurumu == false ? "Teklif" : (get.OdemeDurumu == true ? "Ödendi" : "Ödenmedi"),
                             Tarih = get.TeknikID != null ? (DateTime)get.TeknikTablo.Tarih : (DateTime)get.SatisTablo.Tarih,
                             OdemeID = get.OdemeID
                         };
            gcBelgeDetay.DataSource = result;
        }
        public void BilgileriDoldur(int MusteriID)
        {
            MusteriTablo musteri = Models.MusteriTablos.First(x => x.MusteriID == MusteriID);
            txMusteriAdi_G.Text = musteri.MusteriIsim;
            txMusteriAdres_G.Text = musteri.MusteriAdres;
            txTelefon_G.Text = musteri.MusteriTelefon;

            txMusteriAdi_S.Text = musteri.MusteriIsim;
            txMusteriAdres_S.Text = musteri.MusteriAdres;
            txTelefon_S.Text = musteri.MusteriTelefon;

            lbMusteriAdi.Text = "Müşteriye Ait Bilgiler (" + musteri.MusteriIsim + ")";
            ShowRecDetail(MusteriID);
        }
        private void gvMusteri_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                MusteriIDGonder = Convert.ToInt32(gvMusteri.GetRowCellValue(e.FocusedRowHandle, clMusteriID));
                if (MusteriIDGonder != 0)
                    BilgileriDoldur(MusteriIDGonder);
            }
            catch (Exception)
            {
                MyControl.Mesaj("Sorun ile karşılaşıldı.", 1);
            }
        }

        int OdemeID = 0;
        private void btnOdemeYonlendir_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OdemeID = Convert.ToInt32(gvOBelgeDetay.GetFocusedRowCellValue(clOdemeID));

            Odemeler.Instance.ShowRec(Models.OdemeTablos.Where(x => x.OdemeID == OdemeID).Select(x => x));
            Odemeler.Instance.BilgileriDoldur(OdemeID);
            Odemeler.Instance.BringToFront();

        }
        private void btnBelgeYonlendir_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OdemeID = Convert.ToInt32(gvOBelgeDetay.GetFocusedRowCellValue(clOdemeID));
            if (Models.GetTeknikRecordsOdemeID(OdemeID).Count() != 0) // Boş dönerse Satış Formu
            {
                TeknikSFormListele.Instance.ShowRec(Models.GetTeknikRecordsOdemeID(OdemeID));
                TeknikSFormListele.Instance.BilgileriDoldur(Models.TeknikID);
                TeknikSFormListele.Instance.BringToFront();
            }
            else
            {
                SatisFormListele.Instance.ShowRec(Models.GetSatisRecordsOdemeID(OdemeID));
                SatisFormListele.Instance.BilgileriDoldur(Models.SatisID);
                SatisFormListele.Instance.BringToFront();
            }
            //Satış mı yoksa Teknik Servis Formu mu?
        }
        private void btnButunKayitlar_Click(object sender, EventArgs e)
        {
            ShowRec(Models.MusteriTablos);
        }
        private void btnArama_Click(object sender, EventArgs e)
        {
            string search = txArama.Text;
            if (String.IsNullOrWhiteSpace(search))
            {
                MyControl.Mesaj("Aranacak ifadeyi soldaki kutucuğa giriniz.", 1);
            }
            else
            {
                if (cbArama.SelectedIndex == 0)
                    ShowRec(Models.SearchbyCustName(search));
                else if (cbArama.SelectedIndex == 1)
                    ShowRec(Models.SearchbyCustPhone(search));
                else
                    ShowRec(Models.SearchbyCustAddress(search));
            }
            txArama.Text = "";
        }
        private void btnMusteriKaydet_Click(object sender, EventArgs e)
        {
            string custName = txMusteriAdi_E.Text, custAddress = txMusteriAdres_E.Text, custPhone = txTelefon_E.Text;

            if (String.IsNullOrWhiteSpace(custName))
            {
                MyControl.Mesaj("[Müşteri ve ya Firma Adı] alanı boş geçilemez.", 1);
            }
            else
            {
                if (Models.InsertCustomer(custName, custPhone, custAddress))
                {
                    MyControl.Mesaj("[" + custName + "] isimli müşteri sisteme kaydedilmiştir.", 0);
                    txMusteriAdi_E.Text = ""; txMusteriAdres_E.Text = ""; txTelefon_E.Text = "";
                    ShowRec(Models.MusteriTablos);
                }
                else
                {
                    MyControl.Mesaj("[" + custName + "] isimli ve " + "[" + custPhone + "] telefon numaralı müşteri sistemde zaten kayıtlıdır.", 1);
                }
            }
        }
        private void btnMusteriGuncelle_Click(object sender, EventArgs e)
        {
            string custName = txMusteriAdi_G.Text, custAddress = txMusteriAdres_G.Text, custPhone = txTelefon_G.Text;

            if (String.IsNullOrWhiteSpace(custName))
            {
                MyControl.Mesaj("[Müşteri ve ya Firma Adı] alanı boş geçilemez.", 1);
            }
            else
            {
                MyControl.Mesaj(Models.UpdateCustomer(custName, custPhone, custAddress, MusteriIDGonder), 0);
                ShowRec(Models.MusteriTablos);
            }

        }
        private void HideTab()
        {
            gcMusteri.Width = 916;
            btnGoster.Visible = true;
            btnGizle.Visible = false;
            TabMusteri.Visible = false;
            labelControl11.Visible = false;
        }
        private void btnGizle_Click(object sender, EventArgs e)
        {
            HideTab();
        }
        private void btnGoster_Click(object sender, EventArgs e)
        {
            gcMusteri.Width = 455;
            btnGoster.Visible = false;
            btnGizle.Visible = true;
            TabMusteri.Visible = true;
            labelControl11.Visible = true;
        }
        private void btnGizle3_Click(object sender, EventArgs e)
        {
            HideTab();
        }
        private void btnGizle2_Click(object sender, EventArgs e)
        {
            HideTab();
        }
        private void btnBelgeGizleGoster_Click(object sender, EventArgs e)
        {
            if (btnBelgeGizleGoster.Text == "Gizle")
            {
                btnBelgeGizleGoster.ImageOptions.Image = MalkocBilisim.Properties.Resources.down;
                btnBelgeGizleGoster.Text = "Göster";
                gcBelgeDetay.Visible = false;
            }
            else
            {
                btnBelgeGizleGoster.ImageOptions.Image = MalkocBilisim.Properties.Resources.up;
                btnBelgeGizleGoster.Text = "Gizle";
                gcBelgeDetay.Visible = true;
            }
        }
        private void btnMusteriSil_Click(object sender, EventArgs e)
        {
            string isim = txMusteriAdi_S.Text;
            System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
            result = XtraMessageBox.Show("[" + isim + "] isimli müşteriyle birlikte kişiye ve ya kuruma ait olan ödeme ve kayıtlı belgeler silinecektir. Onaylıyor musunuz?", "Uyarı", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ShowRec(Models.DeleteCustomer(MusteriIDGonder));
                TeknikSFormListele.Instance.ShowRec(Models.TeknikTablos);
                SatisFormListele.Instance.ShowRec(Models.SatisTablos);
            }
            BilgileriDoldur(Convert.ToInt32(gvMusteri.GetFocusedRowCellValue(clMusteriID)));
        }
    }
}
