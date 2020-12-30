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
using MalkocBilisim.DB;
using MalkocBilisim.Classes;
using MalkocBilisim.Properties;
using DevExpress.Utils.Menu;

namespace MalkocBilisim
{
    public partial class Odemeler : DevExpress.XtraEditors.XtraUserControl
    {
        private static Odemeler _instance;
        private int OdemeIDGonder = 0;
        OdemeTablo record;
        int teknikorSatis = 0; //0 teklif, 1 teknik, 2 satis
        public static Odemeler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Odemeler();
                }
                return _instance;
            }
        }
        public Odemeler()
        {
            InitializeComponent();
            ShowRec(Models.OdemeTablos);

        }
        public void ShowRec(IQueryable<OdemeTablo> t)
        {
            var result = from get in t
                         select new
                         {
                             get.OdemeID,
                             MusteriAdi = get.MusteriTablo.MusteriIsim,
                             Kategori = get.TeknikID != null ? "Teknik Servis" : (get.SatisTablo.SatisDurumu == false ? "Teklif" : "Satış"),
                             Tarih = get.TeknikID != null ? (DateTime)get.TeknikTablo.Tarih : (DateTime)get.SatisTablo.Tarih,
                             OdemeDurumu = get.SatisTablo.SatisDurumu == false ? "Teklif" : (get.OdemeDurumu == true ? "Ödendi" : "Ödenmedi")
                         };
            gcOdemeListesi.DataSource = result;
            BilgileriDoldur(OdemeIDGonder);
        }
        public void visible(bool value)
        {
            listOdemeTarih.Visible = value;
            label6.Visible = value;
            panelTutar.Visible = value;
            btnBorcuKapat.Visible = value;
            lbOdemeTarih.Visible = value;
            ckTekliftoSatis.Visible = value == true ? false : true;
        }
        public void tekliftoSatisClear()
        {
            ckTekliftoSatis.Checked = false;
            txTeklifTutar.Text = "0";
            panelTeklif.Visible = false;
        }
        public void BilgileriDoldur(int OdemeID)
        {
            lbBelgeDetay.Visible = false;
            panelIcerik.Visible = false;
            btnBelgeDetayGoster.Text = "İçeriğini Göster";
            btnBelgeDetayGoster.ImageOptions.Image = Resources.arrow_28_16;
            lbNo.Text = OdemeID.ToString();
            record = Models.GetOdemeRecord(OdemeID);
            teknikorSatis = record.TeknikID != null ? 1 : (record.SatisTablo.SatisDurumu == true ? 2 : 0); //0 teklif, 1 teknik, 2 satis
            lbBaslik.Text = "Ödeme Detayları (" + record.MusteriTablo.MusteriIsim + " - " + (teknikorSatis == 1 ? "Teknik Servis" : (teknikorSatis == 2 ? "Satış" : "Teklif")) + ")";

            if (teknikorSatis == 0)
            {
                visible(false);
            }
            else
            {
                visible(true);
                tekliftoSatisClear();
                listOdemeTarih.DataSource = teknikorSatis == 0 ? null : Models.GetOdemeDetayRecordsList(OdemeID);
                lbToplamTutar.Text = String.Format("{0:C}", record.ToplamTutar);
                lbAlinan.Text = String.Format("{0:C}", record.AlinanTutar);
                lbKalan.Text = String.Format("{0:C}", record.ToplamTutar - record.AlinanTutar);
                lbOdemeSekli.Text = record.OdemeSekliTablo.Secenek;
                lbKur.Text = record.KurTablo.Kur;
                panelOnay.BackgroundImage = record.OdemeDurumu == true ? Resources.check : Resources.warning2;
            }
        }
        private void gvOdemeListesi_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                OdemeIDGonder = Convert.ToInt32(gvOdemeListesi.GetRowCellValue(e.FocusedRowHandle, clOdemeNo));
                if (OdemeIDGonder != 0)
                    BilgileriDoldur(OdemeIDGonder);
            }
            catch (Exception)
            {
                MyControl.Mesaj("Sorun ile karşılaşıldı.", 1);
            }
        }
        private void ckTekliftoSatis_CheckedChanged(object sender, EventArgs e)
        {
            panelTeklif.Visible = ckTekliftoSatis.Checked == true ? true : false;
        }
        private void btnEkle_Click(object sender, EventArgs e)
        {
            txTutarDegisen.Text = txTutarDegisen.Text;
            decimal enterPrice = Convert.ToDecimal(txTutarDegisen.Text);
            decimal kalan = Convert.ToDecimal(record.ToplamTutar - record.AlinanTutar);
            decimal borc = Convert.ToDecimal(record.ToplamTutar) - kalan;

            if (enterPrice > kalan || enterPrice == 0 || (enterPrice * -1) > borc)
            {
                MyControl.Mesaj("Girdiğiniz tutar toplam tutarı geçmekte ve ya tutar girişi yapılmadı, lütfen kontrol edip yeni tutar giriniz.", 1);
            }
            else
            {
                Models.PlusorMinus(enterPrice, OdemeIDGonder);
                ShowRec(Models.OdemeTablos.Where(x => x.OdemeID == OdemeIDGonder));
                BilgileriDoldur(OdemeIDGonder);
            }
            txTutarDegisen.Text = "0";
        }
        private void btnCikar_Click(object sender, EventArgs e)
        {
            txTutarDegisen.Text = txTutarDegisen.Text;
            decimal enterPrice = Convert.ToDecimal(txTutarDegisen.Text);
            decimal kalan = Convert.ToDecimal(record.ToplamTutar - record.AlinanTutar);
            decimal borc = Convert.ToDecimal(record.ToplamTutar) - kalan;
            enterPrice = enterPrice > 0 ? enterPrice * -1 : enterPrice;

            if (enterPrice > kalan || enterPrice == 0 || (enterPrice * -1) > borc)
            {
                MyControl.Mesaj("Girdiğiniz tutar toplam tutarı geçmekte ve ya tutar girişi yapılmadı, lütfen kontrol edip yeni tutar giriniz.", 1);
            }
            else
            {
                Models.PlusorMinus(enterPrice, OdemeIDGonder);
                ShowRec(Models.OdemeTablos.Where(x => x.OdemeID == OdemeIDGonder));
                BilgileriDoldur(OdemeIDGonder);
            }
            txTutarDegisen.Text = "0";
        }
        private void btnBorcuKapat_Click(object sender, EventArgs e)
        {
            txTutarDegisen.Text = txTutarDegisen.Text;
            if (record.OdemeDurumu == false)
            {
                Models.BorcuKapat(OdemeIDGonder);
                ShowRec(Models.OdemeTablos.Where(x => x.OdemeID == OdemeIDGonder));
                BilgileriDoldur(OdemeIDGonder);
            }
            else
            {
                MyControl.Mesaj("Müşterinin borcu yoktur.", 1);
            }
            txTutarDegisen.Text = "0";
        }
        private void btnButunKayitlar_Click(object sender, EventArgs e)
        {
            ShowRec(Models.OdemeTablos);
        }
        private void btnTeklifMAX_Click(object sender, EventArgs e)
        {
            txTeklifTutar.Text = String.Format("{0:C}", record.ToplamTutar);
        }
        private void btnTeklifKaydet_Click(object sender, EventArgs e)
        {
            txTeklifTutar.Text = txTeklifTutar.Text;
            decimal enterPrice = Convert.ToDecimal(txTeklifTutar.Text);

            if (enterPrice == 0 || enterPrice > record.ToplamTutar)
            {
                MyControl.Mesaj("Girdiğiniz tutar teklifin toplam tutarından fazladır ve ya tutar girişi olmamıştır. Girdiğiniz değeri lütfen kontrol edin.", 1);
            }
            else
            {
                Models.TekliftoSatis(enterPrice, OdemeIDGonder, rbNakit.Checked == true ? 1 : 2);
            }
            BilgileriDoldur(OdemeIDGonder);
            ShowRec(Models.OdemeTablos.Where(x => x.OdemeID == OdemeIDGonder).Select(x => x));


        }
        private void txTutarDegisen_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = Char.IsPunctuation(e.KeyChar);
        }
        private void txTeklifTutar_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = Char.IsPunctuation(e.KeyChar);
        }
        private void btnBelgeDetayGoster_Click(object sender, EventArgs e)
        {
            if (btnBelgeDetayGoster.Text == "İçeriğini Göster")
            {
                lbBelgeDetay.Visible = true;
                panelIcerik.Visible = true;
                btnBelgeDetayGoster.Text = "İçeriği Gizle";
                btnBelgeDetayGoster.ImageOptions.Image = Resources.arrow_92_16;

                if (teknikorSatis == 1)
                {
                    gcIcerik.Visible = false;
                    TeknikModel.GetArizaDetail((int)record.TeknikID);
                    TeknikModel.GetIslemDetail((int)record.TeknikID);
                    TeknikModel.GetParcaDetail((int)record.TeknikID);
                    txArizaBilgisi.Text = TeknikModel.ArizaDetails;
                    txYapilanIslemler.Text = TeknikModel.IslemDetails;
                    txDegisenParcalar.Text = TeknikModel.ParcaDetails;
                }
                else
                {
                    gcIcerik.Visible = true;
                    gcIcerik.DataSource = Models.SatisDetayTablos.Where(x => x.SatisID == record.SatisID).Select(x => x);
                }
            }
            else
            {
                btnBelgeDetayGoster.Text = "İçeriğini Göster";
                lbBelgeDetay.Visible = false;
                panelIcerik.Visible = false;
                btnBelgeDetayGoster.ImageOptions.Image = Resources.arrow_28_16;
            }
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            CreatePDF openPDF = new CreatePDF();
            bool check;

            System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();


            if (teknikorSatis == 1)
            {
                check = openPDF.OpenTeknikPDF(Convert.ToInt32(record.TeknikID));
                if (check != true)
                {
                    result = XtraMessageBox.Show("Belirtilen pdf dosyası bulunamamıştır.Yeniden oluşturulma işlemi gerçekleştirilsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        CreatePDF createPdf = new CreatePDF(Convert.ToInt32(record.TeknikID));
                        createPdf.CreateTeknikPDF();
                    }
                }
            }
            else
            {
                check = openPDF.OpenSatisPDF(Convert.ToInt32(record.SatisID));
                if (check != true)
                {
                    result = XtraMessageBox.Show("Belirtilen pdf dosyası bulunamamıştır.Yeniden oluşturulma işlemi gerçekleştirilsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        CreatePDF createPdf = new CreatePDF(Convert.ToInt32(record.SatisID));
                        createPdf.CreateSatisPDF();
                    }
                }
            }


        }

        private void gvOdemeListesi_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                {
                    DXMenuItem musteriyeGit = new DXMenuItem("Müşteri Detayı", MusteriyeYonlendirClick);
                    DXMenuItem belgeyeGit = new DXMenuItem("Belge Detayı", BelgeyeYonlendirClick);
                    e.Menu.Items.Add(musteriyeGit);
                    e.Menu.Items.Add(belgeyeGit);
                }
            }
            catch (NullReferenceException)
            {
            }
        }
        public void MusteriyeYonlendirClick(Object sender, EventArgs e)
        {
            Musteri.Instance.ShowRec(Models.MusteriTablos.Where(x => x.MusteriID == record.MusteriID).Select(x => x));
            Musteri.Instance.BringToFront();
        }
        public void BelgeyeYonlendirClick(Object sender, EventArgs e)
        {
            if (teknikorSatis == 1)
            {
                TeknikSFormListele.Instance.ShowRec(Models.TeknikTablos.Where(x => x.TeknikID == record.TeknikID).Select(x => x));
                TeknikSFormListele.Instance.BringToFront();
            }
            else
            {
                SatisFormListele.Instance.ShowRec(Models.SatisTablos.Where(x => x.SatisID == record.SatisID).Select(x => x));
                SatisFormListele.Instance.BringToFront();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
            result = XtraMessageBox.Show("[" + TeknikModel.CustName + "] isimli müşterinin [" + OdemeIDGonder.ToString() + "] ödeme numaralı kayıt detayları silinecektir. Onaylıyor musunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                if (teknikorSatis == 1)
                {
                    TeknikModel.DeleteRecord(Convert.ToInt32(record.TeknikID), true);
                    TeknikSFormListele.Instance.ShowRec(Models.TeknikTablos);
                }
                else
                {
                    SatisModel.DeleteRecord(Convert.ToInt32(record.SatisID), true);
                    SatisFormListele.Instance.ShowRec(Models.SatisTablos);
                }
                ShowRec(Models.OdemeTablos);
            }
            else
            {
                //Record not delete.
            }

        }

        private void btnOdenmemisBelge_Click(object sender, EventArgs e)
        {
            var result = Models.OdemeTablos.Where(x => x.OdemeDurumu == false && x.SekilID != 3).Select(x => x);

            if (result.Count() == 0)
            {
                MyControl.Mesaj("Ödenmemiş belge bulunmamaktadır.", 0);
            }
            else
            {
                Odemeler.Instance.ShowRec(result);
            }
        }

        private void btnFarkliKaydet_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.Title = "PDF Dosyasını Kaydet";
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "PDF |*.pdf";
            saveFileDialog1.FileName = teknikorSatis == 1 ? (record.TeknikID.ToString() + "TSF") : (teknikorSatis == 2 ? (record.SatisID.ToString() + "S") : (record.SatisID.ToString() + "T")); 
            DialogResult result = saveFileDialog1.ShowDialog();

            if (teknikorSatis==1)
            {
                if (result == DialogResult.OK)
                {
                    CreatePDF crt = new CreatePDF(Convert.ToInt32(record.TeknikID), saveFileDialog1.FileName);
                    crt.CreateTeknikPDF();
                }
            }
            else
            {
                if (result == DialogResult.OK)
                {
                    CreatePDF crt = new CreatePDF(Convert.ToInt32(record.SatisID), saveFileDialog1.FileName);
                    crt.CreateTeknikPDF();
                }
            }
        }
    }
}
