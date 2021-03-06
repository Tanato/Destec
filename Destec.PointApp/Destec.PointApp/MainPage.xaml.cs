﻿using System;
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
using Windows.Storage;

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
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        int shutdownCount;
        bool timetosleep;

        string ajudaFuncionarioCode;

        bool waiting = false;

        public MainPage()
        {
            this.InitializeComponent();

            try
            {
                server = localSettings.Values["server"].ToString();
            }
            catch { }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                localSettings.Values["server"] = e.Parameter.ToString();
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

        private async Task ExecuteActionAsync()
        {
            if (string.IsNullOrEmpty(mainInput.Text) || mainInput.Text.Length != 3)
            {
                ResetView(string.IsNullOrEmpty(ajudaFuncionarioCode) ? "Código Inválido!" : string.Empty);
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
                            case ModeEnum.Ajuda:
                                {
                                    if (!string.IsNullOrEmpty(ajudaFuncionarioCode))
                                        response = await http.PostAsync(new Uri(urlBase + server + ":5000/api/atividade/help?code=" + mainInput.Text + "&ajudaCode=" + ajudaFuncionarioCode), null);
                                    else
                                        response = await http.GetAsync(new Uri(urlBase + server + ":5000/api/atividade/help?code=" + mainInput.Text));
                                }
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
                    ResetView("Erro ao executar requisição de Atividade");
                }
            }
        }

        private async Task HandleResponseAsync(HttpResponseMessage response)
        {
            if (response != null)
            {
                SetViewResponse(response?.Content?.ToString(), response?.StatusCode == HttpStatusCode.Ok);
                await WaitShowAsync(response);
                ResetView();
            }
            else
            {
                throw new Exception();
            }
        }

        private async Task WaitShowAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Ok)
            {
                if (response.Content != null && response.Content.ToString().Equals("Intervalo iniciado."))
                {
                    // Fluxo de intervalo
                    await Task.Delay(TimeSpan.FromMilliseconds(1400));
                }
                else if (mode == ModeEnum.Ajuda) 
                {
                    // Fluxo de ajuda, aguarda mais tempo para inserção de ajudante
                    waiting = false;
                    ajudaFuncionarioCode = mainInput.Text;
                    mainInput.Text = string.Empty;
                    await Task.Delay(TimeSpan.FromMilliseconds(5000));
                }
                else
                {
                    // Fluxo padrão de execução
                    await Task.Delay(TimeSpan.FromMilliseconds(2500));
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void SetViewResponse(string message, bool isOk = true)
        {
            waiting = true;
            labelTarefa.Visibility = isOk ? Visibility.Visible : Visibility.Collapsed;
            labelAtividade.Text = message;
            statusBall.Fill = new SolidColorBrush(isOk ? Colors.Green : Colors.Red);
        }

        private void ResetView(string message = "")
        {
            waiting = false;
            ajudaFuncionarioCode = string.Empty;
            statusBall.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            mainInput.Text = "";
            labelTarefa.Visibility = Visibility.Collapsed;
            labelAtividade.Text = message;
            SetMode(ModeEnum.Normal);
        }

        private void SwitchMode(ModeEnum? value = null)
        {
            switch (mode)
            {
                case ModeEnum.Normal:
                    SetMode(ModeEnum.Intervalo);
                    break;
                case ModeEnum.Intervalo:
                    SetMode(ModeEnum.Parada);
                    break;
                case ModeEnum.Parada:
                    SetMode(ModeEnum.Ajuda);
                    break;
                case ModeEnum.Ajuda:
                    SetMode(ModeEnum.Normal);
                    break;
                default:
                    SetMode(ModeEnum.Normal);
                    break;
            }
        }

        private void SetMode(ModeEnum mode)
        {
            this.mode = mode;
            switch (mode)
            {
                case ModeEnum.Normal:
                    SetColor(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                    break;
                case ModeEnum.Intervalo:
                    SetColor(Colors.Yellow);
                    break;
                case ModeEnum.Parada:
                    SetColor(Colors.Red);
                    break;
                case ModeEnum.Ajuda:
                    SetColor(Colors.DeepSkyBlue);
                    break;
                default:
                    SetColor(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                    mode = ModeEnum.Normal;
                    break;
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

        #region Eventos
        private async void mainInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (timetosleep && e.Key != VirtualKey.Subtract)
            {
                await CancelShutdown(e);
                return;
            }
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
                case VirtualKey.Subtract:
                    e.Handled = true;
                    shutdown();
                    break;
                //case VirtualKey.Decimal:
                //    break;
                case VirtualKey.Divide:
                    e.Handled = true;
                    SwitchMode();
                    break;
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

        private async Task CancelShutdown(KeyRoutedEventArgs e)
        {
            e.Handled = true;
            timetosleep = false;
            shutdownCount = 0;
            ShutdownManager.CancelShutdown();
            labelAtividade.Text = "Desligamento cancelado.";
            await Task.Delay(TimeSpan.FromSeconds(1));
            labelAtividade.Text = string.Empty;
            return;
        }

        private void shutdown()
        {
            shutdownCount++;
            if (shutdownCount >= 5 && !timetosleep)
            {
                try
                {
                    labelAtividade.Text = $"Desligando.{Environment.NewLine}Pressione para cancelar.";
                    timetosleep = true;
                    ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(5));
                }
                catch
                {
                    labelAtividade.Text = "";
                    timetosleep = false;
                }
            }
        }

        private void keyPress(int v)
        {
            if (mainInput.Text.Length < mainInput.MaxLength)
            {
                mainInput.Text += v;
            }
        }

        private void mainInput_LostFocus(object sender, RoutedEventArgs e)
        {
            mainInput.Focus(FocusState.Programmatic);
        }
    }

    public enum ModeEnum
    {
        Normal,
        Ajuda,
        Intervalo,
        Parada,
    }
}
