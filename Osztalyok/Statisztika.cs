using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace kikeletPanzio.Osztalyok
{
    internal class Statisztika
    {
        public static string LegtobbetKiadottSzobaFunc(DateTime StartDate, DateTime EndDate, string connStr)
        {

            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            string sqlCode = @$"SELECT 
                                    f.szobaszam,
                                    COUNT(*) AS usage_count
                                FROM 
                                    foglal f
                                WHERE 
                                    f.foglalas_kezdete <= @EndDate AND f.foglalas_vege >= @StartDate
                                GROUP BY 
                                    f.szobaszam
                                ORDER BY 
                                    usage_count DESC
                                LIMIT 1;";

            MySqlCommand mySqlcmd = new MySqlCommand(sqlCode, connection);
            mySqlcmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy-MM-dd"));
            mySqlcmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy-MM-dd"));

            MySqlDataReader rdr = mySqlcmd.ExecuteReader();

            int szoba = 0;
            if (rdr.Read() && rdr["szobaszam"] != DBNull.Value)
            {
                szoba = rdr.GetInt32("szobaszam");
            }

            rdr.Close();

            return szoba.ToString();
        }

        public static string OsszbevetelIntervallum(DateTime StartDate, DateTime EndDate, string connStr)
        {

            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            string sqlCode = @$"
                SELECT 
                    SUM(CASE 
                            WHEN u.VIP = 1 THEN f.letszam * s.ar * 0.97
                            ELSE f.letszam * s.ar
                        END) AS total_amount
                FROM 
                    foglal f
                JOIN 
                    ugyfel u ON f.ugyfel_id = u.id
                JOIN 
                    szoba s ON f.szobaszam = s.szobaszam
                WHERE 
                    f.foglalas_kezdete <= @EndDate AND f.foglalas_vege >= @StartDate";

            MySqlCommand mySqlcmd = new MySqlCommand(sqlCode, connection);
            mySqlcmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy-MM-dd"));
            mySqlcmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy-MM-dd"));

            MySqlDataReader rdr = mySqlcmd.ExecuteReader();

            decimal ossz = 0;
            if (rdr.Read() && rdr["total_amount"] != DBNull.Value)
            {
                ossz = rdr.GetDecimal("total_amount");
            }

            rdr.Close();
            connection.Close();

            return $"{ossz}Ft";
        }

        public static Dictionary<string, decimal> VisszaJaroVendegek(DateTime StartDate, DateTime EndDate, string connStr)
        {
            Dictionary<string, decimal> visszaterok = new Dictionary<string, decimal>();
            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            string sqlCode = @$"SELECT 
                            f.ugyfel_id,
                            SUM(CASE 
                                    WHEN u.VIP = 1 THEN f.letszam * s.ar * 0.97
                                    ELSE f.letszam * s.ar
                                END) AS total_spent
                            FROM 
                                foglal f
                            JOIN 
                                ugyfel u ON f.ugyfel_id = u.id
                            JOIN 
                                szoba s ON f.szobaszam = s.szobaszam
                            WHERE 
                                f.foglalas_kezdete <= @EndDate AND f.foglalas_vege >= @StartDate
                            GROUP BY 
                                f.ugyfel_id
                            HAVING 
                                COUNT(f.ugyfel_id) > 1
                            ORDER BY 
                                total_spent DESC;";

            MySqlCommand mySqlcmd = new MySqlCommand(sqlCode, connection);
            mySqlcmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy-MM-dd"));
            mySqlcmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy-MM-dd"));

            MySqlDataReader rdr = mySqlcmd.ExecuteReader();

            int run = 0;
            while (rdr.Read() && rdr["ugyfel_id"] != DBNull.Value)
            {
                visszaterok.Add(rdr[0].ToString(), (decimal)rdr[1]);
                run++;
            }

            rdr.Close();
            return visszaterok;
        }
    }
}
