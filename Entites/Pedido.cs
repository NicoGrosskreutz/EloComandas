using Android.App;
using Android.Util;
using SQLite;
using System;
using System.Collections.Generic;

namespace EloComandas.Entites
{
    [Table("FT_PEDIDO")]
    public class Pedido
    {
        [PrimaryKey, Column("FT_PEDIDO_ID")]
        public long? FT_PEDIDO_ID { get; set; }
        public string CODEMPRE { get; set; }
        public string NROMESA { get; set; }
        public string NOMBALC { get; set; }
        public string NROFONE { get; set; }
        public DateTime DATEMISS { get; set; }
        public long? CG_VENDEDOR_ID { get; set; }
        public double VLRTOTPED { get; set; }
		public string OBSPEDID { get; set; }
		public bool INDSINC { get; set; }
        public string USRULTAT { get; set; }
        public DateTime DTHULTAT { get; set; }


        /// <summary>
        ///  Auxíliar da classe para armazenar retorno do socket
        /// </summary>
        public string MSGPEDID { get; set; }

        public Pedido()
		{
		}
	}
}