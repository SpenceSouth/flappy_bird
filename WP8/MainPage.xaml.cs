using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

namespace FlappybirdHd
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Url of Home page
        private string MainUri = "/index.html";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.IsScriptEnabled = true;
            Browser.Navigate(new Uri(MainUri, UriKind.Relative));
        }

        // Handle navigation failures.
        private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            MessageBox.Show("Navigation to this page failed, check your internet connection");
        }

        public void OnAppActivated()
        {
            Browser.InvokeScript("eval", "if (window.C2WP8Notify) C2WP8Notify('activated');");
        }

        public void OnAppDeactivated()
        {
            Browser.InvokeScript("eval", "if (window.C2WP8Notify) C2WP8Notify('deactivated');");
        }
        // Intercept external links and open in the browser
        
        void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            String scheme = null;

            try
            {
                scheme = e.Uri.Scheme;
            }
            catch
            {
            }

            if (scheme == null || scheme == "file")
                return;
            // Not going to follow any other link
            e.Cancel = true;
            if (scheme == "http")
            {
                // start it in Internet Explorer
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(e.Uri.AbsoluteUri);
                webBrowserTask.Show();
            }
            if (scheme == "mailto")
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.To = e.Uri.AbsoluteUri;
                emailComposeTask.Show();
            }
        }
        
        
        //Back button

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            try
            {
                Browser.InvokeScript("eval", "window['wp_call_OnBack']()");
            }
            catch
            {
                // Back function not available
            }
        }


        // **********************************************************************
        // JavaScript communicating with C#
        // **********************************************************************

        private async void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            // Get a comma delimited string from js and convert to array
            string valueStr = e.Value;
            string[] valueArr = valueStr.Split(',');

            // Trim and convert empty strings to null
            for (int i = 0; i < valueArr.Length; i++)
            {
                valueArr[i] = valueArr[i].Trim();
                if (string.IsNullOrWhiteSpace(valueArr[i]))
                    valueArr[i] = null;
            }

            // Quit app
            if (valueArr[0] == "quitApp")
            {
                App.Current.Terminate();
            }

        }
    }
}