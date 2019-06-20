using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_Ispit_2017_06_21_v1.EF;
using RS1_Ispit_2017_06_21_v1.Models;
using RS1_Ispit_2017_06_21_v1.ViewModels;

namespace RS1_Ispit_2017_06_21_v1.Controllers
{
    public class MaturskiIspitController : Controller
    {
        private MojContext _db;

        public MaturskiIspitController(MojContext db)
        {
            _db = db;
        }
        public IActionResult Index(int id)
        {
            var model = new MaturskiIspitIndexVM
            {
                NastavnikId = id,
                listaIspita= _db.MaturskiIspit.Select(y=>new MaturskiIspitIndexVM.Rows
                {
                    Datum=y.Datum.ToShortDateString(),
                    Ispitivac=y.Nastavnik.ImePrezime,
                    NastavnikNaIspituId=y.NastavnikId,
                    MaturskiIspitId=y.Id,
                    Odjeljenje=y.Odjeljenje.Naziv,
                    ProsjecniBodovi=(_db.MaturskiIspitStavka.Where(x=>x.MaturskiIspitId==y.Id).Select(a=>a.Bodovi).Sum()) / _db.MaturskiIspitStavka.Where(x => x.MaturskiIspitId == y.Id && x.Bodovi!=null).Count()
                }).ToList()
            };
            return View(model);
        }
        public IActionResult Dodaj(int id)
        {
            var model = new MaturskiIspitDodajVM
            {
                Datum=DateTime.Now,
                Ispitivac=_db.Nastavnik.Where(x=>x.Id==id).Select(a=>a.ImePrezime).FirstOrDefault(),
                listaOdjeljenja =  _db.Odjeljenje.Where(x=>x.NastavnikId==id).Select(a=> new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text=a.Naziv,
                    Value=a.Id.ToString()
                }).ToList(),
                IspitivacId=id
            };

            return View(model);
        }
        public IActionResult Snimi(MaturskiIspitDodajVM vm)
        {
            MaturskiIspit m = new MaturskiIspit
            {
                Datum=vm.Datum,
                NastavnikId=vm.IspitivacId,
                OdjeljenjeId=vm.OdjeljenjeId
            };

            _db.Add(m);
            _db.SaveChanges();

            _db.MaturskiIspitStavka.AddRange(_db.UpisUOdjeljenje.Where(x => x.OdjeljenjeId == m.OdjeljenjeId && x.OpciUspjeh > 1).Select(y => new MaturskiIspitStavka
            {
                Oslobodjen = y.OpciUspjeh == 5 ? true : false,
                Bodovi=null,
                MaturskiIspitId=m.Id,
                UpisUOdjeljenjeId=y.Id
            }).ToList());

            _db.SaveChanges();

            return RedirectToAction(nameof(Index), new { id = m.NastavnikId });
        }
        public IActionResult Detalji(int id)
        {
            var ispit = _db.MaturskiIspit.Where(x => x.Id == id)
                .Include(a => a.Nastavnik)
                .Include(a => a.Odjeljenje)
                .FirstOrDefault();

            var model = new MaturskiIspitDetaljiVM
            {
                Datum=ispit.Datum.ToShortDateString(),
                Ispitivac=ispit.Nastavnik.ImePrezime,
                Odjeljenje=ispit.Odjeljenje.Naziv,
                MaturskiIspitId=ispit.Id
            };

            return View(model);
        }
        public IActionResult Rezultati(int id)
        {
            var model = new MaturskiIspitRezultatiVM
            {
                listaRezultata=_db.MaturskiIspitStavka.Where(x=>x.MaturskiIspitId==id).Select(y=> new MaturskiIspitRezultatiVM.RezultatiVM
                {
                    MaturskiIpsitStavkaId=y.Id,
                    MaxBodovi=y.Oslobodjen?-1:y.Bodovi,
                    OpciUspjeh=y.UpisUOdjeljenje.OpciUspjeh,
                    Oslobodjen=y.Oslobodjen?"DA":"NE",
                    ucenik=y.UpisUOdjeljenje.Ucenik.ImePrezime
                }).ToList()
            };

            return PartialView(model);
        }
        public IActionResult EvidentiranjeBodova(int id)
        {
            MaturskiIspitStavka m = _db.MaturskiIspitStavka.Where(x => x.Id == id)
                .Include(a => a.UpisUOdjeljenje)
                .ThenInclude(a => a.Ucenik).FirstOrDefault();

            var model = new EvidentiranjeBodovaVM
            {
                Ucenik = m.UpisUOdjeljenje.Ucenik.ImePrezime,
                Bodovi = m.Bodovi,
                MaturskiIspitStavkaId = m.Id
            };

            return PartialView(model);
        }
        public IActionResult SnimiBodove(EvidentiranjeBodovaVM vm)
        {
            MaturskiIspitStavka m = _db.MaturskiIspitStavka.Find(vm.MaturskiIspitStavkaId);
            m.Bodovi = vm.Bodovi;
            _db.SaveChanges();

            return RedirectToAction(nameof(Detalji), new{ id = m.MaturskiIspitId});
        }
        public void SnimiJQ(float? MaxBodovi, int MaturskiIpsitStavkaId)
        {

            MaturskiIspitStavka m = _db.MaturskiIspitStavka.Find(MaturskiIpsitStavkaId);
            m.Bodovi = MaxBodovi;
            _db.SaveChanges();
        }

        public IActionResult Promijeni(int Id)
        {
            MaturskiIspitStavka ms = _db.MaturskiIspitStavka.Find(Id);

            if (ms.Oslobodjen)
                ms.Oslobodjen = false;
            else
                ms.Oslobodjen = true;

            _db.SaveChanges();

            return RedirectToAction(nameof(Detalji), new { id = ms.MaturskiIspitId });
        }
    }
}