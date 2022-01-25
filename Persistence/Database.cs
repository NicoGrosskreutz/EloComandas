using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using EloComandas.Entites;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EloComandas.Persistence
{
    public class Database
    {
        private static SQLiteConnection conn = null;

        public static void Drop()
        {
            var conn = GetConnection();

            conn.DropTable<ClasseProduto>();
            conn.DropTable<ClasseProdutoAdicional>();
            conn.DropTable<Entites.Config>();
            conn.DropTable<DNS>();
            conn.DropTable<Empresa>();
            conn.DropTable<ItemPedido>();
            conn.DropTable<Adicionais>();
            conn.DropTable<Operador>();
            conn.DropTable<Pedido>();
            conn.DropTable<Produto>();
            conn.DropTable<Vendedor>();
        }
        public static SQLiteConnection GetConnection()
        {
            try
            {
                if (conn == null)
                {
                    string dbPath = Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Dados.db3");

                    conn = new SQLiteConnection(dbPath);
                    conn.Execute("PRAGMA encoding = 'UTF-8'");

                    //var folder = new LocalRootFolder();
                    //               var file = folder.CreateFile("DATABASE", PCLExt.FileStorage.CreationCollisionOption.OpenIfExists);
                    //               conn = new SQLiteConnection(file.Path);
                    //               conn.Execute("PRAGMA encoding = 'UTF-8'");
                }

                return conn;
            }
            catch (Exception ex)
            {
                Log.Error("error", ex.ToString());
                return GetConnection();
            }
        }

        public static void RunInTransaction(Action action)
            => GetConnection().RunInTransaction(() => action());
    }
}