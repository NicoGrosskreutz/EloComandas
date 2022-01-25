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
	public class ProdutoDAO
	{
		public ProdutoDAO()
		{
			Database.GetConnection().CreateTable<Produto>();
		}

		public bool Insert(Produto produto)
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
		public bool Update(Produto produto)
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
				return conn.Table<Produto>()
					.Max(p => p.CG_PRODUTO_ID);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}
		public Produto FindById(object id)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Find<Produto>(id);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}

		public List<Produto> FindAll()
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Produto>()
					.OrderBy(p => p.DSCPROD).ToList();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}


		public Produto FindByCODPROD(long pCODPROD)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Produto>().ToList()
					.Where(p => p.CODPROD == pCODPROD)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return null;
			}
		}

		public IList<Produto> FindByDSCPROD(string pDSCPROD)
		{
			var conn = Database.GetConnection();
			try
			{
				return conn.Table<Produto>().ToList()
					.Where(p => p.DSCPROD.ToLower().StartsWith(pDSCPROD.ToLower()))
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