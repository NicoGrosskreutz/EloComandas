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
	public class ClasseProdutoAdicional
	{
		public long? CG_CLASSE_PRODUTO_ADICIONAL_ID { get; set; } = null;
		public long CG_CLASSE_PRODUTO_ID { get; set; }
		public DateTime DTHINCLU { get; set; }
		public string USRINCLU { get; set; }
	}
}