using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Fiddler;

[assembly: RequiredVersion("2.3.5.0")]

namespace EverythingThroughFiddlerPlugin
{
    public class EverythingThroughFiddler : IAutoTamper3
    {
        private const string ExtProxysSettingValue = "ext.EverythingThroughFiddler.Proxys";
        public static string Proxy = "";
        public static List<string> Proxies;
        private MenuItem _generatedMenuItem;

        public void OnLoad()
        {
            //For debug - reset preferences
            //FiddlerApplication.Prefs.SetStringPref(ExtProxysSettingValue, "");

            Proxies = FiddlerApplication.Prefs.GetStringPref(ExtProxysSettingValue, string.Empty).Split(',').ToList();
            Proxies.RemoveAll(string.IsNullOrWhiteSpace);
            FiddlerApplication.UI.mnuMain.MenuItems.Add(GenerateMenuItem());
        }

        private MenuItem GenerateMenuItem()
        {
            var menuItems = Proxies.Select(CreateMenuItem).ToList();
            menuItems.Add(new MenuItem("-"));
            menuItems.Add(new MenuItem(MenuMerge.MergeItems, 2, Shortcut.None, "Add Proxy", LaunchAddProxyForm, null, null, null));
            menuItems.Add(new MenuItem(MenuMerge.MergeItems, 3, Shortcut.None, "Remove Proxy", LaunchRemoveProxyForm, null, null, null));

            _generatedMenuItem = new MenuItem(MenuMerge.MergeItems, 0, Shortcut.None, "EverythingThrough", MainMenuClick, null, MainMenuClick, menuItems.ToArray());
            return _generatedMenuItem;
        }

        private static MenuItem CreateMenuItem(string previouslyAddedProxy)
        {
            EventHandler onChecked = delegate(object sender, EventArgs args)
            {
                var item = ((MenuItem)sender);
                item.Checked = true;
                Proxy = item.Text;
            };

            return new MenuItem(
                MenuMerge.MergeItems,
                1,
                Shortcut.None,
                previouslyAddedProxy,
                onChecked,
                null,
                null,
                null);
        }

        private void MainMenuClick(Object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Proxy))
            {
                foreach (MenuItem menuItem in ((MenuItem)sender).MenuItems)
                {
                    menuItem.Checked = Proxy == menuItem.Text;
                }
            }
        }

        private void LaunchAddProxyForm(Object sender, EventArgs e)
        {
            var frm = new AddProxy();
            frm.ProxyAdded += delegate(string value)
                              {
                                  if (!string.IsNullOrWhiteSpace(value))
                                  {
                                      Proxies.Add(value);
                                      _generatedMenuItem.MenuItems.Add(0, CreateMenuItem(value));
                                  }
                              };
            frm.ShowDialog();
        }

        private void LaunchRemoveProxyForm(Object sender, EventArgs e)
        {
            var frm = new RemoveProxy(Proxies);
            frm.ProxyRemoved += delegate(string value)
                                {
                                    MessageBox.Show(value);
                                    Proxies.RemoveAll(x => x == value);
                                    foreach (MenuItem menu in _generatedMenuItem.MenuItems)
                                    {
                                        if (menu.Name == value)
                                        {
                                            _generatedMenuItem.MenuItems.RemoveByKey(menu.Name);
                                        }
                                    }
                                };
            frm.ShowDialog();
        }


        public void OnBeforeUnload()
        {
            FiddlerApplication.Prefs.SetStringPref(ExtProxysSettingValue, string.Join(",", Proxies.ToArray()));
        }

        public void AutoTamperRequestBefore(Session oSession) { SetGateway(oSession); }
        public void AutoTamperRequestAfter(Session oSession) { }
        public void AutoTamperResponseBefore(Session oSession) { }
        public void AutoTamperResponseAfter(Session oSession) { }
        public void OnBeforeReturningError(Session oSession) { }
        public void OnPeekAtResponseHeaders(Session oSession) { }
        public void OnPeekAtRequestHeaders(Session oSession) { }

        private void SetGateway(Session oSession)
        {
            if (!string.IsNullOrWhiteSpace(Proxy))
            {
                oSession["X-OverrideGateway"] = Proxy;
                oSession.bypassGateway = true;
                //oSession["ui-backcolor"] = "#EC921A";
            }
        }
    }
}