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
using DevExpress.Utils.Menu;
using MalkocBilisim.Classes;
using MalkocBilisim.DB;
using MalkocBilisim.Properties;

namespace MalkocBilisim
{
    public partial class TeknikSFormOlustur : DevExpress.XtraEditors.XtraUserControl
    {
        private static TeknikSFormOlustur _instance;
        #region Model
        class BilgiAriza
        {
            public string ArizaBilgisi { get; set; }
        }
        class IslemYapilan
        {
            public string YapilanIslem { get; set; }
        }
        class ParcaDegisen
        {
            public string DegisenParcalar { get; set; }
        }
        #endregion
        public TeknikSFormOlustur()
        {
            InitializeComponent();
            dpTarih.EditValue = DateTime.Now;

            bindingAriza.DataSource = typeof(BilgiAriza);
            bindingIslem.DataSource = typeof(IslemYapilan);
            bindingParca.DataSource = typeof(ParcaDegisen);
            gcArizaBilgisi.DataSource = bindingAriza;
            gcDegisenParca.DataSource = bindingParca;
            gcYapilanIslem.DataSource = bindingIslem;
            UpdateTeknikID();

            tooltipCheck.SetToolTip(panelCheck, "Müşteri kayıt durumu (Ünlem: Kayıtlı değil, Onay: Kayıtlı)");
        }
        public static TeknikSFormOlustur Instance
        {
            get
            {
                if (_instance == null)
                { _instance = new TeknikSFormOlustur(); }

                return _instance;
            }
        }
        private void UpdateTeknikID()
        {
            lbNo.Text = (TeknikModel.getLastTeknikID() + 1).ToString();
        }
        private void Temizle()
        {
            txAdi.Text = ""; txAdres.Text = ""; txTelefon.Text = ""; dpTarih.EditValue = DateTime.Now;
            txOnarimUcreti.Text = ""; txAlinanTutar.Text = ""; txNotEkle.Text = "";
            rbNakit.Checked = true; ckNotEkle.Checked = false; ckErtele.Checked = false;
            panelCheck.BackgroundImage = null;

            for (int i = 0; i < gvArizaBilgisi.RowCount; i++)
                gvArizaBilgisi.DeleteRow(0);
            for (int i = 0; i < gvYapilanIslem.RowCount; i++)
                gvYapilanIslem.DeleteRow(0);
            for (int i = 0; i < gvDegisenParca.RowCount; i++)
                gvDegisenParca.DeleteRow(0);
        }
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txAdi.Text == "" || Convert.ToDecimal(txOnarimUcreti.Text) == 0)
            {
                
                MyControl.Mesaj("[Müsteri ve ya Firma Adı] ve [Onarım Ücreti] alanları boş geçilemez.", 1);
            }
            else
            {

                string arizaBilgisi = "", yapilanIslem = "", degisenParca = "";

                for (int i = 0; i < gvArizaBilgisi.RowCount - 1; i++)
                {
                    arizaBilgisi = gvArizaBilgisi.GetRowCellValue(i, clArizaBilgisi).ToString();
                    if (String.IsNullOrWhiteSpace(arizaBilgisi) == false)
                        TeknikModel.ListArizaDetails.Add(
                        new ArizaTablo
                        {
                            TeknikID = 0,
                            ArizaBilgisi = arizaBilgisi,
                            Sil = true
                        });
                }
                for (int i = 0; i < gvYapilanIslem.RowCount - 1; i++)
                {
                    yapilanIslem = gvYapilanIslem.GetRowCellValue(i, clYapilanIslem).ToString();
                    if (String.IsNullOrWhiteSpace(yapilanIslem) == false)
                        TeknikModel.ListIslemDetails.Add(
                        new IslemTablo
                        {
                            TeknikID = 0,
                            YapilanIslem = yapilanIslem,
                            Sil = true
                        });
                }
                for (int i = 0; i < gvDegisenParca.RowCount - 1; i++)
                {
                    degisenParca = gvDegisenParca.GetRowCellValue(i, clDegisenParca).ToString();
                    if (String.IsNullOrWhiteSpace(degisenParca) == false)
                        TeknikModel.ListParcaDetails.Add(
                        new ParcaTablo
                        {
                            TeknikID = 0,
                            DegisenParca = degisenParca,
                            Sil = true
                        });
                }

                TeknikModel.InsertRecord(txAdi.Text, txTelefon.Text, txAdres.Text, dpTarih.DateTime, txNotEkle.Text, Convert.ToDecimal(txOnarimUcreti.Text),
                    ckErtele.Checked == false ? Convert.ToDecimal(txOnarimUcreti.Text) : Convert.ToDecimal(txAlinanTutar.Text), rbNakit.Checked == true ? 1 : 2);

                TeknikSFormListele.Instance.ShowRec(Models.TeknikTablos);
                UpdateTeknikID();
                Temizle();
                Musteri.Instance.ShowRec(Models.MusteriTablos);
                Odemeler.Instance.ShowRec(Models.OdemeTablos);

            }
        }
        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }
        private void ckErtele_CheckStateChanged(object sender, EventArgs e)
        {
            panelErtelemeli.Visible = ckErtele.Checked == true ? true : false;
        }
        private void txAdi_Leave(object sender, EventArgs e)
        {
            string araIsim = txAdi.Text;

            if (araIsim != "")
            {
                MusteriTablo musteri = TeknikModel.MusteriTablos.FirstOrDefault(x => x.MusteriIsim == araIsim);
                if (musteri == null)
                {
                    panelCheck.BackgroundImage = Properties.Resources.warning2;
                }
                else
                {
                    panelCheck.BackgroundImage = Properties.Resources.check;
                    txAdi.Text = musteri.MusteriIsim;
                    txAdres.Text = musteri.MusteriAdres;
                    txTelefon.Text = musteri.MusteriTelefon;
                    
                }
            }
        }
        private void ckNotEkle_CheckStateChanged(object sender, EventArgs e)
        {
            panelNot.Visible = ckNotEkle.Checked == true ? true : false;
        }
        private void dpTarih_Leave(object sender, EventArgs e)
        {
            if (dpTarih.Text == "")
            {
                dpTarih.DateTime = DateTime.Now;
            }
        }
        private void gvArizaBilgisi_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                {
                    DXMenuItem sil = new DXMenuItem("Satırı Sil", silonClick, Properties.Resources.delete);
                    e.Menu.Items.Add(sil);
                }
            }
            catch (NullReferenceException)
            {
            }
        }
        private void gvYapilanIslem_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                {
                    DXMenuItem sil = new DXMenuItem("Satırı Sil", silonClick2, Properties.Resources.delete);
                    e.Menu.Items.Add(sil);
                }
            }
            catch (NullReferenceException)
            {
            }
        }
        private void gvDegisenParca_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                {
                    DXMenuItem sil = new DXMenuItem("Satırı Sil", silonClick3, Properties.Resources.delete);
                    e.Menu.Items.Add(sil);
                }
            }
            catch (NullReferenceException)
            {
            }
        }
        public void silonClick(Object sender, EventArgs e)
        {
            gvArizaBilgisi.DeleteSelectedRows();
        }
        public void silonClick2(Object sender, EventArgs e)
        {
            gvYapilanIslem.DeleteSelectedRows();
        }
        public void silonClick3(Object sender, EventArgs e)
        {
            gvDegisenParca.DeleteSelectedRows();
        }

    }
}
