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
using System.Threading;
using System.Net.Sockets;
using Android.Util;
using EloComandas.Utils;
using EloComandas.Controller;
using EloComandas.Persistence;

namespace EloComandas.Controller
{
	public class AparelhoController
	{
		AparelhoDAO DAO = new AparelhoDAO();
		public bool Insert(Aparelho a) => DAO.Insert(a);
		public bool Update(Aparelho a) => DAO.Update(a);
		public List<Aparelho> FindAll() => DAO.FindAll();
		public Aparelho GetAparelho()
		{
			return FindAll().FirstOrDefault();
		}
		public bool syncDevice(Aparelho aparelho)
		{
			bool result = false;

			Thread t = new Thread(() =>
			{
				TcpClient client = null;
				NetworkStream netStream = null;
				StringBuilder builder = null;

				try
				{
					client = new TcpClient();
					DNS dns = new ConfigController().GetDNS();
					client.Connect("elosoftware.dyndns.org", 8560);
					//client.Connect("192.168.0.78", 8560);

					netStream = client.GetStream();

					builder = new StringBuilder();

					builder.Append("MANUTENIRAPARELHO")
					.Append(aparelho.DSCAPAR)
					.Append(";")
					.Append(aparelho.NOMOPER)
					.Append(";")
					.Append(aparelho.IDTPESS)
					.Append(";")
					.Append(aparelho.NROVERS)
					.Append(";")
					.Append(aparelho.TIPSAPAR)
					.Append(";")
					.Append(aparelho.INDINAT)
					.Append(";")
					.Append(aparelho.DTHULTAT.ToString("dd/MM/yyyy HH:mm:ss"))
					.Append(";")
					.Append(aparelho.USRULTAT);

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
						if (received.ToUpper().Contains("MANUTENIRAPARELHO@@MANUTENIROK"))
						{
							aparelho.INDSYNC = true;
							this.Update(aparelho);
							result = true;
						}
						else if (received.ToUpper().Contains("MANUTENIRAPARELHO@@APARELHO JÁ CADASTRADO"))
						{
							aparelho.INDSYNC = true;
							this.Update(aparelho);
							result = true;
						}
						else
							result = false;
					}

					builder.Clear();

				}
				catch(Exception e)
				{
					Log.Error("SyncError", e.ToString());
					result = false;
				}
				finally
				{
					if (client != null) client.Close();
					if (netStream != null) netStream.Close();
				}
			});
			t.Start();
			t.Join();

			return result;
		}
	}
}