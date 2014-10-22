using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;
using Microsoft.Win32;

namespace EverythingThroughFiddlerPlugin
{
    public partial class AddProxy : Form
    {
        public delegate void ProxyAddedEventHandler(string value);
        public event ProxyAddedEventHandler ProxyAdded;

        protected virtual void OnProxyAdded(string value)
        {
            ProxyAddedEventHandler handler = ProxyAdded;
            if (handler != null) handler(value);
        }

        public AddProxy()
        {
            InitializeComponent();
            string reuseClientSockets = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Fiddler2", "ReuseClientSockets", "False").ToString();
            string reuseServerSockets = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Fiddler2", "ReuseServerSockets", "False").ToString();
            if (reuseServerSockets == "True" || reuseClientSockets == "True")
            {
                lblWarning.Text = "If 'Reuse client connections' or 'Reuse connections \r\n" +
                                  "to servers options are enabled in config, then it \r\n" +
                                  "will take a while for the connections to be reset \r\n" +
                                  "when switching proxys";
                lblWarning.Visible = true;
                Height = 130;
            }
        }

        private void BtnAddClick(object sender, EventArgs e)
        {
            string proxyValue = txtProxy.Text.Replace(",", string.Empty).Trim();
            OnProxyAdded(proxyValue);
            Close();
        }
    }
}
