using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PemohonanLuarNegaraDetailsModel
    {
        public virtual DbSet<HR_SEMINAR_LUAR_DETAIL> HR_SEMINAR_LUAR_DETAIL { get; set; }
    }
    public class HR_SEMINAR_LUAR_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_LAWATAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KERAP_IND { get; set; }
        public string HR_LAPORAN_IND { get; set; }

        public virtual HR_SEMINAR_LUAR HR_SEMINAR_LUAR { get; set; }
    }
    public class CARI_NOPEKERJA
    {
        public HR_SEMINAR_LUAR_DETAIL HR_SEMINAR_LUAR_DETAIL { get; set; }
        public string HR_MESEJ { get; set; }
    }

}