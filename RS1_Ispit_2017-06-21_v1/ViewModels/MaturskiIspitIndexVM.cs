using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_2017_06_21_v1.ViewModels
{
    public class MaturskiIspitIndexVM
    {
        public List<Rows> listaIspita { get; set; }
        public int NastavnikId { get; set; }
        public class Rows
        {
            public int MaturskiIspitId { get; set; }
            public int NastavnikNaIspituId { get; set; }
            public string Datum { get; set; }
            public string Odjeljenje { get; set; }
            public string Ispitivac { get; set; }
            public float? ProsjecniBodovi { get; set; }
        }
    }
}
