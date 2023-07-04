using AutoBot.Services;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoBot.Views
{
    /// <summary>
    /// Interaction logic for TransparentWindow.xaml
    /// </summary>
    public partial class TransparentWindow : Window
    {
        ImageBrush brush;
        ScreenCaptureService _svn;
        PeriodicTimer t;
        EventBus _bus;
        public TransparentWindow() : this(null, null)
        {
            
        }
        public TransparentWindow(EventBus bus, ScreenCaptureService svn)
        {
            
            if (bus == null)
            {
                return;
            }
            InitializeComponent();
            brush = new ImageBrush();
            _bus = bus;
            _svn = svn;
           
            _bus.Subscribe<TestEvent>(this, Callback);
        }

        private void Callback()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                brush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(_svn.Image.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions());
                Background = brush;
            }));
        }
    }
}
