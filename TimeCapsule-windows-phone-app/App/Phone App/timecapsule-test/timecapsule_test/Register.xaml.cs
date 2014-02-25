using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace timecapsule_test
{

    class tbl_user
    {
        public String id { get; set; }
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public String password { get; set; }

        [JsonProperty(PropertyName = "email")]
        public String email { get; set; }

        [JsonProperty(PropertyName = "firstname")]
        public String firstname { get; set; }

        [JsonProperty(PropertyName = "lastname")]
        public String lastname { get; set; }
        
    }

    public partial class Register : PhoneApplicationPage
    {
        //private MobileServiceCollection<tbl_user, tbl_user> items;
        private IMobileServiceTable<tbl_user> userTable = App.MobileService.GetTable<tbl_user>();
        public Register()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tbl_user u = new tbl_user();
            u.firstname = tbfirstname.Text;
            u.lastname = tblastname.Text;
            u.username = tbuname.Text;
            u.password = tbpwd.Text;
            u.email = tbemail.Text;
            InsertUser(u);
        }

        private async void InsertUser(tbl_user u)
        {
            try
            {
                var call = userTable.InsertAsync(u);
                await call;
                if (call.IsFaulted)
                {
                    MessageBox.Show("Registration failed");
                }
                if (call.IsCompleted)
                {
                    MessageBox.Show("Registration success");
                }
                if (call.IsCanceled)
                {
                    MessageBox.Show("Registration cancelled");
                }

            }
            catch (MobileServicePreconditionFailedException)
            {
                MessageBox.Show("Registration failed");
            }
            //items.Add(u);
        }
    }
}