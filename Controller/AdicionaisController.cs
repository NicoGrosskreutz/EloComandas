using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using EloComandas.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EloComandas.Persistence;

namespace EloComandas.Controller
{
	public class AdicionaisController
	{
        AdicionaisDAO DAO = new AdicionaisDAO();
		public AdicionaisController()
		{
			Database.GetConnection().CreateTable<Adicionais>();
            
		}
        public bool Save(Adicionais i)
        {
            var conn = Database.GetConnection();
            try
            {
                if (i.FT_ITEM_ADD_ID == null)
                    i.FT_ITEM_ADD_ID = GetLastId() == null ? 1 : GetLastId() + 1;

                if (FindById(i.FT_ITEM_ADD_ID) == null)
                    return DAO.Insert(i);
                else
                    return DAO.Update(i);
            }
            catch (Exception ex)
            {
                string error = "";
                Log.Error(error, ex.ToString());
                return false;
            }
        }
        public long? GetLastId() => DAO.GetLastId();
        public Adicionais FindById(object id) => DAO.FindById(id);
        public List<Adicionais> FindItensByFT_PEDIDO_ITEM_ID(long id) => DAO.FindItensByFT_PEDIDO_ITEM_ID(id);
    }
}