using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MalkocBilisim
{
    public partial class Hakkında : Form
    {
        public Hakkında()
        {
            InitializeComponent();
        }

        public Version AssemblyVersion
        {
            get
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(ApplicationDeployment.IsNetworkDeployed)
            {
                lbVersion.Text = AssemblyVersion.ToString(4);
            }
        }

        private bool openClose = false;
        private void btniletisim_Click(object sender, EventArgs e)
        {
           if(openClose==false)
           {
               this.Height = 311;
               openClose = true;
           }
           else
           {
               this.Height = 162;
               openClose = false;
           }
        }
    }
}
