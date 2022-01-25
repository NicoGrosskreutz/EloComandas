using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using EloComandas.Entites;
using EloComandas.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EloComandas.Controller
{
	public class ItemPedidoController
	{
        ItemPedidoDAO DAO = new ItemPedidoDAO();
        public bool Save(ItemPedido i)
        {
            var conn = Database.GetConnection();
            try
            {
                if (i.FT_PEDIDO_ITEM_ID == null)
                    i.FT_PEDIDO_ITEM_ID = GetLastId() == null ? 1 : GetLastId().Value + 1;

                if (FindById(i.FT_PEDIDO_ITEM_ID) == null)
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

        public List<ItemPedido> FindAll() => DAO.FindAll();
        public ItemPedido FindById(object id) => DAO.FindById(id);

        public List<ItemPedido> FindItemsByFT_PEDIDO_ID(long id) => DAO.FindItemsByFT_PEDIDO_ID(id);
    }
}