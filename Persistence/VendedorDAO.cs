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
	public class VendedorDAO
	{
		public VendedorDAO()
		{
			Database.GetConnection().CreateTable<Vendedor>();
		}

		public bool Save(Vendedor v)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(v);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public bool Update(Vendedor v)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(v);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}

		public Vendedor GetVendedor()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Vendedor>().FirstOrDefault();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public long? GetLastId()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Vendedor>()
					.Max(p => p.CG_VENDEDOR_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public Vendedor FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<Vendedor>(id);
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