using kikeletPanzio.Osztalyok;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace kikeletPanzio
{
    /// <summary>
    /// Interaction logic for UjUgyfelFelvitel.xaml
    /// </summary>
    public partial class UjUgyfelFelvitel : Page
    {
        string nev, jelszo, connString;
        Ugyfel ujUgyfel = new Ugyfel();

        public UjUgyfelFelvitel(string nev, string jelszo)
        {
            //throw new Exception("VIP binding ellenőrzése!!!!!");
            InitializeComponent();
            this.nev = nev;
            this.jelszo = jelszo;
            connString = @$"server=localhost;user={nev};password={jelszo};database=hotel";

            containerGrid.DataContext = ujUgyfel;
        }

        private void VisszaMainMenu_Click(object sender, RoutedEventArgs e)
        {
            ujUgyfelFrame.Navigate(new MainPage(nev, jelszo));
        }

        private void UjSzobaFoglalas_Click(object sender, RoutedEventArgs e)
        {
            ujUgyfelFrame.Navigate(new UjSzobaFoglalasPage(nev, jelszo));
        }

        private void AdatokRögzítése_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ujUgyfel.Vip && ujUgyfel.Email == null)
                {
                    throw new Exception("A VIP ügyfeleknek meg kell adniuk az email címüket!");
                }

                DatumHelyes(ujUgyfel.SzulDatum);
                NevHelyes(ujUgyfel.Nev);
                EmailHelyes(ujUgyfel.Email, ujUgyfel.Vip);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba! {ex}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Helyesek az adatok?\n{ujUgyfel.ToString()}", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            Ugyfel.InsertUgyfelekDatabase(connString, ujUgyfel);
            MessageBox.Show("Sikeres regisztráció!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NevHelyes(string nev)
        {
            Regex regex = new Regex(@"^[A-ZÁÉ][a-záé]+( [A-ZÁÉ][a-záé]+)+$");

            if (!regex.IsMatch(nev))
            {
                throw new Exception("Helytelen a név beviteli formátuma!\nPélda: Példa János");
            }
        }

        private void TextBoxNev_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                ujUgyfel.CreateAzon();
                tbxAzon.Text = ujUgyfel.Azon;
            }
        }

        private void EmailHelyes(string email, bool VIP)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            if (email == null && VIP)
            {
                throw new Exception("VIP ügyfél esetén az email cím megadása KÖTELEZŐ!");
            }

            if (email == null)
            {
                return;
            }

            if (!regex.IsMatch(email))
            {
                throw new Exception("Helytelen az email cím beviteli formátuma!\nPélda: Példa János");
            }
        }

        private void DatumHelyes(DateTime datum)
        {
            if (DateTime.Now.Year - 18 < datum.Year)
            {
                throw new Exception("Hibás születési dátum! Csak nagykorúak regisztrálhatnak!");
            }
        }
    }
}
