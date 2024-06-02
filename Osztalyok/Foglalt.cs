using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kikeletPanzio.Osztalyok
{
    class Foglalt
    {
        int? foglalID;
        byte szobaszam, letszam;
        string ugyfelAzon;
        DateTime foglalasKezdetDate, foglalasVegeDate;

        public Foglalt(int? foglalID, byte szobaszam, string ugyfelAzon, byte letszam, DateTime foglalasKezdetDate, DateTime foglalasVegeDate)
        {
            FoglalID = foglalID;
            Letszam = letszam;
            Szobaszam = szobaszam;
            UgyfelAzon = ugyfelAzon;
            FoglalasKezdetDate = foglalasKezdetDate;
            FoglalasVegeDate = foglalasVegeDate;
        }

        public Foglalt()
        {
            ugyfelAzon = "-";
            szobaszam = 1;
            foglalasKezdetDate = DateTime.Today;
            foglalasVegeDate = DateTime.Today;
        }

        /// <summary>
        /// Lekérdezi az összes lefoglalt szoba adatait az adatbázisból
        /// </summary>
        /// <param name="connStr">Az adatbázis kapcsolat létrehozásához szükséges parancs</param>
        /// <param name="foglalt">Egy lista amibe be fognak töltődni a lekérdezett foglalások</param>
        public static void LoadFoglaltFromDatabase(string connStr, ref List<Foglalt> foglalt)
        {
            foglalt.Clear();
            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            string sqlCode = "SELECT * FROM foglal";
            MySqlCommand mySqlcmd = new MySqlCommand(sqlCode, connection);
            MySqlDataReader rdr = mySqlcmd.ExecuteReader();
            while (rdr.Read())
            {
                foglalt.Add(new Foglalt((int)rdr[0], (byte)rdr[1], rdr[2].ToString(), (byte)rdr[3], (DateTime)rdr[4], (DateTime)rdr[5]));
            }
            rdr.Close();
        }

        /// <summary>
        /// Új foglalás felvitele
        /// </summary>
        /// <param name="connStr">Az adatbázis kapcsolat létrehozásához szükséges parancs</param>
        /// <param name="foglalt">Az 'Foglalt' osztály egy példánya amit felkívánunk tölteni az adatbázisba</param>
        public static void InsertFoglalasToDatabase(string connStr, Foglalt foglalt)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = @$"INSERT INTO foglal(szobaszam, ugyfel_id, letszam, foglalas_kezdete, foglalas_vege) VALUES({foglalt.Szobaszam},'{foglalt.ugyfelAzon}', {foglalt.letszam},
                         '{foglalt.foglalasKezdetDate.ToString("yyyy-MM-dd")}', '{foglalt.foglalasVegeDate.ToString("yyyy-MM-dd")}')";
            MySqlCommand mySqlCommand = new MySqlCommand(sql, conn);
            mySqlCommand.ExecuteNonQuery();
            conn.Close();
        }

        #region Mezők
        public int? FoglalID { get => foglalID; set => foglalID = value; }
        public byte Szobaszam { get => szobaszam; set => szobaszam = value; }
        public byte Letszam { get => letszam; set => letszam = value; }
        public string UgyfelAzon { get => ugyfelAzon; set => ugyfelAzon = value; }
        public DateTime FoglalasKezdetDate { get => foglalasKezdetDate; set => foglalasKezdetDate = value; }
        public DateTime FoglalasVegeDate { get => foglalasVegeDate; set => foglalasVegeDate = value; }
        #endregion
    }
}
