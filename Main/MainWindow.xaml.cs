using MySqlConnector;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string jelszo;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Bejelentkez_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Login())
            {
                MessageBox.Show("Sikeres bejelentkezés!");
                mainWindowFrame.Navigate(new MainPage("root", tbJelszo.Text));//teszt után visszarakni
                return;
            }
        }

        private void tbJelszo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            jelszo += e.Text;
            e.Handled = true;
            textBox.Text += "*";
            textBox.CaretIndex = textBox.Text.Length;
        }

        private bool Login()
        {
            return true;//VÉGSŐ TERMÉKBŐL KISZEDNI!!!!!!!!!!
            try
            {
                string connStr = @$"server=localhost;user={"root"};password={tbJelszo.Text};database=hotel";//teszt után visszarakni
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Nem sikerült csatlakozni az adatbázishoz!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
