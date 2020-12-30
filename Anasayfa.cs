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
using System.Xml;
using System.Deployment.Application;

namespace MalkocBilisim
{
    public partial class Anasayfa : DevExpress.XtraEditors.XtraUserControl
    {

        private static Anasayfa _instance;
        string USD, EUR;
        int index;
        SonucModel bilgiDoldur = new SonucModel();


        class SonucModel
        {
            private IQueryable<AnasayfaModel> AnasayfaModels;
            public string[,] Odeme { get; set; }
            public string[,] Belge { get; set; }
            public SonucModel()
            {
                //4 - 0 hepsi, 1 teknik, 2 satis, 3teklif
                //7 -Odeme - 0 Gun, 1 Hafta, 2 Ay, 3 Yil, 4 alinantutar, 5 toplamtutar,6 borc tutar
                //5 ,Belge -  0 Gun, 1 Hafta, 2 Ay, 3 Yil , 4 hepsi
                Odeme = new string[4, 7];
                Belge = new string[4, 5];

                AnasayfaModels = from get in Models.OdemeTablos
                                 select new AnasayfaModel
                                 {
                                     OdemeID = get.OdemeID,
                                     Tarih = get.TeknikID == null ? (DateTime)get.SatisTablo.Tarih : (DateTime)get.TeknikTablo.Tarih,
                                     SekilID = Convert.ToInt32(get.SekilID),
                                     AlinanTutar = Convert.ToDecimal(get.AlinanTutar),
                                     ToplamTutar = Convert.ToDecimal(get.ToplamTutar),
                                     TeknikID = get.TeknikID == null ? 0 : Convert.ToInt16(get.TeknikID),
                                     SatisID = get.SatisID == null ? 0 : Convert.ToInt32(get.SatisID)

                                 };

                //Hepsi=========================================
                Belge[0, 0] = AnasayfaModels.Count().ToString();
                Belge[0, 1] = AnasayfaModels.Where(x => x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Count().ToString(); ;
                Belge[0, 2] = AnasayfaModels.Where(x => x.Tarih > DateTime.Now.AddDays(-7)).Count().ToString(); ;
                Belge[0, 3] = AnasayfaModels.Where(x => x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Count().ToString(); ;
                Belge[0, 4] = AnasayfaModels.Where(x => x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Count().ToString();

                Odeme[0, 0] = Belge[0, 1] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3 && x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Select(x => x.AlinanTutar).Sum());
                Odeme[0, 1] = Belge[0, 2] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3 && x.Tarih > DateTime.Now.AddDays(-7)).Select(x => x.AlinanTutar).Sum());
                Odeme[0, 2] = Belge[0, 3] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3 && x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Select(x => x.AlinanTutar).Sum());
                Odeme[0, 3] = Belge[0, 4] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3 && x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Select(x => x.AlinanTutar).Sum());
                Odeme[0, 4] = String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3).Select(x => x.AlinanTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.SekilID != 3).Select(x => x.AlinanTutar).Sum());
                Odeme[0, 5] = String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3).Select(x => x.ToplamTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.SekilID != 3).Select(x => x.ToplamTutar).Sum());
                Odeme[0, 6] = Odeme[0, 5] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID != 3).Select(x => x.ToplamTutar).Sum() - AnasayfaModels.Where(x => x.SekilID != 3).Select(x => x.AlinanTutar).Sum());

                //Teknik========================================
                Belge[1, 0] = AnasayfaModels.Count().ToString();
                Belge[1, 1] = AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Count().ToString(); ;
                Belge[1, 2] = AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih > DateTime.Now.AddDays(-7)).Count().ToString(); ;
                Belge[1, 3] = AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Count().ToString(); ;
                Belge[1, 4] = AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Count().ToString(); ;

                Odeme[1, 0] = Belge[1, 1] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Select(x => x.AlinanTutar).Sum());
                Odeme[1, 1] = Belge[1, 2] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih > DateTime.Now.AddDays(-7)).Select(x => x.AlinanTutar).Sum());
                Odeme[1, 2] = Belge[1, 3] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Select(x => x.AlinanTutar).Sum());
                Odeme[1, 3] = Belge[1, 4] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0 && x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Select(x => x.AlinanTutar).Sum());
                Odeme[1, 4] = String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0).Select(x => x.AlinanTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.SatisID == 0).Select(x => x.AlinanTutar).Sum());
                Odeme[1, 5] = String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0).Select(x => x.ToplamTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.SatisID == 0).Select(x => x.ToplamTutar).Sum());
                Odeme[1, 6] = Odeme[1, 5] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.SatisID == 0).Select(x => x.ToplamTutar).Sum() - AnasayfaModels.Where(x => x.SatisID == 0).Select(x => x.AlinanTutar).Sum());

                //Satış=========================================
                Belge[2, 0] = AnasayfaModels.Count().ToString();
                Belge[2, 1] = AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Count().ToString(); ;
                Belge[2, 2] = AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih > DateTime.Now.AddDays(-7)).Count().ToString(); ;
                Belge[2, 3] = AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Count().ToString(); ;
                Belge[2, 4] = AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Count().ToString(); ;

                Odeme[2, 0] = Belge[2, 1] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Select(x => x.AlinanTutar).Sum());
                Odeme[2, 1] = Belge[2, 2] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih > DateTime.Now.AddDays(-7)).Select(x => x.AlinanTutar).Sum());
                Odeme[2, 2] = Belge[2, 3] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Select(x => x.AlinanTutar).Sum());
                Odeme[2, 3] = Belge[2, 4] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3 && x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Select(x => x.AlinanTutar).Sum());
                Odeme[2, 4] = String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3).Select(x => x.AlinanTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3).Select(x => x.AlinanTutar).Sum());
                Odeme[2, 5] = String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3).Select(x => x.ToplamTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3).Select(x => x.ToplamTutar).Sum());
                Odeme[2, 6] = Odeme[2, 5] == "0" ? "0" : String.Format("{0:C}", AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3).Select(x => x.ToplamTutar).Sum() - AnasayfaModels.Where(x => x.TeknikID == 0 && x.SekilID != 3).Select(x => x.AlinanTutar).Sum());
                //Teklif========================================
                Belge[3, 0] = AnasayfaModels.Count().ToString();
                Belge[3, 1] = AnasayfaModels.Where(x => x.SekilID == 3 && x.Tarih >= DateTime.Today && x.Tarih <= DateTime.Today.AddDays(1).AddTicks(-1)).Count().ToString(); ;
                Belge[3, 2] = AnasayfaModels.Where(x => x.SekilID == 3 && x.Tarih > DateTime.Now.AddDays(-7)).Count().ToString(); ;
                Belge[3, 3] = AnasayfaModels.Where(x => x.SekilID == 3 && x.Tarih > DateTime.Now.AddDays(1 - DateTime.Now.Day)).Count().ToString(); ;
                Belge[3, 4] = AnasayfaModels.Where(x => x.SekilID == 3 && x.Tarih > DateTime.Now.AddYears(-1) && x.Tarih < DateTime.Now.AddYears(1)).Count().ToString(); ;

                Odeme[3, 0] = "0";
                Odeme[3, 1] = "0";
                Odeme[3, 2] = "0";
                Odeme[3, 3] = "0";
                Odeme[3, 4] = "0";
                Odeme[3, 5] = String.Format("{0:C}", AnasayfaModels.Where(x => x.SekilID == 3).Select(x => x.ToplamTutar).Count() == 0 ? 0 : AnasayfaModels.Where(x => x.SekilID == 3).Select(x => x.ToplamTutar).Sum());
                Odeme[3, 6] = "0";
                //===============================================


            }

            class AnasayfaModel
            {
                public int OdemeID { get; set; }
                public DateTime Tarih { get; set; }
                public int SekilID { get; set; }
                public decimal AlinanTutar { get; set; }
                public decimal ToplamTutar { get; set; }
                public int SatisID { get; set; }
                public int TeknikID { get; set; }
            }
        }
        public static Anasayfa Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Anasayfa();
                }
                return _instance;
            }
        }
        public Anasayfa()
        {
            InitializeComponent();
            Kur();
            BilgileriDoldur(0);

        }
        public void Kur()
        {
            string exchangeRate = "http://www.tcmb.gov.tr/kurlar/today.xml";
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(exchangeRate);

            USD = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod ='USD'] / BanknoteSelling").InnerXml;
            EUR = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod ='EUR'] / BanknoteSelling").InnerXml;
            lbdolar.Text = USD;
            lbeuro.Text = EUR;
        }
        private void btnKurGuncelle_Click(object sender, EventArgs e)
        {
            Kur();
        }
        private void cbCesit_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = cbCesit.SelectedIndex;
            if (index == 0)
            { lbOdemeBaslik.Text = "Ödeme Bilgileri (Bütün İşlemleri)"; }
            else if (index == 1)
            { lbOdemeBaslik.Text = "Ödeme Bilgileri (Teknik Servis İşlemleri)"; }
            else if (index == 2)
            { lbOdemeBaslik.Text = "Ödeme Bilgileri (Satış İşlemleri)"; }
            else
            { lbOdemeBaslik.Text = "Ödeme Bilgileri (Teklif İşlemleri)"; }

            BilgileriDoldur(index);



        }
        private void BilgileriDoldur(int index)
        {
            lbToplamBelge.Text = bilgiDoldur.Belge[index, 0];

            lbSaat.Text = bilgiDoldur.Belge[index, 1];
            lbHafta.Text = bilgiDoldur.Belge[index, 2];
            lbAy.Text = bilgiDoldur.Belge[index, 3];
            lbYil.Text = bilgiDoldur.Belge[index, 4];

            lbAlinan24.Text = bilgiDoldur.Odeme[index, 0];
            lbAlinanHafta.Text = bilgiDoldur.Odeme[index, 1];
            lbAlinanAy.Text = bilgiDoldur.Odeme[index, 2];
            lbAlinanYil.Text = bilgiDoldur.Odeme[index, 3];

            lbToplamAlinan.Text = bilgiDoldur.Odeme[index, 4];
            lbToplamTutar.Text = bilgiDoldur.Odeme[index, 5];
            lbToplamBorc.Text = bilgiDoldur.Odeme[index, 6];
        }

        private void btnOdenmemisBelge_Click(object sender, EventArgs e)
        {
            var result = Models.OdemeTablos.Where(x => x.OdemeDurumu == false && x.SekilID != 3).Select(x => x);
            
            if(result.Count()==0)
            {
                MyControl.Mesaj("Ödenmemiş belge bulunmamaktadır.", 0);
            }
            else
            {
                Odemeler.Instance.ShowRec(result);
                Odemeler.Instance.BringToFront();
            }
        }

        private void btnKurHesapla_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txPara.Text) || txPara.Text == "0")
            {
                MyControl.Mesaj("Çevirelecek tutarı doğru girdiğinizden emin olun.", 1);
            }
            else
            {
                decimal girilenTutar = Convert.ToDecimal(txPara.Text) / 10000;

                string kur;
                if (cbKurCesit.SelectedIndex == 0)
                    kur = String.Format("{0:N4}", (girilenTutar * Convert.ToDecimal(USD)));
                else if (cbKurCesit.SelectedIndex == 1)
                    kur = String.Format("{0:N4}", (girilenTutar * Convert.ToDecimal(EUR)));
                else if (cbKurCesit.SelectedIndex == 2)
                    kur = String.Format("{0:N4}", ((girilenTutar * 10000 * 10000) / Convert.ToDecimal(USD)));
                else
                    kur = String.Format("{0:N4}", ((girilenTutar * 10000 * 10000) / Convert.ToDecimal(EUR)));

                txDonusenPara.Text = String.Format("{0:N4}", kur);
            }
        }
    }
}
