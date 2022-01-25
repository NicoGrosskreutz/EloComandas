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
    [Table("CG_PRODUTO")]
    public class Produto
    {
        [PrimaryKey, Column("CG_PRODUTO_ID")]
        public long? CG_PRODUTO_ID { get; set; } = null;
        public string CODEMPRE { get; set; }
        public long? CODPROD { get; set; }
        public string DSCPROD { get; set; }
        public string IDTUNID { get; set; }
        public long? CG_CLASSE_PRODUTO_ID { get; set; }
        public double QTDUNID { get; set; }
        public string CODEAN { get; set; }
        public long? CODNCM { get; set; }
        public double PRCCUSTO { get; set; }
        public double PRCVENDA { get; set; }
        public double PERDSESP { get; set; }
        public double PERVISTA { get; set; }
        public bool INDINAT { get; set; }
        public DateTime DTHULTAT { get; set; }
        public string USRULTAT { get; set; }
       
    }
}