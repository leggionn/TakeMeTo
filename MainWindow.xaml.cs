using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TakeMeTo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ArrayList list = new ArrayList();
        private Hotkey _hotkey;
        public MainWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            MainTB.Focus();
            AddToList("http://google.com");
            MainTB.VerticalContentAlignment = VerticalAlignment.Center;
        }
        protected override void OnClosed(EventArgs e)
        {
            _hotkey.UnregisterHotkeys();
            base.OnClosed(e);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_KEYMENU = 0xF100;

            if (msg == WM_SYSCOMMAND && (int)wParam == SC_KEYMENU)
            {
                handled = true;
                return IntPtr.Zero;
            }
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == 1)
                {
                    if (this.IsVisible)
                    {
                        this.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.Visibility = Visibility.Visible;
                        this.Activate();
                    }
                }
            }

            return IntPtr.Zero;
        }
        class Hotkey
        {
            [DllImport("user32.dll")]
            private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32.dll")]
            private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            private IntPtr _hWnd;

            public Hotkey(IntPtr hWnd)
            {
                this._hWnd = hWnd;
            }

            public enum fsmodifiers
            {
                None = 0x0000,
                Alt = 0x0001,
                Control = 0x0002,
                Shift = 0x0004,
                Window  = 0x0008
            }

            public void RegisterHotkeys()
            {
                RegisterHotKey(this._hWnd, 1, (uint)fsmodifiers.Alt, (uint)KeyInterop.VirtualKeyFromKey(Key.Space));
            }

            public void UnregisterHotkeys()
            {
                UnregisterHotKey(this._hWnd, 1);
            }
        }

        private void SearchBar_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            _hotkey = new Hotkey(hWnd);
            _hotkey.RegisterHotkeys();

            HwndSource source = HwndSource.FromHwnd(hWnd);
            source.AddHook(WndProc);
        }

        private void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
        {

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void TrayIcon_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

        }
        private void App_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void TrayIcon_LostMouseCapture(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

        }
        private void AddToList(String text)
        {
            Label lbl = new Label();
            Border border = new Border();
            border.Width = 500;
            border.Height = 50;
            border.BorderBrush = null;
            border.Background = new SolidColorBrush(Color.FromRgb(89, 89, 89));
            RootGrid.Children.Add(border);
            lbl.Content = text;
            int top = list.Count * 50 + 50;
            lbl.Margin = new Thickness(34, top + 7, 0, 0);
            border.Margin = new Thickness(0, top, 0, 0);
            this.Height += 50;
            list.Add(lbl);
            MainTB.Margin = new Thickness(34, 0, 0, list.Count * 50);
            MainIco.Margin = new Thickness(10, 16, 372, 16 + list.Count * 50);
            RootGrid.Children.Add(lbl);
            lbl.FontSize = 20;
            lbl.Foreground = Brushes.White;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (MainTB.Text.Length > 0)
                {
                    String MainText = MainTB.Text;
                    if (MainText[0].Equals('/'))
                    {
                        System.Diagnostics.Process.Start("http:/" + MainText);
                    }
                    else if(MainText.Contains("http://")){
                        System.Diagnostics.Process.Start(MainText);
                    }
                }
            }
        }
    }
}
