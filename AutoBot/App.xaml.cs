using AutoBot.Services;
using AutoBot.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<EventBus>();
            serviceCollection.AddSingleton<TransparentWindow>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<ScreenCaptureService>();

            var provider = serviceCollection.BuildServiceProvider();
            MainWindow = provider.GetService<MainWindow>();
            MainWindow.Show();
        }
    }
}
