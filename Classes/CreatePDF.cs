using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MalkocBilisim.DB;

namespace MalkocBilisim.Classes
{
    class CreatePDF
    {
        private int ID { get; set; }
        private string MusteriAdi { get; set; }
        private string Adres { get; set; }
        private string Telefon { get; set; }
        private string Tarih { get; set; }
        private string Konu { get; set; }
        private decimal OnarimUcreti { get; set; }
        private decimal Tutar { get; set; }
        private decimal KDV { get; set; }
        private decimal ToplamTutar { get; set; }
        private bool satisorTeklif { get; set; }
        private bool KDVCheck { get; set; }
        private string KUR { get; set; }
        private string SavePath { get; set; }

        public CreatePDF()
        {

        }
        public CreatePDF(int _ID)
        {
            ID = _ID;
        }
        public CreatePDF(int _ID, string Path)
        {
            ID = _ID;
            SavePath = Path;
        }
        private void CreatePath()
        {
            Directory.CreateDirectory("C:\\KazancYazilim\\MalkocBilisim\\PDF");
            Directory.CreateDirectory("C:\\KazancYazilim\\MalkocBilisim\\PDF\\TeknikServisFormu");
            Directory.CreateDirectory("C:\\KazancYazilim\\MalkocBilisim\\PDF\\Satis_Teklif");
        }
        public void CreateTeknikPDF()
        {
            TeknikTablo recordTeknik = TeknikModel.GetTeknikRecord(ID);
            OdemeTablo recordOdeme = Models.GetOdemeRecordTeknik(ID);
            MusteriAdi = recordTeknik.MusteriTablo.MusteriIsim;
            Adres = recordTeknik.MusteriTablo.MusteriAdres == null ? " " : recordTeknik.MusteriTablo.MusteriAdres;
            Telefon = recordTeknik.MusteriTablo.MusteriTelefon == null ? " " : recordTeknik.MusteriTablo.MusteriTelefon;
            Tarih = ((DateTime)recordTeknik.Tarih).ToString(("dd/MM/yyyy"));
            OnarimUcreti = Convert.ToDecimal(recordOdeme.ToplamTutar);

            TeknikModel.GetArizaDetail(ID); string Ariza = TeknikModel.ArizaDetails;
            TeknikModel.GetIslemDetail(ID); string Islem = TeknikModel.IslemDetails;
            TeknikModel.GetParcaDetail(ID); string Parca = TeknikModel.ParcaDetails;

            Document doc = new Document(PageSize.A5.Rotate());

            /// BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED); //mobil tarafta türkçe karakter göstermiyor.
            BaseFont myFont = BaseFont.CreateFont(@"C:\windows\fonts\arial.ttf", "windows-1254", BaseFont.EMBEDDED); //sorun bu şekilde düzeltildi.

            Font fontNormal = new Font(myFont, 12, Font.NORMAL);
            Font fontBold = new Font(myFont, 12, Font.BOLD);
            Font fontIcerik = new Font(myFont, 10, Font.NORMAL);
            Font fontSon = new Font(myFont, 11, Font.BOLD);

            BaseFont arial = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font f_15_bold = new Font(arial, 15, Font.BOLD);
            Font f_12_normal = new Font(arial, 13, Font.NORMAL);

            FileStream os;
            
            if(SavePath==null)
                SavePath = "C://KazancYazilim//MalkocBilisim//PDF//TeknikServisFormu//" + ID.ToString() + "TSF.pdf";
            try
            {
                os = new FileStream(@SavePath, FileMode.Create);
            }
            catch
            {
                CreatePath();
                 os = new FileStream(@SavePath, FileMode.Create);
            }
            
            using (os)
            {
                PdfWriter.GetInstance(doc, os);
                doc.Open();

                //---------------------------------------------------------------------------------
                //LOGO, BAŞLIK,TARİH
                //---------------------------------------------------------------------------------
                PdfPTable table1 = new PdfPTable(3);

                iTextSharp.text.Image myImage;
                try
                {
                    myImage = iTextSharp.text.Image.GetInstance("http://www.kazancyazilim.com/img/LogoForm.png");
                }
                catch(Exception)
                {
                    myImage = null;
                }
                float[] width = new float[] { 30f, 50f, 30f }; //"C:/KazancYazilim/MalkocBilisim/LogoForm.png"
                PdfPCell cell = new PdfPCell(myImage);
                PdfPCell cell2 = new PdfPCell(new Phrase("TEKNIK SERVIS FORMU", f_15_bold));
                PdfPCell cell3 = new PdfPCell(new Phrase("Tarih : " + Tarih, f_12_normal));
                myImage.ScaleAbsolute(140f, 45f);

                //LOGO
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = 50;
                table1.AddCell(cell);
                //Teknik Servis Formu
                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.FixedHeight = 50;
                table1.AddCell(cell2);
                //tarih
                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                cell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell3.FixedHeight = 50;

                table1.AddCell(cell3);

                table1.WidthPercentage = 110;
                table1.SetWidths(width);
                // table1.SpacingBefore = 50;
                doc.Add(table1);

                //---------------------------------------------------------------------------------
                //MÜŞTERİ İSİM, SİPARİŞ NO
                //---------------------------------------------------------------------------------

                table1 = new PdfPTable(4); //Müşteri Adı, sipariş NO
                cell = new PdfPCell(new Phrase("MÜŞTERİ ADI", fontNormal));
                cell2 = new PdfPCell(new Phrase(MusteriAdi, fontNormal));
                cell3 = new PdfPCell(new Phrase("Belge No", fontNormal));
                PdfPCell cell4 = new PdfPCell(new Phrase(ID.ToString(), fontNormal));

                width = new float[] { 30f, 50f, 15f, 15f };

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell2);

                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                cell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell3.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table1.AddCell(cell3);

                cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                cell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell4);

