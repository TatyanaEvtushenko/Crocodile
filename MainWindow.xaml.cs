using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ClassLibrary;

namespace Crocodile
{
    public partial class MainWindow : Window
    {
        public Point LastPoint { get; set; }

        private Purpose purpose;
        private Client client;
        private bool isPaint;

        public MainWindow()
        {
            InitializeComponent();
            GridMain.Visibility = Visibility.Hidden;
            GridRegister.Visibility = Visibility.Visible;
        }

        private void Enter(object sender, RoutedEventArgs e)
        {
            client = new Client(this, TextBoxUsername.Text);
            client.SendEnter();
            GridMain.Visibility = Visibility.Visible;
            GridRegister.Visibility = Visibility.Hidden;
        }

        private void ChangeParameters(object sender, TextChangedEventArgs e)
        {
            ButtonEnter.IsEnabled = TextBoxUsername.Text != "";
        }

        public void SetPurpose()
        {
            
        }

        public void StartPlay()
        {
            
        }

        public void PrintStr(string str)
        {
            Dispatcher.Invoke(() =>
            {
                TextBoxMessages.Text += str + "\n";
            });
        }

        public void PrintPoint(Point point)
        {
            Dispatcher.Invoke(() =>
            {
                var line = new Line { X1 = LastPoint.X, Y1 = LastPoint.Y, X2 = point.X, Y2 = point.Y, Stroke = new SolidColorBrush(Colors.Black) };
                CanvasPaint.Children.Add(line);
                LastPoint = point;
            });
        }

        private void SendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && TextBoxEnter.Text != "")
            {
                client.SendMessage(TextBoxEnter.Text);
                TextBoxEnter.Text = "";
            }
        }

        private void SendPoint(object sender, MouseEventArgs e)
        {
            if (isPaint)
            {
                var point = e.GetPosition((Canvas)sender);
                client.SendPoint(point);
            }
        }

        private void BeginPaint(object sender, MouseButtonEventArgs e)
        {
            isPaint = true;
            var point = e.GetPosition((Canvas)sender);
            client.SendBeginPaint(point);
        }

        private void EndPaint(object sender, MouseButtonEventArgs e)
        {
            isPaint = false;
        }

        private void Exit(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                    client.SendLogOff();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "ERROR");
            }
        }
    }
}
