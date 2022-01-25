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
	public class EmpresaDAO
	{
		public EmpresaDAO()
		{
			Database.GetConnection().CreateTable<Empresa>();
		}
		public Empresa GetEmpresa()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Empresa>().FirstOrDefault();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public bool Insert(Empresa t)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(t);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public bool Update(Empresa t)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(t);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}


		public Empresa FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<Empresa>(id);
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