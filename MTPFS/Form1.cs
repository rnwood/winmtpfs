using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NeoGeo.Library.SMB;
using NeoGeo.Library.SMB.Provider;
using System;
using System.Diagnostics;

namespace MTPFS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _smb = new SMB();
            string hostname = Environment.MachineName + "-MTP";
            linkLabel1.Text = string.Format(@"\\{0}\mtpdevices", hostname);
            _smb.Start(hostname, 16384, NetFlag.ListenAllNetworkCards, 8, "");
        }

        SMB _smb;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", linkLabel1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _smb.Stop(true, 1000);
            Application.Exit();
        }
    }
}
