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
	public class ClasseProdutoAdicionalDAO
	{
		public ClasseProdutoAdicionalDAO()
		{
			Database.GetConnection().CreateTable<ClasseProdutoAdicional>();
		}

		public bool Insert(ClasseProdutoAdicional produto)
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
		public bool Update(ClasseProdutoAdicional produto)
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
		public long? GetLastId()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ClasseProdutoAdicional>()
					.Max(p => p.CG_CLASSE_PRODUTO_ADICIONAL_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public ClasseProdutoAdicional FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<ClasseProdutoAdicional>(id);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public List<ClasseProdutoAdicional> FindAll()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<ClasseProdutoAdicional>().ToList();
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