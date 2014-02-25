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
    class comments
    {
        public String id { set; get; }
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }
        [JsonProperty(PropertyName = "commentposted")]
        public String commentposted { get; set; }
        [JsonProperty(PropertyName = "loc_lat")]
        public double loc_lat { get; set; }
        [JsonProperty(PropertyName = "loc_lng")]
        public double loc_lng { get; set; }

    }

    public partial class AddComment : PhoneApplicationPage
    {
        string uname = null;
        double lat = 0.0, lng = 0.0;
        private IMobileServiceTable<comments> commentTable = App.MobileService.GetTable<comments>();
        public AddComment()
        {
            InitializeComponent();            
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string parameterValue = NavigationContext.QueryString["u"];
            lat = Double.Parse(NavigationContext.QueryString["lat"]);
            lng = Double.Parse(NavigationContext.QueryString["lng"]);
            uname = parameterValue;
            txtUser.Text = "Hi " + uname;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            comments temp = new comments();
            temp.loc_lat = lat;
            temp.loc_lng = lng;
            temp.username = uname;
            temp.commentposted = tbCom.Text;
            InsertComment(temp);
        }

        private async void InsertComment(comments c)
        {
            try
            {
                var call = commentTable.InsertAsync(c);
                await call;
                if (call.IsFaulted)
                {
                    MessageBox.Show("Comment add failed");
                }
                if (call.IsCompleted)
                {
                    MessageBox.Show("Comment add success");
                }
                if (call.IsCanceled)
                {
                    MessageBox.Show("Comment add cancelled");
                }

            }
            catch (MobileServicePreconditionFailedException)
            {
                MessageBox.Show("Comment add failed");
            }
            //items.Add(u);
        }
    }
}