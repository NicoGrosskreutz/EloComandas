using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using EloComandas.Controller;
using EloComandas.Entites;
using EloComandas.Persistence;
using EloComandas.Utils;

namespace EloComandas.Controller
{
	public class VendedorController
	{
		VendedorDAO DAO = new VendedorDAO();
		public bool Save(Vendedor v)
		{
			try
			{
				if (v.CG_VENDEDOR_ID == null)
					v.CG_VENDEDOR_ID = GetLastId() == null ? 1 : GetLastId() + 1;

				if (FindById(v.CG_VENDEDOR_ID) == null)
					return DAO.Save(v);
				else if (FindById(v.CG_VENDEDOR_ID) != null)
					return DAO.Update(v);

				return true;
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}

		public Vendedor GetVendedor() => DAO.GetVendedor();
		public long? GetLastId() => DAO.GetLastId();
		public Vendedor FindById(object id) => DAO.FindById(id);


		public bool ComSocket(string request, string host, int port)
		{
			TcpClient client = null;
			NetworkStream netStream = null;

			bool result = true;

			try
			{
				client = new TcpClient();
				client.Connect(host, port);
				netStream = client.GetStream();

				byte[] msg = request.ToUTF8(true);
				netStream.Write(msg, 0, msg.Length);

				string dados = string.Empty;

				bool loop = true;
				while (loop)
					if (netStream.CanRead)
					{
						byte[] bytes = new byte[client.ReceiveBufferSize];

						netStream.Read(bytes, 0, bytes.Length);
						string receiveMsg = bytes.UTF7ToString();

						if (receiveMsg.Contains("\0\0"))
							receiveMsg = receiveMsg.Split("\0\0")[0];

						if (!receiveMsg.EndsWith("FIMVEN"))
							dados = dados + receiveMsg.Replace("CARGAVENDEDOR@@", "");
						else
						{
							dados = dados + receiveMsg.Replace("CARGAVENDEDOR@@", "");
							loop = false;
						}
					}

				dados = dados.Replace("@@FIMVEN", "");
				string[] lines = dados.Split("#");
				if (lines.Length > 0)
				{
					lines.ToList().ForEach((str) =>
					{
						if (!str.Contains("FIMVEN"))
						{
							string[] data = str.Split(';');

							Vendedor v = new Vendedor()
							{
								CG_VENDEDOR_ID = data[0].ToLong(),
								CODEMPRE = data[1],
								CODVEND = data[2].ToLong(),
								NOMVEND = data[3],
								USROPER = data[4],
								ES_ESTOQUE_LOCAL_ID = data[5].ToLong(),
								NROTLFN = data[6],
								DTHULTAT = DateTime.Parse(data[7]),
								USRULTAT = data[8]
							};

							this.Save(v);
						}
					});
				}

				return result;
			}
			catch (Exception ex)
			{
				Log.Error("LOG_COMANDAS", ex.ToString()); ;
				return false;
			}
			finally
			{
				if (client != null) client.Close();
				if (netStream != null) netStream.Close();
			}
		}

		private void GetError(string msg)
		{
			string error = "";
			Log.Error(error, msg);
		}
	}
}