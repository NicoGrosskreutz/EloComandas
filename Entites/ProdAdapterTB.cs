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
	public class ProdAdapterTB
	{
		public long ID_PROD { get; set; }
		public string NOMEPROD { get; set; }
		public string VLRPREC { get; set; }
		public int POSITION { get; set; }

		public ProdAdapterTB()
		{
		}

		public ProdAdapterTB(string nOMEPROD, string vLRPREC, int pOSITION)
		{
			NOMEPROD = nOMEPROD;
			VLRPREC = vLRPREC;
			POSITION = pOSITION;
		}
	}
}