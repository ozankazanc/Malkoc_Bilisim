using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalkocBilisim.DB;

namespace MalkocBilisim.Classes
{
    class TeknikModel:Models
    {
        public static string ArizaDetails { get; set; }
        public static string IslemDetails { get; set; }
        public static string ParcaDetails { get; set; }
        public static List<ArizaTablo> ListArizaDetails { get; set; }
        public static List<IslemTablo> ListIslemDetails { get; set; }
        public static List<ParcaTablo> ListParcaDetails { get; set; }
        public TeknikModel()
        {
            ListArizaDetails = new List<ArizaTablo>();
            ListIslemDetails = new List<IslemTablo>();
            ListParcaDetails = new List<ParcaTablo>();
        }
        private static void GetTeknikDetails(int id)
        {
            OdemeTablo record = GetOdemeRecordTeknik(id);
            if (record != null)
            {
                GetCustomerDetails(Convert.ToInt32(record.TeknikTablo.MusteriID));
            }
        }
        public static IQueryable<TeknikTablo> GetListofTeknikRecords()
        {
            return TeknikTablos;
        }
        public static TeknikTablo GetTeknikRecord(int id)
        {

            GetArizaDetail(id);
            GetIslemDetail(id);
            GetParcaDetail(id);
            GetTeknikDetails(id);
            return TeknikTablos.FirstOrDefault(x => x.TeknikID == id);
        }
        public static void GetArizaDetail(int id)
        {
            string arizaBilgisi = "";
            try
            {
                var result = ArizaTablos.Where(x => x.TeknikID == id).Select(x => x.ArizaBilgisi);
                foreach (var item in result)
                {
                    arizaBilgisi += "[" + item + "] ";
                }

                ArizaDetails = arizaBilgisi;
            }
            catch (ArgumentNullException)
            {
                ArizaDetails = "[Girilmemiş]";
            }
        }
        public static void GetIslemDetail(int id)
        {
            string islemBilgisi = "";
            try
            {
                var result = IslemTablos.Where(x => x.TeknikID == id).Select(x => x.YapilanIslem);
                foreach (var item in result)
                {
                    islemBilgisi += "[" + item + "] ";
                }
                IslemDetails = islemBilgisi;

            }
            catch (ArgumentNullException)
            {
                IslemDetails = "[Girilmemiş]";
            }
        }
        public static void GetParcaDetail(int id)
        {
            string parcaBilgisi = "";
            try
            {
                var result = ParcaTablos.Where(x => x.TeknikID == id).Select(x => x.DegisenParca);
                foreach (var item in result)
                {
                    parcaBilgisi += "[" + item + "] ";
                }
                ParcaDetails = parcaBilgisi;

            }
            catch (ArgumentNullException)
            {
                ParcaDetails = "[Girilmemiş]";
            }
        }
        public static IQueryable<TeknikTablo> SearchbyName(string search)
        {
            return TeknikTablos.Where(x => x.MusteriTablo.MusteriIsim.Contains(search)).Select(x => x);
        }
        public static IQueryable<TeknikTablo> SearchbyPhone(string search)
        {
            return TeknikTablos.Where(x => x.MusteriTablo.MusteriIsim.Contains(search)).Select(x => x);
        }
        public static IQueryable<TeknikTablo> SearchbyAriza(string search)
        {
            var result = from teknik in TeknikTablos
                         join ariza in ArizaTablos on teknik.TeknikID equals ariza.TeknikID
                         where ariza.ArizaBilgisi.Contains(search)
                         select teknik;
            return result;
        }
        public static IQueryable<TeknikTablo> SearchbyIslem(string search)
        {
            var result = from teknik in TeknikTablos
                         join islem in IslemTablos on teknik.TeknikID equals islem.TeknikID
                         where islem.YapilanIslem.Contains(search)
                         select teknik;
            return result;
        }
        public static IQueryable<TeknikTablo> SearchbyParca(string search)
        {
            var result = from teknik in TeknikTablos
                         join parca in ParcaTablos on teknik.TeknikID equals parca.TeknikID
                         where parca.DegisenParca.Contains(search)
                         select teknik;
            return result;
        }
        public static IQueryable<TeknikTablo> SearchbyTeknikID(int search)
        {
            return TeknikTablos.Where(x => x.TeknikID == search).Select(x => x);
        }
        public static IQueryable<TeknikTablo> SearchDate(DateTime datetime)
        {
            return TeknikTablos.Where(x => x.Tarih > datetime).Select(x => x);
        }
        public static IQueryable<TeknikTablo> SearchDateRange(DateTime start, DateTime end, bool dayoryear)
        {
            if (dayoryear == true)
                return TeknikTablos.Where(x => x.Tarih >= start && x.Tarih <= end).Select(x => x); //Gün
            else
                return TeknikTablos.Where(x => x.Tarih > start && x.Tarih < end).Select(x => x); //Yıl
        }
        public static IQueryable<TeknikTablo> DeleteRecord(int id, bool all)
        {
            TeknikTablo recordTeknik = dc.TeknikTablos.FirstOrDefault(x => x.TeknikID == id);
            var recordsAriza = dc.ArizaTablos.Where(x => x.TeknikID == id).Select(x => x);
            var recordsIslem = dc.IslemTablos.Where(x => x.TeknikID == id).Select(x => x);
            var recordsParca = dc.ParcaTablos.Where(x => x.TeknikID == id).Select(x => x);
            recordTeknik.Sil = false;

            foreach (var item in recordsAriza)
                item.Sil = false;
            foreach (var item in recordsIslem)
                item.Sil = false;
            foreach (var item in recordsParca)
                item.Sil = false;

            if (all == true) //with delete OdemeRecord of TeknikRecord
            {
                OdemeTablo recordOdeme = dc.OdemeTablos.FirstOrDefault(x => x.TeknikID == id);
                var recordsOdemeDetail = dc.OdemeDetayTablos.Where(x => x.OdemeID == recordOdeme.OdemeID).Select(x => x);

                recordOdeme.Sil = false;

                foreach (var item in recordsOdemeDetail)
                    item.Sil = false;
            }
            dc.SubmitChanges();
            UpdateTeknikTables();

            return TeknikTablos;
        }
        public static int getLastTeknikID()
        {
            return (from get in dc.TeknikTablos orderby get.TeknikID descending select get.TeknikID).First();
        }
        public static string InsertRecord(string CustName, string CustPhone, string custAddress, DateTime date, string note, decimal totalPrice, decimal takenPrice, int paymentChoise)
        {
            MyControl customerCheck = new MyControl();
            int CustomerID = customerCheck.MusteriKontrol(CustName, CustPhone, custAddress);

            TeknikTablo newRecord = new TeknikTablo()
            {
                MusteriID = CustomerID,
                Tarih = date,
                Not = note.ToUpper(),
                Sil = true
            };
            dc.TeknikTablos.InsertOnSubmit(newRecord);
            dc.SubmitChanges();

            foreach (var item in ListArizaDetails)
                item.TeknikID = getLastTeknikID();
            foreach (var item in ListIslemDetails)
                item.TeknikID = getLastTeknikID();
            foreach (var item in ListParcaDetails)
                item.TeknikID = getLastTeknikID();

            dc.ArizaTablos.InsertAllOnSubmit<ArizaTablo>(ListArizaDetails);
            dc.IslemTablos.InsertAllOnSubmit<IslemTablo>(ListIslemDetails);
            dc.ParcaTablos.InsertAllOnSubmit<ParcaTablo>(ListParcaDetails);
            dc.SubmitChanges();


            OdemeTablo odemeRecord = new OdemeTablo
            {
                AlinanTutar = takenPrice,
                ToplamTutar = totalPrice,
                OdemeDurumu = takenPrice == totalPrice ? true : false,
                KurID = 1,
                SekilID = paymentChoise,
                SatisID = null,
                TeknikID = getLastTeknikID(),
                MusteriID = CustomerID,
                Sil = true
            };
            dc.OdemeTablos.InsertOnSubmit(odemeRecord);
            dc.SubmitChanges();


            OdemeDetayTablo odemeDetayRecord = new OdemeDetayTablo()
            {
                OdemeID = getLastOdemeID(),
                Tutar = takenPrice,
                Tarih = date,
                Sil = true
            };
            dc.OdemeDetayTablos.InsertOnSubmit(odemeDetayRecord);
            dc.SubmitChanges();

            UpdateTeknikTables();
            ListArizaDetails.Clear();
            ListIslemDetails.Clear();
            ListParcaDetails.Clear();

            CreatePDF createPdf = new CreatePDF(getLastTeknikID());
            createPdf.CreateTeknikPDF();

            return "Teknik Servis Formu sisteme kaydedilmiştir.";
        }


    }
}
