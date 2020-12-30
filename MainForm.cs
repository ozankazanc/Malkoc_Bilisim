using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using MalkocBilisim.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MalkocBilisim.DB;

namespace MalkocBilisim
{
    public partial class MainForm : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private MKocdbEntityDataContext dc = new MKocdbEntityDataContext();
        private static MainForm _instance;
        Models m = new Models();
        TeknikModel t = new TeknikModel();
        SatisModel f = new SatisModel();
        public static MainForm Instance
        {
            get
            {
                if (_instance == null)
                { _instance = new MainForm(); }

                return _instance;
            }

        }
        public MainForm()
        {
            InitializeComponent();
            AddInstance();
            if(ApplicationDeployment.IsNetworkDeployed==false) //Developer Girişi
            {
                btnAdminPanel.Visibility = BarItemVisibility.Always;
            }
        }
        private void UpdateControl()
        {
            if (ApplicationDeployment.IsNetworkDeployed) //Developer Girişi
            {
                bool check;
                check = Models.CheckUpdate();

                if (check == false) //yeni güncelleme kullanıcıya gösterilmediyse
                {
                    var result = (from get in dc.Guncellemes
                                  orderby get.GuncellemeID descending
                                  select get).First();
                    result.Checked = true;
                    dc.SubmitChanges();

                    Yardım obj = new Yardım();
                    obj.ShowDialog();
                }
            }
            
        }
        private void AddInstance()
        {
            Container.Controls.Add(Anasayfa.Instance);
            Container.Controls.Add(TeknikSFormOlustur.Instance);
            Container.Controls.Add(TeknikSFormListele.Instance);
            Container.Controls.Add(SatisFormOlustur.Instance);
            Container.Controls.Add(SatisFormListele.Instance);
            Container.Controls.Add(Odemeler.Instance);
            Container.Controls.Add(Musteri.Instance);


        }
        private void aceFormOlustur_Click(object sender, EventArgs e)
        {
            
            if (!Container.Controls.Contains(TeknikSFormOlustur.Instance))
            {
                TeknikSFormOlustur.Instance.Dock = DockStyle.Fill;
                TeknikSFormOlustur.Instance.BringToFront();
            }
            TeknikSFormOlustur.Instance.BringToFront();

        }
        private void aceFormListele_Click(object sender, EventArgs e)
        {
            if (!Container.Controls.Contains(TeknikSFormListele.Instance))
            {
                TeknikSFormListele.Instance.Dock = DockStyle.Fill;
                TeknikSFormListele.Instance.BringToFront();
            }
            TeknikSFormListele.Instance.BringToFront();

        }

        private void aceTeklifOlustur_Click(object sender, EventArgs e)
        {
            if (!Container.Controls.Contains(SatisFormOlustur.Instance))
            {
                SatisFormOlustur.Instance.Dock = DockStyle.Fill;
                SatisFormOlustur.Instance.BringToFront();
            }
            SatisFormOlustur.Instance.BringToFront();
        }

        private void aceTeklifListele_Click(object sender, EventArgs e)
        {
            if (!Container.Controls.Contains(SatisFormListele.Instance))
            {
                SatisFormListele.Instance.Dock = DockStyle.Fill;
                SatisFormListele.Instance.BringToFront();
            }
            SatisFormListele.Instance.BringToFront();
        }

        private void aceOdemeOlustur_Click(object sender, EventArgs e)
        {
            if (!Container.Controls.Contains(Odemeler.Instance))
            {
                Odemeler.Instance.Dock = DockStyle.Fill;
                Odemeler.Instance.BringToFront();
            }
            Odemeler.Instance.BringToFront();
        }

        private void aceMusteri_Click(object sender, EventArgs e)
        {
            if (!Container.Controls.Contains(Musteri.Instance))
            {
                Musteri.Instance.Dock = DockStyle.Fill;
                Musteri.Instance.BringToFront();
            }
            Musteri.Instance.BringToFront();
        }

        private void aceAnasayfa_Click(object sender, EventArgs e)
        {
            if (!Container.Controls.Contains(Anasayfa.Instance))
            {
                Anasayfa.Instance.Dock = DockStyle.Fill;
                Anasayfa.Instance.BringToFront();
            }
            Anasayfa.Instance.BringToFront();
        }

        public Version AssemblyVersion
        {
            get
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            
            FluentSplashScreenOptions options = new FluentSplashScreenOptions();
            options.LogoImageOptions.Image = MalkocBilisim.Properties.Resources.kazanclogoORGkucuk;
            options.Title = "Bilgisayar Teknik Servisi";
            options.Subtitle = ApplicationDeployment.IsNetworkDeployed==true ? "Malkoç Bilişim" + AssemblyVersion.ToString(4) : "Malkoç Bilişim";
            options.RightFooter = "Program Başlatılıyor...";
            options.LeftFooter = "Kazanc Yazılım" + Environment.NewLine + "Otomasyon ve Yazılım Sistemleri";
            options.LoadingIndicatorType = FluentLoadingIndicatorType.Dots;
            options.Opacity = 130;


            SplashScreenManager.ShowFluentSplashScreen(options, parentForm: this, customDrawEventHandler: CustomDraw, useFadeIn: true, useFadeOut: true);

            System.Threading.Thread.Sleep(10000);
            SplashScreenManager.CloseForm();
            
            UpdateControl();
        }
        private void CustomDraw(object sender, FluentSplashScreenCustomDrawEventArgs e)
        {
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
            new Point(0, 0),
            new Point(e.Bounds.Height + 1200, e.Bounds.Width),
            Color.FromArgb(120, 40, 40, 40),
            Color.FromArgb(120, 243, 107, 3));
            e.Cache.FillRectangle(linGrBrush, e.Bounds);

        }

        private void btnYardim_ItemClick(object sender, ItemClickEventArgs e)
        {
            Yardım obj = new Yardım();
            obj.ShowDialog();
        }

        private void btnHakkinda_ItemClick(object sender, ItemClickEventArgs e)
        {

            Hakkında obj = new Hakkında();
            obj.ShowDialog();
        }

        private void btnAdminPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            AdminForm obj = new AdminForm();
            obj.ShowDialog();
        }
    }
}
