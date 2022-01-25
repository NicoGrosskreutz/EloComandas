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
using SQLite;
using EloComandas.Controller;
using EloComandas.Entites;
using Android.Util;

namespace EloComandas.Persistence
{
	public class ItemPedidoDAO
	{
		public ItemPedidoDAO()
		{
			Database.GetConnection().CreateTable<ItemPedido>();
		}

		public bool Insert(ItemPedido i)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(i);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public bool Update(ItemPedido i)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(i);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public long? GetLastId()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ItemPedido>()
					.Max(i => i.FT_PEDIDO_ITEM_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}

		public List<ItemPedido> FindAll()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ItemPedido>().ToList();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public ItemPedido FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<ItemPedido>(id);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}

		public List<ItemPedido> FindItemsByFT_PEDIDO_ID(long id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ItemPedido>()
					.Where(i => i.FT_PEDIDO_ID == id)
					.ToList();

			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
	}
}