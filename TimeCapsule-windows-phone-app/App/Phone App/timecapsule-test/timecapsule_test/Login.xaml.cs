using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;

namespace timecapsule_test
{
    public partial class Login : PhoneApplicationPage
    {
        private async void loginButton_SessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            if (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Opened)
            {
                var fb = new Facebook.FacebookClient(this.loginButton.CurrentSession.AccessToken);

                var result = await fb.GetTaskAsync("fql",
                    new
                    {
                        q = "SELECT username,uid, name FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1 = me())"
                    });

                System.Diagnostics.Debug.WriteLine("Result: " + result.ToString());
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/map1.xaml?u=" + tbFbUser.Text, UriKind.Relative)));

            }
            else if (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Closed)
            {
                // this.queryButton.Visibility = Visibility.Collapsed;
                //this.multiQueryButton.Visibility = Visibility.Collapsed;
            }
            // MessageBox.Show(tbUser.Text);
            // Dispatcher.BeginInvoke(() => MessageBox.Show(tbUser.Text));
        }
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Register.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            authHttp();
        }

        public void authHttp()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://timecapsule-test.azure-mobile.net/api/login?un="+tbuname.Text+"&p="+tbpwd.Password);
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);

        }
        String results=null;
        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);

            using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
            {
                results = httpwebStreamReader.ReadToEnd();
                String[] temp = results.Split(':');
                String uname = temp[1].Substring(0, temp[1].Length - 2).Replace("\"","");
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/map1.xaml?u=" + uname, UriKind.Relative)));

            }
            myResponse.Close();
        }
    }
}