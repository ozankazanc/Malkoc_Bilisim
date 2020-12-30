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
    public partial class TeknikSFormListele : DevExpress.XtraEditors.XtraUserControl
    {
        private static TeknikSFormListele _instance;
        int teknikIDGonder = 0;
        TeknikTablo record;
        public static TeknikSFormListele Instance
        {
            get
            {
                if (_instance == null)
                { _instance = new TeknikSFormListele(); }

                return _instance;
            }

        }
        public TeknikSFormListele()
        {
            InitializeComponent();
            ShowRec(Models.TeknikTablos);
        }
        private void ClearSearchControl()
        {
            cbArama.SelectedIndex = 0;
            txArama.Text = "";
            cbDonem.Text = "";
        }
        public void ShowRec(IQueryable<TeknikTablo> t)
        {
            ClearSearchControl();
            gcSonucGosterme.DataSource = t;
            BilgileriDoldur(Convert.ToInt32(gvSonucGosterme.GetFocusedRowCellValue(clTeknikID)));
        }
        public void BilgileriDoldur(int teknikID)
        {
            record = TeknikModel.GetTeknikRecord(teknikID);
            if (record != null)
            {
                lbNo.Text = record.TeknikID.ToString();
                lbNot.Text = record.Not;
                lbDetay.Text = "Servis Formu Detayları (" + TeknikModel.CustName + " - " + String.Format("{0:C}", TeknikModel.TotalPrice) + ")";

                txArizaBilgisi.Text = TeknikModel.ArizaDetails;
                txYapilanIslem.Text = TeknikModel.IslemDetails;
                txDegisenParca.Text = TeknikModel.ParcaDetails;
            }
            else
            {
                txArizaBilgisi.Text = "";
                txYapilanIslem.Text = "";
                txDegisenParca.Text = "";
                lbDetay.Text = "Servis Formu Detayları";
                lbNot.Text = "";
            }
        }
        private void gvSonucGosterme_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                teknikIDGonder = Convert.ToInt32(gvSonucGosterme.GetRowCellValue(e.FocusedRowHandle, clTeknikID));

                if (teknikIDGonder != 0)
                    BilgileriDoldur(teknikIDGonder);
            }
            catch (Exception)
            {
                MyControl.Mesaj("Sorun ile karşılaşıldı.", 1);
            }
        }
        private void btnArama_Click(object sender, EventArgs e)
        {
            string search = txArama.Text;

            if (search == "")
            {
                MyControl.Mesaj("Aranacak ifadeyi yukarıdaki kutucuğa giriniz.", 0);
            }
            else
            {
                if (cbArama.SelectedIndex == 0)
                {
                    ShowRec(TeknikModel.SearchbyName(search));
                }
                else if (cbArama.SelectedIndex == 1)
                {
                    ShowRec(TeknikModel.SearchbyPhone(search));
                }
                else if (cbArama.SelectedIndex == 2)
                {
                    ShowRec(TeknikModel.SearchbyAriza(search));
                }
                else if (cbArama.SelectedIndex == 3)
                {
                    ShowRec(TeknikModel.SearchbyIslem(search));
                }
                else if (cbArama.SelectedIndex == 4)
                {
                    ShowRec(TeknikModel.SearchbyParca(search));
                }
                else if (cbArama.SelectedIndex == 5)
                {
                    try
                    {
                        ShowRec(TeknikModel.SearchbyTeknikID(Convert.ToInt32(search)));
                    }
                    catch (FormatException)
                    {
                        MyControl.Mesaj("[Belge No] rakamlardan oluşmalıdır.", 2);
                    }
                }
            }
        }
        private void btnKayitGoster_Click(object sender, EventArgs e)
        {
            ShowRec(Models.TeknikTablos);
        }
        private void btnSil_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
            result = XtraMessageBox.Show("[" + TeknikModel.CustName + "] isimli müşterinin [" + teknikIDGonder.ToString() + "] belge numaralı kayıt detayları silinecektir. Onaylıyor musunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ShowRec(TeknikModel.DeleteRecord(teknikIDGonder, true));
            }
            else
            {
                //Record not delete.
            }
        }
        private void cbDonem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDonem.SelectedIndex == 0)
            {
                ShowRec(TeknikModel.SearchDateRange(DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1), true));
            } //day
            else if (cbDonem.SelectedIndex == 1)
            {
                ShowRec(TeknikModel.SearchDate(DateTime.Now.AddDays(-7)));
            } //week
            else if (cbDonem.SelectedIndex == 2)
            {
                ShowRec(TeknikModel.SearchDate(DateTime.Now.AddDays(1 - DateTime.Now.Day)));
            } //month
            else if (cbDonem.SelectedIndex == 3)
            {
                ShowRec(TeknikModel.SearchDateRange(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1), false));
            } //year
        }

        DateTime startDate;
        private void dtBaslangic_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                startDate = dtBaslangic.DateTime;
                ShowRec(TeknikModel.SearchDate(startDate));
            }
            catch (Exception)
            {

            }
        }
        private void dtBitis_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (startDate == null) { }
                else
                    ShowRec(TeknikModel.SearchDateRange(startDate, dtBitis.DateTime, false));
            }
            catch (Exception)
            {

            }
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            CreatePDF openPdf = new CreatePDF();
            bool check = openPdf.OpenTeknikPDF(teknikIDGonder);
            if (check != true)
            {
                System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
                result = XtraMessageBox.Show("Belirtilen pdf dosyası bulunamamıştır.Yeniden oluşturulma işlemi gerçekleştirilsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    CreatePDF createPdf = new CreatePDF(teknikIDGonder);
                    createPdf.CreateTeknikPDF();
                }
            }
        }

        private void gvSonucGosterme_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
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

            Odemeler.Instance.ShowRec(Models.OdemeTablos.Where(x => x.TeknikID == record.TeknikID).Select(x => x));
            Odemeler.Instance.BringToFront();
        }

        private void btnFarkliKaydet_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.Title = "PDF Dosyasını Kaydet";
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "PDF |*.pdf";
            saveFileDialog1.FileName = record.TeknikID.ToString() + "TSF";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                CreatePDF crt = new CreatePDF(teknikIDGonder, saveFileDialog1.FileName);
                crt.CreateTeknikPDF();
            }
        }
    }
}
