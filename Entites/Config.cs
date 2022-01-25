using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace EloComandas.Entites
{
    [Table("CONFIG")]
    public class Config
    {
        [PrimaryKey, AutoIncrement, Column("CONFIG_ID")]
        public long? CONFIG_ID { get; set; } = null;
        public string DNSINT { get; set; }
        public string DNSEXT { get; set; }
        public bool INDSINC { get; set; }
        /// <summary>
        ///  True - DNS Externo / False - DNS Interno
        /// </summary>
        public bool INDDNS { get; set; }
        public string DTHSINC { get; set; }
        public long VERSAODB { get; set; }
        public DateTime? DTHULTVER { get; set; } = null;
		public bool isAuthorized { get; set; }
		public string DSCRERRO { get; set; }
	}
}