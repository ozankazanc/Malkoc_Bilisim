using DevExpress.XtraEditors;
using MalkocBilisim.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MalkocBilisim.Classes
{
    class MyControl
    {
        MKocdbEntityDataContext dc = new MKocdbEntityDataContext();

        public int MusteriKontrol(string musteriIsmi, string telefon, string Adres)
        {
            var sonuc = dc.MusteriTablos.FirstOrDefault(x => x.MusteriIsim == musteriIsmi);

            if (sonuc == null)
            {
                MusteriTablo musteri = new MusteriTablo
                {
                    MusteriIsim = musteriIsmi.ToUpper(),
                    MusteriTelefon = telefon,
                    MusteriAdres = Adres.ToUpper(),
                    MusteriNumara = null,
                    KayitTarihi = DateTime.Now,
                    Sil = true
                };
                dc.MusteriTablos.InsertOnSubmit(musteri);
                dc.SubmitChanges();

                Mesaj("[" + musteriIsmi + "] isimli müşteri sisteme kaydedilmiştir.", 0);
            }
            var returnMusteri = dc.MusteriTablos.FirstOrDefault(x => x.MusteriIsim == musteriIsmi);

            return returnMusteri.MusteriID;
        }
        public static void Mesaj(string mesaj, byte deger)
        {
            if (deger == 0)
                XtraMessageBox.Show(mesaj, "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                XtraMessageBox.Show(mesaj, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }
}
