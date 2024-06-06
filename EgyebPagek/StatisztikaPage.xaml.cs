using kikeletPanzio.Osztalyok;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace kikeletPanzio.EgyebPagek
{
    /// <summary>
    /// Interaction logic for StatisztikaPage.xaml
    /// </summary>
    public partial class StatisztikaPage : Page
    {
        string nev, jelszo, connString;
        List<Szoba> szobak = new List<Szoba>();
        List<Ugyfel> ugyfelek = new List<Ugyfel>();
        List<Foglalt> foglaltak = new List<Foglalt>();        

        public StatisztikaPage(string nev, string jelszo)
        {
            InitializeComponent();
            this.nev = nev;
            this.jelszo = jelszo;
            connString = @$"server=localhost;user={nev};password={jelszo};database=hotel";
            GetDatasFromDatabase();
        }

        private void GetDatasFromDatabase()
        {
            Szoba.LoadSzobakFromDatabase(connString, ref szobak);
            Ugyfel.LoadUgyfelekFromDatabase(connString, ref ugyfelek);
            Foglalt.LoadFoglaltFromDatabase(connString, ref foglaltak);
        }

        private void dpSelectedDateChanged_ShowDatas(object sender, SelectionChangedEventArgs e)
        {
            if (dpKezdet.SelectedDate == null || dpVeg.SelectedDate == null)
            {
                return;
            }

            tbLegtobbSzoba.Text = Statisztika.LegtobbetKiadottSzobaFunc(dpKezdet.SelectedDate.Value,dpVeg.SelectedDate.Value,connString);
            tbBevetel.Text = Statisztika.OsszbevetelIntervallum(dpKezdet.SelectedDate.Value, dpVeg.SelectedDate.Value, connString);
            dgVendekeg.ItemsSource = Statisztika.VisszaJaroVendegek(dpKezdet.SelectedDate.Value, dpVeg.SelectedDate.Value, connString);
        }

        private void VisszaMainMenu_Click(object sender, RoutedEventArgs e)
        {
            StatisztikaFrame.Navigate(new MainPage(nev, jelszo));
        }
    }
}
