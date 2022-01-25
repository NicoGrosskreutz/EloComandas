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
using System.Threading;

namespace EloComandas.Controller
{
	public class ProdutoController
	{
		ProdutoDAO DAO = new ProdutoDAO();
		public bool Save(Produto produto)
		{
			try
			{
				if (produto.CG_PRODUTO_ID == null)
					produto.CG_PRODUTO_ID = GetLastId() == null ? 1 : GetLastId() + 1;

				if (produto.CODPROD == null)
					produto.CODPROD = GetLastId() == null ? 1 : GetLastId() + 1;

				if (produto.CODPROD == 0)
					produto.CODPROD = GetLastId() == null ? 1 : GetLastId() + 1;

				if (FindById(produto.CG_PRODUTO_ID) == null)
					return DAO.Insert(produto);
				else
					return DAO.Update(produto);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
		public long? GetLastId() => DAO.GetLastId();
		public Produto FindById(object id) => DAO.FindById(id);
		/// <summary>
		/// Retorna a data da ultima atualização
		/// </summary>
		/// <returns></returns>
		public DateTime GetLastDateTime()
		{
			DateTime DTHULTAT = FindAll().Max(p => p.DTHULTAT);
			return DTHULTAT;
		}
		public List<Produto> FindAll() => DAO.FindAll();


		public Produto FindByCODPROD(long pCODPROD) => DAO.FindByCODPROD(pCODPROD);

		public IList<Produto> FindByDSCPROD(string pDSCPROD) => DAO.FindByDSCPROD(pDSCPROD);

		public bool ComSocket(string request, string host, int port)
		{
			bool aux = false;

			Thread t = new Thread(() =>
			{

				TcpClient client = null;
				NetworkStream netStream = null;

				try
				{
					client = new TcpClient();
					client.Connect(host, port);

					netStream = client.GetStream();

					byte[] msg = request.ToUTF8(true);
					netStream.Write(msg, 0, msg.Length);

					Empresa empresa = new EmpresaController().GetEmpresa();

					bool loop = true;
					string dados = string.Empty;
					if (netStream.CanRead)
						while (loop)
						{
							byte[] bytes = new byte[client.ReceiveBufferSize];
							netStream.Read(bytes, 0, client.ReceiveBufferSize);
							string receiveMsg = bytes.UTF7ToString();

							if (receiveMsg.Contains("\0\0"))
								receiveMsg = receiveMsg.Split("\0\0")[0];

							receiveMsg = receiveMsg.Replace("CARGAPRODUTO@@", "");

							if (receiveMsg.ToUpper().Contains("@ERRO"))
								throw new Exception(receiveMsg.ToUpper());

							if (receiveMsg.Contains("FIMPRO"))
							{
								dados = dados + receiveMsg;
								loop = false;
							}
							else
								dados = dados + receiveMsg;
						}

					dados = dados.Replace("@@FIMPRO", "");
					dados = dados.Replace("FIMPRO@@", "");

					if (!dados.Contains("@ERRO"))
					{
						string[] lines = dados.Split("@@");
						foreach (var str in lines)
						{
							if (!string.IsNullOrEmpty(str))
							{
								string[] data = str.Split(';');

								Produto p = new Produto()
								{
									CG_PRODUTO_ID = data[0].ToLong(),
									CODEMPRE = data[1],
									CODPROD = data[2].ToLong(),
									DSCPROD = data[3],
									IDTUNID = data[4],
									CG_CLASSE_PRODUTO_ID = data[5].ToLong(),
									QTDUNID = data[6].ToDouble(),
									CODEAN = data[7],
									CODNCM = data[8].ToLong(),
									PRCCUSTO = data[9].ToDouble(),
									PRCVENDA = data[10].ToDouble(),
									PERDSESP = data[11].ToDouble(),
									PERVISTA = data[12].ToDouble(),
									INDINAT = data[13].ToBool(),
									DTHULTAT = DateTime.Parse(data[14]),
									USRULTAT = data[15]
								};

								this.Save(p);
							}
						}
					}
					aux = true;
				}
				catch (Exception ex)
				{
					string error = "";
					Log.Error(error, ex.ToString());
					aux = false;
				}
				finally
				{
					if (client != null) client.Close();
					if (netStream != null) netStream.Close();
				}

			});

			t.Start();
			t.Join();

			return aux;
		}
	}
}