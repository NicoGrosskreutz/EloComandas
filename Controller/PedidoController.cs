using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using EloComandas.Entites;
using EloComandas.Persistence;
using EloComandas.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EloComandas.Controller
{
	public class PedidoController
	{
		PedidoDAO DAO = new PedidoDAO();
		public bool Save(Pedido p)
		{
			try
			{
				if (p.FT_PEDIDO_ID == null)
					p.FT_PEDIDO_ID = GetLastId() == null ? 1 : GetLastId() + 1;

				if (FindById(p.FT_PEDIDO_ID) == null)
					return DAO.Insert(p);
				else
					return DAO.Update(p);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public long? GetLastId() => DAO.GetLastId();
		public Pedido FindById(object id) => DAO.FindById(id);
		public List<Pedido> FindAll() => DAO.FindAll();
		public Pedido FindLast() => DAO.FindLast();

		/// <summary>
		///  Transmite o pedido para o servidor
		/// </summary>
		/// <param name="pedido"></param>
		/// <returns></returns>
		public bool ComSocket(Pedido pedido, out string message)
		{
			string outStr = string.Empty;

			DNS dns = new ConfigController().GetDNS();

			TcpClient client = null;
			NetworkStream netStream = null;
			StringBuilder builder = null;

			bool result = false;

			try
			{
				Task.Run(() =>
				{
					client = new TcpClient(dns.Host, dns.Port);

					netStream = client.GetStream();

					builder = new StringBuilder();

					/* Parte responsável por organizar a string a ser enviada (pedido) */
					builder.Append("GERARCOMANDA")
					.Append(pedido.CODEMPRE)
					.Append("@@")
					.Append(pedido.DATEMISS.ToString("dd/MM/yyyy"))
					.Append(";")
					.Append(pedido.NROMESA)
					.Append(";")
					.Append(pedido.NOMBALC)
					.Append(";")
					.Append(pedido.NROFONE)
					.Append(";")
					.Append(pedido.CG_VENDEDOR_ID)
					.Append(";")
					.Append(pedido.OBSPEDID)
					.Append(";")
					.Append(pedido.DTHULTAT)
					.Append(";")
					.Append(pedido.USRULTAT)
					.Append("@@");

					var items = new ItemPedidoController().FindItemsByFT_PEDIDO_ID(pedido.FT_PEDIDO_ID.Value);

					/* Parte responsável por organizar a string a ser enviada (itens do pedido) */
					items.ForEach((aux) =>
					{
						builder.Append(aux.CODPROD)
						.Append(";")
						.Append(aux.DSCROBS)
						.Append(";")
						.Append(aux.QTDPROD)
						.Append(";")
						.Append(aux.VLRUNIT)
						.Append("#");

						if (aux.INDADIC == true)
						{
							var itemsAdicionais = new AdicionaisController().FindItensByFT_PEDIDO_ITEM_ID(aux.FT_PEDIDO_ITEM_ID.Value);

							/* Parte responsável por organizar a string a ser enviada (itens adicionais do item) */
							itemsAdicionais.ForEach((aux) =>
							{
								builder.Append(aux.CODPROD)
								.Append(";")
								.Append(aux.DSCOBS)
								.Append(";")
								.Append(aux.QTDPROD)
								.Append(";")
								.Append(aux.VLRUNIT)
								.Append("#");
							});
						}
					});



					if (builder.ToString().EndsWith("#"))
					{
						string temp = builder.ToString().Substring(0, builder.Length - 1);
						builder.Clear();
						builder.Append(temp);
						builder.Append("@@FIM@@");
					}

					// Envia os bytes para o servidor
					byte[] msg = builder.ToString().ToUTF8(true);
					netStream.Write(msg, 0, msg.Length);

					if (netStream.CanRead)
					{
						byte[] bytes = new byte[client.ReceiveBufferSize];
						netStream.Read(bytes, 0, client.ReceiveBufferSize);
						string received = bytes.UTF7ToString();

						if (received.Contains("\0\0"))
							received = received.Split("\0\0")[0];

						/* Caso o pedido retorne Ok */
						if (received.ToLower().Contains("comandaok"))
						{
							this.SetSync(pedido.FT_PEDIDO_ID.Value);
							result = true;
						}
						/* Caso erro */
						else if (received.ToLower().Contains("comandaerro"))
						{
							outStr = pedido.MSGPEDID = received.Split("@@")[1];

							result = false;
						}
						else
							result = false;
					}

					builder.Clear();

				}).Wait();

				message = outStr;
				return result;
			}
			catch (Exception ex)
			{
				Log.Error("erro", ex.ToString());
				message = ex.Message;
				return false;
			}
			finally
			{
				if (client != null) client.Close();
				if (netStream != null) netStream.Close();
			}
		}

		public bool SetSync(long id)
		{
			try
			{
				Pedido p = this.FindById(id);
				p.INDSINC = true;
				return this.Save(p);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}

	}
}