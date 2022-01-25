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
    [Table("FT_PEDIDO_ITEM")]
    public class ItemPedido
    {
        [PrimaryKey, AutoIncrement, Column("FT_PEDIDO_ITEM_ID")]
        public long? FT_PEDIDO_ITEM_ID { get; set; } = null;
        public long? FT_PEDIDO_ID { get; set; }
        public string CODPROD { get; set; }
        public string NOMPROD { get; set; }
        public double QTDUNID { get; set; }
        public double QTDPROD { get; set; }
        public double VLRUNIT { get; set; }
        public double VLRTOTAL { get; set; }
		public long SITPEDID { get; set; }
		public string DSCROBS { get; set; }
		public string USRULTAT { get; set; }
        public string DTHULTAT { get; set; }
		public bool INDADIC { get; set; }

		public ItemPedido()
		{
		}
	}

}