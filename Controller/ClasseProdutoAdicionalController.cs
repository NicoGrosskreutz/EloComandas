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
	public class ClasseProdutoAdicionalController
	{
        ClasseProdutoAdicionalDAO DAO = new ClasseProdutoAdicionalDAO();
        public bool Save(ClasseProdutoAdicional produto)
        {
            var conn = Database.GetConnection();
            try
            {
                if (produto.CG_CLASSE_PRODUTO_ADICIONAL_ID == null)
                    produto.CG_CLASSE_PRODUTO_ADICIONAL_ID = GetLastId() == null ? 1 : GetLastId() + 1;

                if (FindById(produto.CG_CLASSE_PRODUTO_ADICIONAL_ID) == null)
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
        public ClasseProdutoAdicional FindById(object id) => DAO.FindById(id);
        public List<ClasseProdutoAdicional> FindAll() => DAO.FindAll();
        public DateTime GetLastDateTime()
        {
            DateTime DTHULTAT = FindAll().Max(p => p.DTHINCLU);
            return DTHULTAT;
        }

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
                    while (loop)
                        if (netStream.CanRead)
                        {
                            byte[] bytes = new byte[client.ReceiveBufferSize];
                            netStream.Read(bytes, 0, bytes.Length);
                            string receiveMsg = bytes.UTF7ToString();

                            if (receiveMsg.Contains("/0/0"))
                                receiveMsg = receiveMsg.Split("/0/0")[0];

                            receiveMsg = receiveMsg.Replace("CARGAADICIONALPRODUTO@@", "");

                            if (!receiveMsg.StartsWith("FIMADICIONAL"))
                            {

                                string[] lines = receiveMsg.Split("@@");

                                lines.ToList().ForEach((str) =>
                                {
                                    if (str != lines[lines.Length - 1])
                                    {
                                        string[] data = str.Split(';');

                                        ClasseProdutoAdicional p = new ClasseProdutoAdicional()
                                        {
                                            CG_CLASSE_PRODUTO_ADICIONAL_ID = data[0].ToLong(),
                                            CG_CLASSE_PRODUTO_ID = data[1].ToLong(),
                                            DTHINCLU = DateTime.Parse(data[2]),
                                            USRINCLU = data[3]

                                        };

                                        this.Save(p);

                                    }
                                    else
                                    {
                                        if (str.StartsWith("FIMADICIONAL"))
                                            loop = false;
                                        else
                                        {
                                            if (str.Contains("\0\0"))
                                                str = str.Split("\0\0")[0];
                                            if (str.Length == 6)
                                                str = str.Substring(2, 4);

                                            if (request.EndsWith($"CARGAADICIONALPRODUTO{empresa.CODEMPRE}0000"))
                                            {
                                                string msg1 = $"CARGAADICIONALPRODUTO{empresa.CODEMPRE}{str}";
                                                byte[] bytesMsg = msg1.ToUTF8(true);
                                                netStream.Write(bytesMsg, 0, bytesMsg.Length);
                                            }
                                            else
                                            {
                                                string auxData = request.Substring(27, 19);
                                                string msg1 = $"CARGAADICIONALPRODUTO{empresa.CODEMPRE}{str}{auxData}";
                                                byte[] bytesMsg = msg1.ToUTF8(true);
                                                netStream.Write(bytesMsg, 0, bytesMsg.Length);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                                loop = false;
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