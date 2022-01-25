using Android.App;
using Android.Views;
using Android.Widget;
using EloComandas.Entites;
using System;
using System.Collections.Generic;


namespace EloComandas.Adapter
{
	public class ProdutosAdapter : BaseAdapter<ProdAdapterTB>
	{
		private Activity context;
		private List<ProdAdapterTB> list;

		public ProdutosAdapter(Activity _context, List<ProdAdapterTB> _list)
		{
			context = _context;
			list = _list;
		}

		public override ProdAdapterTB this[int position] => list[position];

		public override int Count => list == null ? 0 : list.Count;

		public override long GetItemId(int position)
		{
			return list[position].ID_PROD;
		}

		public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.adapterproduto, parent, false);

			var nomeprod = view.FindViewById<TextView>(Resource.Id.lblNOMEPROD);
			var preco = view.FindViewById<TextView>(Resource.Id.lblVALOR);

			nomeprod.Text = list[position].NOMEPROD;
			//preco.Text = list[position].VLRPREC;

			double vlr = double.Parse(list[position].VLRPREC);

			preco.Text = vlr.ToString("C");


			return view;
		}


	}
}