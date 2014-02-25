using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Windows.Media.Imaging;

namespace timecapsule_test
{
    class commentsObj
    {
        public string un { set; get; }
        public double comLat { set; get; }
        public double comLng { set; get; }
        public string commentposted { set; get; }
    }

    class allimages
    {
        public string Id { get; set; }


        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }

        [JsonProperty(PropertyName = "containerName")]
        public string ContainerName { get; set; }

        [JsonProperty(PropertyName = "resourceName")]
        public string ResourceName { get; set; }

        [JsonProperty(PropertyName = "sasQueryString")]
        public string SasQueryString { get; set; }

        [JsonProperty(PropertyName = "imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public double lat { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public double lng { get; set; }
    }

    public partial class map1 : PhoneApplicationPage
    {
        List<commentsObj> nearbyComments = new List<commentsObj>();
        double Latitude;
        double Longitude;
        Map MyMap = new Map();
        Image image;
        private ReverseGeocodeQuery MyReverseGeocodeQuery = null;
        String msgBoxText = "";
        String uname = string.Empty;
        String friend = string.Empty;
        MapLayer myLocationLayer = new MapLayer();
        CameraCaptureTask cameraCaptureTask;

        public map1()
        {
            InitializeComponent();
            AddAppBar();
            ShowMyLocationOnTheMap();

            MyMap.ZoomLevel = 18;
            MyMap.LandmarksEnabled = true;
            MyMap.PedestrianFeaturesEnabled = true;

            ContentPanel.Children.Add(MyMap);

            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        void AddAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Icons/edit.png", UriKind.Relative);
            button1.Text = "comment";
            button1.Click += new EventHandler(Comment_Click);
            ApplicationBar.Buttons.Add(button1);
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Icons/feature.camera.png", UriKind.Relative);
            button2.Text = "camera";
            button2.Click += new EventHandler(Camera_Click);
            ApplicationBar.Buttons.Add(button2);
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Icons/microphone.png", UriKind.Relative);
            button3.Text = "record";
            button3.Click += new EventHandler(Record_Click);
            ApplicationBar.Buttons.Add(button3);
            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Icons/questionmark.png", UriKind.Relative);
            button4.Text = "help";
            ApplicationBar.Buttons.Add(button4);
        }
        

        // Using a stream reference to upload the image to blob storage.
        Stream imageStream = null;
        // MobileServiceCollectionView implements ICollectionView (useful for databinding to lists) and 
        // is integrated with your Mobile Service to make it easy to bind your data to the ListView
        private MobileServiceCollection<allimages, allimages> items;

        private IMobileServiceTable<allimages> todoTable = App.MobileService.GetTable<allimages>();

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.ChosenPhoto.Length.ToString());
                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                myImage.Source = bmp;
                myImage.Tap += new EventHandler<GestureEventArgs>(onImageTap);
                var todoItem = new allimages { username = uname };
                InsertTodoItem(todoItem);
            }
            
        }

        private async void InsertTodoItem(allimages todoItem)
        {
            string errorString = string.Empty;

            if (imageStream != null)
            {
                // Set blob properties of TodoItem.
                todoItem.ContainerName = "todoitemimages";
                todoItem.ResourceName = Guid.NewGuid().ToString() + ".jpg";
            }

            // Send the item to be inserted. When blob properties are set this
            // generates an SAS in the response.
            await todoTable.InsertAsync(todoItem);

            // If we have a returned SAS, then upload the blob.
            if (!string.IsNullOrEmpty(todoItem.SasQueryString))
            {
                // Get the URI generated that contains the SAS 
                // and extract the storage credentials.
                StorageCredentials cred = new StorageCredentials(todoItem.SasQueryString);
                var imageUri = new Uri(todoItem.ImageUri);

                // Instantiate a Blob store container based on the info in the returned item.
                CloudBlobContainer container = new CloudBlobContainer(
                    new Uri(string.Format("https://{0}/{1}",
                        imageUri.Host, todoItem.ContainerName)), cred);

                // Upload the new image as a BLOB from the stream.
                CloudBlockBlob blobFromSASCredential =
                    container.GetBlockBlobReference(todoItem.ResourceName);
                await blobFromSASCredential.UploadFromStreamAsync(imageStream);

                // When you request an SAS at the container-level instead of the blob-level,
                // you are able to upload multiple streams using the same container credentials.

                imageStream = null;
            }

            // Add the new item to the collection.
            items.Add(todoItem);
            
        }

        private void onImageTap(object sender, GestureEventArgs e)
        {
            ShowImage SI = new ShowImage((Image)sender);
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string parameterValue = NavigationContext.QueryString["u"];
            uname = parameterValue;
            txtUser.Text =  uname;
        }

        private async void ShowMyLocationOnTheMap()
        {
            //// Create a small circle to mark the current location.
            //Ellipse myCircle = new Ellipse();
            //myCircle.Fill = new SolidColorBrush(Colors.Red);
            //myCircle.Height = 20;
            //myCircle.Width = 20;
            //myCircle.Opacity = 50;



            // Get my current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            Latitude = myGeocoordinate.Latitude;
            Longitude = myGeocoordinate.Longitude;


            MyMap.Center = new GeoCoordinate(Latitude, Longitude);


            // Create a MapLayer to contain the MapOverlay.
           
            DrawLocMarker(MyMap.Center, Colors.Red, myLocationLayer);
            
             await SendHttpClientRequest();

             RefreshTodoItems();
            if (nearbyComments.Count > 0)
            {
                foreach (commentsObj c in nearbyComments)
                {
                    friend = c.un;
                    msgBoxText = c.commentposted;
                    DrawMapMarker(new GeoCoordinate(c.comLat, c.comLng), Colors.Blue, myLocationLayer);


                }
            }

            //GeoCoordinate coordinate = new GeoCoordinate(Latitude + 0.001, Longitude + 0.001);
            //DrawMapMarker(coordinate, Colors.Blue, myLocationLayer);
            MyMap.Layers.Add(myLocationLayer);

            //MapLayer myLocationLayer2 = new MapLayer();
            //DrawMapMarker(coordinate, Colors.Blue, myLocationLayer2);
            //MyMap.Layers.Add(myLocationLayer2);

        }

        private async void RefreshTodoItems()
        {
            // This code refreshes the entries in the list view be querying the TodoItems table.
            // The query excludes completed TodoItems
            try
            {
                items = await todoTable
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Error loading items", MessageBoxButton.OK);
            }

            foreach(allimages a in items)
            {
                BitmapImage Icon = new BitmapImage();
                Icon.SetSource(Application.GetResourceStream(new Uri(a.ImageUri, UriKind.Relative)).Stream);
                image = new Image { Source = Icon };
                DrawMapMarker(new GeoCoordinate(a.lat, a.lng), Colors.Blue, myLocationLayer);
                LayoutRoot.Children.Add(image);
            }
            image = null;
        }

        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    MapAddress address = e.Result[0].Information.Address;
                    


                    string Header = "";
                    Header = friend + " says";

                    //if (address.Country.Length > 0) msgBoxText += "line\nline\nline\nline\nline\nline\n";
                    MessageBox.Show(msgBoxText, Header, MessageBoxButton.OK);
                }
            }
        }

        private void Loc_Click(object sender, EventArgs e)
        {

            Polygon p = (Polygon)sender;
            //GeoCoordinate geoCoordinate = (GeoCoordinate)p.Tag;
            //MyReverseGeocodeQuery = new ReverseGeocodeQuery();
            //MyReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude);
            ////MyReverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
            //MyReverseGeocodeQuery.QueryAsync();
        }

        private void Marker_Click(object sender, EventArgs e)
        {

            Ellipse p = (Ellipse)sender;
            GeoCoordinate geoCoordinate = (GeoCoordinate)p.Tag;
            MyReverseGeocodeQuery = new ReverseGeocodeQuery();
            MyReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude);
            MyReverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
            MyReverseGeocodeQuery.QueryAsync();
        }

        private void DrawLocMarker(GeoCoordinate coordinate, Color color, MapLayer mapLayer)
        {

            // Create a map marker
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(0, 75));
            polygon.Points.Add(new Point(25, 0));
            polygon.Fill = new SolidColorBrush(color);

            // Enable marker to be tapped for location information
            polygon.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            polygon.MouseLeftButtonUp += new MouseButtonEventHandler(Loc_Click);

            // Create a MapOverlay and add marker
            MapOverlay overlay = new MapOverlay();
            overlay.Content = polygon;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0, 1.0);
            mapLayer.Add(overlay);

        }

        private void DrawMapMarker(GeoCoordinate coordinate, Color color, MapLayer mapLayer)
        {

            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(color);
            myCircle.Height = 40;
            myCircle.Width = 40;
            myCircle.Opacity = 50;

            //Image img = new Image();
            //BitmapImage myBitmapImage = new BitmapImage();

            //myBitmapImage.UriSource = new Uri(@"message.png");

            ////set image source
            //img.Source = myBitmapImage;
            

            // Enable marker to be tapped for location information
            myCircle.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            myCircle.MouseLeftButtonUp += new MouseButtonEventHandler(Marker_Click);

            // Create a MapOverlay and add marker
            MapOverlay overlay = new MapOverlay();
            if(image!=null)
                overlay.Content = image;
            else
                overlay.Content = myCircle;

            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0, 1.0);
            mapLayer.Add(overlay);
        }

        private void Comment_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddComment.xaml?u=" + uname + "&lat=" + Latitude + "&lng=" + Longitude, UriKind.Relative));
        }

        private void Camera_Click(object sender, EventArgs e)
        {
            cameraCaptureTask.Show();
        }

        private void Record_Click(object sender, EventArgs e)
        {
            //StackPanel sp= LayoutRoot.Children.
            //if(this.LayoutRoot.Children.Contains("RecordPanel"))
            RecordControl RC = new RecordControl();
            RC.VerticalAlignment = VerticalAlignment.Top;
            LayoutRoot.Children.Add(RC);
        }


        public async Task SendHttpClientRequest()
        {
            //HttpResponseMessage response = await httpClient.GetAsync(resourceAddress)
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("https://timecapsule-test.azure-mobile.net/api/getcomments?un=" + uname + "&lat=" + Latitude + "&lng=" + Longitude);
                //response.EnsureSuccessStatusCode();
                //string responseBody = string.Empty;
                results = await response.Content.ReadAsStringAsync();
                String[] t = results.Split('_');
                if (t[1] != "")
                {
                    String[] res = t[1].Split('\n');
                    foreach (String r in res)
                    {
                        if (r != "")
                        {
                            String[] cols = r.Split(';');
                            commentsObj c = new commentsObj();
                            c.un = cols[0];
                            c.comLat = Double.Parse(cols[1]);
                            c.comLng = Double.Parse(cols[2]);
                            c.commentposted = cols[3];
                            nearbyComments.Add(c);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("\nException Message :{0} ", e.Message);
            }
        }



        public void authHttp()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://timecapsule-test.azure-mobile.net/api/getcomments?un=" + uname + "&lat=" + Latitude + "&lng=" + Longitude);
            
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            

        }
        String results = null;
        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);

            using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
            {
                results = httpwebStreamReader.ReadToEnd();
            }
            myResponse.Close();
           
            
        }


        private void onQues()
        {
            
        }
    }
}