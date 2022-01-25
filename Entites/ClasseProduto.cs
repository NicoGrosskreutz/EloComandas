using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EloComandas.Entites
{
	public class ClasseProduto
	{
		public long? CG_CLASSE_PRODUTO_ID { get; set; } = null;
		public string CODEMPRE { get; set; }
		public long CODCLASS { get; set; }
		public string DSCCLASS { get; set; }
		public DateTime DTHULTAT { get; set; }
		public string USRULTAT { get; set; }
	}
}