                table1.WidthPercentage = 110;
                table1.SetWidths(width);
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //ADDRESS
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(2); // Adres
                width = new float[] { 30f, 80f };
                cell = new PdfPCell(new Phrase("ADRESİ", fontNormal));
                cell2 = new PdfPCell(new Phrase(Adres, fontNormal));

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell2);

                table1.WidthPercentage = 110;
                table1.SetWidths(width);
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //TELEFON
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(2);
                width = new float[] { 30f, 80f };
                cell = new PdfPCell(new Phrase("TELEFON", fontNormal));
                cell2 = new PdfPCell(new Phrase(Telefon, fontNormal));

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell2);

                table1.WidthPercentage = 110;
                table1.SetWidths(width);
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //ariza bilgisi başlık
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(1);
                cell = new PdfPCell(new Phrase("ARIZA BİLGİSİ", fontBold));

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

                table1.AddCell(cell);
                table1.WidthPercentage = 110;
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //ariza bilgisi içerik
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(1);
                cell = new PdfPCell(new Phrase(Ariza, fontIcerik));

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = 40;

                table1.AddCell(cell);
                table1.WidthPercentage = 110;
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //YAPILAN İŞLEM başlık
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(1);
                cell = new PdfPCell(new Phrase("YAPILAN İŞLEMLER", fontBold));

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

                table1.AddCell(cell);
                table1.WidthPercentage = 110;
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //YAPILAN İŞLEM içerik
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(1);
                cell = new PdfPCell(new Phrase(Islem, fontIcerik));

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = 40;

                table1.AddCell(cell);
                table1.WidthPercentage = 110;
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //dEĞŞEN PARÇA, TAMİR ÜCRETİ
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(2);
                cell = new PdfPCell(new Phrase("DEĞİŞEN PARÇALAR", fontBold));
                cell2 = new PdfPCell(new Phrase("TAMİR ÜCRETİ", fontBold));
                width = new float[] { 70f, 40f };

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table1.AddCell(cell2);

                table1.AddCell(cell);
                table1.SetWidths(width);
                table1.WidthPercentage = 110;
                doc.Add(table1);

                //---------------------------------------------------------------------------------
                //dEĞŞEN PARÇA, TAMİR ÜCRETİ İÇERİĞİ
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(2);
                cell = new PdfPCell(new Phrase(Parca, fontIcerik));
                cell2 = new PdfPCell(new Phrase(String.Format("{0:C}", OnarimUcreti), fontNormal));

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.FixedHeight = 35;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.FixedHeight = 35;
                table1.AddCell(cell2);

                table1.AddCell(cell);
                table1.SetWidths(width);
                table1.WidthPercentage = 110;
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //SERVİS EKİBİ,MONTAJ EKİBİ, MÜŞTERİ AD SOYAD başlık
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(3);
                cell = new PdfPCell(new Phrase("Servis Ekibi \n (Adı Soyadı - İmza)", fontSon));
                cell2 = new PdfPCell(new Phrase("Montaj Ekibi \n (Adı Soyadı - İmza)", fontSon));
                cell3 = new PdfPCell(new Phrase("Müşteri (Adı Soyadı - İmza)", fontSon));

                width = new float[] { 35f, 35f, 40f };

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table1.AddCell(cell2);

                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                cell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell3.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table1.AddCell(cell3);

                table1.AddCell(cell);
                table1.SetWidths(width);
                table1.WidthPercentage = 110;
                doc.Add(table1);
                //---------------------------------------------------------------------------------
                //SERVİS EKİBİ,MONTAJ EKİBİ, MÜŞTERİ AD SOYAD içerik
                //---------------------------------------------------------------------------------
                table1 = new PdfPTable(3);
                cell = new PdfPCell(new Phrase("", fontNormal));
                cell2 = new PdfPCell(new Phrase("", fontNormal));
                cell3 = new PdfPCell(new Phrase("", fontNormal));

                width = new float[] { 35f, 35f, 40f };

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.FixedHeight = 25;
                table1.AddCell(cell);

                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.FixedHeight = 25;
                table1.AddCell(cell2);

                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                cell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell2.FixedHeight = 25;
                table1.AddCell(cell3);

                table1.AddCell(cell);
                table1.SetWidths(width);
                table1.WidthPercentage = 110;
                doc.Add(table1);

                doc.Close();
                //open document
                System.Diagnostics.Process.Start(@SavePath);

            }
        }
        public void CreateSatisPDF()
        {
            SatisTablo recordSatis = SatisModel.GetSatisRecord(ID);
            OdemeTablo recordOdeme = Models.GetOdemeRecordSatis(ID);
            MusteriAdi = recordSatis.MusteriTablo.MusteriIsim;
            Adres = recordSatis.MusteriTablo.MusteriAdres;
            Telefon = recordSatis.MusteriTablo.MusteriTelefon;
            Tarih = ((DateTime)recordSatis.Tarih).ToString("dd/MM/yyyy");
            Konu = recordSatis.Konu;
            satisorTeklif = Convert.ToBoolean(recordSatis.SatisDurumu);
            KDVCheck = Convert.ToBoolean(recordSatis.KDV);
            ToplamTutar = Convert.ToDecimal(recordOdeme.ToplamTutar);
            KUR = recordOdeme.KurID == 1 ? "TL" : "USD";


            if (KDVCheck == true)
            {
                KDV = ToplamTutar - (ToplamTutar / 1.18M);
                Tutar = ToplamTutar / (1.18M);
            }
            else
            {
                Tutar = ToplamTutar / 1.18M;
                KDV = ToplamTutar - Tutar;
            }

            var satisDetayList = SatisModel.GetSatisDetail(ID);

            Document doc = new Document(PageSize.A4, 10f, 10f, 60f, 10f);

            BaseColor myColor = iTextSharp.text.html.WebColors.GetRGBColor("#FFD700");
            var myGoldColor = new BaseColor(255, 215, 0);

            BaseFont myFont = BaseFont.CreateFont(@"C:\windows\fonts\arial.ttf", "windows-1254", BaseFont.EMBEDDED);
            Font fontNormal = new Font(myFont, 10, Font.NORMAL);
            Font fontBold = new Font(myFont, 10, Font.BOLD);
            Font fontBoldBlack = new Font(myFont, 10, Font.BOLD, BaseColor.BLACK);
            Font fontBoldGold = new Font(myFont, 10, Font.BOLD, BaseColor.BLACK);
            Font fontIcerik = new Font(myFont, 10, Font.NORMAL);
            Font fontSon = new Font(myFont, 9, Font.NORMAL);
            
            FileStream os;
            
            if(SavePath==null)
                SavePath = "C://KazancYazilim//MalkocBilisim//PDF//Satis_Teklif//" + ID.ToString() + (satisorTeklif == true ? "S" : "T") + ".pdf";
            
            try
            {
                os = new FileStream(@SavePath, FileMode.Create);
            }
            catch
            {
                CreatePath();
                os = new FileStream(@SavePath, FileMode.Create);
            }

            using (os)
            {
                PdfWriter.GetInstance(doc, os);
                doc.Open();

                iTextSharp.text.Image myImage;
                try
                {
                    myImage = iTextSharp.text.Image.GetInstance("http://www.kazancyazilim.com/img/LogoForm.png");
                }
                catch (Exception)
                {
                    myImage = null;
                }

                PdfPTable table = new PdfPTable(3);
                myImage.ScaleAbsolute(150f, 50f);

                PdfPCell image = new PdfPCell(myImage);
                image.HorizontalAlignment = Element.ALIGN_CENTER;
                image.VerticalAlignment = Element.ALIGN_MIDDLE;
                image.Border = Rectangle.NO_BORDER;
                table.AddCell(image);

                PdfPCell info = new PdfPCell(new Phrase(satisorTeklif == true ? "Satış bildirgesidir." : "Tekliftir.", fontBold));
                info.HorizontalAlignment = Element.ALIGN_CENTER;
                info.VerticalAlignment = Element.ALIGN_BOTTOM;
                info.Border = Rectangle.NO_BORDER;
                info.Colspan = 1;
                table.AddCell(info);

                PdfPTable infofirma = new PdfPTable(1);
                PdfPCell Telefon = new PdfPCell(new Phrase("       Telefon: " + "(530) 674 9516", fontNormal));
                PdfPCell Email = new PdfPCell(new Phrase("       E-mail  : " + "info@malkocbilisim.com", fontNormal));
                PdfPCell Tarih = new PdfPCell(new Phrase("       Tarih    : " + DateTime.Now.ToString("dd/MM/yyyy"), fontNormal));
                PdfPCell web = new PdfPCell(new Phrase("       www.malkocbilisim.com", fontNormal));
                Telefon.Border = Rectangle.NO_BORDER;
                Email.Border = Rectangle.NO_BORDER;
                Tarih.Border = Rectangle.NO_BORDER;
                web.Border = Rectangle.NO_BORDER;

                infofirma.AddCell(Telefon);
                infofirma.AddCell(Email);
                infofirma.AddCell(Tarih);
                infofirma.AddCell(web);

                PdfPCell cell = new PdfPCell(infofirma);
                cell.Border = Rectangle.NO_BORDER;
                cell.Colspan = 1;
                table.AddCell(cell);

                table.WidthPercentage = 100;
                table.SpacingAfter = 30;
                doc.Add(table);

                PdfPTable musteriBilgi = new PdfPTable(3);
                PdfPCell musteriAdi = new PdfPCell(new Phrase("Sayın : " + MusteriAdi, fontBold));
                musteriAdi.Colspan = 2;
                musteriAdi.Border = Rectangle.NO_BORDER;
                musteriBilgi.AddCell(musteriAdi);

                PdfPCell saygi = new PdfPCell(new Phrase("Saygılarımızla", fontNormal));
                saygi.Colspan = 1;
                saygi.Border = Rectangle.NO_BORDER;
                saygi.HorizontalAlignment = Element.ALIGN_CENTER;
                musteriBilgi.AddCell(saygi);

                PdfPCell konu = new PdfPCell(new Phrase("Konu  : " + Konu, fontNormal));
                konu.Colspan = 2;
                konu.Border = Rectangle.NO_BORDER;
                musteriBilgi.AddCell(konu);

                PdfPCell firmaSahibi = new PdfPCell(new Phrase("Sinan MALKOÇ", fontNormal));
                firmaSahibi.Colspan = 1;
                firmaSahibi.Border = Rectangle.NO_BORDER;
                firmaSahibi.HorizontalAlignment = Element.ALIGN_CENTER;
                musteriBilgi.AddCell(firmaSahibi);

                musteriBilgi.WidthPercentage = 93;
                doc.Add(musteriBilgi);

                //255,215,0	#FFD700
                PdfPTable satisDetayBaslik = new PdfPTable(5);
                float[] width = new float[] { 6f, 55f, 9f, 15f, 20f };

                PdfPCell kod = new PdfPCell(new Phrase("SIRA", fontBoldBlack));
                PdfPCell aciklama = new PdfPCell(new Phrase("ÜRÜN BİLGİSİ", fontBoldBlack));
                PdfPCell miktar = new PdfPCell(new Phrase("MİKTAR", fontBoldBlack));
                PdfPCell birimFiyat = new PdfPCell(new Phrase("BİRİM FİYAT", fontBoldBlack));
                PdfPCell tutar = new PdfPCell(new Phrase("TUTAR " + KUR, fontBoldBlack));

                kod.BackgroundColor = myColor;
                aciklama.BackgroundColor = myColor;
                miktar.BackgroundColor = myColor;
                birimFiyat.BackgroundColor = myColor;
                tutar.BackgroundColor = myColor;

                kod.HorizontalAlignment = Element.ALIGN_CENTER;
                aciklama.HorizontalAlignment = Element.ALIGN_CENTER;
                miktar.HorizontalAlignment = Element.ALIGN_CENTER;
                birimFiyat.HorizontalAlignment = Element.ALIGN_CENTER;
                tutar.HorizontalAlignment = Element.ALIGN_CENTER;

                kod.VerticalAlignment = Element.ALIGN_MIDDLE;
                aciklama.VerticalAlignment = Element.ALIGN_MIDDLE;
                miktar.VerticalAlignment = Element.ALIGN_MIDDLE;
                birimFiyat.VerticalAlignment = Element.ALIGN_MIDDLE;
                tutar.VerticalAlignment = Element.ALIGN_MIDDLE;

                kod.FixedHeight = 20;
                aciklama.FixedHeight = 20;
                miktar.FixedHeight = 20;
                birimFiyat.FixedHeight = 20;
                tutar.FixedHeight = 20;

                kod.BorderWidth = 1;
                aciklama.BorderWidth = 1;
                miktar.BorderWidth = 1;
                birimFiyat.BorderWidth = 1;
                tutar.BorderWidth = 1;

                satisDetayBaslik.AddCell(kod);
                satisDetayBaslik.AddCell(aciklama);
                satisDetayBaslik.AddCell(miktar);
                satisDetayBaslik.AddCell(birimFiyat);
                satisDetayBaslik.AddCell(tutar);

                satisDetayBaslik.WidthPercentage = 92;
                satisDetayBaslik.SetWidths(width);
                satisDetayBaslik.SpacingBefore = 30;

                doc.Add(satisDetayBaslik);



                PdfPTable satisDetay = new PdfPTable(5);

                int i = 1;
                foreach (var item in satisDetayList)
                {
                    kod = new PdfPCell(new Phrase(i.ToString(), fontNormal));
                    aciklama = new PdfPCell(new Phrase(item.UrunAdi, fontNormal));
                    miktar = new PdfPCell(new Phrase(item.UrunMiktari.ToString(), fontNormal));
                    birimFiyat = new PdfPCell(new Phrase(String.Format("{0:C}", item.UrunBirimFiyat), fontNormal));
                    tutar = new PdfPCell(new Phrase(String.Format("{0:C}", item.UrunToplamTutar), fontNormal));

                    kod.HorizontalAlignment = Element.ALIGN_CENTER;
                    aciklama.HorizontalAlignment = Element.ALIGN_LEFT;
                    miktar.HorizontalAlignment = Element.ALIGN_CENTER;
                    birimFiyat.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tutar.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tutar.PaddingRight = 5f;

                    kod.VerticalAlignment = Element.ALIGN_MIDDLE;
                    aciklama.VerticalAlignment = Element.ALIGN_MIDDLE;
                    miktar.VerticalAlignment = Element.ALIGN_MIDDLE;
                    birimFiyat.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tutar.VerticalAlignment = Element.ALIGN_MIDDLE;


                    kod.BorderWidth = 1;
                    aciklama.BorderWidth = 1;
                    miktar.BorderWidth = 1;
                    birimFiyat.BorderWidth = 1;
                    tutar.BorderWidth = 1;

                    satisDetay.AddCell(kod);
                    satisDetay.AddCell(aciklama);
                    satisDetay.AddCell(miktar);
                    satisDetay.AddCell(birimFiyat);
                    satisDetay.AddCell(tutar);
                    i++;
                }




                satisDetay.WidthPercentage = 92;
                satisDetay.SetWidths(width);
                doc.Add(satisDetay);

                PdfPTable toplam = new PdfPTable(3);

                PdfPCell ttutarBaslik = new PdfPCell(new Phrase("TUTAR", fontNormal));
                PdfPCell ttutar = new PdfPCell(new Phrase(String.Format("{0:C}", Tutar), fontNormal));
                PdfPCell kdvBaslik = new PdfPCell(new Phrase("KDV %18", fontNormal));
                PdfPCell kdv = new PdfPCell(new Phrase(String.Format("{0:C}", KDV), fontNormal));
                PdfPCell toplamtutarBaslik = new PdfPCell(new Phrase("TOPLAM", fontNormal));
                PdfPCell toplamTutar = new PdfPCell(new Phrase(String.Format("{0:C}", ToplamTutar), fontNormal));
                PdfPCell bos = new PdfPCell();

                bos.Border = Rectangle.NO_BORDER;

                ttutarBaslik.HorizontalAlignment = Element.ALIGN_CENTER;
                ttutar.HorizontalAlignment = Element.ALIGN_CENTER;
                kdvBaslik.HorizontalAlignment = Element.ALIGN_CENTER;
                kdv.HorizontalAlignment = Element.ALIGN_CENTER;
                toplamtutarBaslik.HorizontalAlignment = Element.ALIGN_CENTER;
                toplamTutar.HorizontalAlignment = Element.ALIGN_CENTER;

                ttutarBaslik.VerticalAlignment = Element.ALIGN_MIDDLE;
                ttutar.VerticalAlignment = Element.ALIGN_MIDDLE;
                kdvBaslik.VerticalAlignment = Element.ALIGN_MIDDLE;
                kdv.VerticalAlignment = Element.ALIGN_MIDDLE;
                toplamtutarBaslik.VerticalAlignment = Element.ALIGN_MIDDLE;
                toplamTutar.VerticalAlignment = Element.ALIGN_MIDDLE;

                ttutarBaslik.BorderWidth = 1;
                ttutar.BorderWidth = 1;
                kdvBaslik.BorderWidth = 1;
                kdv.BorderWidth = 1;
                toplamtutarBaslik.BorderWidth = 1;
                toplamTutar.BorderWidth = 1;

                ttutarBaslik.FixedHeight = 30;
                ttutar.FixedHeight = 30;
                kdvBaslik.FixedHeight = 30;
                kdv.FixedHeight = 30;
                toplamtutarBaslik.FixedHeight = 30;
                toplamTutar.FixedHeight = 30;

                toplam.AddCell(bos);
                toplam.AddCell(ttutarBaslik);
                toplam.AddCell(ttutar);
                toplam.AddCell(bos);
                toplam.AddCell(kdvBaslik);
                toplam.AddCell(kdv);
                toplam.AddCell(bos);
                toplam.AddCell(toplamtutarBaslik);
                toplam.AddCell(toplamTutar);

                toplam.WidthPercentage = 92;
                width = new float[] { 70f, 15f, 20f };
                toplam.SetWidths(width);
                toplam.SpacingAfter = 30;
                doc.Add(toplam);

                PdfPTable kosul = new PdfPTable(1);
                PdfPCell kosuldetay = new PdfPCell(new Phrase("Bilgilendirme", fontBold));
                PdfPCell kosuldetay2 = new PdfPCell(new Phrase("*Fiyatlar," + KUR + " olarak fiyatlandırılmıştır.", fontNormal));

                kosuldetay.Border = Rectangle.NO_BORDER;
                kosuldetay2.Border = Rectangle.NO_BORDER;

                kosuldetay.FixedHeight = 20;
                kosuldetay2.FixedHeight = 20;

                kosul.AddCell(kosuldetay);
                if (KDVCheck == true)
                {
                    PdfPCell kosuldetay3 = new PdfPCell(new Phrase("*Listedeki ürünlerin tutarları, KDV dahil edilmiş tutarlardır.", fontNormal));
                    kosuldetay3.Border = Rectangle.NO_BORDER;
                    kosuldetay3.FixedHeight = 20;
                    kosul.AddCell(kosuldetay3);
                }
                kosul.AddCell(kosuldetay2);
                doc.Add(kosul);

                PdfPTable firmafooter = new PdfPTable(1);
                PdfPCell firmaAdi = new PdfPCell(new Phrase("MALKOÇ BİLİŞİM BİLGİSAYAR TEKNİK SERVİSİ - SİNAN MALKOÇ", fontBoldGold));
                PdfPCell adres = new PdfPCell(new Phrase("Kemerçeşme Mahallesi Kemerçeşme Caddesi No:67/1 Osmangazi/BURSA", fontSon));
                PdfPCell telefon = new PdfPCell(new Phrase("Telefon : (530) 674 95 16", fontSon));
                PdfPCell Emailweb = new PdfPCell(new Phrase("E-mail : info@malkocbilisim.com | www.malkocbilisim.com", fontSon));
                PdfPCell line = new PdfPCell(new Phrase("________________________________________________________", fontBold));

                line.VerticalAlignment = Element.ALIGN_BOTTOM;
                line.Border = Rectangle.NO_BORDER; ;

                firmaAdi.Border = Rectangle.NO_BORDER;
                adres.Border = Rectangle.NO_BORDER;
                telefon.Border = Rectangle.NO_BORDER;
                Emailweb.Border = Rectangle.NO_BORDER;

                firmafooter.AddCell(line);
                firmafooter.AddCell(firmaAdi);
                firmafooter.AddCell(adres);
                firmafooter.AddCell(telefon);
                firmafooter.AddCell(Emailweb);

                firmafooter.WidthPercentage = 92;

                doc.Add(firmafooter);





                doc.Close();
                //open document
                System.Diagnostics.Process.Start(@SavePath);

            }
        }
        public bool OpenTeknikPDF(int TeknikID)
        {
            try
            {
                System.Diagnostics.Process.Start(@"C://KazancYazilim//MalkocBilisim//PDF//TeknikServisFormu//" + TeknikID.ToString() + "TSF.pdf");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool OpenSatisPDF(int SatisID)
        {
            bool satisDurumu = Convert.ToBoolean(SatisModel.GetSatisRecord(SatisID).SatisDurumu);

            try
            {
                System.Diagnostics.Process.Start(@"C://KazancYazilim//MalkocBilisim//PDF//Satis_Teklif//" + SatisID.ToString() + (satisDurumu == true ? "S" : "T") + ".pdf");
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
