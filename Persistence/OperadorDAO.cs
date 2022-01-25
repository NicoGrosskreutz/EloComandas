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
	public class OperadorDAO
	{
		public OperadorDAO()
		{
			Database.GetConnection().CreateTable<Operador>();
		}

		public bool Insert(Operador operador)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(operador);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public bool Update(Operador operador)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(operador);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public Operador FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<Operador>(id);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public Operador GetOperador()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Operador>()
					.FirstOrDefault();
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