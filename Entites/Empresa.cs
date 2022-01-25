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
    [Table("TB_EMPRESA")]
    public class Empresa
    {

        [PrimaryKey, Column("CODEMPRE"), MaxLength(2)]
        public string CODEMPRE { get; set; }
        public string NOMRZSOC { get; set; }
        public string NOMFANTA { get; set; }
        public string NROFONE { get; set; }
        public string DSCENDER { get; set; }
        public int NROENDER { get; set; }
        public string CPLENDER { get; set; }
        public string NOMBAIRR { get; set; }
        public long CODMUNIC { get; set; }
        public long NROCEP { get; set; }
        public string NROCNPJ { get; set; }
        public string NROINEST { get; set; }
        public DateTime DTHULTAT { get; set; }
        public string USRULTAT { get; set; }
        public string DSCEMAIL { get; set; }
        public long CG_VENDEDOR_ID { get; set; }
        public string SNHEMAIL { get; set;}
        public string NOMSSMTP { get; set; }
        public int NROPORTA { get; set; }
        public bool INDAUTSV { get; set; }
        public bool INDSSLSV { get; set; }
        public bool INDTLSSV { get; set; }
        public string EMLPRINC { get; set; }
    }
}