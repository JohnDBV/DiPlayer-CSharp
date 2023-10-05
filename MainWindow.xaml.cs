using DiPlayer_CSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiPlayer_CSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Add this event handler(which verifies handled events too), on the "playbackSlider" only :
            playbackSlider.AddHandler(PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(playbackSlider_PreviewMouseLeftButtonDown),
                true);

            //ImageSourceConverter conv = new ImageSourceConverter();
            //conv.ConvertFrom(new System.Drawing.Bitmap(""));
           
        }

        private void playbackSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Seek through the song using the ViewModel command
            (DataContext as MainViewModel)?.OnSeekTrack((sender as Slider)?.Value);
        }

        //private void mainListView_MouseDown(object sender, MouseButtonEventArgs e)
        //{//it seems the ListView refuses to ignore only the CLick event
        //    if (e.ClickCount < 2)
        //        e.Handled = true;
        //}

    }
}
