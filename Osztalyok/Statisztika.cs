using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace kikeletPanzio.Osztalyok
{
    internal class Statisztika
    {
        private  Dictionary<int, int> szobaFoglaltDarab;
        int legtobbetKiadottSzoba;

        public Statisztika(){}

        private  List<Foglalt> DatumEgyezik(DateTime kezdet, DateTime veg, Foglalt foglalt, List<Foglalt> foglaltak)
        {
            return foglaltak
               .Where(x => x.Szobaszam == foglalt.Szobaszam &&
                           ((kezdet >= x.FoglalasKezdetDate && veg <= x.FoglalasVegeDate) ||
                            (veg >= x.FoglalasKezdetDate && kezdet <= x.FoglalasVegeDate)))
               .ToList();
        }

        public void LegtobbetKiadottSzobaFunc(DateTime kezdet, DateTime veg, List<Foglalt> foglaltak)
        {
            Dictionary<int, int> kiadottSzobak = new Dictionary<int, int>();
            foreach (Foglalt foglalt in foglaltak)
            {
                List<Foglalt> foglaltIdopontok = DatumEgyezik(kezdet, veg, foglalt, foglaltak);
                if (!kiadottSzobak.ContainsKey(foglalt.Szobaszam) && foglaltIdopontok.Contains(foglalt))
                {
                    kiadottSzobak.Add(foglalt.Szobaszam,foglaltak.Where(x => x.Szobaszam == foglalt.Szobaszam).Count());
                }
            }
            LegtobbetKiadottSzoba = kiadottSzobak.Max(x => x.Key);//jó?
        }

        public int OsszbevetelIntervallum(DateTime kezdet, DateTime veg, List<Foglalt> foglaltak, List<Szoba> szobak)
        {
            int osszeg = 0;
            foreach (Foglalt foglalt in foglaltak)
            {
                List<Foglalt> foglaltIdopontok = DatumEgyezik(kezdet, veg, foglalt, foglaltak);
                if (foglaltIdopontok.Contains(foglalt))
                {
                    
                }
            }
            
            return osszeg;
        }



        public List<int> SzobafoglaltSzobak { get => szobaFoglaltDarab.Keys.ToList();}
        public List<int> SzobafoglaltSzobakEnnyiszer { get => szobaFoglaltDarab.Values.ToList();}
        public int LegtobbetKiadottSzoba { get => legtobbetKiadottSzoba; private set => legtobbetKiadottSzoba = value; }

    }
}
