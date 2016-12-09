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
        string server = "localhost";

        public MainPage()
        {
            this.InitializeComponent();

            //Ping().GetAwaiter().GetResult();
        }

        private async Task Ping()
        {
            var response = await httpClient.GetAsync(new Uri(server + ":5000/ping/" + mainInput.Text));
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {

            }
        }

        private void SetColor(Color color)
        {
            this.modeBorderLeft.Fill = new SolidColorBrush(color);
            this.modeBorderBottom.Fill = new SolidColorBrush(color);
            this.modeBorderRight.Fill = new SolidColorBrush(color);
            this.modeBorderTop.Fill = new SolidColorBrush(color);

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
                    switch (mode)
                    {
                        case ModeEnum.Normal:
                            response = await httpClient.GetAsync(new Uri(server + ":5000/execute/" + mainInput.Text));
                            break;
                        case ModeEnum.Intervalo:
                            response = await httpClient.GetAsync(new Uri(server + ":5000/interval/" + mainInput.Text));
                            break;
                        case ModeEnum.Parada:
                            response = await httpClient.GetAsync(new Uri(server + ":5000/stop/" + mainInput.Text));
                            break;
                        default:
                            break;
                    }
                    handleResponse(response);
                }
                catch (Exception)
                {
                    labelAtividade.Text = "Erro ao executar requisição de Atividade";
                    mainInput.Text = string.Empty;
                }
            }
        }

        private static void handleResponse(HttpResponseMessage response)
        {
            if (response != null)
            {

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
                    SetColor(Color.FromArgb(0xFF,0xF0, 0xF0, 0xF0));
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
                    //e.Key = VirtualKey.NumberPad5;
                    break;
                case VirtualKey.Enter:
                    await ExecuteActionAsync();
                    e.Handled = true;
                    break;
                //case VirtualKey.PageUp:
                //    break;
                //case VirtualKey.PageDown:
                //    break;
                //case VirtualKey.End:
                //    break;
                //case VirtualKey.Home:
                //    break;
                //case VirtualKey.Left:
                //    break;
                //case VirtualKey.Up:
                //    break;
                //case VirtualKey.Right:
                //    break;
                //case VirtualKey.Down:
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
