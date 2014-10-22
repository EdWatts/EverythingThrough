using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EverythingThroughFiddlerPlugin
{
    public partial class RemoveProxy : Form
    {
        public delegate void ProxyRemovedEventHandler(string value);
        public event ProxyRemovedEventHandler ProxyRemoved;

        protected virtual void OnProxyRemoved(string value)
        {
            ProxyRemovedEventHandler handler = ProxyRemoved;
            if (handler != null) handler(value);
        }

        public RemoveProxy(IEnumerable<string> proxys)
        {
            InitializeComponent();

            if (proxys != null)
            {
                foreach (var proxy in proxys)
                {
                    cmbProxys.Items.Add(proxy);
                }
            }
        }

        private void BtnRemoveClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbProxys.SelectedText))
            {
                OnProxyRemoved(cmbProxys.SelectedText);
                Close();
            }
        }
    }
}
