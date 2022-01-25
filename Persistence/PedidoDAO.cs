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
using EloComandas.Entites;
using EloComandas.Controller;
using Android.Util;

namespace EloComandas.Persistence
{
	public class PedidoDAO
	{
		public PedidoDAO()
		{
			Database.GetConnection().CreateTable<Pedido>();
		}

		public bool Insert(Pedido p)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(p);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}

		public bool Update(Pedido p)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(p);
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
				return conn.Table<Pedido>()
					.Max(p => p.FT_PEDIDO_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public Pedido FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<Pedido>(id);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public List<Pedido> FindAll()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Pedido>().ToList();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public Pedido FindLast()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Pedido>().LastOrDefault();
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