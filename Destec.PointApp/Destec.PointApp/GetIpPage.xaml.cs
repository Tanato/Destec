using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Destec.PointApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetIpPage : Page
    {
        string urlBase = "http://";
        HttpClient httpClient = new HttpClient();

        public GetIpPage()
        {
            this.InitializeComponent();
        }

        private async Task Ping(string server)
        {
            try
            {
                var response = await httpClient.GetAsync(new Uri(urlBase + server + ":5000/api/ping"));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.Frame.Navigate(typeof(MainPage), ipInput.Text.Replace(',', '.'));
                }
            }
            catch (Exception)
            {
                message.Text = "Ip Inválido!";
            }
        }

        private async void ipInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await Ping(ipInput.Text.Replace(',','.'));
                e.Handled = true;
            }
        }

        private void ipInput_LostFocus(object sender, RoutedEventArgs e)
        {
            ipInput.Focus(FocusState.Programmatic);
        }
    }
}
