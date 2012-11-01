using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NotificationsExtensions.TileContent;
using System.Net;
using System.Threading.Tasks;
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace slxWindows8App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var task = await MakeAsyncRequest("http://localhost:3333/sdata/%24app/mashups/-/mashups%28%27RemindersAndAlarms%27%29/%24queries/execute?_includeContent=false&_resultName=GetCounts&_userId=UDEMOA00000I&format=json");

            string numberoftasks = "";
            numberoftasks = task.Substring(task.IndexOf("pastDue") + 9, 2);
            txtResult.Text = "Your tile is updated: " + task.ToString();

            //this is the stuff that will go into the large template (the rectangle)
            ITileWideText03 tileContent = TileContentFactory.CreateTileWideText03();
            tileContent.TextHeadingWrap.Text = "Lee, you have " + numberoftasks + " task(s) due!";

            // This is the stuff that will go into the square (less space)
            ITileSquareText04 squareContent = TileContentFactory.CreateTileSquareText04();
            squareContent.TextBodyWrap.Text = "Lee \n " + numberoftasks + " task(s) due!";
            tileContent.SquareContent = squareContent;

            // send the notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

        }
        // Define other methods and classes here
        public static Task<string> MakeAsyncRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/html";
            request.Method = "GET";
            NetworkCredential nc = new NetworkCredential("Lee", "");
            request.Credentials = nc;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
           Launcher.LaunchUriAsync(new Uri("http://localhost:3333/SlxClient/ActivityManager.aspx"));

        }
    }
}
