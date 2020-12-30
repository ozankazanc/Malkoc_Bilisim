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
using DevExpress.Utils.Menu;

namespace MalkocBilisim
{
    public partial class SatisFormListele : DevExpress.XtraEditors.XtraUserControl
    {
        private static SatisFormListele _instance;
        int Category = 0;
        int satisIDGonder = 0;
        SatisTablo record;
        public static SatisFormListele Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SatisFormListele();
                }
                return _instance;
            }
        }
        public SatisFormListele()
        {
            InitializeComponent();
            ShowRec(Models.SatisTablos);
           
        }
        private void ClearSearchControl()
        {
            cbCesit.SelectedIndex = 0;
            cbArama.SelectedIndex = 0;
            txArama.Text = "";
            cbDonem.Text = "";
        }
        public void ShowRec(IQueryable<SatisTablo> t)
        {
            ClearSearchControl();
            gcTeklifler.DataSource = t;
            BilgileriDoldur(Convert.ToInt32(gvTeklifler.GetFocusedRowCellValue(clSatisID)));
        }
        public void BilgileriDoldur(int satisID)
        {
            record = SatisModel.GetSatisRecord(satisID); //Teklif Listesinden Seçilen kaydın ürün bilgileri
            if (record != null)
            {
                gcTeklifIcerik.DataSource = SatisModel.GetSatisDetail(satisID);
                lbSatisDetays.Text = "(" + SatisModel.CustName + " - " + (record.SatisDurumu == true ? "Satış" : "Teklif") + ")";
                txKDV.Text = String.Format("{0:C}", SatisModel.KDV);
                txTutar.Text = String.Format("{0:C}", SatisModel.TotPrice);
                txToplamTutar.Text = String.Format("{0:C}", SatisModel.TotalPrice);
                lbNo.Text = satisID.ToString();
                int toplamUrun = 0;
                for (int i = 0; i < gvTeklifIcerik.RowCount; i++)
                    toplamUrun += Convert.ToInt32(gvTeklifIcerik.GetRowCellValue(i, clMiktar));

                txToplamUrun.Text = toplamUrun.ToString();
                if (SatisModel.KDVCheck)
                {
                    txKDVBilgilendirme.ForeColor = Color.Green;
                    txKDVBilgilendirme.Text = "Ürünlere KDV Dahil";
                }
                else
                {
                    txKDVBilgilendirme.ForeColor = Color.Yellow;
                    txKDVBilgilendirme.Text = "Toptan Üzerinden";
                }
            }
            else
            {
                lbSatisDetays.Text = "";
                gcTeklifIcerik.DataSource = null;
            }

        }
        private void gvTeklifler_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                satisIDGonder = Convert.ToInt32(gvTeklifler.GetRowCellValue(e.FocusedRowHandle, clSatisID));
                if (satisIDGonder != 0)
                {
                    BilgileriDoldur(satisIDGonder);
                }
            }
            catch (Exception)
            {
                MyControl.Mesaj("Sorun ile karşılaşıldı.", 1);
            }
        }
        private void btnKayitGoster_Click(object sender, EventArgs e)
        {
            ShowRec(Models.SatisTablos);
        }
        private void btnSatisGoster_Click(object sender, EventArgs e)
        {
            ClearSearchControl();
            ShowRec(SatisModel.GetListonlySatis());
        }
        private void btnTeklifGoster_Click(object sender, EventArgs e)
        {
            ClearSearchControl();
            ShowRec(SatisModel.GetListonlyTeklif());
        }
        private void btnArama_Click(object sender, EventArgs e)
        {
            string search = txArama.Text;
            if (cbArama.SelectedIndex == 0)
            {
                ShowRec(SatisModel.SearchbyCustNameSatis(search, Category));
            }
            else if (cbArama.SelectedIndex == 1)
            {
                try
                {
                    ShowRec(SatisModel.SearchbySatisID(Convert.ToInt32(search)));
                }
                catch (FormatException)
                {
                    MyControl.Mesaj("[Belge No] rakamlardan oluşmalıdır.", 2);
                }
            }
            else
            {
                ShowRec(SatisModel.SearchbyProductSatis(search, Category));
            }
        }
        private void cbDonem_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Months = 1 - DateTime.Now.Day;

            if (cbDonem.SelectedIndex == 0)
            {
                ShowRec(SatisModel.SearchDateRangeDay(DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1), Category));
            }
            else if (cbDonem.SelectedIndex == 1)
            {
                ShowRec(SatisModel.SearchDate(DateTime.Now.AddDays(-7), Category)); //week
            }
            else if (cbDonem.SelectedIndex == 2)
            {
                ShowRec(SatisModel.SearchDate(DateTime.Now.AddDays(Months), Category)); //month
            }
            else if (cbDonem.SelectedIndex == 3)
            {
                ShowRec(SatisModel.SearchDateRangeYear(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1), Category));
            }
        }

        DateTime startDate;
        private void dtBaslangic_EditValueChanged(object sender, EventArgs e)
        {
            startDate = dtBaslangic.DateTime;
            ShowRec(SatisModel.SearchDate(startDate, Category));
        }
        private void dtBitis_EditValueChanged(object sender, EventArgs e)
        {
            if (startDate != null)
                ShowRec(SatisModel.SearchDateRangeYear(startDate, dtBitis.DateTime, Category));
        }
        private void btnKayitSil_Click(object sender, EventArgs e)
        {
            string isim = SatisModel.CustName;

            System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
            result = XtraMessageBox.Show("[" + isim + "] isimli müşterinin, [" + satisIDGonder.ToString() + "] belge numaralı kayıt detayları silinecektir.Onaylıyor musunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ShowRec(SatisModel.DeleteRecord(satisIDGonder, true));
            }
            else { }
        }
        private void cbCesit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Category = cbCesit.SelectedIndex;
        }
        private void btnUzat_Click(object sender, EventArgs e)
        {
            gcTeklifler.Height = 523;
            gcTeklifIcerik.Visible = false;
            lbSatisDetay.Visible = false;
            btnUzat.Visible = false;
            btnKucult.Visible = true;
            panelTutar.Visible = false;
        }
        private void btnKucult_Click(object sender, EventArgs e)
        {
            gcTeklifler.Height = 208;
            gcTeklifIcerik.Visible = true;
            lbSatisDetay.Visible = true;
            btnUzat.Visible = true;
            panelTutar.Visible = true;
            btnKucult.Visible = false;
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {

            CreatePDF openPdf = new CreatePDF();
            bool check = openPdf.OpenSatisPDF(satisIDGonder);
            if (check != true)
            {
                System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
                result = XtraMessageBox.Show("Belirtilen pdf dosyası bulunamamıştır.Yeniden oluşturulma işlemi gerçekleştirilsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    CreatePDF createPdf = new CreatePDF(satisIDGonder);
                    createPdf.CreateSatisPDF();
                }
            }
        }

        private void gvTeklifler_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                {
                    DXMenuItem musteriyeGit = new DXMenuItem("Müşteri Detayı", MusteriyeYonlendirClick);
                    DXMenuItem belgeyeGit = new DXMenuItem("Ödeme Detayı", OdemeyeYonlendirClick);
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
        public void OdemeyeYonlendirClick(Object sender, EventArgs e)
        {

            Odemeler.Instance.ShowRec(Models.OdemeTablos.Where(x => x.SatisID == record.SatisID).Select(x => x));
            Odemeler.Instance.BringToFront();
        }

        private void btnFarkliKaydet_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.Title = "PDF Dosyasını Kaydet";
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "PDF |*.pdf";
            saveFileDialog1.FileName = record.SatisID.ToString() + (record.SatisDurumu == true ? "S" : "T"); 
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                CreatePDF crt = new CreatePDF(record.SatisID, saveFileDialog1.FileName);
                crt.CreateSatisPDF();
            }
        }
    }
}
