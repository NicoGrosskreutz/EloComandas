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
	public class ClasseProdutoDAO
	{
		public ClasseProdutoDAO()
		{
			Database.GetConnection().CreateTable<ClasseProduto>();
		}

		public bool Insert(ClasseProduto produto)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Insert(produto);
				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}

		public bool Update(ClasseProduto produto)
		{
			var conn = Database.GetConnection();
			try
			{
				conn.Update(produto);
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
				return conn.Table<ClasseProduto>()
					.Max(p => p.CG_CLASSE_PRODUTO_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public ClasseProduto FindById(long id)
		{
			var conn = Database.GetConnection();
			try
			{
				var classes = conn.Table<ClasseProduto>().Where(p => p.CG_CLASSE_PRODUTO_ID == id).FirstOrDefault();
				//var classes = conn.Find<ClasseProduto>(id);
				return classes;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public ClasseProduto FindByID_CLASSE_PROD(long id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ClasseProduto>()
					.Where(p => p.CG_CLASSE_PRODUTO_ID == id).LastOrDefault();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}

		public List<ClasseProduto> FindAll()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ClasseProduto>().ToList();
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