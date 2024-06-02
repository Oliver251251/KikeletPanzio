using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kikeletPanzio.Osztalyok
{
    class Szoba
    {
        byte szobaszam, ferohely;
        int ar;
        bool foglalt;

        public Szoba(byte szobaszam, byte ferohely, int ar)
        {
            Szobaszam = szobaszam;
            Ferohely = ferohely;
            Ar = ar;
        }

        /// <summary>
        /// Lekérdezi az összes szoba adatait az adatbázisból
        /// </summary>
        /// <param name="connStr">Az adatbázis kapcsolat létrehozásához szükséges parancs</param>
        /// <param name="szobak">Egy lista amibe be fognak töltődni a lekérdezett szobák</param>
        public static void LoadSzobakFromDatabase(string connStr, ref List<Szoba> szobak)
        {
            szobak.Clear();
            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            string sqlCode = "SELECT * FROM szoba";
            MySqlCommand mySqlcmd = new MySqlCommand(sqlCode, connection);
            MySqlDataReader rdr = mySqlcmd.ExecuteReader();
            while (rdr.Read())
            {
                szobak.Add(new Szoba((byte)rdr[0], (byte)rdr[1], (int)rdr[2]));
            }
            rdr.Close();
        }

        /// <summary>
        /// Megállapítja, hogy az adott szoba foglalt-e
        /// </summary>
        /// <param name="szobak">A szobákat tartalmazó lista</param>
        /// <param name="foglaltakList">A foglalásokat tartalmazó lista</param>
        public void IsFoglalt(List<Szoba> szobak, List<Foglalt> foglaltakList)//HIBÁS??
        {
            List<byte> szobaSzamok = szobak.Select(x => x.szobaszam).ToList();
            foreach (Foglalt egyFoglalt in foglaltakList)
            {
                if (szobaSzamok.Contains(egyFoglalt.Szobaszam))
                {
                    foglalt = true;
                    return;
                }
            }
        }

        #region Mezők
        public byte Szobaszam { get => szobaszam; set => szobaszam = value; }
        public byte Ferohely { get => ferohely; set => ferohely = value; }
        public int Ar { get => ar; set => ar = value; }
        public bool Foglalt { get => foglalt; set => foglalt = value; }
        #endregion
    }
}
