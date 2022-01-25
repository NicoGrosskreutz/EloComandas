using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EloComandas.Controller;
using EloComandas.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EloComandas.Adapter
{
	public class ComandaAdapter : BaseAdapter<ItemPedido>
	{
		private Activity context;
		private List<ItemPedido> list;

		public ComandaAdapter(Activity _context, List<ItemPedido> _list)
		{
			context = _context;
			list = _list;
		}

		public override ItemPedido this[int position] => list[position];

		public override int Count => list == null ? 0 : list.Count;

		public override long GetItemId(int position)
		{
			long aux;
			if (list[position].FT_PEDIDO_ITEM_ID == null)
				aux = 0;
			else
				aux = list[position].FT_PEDIDO_ITEM_ID.Value;

			return aux;
		}


		public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.adaptercomandas, parent, false);

			var qtd = view.FindViewById<TextView>(Resource.Id.lblQTD);
			var nomeprod = view.FindViewById<TextView>(Resource.Id.lblNOMEPROD);
			var preco = view.FindViewById<TextView>(Resource.Id.lblPRECO);
			var obs = view.FindViewById<TextView>(Resource.Id.lblOBSERV);

			var itemAdd = new ProdutoController().FindByDSCPROD(list[position].NOMPROD).FirstOrDefault();
			var classe = new ClassesController().FindByID_CLASSE_PROD(itemAdd.CG_CLASSE_PRODUTO_ID.Value);

			if (!classe.DSCCLASS.StartsWith("ADICIONAL") && !classe.DSCCLASS.StartsWith("OPCIONAL"))
			{
				qtd.Text = list[position].QTDPROD.ToString();
				nomeprod.Text = list[position].NOMPROD;
				preco.Text = list[position].VLRTOTAL.ToString("C");
				if (list[position].DSCROBS != null)
				{
					obs.Text = $"*  {list[position].DSCROBS}";
					obs.Visibility = ViewStates.Visible;
				}
				else
				{
					obs.Visibility = ViewStates.Invisible;
				}
			}
			else if (classe.DSCCLASS.StartsWith("ADICIONAL"))
			{
				qtd.Text = list[position].QTDPROD.ToString();
				nomeprod.Text = $"    + {list[position].NOMPROD}";
				preco.Text = list[position].VLRTOTAL.ToString("C");
			}
			else if (classe.DSCCLASS.StartsWith("OPCIONAL"))
			{
				qtd.Text = list[position].QTDPROD.ToString();
				nomeprod.Text = $"    + {list[position].NOMPROD}"+"*";
			}

			return view;
		}
	}
}