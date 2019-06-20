using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_2017_06_21_v1.ViewModels
{
    public class MaturskiIspitDodajVM
    {
        public string Ispitivac { get; set; }
        public int IspitivacId { get; set; }
        public DateTime Datum { get; set; }
        public List<SelectListItem> listaOdjeljenja { get; set; }
        public int OdjeljenjeId { get; set; }

    }
}
