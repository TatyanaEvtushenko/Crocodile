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
        private Brush color = new SolidColorBrush(Colors.Black);

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

        public void PrintPoint(double x, double y)
        {
            Dispatcher.Invoke(() =>
            {
                var line = new Line { X1 = LastPoint.X, Y1 = LastPoint.Y, X2 = x, Y2 = y, Stroke = color };
                CanvasPaint.Children.Add(line);
                LastPoint = new Point(x, y);
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
                client.SendPoint(point.X, point.Y);
            }
        }

        private void BeginPaint(object sender, MouseButtonEventArgs e)
        {
            isPaint = true;
            var point = e.GetPosition((Canvas)sender);
            client.SendBeginPaint(point.X, point.Y);
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
