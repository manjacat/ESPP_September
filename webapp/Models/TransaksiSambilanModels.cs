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