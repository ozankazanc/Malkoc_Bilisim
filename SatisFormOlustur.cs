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
using System.ComponentModel.DataAnnotations;
using MalkocBilisim.Classes;
using MalkocBilisim.DB;
using DevExpress.Utils.Menu;

namespace MalkocBilisim
{
    public partial class SatisFormOlustur : DevExpress.XtraEditors.XtraUserControl
    {
        private static SatisFormOlustur _instance;
        public static SatisFormOlustur Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SatisFormOlustur();
                }
                return _instance;
            }

        }
        class TeklifForm
        {

            [Required(ErrorMessage = "Bu alan boş geçilemez.")]
            public string Aciklama { get; set; } = "Lütfen ürün bilgisini giriniz.";
            [Range(1, Int32.MaxValue, ErrorMessage = "Girilen tutar 0 ve ya negatif olamaz.")]
            public int Miktar { get; set; } = 1;
            [Range(typeof(decimal), "0", "79228162514264337593543950335")]
            public decimal BirimFiyat { get; set; }
            public decimal Tutar { get; set; }
        }
        private void Temizle()
        {
            txAdi.Text = ""; txKonu.Text = "";
            txToplamUrun.Text = ""; txTutar.Text = ""; txKDV.Text = ""; txToplamTutar.Text = "";
            rbTL.Checked = true; rbNakit.Checked = true; rbTeklif.Checked = true;
            txAlinanTutar.Text = "";
            dtTarih.EditValue = DateTime.Now;
            panelCheck.BackgroundImage = null;

            for (int i = 0; i < gvTeklifForm.RowCount; i++)
                gvTeklifForm.DeleteRow(i);
            for (int i = 0; i < gvTeklifForm.RowCount; i++)
                gvTeklifForm.DeleteRow(i);
            for (int i = 0; i < gvTeklifForm.RowCount; i++)
                gvTeklifForm.DeleteRow(i);
            for (int i = 0; i < gvTeklifForm.RowCount; i++)
                gvTeklifForm.DeleteRow(i);
            for (int i = 0; i < gvTeklifForm.RowCount; i++)
                gvTeklifForm.DeleteRow(i);
        }
        private void UpdateSatisID()
        {
            lbNo.Text = (SatisModel.getLastSatisID() + 1).ToString();
        }
        public SatisFormOlustur()
        {
            InitializeComponent();
            bindingForm.DataSource = typeof(TeklifForm);
            gcTeklifForm.DataSource = bindingForm;
            dtTarih.EditValue = DateTime.Now;
            UpdateSatisID();
            toolTipCheck.SetToolTip(panelCheck, "Müşteri kayıt durumu(Ünlem: Kayıtlı değil, Onay: Kayıtlı)");
        }
        private void btnKaydet_Click(object sender, EventArgs e)
        {

            Hesapla();
            if (String.IsNullOrWhiteSpace(txAdi.Text) || (gvTeklifForm.RowCount - 1) == 0)
            {
                MyControl.Mesaj("[Müşteri ve ya Firma adı] alanı boş geçilmemeli ve listeye en az bir ürün girilmelidir.", 1);
            }
            else
            {
                string urunAdi = ""; int urunMiktari = 0; decimal urunBirimFiyat = 0.0M, TotalPrice = 0.0M;

                for (int i = 0; i < gvTeklifForm.RowCount - 1; i++)
                {
                    urunAdi = gvTeklifForm.GetRowCellValue(i, clAciklama).ToString();
                    urunMiktari = Convert.ToInt32(gvTeklifForm.GetRowCellValue(i, clMiktar));
                    urunBirimFiyat = Convert.ToDecimal(gvTeklifForm.GetRowCellValue(i, clBirimFiyat));
                    TotalPrice += urunMiktari * urunBirimFiyat;

                    SatisModel.ListofDetays.Add(
                        new SatisDetayTablo
                        {
                            UrunAdi = urunAdi,
                            UrunMiktari = urunMiktari,
                            UrunBirimFiyat = urunBirimFiyat < 0 ? urunBirimFiyat * -1 : urunBirimFiyat,
                            UrunToplamTutar = urunMiktari * urunBirimFiyat,
                            SatisID = 0,
                            Sil = true
                        });
                }

                MyControl.Mesaj(SatisModel.InsertRecord(txAdi.Text, txKonu.Text, dtTarih.DateTime,
                     rbTL.Checked == true ? true : false, rbSatis.Checked == true ? true : false,
                     rbNakit.Checked == true ? true : false, ckKdvOnay.Checked == true ? true : false, Convert.ToDecimal(txAlinanTutar.Text),
                     ckKdvOnay.Checked == true ? TotalPrice : (TotalPrice * 1.18M)), 0);

                SatisFormListele.Instance.ShowRec(Models.SatisTablos);
                Odemeler.Instance.ShowRec(Models.OdemeTablos);
                Musteri.Instance.ShowRec(Models.MusteriTablos);
                UpdateSatisID();
                Temizle();
            }
        }

        decimal birimFiyat, tutar, gosterilenTutar; int miktar;
        private void gvTeklifForm_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            miktar = Convert.ToInt32(gvTeklifForm.GetRowCellValue(e.RowHandle, clMiktar));
            birimFiyat = Convert.ToDecimal(gvTeklifForm.GetRowCellValue(e.RowHandle, clBirimFiyat));
            gosterilenTutar = Convert.ToDecimal(gvTeklifForm.GetRowCellValue(e.RowHandle, clTutar));

            tutar = miktar * birimFiyat;

            if (gosterilenTutar != tutar)
            {
                gvTeklifForm.SetRowCellValue(e.RowHandle, clTutar, tutar);
            }
        }

        decimal Tutar = 0.0M; int toplamUrun = 0;
        private void Hesapla()
        {
            Tutar = 0.0M;
            toplamUrun = 0;
            for (int i = 0; i < gvTeklifForm.RowCount; i++)
            {
                Tutar += Convert.ToDecimal(gvTeklifForm.GetRowCellValue(i, clTutar));
                toplamUrun += Convert.ToInt32(gvTeklifForm.GetRowCellValue(i, clMiktar));
            }

            txToplamUrun.Text = toplamUrun.ToString();
            txKDV.Text = String.Format("{0:C}", ckKdvOnay.Checked == true ? (Tutar - (Tutar / 1.18M)) : (Tutar * 0.18M));
            txTutar.Text = String.Format("{0:C}", ckKdvOnay.Checked == true ? (Tutar / 1.18M) : Tutar);
            txToplamTutar.Text = String.Format("{0:C}", ckKdvOnay.Checked == true ? Tutar : (Tutar * 1.18M));
        }
        private void btnHesapla_Click(object sender, EventArgs e)
        {
            Hesapla();
        }
        private void gvTeklifForm_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                {
                    DXMenuItem sil = new DXMenuItem("Kaydı Sil", silonClick);
                    e.Menu.Items.Add(sil);
                    DXMenuItem yenile = new DXMenuItem("Yenile", yenileOnClick);
                    e.Menu.Items.Add(yenile);
                }
                else
                {
                    Hesapla(); //gridte boş alana sağ tıklanırsa hesaplama işlemi yapılıyor.
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void silonClick(Object sender, EventArgs e)
        {
            gvTeklifForm.DeleteSelectedRows();
        }
        public void yenileOnClick(Object sender, EventArgs e)
        {
            Hesapla();
        }
        private void btnTamami_Click(object sender, EventArgs e)
        {
            if (txTutar.Text == "" || txTutar.Text == null)
            {
                MyControl.Mesaj("Listede ürün olduğundan ve tutar hesaplaması yapıldığından emin olun. ( [Tutar] alanı boş gözükmemeli. )", 1);
            }
            else
            {
                txAlinanTutar.Text = String.Format("{0:C}", ckKdvOnay.Checked == true ? Tutar : (Tutar * 1.18M));
            }
        }
        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }
        private void txAdi_Leave(object sender, EventArgs e)
        {
            string araIsim = txAdi.Text;
            MusteriTablo musteri = SatisModel.MusteriTablos.FirstOrDefault(x => x.MusteriIsim == araIsim);
            panelCheck.BackgroundImage = musteri == null ? Properties.Resources.warning2 : Properties.Resources.check;
        }
        private void rbSatis_CheckedChanged(object sender, EventArgs e)
        {
            panelSatis.Visible = rbSatis.Checked == true ? true : false;
        }
    }
}
