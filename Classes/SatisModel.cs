using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalkocBilisim.DB;

namespace MalkocBilisim.Classes
{
    class SatisModel : Models
    {

        public static decimal KDV { get; set; }
        public static decimal TotPrice { get; set; }
        public static int Count { get; set; }
        public static bool KDVCheck { get; set; }
        public static List<SatisDetayTablo> ListofDetays { get; set; }
        public SatisModel()
        {
            ListofDetays = new List<SatisDetayTablo>();
        }
        public static void KDVCheckCalculate(int id, bool check)
        {
            decimal _totalPrice = TotalPrice;

            if (check == true)
            {
                KDV = _totalPrice - (_totalPrice / 1.18M);
                TotPrice = TotalPrice / (1.18M);
            }
            else
            {
                TotPrice = _totalPrice / 1.18M;
                KDV = _totalPrice - TotPrice;
            }
        }
        private static void GetSatisTabloDetails(int id)
        {
            OdemeTablo record = GetOdemeRecordSatis(id); //Odeme bilgileri
            if (record != null)
            {
                KDVCheck = Convert.ToBoolean(record.SatisTablo.SatisDurumu);
                GetCustomerDetails(Convert.ToInt32(record.SatisTablo.MusteriID)); // Musteri Bilgileri
                KDVCheckCalculate(id, Convert.ToBoolean(record.SatisTablo.SatisDurumu)); // KDV hesaplama durumu
            }
        }
        public static SatisTablo GetSatisRecord(int id)
        {
            GetSatisTabloDetails(id);

            return SatisTablos.FirstOrDefault(x => x.SatisID == id);
        }
        public static IQueryable<SatisTablo> GetListofAllRecords()
        {
            return SatisTablos;
        }
        public static IQueryable<SatisTablo> GetListonlySatis()
        {
            return SatisTablos.Where(x => x.SatisDurumu == true).Select(x => x);
        }
        public static IQueryable<SatisTablo> GetListonlyTeklif()
        {
            return SatisTablos.Where(x => x.SatisDurumu == false).Select(x => x);
        }
        public static IQueryable<SatisDetayTablo> GetSatisDetail(int id)
        {
            var records = SatisDetayTablos.Where(x => x.SatisID == id).Select(x => x);
            Count = records.Count();

            return records;
        }
        public static IQueryable<SatisTablo> SearchbyCustNameSatis(string search, int status)
        {
            if (status == 0)
                return SatisTablos.Where(x => x.MusteriTablo.MusteriIsim.Contains(search)).Select(x => x);
            else
                return SatisTablos.Where(x => x.MusteriTablo.MusteriIsim.Contains(search) && x.SatisDurumu == (status == 1 ? true : false)).Select(x => x);
        }
        public static IQueryable<SatisTablo> SearchbyProductSatis(string search, int status)
        {
            if (status == 0)
            {
                return from getir in SatisTablos
                       join getirIcerik in SatisDetayTablos
                       on getir.SatisID equals getirIcerik.SatisID
                       where getirIcerik.UrunAdi.Contains(search)
                       orderby getir.SatisID descending
                       select getir;
            }
            else
            {
                return from getir in SatisTablos
                       join getirIcerik in SatisDetayTablos
                       on getir.SatisID equals getirIcerik.SatisID
                       where getirIcerik.UrunAdi.Contains(search) && getir.SatisDurumu == (status == 1 ? true : false)
                       orderby getir.SatisID descending
                       select getir;
            }

        }
        public static IQueryable<SatisTablo> SearchbySatisID(int search)
        {
            return SatisTablos.Where(x => x.SatisID == search).Select(x => x);
        }
        public static IQueryable<SatisTablo> SearchDate(DateTime datetime, int status)
        {
            if (status == 0)
                return SatisTablos.Where(x => x.Tarih > datetime).Select(x => x);
            else
                return SatisTablos.Where(x => x.Tarih > datetime && x.SatisDurumu == (status == 1 ? true : false)).Select(x => x);
        }
        public static IQueryable<SatisTablo> SearchDateRangeDay(DateTime start, DateTime end, int status)
        {
            if (status == 0)
                return SatisTablos.Where(x => x.Tarih >= start && x.Tarih <= end).Select(x => x);
            else
                return SatisTablos.Where(x => x.Tarih >= start && x.Tarih <= end && x.SatisDurumu == (status == 1 ? true : false)).Select(x => x);
        }
        public static IQueryable<SatisTablo> SearchDateRangeYear(DateTime start, DateTime end, int status)
        {
            if (status == 0)
                return SatisTablos.Where(x => x.Tarih > start && x.Tarih < end).Select(x => x);
            else
                return SatisTablos.Where(x => x.Tarih > start && x.Tarih < end && x.SatisDurumu == (status == 1 ? true : false)).Select(x => x);
        }
        public static IQueryable<SatisTablo> DeleteRecord(int id, bool all)
        {
            SatisTablo satisTablo = dc.SatisTablos.FirstOrDefault(x => x.SatisID == id);
            var satisDetays = dc.SatisDetayTablos.Where(x => x.SatisID == id).Select(x => x);
            satisTablo.Sil = false;

            foreach (var item in satisDetays)
                item.Sil = false;

            if (all == true)
            {
                OdemeTablo recordOdeme = dc.OdemeTablos.FirstOrDefault(x => x.SatisID == id);
                var recordsOdemeDetail = dc.OdemeDetayTablos.Where(x => x.OdemeID == recordOdeme.OdemeID).Select(x => x);
                recordOdeme.Sil = false;

                foreach (var item in recordsOdemeDetail)
                    item.Sil = false;
            }

            dc.SubmitChanges();
            UpdateSatisTables();

            return SatisTablos;

        }
        public static int getLastSatisID()
        {
            return (from get in dc.SatisTablos orderby get.SatisID descending select get.SatisID).First();
        }
        public static string InsertRecord(string CustName, string subject, DateTime date, bool currency, bool saleorOffer, bool paymentChoise, bool KDV, decimal takenPrice, decimal totalPrice)
        {
            MyControl customerCheck = new MyControl();
            int customerID = customerCheck.MusteriKontrol(CustName, null, null);

            SatisTablo newRecord = new SatisTablo()
            {
                MusteriID = customerID,
                Tarih = date,
                Konu = subject.ToUpper(),
                SatisDurumu = saleorOffer,
                KDV = KDV,
                Sil = true
            };
            dc.SatisTablos.InsertOnSubmit(newRecord);
            dc.SubmitChanges();

            foreach (var item in ListofDetays)
                item.SatisID = getLastSatisID();

            dc.SatisDetayTablos.InsertAllOnSubmit<SatisDetayTablo>(ListofDetays);
            dc.SubmitChanges();

            OdemeTablo odemeRecord = new OdemeTablo()
            {
                AlinanTutar = takenPrice < 0 ? takenPrice * -1 : takenPrice,
                ToplamTutar = totalPrice,
                OdemeDurumu = takenPrice == totalPrice ? true : false,
                KurID = currency == true ? 1 : 2, //1 TL, 2 Dollar
                SekilID = saleorOffer == false ? 3 : (paymentChoise == true ? 1 : 2), //1 Cash, 2 Credit Card
                TeknikID = null,
                SatisID = getLastSatisID(),
                MusteriID = customerID,
                Sil = true
            };
            dc.OdemeTablos.InsertOnSubmit(odemeRecord);
            dc.SubmitChanges();

            if (saleorOffer == true) //teklifse odeme detayına girmesin.
            {
                OdemeDetayTablo odemeDetayRecord = new OdemeDetayTablo()
                {
                    OdemeID = getLastOdemeID(),
                    Tutar = takenPrice,
                    Tarih = date,
                    Sil = true
                };
                dc.OdemeDetayTablos.InsertOnSubmit(odemeDetayRecord);
                dc.SubmitChanges();
            }


            UpdateSatisTables();
            ListofDetays.Clear();

            CreatePDF createPDF = new CreatePDF(getLastSatisID());
            createPDF.CreateSatisPDF();

            return saleorOffer == true ? "Satış Forum sisteme kaydedilmiştir." : "Teklif Formu sisteme kaydedilmiştir.";
        }

    }
}
