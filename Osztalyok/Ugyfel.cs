using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kikeletPanzio.Osztalyok
{
    class Ugyfel
    {
        string azon, nev;
        string? email;
        bool vip;
        DateTime szulDatum, regisztracioDatum;

        public Ugyfel(string azon, string nev, DateTime szulDatum, string email, bool vip, DateTime regisztracioDatum)
        {
            Azon = azon;
            Nev = nev;
            SzulDatum = szulDatum;
            Email = email;
            Vip = vip;
            RegisztracioDatum = regisztracioDatum;
        }

        public Ugyfel()
        {
            azon = "";
            nev = "";
            email = null;
            vip = false;
            szulDatum = DateTime.Today;
            regisztracioDatum = DateTime.Today;
        }

        /// <summary>
        /// Lekérdezi az összes ügyfél adatait az adatbázisból
        /// </summary>
        /// <param name="connStr">Az adatbázis kapcsolat létrehozásához szükséges parancs</param>
        /// <param name="ugyfelek">Egy lista amibe be fognak töltődni a lekérdezett ügyfelek</param>
        public static void LoadUgyfelekFromDatabase(string connStr, ref List<Ugyfel> ugyfelek)
        {
            ugyfelek.Clear();
            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            string sqlCode = "SELECT * FROM ugyfel";
            MySqlCommand mySqlcmd = new MySqlCommand(sqlCode, connection);
            MySqlDataReader rdr = mySqlcmd.ExecuteReader();
            while (rdr.Read())
            {
                ugyfelek.Add(new Ugyfel(rdr[0].ToString(), rdr[1].ToString(), (DateTime)rdr[2], rdr[3].ToString(), (bool)rdr[4], (DateTime)rdr[5]));
            }
            rdr.Close();
        }

        /// <summary>
        /// Új ügyfél felvitele
        /// </summary>
        /// <param name="connStr">Az adatbázis kapcsolat létrehozásához szükséges parancs</param>
        /// <param name="ugyfel">Az 'Ugyfel' osztály egy példánya akit felkívánunk tölteni az adatbázisba</param>
        public static void InsertUgyfelekDatabase(string connStr, Ugyfel ugyfel)
        {
            MySqlConnection conn = new MySqlConnection(connStr);//MEGFELELŐ MEZŐNEVEK KIALAKÍTÁSA
            conn.Open();
            string sql = @$"INSERT INTO ugyfel(id,nev,szuletesi_datum,email,vip,regisztracio_datum) VALUES('{ugyfel.azon}','{ugyfel.nev}', '{ugyfel.szulDatum.ToString("yyyy-MM-dd")}','{ugyfel.email}', '{ugyfel.vip}' , '{ugyfel.regisztracioDatum.ToString("yyyy-MM-dd")}')";
            MySqlCommand mySqlCommand = new MySqlCommand(sql, conn);
            mySqlCommand.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// Azonosító létrehozása. Csak akkor futtatandó ha a név mező kitöltésre került!
        /// </summary>
        public void CreateAzon()
        {
            Azon = nev.Replace(" ", "") + DateTime.Now.ToString("yyyy-MM-dd");
        }

        public override string ToString()
        {
            string vipString = vip ? "Igen" : "Nem";
            string emailString = email == null ? "--" : email;
            return $"Azonosító (automatikusan generált): {Azon}\nNév: {Nev}\nSzületési dátum: {SzulDatum.ToString("yyyy-MM-dd")}\nEmail (opcionális ha nem VIP): {emailString}\nVip: {vipString}";
        }

        #region Mezők
        public string Azon { get => azon; private set => azon = value; }
        public string Nev { get => nev; set => nev = value; }
        public DateTime SzulDatum { get => szulDatum; set => szulDatum = value; }
        public string? Email { get => email; set => email = value; }
        public bool Vip { get => vip; set => vip = value; }
        public DateTime RegisztracioDatum { get => regisztracioDatum; set => regisztracioDatum = value; }
        #endregion
    }
}
