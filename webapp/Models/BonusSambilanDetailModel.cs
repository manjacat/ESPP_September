using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class BonusSambilanDetailModel
    {
        public int BulanBonus { get; set; }
        public int TahunBonus { get; set; }
        public string Nama { get; set; }
        public string NoPekerja { get; set; }
        public string NoKadPengenalan { get; set; }
        public string NoAkaunBank { get; set; }
        public string NoKWSP { get; set; }
        public DateTime? TarikhLantikan { get; set; }
        /// <summary>
        /// returns TarikhLantikan in dd-MM-yyyy format
        /// </summary>
        public string TarikhLantikanString
        {
            get
            {
                if (TarikhLantikan != null)
                {
                    return string.Format("{0:dd-MM-yyyy}", TarikhLantikan);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public decimal? Jan { get; set; }
        public decimal? Feb { get; set; }
        public decimal? Mac { get; set; }
        public decimal? April { get; set; }
        public decimal? Mei { get; set; }
        public decimal? Jun { get; set; }
        public decimal? Julai { get; set; }
        public decimal? Ogos { get; set; }
        public decimal? September { get; set; }
        public decimal? Oktober { get; set; }
        public decimal? November { get; set; }
        public decimal? Disember { get; set; }
        public decimal? JumlahGaji { get; set; }
        public decimal? GajiPurata { get; set; }
        public decimal? BonusDiterima { get; set; }
        public string Catatan { get; set; }
        public bool IsMuktamad { get; set; }

        //new column sebab nak tengok bulan start dgn bulan end
        //berguna utk kira purata dan nak tengok bulan mana kita nak display kat report
        //hide semua bulan yang di luar dari range MinBulan - MaxBulan
        public int MinBulan { get; set; } 
        public int MaxBulan { get; set; }

        public static List<BonusSambilanDetailModel> GetBonusSambilanDetailData(int month, int year)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<HR_BONUS_SAMBILAN_DETAIL> tableList = HR_BONUS_SAMBILAN_DETAIL.GetBonusSambilanDetailData(month, year);

            List<BonusSambilanDetailModel> outputList = new List<BonusSambilanDetailModel>();
            foreach (HR_BONUS_SAMBILAN_DETAIL y in tableList)
            {
                BonusSambilanDetailModel d = new BonusSambilanDetailModel();
                HR_MAKLUMAT_PERIBADI maklumat = db.HR_MAKLUMAT_PERIBADI
                    .Where(m => m.HR_NO_PEKERJA == y.HR_NO_PEKERJA).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN kerja = db.HR_MAKLUMAT_PEKERJAAN
                    .Where(m => m.HR_NO_PEKERJA == y.HR_NO_PEKERJA).FirstOrDefault();
                d.BulanBonus = y.HR_BULAN_BONUS;
                d.TahunBonus = y.HR_TAHUN_BONUS;
                d.Nama = maklumat.HR_NAMA_PEKERJA;
                d.NoPekerja = y.HR_NO_PEKERJA;
                d.NoKadPengenalan = y.HR_NO_KPBARU;
                d.NoAkaunBank = kerja.HR_NO_AKAUN_BANK;
                d.NoKWSP = kerja.HR_NO_KWSP;
                d.TarikhLantikan = kerja.HR_TARIKH_LANTIKAN;
                d.Jan = y.HR_JANUARI;
                d.Feb = y.HR_FEBRUARI;
                d.Mac = y.HR_MAC;
                d.April = y.HR_APRIL;
                d.Mei = y.HR_MEI;
                d.Jun = y.HR_JUN;
                d.Julai = y.HR_JULAI;
                d.Ogos = y.HR_OGOS;
                d.September = y.HR_SEPTEMBER;
                d.Oktober = y.HR_OKTOBER;
                d.November = y.HR_NOVEMBER;
                d.Disember = y.HR_DISEMBER;
                d.JumlahGaji = y.HR_JUMLAH_GAJI;
                d.GajiPurata = y.HR_GAJI_PURATA;
                d.BonusDiterima = y.HR_BONUS_DITERIMA;
                d.Catatan = y.HR_CATATAN;
                d.MinBulan = y.HR_BULAN_START;
                d.MaxBulan = y.HR_BULAN_END;
                if (y.HR_MUKTAMAD > 0)
                {
                    d.IsMuktamad = true;
                }
                else
                {
                    d.IsMuktamad = false;
                }
                outputList.Add(d);
            }
            //sort by nama
            outputList = outputList.OrderBy(x => x.Nama).ToList();

            return outputList;
        }

        /// <summary>
        /// Output BONUS_SAMBILAN_DETAIL data from HR_TRANSAKSI_SAMBILAN_DETAIL
        /// </summary>
        /// <param name="startMonth">start of month (if not Jan)</param>
        /// <param name="month">bulan bonus dibayar</param>
        /// <param name="year">tahun bonus dibayar</param>
        /// <returns>List BonusSambilanDetailModel</returns>
        public static List<BonusSambilanDetailModel> GetDetailsFromTransaksi
            (int startMonth, int month, int year, int endMonth)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<string> listHR_PEKERJA = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_BULAN_DIBAYAR == month
                && s.HR_BULAN_BEKERJA >= startMonth
                && s.HR_BULAN_BEKERJA <= endMonth
                && s.HR_TAHUN == year).Select(s => s.HR_NO_PEKERJA).Distinct().ToList();

            List<BonusSambilanDetailModel> outputList = new List<BonusSambilanDetailModel>();
            foreach (string hr_pekerja in listHR_PEKERJA)
            {
                BonusSambilanDetailModel det = new BonusSambilanDetailModel();
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain =
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.
                    Where(x => x.HR_NO_PEKERJA == hr_pekerja
                    && x.HR_TAHUN == year
                    && x.HR_BULAN_BEKERJA >= startMonth
                    && x.HR_BULAN_BEKERJA <= endMonth
                    && x.HR_BULAN_DIBAYAR == month).ToList();

                HR_MAKLUMAT_PERIBADI maklumat = db.HR_MAKLUMAT_PERIBADI
                    .Where(m => m.HR_NO_PEKERJA == hr_pekerja).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN kerja = db.HR_MAKLUMAT_PEKERJAAN
                    .Where(m => m.HR_NO_PEKERJA == hr_pekerja).FirstOrDefault();
                det.BulanBonus = month;
                det.TahunBonus = year;
                det.Nama = maklumat.HR_NAMA_PEKERJA;
                det.NoPekerja = hr_pekerja;
                det.NoKadPengenalan = maklumat.HR_NO_KPBARU;
                det.NoAkaunBank = kerja.HR_NO_AKAUN_BANK;
                det.NoKWSP = kerja.HR_NO_KWSP;
                det.TarikhLantikan = kerja.HR_TARIKH_LANTIKAN;
                
                det.Jan = elaunlain
                        .Where(c => c.HR_BULAN_BEKERJA == 1).Sum(c => c.HR_JUMLAH);
                det.Feb = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 2).Sum(c => c.HR_JUMLAH);
                det.Mac = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 3).Sum(c => c.HR_JUMLAH);
                det.April = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 4).Sum(c => c.HR_JUMLAH);
                det.Mei = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 5).Sum(c => c.HR_JUMLAH);
                det.Jun = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 6).Sum(c => c.HR_JUMLAH);
                det.Julai = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 7).Sum(c => c.HR_JUMLAH);
                det.Ogos = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 8).Sum(c => c.HR_JUMLAH);
                det.September = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 9).Sum(c => c.HR_JUMLAH);
                det.Oktober = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 10).Sum(c => c.HR_JUMLAH);
                det.November = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 11).Sum(c => c.HR_JUMLAH);
                det.Disember = elaunlain
                    .Where(c => c.HR_BULAN_BEKERJA == 12).Sum(c => c.HR_JUMLAH);
                det.JumlahGaji = elaunlain.Sum(c => c.HR_JUMLAH);
                det.MinBulan = startMonth;
                det.MaxBulan = endMonth;
                int totalBulan = endMonth - startMonth + 1;
                if (totalBulan > 0)
                {
                    det.GajiPurata = det.JumlahGaji == null ?
                        0 : decimal.Round(Convert.ToDecimal(det.JumlahGaji) / totalBulan, 3);
                }
                det.IsMuktamad = false;
                outputList.Add(det);
            }
            //sort by Nama
            outputList = outputList.OrderBy(x => x.Nama).ToList();
            return outputList;
        }
    }
}