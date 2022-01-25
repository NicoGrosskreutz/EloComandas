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
using EloComandas.Entites;
using EloComandas.Controller;
using Android.Util;

namespace EloComandas.Persistence
{
	public class ConfigDAO
	{
		public ConfigDAO()
		{
			Database.GetConnection().CreateTable<Entites.Config>();
		}

		public List<Entites.Config> FindAll()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Entites.Config>().ToList();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public bool Insert(Entites.Config c)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(c);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}

		public bool Update(Entites.Config c)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(c);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public Entites.Config GetConfig()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Entites.Config>().FirstOrDefault();
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