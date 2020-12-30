using MalkocBilisim.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalkocBilisim.Classes
{
    class Models
    {

        protected static MKocdbEntityDataContext dc;
        public static IQueryable<MusteriTablo> MusteriTablos { get; set; }
        public static IQueryable<TeknikTablo> TeknikTablos { get; set; }
        public static IQueryable<ArizaTablo> ArizaTablos { get; set; }
        public static IQueryable<ParcaTablo> ParcaTablos { get; set; }
        public static IQueryable<IslemTablo> IslemTablos { get; set; }
        public static IQueryable<OdemeTablo> OdemeTablos { get; set; }
        public static IQueryable<OdemeDetayTablo> OdemeDetayTablos { get; set; }
        public static IQueryable<SatisTablo> SatisTablos { get; set; }
        public static IQueryable<SatisDetayTablo> SatisDetayTablos { get; set; }
        public static IQueryable<KurTablo> KurTablos { get; set; }
        public static IQueryable<OdemeSekliTablo> OdemeSekliTablos { get; set; }
        public static IQueryable<Guncelleme> GuncelleTablos { get; set; }
        public static IQueryable<GuncellemeDetayTablo> GuncelleDetayTablos { get; set; }
        public static int TeknikID { get; set; }
        public static int SatisID { get; set; }
        public static string CustName { get; set; }
        public static string CustAddress { get; set; }
        public static string CustPhoneNumber { get; set; }
        public static decimal TotalPrice { get; set; }
        public static decimal TakenPrice { get; set; }
        public static decimal Debt { get; set; }
        public Models()
        {
            UpdateDC();
            UpdateAllTables();
        }
        protected static void UpdateDC()
        {
            dc = new MKocdbEntityDataContext();
        }
        protected static void UpdateAllTables()
        {
            MusteriTablos = from getir in dc.MusteriTablos orderby getir.MusteriID descending where getir.Sil == true select getir;
            TeknikTablos = from getir in dc.TeknikTablos orderby getir.TeknikID descending where getir.Sil == true select getir;
            ArizaTablos = from getir in dc.ArizaTablos where getir.Sil == true select getir;
            ParcaTablos = from getir in dc.ParcaTablos where getir.Sil == true select getir;
            IslemTablos = from getir in dc.IslemTablos where getir.Sil == true select getir;
            OdemeTablos = from getir in dc.OdemeTablos orderby getir.OdemeID descending where getir.Sil == true select getir;
            OdemeDetayTablos = from getir in dc.OdemeDetayTablos orderby getir.OdemeDetayID descending where getir.Sil == true select getir;
            SatisTablos = from getir in dc.SatisTablos orderby getir.SatisID descending where getir.Sil == true select getir;
            SatisDetayTablos = from getir in dc.SatisDetayTablos where getir.Sil == true select getir;
            KurTablos = from getir in dc.KurTablos select getir;
            OdemeSekliTablos = from getir in dc.OdemeSekliTablos select getir;
        }
        public static void UpdateTeknikTables()
        {
            // UpdateDC();
            TeknikTablos = from getir in dc.TeknikTablos orderby getir.TeknikID descending where getir.Sil == true select getir;
            ArizaTablos = from getir in dc.ArizaTablos where getir.Sil == true select getir;
            ParcaTablos = from getir in dc.ParcaTablos where getir.Sil == true select getir;
            IslemTablos = from getir in dc.IslemTablos where getir.Sil == true select getir;
            UpdateCustomerTables();
            UpdateOdemeTables();
        }
        public static void UpdateSatisTables()
        {
            //UpdateDC();
            SatisTablos = from getir in dc.SatisTablos orderby getir.MusteriID descending where getir.Sil == true select getir;
            SatisDetayTablos = from getir in dc.SatisDetayTablos where getir.Sil == true select getir;
            UpdateCustomerTables();
            UpdateOdemeTables();
        }
        public static void UpdateCustomerTables()
        {
            // UpdateDC();
            MusteriTablos = from getir in dc.MusteriTablos orderby getir.MusteriID descending where getir.Sil == true select getir;
        }
        public static void UpdateOdemeTables()
        {
            //UpdateDC();
            OdemeTablos = from getir in dc.OdemeTablos orderby getir.OdemeID descending where getir.Sil == true select getir;
            OdemeDetayTablos = from getir in dc.OdemeDetayTablos orderby getir.OdemeDetayID descending where getir.Sil == true select getir;
        }
        public SatisTablo GetSatisKayit(int id)
        {
            return SatisTablos.FirstOrDefault(x => x.SatisID == id);
        }
        public static MusteriTablo GetMusteriRecord(int MusteriID)
        {
            return MusteriTablos.FirstOrDefault(x => x.MusteriID == MusteriID);
        }
        public static OdemeTablo GetOdemeRecordTeknik(int teknikID)
        {
            try
            {
                var record = OdemeTablos.FirstOrDefault(x => x.TeknikID == teknikID);
                if (record != null)
                {
                    TotalPrice = Convert.ToDecimal(record.ToplamTutar);
                    TakenPrice = Convert.ToDecimal(record.AlinanTutar);
                    Debt = TotalPrice - TakenPrice;
                }

                return record;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
        public static OdemeTablo GetOdemeRecordSatis(int SatisID)
        {
            try
            {
                var record = OdemeTablos.FirstOrDefault(x => x.SatisID == SatisID);
                if (record != null)
                {
                    TotalPrice = Convert.ToDecimal(record.ToplamTutar);
                    TakenPrice = Convert.ToDecimal(record.AlinanTutar);
                    Debt = TotalPrice - TakenPrice;
                }
                return record;
            }
            catch (ArgumentNullException)
            {
                return null;
            }

        }
        protected static MusteriTablo GetCustomerDetails(int id)
        {
            MusteriTablo customer = MusteriTablos.FirstOrDefault(x => x.MusteriID == id);
            CustName = customer.MusteriIsim;
            CustAddress = customer.MusteriAdres;
            CustPhoneNumber = customer.MusteriTelefon;

            return customer;
        }
        public static IQueryable<TeknikTablo> GetTeknikRecordsOdemeID(int odemeID)
        {
            TeknikID = Convert.ToInt32(OdemeTablos.FirstOrDefault(x => x.OdemeID == odemeID).TeknikID);
            return TeknikTablos.Where(x => x.TeknikID == TeknikID).Select(x => x);
        }
        public static IQueryable<SatisTablo> GetSatisRecordsOdemeID(int odemeID)
        {
            SatisID = Convert.ToInt32(OdemeTablos.FirstOrDefault(x => x.OdemeID == odemeID).SatisID);
            return SatisTablos.Where(x => x.SatisID == SatisID).Select(x => x);
        }
        public static int getLastOdemeID()
        {
            return (from get in dc.OdemeTablos orderby get.OdemeID descending select get.OdemeID).First();
        }
        public static IQueryable<MusteriTablo> SearchbyCustName(string search_Musteri)
        {
            return MusteriTablos.Where(x => x.MusteriIsim.Contains(search_Musteri)).Select(x => x);
        }
        public static IQueryable<MusteriTablo> SearchbyCustPhone(string search_Musteri)
        {
            return MusteriTablos.Where(x => x.MusteriTelefon.Contains(search_Musteri)).Select(x => x);
        }
        public static IQueryable<MusteriTablo> SearchbyCustAddress(string search_Musteri)
        {
            return MusteriTablos.Where(x => x.MusteriAdres.Contains(search_Musteri)).Select(x => x);
        }
        public static bool InsertCustomer(string CustName, string CustPhone, string CustAddress)
        {
            MusteriTablo customer = MusteriTablos.FirstOrDefault(x => x.MusteriIsim == CustName && x.MusteriTelefon == CustPhone);

            if (customer == null)
            {
                MusteriTablo record = new MusteriTablo
                {
                    MusteriIsim = CustName.ToUpper(),
                    MusteriTelefon = CustPhone,
                    MusteriAdres = CustAddress.ToUpper(),
                    KayitTarihi = DateTime.Now,
                    Sil = true
                };
                dc.MusteriTablos.InsertOnSubmit(record);
                dc.SubmitChanges();
                UpdateCustomerTables();

                return true;
            }
            else
            {
                return false;
            }
        }
        public static string UpdateCustomer(string CustName, string CustPhone, string CustAddress, int MusteriID)
        {
            MusteriTablo record = MusteriTablos.FirstOrDefault(x => x.MusteriID == MusteriID);
            record.MusteriIsim = CustName.ToUpper();
            record.MusteriTelefon = CustPhone;
            record.MusteriAdres = CustAddress.ToUpper();
            dc.SubmitChanges();
            UpdateCustomerTables();

            return "[" + CustName + "] isimli müşterinin bilgileri güncellenmiştir.";
        }
        public static IQueryable<MusteriTablo> DeleteCustomer(int MusteriID)
        {
            MusteriTablo customer = dc.MusteriTablos.FirstOrDefault(x => x.MusteriID == MusteriID);
            customer.Sil = false;
            dc.SubmitChanges();

            var odemeResult = from get in dc.OdemeTablos where get.MusteriID == MusteriID select get;
            var odemeDetayResult = from get in dc.OdemeDetayTablos where get.OdemeTablo.MusteriID == MusteriID select get;

            foreach (var item in odemeDetayResult)
                item.Sil = false;
            dc.SubmitChanges();

            foreach (var item in odemeResult)
            {
                item.Sil = false;
                dc.SubmitChanges();

                if (item.TeknikID != null) //Var ise Teknik Servis Formu ve detayları silme
                {
                    TeknikTablo teknikRecord = dc.TeknikTablos.FirstOrDefault(x => x.TeknikID == item.TeknikID);
                    teknikRecord.Sil = false;
                    var resultAriza = from get in dc.ArizaTablos where get.TeknikID == item.TeknikID select get;
                    var resultIslem = from get in dc.IslemTablos where get.TeknikID == item.TeknikID select get;
                    var resultParca = from get in dc.ParcaTablos where get.TeknikID == item.TeknikID select get;
                    foreach (var ariza in resultAriza)
                        ariza.Sil = false;
                    foreach (var islem in resultIslem)
                        islem.Sil = false;
                    foreach (var parca in resultParca)
                        parca.Sil = false;
                }
                else //Var ise Satis Formu ve detayları silme
                {
                    SatisTablo satisRecord = dc.SatisTablos.FirstOrDefault(x => x.SatisID == item.SatisID);
                    satisRecord.Sil = false;
                    var resultSatisDetay = from get in dc.SatisDetayTablos where get.SatisID == item.SatisID select get;
                    foreach (var satisDetay in resultSatisDetay)
                    {
                        satisDetay.Sil = false;
                    }
                }
                dc.SubmitChanges();
            }

            UpdateAllTables();
            return MusteriTablos;
        }
        public static OdemeTablo GetOdemeRecord(int OdemeID)
        {
            return OdemeTablos.FirstOrDefault(x => x.OdemeID == OdemeID);
        }
        public static IQueryable<OdemeDetayTablo> GetOdemeDetayRecords(int OdemeID)
        {
            return OdemeDetayTablos.Where(x => x.OdemeID == OdemeID).Select(x => x);
        }
        public static List<string> GetOdemeDetayRecordsList(int OdemeID)
        {
            List<string> values = new List<string>();
            var result = OdemeDetayTablos.Where(x => x.OdemeID == OdemeID).Select(x => x);

            foreach (var item in result)
            {
                if (item.Tutar < 0)
                {
                    values.Add(item.Tarih + " - " + String.Format("{0:C}", item.Tutar * -1) + " [-]");
                }
                else
                {
                    values.Add(item.Tarih + " - " + String.Format("{0:C}", item.Tutar) + " [+]");
                }
            }

            return values;
        }
        public static void PlusorMinus(decimal value, int OdemeID)
        {
            OdemeTablo record = GetOdemeRecord(OdemeID);

            record.OdemeDurumu = (record.AlinanTutar + value) == record.ToplamTutar ? true : false;
            record.AlinanTutar += value;
            dc.SubmitChanges();

            InsertOdemeDetayRecord(Convert.ToDecimal(value), OdemeID);
            UpdateOdemeTables();
        }
        public static void BorcuKapat(int OdemeID)
        {
            OdemeTablo record = OdemeTablos.FirstOrDefault(x => x.OdemeID == OdemeID);
            InsertOdemeDetayRecord(Convert.ToDecimal(record.ToplamTutar - record.AlinanTutar), OdemeID);

            record.AlinanTutar = record.ToplamTutar;
            record.OdemeDurumu = true;
            dc.SubmitChanges();

            UpdateOdemeTables();
        }
        public static void InsertOdemeDetayRecord(decimal price, int OdemeID)
        {
            OdemeDetayTablo recordDetay = new OdemeDetayTablo
            {
                OdemeID = OdemeID,
                Tutar = price,
                Tarih = DateTime.Now,
                Sil = true
            };
            dc.OdemeDetayTablos.InsertOnSubmit(recordDetay);
            dc.SubmitChanges();
        }
        public static void TekliftoSatis(decimal price, int OdemeID, int OdemeSekli)
        {
            OdemeTablo record = GetOdemeRecord(OdemeID);
            record.SatisTablo.SatisDurumu = true;
            record.AlinanTutar = price;
            record.SekilID = OdemeSekli;
            record.OdemeDurumu = price == record.ToplamTutar ? true : false;
            dc.SubmitChanges();

            InsertOdemeDetayRecord(price, OdemeID);

            UpdateSatisTables();
            UpdateOdemeTables();
        }
        public static bool CheckUpdate()
        {
            try //son versiyon numarası kontrol ediliyor ve check false ise kullanıcıya yardım formunda yeni güncellemeler gösteriliyor.
            {
                var result = (from get in dc.Guncellemes
                              orderby get.GuncellemeID descending //ilk kayıtta null dönüyor.
                              select get).First();

                if (result.Checked == false)
                {
                    GuncelleDetayTablos = dc.GuncellemeDetayTablos.Where(x => x.GuncellemeID == result.GuncellemeID);
                    return false;
                }
                else
                { 
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
            
         }
    }
}
