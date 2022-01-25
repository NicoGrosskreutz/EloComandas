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
	public class AdicionaisDAO
	{
		public AdicionaisDAO()
		{
			Database.GetConnection().CreateTable<Adicionais>();
		}

		public bool Insert(Adicionais i)
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
		public bool Update(Adicionais i)
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
				return conn.Table<Adicionais>()
					.Max(i => i.FT_ITEM_ADD_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public Adicionais FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				Adicionais p = conn.Find<Adicionais>(id);
				return p;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}

		public List<Adicionais> FindItensByFT_PEDIDO_ITEM_ID(long id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Adicionais>()
					.Where(i => i.FT_PEDIDO_ITEM_ID == id)
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