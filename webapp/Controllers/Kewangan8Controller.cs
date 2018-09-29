using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.tool.xml;
using ClosedXML.Excel;

namespace eSPP.Controllers
{
    public class Kewangan8Controller : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext mc = new MajlisContext();
        private Entities db2 = new Entities();
        private SPGContext spg = new SPGContext();

        public List<HR_MAKLUMAT_PERIBADI> CariPekerja(string key, string value, int? bulan, string kod)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            if (key == "1")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
                
            }
            else if (key == "2")
            {
                value = value.ToUpper();
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value.ToUpper())  && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
            }
            else if (key == "3")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
            }

            else if (key == "4")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan && s.HR_AKTIF_IND == "Y" && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
            }

            //if (kod != "00025")
            //{
            //    sPeribadi = sPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y").ToList();
            //}
            return sPeribadi;
        }

        //public List<ErrorListModels> ErrorList(string key, string value, int? bulan, string kod)
        //{
        //    List<HR_MAKLUMAT_PERIBADI> mPekerja = CariPekerja(key, value, bulan, kod);
        //    List<ErrorListModels> error = new List<ErrorListModels>();
        //    foreach(HR_MAKLUMAT_PERIBADI pekerja in mPekerja)
        //    {
        //        var msg = "";
        //        if(pekerja.HR_AKTIF_IND == "T")
        //        {
        //            msg = "Pekerja tidak boleh buat " + ;
        //        }
        //        ErrorListModels err = new ErrorListModels();
        //        if (pekerja.HR_AKTIF_IND != "Y")
        //        {
        //            err.HR_NO_PEKERJA = pekerja.HR_NO_PEKERJA;
        //            err.MESEJ = "";
        //        }
        //    }
        //    return error;
        //}

        public string RedirectLink(string HR_KOD_PERUBAHAN)
        {
            var redirect = "";
            if (HR_KOD_PERUBAHAN == "00031")
            {
                redirect = "GanjaranKontrak";
            }
            if (HR_KOD_PERUBAHAN == "00025")
            {
                redirect = "TahanGaji";
            }
            if (HR_KOD_PERUBAHAN == "00030")
            {
                redirect = "PotonganGaji";
            }
            if(HR_KOD_PERUBAHAN == "kew8")
            {
                redirect = "Kewangan8Manual";
            }
            if (HR_KOD_PERUBAHAN == "00026")
            {
                redirect = "BayarGaji";
            }
            if (HR_KOD_PERUBAHAN == "TP")
            {
                redirect = "TamatPerkhidmatan";
            }
            if (HR_KOD_PERUBAHAN == "00022")
            {
                redirect = "TangguhPergerakanGaji";
            }
            if (HR_KOD_PERUBAHAN == "00037")
            {
                redirect = "SambungPergerakanGaji";
            }
            if (HR_KOD_PERUBAHAN == "00036")
            {
                redirect = "PindaanGaji";
            }
            if (HR_KOD_PERUBAHAN == "CUTI")
            {
                redirect = "Cuti";
            }
            if (HR_KOD_PERUBAHAN == "00015")
            {
                redirect = "SambungKontrak";
            }
            if (HR_KOD_PERUBAHAN == "00039")
            {
                redirect = "SemuaJenisPotongan";
            }
            if (HR_KOD_PERUBAHAN == "00024")
            {
                redirect = "SemuaJenisElaun";
            }
            return redirect;
        }

        public PartialViewResult TableKewangan8(string id)
        {
            ViewBag.id = id;
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = new List<HR_MAKLUMAT_KEWANGAN8>();

            kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == id && (s.HR_KOD_PERUBAHAN == "00002" || s.HR_KOD_PERUBAHAN == "00003" || s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00005" || s.HR_KOD_PERUBAHAN == "00006" || s.HR_KOD_PERUBAHAN == "00007" || s.HR_KOD_PERUBAHAN == "00008" || s.HR_KOD_PERUBAHAN == "00009" || s.HR_KOD_PERUBAHAN == "00010" || s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018" || s.HR_KOD_PERUBAHAN == "00023" || s.HR_KOD_PERUBAHAN == "00027" || s.HR_KOD_PERUBAHAN == "00028" || s.HR_KOD_PERUBAHAN == "00039" || s.HR_KOD_PERUBAHAN == "00040" || s.HR_KOD_PERUBAHAN == "00042" || s.HR_KOD_PERUBAHAN == "00044" || s.HR_KOD_PERUBAHAN == "00045")).ToList<HR_MAKLUMAT_KEWANGAN8>();

            foreach (var item in kewangan8)
            {

                PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                pergerakanGaji.HR_KOD_PERUBAHAN = item.HR_KOD_PERUBAHAN;
                pergerakanGaji.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                pergerakanGaji.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                pergerakanGaji.HR_BULAN = item.HR_BULAN;
                pergerakanGaji.HR_TAHUN = item.HR_TAHUN;
                pergerakanGaji.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                pergerakanGaji.HR_BUTIR_PERUBAHAN = item.HR_BUTIR_PERUBAHAN;
                pergerakanGaji.HR_CATATAN = item.HR_CATATAN;
                pergerakanGaji.HR_NO_SURAT_KEBENARAN = item.HR_NO_SURAT_KEBENARAN;
                pergerakanGaji.HR_AKTIF_IND = item.HR_AKTIF_IND;
                pergerakanGaji.HR_NP_UBAH_HR = item.HR_NP_UBAH_HR;
                pergerakanGaji.HR_TARIKH_UBAH_HR = item.HR_TARIKH_UBAH_HR;
                pergerakanGaji.HR_NP_FINALISED_HR = item.HR_NP_FINALISED_HR;
                pergerakanGaji.HR_TARIKH_FINALISED_HR = item.HR_TARIKH_FINALISED_HR;
                pergerakanGaji.HR_FINALISED_IND_HR = item.HR_FINALISED_IND_HR;
                pergerakanGaji.HR_NP_UBAH_PA = item.HR_NP_UBAH_PA;
                pergerakanGaji.HR_TARIKH_UBAH_PA = item.HR_TARIKH_UBAH_PA;
                pergerakanGaji.HR_NP_FINALISED_PA = item.HR_NP_FINALISED_PA;
                pergerakanGaji.HR_TARIKH_FINALISED_PA = item.HR_TARIKH_FINALISED_PA;
                pergerakanGaji.HR_FINALISED_IND_PA = item.HR_FINALISED_IND_PA;
                pergerakanGaji.HR_EKA = item.HR_EKA;
                pergerakanGaji.HR_ITP = item.HR_ITP;
                pergerakanGaji.HR_KEW8_IND = item.HR_KEW8_IND;
                pergerakanGaji.HR_BIL = item.HR_BIL;
                pergerakanGaji.HR_KOD_JAWATAN = item.HR_KOD_JAWATAN;
                pergerakanGaji.HR_KEW8_ID = item.HR_KEW8_ID;
                pergerakanGaji.HR_LANTIKAN_IND = item.HR_LANTIKAN_IND;
                pergerakanGaji.HR_TARIKH_SP = item.HR_TARIKH_SP;
                pergerakanGaji.HR_SP_IND = item.HR_SP_IND;
                pergerakanGaji.HR_JUMLAH_BULAN = item.HR_JUMLAH_BULAN;
                pergerakanGaji.HR_NILAI_EPF = item.HR_NILAI_EPF;
                model.Add(pergerakanGaji);
            }
            return PartialView("_TableKewangan8", model.GroupBy(s => s.HR_KEW8_ID).Select(s => s.FirstOrDefault()));
        }

        public ActionResult Kewangan8Manual(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "kew8");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        private void InfoPekerja(PergerakanGajiModels model)
        {
            HR_MAKLUMAT_PERIBADI Peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_AKTIF_IND == "Y").Count() > 0);
            if (Peribadi == null)
            {
                Peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            int HR_GRED = Convert.ToInt32(Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == HR_GRED);
            if (gred == null)
            {
                gred = new GE_PARAMTABLE();
            }

            ViewBag.HR_GRED = gred.SHORT_DESCRIPTION;
            ViewBag.HR_KOD_GAJI = Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_GAJI_POKOK = Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

            model.HR_ITP = 0;
            model.HR_EKA = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> itp = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (itp.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in itp)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        var j = item.HR_JUMLAH;
                        jumElaun = jumElaun + j;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        var j = item.HR_JUMLAH;
                        jumAwam = jumAwam + j;
                    }
                }
                model.HR_ITP = jumElaun;
                model.HR_EKA = jumAwam;
            }
        }

        public ActionResult TambahKewangan8(PergerakanGajiModels model)
        {

            InfoPekerja(model);

            var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
            var kewID = (lastID.HR_KEW8_ID + 1);

            model.HR_KEW8_ID = kewID;

            List<HR_KEWANGAN8> kewangan8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kewangan8, "HR_KOD_KEW8", "HR_PENERANGAN");
            return PartialView("_TambahKewangan8", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKewangan8(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, PergerakanGajiModels pergerakanGaji, HR_MAKLUMAT_PEKERJAAN pekerjaan)
        {
            if (ModelState.IsValid)
            {
                var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                var kewID = (lastID.HR_KEW8_ID + 1);

                model.HR_KEW8_ID = kewID;

                model.HR_TARIKH_KEYIN = DateTime.Now;
                model.HR_FINALISED_IND_HR = "T";
                model.HR_BULAN = DateTime.Now.Month;
                model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);

                //modelDetail.HR_STATUS_IND = "T";
                //modelDetail.HR_JUMLAH_PERUBAHAN = 0;

                db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                //db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail);
                db.SaveChanges();

                return RedirectToAction("Kewangan8", "Kewangan8", new { key = "1", value = model.HR_NO_PEKERJA });
            }
            List<HR_KEWANGAN8> kewangan8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kewangan8, "HR_KOD_KEW8", "HR_PENERANGAN");
            return PartialView("_TambahKewangan8", pergerakanGaji);
        }

        public ActionResult InfoKewangan8(string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, int? HR_KEW8_ID)
        {
            if (HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KEW8_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tarikhMula = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikhMula && s.HR_KEW8_ID == HR_KEW8_ID);
            if (kewangan8 == null)
            {
                return HttpNotFound();
            }

            PergerakanGajiModels model = new PergerakanGajiModels();
            model.HR_NO_PEKERJA = kewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = kewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = kewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = kewangan8.HR_BULAN;
            model.HR_TAHUN = kewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = kewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = kewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = kewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = kewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = kewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = kewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = kewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = kewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = kewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = kewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = kewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = kewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = kewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = kewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = kewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = kewangan8.HR_EKA;
            model.HR_ITP = kewangan8.HR_ITP;
            model.HR_KEW8_IND = kewangan8.HR_KEW8_IND;
            model.HR_BIL = kewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = kewangan8.HR_KOD_JAWATAN;
            model.HR_KEW8_ID = kewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = kewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = kewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = kewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = kewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = kewangan8.HR_NILAI_EPF;

            InfoPekerja(model);

            List<HR_KEWANGAN8> kew8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kew8, "HR_KOD_KEW8", "HR_PENERANGAN");

            return PartialView("_InfoKewangan8", model);
        }

        public ActionResult EditKewangan8(string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, int? HR_KEW8_ID)
        {
            if (HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KEW8_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tarikhMula = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikhMula && s.HR_KEW8_ID == HR_KEW8_ID);
            if (kewangan8 == null)
            {
                return HttpNotFound();
            }

            PergerakanGajiModels model = new PergerakanGajiModels();
            model.HR_NO_PEKERJA = kewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = kewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = kewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = kewangan8.HR_BULAN;
            model.HR_TAHUN = kewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = kewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = kewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = kewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = kewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = kewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = kewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = kewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = kewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = kewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = kewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = kewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = kewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = kewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = kewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = kewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = kewangan8.HR_EKA;
            model.HR_ITP = kewangan8.HR_ITP;
            model.HR_KEW8_IND = kewangan8.HR_KEW8_IND;
            model.HR_BIL = kewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = kewangan8.HR_KOD_JAWATAN;
            model.HR_KEW8_ID = kewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = kewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = kewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = kewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = kewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = kewangan8.HR_NILAI_EPF;

            InfoPekerja(model);

            List<HR_KEWANGAN8> kew8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kew8, "HR_KOD_KEW8", "HR_PENERANGAN");

            return PartialView("_EditKewangan8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKewangan8(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, PergerakanGajiModels pergerakanGaji)
        {
            if(ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }
                model.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                model.HR_TARIKH_UBAH_HR = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Kewangan8", "Kewangan8", new { key = "1", value = model.HR_NO_PEKERJA });
            }
            return PartialView("_EditKewangan8", pergerakanGaji);
        }


        public ActionResult PadamKewangan8(string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, int? HR_KEW8_ID)
        {
            if (HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KEW8_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tarikhMula = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikhMula && s.HR_KEW8_ID == HR_KEW8_ID);
            if (kewangan8 == null)
            {
                return HttpNotFound();
            }

            

            PergerakanGajiModels model = new PergerakanGajiModels();
            model.HR_NO_PEKERJA = kewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = kewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = kewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = kewangan8.HR_BULAN;
            model.HR_TAHUN = kewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = kewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = kewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = kewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = kewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = kewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = kewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = kewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = kewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = kewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = kewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = kewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = kewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = kewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = kewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = kewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = kewangan8.HR_EKA;
            model.HR_ITP = kewangan8.HR_ITP;
            model.HR_KEW8_IND = kewangan8.HR_KEW8_IND;
            model.HR_BIL = kewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = kewangan8.HR_KOD_JAWATAN;
            model.HR_KEW8_ID = kewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = kewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = kewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = kewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = kewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = kewangan8.HR_NILAI_EPF;

            InfoPekerja(model);

            List<HR_KEWANGAN8> kew8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kew8, "HR_KOD_KEW8", "HR_PENERANGAN");

            return PartialView("_PadamKewangan8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamKewangan8(HR_MAKLUMAT_KEWANGAN8 kewangan8, HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail)
        {
            //db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Remove(modelDetail);
            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
            db.HR_MAKLUMAT_KEWANGAN8.Remove(model);
                
            db.SaveChanges();
            return RedirectToAction("Kewangan8", "Kewangan8", new { key = "1", value = kewangan8.HR_NO_PEKERJA });
        }

        //GET
        public ActionResult Borang(string key, string value)
        {
            MaklumatKakitanganModels model = new MaklumatKakitanganModels();
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            if (key == "1" || key == "4")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(a => a.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_PERIBADI>();
            }
            else if (key == "2")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(a => a.HR_NAMA_PEKERJA.Contains(value)).ToList<HR_MAKLUMAT_PERIBADI>();
            }
            else if (key == "3")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(a => a.HR_NO_KPBARU == value).ToList<HR_MAKLUMAT_PERIBADI>();
            }

            if(key == "4" && mPeribadi.Count() > 0)
            {
                //HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.si
            }
            return View();
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Borang()
        {

            return View();
        }



        public ActionResult Penyata()
        {
            return View();
        }

        public ActionResult SenaraiPergerakanGaji()
        {
            //var model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(DateTime.Now.Month, DateTime.Now.Year);
            List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == DateTime.Now.Month && s.HR_TAHUN == DateTime.Now.Year).ToList<HR_MAKLUMAT_KEWANGAN8>();
            
            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.ToList();
            ViewBag.jabatan = mc.GE_JABATAN.ToList();
            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            ViewBag.bulan = DateTime.Now.Month;
            ViewBag.year = DateTime.Now.Year;
            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;
            return View(model);
        }

        [HttpPost]
        public ActionResult SenaraiPergerakanGaji(int? bulan, int? tahun)
        {
            //var model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(DateTime.Now.Month, DateTime.Now.Year);
            List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == DateTime.Now.Month && s.HR_TAHUN == DateTime.Now.Year).ToList<HR_MAKLUMAT_KEWANGAN8>();
            if (bulan != null && tahun != null)
            {
                //model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(bulan, tahun);
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.ToList();
            ViewBag.jabatan = mc.GE_JABATAN.ToList();
            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            ViewBag.bulan = bulan;
            ViewBag.year = tahun;
            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;
            return View(model);
        }

        public JsonResult SenaraiPergerakanGaji2(int? bulan, int? tahun)
        {
            List<MaklumatKakitanganModels> model = new List<MaklumatKakitanganModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();
            
            foreach(HR_MAKLUMAT_KEWANGAN8 kewangan in kewangan8)
            {
                MaklumatKakitanganModels model2 = new MaklumatKakitanganModels();
                model2.HR_MAKLUMAT_KEWANGAN8 = new HR_MAKLUMAT_KEWANGAN8();
                model2.HR_MAKLUMAT_KEWANGAN8 = kewangan;

                
                model2.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi();
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan.HR_NO_PEKERJA);
                if(peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }
                model2.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA = peribadi.HR_NO_PEKERJA;
                model2.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;

                model2.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan.HR_NO_PEKERJA);
                if(pekerjaan == null)
                {
                    pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }

                GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == pekerjaan.HR_JABATAN);
                if(pekerjaan == null || jabatan == null)
                {
                    jabatan = new GE_JABATAN();
                }

                model2.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == pekerjaan.HR_JAWATAN);
                if (pekerjaan == null || jawatan == null)
                {
                    jawatan = new HR_JAWATAN();
                }
                model2.HR_MAKLUMAT_KEWANGAN8.HR_KOD_JAWATAN = jawatan.HR_NAMA_JAWATAN;
                model.Add(model2);
            }

            JsonResult json = Json(model, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = 2147483647;

            return json;
        }

        [HttpPost]
        public FileStreamResult PDFSenaraiPergerakanGaji(int? bulan, int? tahun)
        {

            List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();
            List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();

            foreach(HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA);
                GE_JABATAN jabatan2 = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                sJabatan.Add(jabatan2);
            }

            var html = "<html><head>";
            html += "<title>Senarai Pergerakan Gaji</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";
            foreach (GE_JABATAN jab in sJabatan.GroupBy(s => s.GE_KOD_JABATAN).Select(s => s.FirstOrDefault()))
            {
                html += "<p>" + jab .GE_KETERANGAN_JABATAN + "</p>";
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 1px solid black;'>";

                //html += "<thead>";
                html += "<tr>";
                html += "<td style='border: 1px solid black; font-size: 60%' align='center'><strong>#</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>No Pekerja</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>Nama Pekerja</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>Tarikh Pergerakan</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>Jabatan</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>Jawatan</strong></td>";
                html += "</tr>";
                //html += "</thead>";
                //html += "<tbody>";
                var no = 0;
                foreach (var l in model)
                {
                    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_NO_PEKERJA);
                    if (peribadi == null)
                    {
                        peribadi = new HR_MAKLUMAT_PERIBADI();
                    }

                    if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jab.GE_KOD_JABATAN)
                    {
                        GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                        if (jabatan == null)
                        {
                            jabatan = new GE_JABATAN();
                        }
                        HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                        if (jawatan == null)
                        {
                            jawatan = new HR_JAWATAN();
                        }
                        ++no;
                        html += "<tr>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + no + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + l.HR_NO_PEKERJA + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_TARIKH_KEYIN) + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + jabatan.GE_KETERANGAN_JABATAN + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + jawatan.HR_NAMA_JAWATAN + "</td>";
                        html += "</tr>";
                    }
                }

                html += "</table>";
            }
            
            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");

                var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "Appril" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "september" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };
                var Bulan = "";
                foreach (var m in associativeArray)
                {
                    if (bulan == m.Key)
                    {
                        Bulan = m.Value;
                    }

                }

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("Senarai Pergerakan Gaji");
                iTextSharp.text.Paragraph paragraph2 = new iTextSharp.text.Paragraph("Bulan                       " + Bulan, contentFont);
                iTextSharp.text.Paragraph paragraph3 = new iTextSharp.text.Paragraph("Tahun                       " + tahun, contentFont);
                paragraph.Alignment = Element.ALIGN_JUSTIFIED;
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                //pic.SpacingBefore = 9f;
                paragraph.SpacingBefore = 10f;
                paragraph2.SpacingBefore = 10f;
                //pic.BorderWidthTop = 36f;
                //paragraph2.SetLeading(20f, 0);
                document.Add(pic);
                document.Add(paragraph);
                document.Add(paragraph2);
                document.Add(paragraph3);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                //PdfPTable table = new PdfPTable(3);
                //PdfPCell cell = new PdfPCell(new Phrase("Header spanning 3 columns"));
                //cell.Colspan = 3;
                //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                //table.AddCell(cell);
                //table.AddCell("Col 1 Row 1");
                //table.AddCell("Col 2 Row 1");
                //table.AddCell("Col 3 Row 1");
                //table.AddCell("Col 1 Row 2");
                //table.AddCell("Col 2 Row 2");
                //table.AddCell("Col 3 Row 2");
                //document.Add(table);

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Paragraph paragraph4 = new iTextSharp.text.Paragraph("Copyright © " + DateTime.Now.Year + " Sistem Bandaraya Petaling Jaya. All Rights Reserved\nUser Id: " + User.Identity.Name.ToLower() + " - Tarikh print: " + DateTime.Now.ToString("dd-MM-yyyy"), contentFont2);
                document.Add(paragraph4);

                document.Close();
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
        }

        [HttpPost]
        public FileResult EXCSenaraiPergerakanGaji(int? bulan, int? tahun)
        {
            List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();
            if(model.Count() <= 0)
            {
                model = new List<HR_MAKLUMAT_KEWANGAN8>();
            }
            DataSet ds = new DataSet();

            List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();

            foreach (HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA);
                GE_JABATAN jabatan2 = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                sJabatan.Add(jabatan2);
            }
            var no2 = 0;
            foreach (GE_JABATAN jab in sJabatan.GroupBy(s => s.GE_KOD_JABATAN).Select(s => s.FirstOrDefault()))
            {
                no2++;
                DataTable dt = new DataTable("SENARAI PERGERAKAN GAJI "+ jab.GE_KOD_JABATAN);
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("#"),
                                            new DataColumn("No Pekerja"),
                                            new DataColumn("Nama Pekerja"),
                                            new DataColumn("Tarikh Pergerakan"),
                                            new DataColumn("Jabatan"),
                                            new DataColumn("Jawatan")});


                var no = 0;
                foreach (var l in model)
                {
                    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_NO_PEKERJA);
                    if (peribadi == null)
                    {
                        peribadi = new HR_MAKLUMAT_PERIBADI();
                    }

                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jab.GE_KOD_JABATAN)
                    {
                        GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                        if (jabatan == null)
                        {
                            jabatan = new GE_JABATAN();
                        }
                        HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                        if (jawatan == null)
                        {
                            jawatan = new HR_JAWATAN();
                        }
                        ++no;
                        dt.Rows.Add(no, l.HR_NO_PEKERJA, peribadi.HR_NAMA_PEKERJA, string.Format("{0:dd/MM/yyyy}", l.HR_TARIKH_KEYIN), jabatan.GE_KETERANGAN_JABATAN, jawatan.HR_NAMA_JAWATAN);
                    }
                    
                }
                ds.Tables.Add(dt);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Senarai_Pergerakan_gaji.xlsx");
                }
            }
        }

        //public ActionResult PergerakanGaji(string key, string value)
        //{
        //    List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
        //    List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

        //    if (key == "1" || key == "4")
        //    {
        //        mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == value).ToList();

        //    }
        //    else if (key == "2")
        //    {
        //        value = value.ToUpper();
        //        mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value.ToUpper())).ToList();
        //    }
        //    else if (key == "3")
        //    {
        //        mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU.Contains(value)).ToList();
        //    }

        //    if(mPeribadi.Count() > 0)
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && s.HR_KOD_PERUBAHAN == "00001").ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    ViewBag.key = key;
        //    ViewBag.mPeribadi = mPeribadi;
        //    ViewBag.detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
        //    ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
        //    ViewBag.Matriks = db.HR_MATRIKS_GAJI.ToList();
        //    ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
        //    return View(model);
        //}

        public ActionResult PergerakanGaji(string key, string value, int? bulan, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.Error ? "Data Tidak Berjaya Dimasukkan."
            : message == ManageMessageId.Success ? "Data Berjaya Dimasukkan."
            : "";
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            
            sPeribadi = CariPekerja(key, value, bulan, "00001");

            ViewBag.gambar = new List<HR_GAMBAR_PENGGUNA>();
            if (sPeribadi.Count() > 0)
            {
                ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            }

            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi)
            {
                PergerakanGajiModels pergerkanGaji = new PergerakanGajiModels();
                int? gred = null;
                if(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred );
                if(sGred == null)
                {
                    sGred = new GE_PARAMTABLE();
                }
                int? peringkat = null;
                //decimal? tahap = null;
                if (item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                {
                    item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI = item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Trim();
                }
                if (item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                {
                    peringkat = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1));
                }
                //if(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(2,1) == "T" && item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.ToCharArray().Count() > 3)
                //{
                //    tahap = Convert.ToDecimal(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3));
                //}
                string pkt = "P" + peringkat;
                decimal? kenaikan = 0;
                decimal? gajiPokokBaru = 0;
                decimal? gajiPokokBaru2 = 0;
                decimal? gaji_maxsimum = 0;
                decimal? tunggakan = 0;
                HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                if(jadualGaji != null)
                {
                    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                }

                gajiPokokBaru = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                if(gajiPokokBaru > gaji_maxsimum)
                {
                    gajiPokokBaru2 = gaji_maxsimum;
                }
                else
                {
                    gajiPokokBaru2 = gajiPokokBaru;
                }

                HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                if (matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                    matriks.HR_GAJI_MIN = 0;
                    matriks.HR_GAJI_MAX = 0;
                    matriks.HR_GAJI_POKOK = 0;
                }

                tunggakan = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                pergerkanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                pergerkanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;
                pergerkanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                pergerkanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                pergerkanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                pergerkanGaji.HR_JUMLAH_PERUBAHAN = tunggakan;
                pergerkanGaji.CHECKED = 0;
                var countKew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(k => k.HR_NO_PEKERJA == item.HR_NO_PEKERJA && k.HR_FINALISED_IND_HR == "P").Count();
                
                if (countKew8 <= 0)
                {
                    pergerkanGaji.CHECKED = 1;
                }
                model.Add(pergerkanGaji);

            }
            ViewBag.countKew8 = model.Where(s => s.CHECKED == 1).Count();
            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PergerakanGaji(IEnumerable<PergerakanGajiModels> pergerakanGaji, PergerakanGajiModels pergerakanGajiDetail, string key2, string value2, int? bulan2)
        {
            if(ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi2 == null)
                {
                    peribadi2 = new HR_MAKLUMAT_PERIBADI();
                }

                foreach (var peribadi in pergerakanGaji)
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA).SingleOrDefault();
                    var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    var peringkat = "";
                    var tahap = "";

                    string gred = null;

                    GE_PARAMTABLE gredList = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == peribadi.HR_GRED).SingleOrDefault();
                    if(gredList != null)
                    {
                        gred = Convert.ToString(gredList.ORDINAL);
                    }

                    if (peribadi.HR_JENIS_PERGERAKAN == "D" || peribadi.HR_JENIS_PERGERAKAN == "S")
                    {
                        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                        {
                            peringkat = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1);
                            if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                            {

                                var t = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                                tahap = t;
                            }
                        }
                    }
                    else if (peribadi.HR_JENIS_PERGERAKAN == "J")
                    {
                        HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == peribadi.HR_GAJI_MIN && s.HR_GAJI_MAX == peribadi.HR_GAJI_MAX && s.HR_GRED_GAJI == peribadi.HR_GRED && s.HR_GAJI_POKOK == peribadi.HR_GAJI_BARU).SingleOrDefault();
                        if(matriks == null)
                        {
                            matriks = new HR_MATRIKS_GAJI();
                        }
                        peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                        tahap = Convert.ToString(matriks.HR_TAHAP);
                    }
                    var pergerakan = db2.ZATUL_INSERT_KEW_GERAK_GAJI2(bulan, tahun, peribadi.HR_NO_PEKERJA, pergerakanGajiDetail.HR_BUTIR_PERUBAHAN, peribadi.HR_JENIS_PERGERAKAN, peringkat, tahap, pergerakanGajiDetail.HR_NP_FINALISED_HR, pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred);
                }


               
                return RedirectToAction("PergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
            }
            ViewBag.key = key2;
            ViewBag.value = value2;
            ViewBag.bulan = bulan2;

            return RedirectToAction("PergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
        }

        public ActionResult PengesahanPergerakanGaji(string key, string value, int? bulan, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.Error ? "Data Tidak Berjaya Dimasukkan."
            : message == ManageMessageId.Success ? "Data Berjaya Dimasukkan."
            : "";
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();

            sPeribadi = CariPekerja(key, value, bulan, "00001");

            ViewBag.gambar = new List<HR_GAMBAR_PENGGUNA>();
            if (sPeribadi.Count() > 0)
            {
                ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            }

            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi)
            {
                PergerakanGajiModels pergerkanGaji = new PergerakanGajiModels();
                HR_MAKLUMAT_KEWANGAN8 kew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (s.HR_FINALISED_IND_HR.Contains("T") || s.HR_FINALISED_IND_HR.Contains("P"))).OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                if (kew8 != null)
                {
                    HR_MAKLUMAT_KEWANGAN8_DETAIL kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kew8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID);
                    if (kew8Detail == null)
                    {
                        kew8Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }

                    int? gred = null;
                    if (kew8Detail.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(kew8Detail.HR_GRED);
                    }
                    GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (sGred == null)
                    {
                        sGred = new GE_PARAMTABLE();
                    }
                    int? peringkat = null;
                    decimal? tahap = null;
                    //decimal? tahap = null;
                    if (kew8Detail.HR_MATRIKS_GAJI != null)
                    {
                        kew8Detail.HR_MATRIKS_GAJI = kew8Detail.HR_MATRIKS_GAJI.Trim();
                    }
                    if (kew8Detail.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                    {
                        peringkat = Convert.ToInt32(kew8Detail.HR_MATRIKS_GAJI.Substring(1, 1));
                    }
                    if (kew8Detail.HR_MATRIKS_GAJI.Substring(2, 1) == "T" && kew8Detail.HR_MATRIKS_GAJI.ToCharArray().Count() > 3)
                    {
                        tahap = Convert.ToDecimal(kew8Detail.HR_MATRIKS_GAJI.Substring(3));
                    }
                    //string pkt = "P" + peringkat;
                    //decimal? kenaikan = 0;
                    //decimal? gajiPokokBaru = 0;
                    //decimal? gajiPokokBaru2 = 0;
                    //decimal? gaji_maxsimum = 0;
                    decimal? tunggakan = 0;
                    //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    //if (jadualGaji != null)
                    //{
                    //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    //}

                    //gajiPokokBaru = kew8Detail.HR_GAJI_BARU;

                    //if (gajiPokokBaru > gaji_maxsimum)
                    //{
                    //    gajiPokokBaru2 = gaji_maxsimum;
                    //}
                    //else
                    //{
                    //    gajiPokokBaru2 = gajiPokokBaru;
                    //}

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }

                    tunggakan = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                    pergerkanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerkanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;
                    pergerkanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerkanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    pergerkanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    pergerkanGaji.HR_JUMLAH_PERUBAHAN = tunggakan;
                    pergerkanGaji.HR_KEW8_ID = kew8.HR_KEW8_ID;
                    pergerkanGaji.HR_TARIKH_MULA = kew8.HR_TARIKH_MULA;
                    pergerkanGaji.HR_KOD_PELARASAN = kew8Detail.HR_KOD_PELARASAN;
                    model.Add(pergerkanGaji);

                    ViewBag.HR_NO_SURAT_KEBENARAN = kew8.HR_NO_SURAT_KEBENARAN;
                    ViewBag.HR_BUTIR_PERUBAHAN = kew8.HR_BUTIR_PERUBAHAN;

                    HR_MAKLUMAT_PERIBADI peribadiPegawai = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NP_FINALISED_HR);
                    if(peribadiPegawai == null)
                    {
                        peribadiPegawai = new HR_MAKLUMAT_PERIBADI();
                    }

                    ViewBag.HR_NAMA_PEGAWAI = peribadiPegawai.HR_NAMA_PEKERJA;
                    ViewBag.HR_NP_FINALISED_HR = kew8.HR_NP_FINALISED_HR;
                    ViewBag.HR_FINALISED_IND_HR = kew8.HR_FINALISED_IND_HR;
                    ViewBag.HR_CATATAN = kew8.HR_CATATAN;

                    List<SelectListItem> pengesahan = new List<SelectListItem>();
                    pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamat" });
                    pengesahan.Add(new SelectListItem { Value = "T", Text = "Tolak" });
                    pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
                    ViewBag.pengesahan = pengesahan;
                }
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PengesahanPergerakanGaji(IEnumerable<PergerakanGajiModels> pergerakanGaji, PergerakanGajiModels pergerakanGajiDetail, string key2, string value2, int? bulan2)
        {
            if (ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi2 == null)
                {
                    peribadi2 = new HR_MAKLUMAT_PERIBADI();
                }

                foreach (var peribadi in pergerakanGaji)
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA).SingleOrDefault();
                    if(mPeribadi == null)
                    {
                        mPeribadi = new HR_MAKLUMAT_PERIBADI();
                    }
                    var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    var peringkat = "";
                    var tahap = "";

                    string gred = null;

                    GE_PARAMTABLE gredList = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == peribadi.HR_GRED).SingleOrDefault();
                    if (gredList != null)
                    {
                        gred = Convert.ToString(gredList.ORDINAL);
                    }

                    if (peribadi.HR_JENIS_PERGERAKAN == "D" || peribadi.HR_JENIS_PERGERAKAN == "S")
                    {
                        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                        {
                            peringkat = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1);
                            if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                            {

                                var t = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                                tahap = t;
                            }
                        }
                    }

                    else if (peribadi.HR_JENIS_PERGERAKAN == "J")
                    {
                        HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == peribadi.HR_GAJI_MIN && s.HR_GAJI_MAX == peribadi.HR_GAJI_MAX && s.HR_GRED_GAJI == peribadi.HR_GRED && s.HR_GAJI_POKOK == peribadi.HR_GAJI_BARU).SingleOrDefault();
                        if (matriks == null)
                        {
                            matriks = new HR_MATRIKS_GAJI();
                        }
                        peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                        tahap = Convert.ToString(matriks.HR_TAHAP);
                    }
                    var pergerakan = db2.ZATUL_UPDATE_KEW_GERAK_GAJI2(bulan2, tahun, peribadi.HR_NO_PEKERJA, pergerakanGajiDetail.HR_BUTIR_PERUBAHAN, peribadi.HR_JENIS_PERGERAKAN, peringkat, tahap, pergerakanGajiDetail.HR_NP_FINALISED_HR, pergerakanGajiDetail.HR_FINALISED_IND_HR, pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred, peribadi.HR_KEW8_ID, peribadi.HR_TARIKH_MULA, peribadi.HR_KOD_PELARASAN);

                }

                return RedirectToAction("PengesahanPergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
            }
            ViewBag.key = key2;
            ViewBag.value = value2;
            ViewBag.bulan = bulan2;

            return RedirectToAction("PengesahanPergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
        }

        public PartialViewResult TablePergerakanGaji(string id, string key, string value, int? bulan)
        {
            ViewBag.id = id;
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERUBAHAN == "00001").ToList<HR_MAKLUMAT_KEWANGAN8>();

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            foreach (var item in kewangan8)
            {
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kewangan8Ditail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERUBAHAN == "00001" && s.HR_KEW8_ID == item.HR_KEW8_ID && s.HR_TARIKH_MULA == item.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
                foreach (var itemDetail in kewangan8Ditail)
                {
                    PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                    pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerakanGaji.HR_KOD_PERUBAHAN = item.HR_KOD_PERUBAHAN;
                    pergerakanGaji.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                    pergerakanGaji.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                    pergerakanGaji.HR_BULAN = item.HR_BULAN;
                    pergerakanGaji.HR_TAHUN = item.HR_TAHUN;
                    pergerakanGaji.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                    pergerakanGaji.HR_BUTIR_PERUBAHAN = item.HR_BUTIR_PERUBAHAN;
                    pergerakanGaji.HR_CATATAN = item.HR_CATATAN;
                    pergerakanGaji.HR_NO_SURAT_KEBENARAN = item.HR_NO_SURAT_KEBENARAN;
                    pergerakanGaji.HR_AKTIF_IND = item.HR_AKTIF_IND;
                    pergerakanGaji.HR_NP_UBAH_HR = item.HR_NP_UBAH_HR;
                    pergerakanGaji.HR_TARIKH_UBAH_HR = item.HR_TARIKH_UBAH_HR;
                    pergerakanGaji.HR_NP_FINALISED_HR = item.HR_NP_FINALISED_HR;
                    pergerakanGaji.HR_TARIKH_FINALISED_HR = item.HR_TARIKH_FINALISED_HR;
                    pergerakanGaji.HR_FINALISED_IND_HR = item.HR_FINALISED_IND_HR;
                    pergerakanGaji.HR_NP_UBAH_PA = item.HR_NP_UBAH_PA;
                    pergerakanGaji.HR_TARIKH_UBAH_PA = item.HR_TARIKH_UBAH_PA;
                    pergerakanGaji.HR_NP_FINALISED_PA = item.HR_NP_FINALISED_PA;
                    pergerakanGaji.HR_TARIKH_FINALISED_PA = item.HR_TARIKH_FINALISED_PA;
                    pergerakanGaji.HR_FINALISED_IND_PA = item.HR_FINALISED_IND_PA;
                    pergerakanGaji.HR_EKA = item.HR_EKA;
                    pergerakanGaji.HR_ITP = item.HR_ITP;
                    pergerakanGaji.HR_KEW8_IND = item.HR_KEW8_IND;
                    pergerakanGaji.HR_BIL = item.HR_BIL;
                    pergerakanGaji.HR_KOD_JAWATAN = item.HR_KOD_JAWATAN;
                    pergerakanGaji.HR_KEW8_ID = item.HR_KEW8_ID;
                    pergerakanGaji.HR_LANTIKAN_IND = item.HR_LANTIKAN_IND;
                    pergerakanGaji.HR_TARIKH_SP = item.HR_TARIKH_SP;
                    pergerakanGaji.HR_SP_IND = item.HR_SP_IND;
                    pergerakanGaji.HR_JUMLAH_BULAN = item.HR_JUMLAH_BULAN;
                    pergerakanGaji.HR_NILAI_EPF = item.HR_NILAI_EPF;

                    pergerakanGaji.HR_KOD_PELARASAN = itemDetail.HR_KOD_PELARASAN;
                    pergerakanGaji.HR_MATRIKS_GAJI = itemDetail.HR_MATRIKS_GAJI;
                    pergerakanGaji.HR_GRED = itemDetail.HR_GRED;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = itemDetail.HR_JUMLAH_PERUBAHAN;
                    pergerakanGaji.HR_GAJI_BARU = itemDetail.HR_GAJI_BARU;
                    pergerakanGaji.HR_JENIS_PERGERAKAN = itemDetail.HR_JENIS_PERGERAKAN;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN_ELAUN = itemDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                    pergerakanGaji.HR_STATUS_IND = itemDetail.HR_STATUS_IND;
                    pergerakanGaji.HR_ELAUN_KRITIKAL_BARU = itemDetail.HR_ELAUN_KRITIKAL_BARU;
                    pergerakanGaji.HR_NO_PEKERJA_PT = itemDetail.HR_NO_PEKERJA_PT;
                    pergerakanGaji.HR_PERGERAKAN_EKAL = itemDetail.HR_PERGERAKAN_EKAL;
                    pergerakanGaji.HR_PERGERAKAN_EWIL = itemDetail.HR_PERGERAKAN_EWIL;
                    pergerakanGaji.HR_GAJI_LAMA = itemDetail.HR_GAJI_LAMA;

                    GE_PARAMTABLE gred = new GE_PARAMTABLE();

                    if (itemDetail.HR_GRED != null)
                    {
                        var grd = Convert.ToInt32(itemDetail.HR_GRED);
                        gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                        if(gred == null)
                        {
                            gred = new GE_PARAMTABLE();
                        }
                    }


                    var peringkat = "";
                    decimal tahap = 0;
                    if (itemDetail.HR_MATRIKS_GAJI != null)
                    {
                        peringkat = itemDetail.HR_MATRIKS_GAJI.Substring(0, 2);
                        if(itemDetail.HR_MATRIKS_GAJI.Contains('T'))
                        {
                            
                            var t = itemDetail.HR_MATRIKS_GAJI.Substring(3);
                            tahap = Convert.ToDecimal(t);
                        }
                        
                    }

                    HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
                    if(jadual == null)
                    {
                        jadual = new HR_JADUAL_GAJI();
                    }
                    var jadualPeringkat = "";
                    var jp = 0;
                    if(jadual.HR_PERINGKAT != null)
                    {
                        jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                        jp = Convert.ToInt32(jadualPeringkat);
                    }

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
                    if(matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                    }
                    pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    pergerakanGaji.HR_NAMA_GRED = matriks.HR_GRED_GAJI;
                    model.Add(pergerakanGaji);
                }
            }
            return PartialView("_TablePergerakanGaji", model);
        }

        [HttpGet]
        public ActionResult TambahPergerakanGaji(PergerakanGajiModels model, List<HR_MAKLUMAT_PERIBADI> sPeribadi,  ManageMessageId? message)
        {
            List<HR_MAKLUMAT_PERIBADI> sPekerja = new List<HR_MAKLUMAT_PERIBADI>();

            //List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

            /*if (key2 == "1")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value2).ToList();

            }
            else if (key2 == "2")
            {
                value2 = value2.ToUpper();
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value2.ToUpper())).ToList();
            }
            else if (key2 == "3")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value2)).ToList();
            }

            else if (key2 == "4")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan2).ToList();
            }*/

            if(sPeribadi.Count() > 0)
            {
                foreach (var peribadi in sPeribadi)
                {
                    // HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA).SingleOrDefault();
                    var bulan = Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    var peringkat = "";
                    var tahap = "";
                    if (model.HR_MATRIKS_GAJI != null)
                    {
                        peringkat = model.HR_MATRIKS_GAJI.Substring(0, 2);
                        if (model.HR_MATRIKS_GAJI.Contains('T'))
                        {
                            var t = model.HR_MATRIKS_GAJI.Substring(3);
                            tahap = t;
                        }
                    }
                    sPekerja.Add(peribadi);
                }
            }

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();

            ViewBag.sPekerja = sPekerja;
            ViewBag.sPegawai = sPegawai;
            ViewBag.jabatan = mc.GE_JABATAN.ToList();
            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            return PartialView("_TambahPergerakanGaji", model);
        }

        public ActionResult InfoPergerakanGaji(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string HR_KOD_PELARASAN, string key, string value, int? bulan)
        {
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KOD_PELARASAN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime tarikh = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            HR_MAKLUMAT_KEWANGAN8 mKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh);
            HR_MAKLUMAT_KEWANGAN8_DETAIL mKewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh && s.HR_KOD_PELARASAN == HR_KOD_PELARASAN);
            PergerakanGajiModels model = new PergerakanGajiModels();
            if (mKewangan8 == null || mKewangan8Detail == null || peribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            model.HR_NO_PEKERJA = mKewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = mKewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = mKewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = mKewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = mKewangan8.HR_BULAN;
            model.HR_TAHUN = mKewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = mKewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = mKewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = mKewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = mKewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = mKewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = mKewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = mKewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = mKewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = mKewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = mKewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = mKewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = mKewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = mKewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = mKewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = mKewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = mKewangan8.HR_EKA;
            model.HR_ITP = mKewangan8.HR_ITP;
            model.HR_KEW8_IND = mKewangan8.HR_KEW8_IND;
            model.HR_BIL = mKewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = mKewangan8.HR_KOD_JAWATAN;

            model.HR_KEW8_ID = mKewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = mKewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = mKewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = mKewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = mKewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = mKewangan8.HR_NILAI_EPF;

            model.HR_KOD_PELARASAN = mKewangan8Detail.HR_KOD_PELARASAN;
            model.HR_MATRIKS_GAJI = mKewangan8Detail.HR_MATRIKS_GAJI;

            var grd2 = 0;
            if (mKewangan8Detail.HR_GRED != null)
            {
                grd2 = Convert.ToInt32(mKewangan8Detail.HR_GRED.Trim());
            }

            GE_PARAMTABLE gred2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd2).SingleOrDefault();

            model.HR_GRED = gred2.SHORT_DESCRIPTION;

            model.HR_JUMLAH_PERUBAHAN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN;
            model.HR_GAJI_BARU = mKewangan8Detail.HR_GAJI_BARU;
            model.HR_JENIS_PERGERAKAN = mKewangan8Detail.HR_JENIS_PERGERAKAN;
            model.HR_JUMLAH_PERUBAHAN_ELAUN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN_ELAUN;
            model.HR_STATUS_IND = mKewangan8Detail.HR_STATUS_IND;
            model.HR_ELAUN_KRITIKAL_BARU = mKewangan8Detail.HR_ELAUN_KRITIKAL_BARU;
            model.HR_NO_PEKERJA_PT = mKewangan8Detail.HR_NO_PEKERJA_PT;
            model.HR_PERGERAKAN_EKAL = mKewangan8Detail.HR_PERGERAKAN_EKAL;
            model.HR_PERGERAKAN_EWIL = mKewangan8Detail.HR_PERGERAKAN_EWIL;
            model.HR_GAJI_LAMA = mKewangan8Detail.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            GE_PARAMTABLE gred = new GE_PARAMTABLE();

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                var grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                if (gred == null)
                {
                    gred = new GE_PARAMTABLE();
                }
            }


            var peringkat2 = "";
            decimal tahap2 = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat2 = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(0, 2);
                if (mKewangan8Detail.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(3);
                    tahap2 = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual2 = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred2.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat2).SingleOrDefault();
            if (jadual2 == null)
            {
                jadual2 = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat2 = "";
            var jp2 = 0;
            if (jadual2.HR_PERINGKAT != null)
            {
                jadualPeringkat2 = jadual2.HR_PERINGKAT.Substring(1);
                jp2 = Convert.ToInt32(jadualPeringkat2);
            }

            HR_MATRIKS_GAJI matriks2 = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual2.HR_GRED_GAJI && s.HR_PERINGKAT == jp2 && s.HR_TAHAP == tahap2).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks2 == null)
            {
                matriks2 = new HR_MATRIKS_GAJI();
            }

            model.HR_GAJI_MIN = matriks2.HR_GAJI_MIN;
            model.HR_GAJI_MAX = matriks2.HR_GAJI_MAX;

            var peringkat = "";
            decimal tahap = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    tahap = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
            if (jadual == null)
            {
                jadual = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat = "";
            var jp = 0;
            if (jadual.HR_PERINGKAT != null)
            {
                jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                jp = Convert.ToInt32(jadualPeringkat);
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
            }


            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            float? HR_PERUBAHAN_KRITIKAL = 0;
            float? HR_PERGERAKAN_KRITIKAL = 0;

            float? HR_PERUBAHAN_WILAYAH = 0;
            float? HR_PERGERAKAN_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {

                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }
                }
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_WILAYAH = HR_WILAYAH;
            }
            if (HR_KRITIKAL > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_KRITIKAL = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_KRITIKAL;
            }
            if (HR_KRITIKAL > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_KRITIKAL = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_KRITIKAL;
            }


            if (HR_WILAYAH > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_WILAYAH = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_WILAYAH;
            }
            if (HR_WILAYAH > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_WILAYAH = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_WILAYAH;
            }

            ViewBag.HR_PERUBAHAN_KRITIKAL = HR_PERUBAHAN_KRITIKAL;
            ViewBag.HR_PERGERAKAN_KRITIKAL = HR_PERGERAKAN_KRITIKAL;
            ViewBag.HR_PERUBAHAN_WILAYAH = HR_PERUBAHAN_WILAYAH;
            ViewBag.HR_PERGERAKAN_WILAYAH = HR_PERGERAKAN_WILAYAH;

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == mKewangan8.HR_KOD_PERUBAHAN);

            ViewBag.HR_GAJI_POKOK = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GAJI_MIN_P = matriks.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_P = matriks.HR_GAJI_MAX;

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_KOD_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_SISTEM = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
            ViewBag.HR_JENIS_PERUBAHAN = kew8.HR_PENERANGAN;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            HR_MAKLUMAT_PERIBADI Pegawai = sPegawai.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (Pegawai == null)
            {
                Pegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = Pegawai.HR_NAMA_PEKERJA;

            return PartialView("_InfoPergerakanGaji", model);
        }

        public ActionResult EditPergerakanGaji(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string HR_KOD_PELARASAN, string key, string value, int? bulan)
        {
            if(id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KOD_PELARASAN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime tarikh = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            HR_MAKLUMAT_KEWANGAN8 mKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh);
            HR_MAKLUMAT_KEWANGAN8_DETAIL mKewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh && s.HR_KOD_PELARASAN == HR_KOD_PELARASAN);
            PergerakanGajiModels model = new PergerakanGajiModels();
            if(mKewangan8 == null || mKewangan8Detail == null || peribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            model.HR_NO_PEKERJA = mKewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = mKewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = mKewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = mKewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = mKewangan8.HR_BULAN;
            model.HR_TAHUN = mKewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = mKewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = mKewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = mKewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = mKewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = mKewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = mKewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = mKewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = mKewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = mKewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = mKewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = mKewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = mKewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = mKewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = mKewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = mKewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = mKewangan8.HR_EKA;
            model.HR_ITP = mKewangan8.HR_ITP;
            model.HR_KEW8_IND = mKewangan8.HR_KEW8_IND;
            model.HR_BIL = mKewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = mKewangan8.HR_KOD_JAWATAN;

            model.HR_KEW8_ID = mKewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = mKewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = mKewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = mKewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = mKewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = mKewangan8.HR_NILAI_EPF;

            model.HR_KOD_PELARASAN = mKewangan8Detail.HR_KOD_PELARASAN;
            model.HR_MATRIKS_GAJI = mKewangan8Detail.HR_MATRIKS_GAJI;

            var grd2 = 0;
            if(mKewangan8Detail.HR_GRED != null)
            {
                grd2 = Convert.ToInt32(mKewangan8Detail.HR_GRED.Trim());
            }

            GE_PARAMTABLE gred2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd2).SingleOrDefault();

            model.HR_GRED = gred2.SHORT_DESCRIPTION;

            model.HR_JUMLAH_PERUBAHAN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN;
            model.HR_GAJI_BARU = mKewangan8Detail.HR_GAJI_BARU;
            model.HR_JENIS_PERGERAKAN = mKewangan8Detail.HR_JENIS_PERGERAKAN;
            model.HR_JUMLAH_PERUBAHAN_ELAUN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN_ELAUN;
            model.HR_STATUS_IND = mKewangan8Detail.HR_STATUS_IND;
            model.HR_ELAUN_KRITIKAL_BARU = mKewangan8Detail.HR_ELAUN_KRITIKAL_BARU;
            model.HR_NO_PEKERJA_PT = mKewangan8Detail.HR_NO_PEKERJA_PT;
            model.HR_PERGERAKAN_EKAL = mKewangan8Detail.HR_PERGERAKAN_EKAL;
            model.HR_PERGERAKAN_EWIL = mKewangan8Detail.HR_PERGERAKAN_EWIL;
            model.HR_GAJI_LAMA = mKewangan8Detail.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            GE_PARAMTABLE gred = new GE_PARAMTABLE();

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                var grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                if (gred == null)
                {
                    gred = new GE_PARAMTABLE();
                }
            }


            var peringkat2 = "";
            decimal tahap2 = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat2 = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(0, 2);
                if (mKewangan8Detail.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(3);
                    tahap2 = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual2 = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred2.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat2).SingleOrDefault();
            if (jadual2 == null)
            {
                jadual2 = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat2 = "";
            var jp2 = 0;
            if (jadual2.HR_PERINGKAT != null)
            {
                jadualPeringkat2 = jadual2.HR_PERINGKAT.Substring(1);
                jp2 = Convert.ToInt32(jadualPeringkat2);
            }

            HR_MATRIKS_GAJI matriks2 = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual2.HR_GRED_GAJI && s.HR_PERINGKAT == jp2 && s.HR_TAHAP == tahap2).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks2 == null)
            {
                matriks2 = new HR_MATRIKS_GAJI();
            }

            model.HR_GAJI_MIN = matriks2.HR_GAJI_MIN;
            model.HR_GAJI_MAX = matriks2.HR_GAJI_MAX;

            var peringkat = "";
            decimal tahap = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    tahap = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
            if (jadual == null)
            {
                jadual = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat = "";
            var jp = 0;
            if (jadual.HR_PERINGKAT != null)
            {
                jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                jp = Convert.ToInt32(jadualPeringkat);
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
            }


            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            float? HR_PERUBAHAN_KRITIKAL = 0;
            float? HR_PERGERAKAN_KRITIKAL = 0;

            float? HR_PERUBAHAN_WILAYAH = 0;
            float? HR_PERGERAKAN_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }
                }
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_WILAYAH = HR_WILAYAH;
            }
            if(HR_KRITIKAL > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_KRITIKAL = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_KRITIKAL;
            }
            if(HR_KRITIKAL > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_KRITIKAL = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_KRITIKAL;
            }


            if (HR_WILAYAH > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_WILAYAH = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_WILAYAH;
            }
            if (HR_WILAYAH > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_WILAYAH = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_WILAYAH;
            }

            ViewBag.HR_PERUBAHAN_KRITIKAL = HR_PERUBAHAN_KRITIKAL;
            ViewBag.HR_PERGERAKAN_KRITIKAL = HR_PERGERAKAN_KRITIKAL;
            ViewBag.HR_PERUBAHAN_WILAYAH = HR_PERUBAHAN_WILAYAH;
            ViewBag.HR_PERGERAKAN_WILAYAH = HR_PERGERAKAN_WILAYAH;

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == mKewangan8.HR_KOD_PERUBAHAN);

            ViewBag.HR_GAJI_POKOK = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GAJI_MIN_P = matriks.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_P = matriks.HR_GAJI_MAX;

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_KOD_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_SISTEM = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
            ViewBag.HR_JENIS_PERUBAHAN = kew8.HR_PENERANGAN;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            HR_MAKLUMAT_PERIBADI Pegawai = sPegawai.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (Pegawai == null)
            {
                Pegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = Pegawai.HR_NAMA_PEKERJA;

            return PartialView("_EditPergerakanGaji", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPergerakanGaji(HR_MAKLUMAT_KEWANGAN8 kewangan8, HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail, PergerakanGajiModels model, string key, string value, int? bulan)
        {
            if (ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);

                //var Gred = Convert.ToInt32(model.HR_GRED);
                //GE_PARAMTABLE gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == Gred).ToList().OrderBy(s => s.SHORT_DESCRIPTION).SingleOrDefault();

                HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == model.HR_GRED && s.HR_GAJI_MIN == model.HR_GAJI_MIN && s.HR_GAJI_MAX == model.HR_GAJI_MAX && s.HR_GAJI_POKOK == model.HR_GAJI_BARU).OrderByDescending(s => s.HR_GAJI_MAX).FirstOrDefault();
                if (matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                }

                var peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                var tahap = Convert.ToString(matriks.HR_TAHAP);

                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);

                var bulan2 = Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                var tahun2 = DateTime.Now.Year;

                string gred = null;

                GE_PARAMTABLE gredList = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == model.HR_GRED).SingleOrDefault();
                if (gredList != null)
                {
                    gred = Convert.ToString(gredList.ORDINAL);
                }

                var pergerakan = db2.ZATUL_UPDATE_KEW_GERAK_GAJI2(bulan2, tahun2, peribadi.HR_NO_PEKERJA, model.HR_BUTIR_PERUBAHAN, model.HR_JENIS_PERGERAKAN, peringkat, tahap, model.HR_NP_FINALISED_HR, model.HR_FINALISED_IND_HR, model.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred, model.HR_KEW8_ID, model.HR_TARIKH_MULA, model.HR_KOD_PELARASAN);
                return RedirectToAction("PergerakanGaji", new { key = key, value = value, bulan = bulan } );
            }

            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            return View();
        }

        public ActionResult PadamPergerakanGaji(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string HR_KOD_PELARASAN, string key, string value, int? bulan)
        {
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KOD_PELARASAN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime tarikh = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            HR_MAKLUMAT_KEWANGAN8 mKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh);
            HR_MAKLUMAT_KEWANGAN8_DETAIL mKewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh && s.HR_KOD_PELARASAN == HR_KOD_PELARASAN);
            PergerakanGajiModels model = new PergerakanGajiModels();
            if (mKewangan8 == null || mKewangan8Detail == null || peribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            model.HR_NO_PEKERJA = mKewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = mKewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = mKewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = mKewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = mKewangan8.HR_BULAN;
            model.HR_TAHUN = mKewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = mKewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = mKewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = mKewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = mKewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = mKewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = mKewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = mKewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = mKewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = mKewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = mKewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = mKewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = mKewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = mKewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = mKewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = mKewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = mKewangan8.HR_EKA;
            model.HR_ITP = mKewangan8.HR_ITP;
            model.HR_KEW8_IND = mKewangan8.HR_KEW8_IND;
            model.HR_BIL = mKewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = mKewangan8.HR_KOD_JAWATAN;

            model.HR_KEW8_ID = mKewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = mKewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = mKewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = mKewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = mKewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = mKewangan8.HR_NILAI_EPF;

            model.HR_KOD_PELARASAN = mKewangan8Detail.HR_KOD_PELARASAN;
            model.HR_MATRIKS_GAJI = mKewangan8Detail.HR_MATRIKS_GAJI;

            var grd2 = 0;
            if (mKewangan8Detail.HR_GRED != null)
            {
                grd2 = Convert.ToInt32(mKewangan8Detail.HR_GRED.Trim());
            }

            GE_PARAMTABLE gred2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd2).SingleOrDefault();

            model.HR_GRED = gred2.SHORT_DESCRIPTION;

            model.HR_JUMLAH_PERUBAHAN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN;
            model.HR_GAJI_BARU = mKewangan8Detail.HR_GAJI_BARU;
            model.HR_JENIS_PERGERAKAN = mKewangan8Detail.HR_JENIS_PERGERAKAN;
            model.HR_JUMLAH_PERUBAHAN_ELAUN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN_ELAUN;
            model.HR_STATUS_IND = mKewangan8Detail.HR_STATUS_IND;
            model.HR_ELAUN_KRITIKAL_BARU = mKewangan8Detail.HR_ELAUN_KRITIKAL_BARU;
            model.HR_NO_PEKERJA_PT = mKewangan8Detail.HR_NO_PEKERJA_PT;
            model.HR_PERGERAKAN_EKAL = mKewangan8Detail.HR_PERGERAKAN_EKAL;
            model.HR_PERGERAKAN_EWIL = mKewangan8Detail.HR_PERGERAKAN_EWIL;
            model.HR_GAJI_LAMA = mKewangan8Detail.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            GE_PARAMTABLE gred = new GE_PARAMTABLE();

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                var grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                if (gred == null)
                {
                    gred = new GE_PARAMTABLE();
                }
            }


            var peringkat2 = "";
            decimal tahap2 = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat2 = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(0, 2);
                if (mKewangan8Detail.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(3);
                    tahap2 = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual2 = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred2.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat2).SingleOrDefault();
            if (jadual2 == null)
            {
                jadual2 = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat2 = "";
            var jp2 = 0;
            if (jadual2.HR_PERINGKAT != null)
            {
                jadualPeringkat2 = jadual2.HR_PERINGKAT.Substring(1);
                jp2 = Convert.ToInt32(jadualPeringkat2);
            }

            HR_MATRIKS_GAJI matriks2 = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual2.HR_GRED_GAJI && s.HR_PERINGKAT == jp2 && s.HR_TAHAP == tahap2).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks2 == null)
            {
                matriks2 = new HR_MATRIKS_GAJI();
            }

            model.HR_GAJI_MIN = matriks2.HR_GAJI_MIN;
            model.HR_GAJI_MAX = matriks2.HR_GAJI_MAX;

            var peringkat = "";
            decimal tahap = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    tahap = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
            if (jadual == null)
            {
                jadual = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat = "";
            var jp = 0;
            if (jadual.HR_PERINGKAT != null)
            {
                jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                jp = Convert.ToInt32(jadualPeringkat);
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
            }


            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            float? HR_PERUBAHAN_KRITIKAL = 0;
            float? HR_PERGERAKAN_KRITIKAL = 0;

            float? HR_PERUBAHAN_WILAYAH = 0;
            float? HR_PERGERAKAN_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {

                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }
                }
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_WILAYAH = HR_WILAYAH;
            }
            if (HR_KRITIKAL > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_KRITIKAL = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_KRITIKAL;
            }
            if (HR_KRITIKAL > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_KRITIKAL = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_KRITIKAL;
            }


            if (HR_WILAYAH > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_WILAYAH = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_WILAYAH;
            }
            if (HR_WILAYAH > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_WILAYAH = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_WILAYAH;
            }

            ViewBag.HR_PERUBAHAN_KRITIKAL = HR_PERUBAHAN_KRITIKAL;
            ViewBag.HR_PERGERAKAN_KRITIKAL = HR_PERGERAKAN_KRITIKAL;
            ViewBag.HR_PERUBAHAN_WILAYAH = HR_PERUBAHAN_WILAYAH;
            ViewBag.HR_PERGERAKAN_WILAYAH = HR_PERGERAKAN_WILAYAH;

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == mKewangan8.HR_KOD_PERUBAHAN);

            ViewBag.HR_GAJI_POKOK = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GAJI_MIN_P = matriks.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_P = matriks.HR_GAJI_MAX;

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_KOD_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_SISTEM = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
            ViewBag.HR_JENIS_PERUBAHAN = kew8.HR_PENERANGAN;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            HR_MAKLUMAT_PERIBADI Pegawai = sPegawai.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (Pegawai == null)
            {
                Pegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = Pegawai.HR_NAMA_PEKERJA;

            return PartialView("_PadamPergerakanGaji", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamPergerakanGaji(HR_MAKLUMAT_KEWANGAN8 kewangan8, HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail, PergerakanGajiModels model, string key, string value, int? bulan)
        {
            HR_MAKLUMAT_KEWANGAN8 kew8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN);
            db.HR_MAKLUMAT_KEWANGAN8.Remove(kew8);
            HR_MAKLUMAT_KEWANGAN8_DETAIL kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8Detail.HR_NO_PEKERJA && s.HR_KEW8_ID == kewangan8Detail.HR_KEW8_ID && s.HR_TARIKH_MULA == kewangan8Detail.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == kewangan8Detail.HR_KOD_PERUBAHAN && s.HR_KOD_PELARASAN == kewangan8Detail.HR_KOD_PELARASAN);
            db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Remove(kew8Detail);
            db.SaveChanges();
            return RedirectToAction("PergerakanGaji", new { key = key, value = value, bulan = bulan });
        }

        //[HttpPost, ActionName("TambahPergerakanGaji")]
        //[ValidateAntiForgeryToken]
        //public ActionResult TambahPergerakanGaji2(PergerakanGajiModels model,  string key2, string value2, int? bulan2, ManageMessageId? message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        List<HR_MAKLUMAT_PERIBADI> sPekerja = new List<HR_MAKLUMAT_PERIBADI>();
        //        //List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

        //        /*if (key2 == "1")
        //        {
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value2).ToList();

        //        }
        //        else if (key2 == "2")
        //        {
        //            value2 = value2.ToUpper();
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value2.ToUpper())).ToList();
        //        }
        //        else if (key2 == "3")
        //        {
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value2)).ToList();
        //        }

        //        else if (key2 == "4")
        //        {
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan2).ToList();
        //        }*/
        //        if(model.HR_MAKLUMAT_PERIBADI.Count() > 0)
        //        {
        //            foreach (var peribadi in model.HR_MAKLUMAT_PERIBADI)
        //            {
        //                HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA).SingleOrDefault();
        //                var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
        //                var tahun = DateTime.Now.Year;
        //                var peringkat = "";
        //                var tahap = "";
        //                if (model.HR_MATRIKS_GAJI != null)
        //                {
        //                    peringkat = model.HR_MATRIKS_GAJI.Substring(0, 2);
        //                    if (model.HR_MATRIKS_GAJI.Contains('T'))
        //                    {

        //                        var t = model.HR_MATRIKS_GAJI.Substring(3);
        //                        tahap = t;
        //                    }
        //                }
        //                var pergerakan = db2.ZATUL_INSERT_KEW_GERAK_GAJI2(bulan, tahun, peribadi.HR_NO_PEKERJA, model.HR_BUTIR_PERUBAHAN, model.HR_JENIS_PERGERAKAN, peringkat, tahap, model.HR_NP_FINALISED_HR, model.HR_NO_SURAT_KEBENARAN);
        //            }
        //        }

        //        return RedirectToAction("PergerakanGaji2", new { key = key2, value = value2, bulan = bulan2, Message = ManageMessageId.Success });
        //    }
        //    ViewBag.key2 = key2;
        //    ViewBag.value2 = value2;
        //    ViewBag.bulan2 = bulan2;
        //    return PartialView("_TambahPergerakanGaji", new { key = key2, value = value2, bulan = bulan2, Message = ManageMessageId.Error });
        //}

        public ActionResult BayarSemulaGaji()
        {
            return View();
        }

        

        public PartialViewResult TableKew8(string key, string value, string kod)
        {
            //ViewBag.jawatan = "";
            //ViewBag.gred = "";
            //ViewBag.kodGaji = "";
            //ViewBag.gaji = 0.00;
            //ViewBag.itp = 0.00;
            //ViewBag.awam = 0.00;
            ViewBag.HR_NO_PEKERJA = value;

            //ViewBag.key = key;
            //ViewBag.value = value;
            ViewBag.kod = kod;

            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == value);

            if(peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            ViewBag.HR_AKTIF_IND = peribadi.HR_AKTIF_IND;
            ViewBag.HR_GAJI_IND = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND;
            ViewBag.HR_TANGGUH_GERAKGAJI_IND = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TANGGUH_GERAKGAJI_IND;

            if (kod == "kew8")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00002" || s.HR_KOD_PERUBAHAN == "00003" || s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00005" || s.HR_KOD_PERUBAHAN == "00006" || s.HR_KOD_PERUBAHAN == "00007" || s.HR_KOD_PERUBAHAN == "00008" || s.HR_KOD_PERUBAHAN == "00009" || s.HR_KOD_PERUBAHAN == "00010" || s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018" || s.HR_KOD_PERUBAHAN == "00023" || s.HR_KOD_PERUBAHAN == "00027" || s.HR_KOD_PERUBAHAN == "00028" || s.HR_KOD_PERUBAHAN == "00039" || s.HR_KOD_PERUBAHAN == "00040" || s.HR_KOD_PERUBAHAN == "00042" || s.HR_KOD_PERUBAHAN == "00044" || s.HR_KOD_PERUBAHAN == "00045")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (kod == "TP")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00011" || s.HR_KOD_PERUBAHAN == "00014" || s.HR_KOD_PERUBAHAN == "00016" || s.HR_KOD_PERUBAHAN == "00020" || s.HR_KOD_PERUBAHAN == "00035" || s.HR_KOD_PERUBAHAN == "00044")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (kod == "CUTI")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && s.HR_KOD_PERUBAHAN == kod).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            
            //HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            //if (jawatan == null)
            //{
            //    jawatan = new HR_JAWATAN();
            //}
            //ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            //var kodGred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);

            //GE_PARAMTABLE gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == kodGred);
            //if (gred != null)
            //{
            //    ViewBag.gred = gred.SHORT_DESCRIPTION;
            //}
            //ViewBag.kodGaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            //ViewBag.gaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

            //var jawatan_ind = "";

            //if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
            //{
            //    jawatan_ind = "K" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            //}
            //else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
            //{
            //    jawatan_ind = "P" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            //}

            //List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();

            //List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == value).ToList();
            //if (elaunPotongan.Count() > 0)
            //{
            //    decimal? jumElaun = 0;
            //    decimal? jumAwam = 0;
            //    foreach (var item in elaunPotongan)
            //    {
            //        HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
            //        if (elaun != null)
            //        {
            //            jumElaun = item.HR_JUMLAH;
            //        }
            //        HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
            //        if (awam != null)
            //        {
            //            jumAwam = item.HR_JUMLAH;
            //        }

            //        if(item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_TARIKH_AKHIR >= DateTime.Now)
            //        {
            //            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
            //            elaun3.Add(elaun4);
            //        }
            //    }

            //    ViewBag.elaun3 = elaun3;

            //    ViewBag.itp = jumElaun;
            //    ViewBag.awam = jumAwam;
            //}

            //ViewBag.detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

            return PartialView("_TableKew8", model);
        }

        //[HttpPost]
        public FileStreamResult SlipKewangan8(PergerakanGajiModels model, string Kod)
        {
            //var tarikhMula = string.Format("{0:dd/MM/yyyy}", HR_TARIKH_MULA);
            //DateTime tarikhMula2 = Convert.ToDateTime(tarikhMula);

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if(kew8 == null)
            {
                kew8 = new HR_KEWANGAN8();
            }
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID);
            if(kewangan8 == null)
            {
                kewangan8 = new HR_MAKLUMAT_KEWANGAN8();
            }
            HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID);
            if (kewangan8Detail == null)
            {
                kewangan8Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
            }
            //&& s.HR_TARIKH_AKHIR >= DateTime.Now
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E" && s.HR_AKTIF_IND == "Y").ToList();

            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            if(peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if(jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            var grd = 0;
            if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED.Trim();
                grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
            }
            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();

            GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            if(jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }

            //GE_UNIT unit = mc.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN);
            //if(unit == null)
            //{
            //    unit = new GE_UNIT();
            //}

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA).Count() > 0);
            if(gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }
            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            GE_PARAMTABLE tarafJawatan = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
            if(tarafJawatan == null)
            {
                tarafJawatan = new GE_PARAMTABLE();
            }

            double? gaji = 0.00;
            double? jum = 0.00;
            if (kewangan8 != null && kewangan8Detail != null && Kod == "00031")
            {

                var EPF = Convert.ToDouble(kewangan8.HR_NILAI_EPF);
                var bulan = (0.055 * kewangan8.HR_JUMLAH_BULAN);
                jum = (Convert.ToDouble(kewangan8Detail.HR_JUMLAH_PERUBAHAN) + EPF);
                gaji = jum / bulan;

            }

            var html = "<html><head>";
            html += "<title>Slip</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";
            if(Kod == "00031")
            {
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                //html += "<thead>";
                html += "<tr>";
                html += "<td valign='top' rowspan='2' width='37%' style='font-size: 74%'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                html += "<td width='60%' style='font-size: 74%'><u><strong>GANJARAN YANG DITERIMA :</strong></u></td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td width='60%' style='font-size: 74%'>[&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", gaji) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + kewangan8.HR_JUMLAH_BULAN + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5.5%&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;]&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF) + "<br />=&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", jum) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF) + "<br />=&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", kewangan8Detail.HR_JUMLAH_PERUBAHAN) + "<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;========</div></td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td width='37%' style='font-size: 74%'><u><strong>CATATAN :</strong></u></td>";
                html += "<td width='60%' style='font-size: 74%'></td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td width='37%' style='font-size: 74%'>" + kewangan8.HR_CATATAN + "</td>";
                html += "<td width='60%' style='font-size: 74%'></td>";
                html += "</tr>";

                html += "</table>";
            }
            if(Kod == "00025")
            {
                double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                {
                    gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                    matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                }
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                html += "<tr>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='37%' style='font-size: 74%'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                html += "<td valign='top'  align='center' rowspan='" + (elaunPotongan.Count() + 1) + "' width='12%' style='font-size: 74%'><p>" + string.Format("{0:dd/MM/yyyy}",kewangan8.HR_TARIKH_MULA) + "</p><p>"+ string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) + "</p></td>";
                html += "<td valign='top' width='12%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='24%' style='font-size: 74%'>" + kewangan8.HR_CATATAN + "</td>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='16%' style='font-size: 74%'>" + kewangan8.HR_NO_SURAT_KEBENARAN + "</td>";
                html += "</tr>";

                foreach(HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                {
                    HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                    html += "<tr>";
                    html += "<td width='12%' style='font-size: 74%'><u><strong>" + elaun2.HR_SINGKATAN + "</strong></u><br />RM " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "</td>";
                    html += "</tr>";
                }

                html += "</table>";
            }

            if (Kod == "kew8" || Kod == "00026" || Kod == "00022" || Kod == "00037")
            {
                double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                {
                    gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                    matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                }
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                html += "<tr>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='37%' style='font-size: 74%'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                html += "<td valign='top'  align='center' rowspan='" + (elaunPotongan.Count() + 1) + "' width='12%' style='font-size: 74%'><p>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + "</p><p>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) + "</p></td>";
                html += "<td valign='top' width='12%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='24%' style='font-size: 74%'>" + kewangan8.HR_CATATAN + "</td>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='16%' style='font-size: 74%'>" + kewangan8.HR_NO_SURAT_KEBENARAN + "</td>";
                html += "</tr>";

                foreach (HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                {
                    HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                    html += "<tr>";
                    html += "<td width='12%' style='font-size: 74%'><u><strong>" + elaun2.HR_SINGKATAN + "</strong></u><br />RM / (%) " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "</td>";
                    html += "</tr>";
                }

                html += "</table>";
            }

            if (Kod == "00030" || Kod == "TP" || Kod == "CUTI" || Kod == "00015")
            {
                double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                {
                    gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                    matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                }
               
                HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                if (kewangan8Detail2 == null)
                {
                    kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }
                if(kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                {
                    kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                }

                decimal? jumElaun = 0;
                decimal? jumPotongan = 0;
                html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                html += "<tr>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='35%' style='font-size: 74%'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                html += "<td valign='top' align='center' rowspan='" + (elaunPotongan.Count() + 1) + "' width='12%' style='font-size: 74%'>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) + "</td>";
                html += "<td valign='top' width='12%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                html += "<td valign='top' width='16%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN) + "</td>";
                html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='26%' style='font-size: 74%'>" + kewangan8.HR_CATATAN + "</td>";
                html += "</tr>";

                foreach (HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                {
                    
                    HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                    HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail3 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_KOD_PELARASAN == elaun2.HR_KOD_POTONGAN);
                    if (kewangan8Detail3 == null)
                    {
                        kewangan8Detail3 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }
                    if (kewangan8Detail3.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail3.HR_JUMLAH_PERUBAHAN = 0;
                    }

                    jumElaun += Math.Abs(Convert.ToDecimal(kewangan8Detail3.HR_JUMLAH_PERUBAHAN));

                    html += "<tr>";
                    html += "<td valign='top' width='12%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "<br />( " + elaun2.HR_SINGKATAN + " )</td>";
                    html += "<td valign='top' width='16%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", kewangan8Detail3.HR_JUMLAH_PERUBAHAN) + "</td>";
                    html += "</tr>";
                }

                jumPotongan = Math.Abs(Convert.ToDecimal(kewangan8Detail2.HR_JUMLAH_PERUBAHAN)) + jumElaun;

                html += "<tr>";
                html += "<td valign='top' width='35%' style='font-size: 74%'></td>";
                html += "<td valign='top' align='center' width='12%' style='font-size: 74%'></td>";
                html += "<td valign='top' width='12%' style='font-size: 74%'></td>";
                html += "<td valign='top' width='16%' style='font-size: 74%'>Jumlah Potongan =</td>";
                html += "<td valign='top' width='26%' style='font-size: 74%'>RM " + string.Format("{0:#,0.00}", -Math.Abs(Convert.ToDecimal(jumPotongan))) + "</td>";
                html += "</tr>";

                html += "</table>";
            }

            html += "<br /><br />";

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");

                var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "April" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "September" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };

                var Bulan = "";
                foreach (var m in associativeArray)
                {
                    if (DateTime.Now.Month == m.Key)
                    {
                        Bulan = m.Value;
                    }

                }

                var KPLama = "";
                if (peribadi.HR_NO_KPLAMA != null)
                {
                    KPLama = " / " + peribadi.HR_NO_KPLAMA;
                }

                //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentFont4 = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Paragraph noPekerja = new iTextSharp.text.Paragraph(peribadi.HR_NAMA_PEKERJA, contentFont2);
                noPekerja.Alignment = Element.ALIGN_JUSTIFIED;
                noPekerja.IndentationLeft = 56f;

                iTextSharp.text.Paragraph IC = new iTextSharp.text.Paragraph(peribadi.HR_NO_KPBARU + KPLama, contentFont2);
                IC.Alignment = Element.ALIGN_JUSTIFIED;

                iTextSharp.text.Paragraph jwt = new iTextSharp.text.Paragraph(jawatan.HR_NAMA_JAWATAN, contentFont2);
                jwt.Alignment = Element.ALIGN_JUSTIFIED;
                jwt.IndentationLeft = 35f;

                iTextSharp.text.Paragraph jawatan2 = new iTextSharp.text.Paragraph(jawatan.HR_NAMA_JAWATAN, contentFont2);
                jawatan2.Alignment = Element.ALIGN_JUSTIFIED;
                

                iTextSharp.text.Paragraph gred2 = new iTextSharp.text.Paragraph(gred.SHORT_DESCRIPTION, contentFont2);
                gred2.Alignment = Element.ALIGN_JUSTIFIED;


                iTextSharp.text.Paragraph tJawatan = new iTextSharp.text.Paragraph(tarafJawatan.SHORT_DESCRIPTION, contentFont2);
                tJawatan.Alignment = Element.ALIGN_JUSTIFIED;

                iTextSharp.text.Paragraph jabatan2 = new iTextSharp.text.Paragraph(jabatan.GE_KETERANGAN_JABATAN, contentFont2);
                jabatan2.Alignment = Element.ALIGN_JUSTIFIED;
                jabatan2.IndentationLeft = 99f;

                iTextSharp.text.Paragraph votGaji = new iTextSharp.text.Paragraph("11-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + gajiUpah.HR_VOT_UPAH, contentFont2);
                votGaji.Alignment = Element.ALIGN_JUSTIFIED;

                //pic.ScaleToFit(100f, 80f);
                //pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                //pic.IndentationRight = 10f;

                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));
                //document.Add(pic);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                PdfPTable table = new PdfPTable(3);
                table.TotalWidth = 510f;
                table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.LockedWidth = true;
                float[] widths = new float[] { 7f, 1f, 2f};

                table.SetWidths(widths);

                table.HorizontalAlignment = 1;

                //leave a gap before and after the table

                //table.SpacingBefore = 20f;

                //table.SpacingAfter = 30f;

                PdfPCell cell = new PdfPCell();
                cell.AddElement(noPekerja);
                cell.PaddingTop = 8f;
                cell.BorderWidthBottom = 0f;
                cell.BorderWidthLeft = 0f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthRight = 0f;

                PdfPCell cell2 = new PdfPCell();
                cell2.AddElement(IC);
                cell2.PaddingTop = 8f;
                cell2.Colspan = 4;
                cell2.BorderWidthBottom = 0f;
                cell2.BorderWidthLeft = 0f;
                cell2.BorderWidthTop = 0f;
                cell2.BorderWidthRight = 0f;

                PdfPCell cell3 = new PdfPCell();
                cell3.AddElement(jwt);
                cell3.PaddingTop = 4f;
                cell3.BorderWidthBottom = 0f;
                cell3.BorderWidthLeft = 0f;
                cell3.BorderWidthTop = 0f;
                cell3.BorderWidthRight = 0f;

                PdfPCell cell4 = new PdfPCell();
                cell4.AddElement(gred2);
                cell4.PaddingTop = 4f;
                cell4.BorderWidthBottom = 0f;
                cell4.BorderWidthLeft = 0f;
                cell4.BorderWidthTop = 0f;
                cell4.BorderWidthRight = 0f;

                PdfPCell cell5 = new PdfPCell();
                cell5.AddElement(tJawatan);
                cell5.PaddingTop = 4f;
                cell5.BorderWidthBottom = 0f;
                cell5.BorderWidthLeft = 0f;
                cell5.BorderWidthTop = 0f;
                cell5.BorderWidthRight = 0f;

                PdfPCell cell6 = new PdfPCell();
                cell6.AddElement(jabatan2);
                cell6.PaddingTop = 4f;
                cell6.BorderWidthBottom = 0f;
                cell6.BorderWidthLeft = 0f;
                cell6.BorderWidthTop = 0f;
                cell6.BorderWidthRight = 0f;

                PdfPCell cell7 = new PdfPCell();
                cell7.AddElement(new iTextSharp.text.Paragraph("Vot Gaji :", contentFont2));
                cell7.PaddingTop = 4f;
                cell7.BorderWidthBottom = 0f;
                cell7.BorderWidthLeft = 0f;
                cell7.BorderWidthTop = 0f;
                cell7.BorderWidthRight = 0f;

                PdfPCell cell8 = new PdfPCell();
                cell8.AddElement(votGaji);
                cell8.PaddingTop = 4f;
                cell8.BorderWidthBottom = 0f;
                cell8.BorderWidthLeft = 0f;
                cell8.BorderWidthTop = 0f;
                cell8.BorderWidthRight = 0f;



                //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                //table.AddCell(cell);
                //table.AddCell(new iTextSharp.text.Paragraph("NAMA", contentFont));
                //table.AddCell(new iTextSharp.text.Paragraph(" : ", contentFont));
                table.AddCell(cell);
                table.AddCell(cell2);

                table.AddCell(cell3);
                table.AddCell(cell4);
                table.AddCell(cell5);

                table.AddCell(cell6);
                table.AddCell(cell7);
                table.AddCell(cell8);

                document.Add(table);

                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                //PdfPTable table2 = new PdfPTable(2);
                //table2.TotalWidth = 600f;
                //table2.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //table2.LockedWidth = true;
                //float[] widths2 = new float[] { 2f, 2f };

                //table2.SetWidths(widths2);

                //table2.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right

                ////leave a gap before and after the table

                ////table.SpacingBefore = 20f;

                ////table.SpacingAfter = 30f;

                

                //Chunk c2 = new Chunk("GANJARAN YANG DITERIMA :", contentFont);
                //c2.SetUnderline(0.5f, -1.5f);
                //Chunk c = new Chunk("BUTIRAN :", contentFont);
                //c.SetUnderline(0.5f, -1.5f);
                //Chunk c3 = new Chunk("CATATAN :", contentFont);
                //c3.SetUnderline(0.5f, -1.5f);

                //Chunk total = new Chunk(string.Format("{0:#,0.00}", kewangan8Detail.HR_JUMLAH_PERUBAHAN), contentFont);
                //total.SetUnderline(0.5f, -1.5f); ;

                //table2.AddCell(new iTextSharp.text.Paragraph(c));
                //table2.AddCell(new iTextSharp.text.Paragraph(c2));
                //table2.AddCell(new iTextSharp.text.Paragraph(kewangan8.HR_BUTIR_PERUBAHAN, contentFont2));
                //table2.AddCell(new iTextSharp.text.Paragraph("[     RM     "+ string.Format("{0:#,0.00}", gaji) + "     X     "+ kewangan8.HR_JUMLAH_BULAN +"     X     5.5%     ]     -     RM     "+ string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF)+"\n=     RM     "+ string.Format("{0:#,0.00}", jum) +"     -     RM     "+ string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF)+"\n=     RM     "+ total, contentFont2));

                //table2.AddCell(new iTextSharp.text.Paragraph(c3));
                //table2.AddCell(new iTextSharp.text.Paragraph(""));
                //table2.AddCell(new iTextSharp.text.Paragraph(kewangan8.HR_CATATAN, contentFont2));
                //table2.AddCell(new iTextSharp.text.Paragraph(""));

                //document.Add(table2);

                //document.Add(new iTextSharp.text.Paragraph("\n"));
                //document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                //iTextSharp.text.Font contentFont3 = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL);



                iTextSharp.text.Paragraph paragraph5 = new iTextSharp.text.Paragraph(DateTime.Now.Day + "-" + Bulan + "-" + DateTime.Now.Year+ "\nKod Kew. 8 : "+ kewangan8.HR_KEW8_ID, contentFont2);
                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);
                ct.SetSimpleColumn(70f, 395f, 395f, 150f);
                ct.SetText(paragraph5);
                ct.Go();

                document.Close();
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
        }

        public GE_PARAMTABLE cariGred(int? kodGred, string kodGred2)
        {
            GE_PARAMTABLE gred = new GE_PARAMTABLE();
            if (kodGred != null)
            {
                gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == kodGred);
            }
            else
            {
                gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == kodGred2);
            }
            
            if(gred == null)
            {
                gred = new GE_PARAMTABLE();
            }
            return gred;
        }

        public HR_MATRIKS_GAJI cariMatriks(string gred, string matrik, decimal? gaji)
        {
            int? peringkat = null;
            if (matrik != null)
            {
                matrik = matrik.Trim();

                if (matrik.Substring(0, 1) == "P" && matrik.Length >= 2)
                {
                    peringkat = Convert.ToInt32(matrik.Substring(1, 1));
                }
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == gred && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gaji).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
                matriks.HR_GAJI_MIN = 0;
                matriks.HR_GAJI_MAX = 0;
                matriks.HR_GAJI_POKOK = 0;
            }

            return matriks;
        }

        public ActionResult TambahKew8(HR_MAKLUMAT_KEWANGAN8 model, string Kod)
        {
            model.HR_TARIKH_KEYIN = DateTime.Now;
            ViewBag.Kod = Kod;
            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if(kewangan8 != null)
            {
                model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                //ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }
            
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);

            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;
            ViewBag.HR_MATRIKS_GAJI_LAMA = mPekerjaan.HR_MATRIKS_GAJI;
            ViewBag.HR_TARIKH_MULA = null;
            ViewBag.HR_TARIKH_AKHIR = null;
            if (Kod == "CUTI")
            {
                ViewBag.HR_TARIKH_MULA = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }
            }

            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
                ViewBag.HR_TARIKH_TAMAT = null;
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if(mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}",mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }

            }

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);


            HR_MATRIKS_GAJI matriksPekerja = cariMatriks(cariGred(kodGred, null).SHORT_DESCRIPTION, mPekerjaan.HR_MATRIKS_GAJI, mPekerjaan.HR_GAJI_POKOK);

            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerja.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerja.HR_GAJI_MAX;

            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == kodGred);
            if (gred != null)
            {
                ViewBag.HR_GRED_LAMA = gred.SHORT_DESCRIPTION;
            }
            ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;

            decimal? gaji = 0;
            if (mPekerjaan.HR_GAJI_POKOK != null)
            {
                gaji = mPekerjaan.HR_GAJI_POKOK;
            }
            ViewBag.HR_GAJI_LAMA = gaji;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA).Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

            var jawatan_ind = "";

            if (mPekerjaan.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + mPekerjaan.HR_TARAF_JAWATAN;
            }
            else if (mPekerjaan.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + mPekerjaan.HR_TARAF_JAWATAN;
            }

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }
                    // && item.HR_TARIKH_AKHIR >= DateTime.Now
                    if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                        elaun4.HR_NILAI = item.HR_JUMLAH;
                        elaun3.Add(elaun4);
                        
                    }
                    if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                        potongan3.Add(potongan4);

                    }
                }

                ViewBag.elaun3 = elaun3;

                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }

            ViewBag.HR_KAKITANGAN_IND = mPekerjaan.HR_KAKITANGAN_IND;

            ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_SINGKATAN == "EBGK"), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", mPekerjaan.HR_KAKITANGAN_IND == "Y"? "E0069": "E0070");

            if (Kod == "00039")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            }

            if (Kod == "00024")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            model.HR_NP_FINALISED_HR = namaPegawai.HR_NO_PEKERJA;
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8, "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim());
                if(jGaji != null)
                {
                    gredList.Add(sGred);
                }
                
            }
            ViewBag.gredList = gredList;

            return PartialView("_TambahKew8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKew8([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_TARIKH_AKHIR,HR_BULAN,HR_TAHUN,HR_TARIKH_KEYIN,HR_BUTIR_PERUBAHAN,HR_CATATAN,HR_NO_SURAT_KEBENARAN,HR_AKTIF_IND,HR_NP_UBAH_HR,HR_TARIKH_UBAH_HR,HR_NP_FINALISED_HR,HR_TARIKH_FINALISED_HR,HR_FINALISED_IND_HR,HR_NP_UBAH_PA,HR_TARIKH_UBAH_PA,HR_NP_FINALISED_PA,HR_TARIKH_FINALISED_PA,HR_FINALISED_IND_PA,HR_EKA,HR_ITP,HR_KEW8_IND,HR_BIL,HR_KOD_JAWATAN,HR_KEW8_ID,HR_LANTIKAN_IND,HR_TARIKH_SP,HR_SP_IND,HR_JUMLAH_BULAN,HR_NILAI_EPF,HR_GAJI_LAMA,HR_MATRIKS_GAJI_LAMA,HR_GRED_LAMA")] HR_MAKLUMAT_KEWANGAN8 model, [Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_KOD_PELARASAN,HR_MATRIKS_GAJI,HR_GRED,HR_JUMLAH_PERUBAHAN,HR_GAJI_BARU,HR_JENIS_PERGERAKAN,HR_JUMLAH_PERUBAHAN_ELAUN,HR_STATUS_IND,HR_ELAUN_KRITIKAL_BARU,HR_KEW8_ID,HR_NO_PEKERJA_PT,HR_PERGERAKAN_EKAL,HR_PERGERAKAN_EWIL,HR_GAJI_LAMA")] HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, decimal? HR_JUMLAH_POTONGAN, IEnumerable<HR_POTONGAN> sPotongan, string Kod, DateTime? HR_TARIKH_TAMAT)
        {
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.Where(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y").FirstOrDefault();
            HR_MAKLUMAT_PERIBADI pekerja = mPeribadi.SingleOrDefault(s => s.HR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            if (ModelState.IsValid)
            {
                var jawatan_ind = "";
                if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    jawatan_ind = "K" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }
                else if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    jawatan_ind = "P" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }

                HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
                if (gajiUpah == null)
                {
                    gajiUpah = new HR_GAJI_UPAHAN();
                }

                HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
                if (gajiUpah == null)
                {
                    tggkk = new HR_GAJI_UPAHAN();
                }

                HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                if(potongan2 == null)
                {
                    potongan2 = new HR_POTONGAN();
                }

                var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                var incrementID = lastID.HR_KEW8_ID + 1;

                if(Kod == "00031")
                {
                    var jumBulan = Convert.ToString(model.HR_JUMLAH_BULAN);
                    var EPF = Convert.ToString(model.HR_NILAI_EPF);
                    var bil = jumBulan + EPF;
                    model.HR_BIL = Convert.ToDecimal(bil);
                }
                
                if(Kod == "TP")
                {
                    model.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                    model.HR_TARIKH_AKHIR = Convert.ToDateTime(HR_TARIKH_TAMAT);
                    modelDetail.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                }

                //model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                model.HR_BULAN = DateTime.Now.Month;
                model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                model.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                model.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                //HR_JAWATAN jwt4 = db.HR_JAWATAN.SingleOrDefault(s => s.HR_NAMA_JAWATAN == model.HR_KOD_JAWATAN);
                //if (jwt4 == null)
                //{
                //    jwt4 = new HR_JAWATAN();
                //}
                //model.HR_KOD_JAWATAN = jwt4.HR_KOD_JAWATAN;

                //if(model.HR_TARIKH_MULA == null)
                //{
                //    model.HR_TARIKH_MULA = DateTime.Now;
                //}

                model.HR_TARIKH_KEYIN = DateTime.Now;
                //if(model.HR_KOD_PERUBAHAN == "00026")
                //{
                //    model.HR_TARIKH_AKHIR = model.HR_TARIKH_MULA;
                //}

                model.HR_KEW8_ID = incrementID;
                model.HR_FINALISED_IND_HR = "T";
                model.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                model.HR_TARIKH_UBAH_HR = DateTime.Now;

                if(Kod == "CUTI")
                {
                    model.HR_TARIKH_SP = new DateTime(model.HR_TARIKH_MULA.Year, model.HR_TARIKH_MULA.AddMonths(1).Month, 1);
                    model.HR_SP_IND = "Y";
                }

                db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                db.SaveChanges();

                if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015" || Kod == "00024" || Kod == "00039")
                {
                    var no = 0;
                    foreach(HR_POTONGAN potongan in sPotongan)
                    {
                        HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                        //modelDetail.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;

                        //modelDetail.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI;
                        //modelDetail.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                        ////modelDetail.HR_TARIKH_MULA = DateTime.Now;
                        //modelDetail.HR_KEW8_ID = incrementID;
                        //modelDetail.HR_STATUS_IND = "T";
                        //modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                        //modelDetail.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        //modelDetail.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;


                        modelDetail2.HR_NO_PEKERJA = modelDetail.HR_NO_PEKERJA;
                        modelDetail2.HR_KOD_PERUBAHAN = modelDetail.HR_KOD_PERUBAHAN;
                        modelDetail2.HR_TARIKH_MULA = modelDetail.HR_TARIKH_MULA;
                        

                        if (potongan.HR_NILAI == null)
                        {
                            potongan.HR_NILAI = 0;
                        }

                        if(Kod == "00031" || Kod == "00024" || Kod == "00039")
                        {
                            modelDetail2.HR_KOD_PELARASAN = modelDetail.HR_KOD_PELARASAN;
                            potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                        }
                        else
                        {
                            modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                            if (Kod == "00036")
                            {
                                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                            }
                            else
                            {
                                potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                            }
                        }

                        if (no == 0 && (Kod != "00031" && Kod != "00024" && Kod != "00039"))
                        {
                            //potongan.HR_NILAI = -potongan.HR_NILAI;
                            if (potongan.HR_NILAI > 0)
                            {
                                //tunggakan
                                modelDetail2.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                            }
                            else
                            {
                                //potongan
                                modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                            }
                        }

                        if (Kod != "00036")
                        {
                            modelDetail2.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                            modelDetail2.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                            modelDetail2.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        }
                        else
                        {
                            modelDetail2.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                            modelDetail2.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                            modelDetail2.HR_GAJI_BARU = modelDetail.HR_GAJI_BARU;
                        }

                        modelDetail2.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI;

                        modelDetail2.HR_JENIS_PERGERAKAN = modelDetail.HR_JENIS_PERGERAKAN;
                        modelDetail2.HR_JUMLAH_PERUBAHAN_ELAUN = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                        modelDetail2.HR_STATUS_IND = "T";

                        if (Kod == "00039")
                        {
                            if (model.HR_KEW8_IND != "P")
                            {
                                modelDetail.HR_JUMLAH_PERUBAHAN = 0;
                                modelDetail.HR_STATUS_IND = null;
                            }
                            else
                            {
                                modelDetail.HR_STATUS_IND = "P";
                            }
                        }

                        modelDetail2.HR_ELAUN_KRITIKAL_BARU = modelDetail.HR_ELAUN_KRITIKAL_BARU;

                        modelDetail2.HR_KEW8_ID = incrementID;
                        modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                        modelDetail2.HR_PERGERAKAN_EKAL = modelDetail.HR_PERGERAKAN_EKAL;
                        modelDetail2.HR_PERGERAKAN_EWIL = modelDetail.HR_PERGERAKAN_EWIL;
                        modelDetail2.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        //modelDetail2.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                        //modelDetail2.HR_GRED_LAMA = modelDetail.HR_GRED;

                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail2);

                        db.SaveChanges();

                        no++;
                    } 
                }
                ////else if(Kod != "00022" && Kod != "00037" && Kod != "kew8" && Kod != "00025")
                //else if (Kod == "00039" || Kod == "00024")
                //{
                //    //modelDetail.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                //    //modelDetail.HR_TARIKH_MULA = DateTime.Now;
                //    modelDetail.HR_KEW8_ID = incrementID;
                //    //if(Kod != "00022" && Kod != "00037")
                //    //{
                //        modelDetail.HR_STATUS_IND = "T";
                //    //}
                    
                //    //if(Kod != "00036")
                //    //{
                //    //    modelDetail.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                //    //    modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                //    //    modelDetail.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                //    //}
                //    //else
                //    //{
                //    //    //modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                //    //    modelDetail.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                //    //}

                //    if(Kod == "00039")
                //    {
                //        if(model.HR_KEW8_IND != "P")
                //        {
                //            modelDetail.HR_JUMLAH_PERUBAHAN = 0;
                //            modelDetail.HR_STATUS_IND = null;
                //        }
                //        else
                //        {
                //            modelDetail.HR_STATUS_IND = "P";
                //        }
                //    }

                //    //modelDetail.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                //    //modelDetail.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                //    modelDetail.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                //    db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail);
                //    db.SaveChanges();
                //}
                
                var redirect = RedirectLink(Kod);

                return RedirectToAction(redirect, new { key = "1", value = model.HR_NO_PEKERJA });
            }
            ViewBag.HR_KOD_JAWATAN = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;

            ViewBag.Kod = Kod;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                foreach (var item in elaunPotongan)
                {
                    // && item.HR_TARIKH_AKHIR >= DateTime.Now
                    if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                        elaun3.Add(elaun4);

                    }
                    if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                        potongan3.Add(potongan4);

                    }
                }

            }
            if(Kod == "00031")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015"), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            }
            
            if (Kod == "00039")
            {
                if(model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }
                
            }

            if (Kod == "00024")
            {
                if (model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }
                    
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            ViewBag.pengesahan = pengesahan;

            
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8, "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }


            decimal? gaji = 0;
            if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK != null)
            {
                gaji = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            }
            ViewBag.HR_GAJI_LAMA = gaji;

            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && (s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI.Trim() == modelDetail.HR_GRED));
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList;

            return PartialView("_TambahKew8", model);
        }

        public ActionResult InfoKew8(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string Kod)
        {
            ViewBag.Kod = Kod;
            var date = Convert.ToDateTime(HR_TARIKH_MULA);
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (model == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (Detail2 == null)
            {
                Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                Detail2.HR_JUMLAH_PERUBAHAN = 0;
            }
            int? gredDetail = Convert.ToInt32(model.HR_GRED_LAMA);

            ViewBag.HR_JUMLAH_PERUBAHAN_ELAUN = Detail2.HR_JUMLAH_PERUBAHAN_ELAUN;
            ViewBag.HR_JUMLAH_PERUBAHAN = Detail2.HR_JUMLAH_PERUBAHAN;

            HR_MATRIKS_GAJI matriksDetail2 = cariMatriks(cariGred(gredDetail, null).SHORT_DESCRIPTION, model.HR_MATRIKS_GAJI_LAMA, model.HR_GAJI_LAMA);

            ViewBag.HR_GRED = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_BARU = matriksDetail2.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_BARU = matriksDetail2.HR_GAJI_MAX;
            ViewBag.HR_GAJI_BARU = Detail2.HR_GAJI_BARU;
            ViewBag.HR_MATRIKS_GAJI = Detail2.HR_MATRIKS_GAJI;

            ViewBag.kodGaji = matriksDetail2.HR_KOD_GAJI;

            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (kewangan8 != null)
            {
                ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }

            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(model.HR_GRED_LAMA);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, model.HR_MATRIKS_GAJI_LAMA, model.HR_GAJI_LAMA);

            //var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);
            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(gredPekerjaan, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = model.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == model.HR_KOD_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            //int? kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);

            //ViewBag.gred = cariGred(kodGred, null).SHORT_DESCRIPTION;
            //if(cariGred(gredDetail, null).SHORT_DESCRIPTION != null)
            //{
            //    ViewBag.gred = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            //}
            
            //ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;
            //ViewBag.gaji = mPekerjaan.HR_GAJI_POKOK;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.pGaji = potongan2.HR_KOD_POTONGAN;


            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            ViewBag.nilaiPGaji = 0;
            ViewBag.nilaiPotongan = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }

                    if ((Kod == "00030" && model.HR_KEW8_IND == "H") || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015")
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4 != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_POTONGAN);
                                if (Detail == null)
                                {
                                    Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                }
                                if (Detail.HR_JUMLAH_PERUBAHAN == null)
                                {
                                    Detail.HR_JUMLAH_PERUBAHAN = 0;
                                }
                                elaun4.HR_NILAI = item.HR_JUMLAH;
                                elaun3.Add(elaun4);

                                HR_POTONGAN potongan4 = new HR_POTONGAN();
                                potongan4.HR_KOD_POTONGAN = Detail.HR_KOD_PELARASAN;
                                potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                ViewBag.nilaiPotongan += Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN));
                                potongan3.Add(potongan4);
                            }
                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "G" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                            if (Detail == null)
                            {
                                Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            }
                            if (Detail.HR_JUMLAH_PERUBAHAN == null)
                            {
                                Detail.HR_JUMLAH_PERUBAHAN = 0;
                            }
                            ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN));
                        }

                    }
                    else
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            elaun4.HR_NILAI = item.HR_JUMLAH;
                            elaun3.Add(elaun4);

                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                            potongan3.Add(potongan4);

                        }
                    }
                }
                ViewBag.nilaiPotongan += ViewBag.nilaiPGaji;
                if (Kod == "00030" && model.HR_KEW8_IND == "A")
                {
                    ViewBag.nilaiPGaji = model.HR_BIL;
                    ViewBag.nilaiPotongan = model.HR_BIL;
                }

                ViewBag.elaun3 = elaun3;
                ViewBag.potongan3 = potongan3;
                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }


            //if (HR_KOD_PERUBAHAN == "00030")
            //{
            //    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            //    if (modelDetail.Count() > 0)
            //    {
            //        foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
            //        {
            //            if(item.HR_JUMLAH_PERUBAHAN == null)
            //            {
            //                item.HR_JUMLAH_PERUBAHAN = 0;
            //            }
            //            if(item.HR_KOD_PELARASAN != potongan2.HR_KOD_POTONGAN)
            //            {
            //                HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                elaun3.Add(elaun4);

            //                HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                potongan4.HR_NILAI = item.HR_JUMLAH_PERUBAHAN;
            //                potongan3.Add(potongan4);
            //            }
            //            else
            //            {
            //                ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(item.HR_JUMLAH_PERUBAHAN));
            //            }
            //        }
            //        ViewBag.elaun3 = elaun3;
            //        ViewBag.potongan3 = potongan3;

            //    }
            //}

            if (Kod == "00031" || Kod == "00039" || Kod == "00024")
            {
                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
                if (modelDetail == null)
                {
                    modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                ViewBag.kodPelarasan = modelDetail.HR_KOD_PELARASAN;

                if (Kod == "00031")
                {
                    ViewBag.HR_JUMLAH_PERUBAHAN = modelDetail.HR_JUMLAH_PERUBAHAN;
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015"), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                }

                if (Kod == "00039")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                }

                if (Kod == "00024")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                }
            }

            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            HR_MAKLUMAT_PERIBADI pengesahan2 = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if (pengesahan2 == null)
            {
                pengesahan2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.pengesahan2 = pengesahan2.HR_NO_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8, "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            //decimal? gaji = 0;
            //if (mPekerjaan.HR_GAJI_POKOK != null)
            //{
            //    gaji = mPekerjaan.HR_GAJI_POKOK;
            //}
            //ViewBag.HR_GAJI_LAMA = gaji;
            //if(Detail2.HR_GAJI_BARU != null)
            //{
            //    ViewBag.gaji = Detail2.HR_GAJI_BARU;
            //}
            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI == Detail2.HR_GRED);
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList;
            return PartialView("_InfoKew8", model);
        }

        public ActionResult EditKew8(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string Kod)
        {
            ViewBag.Kod = Kod;
            var date = Convert.ToDateTime(HR_TARIKH_MULA);
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            
            if (model == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (Detail2 == null)
            {
                Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                Detail2.HR_JUMLAH_PERUBAHAN = 0;
            }
            int? gredDetail = Convert.ToInt32(Detail2.HR_GRED);

            ViewBag.HR_JUMLAH_PERUBAHAN_ELAUN = Detail2.HR_JUMLAH_PERUBAHAN_ELAUN;
            ViewBag.HR_JUMLAH_PERUBAHAN = Detail2.HR_JUMLAH_PERUBAHAN;

            HR_MATRIKS_GAJI matriksDetail2 = cariMatriks(cariGred(gredDetail, null).SHORT_DESCRIPTION, Detail2.HR_MATRIKS_GAJI, Detail2.HR_GAJI_BARU);

            ViewBag.HR_GRED = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_BARU = matriksDetail2.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_BARU = matriksDetail2.HR_GAJI_MAX;
            ViewBag.HR_GAJI_BARU = Detail2.HR_GAJI_BARU;
            ViewBag.HR_MATRIKS_GAJI = Detail2.HR_MATRIKS_GAJI;

            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (kewangan8 != null)
            {
                ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }

            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(mPekerjaan.HR_GRED);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, mPekerjaan.HR_MATRIKS_GAJI, mPekerjaan.HR_GAJI_POKOK);

            //var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);
            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(gredPekerjaan, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_MATRIKS_GAJI_LAMA = mPekerjaan.HR_MATRIKS_GAJI;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.pGaji = potongan2.HR_KOD_POTONGAN;


            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            ViewBag.nilaiPGaji = 0;
            ViewBag.nilaiPotongan = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }

                    if(Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015")
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4 != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_POTONGAN);
                                if (Detail == null)
                                {
                                    Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                }
                                if (Detail.HR_JUMLAH_PERUBAHAN == null)
                                {
                                    Detail.HR_JUMLAH_PERUBAHAN = 0;
                                }
                                elaun4.HR_NILAI = item.HR_JUMLAH;
                                elaun3.Add(elaun4);

                                HR_POTONGAN potongan4 = new HR_POTONGAN();
                                potongan4.HR_KOD_POTONGAN = Detail.HR_KOD_PELARASAN;
                                potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                ViewBag.nilaiPotongan += Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN));
                                potongan3.Add(potongan4);
                            }
                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "G" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                            if (Detail == null)
                            {
                                Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            }
                            if (Detail.HR_JUMLAH_PERUBAHAN == null)
                            {
                                Detail.HR_JUMLAH_PERUBAHAN = 0;
                            }
                            ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN));
                        }
                        
                    }
                    else
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            elaun4.HR_NILAI = item.HR_JUMLAH;
                            elaun3.Add(elaun4);

                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                            potongan3.Add(potongan4);

                        }
                    }
                }
                ViewBag.nilaiPotongan += ViewBag.nilaiPGaji;
                if (Kod == "00030" && model.HR_KEW8_IND == "A")
                {
                    ViewBag.nilaiPGaji = model.HR_BIL;
                    ViewBag.nilaiPotongan = model.HR_BIL;
                }
                
                ViewBag.elaun3 = elaun3;
                ViewBag.potongan3 = potongan3;
                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }

            
            //if (HR_KOD_PERUBAHAN == "00030")
            //{
            //    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            //    if (modelDetail.Count() > 0)
            //    {
            //        foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
            //        {
            //            if(item.HR_JUMLAH_PERUBAHAN == null)
            //            {
            //                item.HR_JUMLAH_PERUBAHAN = 0;
            //            }
            //            if(item.HR_KOD_PELARASAN != potongan2.HR_KOD_POTONGAN)
            //            {
            //                HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                elaun3.Add(elaun4);

            //                HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                potongan4.HR_NILAI = item.HR_JUMLAH_PERUBAHAN;
            //                potongan3.Add(potongan4);
            //            }
            //            else
            //            {
            //                ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(item.HR_JUMLAH_PERUBAHAN));
            //            }
            //        }
            //        ViewBag.elaun3 = elaun3;
            //        ViewBag.potongan3 = potongan3;
                    
            //    }
            //}

            if (Kod == "00031" || Kod == "00039" || Kod == "00024")
            { 
                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
                if (modelDetail == null)
                {
                    modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                ViewBag.kodPelarasan = modelDetail.HR_KOD_PELARASAN;

                if (Kod == "00031")
                {
                    ViewBag.HR_JUMLAH_PERUBAHAN = modelDetail.HR_JUMLAH_PERUBAHAN;
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015"), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                }

                if (Kod == "00039")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                }

                if (Kod == "00024")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                }
            }
            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            HR_MAKLUMAT_PERIBADI pengesahan2 = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if (pengesahan2 == null)
            {
                pengesahan2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.pengesahan2 = pengesahan2.HR_NO_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8, "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI == Detail2.HR_GRED);
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList;

            return PartialView("_EditKew8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKew8([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_TARIKH_AKHIR,HR_BULAN,HR_TAHUN,HR_TARIKH_KEYIN,HR_BUTIR_PERUBAHAN,HR_CATATAN,HR_NO_SURAT_KEBENARAN,HR_AKTIF_IND,HR_NP_UBAH_HR,HR_TARIKH_UBAH_HR,HR_NP_FINALISED_HR,HR_TARIKH_FINALISED_HR,HR_FINALISED_IND_HR,HR_NP_UBAH_PA,HR_TARIKH_UBAH_PA,HR_NP_FINALISED_PA,HR_TARIKH_FINALISED_PA,HR_FINALISED_IND_PA,HR_EKA,HR_ITP,HR_KEW8_IND,HR_BIL,HR_KOD_JAWATAN,HR_KEW8_ID,HR_LANTIKAN_IND,HR_TARIKH_SP,HR_SP_IND,HR_JUMLAH_BULAN,HR_NILAI_EPF,HR_GAJI_LAMA,HR_MATRIKS_GAJI_LAMA,HR_GRED_LAMA")] HR_MAKLUMAT_KEWANGAN8 model, [Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_KOD_PELARASAN,HR_MATRIKS_GAJI,HR_GRED,HR_JUMLAH_PERUBAHAN,HR_GAJI_BARU,HR_JENIS_PERGERAKAN,HR_JUMLAH_PERUBAHAN_ELAUN,HR_STATUS_IND,HR_ELAUN_KRITIKAL_BARU,HR_KEW8_ID,HR_NO_PEKERJA_PT,HR_PERGERAKAN_EKAL,HR_PERGERAKAN_EWIL,HR_GAJI_LAMA")] HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, decimal? HR_JUMLAH_POTONGAN, IEnumerable<HR_POTONGAN> sPotongan, int? PA_BULAN, short? PA_TAHUN, string Kod, DateTime? HR_TARIKH_TAMAT)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.Where(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y").FirstOrDefault();
            HR_MAKLUMAT_PERIBADI pekerja = mPeribadi.SingleOrDefault(s => s.HR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI, pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
            var kodGred = Convert.ToInt32(pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED);

            ViewBag.HR_KOD_JAWATAN = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(kodGred, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

            DateTime tMula = model.HR_TARIKH_MULA;

            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            //HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA).Count() > 0);
            //if (gajiUpah == null)
            //{
            //    gajiUpah = new HR_GAJI_UPAHAN();
            //}

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            var jawatan_ind = "";
            if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            }
            else if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (tggkk == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }

            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (ModelState.IsValid)
            {
                if (Kod == "00031")
                {
                    var jumBulan = Convert.ToString(model.HR_JUMLAH_BULAN);
                    var EPF = Convert.ToString(model.HR_NILAI_EPF);
                    var bil = jumBulan + EPF;
                    model.HR_BIL = Convert.ToDecimal(bil);
                }
                
                model.HR_BULAN = DateTime.Now.Month;
                model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                model.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                model.HR_TARIKH_UBAH_HR = DateTime.Now;
                model.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                model.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                //HR_JAWATAN jwt4 = db.HR_JAWATAN.SingleOrDefault(s => s.HR_NAMA_JAWATAN == model.HR_KOD_JAWATAN);
                //if (jwt4 == null)
                //{
                //    jwt4 = new HR_JAWATAN();
                //}

                //model.HR_KOD_JAWATAN = jwt4.HR_KOD_JAWATAN;

                if (Kod == "CUTI")
                {
                    model.HR_TARIKH_SP = new DateTime(model.HR_TARIKH_MULA.Year, model.HR_TARIKH_MULA.AddMonths(1).Month, 1);
                    model.HR_SP_IND = "Y";
                }

                if (model.HR_FINALISED_IND_HR == "Y")
                {
                    model.HR_TARIKH_FINALISED_HR = DateTime.Now;
                    HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(model.HR_NO_PEKERJA);
                    if (Kod == "00026")
                    {
                        pekerjaan.HR_GAJI_IND = "Y";
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00025")
                    { 
                        pekerjaan.HR_GAJI_IND = "N";
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "TP")
                    {
                        pekerjaan.HR_GAJI_IND = "T";
                        db.Entry(pekerjaan).State = EntityState.Modified;

                        HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Find(model.HR_NO_PEKERJA);
                        peribadi2.HR_AKTIF_IND = "T";
                        db.Entry(peribadi2).State = EntityState.Modified;
                    }

                    if(Kod == "00022")
                    {
                        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "Y";
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00037")
                    {
                        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "T";
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00036")
                    {
                        pekerjaan.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                        pekerjaan.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                        pekerjaan.HR_GAJI_POKOK = modelDetail.HR_GAJI_BARU;
                        pekerjaan.HR_KOD_GAJI = cariMatriks(modelDetail.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_KOD_GAJI;
                        pekerjaan.HR_SISTEM = cariMatriks(modelDetail.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_SISTEM_SARAAN;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00039" || Kod == "00039")
                    {
                        model.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_HR;
                        model.HR_TARIKH_FINALISED_PA = DateTime.Now;
                        model.HR_FINALISED_IND_PA = "Y";
                    }

                    if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015" || Kod == "00024" || Kod == "00039")
                    {
                        List<PA_PELARASAN> pelarasan2 = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == tMula && s.PA_BULAN == PA_BULAN && s.PA_TAHUN == PA_TAHUN && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                        spg.PA_PELARASAN.RemoveRange(pelarasan2);
                        spg.SaveChanges();
                        var no = 0;
                        foreach (HR_POTONGAN potongan in sPotongan)
                        {
                            if (potongan.HR_NILAI == null)
                            {
                                potongan.HR_NILAI = 0;
                            }

                            if (Kod == "00031")
                            {
                                potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                            }
                            else if (Kod == "00039" || Kod == "00024")
                            {
                                if(Kod == "00024")
                                {
                                    HR_ELAUN cariElaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);
                                    if (cariElaun == null)
                                    {
                                        cariElaun = new HR_ELAUN();
                                    }
                                    if (model.HR_KEW8_IND == "E")
                                    {

                                        if (cariElaun.HR_KOD_TUNGGAKAN != null)
                                        {
                                            potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_TUNGGAKAN;
                                        }
                                        else
                                        {
                                            potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                        }
                                    }
                                    else
                                    {
                                        if (cariElaun.HR_KOD_POTONGAN != null)
                                        {
                                            potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_POTONGAN;
                                        }
                                        else
                                        {
                                            potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                        }
                                    }
                                }
                                else
                                {
                                    HR_POTONGAN cariPotongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == modelDetail.HR_KOD_PELARASAN);
                                    if(cariPotongan == null)
                                    {
                                        cariPotongan = new HR_POTONGAN();
                                    }
                                    potongan.HR_KOD_POTONGAN = cariPotongan.HR_KOD_POTONGAN;
                                }
                                

                                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                            }
                            else
                            {
                                potongan.HR_KOD_POTONGAN = potongan.HR_KOD_POTONGAN;
                                if (Kod == "00036")
                                {
                                    potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                                }
                                else
                                {
                                    potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                                }
                            }

                            //if (no == 0 && Kod != "00031")
                            //{
                            //    potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                            //}

                            if (no == 0 && (Kod != "00031" && Kod != "00039" && Kod != "00024"))
                            {
                                //potongan.HR_NILAI = -potongan.HR_NILAI;
                                if (potongan.HR_NILAI <= 0)
                                {
                                    //potongan
                                    potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                                }
                                else
                                {
                                    //tunggakan
                                    potongan.HR_KOD_POTONGAN = tggkk.HR_KOD_UPAH;
                                }
                            }

                            //HR_POTONGAN pot = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                            //if (pot == null)
                            //{
                            //    pot = new HR_POTONGAN();
                            //}

                            //HR_GAJI_UPAHAN gUpah = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                            //if(gUpah == null)
                            //{
                            //    gUpah = new HR_GAJI_UPAHAN();
                            //}

                            

                            string jenis = null;
                            string vot = null;
                            string singkatan = null;
                            string laporan = null;
                            //HR_MAKLUMAT_ELAUN_POTONGAN potonganInd = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E" && db.HR_ELAUN.Where(e => e.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN && e.HR_KOD_ELAUN == s.HR_KOD_ELAUN_POTONGAN).Count() > 0);
                            //if (potonganInd == null)
                            //{
                            //    potonganInd = new HR_MAKLUMAT_ELAUN_POTONGAN();
                            //    potonganInd = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G" && db.HR_GAJI_UPAHAN.Where(e => e.HR_KOD_UPAH == s.HR_KOD_ELAUN_POTONGAN && e.HR_VOT_UPAH == pot.HR_VOT_POTONGAN).Count() > 0);
                            //    if (potonganInd == null)
                            //    {
                            //        potonganInd = new HR_MAKLUMAT_ELAUN_POTONGAN();
                            //    }
                            //}

                            HR_GAJI_UPAHAN salary = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                            if(salary != null)
                            {
                                jenis = "G";
                                laporan = "G";
                                singkatan = salary.HR_SINGKATAN;
                                vot = salary.HR_VOT_UPAH;
                            }
                            else
                            {
                                HR_ELAUN ellowance2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN);
                                if (ellowance2 != null)
                                {
                                    jenis = "E";

                                    singkatan = ellowance2.HR_SINGKATAN;
                                    vot = ellowance2.HR_VOT_ELAUN;                                
                                }

                                HR_POTONGAN deduction = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                                if (deduction != null)
                                {
                                    jenis = "P";
                                    singkatan = deduction.HR_SINGKATAN;
                                    vot = deduction.HR_VOT_POTONGAN;
                                }

                                List<HR_ELAUN> ellowance = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN || s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN).ToList();
                                if(ellowance.Count() > 0)
                                {
                                    laporan = "E";
                                }
                                else
                                {
                                    if(gajiUpah.HR_VOT_UPAH != deduction.HR_VOT_POTONGAN || Kod == "00036")
                                    {
                                        laporan = "P";
                                    }
                                    else
                                    {
                                        laporan = "G";
                                    }
                                }
                                
                            }


                            //if (Kod == "00031")
                            //{
                            //    jenis = "E";

                            //    singkatan = ellowance.HR_SINGKATAN;
                            //}
                            //else
                            //{  
                            //    pelarasan.PA_JENIS_PELARASAN = "P";
                            //}

                            if(Kod == "00039" || Kod == "00024")
                            {
                                HR_MAKLUMAT_ELAUN_POTONGAN ElaunPotongan4 = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == modelDetail.HR_KOD_PELARASAN);

                                var aktifind = "T";
                                if(model.HR_KEW8_IND == "E" && model.HR_TARIKH_MULA <= DateTime.Now && model.HR_TARIKH_AKHIR >= DateTime.Now)
                                {
                                    aktifind = "Y";
                                }

                                if(ElaunPotongan4 == null)
                                {
                                    ElaunPotongan4 = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    ElaunPotongan4.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                                    ElaunPotongan4.HR_KOD_ELAUN_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                    ElaunPotongan4.HR_JUMLAH = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = jenis;
                                    ElaunPotongan4.HR_MOD_BAYARAN = "1";
                                    ElaunPotongan4.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
                                    ElaunPotongan4.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                                    ElaunPotongan4.HR_AKTIF_IND = aktifind;
                                    ElaunPotongan4.HR_TARIKH_KEYIN = DateTime.Now;
                                    ElaunPotongan4.HR_TARIKH_UBAH = DateTime.Now;
                                    ElaunPotongan4.HR_UBAH_IND = "B";
                                    ElaunPotongan4.HR_NP_KEYIN = peribadi.HR_NO_PEKERJA;
                                    ElaunPotongan4.HR_NP_UBAH = peribadi.HR_NO_PEKERJA;
                                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(ElaunPotongan4);
                                }
                                else
                                {
                                    
                                    ElaunPotongan4.HR_JUMLAH = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = jenis;
                                    ElaunPotongan4.HR_MOD_BAYARAN = "1";
                                    ElaunPotongan4.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
                                    ElaunPotongan4.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                                    ElaunPotongan4.HR_AKTIF_IND = aktifind;
                                    ElaunPotongan4.HR_TARIKH_KEYIN = DateTime.Now;
                                    ElaunPotongan4.HR_TARIKH_UBAH = DateTime.Now;
                                    ElaunPotongan4.HR_UBAH_IND = "K";
                                    ElaunPotongan4.HR_NP_UBAH = peribadi.HR_NO_PEKERJA;
                                    db.Entry(ElaunPotongan4).State = EntityState.Modified;
                                }
                            }

                            PA_PELARASAN pelarasan = spg.PA_PELARASAN.AsEnumerable().SingleOrDefault(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_BULAN == Convert.ToInt32(model.HR_BULAN) && s.PA_TAHUN == Convert.ToInt16(model.HR_TAHUN) && s.PA_KOD_PELARASAN == potongan.HR_KOD_POTONGAN);
                            if(pelarasan == null)
                            {
                                pelarasan = new PA_PELARASAN();
                                pelarasan.PA_NO_PEKERJA = model.HR_NO_PEKERJA;
                                pelarasan.PA_BULAN = Convert.ToInt32(model.HR_BULAN);
                                pelarasan.PA_TAHUN = Convert.ToInt16(model.HR_TAHUN);
                                pelarasan.PA_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                                pelarasan.PA_PERATUS = 0;
                                pelarasan.PA_NILAI = potongan.HR_NILAI;
                                pelarasan.PA_NILAI_MAKSIMUM = 0;
                                pelarasan.PA_NILAI_MINIMUM = 0;
                                pelarasan.PA_JENIS_PELARASAN = jenis;

                                DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                                pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                                pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month + 1, 1).AddDays(-1);

                                pelarasan.PA_PROSES_IND = "T";

                                pelarasan.PA_VOT_PELARASAN = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + vot;
                                pelarasan.PA_SINGKATAN = singkatan;
                                pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                                pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                                pelarasan.PA_LAPORAN_IND = laporan;
                                pelarasan.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                                spg.PA_PELARASAN.Add(pelarasan);
                            }
                            else
                            {
                                pelarasan.PA_PERATUS = 0;
                                pelarasan.PA_NILAI = potongan.HR_NILAI;
                                pelarasan.PA_NILAI_MAKSIMUM = 0;
                                pelarasan.PA_NILAI_MINIMUM = 0;
                                pelarasan.PA_JENIS_PELARASAN = jenis;

                                DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                                pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                                pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month + 1, 1).AddDays(-1);

                                pelarasan.PA_PROSES_IND = "T";

                                pelarasan.PA_VOT_PELARASAN = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + vot;
                                pelarasan.PA_SINGKATAN = singkatan;
                                pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                                pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                                pelarasan.PA_LAPORAN_IND = laporan;
                                pelarasan.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                                spg.Entry(pelarasan).State = EntityState.Modified;
                            }
                            
                            spg.SaveChanges();
                            no++;

                        }

                    }
                }
                
                if (Kod == "TP")
                {
                    HR_MAKLUMAT_KEWANGAN8 cariKew8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tMula);
                    db.HR_MAKLUMAT_KEWANGAN8.Remove(cariKew8);

                    model.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                    model.HR_TARIKH_AKHIR = Convert.ToDateTime(HR_TARIKH_TAMAT);
                    modelDetail.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                    db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                db.SaveChanges();

                if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015" || Kod == "00024" || Kod == "00039")
                {
                    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tMula).ToList();
                    db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(Detail);
                    db.SaveChanges();
                    var no = 0;
                    foreach (HR_POTONGAN potongan in sPotongan)
                    {
                        HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();

                        modelDetail2.HR_NO_PEKERJA = modelDetail.HR_NO_PEKERJA;
                        modelDetail2.HR_KOD_PERUBAHAN = modelDetail.HR_KOD_PERUBAHAN;
                        modelDetail2.HR_TARIKH_MULA = modelDetail.HR_TARIKH_MULA;
                        modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;

                        if (potongan.HR_NILAI == null)
                        {
                            potongan.HR_NILAI = 0;
                        }

                        if (Kod == "00031" || Kod == "00039" || Kod == "00024")
                        {
                            modelDetail2.HR_KOD_PELARASAN = modelDetail.HR_KOD_PELARASAN;
                            potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                        }
                        else
                        {
                            modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                            if(Kod == "00036")
                            {
                                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                            }
                            else
                            {
                                potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                            }
                        }

                        //if (no == 0 && (Kod != "00031" && Kod != "00036"))
                        //{
                        //    //potongan.HR_NILAI = -potongan.HR_NILAI;
                        //    modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                        //}

                        if (no == 0 && (Kod != "00031" && Kod != "00039" && Kod != "00024"))
                        {
                            if (potongan.HR_NILAI > 0)
                            {
                                //tunggakan
                                modelDetail2.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                            }
                            else
                            {
                                //potongan
                                modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                            }
                        }

                        if (Kod != "00036")
                        {
                            modelDetail2.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                            modelDetail2.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                            modelDetail2.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        }
                        else
                        {
                            modelDetail2.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                            modelDetail2.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                            modelDetail2.HR_GAJI_BARU = modelDetail.HR_GAJI_BARU;
                        }


                        modelDetail2.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI;

                        modelDetail2.HR_JENIS_PERGERAKAN = modelDetail.HR_JENIS_PERGERAKAN;
                        modelDetail2.HR_JUMLAH_PERUBAHAN_ELAUN = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                        modelDetail2.HR_STATUS_IND = "E";
                        modelDetail2.HR_ELAUN_KRITIKAL_BARU = modelDetail.HR_ELAUN_KRITIKAL_BARU;
                        modelDetail2.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                        modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                        modelDetail2.HR_PERGERAKAN_EKAL = modelDetail.HR_PERGERAKAN_EKAL;
                        modelDetail2.HR_PERGERAKAN_EWIL = modelDetail.HR_PERGERAKAN_EWIL;
                        modelDetail2.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        //modelDetail2.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                        //modelDetail2.HR_GRED_LAMA = modelDetail.HR_GRED;

                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail2);

                        no++;
                        db.SaveChanges();
                    }

                }
                //else if(Kod != "00022" && Kod != "00037" && Kod != "kew8" && Kod != "00025")
                else if (Kod == "00039" || Kod == "00024")
                {
                    modelDetail.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                    //modelDetail.HR_TARIKH_MULA = DateTime.Now;
                    //if(Kod != "00022" && Kod != "00037")
                    //{
                        modelDetail.HR_STATUS_IND = "E";
                    //}
                    
                    //if (Kod != "00036")
                    //{
                    //    modelDetail.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                    //    modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    //    modelDetail.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    //}
                    //else
                    //{
                    //    modelDetail.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                    //}

                    modelDetail.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    //modelDetail.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                    //modelDetail.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    db.Entry(modelDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var redirect = RedirectLink(Kod);

                return RedirectToAction(redirect, new { key = "1", value = model.HR_NO_PEKERJA });
            }
            ViewBag.Kod = Kod;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                foreach (var item in elaunPotongan)
                {
                    // && item.HR_TARIKH_AKHIR >= DateTime.Now
                    if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                        elaun3.Add(elaun4);

                    }
                    if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                        potongan3.Add(potongan4);

                    }
                }

            }

            if (Kod == "00031")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015"), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            }

            if (Kod == "00039")
            {
                if (model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }

            }

            if (Kod == "00024")
            {
                if (model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }

            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            ViewBag.pengesahan = pengesahan;

            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if(namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8, "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            decimal? gaji = 0;
            if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK != null)
            {
                gaji = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            }
            ViewBag.gaji = gaji;

            return PartialView("_EditKew8", model);
        }

        public ActionResult PadamKew8(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string Kod)
        {
            ViewBag.Kod = Kod;
            var date = Convert.ToDateTime(HR_TARIKH_MULA);
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (model == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (Detail2 == null)
            {
                Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                Detail2.HR_JUMLAH_PERUBAHAN = 0;
            }
            int? gredDetail = Convert.ToInt32(model.HR_GRED_LAMA);

            ViewBag.HR_JUMLAH_PERUBAHAN_ELAUN = Detail2.HR_JUMLAH_PERUBAHAN_ELAUN;
            ViewBag.HR_JUMLAH_PERUBAHAN = Detail2.HR_JUMLAH_PERUBAHAN;

            HR_MATRIKS_GAJI matriksDetail2 = cariMatriks(cariGred(gredDetail, null).SHORT_DESCRIPTION, model.HR_MATRIKS_GAJI_LAMA, model.HR_GAJI_LAMA);

            ViewBag.HR_GRED = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_BARU = matriksDetail2.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_BARU = matriksDetail2.HR_GAJI_MAX;
            ViewBag.HR_GAJI_BARU = Detail2.HR_GAJI_BARU;
            ViewBag.HR_MATRIKS_GAJI = Detail2.HR_MATRIKS_GAJI;

            ViewBag.kodGaji = matriksDetail2.HR_KOD_GAJI;

            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (kewangan8 != null)
            {
                ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }

            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(model.HR_GRED_LAMA);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, model.HR_MATRIKS_GAJI_LAMA, model.HR_GAJI_LAMA);

            //var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);
            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(gredPekerjaan, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = model.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == model.HR_KOD_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            //int? kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);

            //ViewBag.gred = cariGred(kodGred, null).SHORT_DESCRIPTION;
            //if(cariGred(gredDetail, null).SHORT_DESCRIPTION != null)
            //{
            //    ViewBag.gred = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            //}

            //ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;
            //ViewBag.gaji = mPekerjaan.HR_GAJI_POKOK;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.pGaji = potongan2.HR_KOD_POTONGAN;


            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            ViewBag.nilaiPGaji = 0;
            ViewBag.nilaiPotongan = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }

                    if ((Kod == "00030" && model.HR_KEW8_IND == "H") || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015")
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4 != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_POTONGAN);
                                if (Detail == null)
                                {
                                    Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                }
                                if (Detail.HR_JUMLAH_PERUBAHAN == null)
                                {
                                    Detail.HR_JUMLAH_PERUBAHAN = 0;
                                }
                                elaun4.HR_NILAI = item.HR_JUMLAH;
                                elaun3.Add(elaun4);

                                HR_POTONGAN potongan4 = new HR_POTONGAN();
                                potongan4.HR_KOD_POTONGAN = Detail.HR_KOD_PELARASAN;
                                potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                ViewBag.nilaiPotongan += Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN));
                                potongan3.Add(potongan4);
                            }
                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "G" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                            if (Detail == null)
                            {
                                Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            }
                            if (Detail.HR_JUMLAH_PERUBAHAN == null)
                            {
                                Detail.HR_JUMLAH_PERUBAHAN = 0;
                            }
                            ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN));
                        }

                    }
                    else
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            elaun4.HR_NILAI = item.HR_JUMLAH;
                            elaun3.Add(elaun4);

                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y" && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                            potongan3.Add(potongan4);

                        }
                    }
                }
                ViewBag.nilaiPotongan += ViewBag.nilaiPGaji;
                if (Kod == "00030" && model.HR_KEW8_IND == "A")
                {
                    ViewBag.nilaiPGaji = model.HR_BIL;
                    ViewBag.nilaiPotongan = model.HR_BIL;
                }

                ViewBag.elaun3 = elaun3;
                ViewBag.potongan3 = potongan3;
                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }


            //if (HR_KOD_PERUBAHAN == "00030")
            //{
            //    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            //    if (modelDetail.Count() > 0)
            //    {
            //        foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
            //        {
            //            if(item.HR_JUMLAH_PERUBAHAN == null)
            //            {
            //                item.HR_JUMLAH_PERUBAHAN = 0;
            //            }
            //            if(item.HR_KOD_PELARASAN != potongan2.HR_KOD_POTONGAN)
            //            {
            //                HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                elaun3.Add(elaun4);

            //                HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                potongan4.HR_NILAI = item.HR_JUMLAH_PERUBAHAN;
            //                potongan3.Add(potongan4);
            //            }
            //            else
            //            {
            //                ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(item.HR_JUMLAH_PERUBAHAN));
            //            }
            //        }
            //        ViewBag.elaun3 = elaun3;
            //        ViewBag.potongan3 = potongan3;

            //    }
            //}

            if (Kod == "00031" || Kod == "00039" || Kod == "00024")
            {
                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
                if (modelDetail == null)
                {
                    modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                ViewBag.kodPelarasan = modelDetail.HR_KOD_PELARASAN;

                if (Kod == "00031")
                {
                    ViewBag.HR_JUMLAH_PERUBAHAN = modelDetail.HR_JUMLAH_PERUBAHAN;
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015"), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                }

                if (Kod == "00039")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                }

                if (Kod == "00024")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                }
            }

            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            HR_MAKLUMAT_PERIBADI pengesahan2 = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if (pengesahan2 == null)
            {
                pengesahan2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.pengesahan2 = pengesahan2.HR_NO_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8, "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            //decimal? gaji = 0;
            //if (mPekerjaan.HR_GAJI_POKOK != null)
            //{
            //    gaji = mPekerjaan.HR_GAJI_POKOK;
            //}
            //ViewBag.HR_GAJI_LAMA = gaji;
            //if(Detail2.HR_GAJI_BARU != null)
            //{
            //    ViewBag.gaji = Detail2.HR_GAJI_BARU;
            //}
            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI == Detail2.HR_GRED);
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList;
            return PartialView("_PadamKew8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamKew8(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, string Kod)
        {
            model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA);
            List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA).ToList();
            db.HR_MAKLUMAT_KEWANGAN8.Remove(model);
            db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(modelDetail2);
            db.SaveChanges();

            var redirect = RedirectLink(Kod);

            return RedirectToAction(redirect, new { key = "1", value = model.HR_NO_PEKERJA });
        }

        public ActionResult GanjaranKontrak(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00031");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TahanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();  
            mPeribadi = CariPekerja(key, value, null, "00025");
            
            ViewBag.key = key;
            ViewBag.value = value;
            
            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult PotonganGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00030");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult BayarGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00026");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TamatPerkhidmatan(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "TP");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult Tanggung()
        {
            return View();
        }

        public ActionResult TangguhPergerakanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00022");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }
        public ActionResult SambungPergerakanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00037");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult PindaanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00036");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SambungKontrak(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00015");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SemuaJenisElaun(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00024");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult Perlantikan()
        {
            return View();
        }

        public ActionResult Cuti(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "CUTI");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SemuaJenisPotongan(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00039");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public JsonResult JumlahPerubahan(string HR_NO_PEKERJA, int? HR_JUMLAH_BULAN, decimal? HR_NILAI_EPF)
        {
            double? item = 0.00;
            var mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA);
            if(mPekerjaan != null)
            {
                var HR_GAJI_POKOK = Convert.ToDouble(mPekerjaan.HR_GAJI_POKOK);
                var EPF = Convert.ToDouble(HR_NILAI_EPF);
                item = ((HR_GAJI_POKOK * 0.055) * HR_JUMLAH_BULAN) - EPF;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LaporanKewangan8Bulanan()
        {
            //var model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(DateTime.Now.Month, DateTime.Now.Year);
            List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == DateTime.Now.Month && s.HR_TAHUN == DateTime.Now.Year).OrderBy(s => s.HR_NO_PEKERJA).OrderByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();

            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.kew8 = db.HR_KEWANGAN8.ToList();
            ViewBag.bulan = DateTime.Now.Month;
            ViewBag.year = DateTime.Now.Year;
            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;
            return View(model);
        }

        [HttpPost]
        public ActionResult LaporanKewangan8Bulanan(int? bulan, int? tahun, string status)
        {
            //var model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(DateTime.Now.Month, DateTime.Now.Year);
            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
            if (status == "S")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).OrderBy(s => s.HR_NO_PEKERJA).OrderByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status).OrderBy(s => s.HR_NO_PEKERJA).OrderByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }

            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.kew8 = db.HR_KEWANGAN8.ToList();
            ViewBag.bulan = DateTime.Now.Month;
            ViewBag.year = DateTime.Now.Year;
            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;
            return View(model);
        }

        [HttpPost]
        public FileStreamResult PDFLaporanKewangan8Bulanan(int? bulan, int? tahun, string status)
        {

            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
            if (status == "S")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).OrderByDescending(s => s.HR_TARIKH_MULA).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status).OrderByDescending(s => s.HR_TARIKH_MULA).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            List<HR_KEWANGAN8> sKewangan8 = new List<HR_KEWANGAN8>();

            foreach (HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            {
                HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == pekerja.HR_KOD_PERUBAHAN);
                if(kewangan8 == null)
                {
                    kewangan8 = new HR_KEWANGAN8();
                }
                sKewangan8.Add(kewangan8);
            }

            var html = "<html><head>";
            html += "<title>Senarai Pergerakan Gaji</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";
            foreach (HR_KEWANGAN8 kew in sKewangan8.GroupBy(s => s.HR_KOD_KEW8).Select(s => s.FirstOrDefault()))
            {
                html += "<table width='100%' cellpadding='5' cellspacing='0'>";
                html += "<tr>";
                html += "<td valign='top' height='40' style='font-size: 80%;'>";
                html += "<strong>KEWANGAN 8 :  "+kew.HR_KOD_KEW8+ "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + kew.HR_PENERANGAN+"</strong>";
                html += "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td style='font-size: 80%' align='center'>";
                
                var no = 0;
                foreach (var l in model.Join(db.HR_MAKLUMAT_PERIBADI, HR_MAKLUMAT_KEWANGAN8 => HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA, HR_MAKLUMAT_PERIBADI => HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA, (HR_MAKLUMAT_KEWANGAN8, HR_MAKLUMAT_PERIBADI) => new { HR_MAKLUMAT_KEWANGAN8, HR_MAKLUMAT_PERIBADI }).OrderBy(s => s.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA))
                {
                    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA);
                    if (peribadi == null)
                    {
                        peribadi = new HR_MAKLUMAT_PERIBADI();
                    }

                    string HR_TARIKH_MULA = "TIADA";
                    string HR_TARIKH_AKHIR = "TIADA";
                    string HR_AKTIF_IND = "TIADA";

                    HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G");
                    if(elaunPotongan != null)
                    {
                        HR_TARIKH_MULA = string.Format("{0:dd/MM/yyyy}", elaunPotongan.HR_TARIKH_MULA);
                        HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", elaunPotongan.HR_TARIKH_AKHIR);
                        HR_AKTIF_IND = elaunPotongan.HR_AKTIF_IND;
                    }

                    if (l.HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN == kew.HR_KOD_KEW8)
                    {
                        GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                        if (jabatan == null)
                        {
                            jabatan = new GE_JABATAN();
                        }
                        HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                        if (jawatan == null)
                        {
                            jawatan = new HR_JAWATAN();
                        }
                        html += "<table width='100%' cellpadding='5' cellspacing='0'>";
                        ++no;
                        html += "<tr>";
                        html += "<td colspan='5' style='font-size: 80%'>" + kew.HR_KOD_KEW8 + "&nbsp;&nbsp;&nbsp;&nbsp;" + kew.HR_PENERANGAN + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td width='35%' valign='top' rowspan='2' style='font-size: 80%'><strong>" + peribadi.HR_NAMA_PEKERJA + "</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'><strong>MATRIKS GAJI</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI + "</td>";
                        html += "<td width='12%' style='font-size: 80%'><strong>TARIKH MULA</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>" + HR_TARIKH_MULA + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td width='12%' style='font-size: 80%'><strong>GAJI POKOK</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>RM " + string.Format("{0:#,0.00}",peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) + "</td>";
                        html += "<td width='12%' style='font-size: 80%'><strong>TARIKH TAMAT</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>" + HR_TARIKH_AKHIR + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td width='35%' valign='top' style='font-size: 80%'><strong>NO PEKERJA : </strong>&nbsp;&nbsp;&nbsp;" + peribadi.HR_NO_PEKERJA + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><strong>STATUS GAJI<br/>(HR)</strong></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><strong>STATUS GEPC</strong></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + HR_AKTIF_IND + "</td>";
                        html += "</tr>";

                        //html += "<tr>";
                        //html += "<td align='center' style='font-size: 80%'>" + no + "</td>";
                        //html += "<td style='font-size: 80%'>" + l.HR_NO_PEKERJA + "</td>";
                        //html += "<td style='font-size: 80%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                        //html += "<td align='center' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_TARIKH_KEYIN) + "</td>";
                        //html += "<td style='font-size: 80%'>" + jabatan.GE_KETERANGAN_JABATAN + "</td>";
                        //html += "<td style='font-size: 80%'>" + jawatan.HR_NAMA_JAWATAN + "</td>";
                        //html += "</tr>";
                        html += "</table>";

                        html += "<table width='100%' cellpadding='5' cellspacing='0'>";
                        html += "<tr>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><u>TARIKH MULA</u></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><u>TARIKH AKHIR</u></td>";
                        html += "<td width='35%' style='font-size: 80%'><u>BUTIR PERUBAHAN</u></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><u>FINALISED</u></td>";
                        html += "</tr>";
                        html += "<tr>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA) + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR) + "</td>";
                        html += "<td width='35%' style='font-size: 80%'>" + l.HR_MAKLUMAT_KEWANGAN8.HR_BUTIR_PERUBAHAN + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + l.HR_MAKLUMAT_KEWANGAN8.HR_FINALISED_IND_HR + "</td>";
                        html += "</tr>";
                        html += "</table>";

                        html += "<table width='100%' cellpadding='5' cellspacing='0'>";
                        html += "<tr>";
                        html += "<td width='5%' style='font-size: 80%'><u>KOD</u></td>";
                        html += "<td width='35%' style='font-size: 80%'><u>KETERANGAN</u></td>";
                        html += "<td align='right' width='10%' style='font-size: 80%'><u>JUMLAH</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>GAJI BARU</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>STATUS GEPC</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>TARIKH MULA</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>TARIKH AKHIR</u></td>";
                        html += "</tr>";
                        

                        List<HR_MAKLUMAT_KEWANGAN8_DETAIL> sDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == l.HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == l.HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA).ToList();
                        if (sDetail.Count > 0)
                        {
                            foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL detail in sDetail)
                            {
                                string HR_KETERANGAN = null;
                                HR_GAJI_UPAHAN gaji = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == detail.HR_KOD_PELARASAN);
                                if(gaji == null)
                                {
                                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                                    if(elaun == null)
                                    {
                                        HR_POTONGAN potongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == detail.HR_KOD_PELARASAN);
                                        if(potongan == null)
                                        {
                                            HR_CARUMAN caruman = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == detail.HR_KOD_PELARASAN);
                                            if(caruman != null)
                                            {
                                                HR_KETERANGAN = caruman.HR_PENERANGAN_CARUMAN;
                                            }
                                        }
                                        else
                                        {
                                            HR_KETERANGAN = potongan.HR_PENERANGAN_POTONGAN;
                                        }
                                    }
                                    else
                                    {
                                        HR_KETERANGAN = elaun.HR_PENERANGAN_ELAUN;
                                    }
                                }
                                else
                                {
                                    HR_KETERANGAN = gaji.HR_PENERANGAN_UPAH;
                                }
                                

                                html += "<tr>";
                                html += "<td width='6%' style='font-size: 80%'>" + detail.HR_KOD_PELARASAN + "</td>";
                                html += "<td width='35%' style='font-size: 80%'>" + HR_KETERANGAN + "</td>";
                                html += "<td align='right' width='10%' style='font-size: 80%'>" + string.Format("{0:#,0.00}", detail.HR_JUMLAH_PERUBAHAN) + "</td>";
                                html += "<td width='10%' style='font-size: 80%'></td>";
                                html += "<td width='10%' style='font-size: 80%'></td>";
                                html += "<td width='10%' style='font-size: 80%'></td>";
                                html += "<td width='10%' style='font-size: 80%'></td>";
                                html += "</tr>";
                            }
                        }
                        html += "</table>";
                        html += "<br/>";
                        html += "<br/>";
                        html += "<hr/>";
                    }
                }
                html += "</td>";
                html += "</tr>";
                html += "</table>";
                html += "<br/>";
            }

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");

                var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "Appril" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "september" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };
                var Bulan = "";
                foreach (var m in associativeArray)
                {
                    if (bulan == m.Key)
                    {
                        Bulan = m.Value;
                    }

                }

                //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Paragraph tarikhCetak = new iTextSharp.text.Paragraph("Tarikh Cetak:   " + string.Format("{0:dd/MM/yyyy}", DateTime.Now), contentFont);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("SENARAI KEWANGAN 8 BULAN "+ Bulan.ToUpper() +" TAHUN "+ tahun);
                //iTextSharp.text.Paragraph paragraph2 = new iTextSharp.text.Paragraph("Bulan                       " + Bulan, contentFont);
                //iTextSharp.text.Paragraph paragraph3 = new iTextSharp.text.Paragraph("Tahun                       " + tahun, contentFont);
                paragraph.Alignment = Element.ALIGN_CENTER;
                //pic.ScaleToFit(100f, 80f);
                //pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                //pic.IndentationRight = 30f;
                //pic.SpacingBefore = 9f;
                //paragraph.SpacingBefore = 10f;
                //paragraph2.SpacingBefore = 10f;
                //pic.BorderWidthTop = 36f;
                //paragraph2.SetLeading(20f, 0);
                //document.Add(pic);
                tarikhCetak.IndentationRight = 100f;
                tarikhCetak.Alignment = Element.ALIGN_RIGHT;
                document.Add(tarikhCetak);
                document.Add(paragraph);
                //document.Add(paragraph2);
                //document.Add(paragraph3);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                //document.Add(new iTextSharp.text.Paragraph("\n"));

                //PdfPTable table = new PdfPTable(3);
                //PdfPCell cell = new PdfPCell(new Phrase("Header spanning 3 columns"));
                //cell.Colspan = 3;
                //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                //table.AddCell(cell);
                //table.AddCell("Col 1 Row 1");
                //table.AddCell("Col 2 Row 1");
                //table.AddCell("Col 3 Row 1");
                //table.AddCell("Col 1 Row 2");
                //table.AddCell("Col 2 Row 2");
                //table.AddCell("Col 3 Row 2");
                //document.Add(table);

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Paragraph paragraph4 = new iTextSharp.text.Paragraph("Copyright © " + DateTime.Now.Year + " Sistem Bandaraya Petaling Jaya. All Rights Reserved\nUser Id: " + User.Identity.Name.ToLower() + " - Tarikh print: " + DateTime.Now.ToString("dd-MM-yyyy"), contentFont2);
                document.Add(paragraph4);

                document.Close();
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
        }

        [HttpPost]
        public FileResult EXCLaporanKewangan8Bulanan(int? bulan, int? tahun, string status)
        {
            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
            if (status == "S")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            if (model.Count() <= 0)
            {
                model = new List<HR_MAKLUMAT_KEWANGAN8>();
            }
            DataSet ds = new DataSet();

            List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();

            foreach (HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA);
                GE_JABATAN jabatan2 = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                sJabatan.Add(jabatan2);
            }
            var no2 = 0;
            foreach (GE_JABATAN jab in sJabatan.GroupBy(s => s.GE_KOD_JABATAN).Select(s => s.FirstOrDefault()))
            {
                no2++;
                DataTable dt = new DataTable("SENARAI PERGERAKAN GAJI " + jab.GE_KOD_JABATAN);
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("#"),
                                            new DataColumn("No Pekerja"),
                                            new DataColumn("Nama Pekerja"),
                                            new DataColumn("Tarikh Pergerakan"),
                                            new DataColumn("Jabatan"),
                                            new DataColumn("Jawatan")});


                var no = 0;
                foreach (var l in model)
                {
                    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_NO_PEKERJA);
                    if (peribadi == null)
                    {
                        peribadi = new HR_MAKLUMAT_PERIBADI();
                    }

                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jab.GE_KOD_JABATAN)
                    {
                        GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                        if (jabatan == null)
                        {
                            jabatan = new GE_JABATAN();
                        }
                        HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                        if (jawatan == null)
                        {
                            jawatan = new HR_JAWATAN();
                        }
                        ++no;
                        dt.Rows.Add(no, l.HR_NO_PEKERJA, peribadi.HR_NAMA_PEKERJA, string.Format("{0:dd/MM/yyyy}", l.HR_TARIKH_KEYIN), jabatan.GE_KETERANGAN_JABATAN, jawatan.HR_NAMA_JAWATAN);
                    }

                }
                ds.Tables.Add(dt);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Senarai_Pergerakan_gaji.xlsx");
                }
            }
        }

        //Json

        public ActionResult GajiMin(string HR_GRED)
        {
            List<HR_JADUAL_GAJI> item = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GajiMax(string HR_GRED, decimal? HR_GAJI_MIN)
        {
            List<HR_MATRIKS_GAJI> item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN == HR_GAJI_MIN).OrderByDescending(s => s.HR_GAJI_MAX).GroupBy(s => s.HR_GAJI_MAX).Select(s => s.FirstOrDefault()).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GajiPokok(string HR_GRED, decimal? HR_GAJI_MIN, decimal? HR_GAJI_MAX)
        {
            List<HR_MATRIKS_GAJI> item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK >= HR_GAJI_MIN && s.HR_GAJI_POKOK <= HR_GAJI_MAX)).OrderByDescending(s => s.HR_TAHAP).GroupBy(s => s.HR_GAJI_POKOK).Select(s => s.FirstOrDefault()).ToList();
            return Json(item.OrderBy(s => s.HR_GAJI_POKOK), JsonRequestBehavior.AllowGet);
        }

        public ActionResult KodMatriks(string HR_GRED, decimal? HR_GAJI_POKOK)
        {
            HR_MATRIKS_GAJI item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_POKOK == HR_GAJI_POKOK).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult PerubahanGaji(string HR_GRED, decimal? HR_GAJI_MIN, decimal? HR_GAJI_MAX, decimal? HR_GAJI_POKOK)
        //{
        //    HR_MATRIKS_GAJI item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN == HR_GAJI_MIN && s.HR_GAJI_MAX == HR_GAJI_MAX && s.HR_GAJI_POKOK == HR_GAJI_POKOK).FirstOrDefault();
        //    if(item == null)
        //    {
        //        item = new HR_MATRIKS_GAJI();
        //    }
        //    return Json(item, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult JenisPergerakan(string HR_NO_PEKERJA, string HR_JENIS_PERGERAKAN)
        {
            PergerakanGajiModels item = new PergerakanGajiModels();
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            if(peribadi != null)
            {
                int? gred = null;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                if (sGred == null)
                {
                    sGred = new GE_PARAMTABLE();
                }
                int? peringkat = null;
                decimal? tahap = null;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                {
                    peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Trim();
                }
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                {
                    peringkat = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1));
                }
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(2, 1) == "T" && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.ToCharArray().Count() > 3)
                {
                    tahap = Convert.ToDecimal(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3));
                }
                string pkt = "P" + peringkat;
                decimal? kenaikan = 0;
                decimal? gajiPokokBaru = 0;
                decimal? gajiPokokBaru2 = 0;
                decimal? gaji_maxsimum = 0;
                decimal? tunggakan = 0;
                HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                if (jadualGaji != null)
                {
                    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                }

                gajiPokokBaru = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                if (gajiPokokBaru > gaji_maxsimum)
                {
                    gajiPokokBaru2 = gaji_maxsimum;
                }
                else
                {
                    gajiPokokBaru2 = gajiPokokBaru;
                }
                HR_MATRIKS_GAJI matriks = new HR_MATRIKS_GAJI();
                if (HR_JENIS_PERGERAKAN == "D")
                {
                    matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                }
                else
                {
                    matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap && s.HR_GAJI_POKOK == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                }

                if(matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                    matriks.HR_GAJI_MIN = 0;
                    matriks.HR_GAJI_MAX = 0;
                    matriks.HR_GAJI_POKOK = 0;
                }

                tunggakan = matriks.HR_GAJI_POKOK - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                if (matriks != null)
                {
                    item.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    item.HR_GRED = sGred.SHORT_DESCRIPTION;
                    item.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    item.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    item.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    item.HR_JUMLAH_PERUBAHAN = tunggakan;
                }

                
            }
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PerubahanGaji(string HR_NO_PEKERJA, decimal? HR_GAJI_BARU)
        {
            decimal? item = 0;
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            if (peribadi != null)
            {
                item = HR_GAJI_BARU - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JobList()
        {
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            List<HR_KEWANGAN8> ListQ8 = db.HR_KEWANGAN8.ToList();
            List<PergerakanGajiModels> item = new List<PergerakanGajiModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = new List<HR_MAKLUMAT_KEWANGAN8>();
            foreach(HR_KEWANGAN8 Q8 in ListQ8)
            {
                
                if (Q8.HR_KOD_KEW8 == "00001")
                {
                    kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => (s.HR_FINALISED_IND_HR == "T" || s.HR_FINALISED_IND_HR == "P") && s.HR_NP_FINALISED_HR == peribadi.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == Q8.HR_KOD_KEW8).GroupBy(s => s.HR_TARIKH_MULA).Select(s => s.FirstOrDefault()).ToList();
                }
                else
                {
                    kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => (s.HR_FINALISED_IND_HR == "T" || s.HR_FINALISED_IND_HR == "P") && s.HR_NP_FINALISED_HR == peribadi.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == Q8.HR_KOD_KEW8).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();
                }
                
                string key = null;
                string value = null;
                int? bulan = null;
                foreach (var kew8 in kewangan8)
                {
                    HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA);
                    PergerakanGajiModels data = new PergerakanGajiModels();
                    data.HR_TARIKH_MULA = kew8.HR_TARIKH_MULA;
                    data.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
                    if(Q8.HR_KOD_KEW8 == "00001")
                    {
                        data.COUNTLIST = kewangan8.Where(s => s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA).Count();
                    }
                    else
                    {
                        data.COUNTLIST = kewangan8.Where(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA).Count();
                    }


                    key = "4";
                    bulan = Convert.ToDateTime(data.HR_TARIKH_MULA).Month;
                    if (data.COUNTLIST == 1)
                    {
                        key = "1";
                        value = data.HR_NO_PEKERJA;
                    }

                    if (Q8.HR_KOD_KEW8 == "00001")
                    {
                        data.HR_BUTIR_PERUBAHAN = "Seramai <strong>" + data.COUNTLIST + "orang </strong> yang belum muktamatkan pergerakan gaji!. <a href='../Kewangan8/PengesahanPergerakanGaji?key=" + key + "&value=" + value + "&bulan=" + bulan + "' class='display-normal'>Klik sini</a> untuk hantar pengesahan anda<br>" +
                                                                        "<span class='pull-right font-xs text-muted'>" +
                                                                            "<i>" + string.Format("{0:dd/MM/yyyy}", data.HR_TARIKH_MULA) + "</i>" +
                                                                        "</span>";
                    }
                    else
                    {
                        var link = Q8.HR_PENERANGAN.Replace(" ", "").ToLower();
                        data.HR_BUTIR_PERUBAHAN = "<strong>" + peribadi2.HR_NAMA_PEKERJA + "</strong> belum muktamatkan "+Q8.HR_PENERANGAN.ToLower()+"!. <a href='../Kewangan8/" + link + "?key=" + key + "&value=" + value + "' class='display-normal'>Klik sini</a> untuk hantar pengesahan anda<br>" +
                                                                        "<span class='pull-right font-xs text-muted'>" +
                                                                            "<i>" + string.Format("{0:dd/MM/yyyy}", data.HR_TARIKH_MULA) + "</i>" +
                                                                        "</span>";
                    }
                        
                    item.Add(data);
                }
            }
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JumlahPelarasan(string HR_NO_PEKERJA)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            foreach(HR_MAKLUMAT_ELAUN_POTONGAN item in elaunPotongan)
            {
                if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                {
                    HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    elaun4.HR_NILAI = item.HR_JUMLAH;
                    elaun3.Add(elaun4);
                }
            }

            return Json(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
        }

        public ActionResult KodPelarasan(string HR_NO_PEKERJA, string Kod, string Value)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            foreach (HR_MAKLUMAT_ELAUN_POTONGAN item in elaunPotongan)
            {
                if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                {
                    HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    //elaun4.HR_NILAI = item.HR_JUMLAH;
                    elaun3.Add(elaun4);
                }
                if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                {
                    HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                    potongan3.Add(potongan4);
                }
            }
            if(Kod == "00039")
            {
                if(Value == "P")
                {
                    return Json(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (Value == "P")
                {
                    return Json(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
                }
            }
            
        }
        public ActionResult CariJumlahPotonganElaun(string HR_NO_PEKERJA, string Kod, string KodElaun)
        {
            db.Configuration.ProxyCreationEnabled = false;

            HR_MAKLUMAT_ELAUN_POTONGAN item = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == KodElaun && s.HR_AKTIF_IND == "Y");
            if(item == null)
            {
                item = new HR_MAKLUMAT_ELAUN_POTONGAN();
                
                if (Kod == "00024")
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == KodElaun);
                    if (elaun == null)
                    {
                        elaun = new HR_ELAUN();
                    }
                    item.HR_JUMLAH = elaun.HR_NILAI;
                    item.HR_KOD_ELAUN_POTONGAN = elaun.HR_KOD_ELAUN;
                    item.HR_TARIKH_MULA = DateTime.Now;
                }
                if (Kod == "00039")
                {
                    HR_POTONGAN potongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == KodElaun);
                    if (potongan == null)
                    {
                        potongan = new HR_POTONGAN();
                    }
                    item.HR_JUMLAH = potongan.HR_NILAI;
                    item.HR_KOD_ELAUN_POTONGAN = potongan.HR_KOD_POTONGAN;
                    item.HR_TARIKH_MULA = DateTime.Now;
                }
                
                
                if(item.HR_JUMLAH == null)
                {
                    item.HR_JUMLAH = 0;
                }
            }
            else
            {
                item.HR_KOD_ELAUN_POTONGAN = null;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConvertTarikhMula(string tarikh)
        {
            var date = Convert.ToDateTime(tarikh);
            var date2 = string.Format("{0:MM/dd/yyyy}", date);
            return Json(date2, JsonRequestBehavior.AllowGet);
        }

        
        //public ActionResult DataKewangan8(string value, string kod)
        //{
        //    List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();

        //    List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
        //    HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == value);

        //    if (peribadi == null)
        //    {
        //        peribadi = new HR_MAKLUMAT_PERIBADI();
        //    }

        //    if (kod == "kew8")
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00002" || s.HR_KOD_PERUBAHAN == "00003" || s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00005" || s.HR_KOD_PERUBAHAN == "00006" || s.HR_KOD_PERUBAHAN == "00007" || s.HR_KOD_PERUBAHAN == "00008" || s.HR_KOD_PERUBAHAN == "00009" || s.HR_KOD_PERUBAHAN == "00010" || s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018" || s.HR_KOD_PERUBAHAN == "00023" || s.HR_KOD_PERUBAHAN == "00027" || s.HR_KOD_PERUBAHAN == "00028" || s.HR_KOD_PERUBAHAN == "00039" || s.HR_KOD_PERUBAHAN == "00040" || s.HR_KOD_PERUBAHAN == "00042" || s.HR_KOD_PERUBAHAN == "00044" || s.HR_KOD_PERUBAHAN == "00045")).ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    else if (kod == "TP")
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00011" || s.HR_KOD_PERUBAHAN == "00014" || s.HR_KOD_PERUBAHAN == "00016" || s.HR_KOD_PERUBAHAN == "00020" || s.HR_KOD_PERUBAHAN == "00035" || s.HR_KOD_PERUBAHAN == "00044")).ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    else if (kod == "CUTI")
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018")).ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    else
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && s.HR_KOD_PERUBAHAN == kod).ToList();
        //    }
        //    return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        //}

        //auto post
        [HttpPost]
        public ActionResult AutoAktif(string HR_NO_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotonganList = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            foreach(HR_MAKLUMAT_ELAUN_POTONGAN item in elaunPotonganList)
            {
                HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                if (elaunPotongan != null && (item.HR_TARIKH_MULA != null || item.HR_TARIKH_AKHIR != null))
                {
                    if(item.HR_TARIKH_MULA <= DateTime.Now && item.HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        item.HR_AKTIF_IND = "Y";
                    }
                    else
                    {
                        item.HR_AKTIF_IND = "T";
                    }
                }
                db.Entry(elaunPotongan).State = EntityState.Modified;
                db.SaveChanges();
            }
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public enum ManageMessageId
        {
            Error,
            Success
        }
    }
}
