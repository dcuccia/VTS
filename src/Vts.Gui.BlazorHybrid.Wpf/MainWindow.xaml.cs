using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Vts.Gui.BlazorHybrid.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Resources.Add("services", new ServiceCollection().AddBlazorWebView().BuildServiceProvider());

            InitializeComponent();
        }
    }
}
