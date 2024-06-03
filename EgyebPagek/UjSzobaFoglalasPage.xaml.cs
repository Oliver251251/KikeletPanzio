using kikeletPanzio.Osztalyok;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace kikeletPanzio
{
    /// <summary>
    /// Interaction logic for UjSzobaFoglalasPage.xaml
    /// </summary>
    public partial class UjSzobaFoglalasPage : Page
    {
        string nev, jelszo, connString;
        List<Szoba> szobak = new List<Szoba>();
        List<Ugyfel> ugyfelek = new List<Ugyfel>();
        List<Foglalt> foglaltak = new List<Foglalt>();
        Foglalt ujFoglal = new Foglalt();

        public UjSzobaFoglalasPage(string nev, string jelszo)
        {
            InitializeComponent();
            this.nev = nev;
            this.jelszo = jelszo;
            connString = @$"server=localhost;user={nev};password={jelszo};database=hotel";
            try
            {
                Setup();
            }
            catch (Exception)
            {
                MessageBox.Show($"Hiba történt a csatlakozás során, ellenőrizze internetkapcsolatát!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Setup()
        {
            GetDatasFromDatabase();

            for (byte i = 1; i < 7; i++)
            {
                cbSzobaszam.Items.Add(i);
            }

            ugyfelek.ForEach(x => cbUgyfelID.Items.Add(x.Azon));
            szobak.ForEach(x => x.IsFoglalt(szobak, foglaltak));//NEM JÓ????????

            dgSzobak.ItemsSource = szobak;
            dgSzobak.SelectedIndex = 0;
            spkUjFoglalasAdatok.DataContext = ujFoglal;
        }

        private void GetDatasFromDatabase()
        {
            Szoba.LoadSzobakFromDatabase(connString, ref szobak);
            Ugyfel.LoadUgyfelekFromDatabase(connString, ref ugyfelek);
            Foglalt.LoadFoglaltFromDatabase(connString, ref foglaltak);
        }

        private void dgSzobak_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;//ROVIDITENI?!
            DataContext = dataGrid.SelectedValue;
            SzobaFoglaltIdopontok(dataGrid.SelectedValue as Szoba, foglaltak);
            tbFizetendo.Text = "";
        }

        private void SzobaFoglaltIdopontok(Szoba szoba, List<Foglalt> foglalts)
        {
            foreach (Foglalt foglalt in foglalts)
            {
                if (foglalt.Szobaszam == szoba.Szobaszam)
                {
                    dgFoglaltDate.ItemsSource = foglalts.Where(x => x.Szobaszam == szoba.Szobaszam).ToList();
                    return;
                }
            }

            dgFoglaltDate.ItemsSource = new List<char>();
        }
        private void tbFo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Szoba valasztott = szobak[cbSzobaszam.SelectedIndex];

            int a;
            if (!int.TryParse(e.Text, out a))
            {
                MessageBox.Show("Csak számot adjon meg!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                return;
            }

            if (valasztott.Ferohely < a || valasztott.Ferohely < int.Parse(((sender as TextBox).Text + e.Text)) || a == 0)
            {
                MessageBox.Show($"Az elszállásolni kívánt fők nem lehetnek többen mint a szoba kapacitása, illetve nagyobbnak kell lennie mint 0! ({valasztott.Ferohely} fő)", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                return;
            }

            tbFizetendo.Text = ugyfelek.Where(x => x.Azon == cbUgyfelID.SelectedItem.ToString()).ToList()[0].Vip ? $"{(valasztott.Ar * int.Parse(e.Text))} Ft" : "";
        }

        private void TbUgyfelID_Keres_TextInput(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text + e.Key == "")
            {
                ugyfelek.ForEach(x => cbUgyfelID.Items.Add(x.Azon));
                cbUgyfelID.IsDropDownOpen = true;
                return;
            }

            string keres = textBox.Text + e.Key;
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                keres = textBox.Text.Substring(0, textBox.Text.Length - 1);
            }

            List<string> hasonlit = new List<string>();
            cbUgyfelID.Items.Clear();

            foreach (Ugyfel ugyfel in ugyfelek)
            {
                if (ugyfel.Azon.ToLower().Contains((keres).ToLower()))
                {
                    hasonlit.Add(ugyfel.Azon);
                }
            }

            hasonlit.ForEach(x => cbUgyfelID.Items.Add(x));
            cbUgyfelID.IsDropDownOpen = true;
        }

        private void AdatokRogzitese_click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdatokEllenőrzése();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Foglalt.InsertFoglalasToDatabase(connString, ujFoglal);
            Foglalt.LoadFoglaltFromDatabase(connString, ref foglaltak);
            SzobaFoglaltIdopontok(dgSzobak.SelectedValue as Szoba, foglaltak);
        }

        /// <summary>
        /// Adatok ellenőrzése, hogy ne legyen probléma feltöltéskor
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void AdatokEllenőrzése()
        {
            if (dgSzobak.SelectedValue == null)
            {
                throw new Exception("Válassza ki a lefoglalni kívánt szobát!");
            }

            if (cbUgyfelID.SelectedValue == null)
            {
                throw new Exception("Válasszon ki egy ügyfél azonosítót!");
            }

            if (ujFoglal.FoglalasKezdetDate > ujFoglal.FoglalasVegeDate)
            {
                throw new Exception("A kezdési dátum nem lehet későbbi mint a befejezési!");
            }

            List<Foglalt> foglaltIdopontok = foglaltak
                .Where(x => x.Szobaszam == ujFoglal.Szobaszam &&
                            ((ujFoglal.FoglalasKezdetDate >= x.FoglalasKezdetDate && ujFoglal.FoglalasKezdetDate <= x.FoglalasVegeDate) ||
                             (ujFoglal.FoglalasVegeDate >= x.FoglalasKezdetDate && ujFoglal.FoglalasVegeDate <= x.FoglalasVegeDate)))
                .ToList();

            if (foglaltIdopontok.Any())
            {
                throw new Exception("A szoba a megadott időintervallumban le van foglalva!");
            }

        }

        private void cbSzobaszam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgSzobak.SelectedIndex = (sender as ComboBox).SelectedIndex;
        }

        private void Vissza_A_Menube(object sender, RoutedEventArgs e)
        {
            ujszobaFoglalasFrame.Navigate(new MainPage(nev, jelszo));
        }
    }
}
