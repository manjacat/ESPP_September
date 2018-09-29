using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TraksaksiSambilanModels
    {
		public TraksaksiSambilanModels() { }
		public virtual DbSet<HR_TRANSAKSI_SAMBILAN> HR_TRANSAKSI_SAMBILAN { get; set; }

        //will be used in to Store claculation Related to Transaksi Sambilan

        public static AgreementModels Maklumat(string HR_PEKERJA,
            int? tahunbekerja,
            int? bulanbekerja,
            int? tahundibayar,
            int? bulandibayar,
            string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga,
            decimal? jumlahot, int? jumlahhari)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            int consHariBekerja = PageSejarahModel.ConstHariBekerja; //value is 21

            //Cari Maklumat Pekerjaan
            HR_MAKLUMAT_PEKERJAAN userMaklumat = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();
            List<string> listElaunKa = new List<string>
            {
                "E0064",
                "E0096",
                "E0151",
                "E0105"
            };

            if (userMaklumat != null)
            {
                GE_JABATAN userJabatan = mc.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == userMaklumat.HR_JABATAN).SingleOrDefault();
                GE_BAHAGIAN userBahagian = mc.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == userMaklumat.HR_BAHAGIAN && s.GE_KOD_JABATAN == userMaklumat.HR_JABATAN).SingleOrDefault();

                //dapatkan Transaksi Sambilan Detail. ada banyak kod: C0020, E0064, E0164, E0234, GAJPS, P0015, P0035
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> userTransaksiDetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA 
                    && s.HR_BULAN_BEKERJA == bulanbekerja 
                    && s.HR_BULAN_DIBAYAR == bulandibayar 
                    && s.HR_TAHUN == tahundibayar 
                    && s.HR_TAHUN_BEKERJA == tahunbekerja).ToList();

                HR_MAKLUMAT_PERIBADI userPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA);
                HR_MAKLUMAT_PEKERJAAN userPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA);
                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == userPekerjaan.HR_JAWATAN);
                var gajibonussehari = userPekerjaan.HR_GAJI_POKOK / consHariBekerja;

                HR_MAKLUMAT_PEKERJAAN mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();

                //if (transaksisambilandetail.Count != 0 && transaksisambilan.Count != 0)
                if (userTransaksiDetail.Count != 0)
                {
                    //kalau ada tranaksi, ambik gaji dan potongan dari tranasksi
                    HR_TRANSAKSI_SAMBILAN_DETAIL gaji = userTransaksiDetail.SingleOrDefault(s => s.HR_KOD_IND == "G");

                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunka = userTransaksiDetail
                        .Where(s => listElaunKa.Contains(s.HR_KOD)).ToList();
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunot = userTransaksiDetail
                        .Where(s => s.HR_KOD == "E0164").ToList();
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain = userTransaksiDetail
                        .Where(s => s.HR_KOD_IND == "E" 
                        && (!listElaunKa.Contains(s.HR_KOD) && s.HR_KOD != "E0164")).ToList();

                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> potonganksdk = userTransaksiDetail
                        .Where(s => s.HR_KOD == "P0015").ToList();
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> potonganlain = userTransaksiDetail
                        .Where(s => s.HR_KOD_IND == "P" && (s.HR_KOD != "P0015")).ToList();

                    List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
                    AgreementModels kerjaelaun = new AgreementModels();
                    
                    kerjaelaun.JABATAN = userJabatan.GE_KETERANGAN_JABATAN;
                    kerjaelaun.BAHAGIAN = userBahagian.GE_KETERANGAN;
                    kerjaelaun.NOKP = userPeribadi.HR_NO_KPBARU;
                    kerjaelaun.JAWATAN = jawatan.HR_NAMA_JAWATAN;
                    kerjaelaun.GAJISEHARI = gajibonussehari.Value.ToString("0.00");
                    //gaji bersih = gaji pokok + elaun
                    var gajikasar = gaji.HR_JUMLAH
                        + elaunka.Sum(s => s.HR_JUMLAH)
                        + elaunlain.Sum(s => s.HR_JUMLAH)
                        + elaunot.Sum(s => s.HR_JUMLAH);
                    kerjaelaun.GAJIKASAR = gajikasar.Value.ToString("0.00");
                    double? gajikasar1 = (double)gajikasar * 0.11;
                    kerjaelaun.GAJISEBELUMKWSP = gajikasar1.Value.ToString();
                    kerjaelaun.GAJIPER3 = (gaji.HR_JUMLAH / 3).Value.ToString("0.00");

                    HR_SOCSO userSocso = db.HR_SOCSO.Where(s => s.HR_GAJI_DARI <= gajikasar
                        && gajikasar <= s.HR_GAJI_HINGGA).SingleOrDefault();
                    kerjaelaun.POTONGANSOCSO = userSocso.HR_JUMLAH;
                    //HR_KWSP userKWSP = db.HR_KWSP.Where(s => s.HR_UPAH_DARI <= gajikasar
                    //    && gajikasar <= s.HR_UPAH_HINGGA).SingleOrDefault();
                    //kerjaelaun.POTONGANKWSP = userKWSP.HR_CARUMAN_PEKERJA;

                    HR_MAKLUMAT_ELAUN_POTONGAN userCaruman = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "C").First();
                    kerjaelaun.POTONGANKWSP = userCaruman.HR_JUMLAH;
                    
                    kerjaelaun.JAMBEKERJA = elaunot.SingleOrDefault().HR_JAM_HARI;
                    kerjaelaun.HARIBEKERJA = gaji.HR_JAM_HARI;
                    kerjaelaun.GAJIPOKOK = gaji.HR_JUMLAH;
                    kerjaelaun.ELAUNKA = elaunka.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.JUMLAHBAYARANOT = elaunot.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.ELAUNLAIN = elaunlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");                    
                    kerjaelaun.POTONGANKSDK = potonganksdk.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.POTONGLAIN = potonganlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    
                    kerjaelaun.TUNGGAKANIND = userTransaksiDetail
                        .Where(s => s.HR_KOD == "GAJPS").SingleOrDefault().HR_TUNGGAKAN_IND;
                    
                    //gaji bersih = gaji pokok + elaun - potongan
                    var bersih = kerjaelaun.GAJIPOKOK 
                        + elaunka.Sum(s => s.HR_JUMLAH) 
                        + elaunot.Sum(s => s.HR_JUMLAH) 
                        + elaunlain.Sum(s => s.HR_JUMLAH)
                        - potonganksdk.Sum(s => s.HR_JUMLAH)
                        - potonganlain.Sum(s => s.HR_JUMLAH);
                    kerjaelaun.GAJIBERSIH = decimal.Parse(bersih.Value.ToString("0.00"));
                    kerjaelaun.MUKTAMAD = gaji.HR_MUKTAMAD;

                    return kerjaelaun;
                }
                else if (userTransaksiDetail.Count == 0)
                {
                    //kalau xde transksi, ambik gaji dari maklumat elaun potongan
                    List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();

                    HR_MAKLUMAT_ELAUN_POTONGAN gaji = userMaklumatPotongan
                        .SingleOrDefault(s => s.HR_KOD_ELAUN_POTONGAN == "GAJPS");

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganksdk = userMaklumatPotongan
                        .Where(s => s.HR_KOD_ELAUN_POTONGAN == "P0015").ToList();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganlain = userMaklumatPotongan
                        .Where(s => s.HR_ELAUN_POTONGAN_IND == "P" 
                        && (s.HR_KOD_ELAUN_POTONGAN != "P0015")).ToList();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunka = userMaklumatPotongan
                        .Where(s => listElaunKa.Contains(s.HR_KOD_ELAUN_POTONGAN)).ToList();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunLain = userMaklumatPotongan
                        .Where(s => s.HR_ELAUN_POTONGAN_IND == "E" 
                        && (!listElaunKa.Contains(s.HR_KOD_ELAUN_POTONGAN) && s.HR_KOD_ELAUN_POTONGAN != "E0164"))
                        .ToList();

                    AgreementModels kerjaelaun = new AgreementModels();
                    List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
                
                    kerjaelaun.GAJIPOKOK = mpekerjaan.HR_GAJI_POKOK;
                    var gajisehari = mpekerjaan.HR_GAJI_POKOK / consHariBekerja;
                    var gajiSejamOT = ((gajisehari * jumlahhari) * 12 / 2504);
                    var elaunot = gajiSejamOT * jumlahot;
                    kerjaelaun.JUMLAHBAYARANOT = elaunot.Value.ToString("0.00");

                    decimal? gajiper3 = kerjaelaun.GAJIPOKOK / 3;
                    kerjaelaun.GAJIPER3 = gajiper3.Value.ToString("0.00");
                    kerjaelaun.ELAUNKA = elaunka.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.POTONGANKSDK = potonganksdk.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.ELAUNLAIN = elaunLain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.POTONGLAIN = potonganlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.JABATAN = userJabatan.GE_KETERANGAN_JABATAN;
                    kerjaelaun.BAHAGIAN = userBahagian.GE_KETERANGAN;
                    kerjaelaun.NOKP = userPeribadi.HR_NO_KPBARU;
                    kerjaelaun.JAWATAN = jawatan.HR_NAMA_JAWATAN;
                    kerjaelaun.GAJISEHARI = gajibonussehari.Value.ToString("0.00");

                    HR_MAKLUMAT_ELAUN_POTONGAN userCaruman = userMaklumatPotongan
                      .Where(s => s.HR_ELAUN_POTONGAN_IND == "C").First();
                    kerjaelaun.POTONGANKWSP = userCaruman.HR_JUMLAH;

                    decimal? gajikasar = kerjaelaun.GAJIPOKOK 
                        + elaunka.Sum(s => s.HR_JUMLAH) 
                        + elaunLain.Sum(s => s.HR_JUMLAH) 
                        + elaunot;
                    kerjaelaun.GAJIKASAR = gajikasar.Value.ToString("0.00");
                    HR_SOCSO userSocso = db.HR_SOCSO.Where(s => s.HR_GAJI_DARI <= gajikasar
                       && gajikasar <= s.HR_GAJI_HINGGA).SingleOrDefault();
                    kerjaelaun.POTONGANSOCSO = userSocso.HR_JUMLAH;
                    double? gajikasar1 = (double)gajikasar * 0.11;
                    kerjaelaun.GAJISEBELUMKWSP = gajikasar1.Value.ToString("0.00");
                    //gaji bersih = gaji pokok + elaun - potongan
                    var bersih = kerjaelaun.GAJIPOKOK
                        + elaunka.Sum(s => s.HR_JUMLAH)
                        + elaunLain.Sum(s => s.HR_JUMLAH)
                        + elaunot
                        - potonganksdk.Sum(s => s.HR_JUMLAH)
                        - potonganlain.Sum(s => s.HR_JUMLAH);
                    kerjaelaun.GAJIBERSIH = decimal.Parse(bersih.Value.ToString("0.00"));
                    return kerjaelaun;
                }
            }

            return null;
        }

    }
	public partial class HR_TRANSAKSI_SAMBILAN
	{
		[Key]
		public string HR_NO_PEKERJA { get; set; }
		public int HR_BULAN_DIBAYAR { get; set; }
		public int HR_TAHUN { get; set; }
		public int HR_BULAN_BEKERJA { get; set; }
		public int HR_TAHUN_BEKERJA { get; set; }
	}
}