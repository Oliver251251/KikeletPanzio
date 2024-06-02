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

namespace kikeletPanzio
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        string nev, jelszo;
        public MainPage(string nev, string jelszo)
        {
            InitializeComponent();
            this.nev = nev;
            this.jelszo = jelszo;
        }

        private void UjUgyfelRegisztralas_Click(object sender, RoutedEventArgs e)
        {
            mainWindowFrame.Navigate(new UjUgyfelFelvitel(nev, jelszo));
        }

        private void Szobafoglalas_click(object sender, RoutedEventArgs e)
        {
            mainWindowFrame.Navigate(new UjSzobaFoglalasPage(nev, jelszo));
        }
    }
}
