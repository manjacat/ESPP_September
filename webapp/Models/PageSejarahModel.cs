using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{

    public class PageSejarahModel
    {
        public PageSejarahModel()
        {
            bulandibayar = DateTime.Now.Month;
            bulanbekerja = DateTime.Now.AddMonths(-1).Month;
            tahundibayar = DateTime.Now.Year;
            tahunbekerja = DateTime.Now.AddMonths(-1).Year;
            kelulusanydp = "P";
            kelulusanydptunggakan = "P";
            IsMuktamad = false;
        }

        public PageSejarahModel(string HR_PEKERJAstr,
            string tahunbekerjastr,
            string bulanbekerjastr,
            string tahundibayarstr,
            string bulandibayarstr)
        {
            HR_PEKERJA = HR_PEKERJAstr;
            tahunbekerja = string.IsNullOrEmpty(tahunbekerjastr) ? DateTime.Now.Year : Convert.ToInt32(tahunbekerjastr);
            bulanbekerja = string.IsNullOrEmpty(bulanbekerjastr) ? DateTime.Now.AddMonths(-1).Month : Convert.ToInt32(bulanbekerjastr);
            tahundibayar = string.IsNullOrEmpty(tahundibayarstr) ? DateTime.Now.Year : Convert.ToInt32(tahundibayarstr);
            bulandibayar = string.IsNullOrEmpty(bulandibayarstr) ? DateTime.Now.Month : Convert.ToInt32(bulandibayarstr);
            try
            {
                //HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_TAHUN_BEKERJA == tahunbekerja && s.HR_KOD_IND == "G");
                ApplicationDbContext db = new ApplicationDbContext();
                int? muktamad = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(x => x.HR_NO_PEKERJA == HR_PEKERJA
                    && x.HR_TAHUN_BEKERJA == tahunbekerja
                    && x.HR_TAHUN == tahundibayar
                    && x.HR_BULAN_BEKERJA == bulanbekerja
                    && x.HR_BULAN_DIBAYAR == bulandibayar
                    && x.HR_KOD_IND == "G").Select(x => x.HR_MUKTAMAD).FirstOrDefault();
                if (muktamad == 1) //if muktamad = 1
                {
                    IsMuktamad = true;
                }
            }
            catch
            {
                IsMuktamad = false;
            }

        }

        // bulan bekerja (current)
        [Display(Name = "No. Pekerja")]
        public string HR_PEKERJA { get; set; }
        [Display(Name = "Tahun Bekerja")]
        public int tahunbekerja { get; set; }
        [Display(Name = "Bahagian")]
        public string bahagian { get; set; }
        [Display(Name = "Bulan Bekerja")]
        public int bulanbekerja { get; set; }
        [Display(Name = "Jabatan")]
        public string jabatan { get; set; }
        [Display(Name = "Tahun Dibayar")]
        public int tahundibayar { get; set; }
        [Display(Name = "Tunggakan")]
        public string tunggakan { get; set; } //values: Y/T
        [Display(Name = "Bulan Dibayar")]
        public int bulandibayar { get; set; }
        [Display(Name = "Gaji Pokok")]
        public decimal gajipokok { get; set; }
        [Display(Name = "Jumlah Hari")]
        public int jumlahhari { get; set; }
        [Display(Name = "Elaun Khidmat Awam")]
        public decimal elaunkhidmatawam { get; set; }
        [Display(Name = "Jumlah OT")]
        public int jumlahot { get; set; }
        [Display(Name = "Elaun Lain")]
        public decimal elaunlain { get; set; }
        [Display(Name = "Jumlah Bayar OT")]
        public decimal jumlahbayaranot { get; set; }
        [Display(Name = "Potongan KDSK")]
        public decimal potonganksdk { get; set; }
        [Display(Name = "1/3 Gaji Pokok")]
        public decimal gajiper3 { get; set; }
        [Display(Name = "Potongan KSWP")]
        public decimal potongankwsp { get; set; }
        [Display(Name = "SOCSO")]
        public decimal socso { get; set; }
        [Display(Name = "Lain-lain Potongan")]
        public decimal potonganlain { get; set; }
        [Display(Name = "Gaji Sebelum Potongan KWSP")]
        public decimal gajisebelumkwsp { get; set; }
        [Display(Name = "Gaji Kasar Bulan Semasa")]
        public decimal gajikasar { get; set; }
        [Display(Name = "Gaji Bersih")]
        public decimal gajibersih { get; set; }
        [Display(Name = "Kelulusan Datuk Bandar")]
        public string kelulusanydp { get; set; } //values Y, T, P

        //bulan sebelum
        [Display(Name = "Tahun Bekerja")]
        public int tunggakantahunbekerja
        {
            get
            {
                try
                {
                    if (bulanbekerja == 1)
                    {
                        return tahunbekerja - 1;
                    }
                    return tahunbekerja;
                }
                catch
                {
                    return 2000;
                }
            }
        }

        [Display(Name = "Bulan Bekerja")]
        public int tunggakanbulanbekerja
        {
            get
            {
                try
                {
                    if (bulanbekerja == 1)
                    {
                        return 12;
                    }
                    return bulanbekerja - 1;
                }
                catch
                {
                    return 1;
                }
            }
        }
        [Display(Name = "Tahun Dibayar")]
        public int tunggakantahundibayar
        {
            get
            {
                return tahundibayar;
            }
        }
        [Display(Name = "Bulan Dibayar")]
        public int tunggakanbulandibayar
        {
            get
            {
                return bulandibayar;
            }
        }

        [Display(Name = "Gaji Pokok")]
        public decimal tunggakangajipokok { get; set; }
        [Display(Name = "Jumlah Hari")]
        public int tunggakanjumlahhari { get; set; }
        [Display(Name = "Elaun Khidmat Awam")]
        public decimal tunggakanelaunkhidmatawam { get; set; }
        [Display(Name = "Jumlah OT")]
        public int tunggakanjumlahot { get; set; }
        [Display(Name = "Elaun Lain")]
        public decimal tunggakanelaunlain { get; set; }
        [Display(Name = "Jumlah Bayar OT")]
        public decimal tunggakanjumlahbayaranot { get; set; }
        [Display(Name = "Potongan KDSK")]
        public decimal tunggakanpotonganksdk { get; set; }
        [Display(Name = "1/3 Gaji Pokok")]
        public decimal tunggakangajiper3 { get; set; }
        [Display(Name = "Potongan KSWP")]
        public decimal tunggakanpotongankwsp { get; set; }
        [Display(Name = "SOCSO")]
        public decimal tunggakansocso { get; set; }
        [Display(Name = "Lain-lain Potongan")]
        public decimal tunggakanpotonganlain { get; set; }
        [Display(Name = "Gaji Sebelum Potongan KWSP")]
        public decimal tunggakangajisebelumkwsp { get; set; }
        [Display(Name = "Gaji Kasar Bulan Semasa")]
        public decimal tunggakangajikasar { get; set; }
        [Display(Name = "Gaji Bersih")]
        public decimal tunggakangajibersih { get; set; }
        [Display(Name = "Kelulusan Datuk Bandar")]
        public string kelulusanydptunggakan { get; set; } //values Y, T, P

        public bool IsMuktamad { get; set; }

        //List Sjarah Pembayaran?

        //Methods
        public int GetJumlahHari()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var jumlahHari = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BEKERJA == bulanbekerja
                && x.HR_TAHUN_BEKERJA == tahunbekerja
                && x.HR_BULAN_DIBAYAR == bulandibayar
                && x.HR_TAHUN == tahundibayar
                && x.HR_KOD == "GAJPS").Select(x => x.HR_JAM_HARI).FirstOrDefault();
            jumlahHari = jumlahHari == null ? 0 : jumlahHari;
            return Convert.ToInt32(jumlahHari);
        }

        public int GetJumlahOT()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var jumlahHari = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BEKERJA == bulanbekerja
                && x.HR_TAHUN_BEKERJA == tahunbekerja
                && x.HR_BULAN_DIBAYAR == bulandibayar
                && x.HR_TAHUN == tahundibayar
                && x.HR_KOD == "E0164").Select(x => x.HR_JAM_HARI).FirstOrDefault();
            jumlahHari = jumlahHari == null ? 0 : jumlahHari;
            return Convert.ToInt32(jumlahHari);
        }


        public static PageSejarahModel Insert(PageSejarahModel agree, string user, string command)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            //MajlisContext mc = new MajlisContext();
            List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
            HR_MAKLUMAT_PEKERJAAN mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA).SingleOrDefault();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun = db.HR_MAKLUMAT_ELAUN_POTONGAN
                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_KOD_ELAUN_POTONGAN.Contains("E")
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && s.HR_KOD_ELAUN_POTONGAN != "E0164").ToList();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatpotongan1 = db.HR_MAKLUMAT_ELAUN_POTONGAN
                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_KOD_ELAUN_POTONGAN.Contains("P")
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && s.HR_KOD_ELAUN_POTONGAN != "P0015").ToList();
            HR_MAKLUMAT_ELAUN_POTONGAN maklumatpotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault
                (s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_KOD_ELAUN_POTONGAN.Contains("P")
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && s.HR_KOD_ELAUN_POTONGAN == "P0015");
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where
                (s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_KOD_ELAUN_POTONGAN.Contains("C")
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && s.HR_KOD_ELAUN_POTONGAN != "C0020").ToList();
            agree.gajipokok = mpekerjaan.HR_GAJI_POKOK != null ? Convert.ToDecimal(mpekerjaan.HR_GAJI_POKOK) : 0;
            var gajisehari = (mpekerjaan.HR_GAJI_POKOK / 23) * agree.jumlahhari;
            decimal gajipokok = gajisehari != null ? Convert.ToDecimal(gajisehari) : 0;
            gajipokok = Decimal.Round(gajipokok, 2);
            var gajisehariot = (((agree.gajipokok / 23) * agree.jumlahhari) * 12 / 2504);
            var gajisehariot1 = gajisehariot * agree.jumlahot;

            var tbl = db.Users.Where(p => p.Id == user).SingleOrDefault();
            var emel = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == tbl.UserName).SingleOrDefault();
            var role1 = db.UserRoles.Where(d => d.UserId == tbl.Id).SingleOrDefault();
            var role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();

            if (string.IsNullOrEmpty(command))
            {
                command = "muktamad";
            }

            switch (command.ToLower())
            {
                case ("hantar"):
                    InsertHantar(db, agree, listkwsp, maklumatelaun, maklumatpotongan,
                        maklumatcaruman, gajipokok, gajisehariot1);
                    TrailLog(emel, role,
                        emel.HR_NAMA_PEKERJA + " Telah menambah data untuk pekerja " + agree.HR_PEKERJA);
                    break;
                case ("kemaskini"):
                    InsertKemaskini(db, agree, listkwsp, maklumatelaun, maklumatpotongan,
                        maklumatcaruman, gajipokok, gajisehariot1);
                    TrailLog(emel, role,
                        emel.HR_NAMA_PEKERJA + " Telah mengubah data untuk pekerja " + agree.HR_PEKERJA);
                    break;
                case ("muktamad"):
                    InsertMuktamad(db, agree);
                    TrailLog(emel, role,
                        emel.HR_NAMA_PEKERJA + " Telah mengubah data untuk pekerja " + agree.HR_PEKERJA);                     
                    break;
                default:
                    break;
            }
            return agree;
        }

        private static void InsertMuktamad(ApplicationDbContext db, PageSejarahModel agree)
        {
            HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                && s.HR_TAHUN == agree.tahundibayar
                && s.HR_KOD == "GAJPS").SingleOrDefault();
            gaji.HR_MUKTAMAD = 1;

            db.Entry(gaji).State = EntityState.Modified;
            db.SaveChanges();
        }

        private static void InsertKemaskini(ApplicationDbContext db, PageSejarahModel agree,
            List<HR_KWSP> listkwsp,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun,
            HR_MAKLUMAT_ELAUN_POTONGAN maklumatpotongan,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
            decimal gajipokok, decimal gajisehariot1)
        {
            InsertBulan(db, agree, gajisehariot1);
            if (agree.tunggakan == "Y")
            {
                InsertTunggakan(db, agree, listkwsp, maklumatelaun, maklumatpotongan, maklumatcaruman, gajipokok, gajisehariot1);
            }
        }

        private static void InsertBulan(ApplicationDbContext db, PageSejarahModel agree,
            decimal gajisehariot1)
        {
            HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                && s.HR_TAHUN == agree.tahundibayar
                && s.HR_KOD == "GAJPS").SingleOrDefault();
            HR_TRANSAKSI_SAMBILAN_DETAIL ot = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                && s.HR_TAHUN == agree.tahundibayar
                && s.HR_KOD == "E0164").SingleOrDefault();
            if (gaji != null)
            {
                gaji.HR_JUMLAH = agree.gajipokok;
                gaji.HR_JAM_HARI = agree.jumlahhari;
                gaji.HR_YDP_LULUS_IND = agree.kelulusanydptunggakan;
                db.Entry(gaji).State = EntityState.Modified;
            }
            else
            {
                gaji = new HR_TRANSAKSI_SAMBILAN_DETAIL
                {
                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                    HR_BULAN_BEKERJA = agree.bulanbekerja,
                    HR_TAHUN_BEKERJA = agree.tahunbekerja,
                    HR_BULAN_DIBAYAR = agree.bulandibayar,
                    HR_TAHUN = agree.tahundibayar,
                    HR_KOD = "GAJPS",
                    HR_KOD_IND = "G",
                    HR_JUMLAH = agree.gajipokok,
                    HR_JAM_HARI = agree.jumlahhari,
                    HR_YDP_LULUS_IND = agree.kelulusanydp
                };
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gaji);
            }
            if (ot != null)
            {
                ot.HR_JUMLAH = gajisehariot1;
                ot.HR_JAM_HARI = agree.jumlahot;
                ot.HR_YDP_LULUS_IND = agree.kelulusanydp;
                db.Entry(ot).State = EntityState.Modified;
            }
            else
            {
                ot = new HR_TRANSAKSI_SAMBILAN_DETAIL
                {
                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                    HR_BULAN_BEKERJA = agree.bulanbekerja,
                    HR_TAHUN_BEKERJA = agree.tahunbekerja,
                    HR_BULAN_DIBAYAR = agree.bulandibayar,
                    HR_TAHUN = agree.tahundibayar,
                    HR_KOD = "E0164",
                    HR_KOD_IND = "E",
                    HR_JUMLAH = gajisehariot1,
                    HR_JAM_HARI = agree.jumlahot,
                    HR_YDP_LULUS_IND = agree.kelulusanydptunggakan
                };
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(ot);
            }

            db.SaveChanges();
        }

        private static void InsertTunggakan(ApplicationDbContext db, PageSejarahModel agree,
            List<HR_KWSP> listkwsp,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun,
            HR_MAKLUMAT_ELAUN_POTONGAN maklumatpotongan,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
            decimal gajipokok, decimal gajisehariot1)
        {
            foreach (var kwsp in listkwsp)
            {
                if (gajipokok >= kwsp.HR_UPAH_DARI && gajipokok <= kwsp.HR_UPAH_HINGGA)
                {
                    InsertHRSAMBILAN(db, agree, true);
                    InsertMAJIKANKWSP(db, agree, kwsp, true);
                    InsertPekerjaKSWP(db, agree, kwsp, true);

                    if (maklumatpotongan != null)
                    {
                        InsertMAKLUMATPOTONGAN(db, agree, maklumatpotongan, true);
                    }
                    InsertELAUNOT(db, agree, gajisehariot1, true);
                    InsertGAJIPEKERJA(db, agree, true);
                    InsertMAKLUMATELAUN(db, agree, maklumatelaun, true);
                    InsertMAKLUMATCARUMAN(db, agree, maklumatcaruman, true);
                }
            }
        }


        private static void TrailLog(HR_MAKLUMAT_PERIBADI emel, IdentityRole role,
            string message)
        {
            try
            {
                System.Web.HttpContext httpContext = System.Web.HttpContext.Current;
                new AuditTrailModels().Log(emel.HR_EMAIL, emel.HR_NAMA_PEKERJA,
                   httpContext.Request.UserHostAddress,
                   role.Name,
                   message,
                   System.Net.Dns.GetHostName(),
                   emel.HR_TELBIMBIT, httpContext.Request.RawUrl, "Sejarah");
            }
            catch
            {

            }

        }

        private static void InsertHantar(ApplicationDbContext db, PageSejarahModel agree,
            List<HR_KWSP> listkwsp,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun,
            HR_MAKLUMAT_ELAUN_POTONGAN maklumatpotongan,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
            decimal gajipokok, decimal gajisehariot1)
        {
            //instead of looping 401 records, a LINQ is used here to get the correct KWSP based on gaji pokok
            var kwsp = listkwsp
                .Where(s => gajipokok >= s.HR_UPAH_DARI
                && gajipokok <= s.HR_UPAH_HINGGA).SingleOrDefault();
            try
            {
                InsertHRSAMBILAN(db, agree);
                InsertMAJIKANKWSP(db, agree, kwsp);
                InsertPekerjaKSWP(db, agree, kwsp);
                if (maklumatpotongan != null)
                {
                    InsertMAKLUMATPOTONGAN(db, agree, maklumatpotongan);
                }
                InsertELAUNOT(db, agree, gajisehariot1);
                InsertGAJIPEKERJA(db, agree);
                InsertMAKLUMATELAUN(db, agree, maklumatelaun);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (agree.tunggakan == "Y")
            {
                try
                {                    
                    InsertHRSAMBILAN(db, agree, true);
                    InsertMAJIKANKWSP(db, agree, kwsp, true);
                    InsertPekerjaKSWP(db, agree, kwsp, true);

                    if (maklumatpotongan != null)
                    {
                        InsertMAKLUMATPOTONGAN(db, agree, maklumatpotongan, true);
                    }
                    InsertELAUNOT(db, agree, gajisehariot1, true);
                    InsertGAJIPEKERJA(db, agree, true);
                    InsertMAKLUMATELAUN(db, agree, maklumatelaun, true);
                    InsertMAKLUMATCARUMAN(db, agree, maklumatcaruman, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


        }

        #region Insert Into HR SAMBILAN DETAIL

        private static void InsertHRSAMBILAN(ApplicationDbContext db, PageSejarahModel agree, 
            bool isTunggakan = false)
        {
            //if bukan tunggakan
            if (!isTunggakan)
            {
                HR_TRANSAKSI_SAMBILAN sambilan = db.HR_TRANSAKSI_SAMBILAN
                       .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                       && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                       && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                       && s.HR_TAHUN == agree.tahundibayar
                       && s.HR_TAHUN_BEKERJA == agree.tahunbekerja).FirstOrDefault();
                if (sambilan == null)
                {
                    try
                    {
                        sambilan = new HR_TRANSAKSI_SAMBILAN
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_BEKERJA = agree.bulanbekerja,
                            HR_TAHUN_BEKERJA = agree.tahunbekerja,
                            HR_TAHUN = agree.tahundibayar,
                            HR_BULAN_DIBAYAR = agree.bulandibayar
                        };
                        db.HR_TRANSAKSI_SAMBILAN.Add(sambilan);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }
                    
                }
            }
            else //if tunggakan
            {
                HR_TRANSAKSI_SAMBILAN sambilan = db.HR_TRANSAKSI_SAMBILAN
                       .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                       && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                       && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                       && s.HR_TAHUN == agree.tunggakantahundibayar
                       && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja).FirstOrDefault();
                if (sambilan == null)
                {
                    //this try/catch block is to prevent system crashing 
                    //when we try to "Tambah" a new tunggakan for a pekerja.
                    //this will ensure all other data is inserted to HR_TRANSAKSI_SAMBILAN_DETAIL later.
                    try
                    {
                        sambilan = new HR_TRANSAKSI_SAMBILAN
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                            HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                            HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                            HR_TAHUN = agree.tunggakantahundibayar
                        };
                        db.HR_TRANSAKSI_SAMBILAN.Add(sambilan);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }                    
                }
            }

        }

        private static void InsertMAJIKANKWSP(ApplicationDbContext db, PageSejarahModel agree, HR_KWSP kwsp, bool isTunggakan = false)
        {
            if (!isTunggakan)
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL majikankwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                      && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                      && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                      && s.HR_TAHUN == agree.tahundibayar
                      && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                      && s.HR_KOD == "C0020").FirstOrDefault();

                if (majikankwsp != null)
                {
                    majikankwsp.HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN;
                    majikankwsp.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    majikankwsp.HR_KOD_IND = "C";
                    majikankwsp.HR_TUNGGAKAN_IND = "T";
                    majikankwsp.HR_MUKTAMAD = 0;
                    db.Entry(majikankwsp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    majikankwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.bulandibayar,
                        HR_TAHUN = agree.tahundibayar,
                        HR_KOD = "C0020",
                        HR_BULAN_BEKERJA = agree.bulanbekerja,
                        HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN,
                        HR_KOD_IND = "C",
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = agree.tahunbekerja,
                        HR_YDP_LULUS_IND = agree.kelulusanydp,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(majikankwsp);
                    db.SaveChanges();
                }
            }
            else
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL majikankwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                      && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                      && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                      && s.HR_TAHUN == agree.tunggakantahundibayar
                      && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                      && s.HR_KOD == "C0020").FirstOrDefault();

                if (majikankwsp != null)
                {
                    majikankwsp.HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN;
                    majikankwsp.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    majikankwsp.HR_KOD_IND = "C";
                    majikankwsp.HR_TUNGGAKAN_IND = "T";
                    majikankwsp.HR_MUKTAMAD = 0;
                    db.Entry(majikankwsp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    majikankwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                        HR_TAHUN = agree.tunggakantahundibayar,
                        HR_KOD = "C0020",
                        HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                        HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN,
                        HR_KOD_IND = "C",
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                        HR_YDP_LULUS_IND = agree.kelulusanydp,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(majikankwsp);
                    db.SaveChanges();
                }
            }
        }

        private static void InsertPekerjaKSWP(ApplicationDbContext db, PageSejarahModel agree, HR_KWSP kwsp, bool isTunggakan = false)
        {
            if (!isTunggakan)
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL pekerjakwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                    && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                    && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                    && s.HR_TAHUN == agree.tahundibayar
                    && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                    && s.HR_KOD == "P0035").FirstOrDefault();

                if (pekerjakwsp != null)
                {
                    pekerjakwsp.HR_BULAN_DIBAYAR = agree.bulandibayar;
                    pekerjakwsp.HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA;
                    pekerjakwsp.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    pekerjakwsp.HR_KOD_IND = "P";
                    pekerjakwsp.HR_TUNGGAKAN_IND = "T";
                    pekerjakwsp.HR_MUKTAMAD = 0;
                    db.Entry(pekerjakwsp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    pekerjakwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.bulandibayar,
                        HR_TAHUN = agree.tahundibayar,
                        HR_KOD = "P0035",
                        HR_BULAN_BEKERJA = agree.bulanbekerja,
                        HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA,
                        HR_KOD_IND = "P",
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = agree.tahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(pekerjakwsp);
                    db.SaveChanges();
                }
            }
            else
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL pekerjakwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                   .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                   && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                   && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                   && s.HR_TAHUN == agree.tunggakantahundibayar
                   && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                   && s.HR_KOD == "P0035").FirstOrDefault();

                if (pekerjakwsp != null)
                {
                    pekerjakwsp.HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA;
                    pekerjakwsp.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    pekerjakwsp.HR_KOD_IND = "P";
                    pekerjakwsp.HR_TUNGGAKAN_IND = "T";
                    pekerjakwsp.HR_MUKTAMAD = 0;
                    db.Entry(pekerjakwsp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    pekerjakwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                        HR_TAHUN = agree.tunggakantahundibayar,
                        HR_KOD = "P0035",
                        HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                        HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA,
                        HR_KOD_IND = "P",
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(pekerjakwsp);
                    db.SaveChanges();
                }
            }

        }

        private static void InsertMAKLUMATPOTONGAN(ApplicationDbContext db, PageSejarahModel agree, HR_MAKLUMAT_ELAUN_POTONGAN maklumatpotongan,
            bool isTunggakan = false)
        {
            if (!isTunggakan)
            {

                HR_TRANSAKSI_SAMBILAN_DETAIL ksdk = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                                && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                                && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                                && s.HR_TAHUN == agree.tahundibayar
                                && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                                && s.HR_KOD == maklumatpotongan.HR_KOD_ELAUN_POTONGAN
                                && s.HR_KOD_IND == maklumatpotongan.HR_ELAUN_POTONGAN_IND
                                && s.HR_TUNGGAKAN_IND == "T").FirstOrDefault();

                if (ksdk != null)
                {
                    ksdk.HR_JUMLAH = maklumatpotongan.HR_JUMLAH;
                    ksdk.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    db.Entry(ksdk).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {

                    ksdk = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.bulandibayar,
                        HR_TAHUN = agree.tahundibayar,
                        HR_KOD = maklumatpotongan.HR_KOD_ELAUN_POTONGAN,
                        HR_BULAN_BEKERJA = agree.bulanbekerja,
                        HR_JUMLAH = maklumatpotongan.HR_JUMLAH,
                        HR_KOD_IND = maklumatpotongan.HR_ELAUN_POTONGAN_IND,
                        HR_TUNGGAKAN_IND = "T",
                        HR_YDP_LULUS_IND = agree.kelulusanydp,
                        HR_TAHUN_BEKERJA = agree.tahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(ksdk);
                    db.SaveChanges();
                }
            }
            else
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL ksdk = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                                && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                                && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                                && s.HR_TAHUN == agree.tunggakantahundibayar
                                && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                                && s.HR_KOD == maklumatpotongan.HR_KOD_ELAUN_POTONGAN
                                && s.HR_KOD_IND == maklumatpotongan.HR_ELAUN_POTONGAN_IND
                                && s.HR_TUNGGAKAN_IND == "T").FirstOrDefault();

                if (ksdk != null)
                {
                    ksdk.HR_JUMLAH = maklumatpotongan.HR_JUMLAH;
                    ksdk.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    db.Entry(ksdk).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    ksdk = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                        HR_TAHUN = agree.tunggakantahundibayar,
                        HR_KOD = maklumatpotongan.HR_KOD_ELAUN_POTONGAN,
                        HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                        HR_JUMLAH = maklumatpotongan.HR_JUMLAH,
                        HR_KOD_IND = maklumatpotongan.HR_ELAUN_POTONGAN_IND,
                        HR_TUNGGAKAN_IND = "T",
                        HR_YDP_LULUS_IND = agree.kelulusanydp,
                        HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(ksdk);
                    db.SaveChanges();
                }
            }

        }

        private static void InsertELAUNOT(ApplicationDbContext db, PageSejarahModel agree, decimal gajisehariot1,
            bool isTunggakan = false)
        {
            if (!isTunggakan)
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL elaunot = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                     .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                     && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                     && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                     && s.HR_TAHUN == agree.tahundibayar
                     && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                     && s.HR_KOD == "E0164").FirstOrDefault();
                if (elaunot != null)
                {
                    elaunot.HR_JAM_HARI = agree.jumlahot;
                    elaunot.HR_JUMLAH = gajisehariot1;
                    elaunot.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    elaunot.HR_KOD_IND = "E";
                    elaunot.HR_TUNGGAKAN_IND = "T";
                    elaunot.HR_MUKTAMAD = 0;
                    db.Entry(elaunot).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    elaunot = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.bulandibayar,
                        HR_TAHUN = agree.tahundibayar,
                        HR_KOD = "E0164",
                        HR_BULAN_BEKERJA = agree.bulanbekerja,
                        HR_JUMLAH = gajisehariot1,
                        HR_KOD_IND = "E",
                        HR_TUNGGAKAN_IND = "T",
                        HR_JAM_HARI = agree.jumlahot,
                        HR_YDP_LULUS_IND = agree.kelulusanydp,
                        HR_TAHUN_BEKERJA = agree.tahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunot);
                    db.SaveChanges();
                }
            }
            else
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL elaunot = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                     .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                     && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                     && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                     && s.HR_TAHUN == agree.tunggakantahundibayar
                     && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                     && s.HR_KOD == "E0164").FirstOrDefault();
                if (elaunot != null)
                {
                    elaunot.HR_JAM_HARI = agree.jumlahot;
                    elaunot.HR_JUMLAH = gajisehariot1;
                    elaunot.HR_YDP_LULUS_IND = agree.kelulusanydp;
                    elaunot.HR_KOD_IND = "E";
                    elaunot.HR_TUNGGAKAN_IND = "T";
                    elaunot.HR_MUKTAMAD = 0;
                    db.Entry(elaunot).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    elaunot = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                        HR_TAHUN = agree.tunggakantahundibayar,
                        HR_KOD = "E0164",
                        HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                        HR_JUMLAH = gajisehariot1,
                        HR_KOD_IND = "E",
                        HR_TUNGGAKAN_IND = "T",
                        HR_JAM_HARI = agree.jumlahot,
                        HR_YDP_LULUS_IND = agree.kelulusanydp,
                        HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunot);
                    db.SaveChanges();
                }
            }

        }

        private static void InsertGAJIPEKERJA(ApplicationDbContext db, PageSejarahModel agree, bool isTunggakan = false)
        {
            if (!isTunggakan)
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL gajipekerja = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                      && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                      && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                      && s.HR_TAHUN == agree.tahundibayar
                      && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                      && s.HR_KOD == "GAJPS").FirstOrDefault();
                if (gajipekerja != null)
                {
                    gajipekerja.HR_JUMLAH = agree.gajipokok;
                    gajipekerja.HR_KOD_IND = "G";
                    gajipekerja.HR_JAM_HARI = agree.jumlahhari;
                    gajipekerja.HR_TUNGGAKAN_IND = "T";
                    gajipekerja.HR_MUKTAMAD = 0;
                    db.Entry(gajipekerja).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    gajipekerja = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.bulandibayar,
                        HR_TAHUN = agree.tahundibayar,
                        HR_KOD = "GAJPS",
                        HR_BULAN_BEKERJA = agree.bulanbekerja,
                        HR_JUMLAH = agree.gajipokok,
                        HR_KOD_IND = "G",
                        HR_JAM_HARI = agree.jumlahhari,
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = agree.tahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gajipekerja);
                    db.SaveChanges();
                }
            }
            else
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL gajipekerja = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                      && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                      && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                      && s.HR_TAHUN == agree.tunggakantahundibayar
                      && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                      && s.HR_KOD == "GAJPS").FirstOrDefault();
                if (gajipekerja != null)
                {
                    gajipekerja.HR_JUMLAH = agree.gajipokok;
                    gajipekerja.HR_KOD_IND = "G";
                    gajipekerja.HR_JAM_HARI = agree.jumlahhari;
                    gajipekerja.HR_TUNGGAKAN_IND = "T";
                    gajipekerja.HR_MUKTAMAD = 0;
                    db.Entry(gajipekerja).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    gajipekerja = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                        HR_TAHUN = agree.tunggakantahundibayar,
                        HR_KOD = "GAJPS",
                        HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                        HR_JUMLAH = agree.gajipokok,
                        HR_KOD_IND = "G",
                        HR_JAM_HARI = agree.jumlahhari,
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gajipekerja);
                    db.SaveChanges();
                }
            }
        }

        private static void InsertMAKLUMATELAUN(ApplicationDbContext db, PageSejarahModel agree,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun, bool isTunggakan = false)
        {
            if (!isTunggakan)
            {
                foreach (var sum in maklumatelaun)
                {
                    HR_TRANSAKSI_SAMBILAN_DETAIL elaunlain = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                         .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                        && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                        && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                        && s.HR_TAHUN == agree.tahundibayar
                        && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                        && s.HR_KOD == sum.HR_KOD_ELAUN_POTONGAN
                        && s.HR_KOD_IND == sum.HR_ELAUN_POTONGAN_IND).FirstOrDefault();
                    if (elaunlain != null)
                    {
                        elaunlain.HR_JUMLAH = sum.HR_JUMLAH;
                        elaunlain.HR_TUNGGAKAN_IND = "T";
                        elaunlain.HR_MUKTAMAD = 0;
                        db.Entry(elaunlain).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        elaunlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = agree.bulandibayar,
                            HR_TAHUN = agree.tahundibayar,
                            HR_KOD = sum.HR_KOD_ELAUN_POTONGAN,
                            HR_BULAN_BEKERJA = agree.bulanbekerja,
                            HR_JUMLAH = sum.HR_JUMLAH,
                            HR_KOD_IND = sum.HR_ELAUN_POTONGAN_IND,
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = agree.tahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunlain);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                foreach (var sum in maklumatelaun)
                {
                    HR_TRANSAKSI_SAMBILAN_DETAIL elaunlain = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                         .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                        && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                        && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                        && s.HR_TAHUN == agree.tunggakantahundibayar
                        && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                        && s.HR_KOD == sum.HR_KOD_ELAUN_POTONGAN
                        && s.HR_KOD_IND == sum.HR_ELAUN_POTONGAN_IND).FirstOrDefault();
                    if (elaunlain != null)
                    {
                        elaunlain.HR_JUMLAH = sum.HR_JUMLAH;
                        elaunlain.HR_TUNGGAKAN_IND = "T";
                        elaunlain.HR_MUKTAMAD = 0;
                        db.Entry(elaunlain).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        elaunlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                            HR_TAHUN = agree.tunggakantahundibayar,
                            HR_KOD = sum.HR_KOD_ELAUN_POTONGAN,
                            HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                            HR_JUMLAH = sum.HR_JUMLAH,
                            HR_KOD_IND = sum.HR_ELAUN_POTONGAN_IND,
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunlain);
                        db.SaveChanges();
                    }
                }
            }

        }

        private static void InsertMAKLUMATCARUMAN(ApplicationDbContext db, PageSejarahModel agree, List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
            bool isTunggakan = false)
        {
            if (!isTunggakan)
            {
                foreach (var sum2 in maklumatcaruman)
                {
                    HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                        .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                        && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                        && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                        && s.HR_TAHUN == agree.tahundibayar
                        && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                        && s.HR_KOD == sum2.HR_KOD_ELAUN_POTONGAN
                        && s.HR_KOD_IND == sum2.HR_ELAUN_POTONGAN_IND).FirstOrDefault();

                    if (potonganlain != null)
                    {
                        potonganlain.HR_JUMLAH = sum2.HR_JUMLAH;
                        potonganlain.HR_TUNGGAKAN_IND = "T";
                        potonganlain.HR_MUKTAMAD = 0;
                        db.Entry(potonganlain).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = agree.bulandibayar,
                            HR_TAHUN = agree.tahundibayar,
                            HR_KOD = sum2.HR_KOD_ELAUN_POTONGAN,
                            HR_BULAN_BEKERJA = agree.bulanbekerja,
                            HR_JUMLAH = sum2.HR_JUMLAH,
                            HR_KOD_IND = sum2.HR_ELAUN_POTONGAN_IND,
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = agree.tahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                foreach (var sum2 in maklumatcaruman)
                {
                    HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                        .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                        && s.HR_BULAN_DIBAYAR == agree.tunggakanbulandibayar
                        && s.HR_BULAN_BEKERJA == agree.tunggakanbulanbekerja
                        && s.HR_TAHUN == agree.tunggakantahundibayar
                        && s.HR_TAHUN_BEKERJA == agree.tunggakantahunbekerja
                        && s.HR_KOD == sum2.HR_KOD_ELAUN_POTONGAN
                        && s.HR_KOD_IND == sum2.HR_ELAUN_POTONGAN_IND).FirstOrDefault();

                    if (potonganlain != null)
                    {
                        potonganlain.HR_JUMLAH = sum2.HR_JUMLAH;
                        potonganlain.HR_TUNGGAKAN_IND = "T";
                        potonganlain.HR_MUKTAMAD = 0;
                        db.Entry(potonganlain).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = agree.tunggakanbulandibayar,
                            HR_TAHUN = agree.tunggakantahundibayar,
                            HR_KOD = sum2.HR_KOD_ELAUN_POTONGAN,
                            HR_BULAN_BEKERJA = agree.tunggakanbulanbekerja,
                            HR_JUMLAH = sum2.HR_JUMLAH,
                            HR_KOD_IND = sum2.HR_ELAUN_POTONGAN_IND,
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = agree.tunggakantahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
                        db.SaveChanges();
                    }
                }
            }
        }

        #endregion


        private static void UpdateSambilanDetail(ApplicationDbContext db, PageSejarahModel agree)
        {
            bool isNew = false;
            HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_NO_PEKERJA == agree.HR_PEKERJA
                && x.HR_TAHUN_BONUS == agree.tahundibayar
                && x.HR_BULAN_BONUS == agree.bulandibayar).FirstOrDefault();
            if (det == null)
            {
                isNew = true;
                //insert det
                det = new HR_BONUS_SAMBILAN_DETAIL();
                det.HR_NO_PEKERJA = agree.HR_PEKERJA;
                det.HR_TAHUN_BONUS = agree.tahundibayar;
                det.HR_BULAN_BONUS = agree.bulandibayar;
                det.HR_NO_KPBARU = db.HR_MAKLUMAT_PERIBADI
                    .Where(p => p.HR_NO_PEKERJA == agree.HR_PEKERJA)
                    .Select(p => p.HR_NO_KPBARU).FirstOrDefault();
            }

            det = UpdateBonusSambilanInfo(db, det, agree);

            if (isNew)
            {
                db.HR_BONUS_SAMBILAN_DETAIL.Add(det);
                db.SaveChanges();
            }
            else
            {
                //update det
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private static HR_BONUS_SAMBILAN_DETAIL UpdateBonusSambilanInfo
                (ApplicationDbContext db, HR_BONUS_SAMBILAN_DETAIL det, PageSejarahModel agree)
        {
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain =
               db.HR_TRANSAKSI_SAMBILAN_DETAIL.
               Where(x => x.HR_NO_PEKERJA == agree.HR_PEKERJA
               && x.HR_TAHUN == agree.tahundibayar
               && x.HR_BULAN_DIBAYAR == agree.bulandibayar).ToList();

            det.HR_JANUARI = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 1).Sum(c => c.HR_JUMLAH);
            det.HR_FEBRUARI = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 2).Sum(c => c.HR_JUMLAH);
            det.HR_MAC = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 3).Sum(c => c.HR_JUMLAH);
            det.HR_APRIL = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 4).Sum(c => c.HR_JUMLAH);
            det.HR_MEI = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 5).Sum(c => c.HR_JUMLAH);
            det.HR_JUN = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 6).Sum(c => c.HR_JUMLAH);
            det.HR_JULAI = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 7).Sum(c => c.HR_JUMLAH);
            det.HR_OGOS = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 8).Sum(c => c.HR_JUMLAH);
            det.HR_SEPTEMBER = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 9).Sum(c => c.HR_JUMLAH);
            det.HR_OKTOBER = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 10).Sum(c => c.HR_JUMLAH);
            det.HR_NOVEMBER = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 11).Sum(c => c.HR_JUMLAH);
            det.HR_DISEMBER = elaunlain
                .Where(c => c.HR_BULAN_BEKERJA == 12).Sum(c => c.HR_JUMLAH);
            det.HR_JUMLAH_GAJI = elaunlain.Sum(c => c.HR_JUMLAH);
            int totalBulan = elaunlain.Select(c => c.HR_BULAN_BEKERJA).Distinct().Count();
            if (totalBulan > 0)
            {
                det.HR_GAJI_PURATA = det.HR_JUMLAH_GAJI == null ?
                    0 : decimal.Round(Convert.ToDecimal(det.HR_JUMLAH_GAJI) / totalBulan, 3);
            }
            det.HR_MUKTAMAD = 0;

            return det;
        }
    }
}