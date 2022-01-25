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
	public class AparelhoDAO
	{
		public AparelhoDAO()
		{
			Database.GetConnection().CreateTable<Aparelho>();
		}
		public bool Insert(Aparelho a)
		{
			var conn = Database.GetConnection();
			if (conn.Insert(a) > 0)
			{
				return true;
			}
			else
				return false;
		}
		public bool Update(Aparelho a)
		{
			var conn = Database.GetConnection();
			if (conn.Update(a) > 0)
				return true;
			else
				return false;
		}
		public List<Aparelho> FindAll()
		{
			var conn = Database.GetConnection();
			return conn.Table<Aparelho>().ToList();
		}
	}

}