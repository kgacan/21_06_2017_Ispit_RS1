using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_2017_06_21_v1.ViewModels
{
    public class MaturskiIspitRezultatiVM
    {
        public List<RezultatiVM> listaRezultata { get; set; }
        public class RezultatiVM
        {
            public int MaturskiIpsitStavkaId { get; set; }
            public string ucenik { get; set; }
            public int OpciUspjeh { get; set; }

            [Range(0, 100, ErrorMessage = "Please enter valid float Number")]
            public float? MaxBodovi { get; set; }
            public string Oslobodjen { get; set; }
        }
    }
}
