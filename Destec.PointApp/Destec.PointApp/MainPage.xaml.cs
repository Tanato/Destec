using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Web.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.System.Threading;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Destec.PointApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ModeEnum mode = ModeEnum.Normal;
        HttpClient httpClient = new HttpClient();
        string urlBase = "http://";
        string server = "localhost";

        bool waiting = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                server = e.Parameter.ToString();
            }
            base.OnNavigatedTo(e);
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var result = await Ping();
            if (!result)
            {
                Frame.Navigate(typeof(GetIpPage));
            }
        }

        private async Task<bool> Ping()
        {
            try
            {
                var response = await httpClient.GetAsync(new Uri(urlBase + server + ":5000/api/ping"));
                return (response.StatusCode == HttpStatusCode.Ok);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetColor(Color color)
        {
            modeBorderLeft.Fill = new SolidColorBrush(color);
            modeBorderBottom.Fill = new SolidColorBrush(color);
            modeBorderRight.Fill = new SolidColorBrush(color);
            modeBorderTop.Fill = new SolidColorBrush(color);

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = color;
                    titleBar.ButtonForegroundColor = color;
                    titleBar.BackgroundColor = color;
                    titleBar.ForegroundColor = color;
                }
            }
        }

        private async Task ExecuteActionAsync()
        {
            if (string.IsNullOrEmpty(mainInput.Text) || mainInput.Text.Length != 3)
            {
                labelAtividade.Text = "Código Inválido!";
                mainInput.Text = string.Empty;
                return;
            }
            else
            {
                try
                {
                    HttpResponseMessage response = null;
                    using (var http = new HttpClient())
                    {
                        switch (mode)
                        {
                            case ModeEnum.Normal:
                                response = await http.PostAsync(new Uri(urlBase + server + ":5000/api/atividade/execute?code=" + mainInput.Text), null);
                                break;
                            case ModeEnum.Intervalo:
                                response = await http.PostAsync(new Uri(urlBase + server + ":5000/api/atividade/interval?code=" + mainInput.Text), null);
                                break;
                            case ModeEnum.Parada:
                                response = await http.PostAsync(new Uri(urlBase + server + ":5000/api/atividade/stop?code=" + mainInput.Text), null);
                                break;
                            default:
                                break;
                        }
                    }
                    await HandleResponseAsync(response);
                    response = null;
                }
                catch (Exception)
                {
                    labelAtividade.Text = "Erro ao executar requisição de Atividade";
                    mainInput.Text = string.Empty;
                }
            }
        }

        private async Task HandleResponseAsync(HttpResponseMessage response)
        {
            if (response != null && response.StatusCode == HttpStatusCode.Ok)
            {
                labelAtividade.Text = response?.Content?.ToString();
                waiting = true;
                statusBall.Fill = new SolidColorBrush(Colors.Green);
                if (response.Content != null && response.Content.ToString().Equals("Intervalo iniciado."))
                    await Task.Delay(TimeSpan.FromMilliseconds(1400));
                else
                    await Task.Delay(TimeSpan.FromMilliseconds(3200));
                waiting = false;
                statusBall.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                mainInput.Text = "";
                labelAtividade.Text = "";
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                labelAtividade.Text = response?.Content?.ToString();
                waiting = true;
                statusBall.Fill = new SolidColorBrush(Colors.Red);
                await Task.Delay(TimeSpan.FromSeconds(1));
                waiting = false;
                statusBall.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                mainInput.Text = "";
                labelAtividade.Text = "";
            }
            else
            {
                throw new Exception();
            }
        }

        private void SwitchMode()
        {
            switch (mode)
            {
                case ModeEnum.Normal:
                    SetColor(Colors.Yellow);
                    mode = ModeEnum.Intervalo;
                    break;
                case ModeEnum.Intervalo:
                    SetColor(Colors.Red);
                    mode = ModeEnum.Parada;
                    break;
                case ModeEnum.Parada:
                    SetColor(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                    mode = ModeEnum.Normal;
                    break;
                default:
                    SetColor(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                    mode = ModeEnum.Normal;
                    break;
            }
        }

        #region Eventos
        private async void mainInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (waiting)
            {
                e.Handled = true;
                waiting = e.Key != VirtualKey.Enter;
                return;
            }

            switch (e.Key)
            {
                case VirtualKey.NumberKeyLock:
                    //ToDo Tratar quando o usuário desativar o numpad.
                    e.Handled = true;
                    break;
                case VirtualKey.Back:
                    e.Handled = false;
                    break;
                case VirtualKey.Tab:
                    e.Handled = true;
                    break;
                case VirtualKey.Clear:
                    e.Handled = true;
                    keyPress(5);
                    break;
                case VirtualKey.Enter:
                    e.Handled = true;
                        await ExecuteActionAsync();
                    break;
                //case VirtualKey.PageUp:
                //    e.Handled = true;
                //    keyPress(9);
                //    break;
                //case VirtualKey.PageDown:
                //    e.Handled = true;
                //    keyPress(3);
                //    break;
                //case VirtualKey.End:
                //    e.Handled = true;
                //    keyPress(1);
                //    break;
                //case VirtualKey.Home:
                //    e.Handled = true;
                //    keyPress(7);
                //    break;
                //case VirtualKey.Left:
                //    e.Handled = true;
                //    keyPress(4);
                //    break;
                //case VirtualKey.Up:
                //    e.Handled = true;
                //    keyPress(8);
                //    break;
                //case VirtualKey.Right:
                //    e.Handled = true;
                //    keyPress(6);
                //    break;
                //case VirtualKey.Down:
                //    e.Handled = true;
                //    keyPress(2);
                //    break;
                case VirtualKey.Number0:
                case VirtualKey.Number1:
                case VirtualKey.Number2:
                case VirtualKey.Number3:
                case VirtualKey.Number4:
                case VirtualKey.Number5:
                case VirtualKey.Number6:
                case VirtualKey.Number7:
                case VirtualKey.Number8:
                case VirtualKey.Number9:
                case VirtualKey.NumberPad0:
                case VirtualKey.NumberPad1:
                case VirtualKey.NumberPad2:
                case VirtualKey.NumberPad3:
                case VirtualKey.NumberPad4:
                case VirtualKey.NumberPad5:
                case VirtualKey.NumberPad6:
                case VirtualKey.NumberPad7:
                case VirtualKey.NumberPad8:
                case VirtualKey.NumberPad9:
                    e.Handled = false;
                    break;
                //case VirtualKey.Multiply:
                //    break;
                //case VirtualKey.Add:
                //    break;
                //case VirtualKey.Separator:
                //    break;
                //case VirtualKey.Subtract:
                //    break;
                //case VirtualKey.Decimal:
                //    break;
                //case VirtualKey.Divide:
                //    break;
                default:
                    // Ao apertar o botão igual
                    if ((int)e.Key == 187)
                    {
                        e.Handled = true;
                        SwitchMode();
                    }
                    else
                        e.Handled = true;
                    break;
            }
        }

        private void keyPress(int v)
        {
            if (mainInput.Text.Length < mainInput.MaxLength)
            {
                mainInput.Text += v;
            }
        }
        #endregion

        private void mainInput_LostFocus(object sender, RoutedEventArgs e)
        {
            mainInput.Focus(FocusState.Programmatic);
        }
    }


    public enum ModeEnum
    {
        Normal,
        Intervalo,
        Parada,
    }
}
