﻿using Android.App;
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
	public class EmpresaController
	{
        EmpresaDAO DAO = new EmpresaDAO();
		public Empresa Empresa { get { return this.GetEmpresa(); } }
        public Empresa GetEmpresa() => DAO.GetEmpresa();
		public bool Save(Empresa t)
		{
			var conn = Database.GetConnection();
			try
			{
				if (null == FindById(t.CODEMPRE))
					return DAO.Insert(t);
				else
					return DAO.Update(t);
			}
			catch (Exception ex)
			{
				string error = "";
				Log.Error(error, ex.ToString());
				return false;
			}
		}
        public Empresa FindById(object id) => DAO.FindById(id);

        public bool ComSocket(string request, string host, int port)
        {
            bool aux = false;

            TcpClient client = null;
            NetworkStream netStream = null;

            Thread t = new Thread(() =>
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(host, port);

                    byte[] msg = request.ToUTF8(true);

                    netStream = client.GetStream();
                    netStream.Write(msg, 0, msg.Length);

                    if (netStream.CanRead)
                    {
                        byte[] bytes = new byte[client.ReceiveBufferSize];
                        netStream.Read(bytes, 0, client.ReceiveBufferSize);
                        string receiveMsg = bytes.UTF7ToString();

                        if (receiveMsg.Contains("\0\0"))
                            receiveMsg = receiveMsg.Split("\0\0")[0];

                        // lógica para receber resposta do sistema e salvar as classes
                        receiveMsg = receiveMsg.Replace("CARGAEMPRESA@@", "").Replace("@@", "#").Replace(";", "#");

                        while (receiveMsg.Contains("##"))
                            receiveMsg = receiveMsg.Replace("##", "# #");

                        string[] data = receiveMsg.Split('#');

                        Empresa e = new Empresa
                        {
                            CODEMPRE = data[0],
                            NOMRZSOC = data[1],
                            NOMFANTA = data[2],
                            NROFONE = data[3],
                            DSCENDER = data[4],
                            NROENDER = data[5].ToInt(),
                            CPLENDER = data[6],
                            NOMBAIRR = data[7],
                            NROCEP = data[8].ToLong(),
                            CODMUNIC = data[9].ToLong(),
                            NROCNPJ = data[10],
                            NROINEST = data[11],
                            DTHULTAT = DateTime.Parse(data[12]),
                            USRULTAT = data[13],
                            DSCEMAIL = data[14],
                            NOMSSMTP = data[15],
                            NROPORTA = data[16].ToInt(),
                            INDAUTSV = data[17].ToBool(),
                            INDSSLSV = data[18].ToBool(),
                            INDTLSSV = data[19].ToBool(),
                            SNHEMAIL = data[20],
                            CG_VENDEDOR_ID = data[21].ToLong(),
                            EMLPRINC = data[22]
                        };

                        aux = this.Save(e);
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