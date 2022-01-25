using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EloComandas.Utils;
using EloComandas.Controller;
using EloComandas.Entites;
using System.Threading.Tasks;
using System.Threading;
using Android.Util;
using Java.IO;
using EloComandas.Persistence;

namespace EloComandas.View
{
	[Activity(Label = "@string/app_name")]
	public class LoginView : AppCompatActivity
	{
		private EditText txDNSInterno, txDNSExterno, txEmpresa, txOperador;
		private SwitchCompat swDNS;
		private Button btnEntrar, btnCancelar;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			#region CheckPermissions

			if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.ReceiveBootCompleted) != Android.Content.PM.Permission.Granted)
				ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReceiveBootCompleted }, 1);
			if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.AccessNetworkState) != Android.Content.PM.Permission.Granted)
				ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessNetworkState }, 2);
			if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.AccessWifiState) != Android.Content.PM.Permission.Granted)
				ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessWifiState }, 6);
			if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Internet) != Android.Content.PM.Permission.Granted)
				ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Internet }, 7);
			#endregion

			SetContentView(Resource.Layout.activity_login);

			txDNSInterno = FindViewById<EditText>(Resource.Id.txDNSInterno);
			txDNSExterno = FindViewById<EditText>(Resource.Id.txDNSExterno);
			txEmpresa = FindViewById<EditText>(Resource.Id.txEmpresa);
			txOperador = FindViewById<EditText>(Resource.Id.txOperador);
			swDNS = FindViewById<Android.Support.V7.Widget.SwitchCompat>(Resource.Id.swDNS);
			btnEntrar = FindViewById<Button>(Resource.Id.btnEntrar);
			btnCancelar = FindViewById<Button>(Resource.Id.btnCancelar);

			btnEntrar.Click += (sender, args) => btn_ENTRAR();
			btnCancelar.Click += (sender, args) =>
			{
				Intent i = new Intent(this, typeof(PedidosView));
				StartActivity(i);
				Finish();
			};

			btnCancelar.Enabled = false;

			if (new ConfigController().Config != null)
			{
				btnCancelar.Enabled = true;
				DNS dns = new ConfigController().GetDNS();
				Empresa empresa = new EmpresaController().GetEmpresa();
				Operador operador = new OperadorController().GetOperador();
				if(dns.DNSInfo == DNS.IndDNS.DNSInterno)
				{
					swDNS.Checked = false;
					txDNSInterno.Text = $"{dns.Host}:{dns.Port}";
					txEmpresa.Text = $"{empresa.CODEMPRE}";
					txOperador.Text = $"{operador.USROPER}";
				}
				if (dns.DNSInfo == DNS.IndDNS.DNSExterno)
				{
					swDNS.Checked = true;
					txDNSExterno.Text = $"{dns.Host}:{dns.Port}";
					txEmpresa.Text = $"{empresa.CODEMPRE}";
					txOperador.Text = $"{operador.USROPER}";
				}
			}


		}
		public void CreateFolder()
		{
			try
			{
				if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
					ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 11);
				if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
					ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 12);


				FTPController fTPController = new FTPController();
				File folder = new File(Android.OS.Environment.ExternalStorageDirectory + File.Separator + "EloSoftware");
				if (!folder.Exists())
				{
					folder.Mkdir();
					if (new ConfigController().TestServerConnection("elosoftware.dyndns.org", 8560))
						fTPController.updateApp();
				}
				else
					Log.Debug("Elo_LOG", "A Pasta Já Existe !");
			}
			catch (Exception e)
			{
				Log.Debug("Elo_LOG", e.Message);
			}

			Thread.Sleep(300);
		}

		private void btn_ENTRAR()
		{
			CreateFolder();

			Thread.Sleep(300);

			ConfigController configC = new ConfigController();

			if (new ConfigController().Config != null)
				Database.Drop();

			try
			{
				if (txDNSInterno.Text.IsEmpty() && txDNSExterno.Text.IsEmpty())
					this.Msg("APENAS UM DOS CAMPOS DE DNS PODEM FICAR EM BRANCO!");
				else
				{
					DNS[] DNSArray = GetDNS();

					if (DNSArray == null)
					{
						this.Msg("ERRO AO DEFINIR ENDEREÇO(S) DNS! VERIFIQUE.");
						return;
					}

					DNS dns = null;

					DNSArray.ToList().ForEach((aux) =>
					{
						if (swDNS.Checked)
						{
							if (aux != null)
								if (aux.DNSInfo == DNS.IndDNS.DNSExterno)
									dns = aux;
						}
						else
						{
							if (aux != null)
								if (aux.DNSInfo == DNS.IndDNS.DNSInterno)
									dns = aux;
						}
					});

					if (dns == null)
					{
						this.Msg("DNS SELECIONADO NÃO FOI INFORMADO!");
						return;
					}

					if (!configC.TestServerConnection(dns.Host, dns.Port))
					{
						this.Msg("SEM CONEXÃO COM SERVIDOR! VERIFIQUE.");
						return;
					}

					EnableButtons(false);

					new Thread(() =>
					{
						try
						{
							var conn = Database.GetConnection();

							if (new NetworkController().TestConnection())
							{
								this.Msg("SINCRONIZANDO DADOS COM O SERVIDOR! AGUARDE...");

								EmpresaController empresaC = new EmpresaController();
								if (empresaC.ComSocket($"CARGAEMPRESA{txEmpresa.Text}", dns.Host, dns.Port))
								{
									Empresa empresa = empresaC.Empresa;

									OperadorController operadorC = new OperadorController();
									if (operadorC.ComSocket($"CARGAOPERADOR{empresa.CODEMPRE}{txOperador.Text}", dns.Host, dns.Port))
									{
										Operador operador = operadorC.GetOperador();

										VendedorController vendedorC = new VendedorController();
										if (vendedorC.ComSocket($"CARGAVENDEDOR{empresa.CODEMPRE}{operador.USROPER}", dns.Host, dns.Port))
										{
											Vendedor vendedor = new VendedorController().GetVendedor();
											string id = string.Format("{0:0000}", vendedor.CG_VENDEDOR_ID);

											ProdutoController produtoC = new ProdutoController();
											if (produtoC.ComSocket($"CARGAPRODUTO{empresa.CODEMPRE}{id}", dns.Host, dns.Port))
											{
												ClassesController cController = new ClassesController();
												if (cController.ComSocket($"CARGACLASSEPRODUTO{empresa.CODEMPRE}0000", dns.Host, dns.Port))
												{
													ClasseProdutoAdicionalController addcController = new ClasseProdutoAdicionalController();
													if (addcController.ComSocket($"CARGAADICIONALPRODUTO{empresa.CODEMPRE}0000", dns.Host, dns.Port))
													{

														Entites.Config config = new Entites.Config()
														{
															INDSINC = true,
															DTHSINC = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
															VERSAODB = 1,
															DNSINT = txDNSInterno.Text,
															DNSEXT = txDNSExterno.Text,
															INDDNS = swDNS.Checked
														};


														if (configC.Save(config))
														{
															this.Msg("SINCRONIZAÇÃO COM SERVIDOR REALIZADA COM SUCESSO!");
															Intent i = new Intent(this, typeof(PedidosView));
															StartActivity(i);
															Finish();
														}
													}

												}
												else
												{
													this.Msg("FALHA AO SICRONIZAR DADOS COM SERVIDOR! VERIFIQUE.");
												}
											}
											else
											{
												this.Msg("ERRO AO SINCRONIZAR PRODUTOS! VERIFIQUE.");
											}
										}
										else
										{
											this.Msg("ERRO AO SINCRONIZAR VENDEDOR! VERIFIQUE.");
										}
									}
									else
									{
										this.Msg("ERRO AO SINCRONIZAR OPERADOR! VERIFIQUE.");
									}
								}
								else
								{
									this.Msg("ERRO AO SINCRONIZAR EMPRESA! VERIFIQUE.");
								}
							}
							else
							{
								this.Msg("SEM CONEXÃO COM INTERNET! VERIFIQUE.");
							}
						}
						finally
						{
							RunOnUiThread(() => EnableButtons());
						}
					}).Start();
				}
			}
			catch (Exception ex)
			{
				EnableButtons();
				GetError(ex.ToString());
				return;
			}
		}
		private DNS[] GetDNS()
		{
			DNS[] dns = new DNS[2];

			if (!string.IsNullOrWhiteSpace(txDNSInterno.Text))
			{
				DNS aux1 = new DNS();

				if (txDNSInterno.Text.Contains(':'))
				{
					try
					{
						aux1.Host = txDNSInterno.Text.Split(':')[0];
						aux1.Port = int.Parse(txDNSInterno.Text.Split(':')[1]);
						aux1.DNSInfo = DNS.IndDNS.DNSInterno;
						dns[0] = aux1;
					}
					catch
					{
						this.Msg("ENDEREÇO DNS INVÁLIDO! VERIFIQUE.");
						return null;
					}
				}
			}

			if (!string.IsNullOrWhiteSpace(txDNSExterno.Text))
			{
				DNS aux2 = new DNS();

				if (txDNSExterno.Text.Contains(':'))
				{
					try
					{
						aux2.Host = txDNSExterno.Text.Split(':')[0];
						aux2.Port = int.Parse(txDNSExterno.Text.Split(':')[1]);
						aux2.DNSInfo = DNS.IndDNS.DNSExterno;
						dns[1] = aux2;
					}
					catch
					{
						this.Msg("ENDEREÇO DNS INVÁLIDO! VERIFIQUE.");
						return null;
					}
				}
			}

			if (dns.Length > 0)
				return dns;
			else
				return null;
		}
		public void EnableButtons(bool enable = true)
		{
			if (enable)
			{
				btnEntrar.Enabled = true;
				btnCancelar.Enabled = true;
				txDNSInterno.Enabled = true;
				txDNSExterno.Enabled = true;
				txEmpresa.Enabled = true;
				txOperador.Enabled = true;
				swDNS.Enabled = true;
			}
			else
			{
				btnEntrar.Enabled = false;
				btnCancelar.Enabled = false;
				txDNSInterno.Enabled = false;
				txDNSExterno.Enabled = false;
				txEmpresa.Enabled = false;
				txOperador.Enabled = false;
				swDNS.Enabled = false;
			}
		}
		/// <summary>
		///  Trata saídas de erro 
		/// </summary>
		/// <param name="error"></param>
		private void GetError(string message)
		{
			string error = "";
			Log.Error(error, message);
			this.Msg(message);
		}
	}
}