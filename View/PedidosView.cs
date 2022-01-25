using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using EloComandas.Adapter;
using EloComandas.Controller;
using EloComandas.Entites;
using EloComandas.Persistence;
using EloComandas.Utils;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlertDialog = Android.App.AlertDialog;

namespace EloComandas.View
{
	[Activity(Label = "EloComandas", WindowSoftInputMode = SoftInput.AdjustResize, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class PedidosView : AppCompatActivity
	{
		private EditText txtPSQPROD, txtMESA, txtOBSPEDD, txtCOMENT, txtNOMBALCAO;
		private Button btnSUB, btnADI, btnADCITEM, btnSALVAR, btnLIMPAR, btnADCOMT;
		private ListView listView, listView2;
		private TextView lblQTD, lblNOMEPROD, lblVALOR, lbVLRTOTAL, lblNOMGAR;
		private RelativeLayout btnSecret;
		private CardView cvINDCONN;
		private FloatingActionButton floatingButton;
		private LinearLayout linearLayout7;
		private RelativeLayout relativeLayoutScroll;
		private ProgressBar progressBar;
		private List<ProdAdapterTB> listProd { get; set; }
		private List<ItemPedido> itemPedidos { get; set; }
		private List<Adicionais> itensADD { get; set; }
		private ProdAdapterTB SelectedItem { get; set; }
		private int? Position { get; set; } = null;
		private int? PositionCOM { get; set; } = null;

		public ItemPedido ItemComandaSelected { get; set; }
		public Adicionais Itenadd { get; set; }

		private System.Timers.Timer timer = null;


		bool indAtt = false;
		int Count = 0;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_pedidos);

			lblNOMGAR = FindViewById<TextView>(Resource.Id.lblNOMGAR);
			txtPSQPROD = FindViewById<EditText>(Resource.Id.txtPSQPROD);
			txtMESA = FindViewById<EditText>(Resource.Id.txtMESA);
			txtOBSPEDD = FindViewById<EditText>(Resource.Id.txtOBSPEDD);
			txtNOMBALCAO = FindViewById<EditText>(Resource.Id.txtNOMBALCAO);
			btnSUB = FindViewById<Button>(Resource.Id.btnSUB);
			btnADI = FindViewById<Button>(Resource.Id.btnADI);
			btnADCITEM = FindViewById<Button>(Resource.Id.btnADCITEM);
			btnSALVAR = FindViewById<Button>(Resource.Id.btnSALVAR);
			btnLIMPAR = FindViewById<Button>(Resource.Id.btnLIMPAR);
			btnADCOMT = FindViewById<Button>(Resource.Id.btnADCOMT);
			listView = FindViewById<ListView>(Resource.Id.listView);
			listView2 = FindViewById<ListView>(Resource.Id.listView2);
			lblQTD = FindViewById<TextView>(Resource.Id.lblQTD);
			lblNOMEPROD = FindViewById<TextView>(Resource.Id.lblNOMEPROD);
			lblVALOR = FindViewById<TextView>(Resource.Id.lblVALOR);
			lbVLRTOTAL = FindViewById<TextView>(Resource.Id.lbVLRTOTAL);
			btnSecret = FindViewById<RelativeLayout>(Resource.Id.btnSecret);
			cvINDCONN = FindViewById<CardView>(Resource.Id.cvINDCONN);
			floatingButton = FindViewById<FloatingActionButton>(Resource.Id.floatingButton);
			linearLayout7 = FindViewById<LinearLayout>(Resource.Id.linearLayout7);
			relativeLayoutScroll = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutScroll);
			progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

			linearLayout7.Visibility = ViewStates.Invisible;

			listProd = new List<ProdAdapterTB>();
			itensADD = new List<Adicionais>();
			itemPedidos = new List<ItemPedido>();
			progressBar.Visibility = ViewStates.Invisible;

			Task.Run(() =>
			{
				versionController();
				this.TestConection();
				Validations.VerificarPermissao(this);
			});

			timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(30000).TotalMilliseconds);
			timer.Elapsed += (s, a) => this.TestConection();
			timer.Enabled = true;
			timer.AutoReset = true;

			timer.Start();

			Enabled(false);

			string nome = new OperadorController().GetOperador().USROPER;
			if (nome.Length > 11)
				nome = nome.Substring(0, 11);

			lblNOMGAR.Text = nome;
			txtMESA.RequestFocus();


			txtPSQPROD.TextChanged += (sender, Args) => Pesquisar(txtPSQPROD.Text);

			txtMESA.TextChanged += (s, a) =>
			{
				if (!string.IsNullOrEmpty(txtMESA.Text))
				{
					txtNOMBALCAO.Text = "";
					txtNOMBALCAO.Enabled = false;
				}
				else
					txtNOMBALCAO.Enabled = true;
			};

			txtNOMBALCAO.TextChanged += (s, a) =>
			{
				if (!string.IsNullOrEmpty(txtNOMBALCAO.Text))
				{
					txtMESA.Text = "";
					txtMESA.Enabled = false;
				}
				else
					txtMESA.Enabled = true;
			};

			listView.ItemClick += (sender, args) =>
			{
				var adapter = (ProdutosAdapter)listView.Adapter;
				var item = SelectedItem = adapter[args.Position];
				var itemAdd = new ProdutoController().FindByDSCPROD(item.NOMEPROD).FirstOrDefault();
				var id = new ClassesController().FindByID_CLASSE_PROD(itemAdd.CG_CLASSE_PRODUTO_ID.Value);

				Position = args.Position;
				btnSUB.Enabled = true;
				btnADI.Enabled = true;
				lblNOMEPROD.Text = item.NOMEPROD;
				lblQTD.Text = (1).ToString();
				btnADCITEM.Enabled = true;
				indAtt = false;
				btnADCOMT.Enabled = false;
			};
			listView2.ItemClick += (sender, args) =>
			{

				var adapter = (ComandaAdapter)listView2.Adapter;
				var item = adapter[args.Position];
				var prod = new ProdutoController().FindByCODPROD(item.CODPROD.ToLong());
				var classeprod = new ClassesController().FindByID_CLASSE_PROD(prod.CG_CLASSE_PRODUTO_ID.Value);

				PositionCOM = args.Position;
				ItemComandaSelected = item;

				if (classeprod.DSCCLASS.StartsWith("OPCIONAL"))
				{
					btnSUB.Enabled = true;
					btnADI.Enabled = false;
					btnADCOMT.Enabled = false;
				}
				else if (classeprod.DSCCLASS.StartsWith("ADICIONAL"))
				{
					btnSUB.Enabled = true;
					btnADI.Enabled = true;
					btnADCOMT.Enabled = false;
				}
				else
				{
					btnSUB.Enabled = true;
					btnADI.Enabled = true;
					btnADCOMT.Enabled = true;
				}



				btnADCITEM.Enabled = false;
				indAtt = true;
				txtPSQPROD.Text = "";
				lblNOMEPROD.Text = item.NOMPROD;
				lblQTD.Text = item.QTDPROD.ToString();
				lblVALOR.Text = item.VLRTOTAL.ToString("C");
			};

			listView2.ItemLongClick += (sender, args) =>
			{
				var adapter = (ComandaAdapter)listView2.Adapter;
				var item = adapter[args.Position];
				var prod = new ProdutoController().FindByDSCPROD(item.NOMPROD).FirstOrDefault();
				var ClassItem = new ClassesController().FindByID_CLASSE_PROD(prod.CG_CLASSE_PRODUTO_ID.Value);

				if (!prod.DSCPROD.StartsWith("ADD"))
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("AVISO DO SISTEMA");
					builder.SetMessage("SELECIONE UMA OPÇÃO");
					builder.SetPositiveButton("ADD COMENTÁRIO", (s, e) =>
					{
						AlertDialog.Builder builder = new AlertDialog.Builder(this);
						builder.SetTitle("ADICIONE UM COMENTÁRIO");
						txtCOMENT = new EditText(this);
						builder.SetView(txtCOMENT);
						if (item.DSCROBS != null)
							txtCOMENT.Text = item.DSCROBS;
						builder.SetPositiveButton("SALVAR", (s, e) => SaveComments(txtCOMENT.Text, item, args.Position));
						builder.SetNegativeButton("CANCELAR", (s, e) => { return; });
						AlertDialog dialog = builder.Create();
						dialog.Show();

						ShowKeyboard(txtCOMENT);

					});
					builder.SetNegativeButton("EXCLUIR", (s, e) =>
					{
						AlertDialog.Builder builder = new AlertDialog.Builder(this);
						builder.SetTitle("CONFIRMA A EXCLUSÃO DESSE ITEM?");
						builder.SetPositiveButton("SIM", (s, e) =>
						{
							DeleteItem(args.Position);
						});
						builder.SetNegativeButton("NÃO", (s, e) => { return; });
						AlertDialog dialog = builder.Create();
						dialog.Show();

					});

					AlertDialog dialog = builder.Create();
					dialog.Show();
				}
				else if (ClassItem.DSCCLASS.StartsWith("OPCIONAL") || prod.DSCPROD.StartsWith("ADD"))
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("AVISO DO SISTEMA");
					builder.SetMessage("SELECIONE UMA OPÇÃO");
					builder.SetPositiveButton("EXCLUIR", (s, e) => DeleteItem(args.Position));
					builder.SetNegativeButton("CANCELAR", (s, e) =>
					{
						return;

					});
					AlertDialog dialog = builder.Create();
					dialog.Show();
				}

			};

			btnADI.Click += (sender, args) =>
			{
				if (indAtt == false)
				{
					if (SelectedItem != null)
					{
						int qnt = int.Parse(lblQTD.Text);
						lblQTD.Text = (qnt + 1).ToString();
					}
				}

				if (indAtt == true)
				{
					if (PositionCOM != null)
					{
						int qnt = int.Parse(lblQTD.Text);
						lblQTD.Text = (qnt + 1).ToString();

						itemPedidos.RemoveAt(PositionCOM.Value);
						var item = new ProdutoController().FindByDSCPROD(ItemComandaSelected.NOMPROD).FirstOrDefault();

						itemPedidos.Insert(PositionCOM.Value, new ItemPedido
						{
							CODPROD = item.CODPROD.ToString(),
							NOMPROD = item.DSCPROD,
							QTDUNID = item.QTDUNID,
							QTDPROD = double.Parse(lblQTD.Text),
							VLRUNIT = item.PRCVENDA,
							VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
						});
						var adapterComand = new ComandaAdapter(this, itemPedidos);
						this.listView2.Adapter = adapterComand;

						FindVlrTotal();

						var precoprod = new ProdutoController().FindById(ItemComandaSelected.CODPROD).PRCVENDA;
						lblVALOR.Text = "";
						lblVALOR.Text = (precoprod * double.Parse(lblQTD.Text)).ToString("C");

						ItemComandaSelected = itemPedidos.ElementAt(PositionCOM.Value);
					}
				}
			};
			btnSUB.Click += (sender, args) =>
			{
				int qnt = int.Parse(lblQTD.Text);
				int aux = (qnt - 1);
				if (indAtt == false)
				{
					if (SelectedItem != null)
					{
						if (aux >= 1)
							lblQTD.Text = aux.ToString();
						else
							lblQTD.Text = (1).ToString();
					}
				}

				if (indAtt == true)
				{
					if (PositionCOM != null)
					{
						if (aux >= 0)
							lblQTD.Text = aux.ToString();
						else
							lblQTD.Text = (1).ToString();

						itemPedidos.RemoveAt(PositionCOM.Value);
						var item = new ProdutoController().FindByDSCPROD(ItemComandaSelected.NOMPROD).FirstOrDefault();

						itemPedidos.Insert(PositionCOM.Value, new ItemPedido
						{
							CODPROD = item.CODPROD.ToString(),
							NOMPROD = item.DSCPROD,
							QTDUNID = item.QTDUNID,
							QTDPROD = double.Parse(lblQTD.Text),
							VLRUNIT = item.PRCVENDA,
							VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
						});
						var adapterComand = new ComandaAdapter(this, itemPedidos);
						this.listView2.Adapter = adapterComand;

						FindVlrTotal();

						ItemComandaSelected = itemPedidos.ElementAt(PositionCOM.Value);

						var precoprod = new ProdutoController().FindById(ItemComandaSelected.CODPROD).PRCVENDA;
						lblVALOR.Text = "";
						lblVALOR.Text = (precoprod * double.Parse(lblQTD.Text)).ToString("C");

						if (aux == 0)
						{
							AlertDialog.Builder builder = new AlertDialog.Builder(this);
							builder.SetTitle("CONFIRMA A EXCLUSÃO DESSE ITEM?");
							builder.SetPositiveButton("SIM", (s, e) =>
							{
								DeleteItem(PositionCOM.Value);
								this.ItemComandaSelected = null;
								this.PositionCOM = null;
							});
							builder.SetNegativeButton("NÃO", (s, e) =>
							{
								lblQTD.Text = "1";
								lblVALOR.Text = (item.PRCVENDA * double.Parse(lblQTD.Text)).ToString("C");

								itemPedidos.RemoveAt(PositionCOM.Value);
								itemPedidos.Insert(PositionCOM.Value, new ItemPedido
								{
									CODPROD = item.CODPROD.ToString(),
									NOMPROD = item.DSCPROD,
									QTDUNID = item.QTDUNID,
									QTDPROD = double.Parse(lblQTD.Text),
									VLRUNIT = item.PRCVENDA,
									VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
								});
								var adapterComand = new ComandaAdapter(this, itemPedidos);
								this.listView2.Adapter = adapterComand;

								FindVlrTotal();

								return;
							});
							AlertDialog dialog = builder.Create();
							dialog.Show();
						}
					}
				}
			};
			lblQTD.TextChanged += (sender, args) =>
			{
				if (SelectedItem != null)
				{
					var item = SelectedItem;
					var precoprod = new ProdutoController().FindById(item.ID_PROD).PRCVENDA;
					lblVALOR.Text = "";
					lblVALOR.Text = (precoprod * double.Parse(lblQTD.Text)).ToString("C");
				}
			};

			btnADCITEM.Click += (sender, args) => AdicionarProd();

			btnLIMPAR.Click += (sender, args) =>
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("AVISO DO SISTEMA");
				builder.SetMessage("DESEJA LIMPAR A TELA?");
				builder.SetPositiveButton("SIM", (s, e) => LimparTela());
				builder.SetNegativeButton("CANCELAR", (s, e) => { return; });
				AlertDialog dialog = builder.Create();
				dialog.Show();
			};

			btnSALVAR.Click += (sender, args) =>
			{
				if (TestConection())
					Salvar();
				else
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("AVISO DO SISTEMA");
					builder.SetMessage("VOCÊ ESTÁ SEM CONEXÃO COM O SERVIDOR");
					builder.SetNeutralButton("OK", (s, e) => { return; });
					AlertDialog dialog = builder.Create();
					dialog.Show();
				}
			};

			btnADCOMT.Click += (sender, args) =>
			{
				if (ItemComandaSelected != null)
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("ADICIONE UM COMENTÁRIO");
					txtCOMENT = new EditText(this);
					builder.SetView(txtCOMENT);
					txtCOMENT.Text = ItemComandaSelected.DSCROBS;
					builder.SetPositiveButton("SALVAR", (s, e) => SaveComments(txtCOMENT.Text, ItemComandaSelected, PositionCOM.Value));
					builder.SetNegativeButton("CANCELAR", (s, e) => { return; });
					AlertDialog dialog = builder.Create();
					dialog.Show();

					ShowKeyboard(txtCOMENT);
				}
				else
				{
					ClasseProduto classe;
					bool auxFindIten = false;
					ItemPedido itemSelected = null;
					var countList = itemPedidos.Count - 1;
					while (auxFindIten == false)
					{
						var auxItem = itemPedidos.ElementAt(countList);
						var auxProd = new ProdutoController().FindByCODPROD(auxItem.CODPROD.ToLong());
						var auxClasse = new ClassesController().FindByID_CLASSE_PROD(auxProd.CG_CLASSE_PRODUTO_ID.Value);
						if (auxItem.NOMPROD.StartsWith("ADD") || auxClasse.DSCCLASS.StartsWith("OPCIONAL"))
							countList -= 1;
						else
						{
							itemSelected = auxItem;
							auxFindIten = true;
						}
					}
					var idLastItemCOM = new ProdutoController().FindByCODPROD(itemSelected.CODPROD.ToLong());
					classe = new ClassesController().FindByID_CLASSE_PROD(idLastItemCOM.CG_CLASSE_PRODUTO_ID.Value);

					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("ADICIONE UM COMENTÁRIO");
					txtCOMENT = new EditText(this);
					builder.SetView(txtCOMENT);
					txtCOMENT.Text = itemSelected.DSCROBS;
					builder.SetPositiveButton("SALVAR", (s, e) => SaveComments(txtCOMENT.Text, itemSelected, countList));
					builder.SetNegativeButton("CANCELAR", (s, e) => { return; });
					AlertDialog dialog = builder.Create();
					dialog.Show();

					ShowKeyboard(txtCOMENT);
				}

			};
			btnSecret.Click += (sender, eventArgs) =>
			{
				if (Count == 0)
					Task.Run(() => { Thread.Sleep(TimeSpan.FromSeconds(1.5)); Count = 0; });

				++Count;

				if (Count == 5)
				{
					btn_logout();
				}
			};
			floatingButton.Click += (sender, args) =>
			{
				progressBar.Visibility = ViewStates.Visible;
				Enabled(false);

				new Thread(() =>
				{
					RunOnUiThread(() =>
					{
						this.Msg("ESTAMOS BAIXANDO A NOVA VERSÃO DO APLICATIVO, FAVOR AGUARDE !");

						Task.Run(() =>
						{
							if (downLoadNewVersion())
							{
								RunOnUiThread(() =>
								{
									try
									{
										FTPController fTPController = new FTPController();

										linearLayout7.Visibility = ViewStates.Invisible;

										this.Msg("DOWNLOAD REALIZADO COM SUCESSO !");

										CreateFolder();

										Intent intent;

										if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
										{
											Java.IO.File apkPath = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloSoftware/EloComandas.EloComandas.apk");
											Android.Net.Uri apkURI = Android.Support.V4.Content.FileProvider.GetUriForFile(this, this.ApplicationContext.PackageName + ".EloComandas.EloComandas.provider", apkPath);

											intent = new Intent(Intent.ActionInstallPackage);
											intent.SetData(apkURI);
											intent.AddFlags(ActivityFlags.GrantReadUriPermission);
										}
										else
										{
											Android.Net.Uri apkUri = Android.Net.Uri.FromFile(new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloSoftware/EloComandas.EloComandas.apk"));
											intent = new Intent(Intent.ActionView);
											intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
											intent.SetFlags(ActivityFlags.NewTask);
										}
										StartActivity(intent);
										Finish();
									}
									catch (Exception e)
									{
										Log.Error("Elo_Log", e.ToString());
									}

									progressBar.Visibility = ViewStates.Invisible;
									Enabled(true);
								});
							}
							else
								RunOnUiThread(() => this.Msg("FALHA AO REALIZAR O DOWNLOAD DA ATUALIZAÇÃO! \nTENTE NOVAMENTE"));
						});
					});
				}).Start();




				//if (downLoadNewVersion())
				//{
				//	try
				//	{
				//		FTPController fTPController = new FTPController();

				//		linearLayout7.Visibility = ViewStates.Invisible;

				//		this.Msg("DOWNLOAD REALIZADO COM SUCESSO !");

				//		Intent intent;

				//		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
				//		{
				//			File apkPath = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloSoftware/EloComandas.EloComandas.apk");
				//			Android.Net.Uri apkURI = FileProvider.GetUriForFile(this, this.ApplicationContext.PackageName + ".EloComandas.EloComandas.provider", apkPath);

				//			intent = new Intent(Intent.ActionInstallPackage);
				//			intent.SetData(apkURI);
				//			intent.AddFlags(ActivityFlags.GrantReadUriPermission);
				//		}
				//		else
				//		{
				//			Android.Net.Uri apkUri = Android.Net.Uri.FromFile(new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloSoftware/EloComandas.EloComandas.apk"));
				//			intent = new Intent(Intent.ActionView);
				//			intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
				//			intent.SetFlags(ActivityFlags.NewTask);
				//		}
				//		StartActivity(intent);
				//		Finish();
				//	}
				//	catch (Exception e)
				//	{
				//		Log.Error("Elo_Log", e.ToString());
				//	}
				//}
				//else
				//	this.Msg("FALHA AO REALIZAR O DOWNLOAD DA ATUALIZAÇÃO");
			};
		}

		private void CreateFolder()
		{
			try
			{
				if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
					ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 20);
				if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
					ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 21);


				FTPController fTPController = new FTPController();
				File folder = new File(Android.OS.Environment.ExternalStorageDirectory + File.Separator + "EloSoftware");
				if (!folder.Exists())
				{
					folder.Mkdir();
					downLoadNewVersion();
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

		public static void ShowKeyboard(EditText editText)
		{
			editText.RequestFocus();

			InputMethodManager inputMethodManager = Application.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
			inputMethodManager.ShowSoftInput(editText, ShowFlags.Forced);
			inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
			editText.SelectAll();
		}
		private void DeleteItem(int posicao)
		{
			var adapterComand = new ComandaAdapter(this, itemPedidos);
			itemPedidos.RemoveAt(posicao);

			int countList = itemPedidos.Count - 1;

			if (posicao <= countList)
			{
				bool auxADD = true;
				while (auxADD == true)
				{
					var adapter = (ComandaAdapter)listView2.Adapter;
					if (itemPedidos.Count > 0)
					{
						var auxItem = adapter[posicao];
						var prod = new ProdutoController().FindByCODPROD(auxItem.CODPROD.ToLong());
						var ClassItem = new ClassesController().FindByID_CLASSE_PROD(prod.CG_CLASSE_PRODUTO_ID.Value);
						if (auxItem != null)
							if (ClassItem.DSCCLASS.StartsWith("ADICIONAL") || ClassItem.DSCCLASS.StartsWith("OPCIONAL"))
								itemPedidos.RemoveAt(posicao);
							else
								auxADD = false;
						else
							auxADD = false;
					}
					else
						auxADD = false;

				}
			}
			adapterComand = new ComandaAdapter(this, itemPedidos);
			this.listView2.Adapter = adapterComand;
			this.PositionCOM = null;
			btnADI.Enabled = false;
			btnSUB.Enabled = false;

			if (listView2.Adapter.Count == 0)
			{
				listView2.Adapter = null;
				btnADCOMT.Enabled = false;
			}
			FindVlrTotal();
		}
		private void Enabled(bool boolean)
		{
			btnSUB.Enabled = boolean;
			btnADI.Enabled = boolean;
			btnADCITEM.Enabled = boolean;
			btnLIMPAR.Enabled = boolean;
			btnSALVAR.Enabled = boolean;
			btnADCOMT.Enabled = boolean;
		}
		private void FindVlrTotal()
		{
			double vlr = 0;
			itemPedidos.ForEach(i =>
			{
				vlr += i.VLRTOTAL;
			});
			lbVLRTOTAL.Text = vlr.ToString("C");
		}
		private void Pesquisar(string name)
		{
			if (name != "")
			{
				listView.Adapter = null;
				listProd.Clear();
				var prod = new ProdutoController().FindByDSCPROD(name).ToList();


				prod.ForEach(p =>
				{
					listProd.Add(new ProdAdapterTB
					{
						ID_PROD = (long)p.CG_PRODUTO_ID,
						NOMEPROD = p.DSCPROD,
						VLRPREC = p.PRCVENDA.ToString()
					});
				});
				var adapter = new ProdutosAdapter(this, listProd);
				this.listView.Adapter = adapter;


				indAtt = false;
			}

			else
			{
				listView.Adapter = null;
				listProd.Clear();
			}

		}
		private void AdicionarProd()
		{
			string[] valorprod = lblVALOR.Text.Split(" ");

			var item = new ProdutoController().FindByDSCPROD(SelectedItem.NOMEPROD).FirstOrDefault();
			var ClassItem = new ClassesController().FindByID_CLASSE_PROD(item.CG_CLASSE_PRODUTO_ID.Value);
			if (!ClassItem.DSCCLASS.StartsWith("ADICIONAL") && !ClassItem.DSCCLASS.StartsWith("OPCIONAL"))
			{
				itemPedidos.Add(new ItemPedido
				{
					CODPROD = item.CODPROD.ToString(),
					NOMPROD = item.DSCPROD,
					QTDUNID = item.QTDUNID,
					QTDPROD = double.Parse(lblQTD.Text),
					VLRUNIT = item.PRCVENDA,
					VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
				});
				var adapterComand = new ComandaAdapter(this, itemPedidos);
				this.listView2.Adapter = adapterComand;

				FindVlrTotal();

				lblQTD.Text = "0";
				lblNOMEPROD.Text = "";
				txtPSQPROD.Text = "";

				btnSALVAR.Enabled = true;
				btnLIMPAR.Enabled = true;
				this.SelectedItem = null;
				this.Position = null;
				this.PositionCOM = null;
			}
			else if (listView2.Adapter != null)
			{
				ClasseProduto classe;
				if (PositionCOM.HasValue)
				{
					var prod = new ProdutoController().FindByCODPROD(ItemComandaSelected.CODPROD.ToLong());
					classe = new ClassesController().FindByID_CLASSE_PROD(prod.CG_CLASSE_PRODUTO_ID.Value);
				}
				else
				{
					bool auxFindIten = false;
					ItemPedido itemSelected = null;
					var countList = itemPedidos.Count - 1;
					while (auxFindIten == false)
					{
						var auxItem = itemPedidos.ElementAt(countList);
						var auxProd = new ProdutoController().FindByCODPROD(auxItem.CODPROD.ToLong());
						var auxClasse = new ClassesController().FindByID_CLASSE_PROD(auxProd.CG_CLASSE_PRODUTO_ID.Value);
						if (auxItem.NOMPROD.StartsWith("ADD") || auxClasse.DSCCLASS.StartsWith("OPCIONAL"))
							countList -= 1;
						else
						{
							itemSelected = auxItem;
							auxFindIten = true;
						}
					}
					var idLastItemCOM = new ProdutoController().FindByCODPROD(itemSelected.CODPROD.ToLong());
					classe = new ClassesController().FindByID_CLASSE_PROD(idLastItemCOM.CG_CLASSE_PRODUTO_ID.Value);


				}


				if (ClassItem.DSCCLASS.EndsWith(classe.DSCCLASS))
				{
					itensADD.Add(new Adicionais
					{
						CODPROD = item.CODPROD.ToString(),
						NOMPROD = item.DSCPROD,
						QTDUNID = item.QTDUNID,
						QTDPROD = double.Parse(lblQTD.Text),
						VLRUNIT = item.PRCVENDA,
						VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
					});
					if (!PositionCOM.HasValue)
					{
						itemPedidos.Add(new ItemPedido
						{
							CODPROD = item.CODPROD.ToString(),
							NOMPROD = item.DSCPROD,
							QTDUNID = item.QTDUNID,
							QTDPROD = double.Parse(lblQTD.Text),
							VLRUNIT = item.PRCVENDA,
							VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
						});
					}
					else
					{
						itemPedidos.Insert(PositionCOM.Value + 1, new ItemPedido
						{
							CODPROD = item.CODPROD.ToString(),
							NOMPROD = item.DSCPROD,
							QTDUNID = item.QTDUNID,
							QTDPROD = double.Parse(lblQTD.Text),
							VLRUNIT = item.PRCVENDA,
							VLRTOTAL = item.PRCVENDA * double.Parse(lblQTD.Text)
						});
					}

					var adapterComand = new ComandaAdapter(this, itemPedidos);
					this.listView2.Adapter = adapterComand;

					FindVlrTotal();

					lblQTD.Text = "0";
					lblNOMEPROD.Text = "";
					txtPSQPROD.Text = "";

					this.Position = null;
					this.PositionCOM = null;
					btnSALVAR.Enabled = true;
					btnLIMPAR.Enabled = true;
					this.SelectedItem = null;
				}
				else
				{
					string[] DSCRCLASS = ClassItem.DSCCLASS.Split(" ");

					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("AVISO DO SISTEMA");
					builder.SetMessage($"ESSE {DSCRCLASS[0]} NÃO FAZ PARTE DA CLASSE DO PRODUTO");
					builder.SetNeutralButton("OK", (s, e) => { return; });
					AlertDialog dialog = builder.Create();
					dialog.Show();

					lblQTD.Text = "0";
					lblNOMEPROD.Text = "";
					txtPSQPROD.Text = "";
					lblVALOR.Text = "R$ 0,00";
					btnADCITEM.Enabled = false;
					this.Position = null;
					this.PositionCOM = null;
					this.SelectedItem = null;
					this.ItemComandaSelected = null;

					txtPSQPROD.RequestFocus();
				}

			}
			else
			{
				string[] DSCRCLASS = ClassItem.DSCCLASS.Split(" ");

				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("AVISO DO SISTEMA");
				builder.SetMessage($"NÃO HÁ NENHUM PRODUTO NA COMANDA PARA VINCULAR ESSE {DSCRCLASS[0]}");
				builder.SetNeutralButton("OK", (s, e) => { return; });
				AlertDialog dialog = builder.Create();
				dialog.Show();

				lblQTD.Text = "0";
				lblNOMEPROD.Text = "";
				txtPSQPROD.Text = "";
				lblVALOR.Text = "R$ 0,00";
				btnADCITEM.Enabled = false;
				this.Position = null;
				this.PositionCOM = null;
				this.SelectedItem = null;
				this.ItemComandaSelected = null;

				txtPSQPROD.RequestFocus();
			}

			btnSUB.Enabled = false;
			btnADI.Enabled = false;
			btnADCOMT.Enabled = false;
			btnADCITEM.Enabled = false;

		}
		private void SaveComments(string commet, ItemPedido item, int positionCOM)
		{
			itemPedidos.RemoveAt(positionCOM);
			commet = commet.ToUpper();
			if (commet != "")
				item.DSCROBS = commet;
			else
				item.DSCROBS = null;

			itemPedidos.Insert(positionCOM, item);

			var adapterComand = new ComandaAdapter(this, itemPedidos);
			this.listView2.Adapter = adapterComand;

		}
		private void LimparTela()
		{
			txtMESA.Text = "";
			txtNOMBALCAO.Text = "";
			lblQTD.Text = "0";
			lblNOMEPROD.Text = "";
			txtPSQPROD.Text = "";
			txtOBSPEDD.Text = "";
			lbVLRTOTAL.Text = "R$ 0,00";
			lblVALOR.Text = "R$ 0,00";
			listView.Adapter = null;
			listView2.Adapter = null;
			this.Position = null;
			this.PositionCOM = null;
			this.SelectedItem = null;
			this.ItemComandaSelected = null;
			this.itensADD.Clear();
			this.itemPedidos.Clear();
			txtMESA.Focusable = true;
			txtNOMBALCAO.Focusable = true;

			Enabled(false);

			txtMESA.RequestFocus();
		}
		private void Salvar()
		{
			if (txtMESA.Text != "" || txtNOMBALCAO.Text != "")
			{
				if (txtMESA.Text != "" && txtNOMBALCAO.Text != "")
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("AVISO DO SISTEMA");
					builder.SetMessage("NÃO É PERMITIDO A MESA E O BALCÃO TEREM VALOR");
					builder.SetNeutralButton("OK", (s, e) => { return; });
					AlertDialog dialog = builder.Create();
					dialog.Show();

					txtMESA.RequestFocus();
				}
				else if (itemPedidos.Count > 0)
				{

					try
					{
						AdicionaisController itensAddController = new AdicionaisController();
						ItemPedidoController itemPCOntroller = new ItemPedidoController();
						PedidoController pController = new PedidoController();
						OperadorController oController = new OperadorController();
						VendedorController vController = new VendedorController();

						string obs = "";
						string[] vlr = lbVLRTOTAL.Text.Split(" ");
						double vlrP = double.Parse(vlr[1]);
						if (txtOBSPEDD.Text != "")
							obs = txtOBSPEDD.Text;

						Pedido p = null;

						progressBar.Visibility = ViewStates.Visible;

						Database.RunInTransaction(() =>
						{
							Task.Run(() =>
							{
								p = new Pedido()
								{
									NROMESA = txtMESA.Text,
									NOMBALC = txtNOMBALCAO.Text,
									CODEMPRE = new EmpresaController().GetEmpresa().CODEMPRE,
									DATEMISS = DateTime.Now,
									CG_VENDEDOR_ID = new VendedorController().GetVendedor().CG_VENDEDOR_ID,
									VLRTOTPED = vlrP,
									OBSPEDID = obs,
									INDSINC = false,
									USRULTAT = new OperadorController().GetOperador().USROPER,
									DTHULTAT = DateTime.Now
								};

								pController.Save(p);


								itemPedidos.ForEach(i =>
								{
									var prod = new ProdutoController().FindByDSCPROD(i.NOMPROD).FirstOrDefault();
									var classe = new ClassesController().FindById(prod.CG_CLASSE_PRODUTO_ID.Value);
									if (!i.NOMPROD.StartsWith("ADD") && !classe.DSCCLASS.StartsWith("OPCIONAL"))
									{
										ItemPedido ip = new ItemPedido()
										{
											FT_PEDIDO_ID = p.FT_PEDIDO_ID,
											CODPROD = i.CODPROD,
											NOMPROD = i.NOMPROD,
											QTDUNID = i.QTDUNID,
											QTDPROD = i.QTDPROD,
											VLRUNIT = i.VLRUNIT,
											VLRTOTAL = i.VLRTOTAL,
											DSCROBS = i.DSCROBS,
											USRULTAT = oController.GetOperador().USROPER,
											DTHULTAT = DateTime.Now.ToString("dd/MM/yyyy"),
											INDADIC = false

										};
										itemPCOntroller.Save(ip);
									}

									else if (i.NOMPROD.StartsWith("ADD"))
									{
										var id = itemPCOntroller.GetLastId();
										var lastItem = itemPCOntroller.FindById(id);

										Adicionais iADD = new Adicionais()
										{
											FT_PEDIDO_ITEM_ID = lastItem.FT_PEDIDO_ITEM_ID,
											FT_PEDIDO_ID = lastItem.FT_PEDIDO_ID,
											CODPROD = i.CODPROD,
											NOMPROD = i.NOMPROD,
											QTDUNID = i.QTDUNID,
											QTDPROD = i.QTDPROD,
											VLRUNIT = i.VLRUNIT,
											VLRTOTAL = i.VLRTOTAL,
											USRULTAT = i.USRULTAT,
											DTHULTAT = i.DTHULTAT
										};
										itensAddController.Save(iADD);

										lastItem.INDADIC = true;
										itemPCOntroller.Save(lastItem);
									}
									else if (classe.DSCCLASS.StartsWith("OPCIONAL"))
									{
										var id = itemPCOntroller.GetLastId();
										var lastItem = itemPCOntroller.FindById(id);
										Adicionais iADD = new Adicionais()
										{
											FT_PEDIDO_ITEM_ID = lastItem.FT_PEDIDO_ITEM_ID,
											FT_PEDIDO_ID = lastItem.FT_PEDIDO_ID,
											CODPROD = i.CODPROD,
											NOMPROD = i.NOMPROD,
											QTDUNID = i.QTDUNID,
											QTDPROD = i.QTDPROD
										};
										itensAddController.Save(iADD);

										lastItem.INDADIC = true;
										itemPCOntroller.Save(lastItem);
									}
								});

								if (pController.ComSocket(p, out string msg))
								{
									p.INDSINC = true;
									pController.Save(p);
								}

								LimparTela();
								Enabled(false);
								RunOnUiThread(() => progressBar.Visibility = ViewStates.Invisible);
							});

						});
					}
					catch(Exception e)
					{
						Log.Error("LOG_COMANDAS", e.Message);
						this.Msg("ERRO AO SALVAR PEDIDO");
					}
					finally
					{
						progressBar.Visibility = ViewStates.Invisible;
					}
				}
				else
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("AVISO DO SISTEMA");
					builder.SetMessage("FAVOR ADICIONE OS PRODUTOS NA COMANDA");
					builder.SetNeutralButton("OK", (s, e) => { return; });
					AlertDialog dialog = builder.Create();
					dialog.Show();
				}
			}
			else
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("AVISO DO SISTEMA");
				builder.SetMessage("FAVOR INFORME A MESA OU O NOME DO BALCÃO");
				builder.SetNeutralButton("OK", (s, e) => { return; });
				AlertDialog dialog = builder.Create();
				dialog.Show();

				txtMESA.RequestFocus();
			}
		}




		/// <summary>
		///  Cria o menu para o app
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			base.OnCreateOptionsMenu(menu);
			MenuInflater menuInflater = MenuInflater;
			menuInflater.Inflate(Resource.Menu.main_menu, menu);

			return true;
		}


		/// <summary>
		///  Controla a ação do botão ao ser clicado (Menu)
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{

			switch (item.ItemId)
			{
				case Resource.Id.atualizar:
					{
						AlertDialog.Builder builder = new AlertDialog.Builder(this);
						builder.SetTitle("AVISO DO SISTEMA");
						builder.SetMessage("DESEJA ATUALIZAR OS DADOS ?");
						builder.SetPositiveButton("SIM", (s, e) => AtualizarDados());
						builder.SetNegativeButton("NÃO", (s, e) => { return; });
						AlertDialog dialog = builder.Create();
						dialog.Show();

						break;
					}
			}

			return true;
		}

		/// <summary>
		///  Atualiza os dados do app com o servidor
		/// </summary>
		protected virtual void AtualizarDados()
		{
			bool loop = false;

			new Thread(() =>
			{
				try
				{
					var controller = new ConfigController();
					var config = controller.Config;
					DNS dns = controller.GetDNS();

					int count = 0;
					loop = true;
					Enabled(false);

					RunOnUiThread(() => Enabled(false));

					Database.RunInTransaction(() =>
					{
						if (!string.IsNullOrEmpty(dns.Host) && dns.Port != 0)
						{
							if (new ConfigController().TestServerConnection())
							{
								this.Msg("SINCRONIZANDO DADOS COM O SERVIDOR! AGUARDE...");


								EmpresaController empresaC = new EmpresaController();
								OperadorController operadorC = new OperadorController();
								ProdutoController produtoC = new ProdutoController();
								ClassesController classController = new ClassesController();
								ClasseProdutoAdicionalController addcController = new ClasseProdutoAdicionalController();

								string empresa = empresaC.GetEmpresa().CODEMPRE.ToString();
								string sequencia = "0000";
								DateTime DateTimeLastPROD = produtoC.GetLastDateTime();
								DateTime DateTimeLastClassProd = classController.GetLastDateTime();
								DateTime DateTimeLastClassProdAdd = addcController.GetLastDateTime();

								Vendedor vendedor = new VendedorController().GetVendedor();
								string id = string.Format("{0:0000}", vendedor.CG_VENDEDOR_ID);

								if (produtoC.ComSocket($"CARGAPRODUTO{empresa}{id}{DateTimeLastPROD}", dns.Host, dns.Port))
								{
									ClassesController cController = new ClassesController();
									if (cController.ComSocket($"CARGACLASSEPRODUTO{empresa}{sequencia}{DateTimeLastClassProd}", dns.Host, dns.Port))
									{
										ClasseProdutoAdicionalController addCController = new ClasseProdutoAdicionalController();
										if (addCController.ComSocket($"CARGAADICIONALPRODUTO{empresa}{sequencia}{DateTimeLastClassProdAdd}", dns.Host, dns.Port))
											this.Msg("SINCRONIA COM SERVIDOR REALIZADA COM SUCESSO!");
										else
											this.Msg("FALHA AO SICRONIZAR CLASSES DOS PRODUTOS ADICIONAIS COM SERVIDOR! VERIFIQUE.");
									}
									else
										this.Msg("FALHA AO SICRONIZAR CLASSES DOS PRODUTOS COM SERVIDOR! VERIFIQUE.");
								}
								else
									this.Msg("ERRO AO SINCRONIZAR PRODUTOS! VERIFIQUE.");


							}
						}
					});

				}
				catch (Exception ex)
				{
					Log.Error("erro", ex.ToString());
				}
				finally
				{
					RunOnUiThread(() =>
					{
						loop = false;
						Enabled(true);

						LimparTela();
					});
				}
			}).Start();
		}
		private void btn_logout()
		{
			Intent i = new Intent(this, typeof(ConfigView));
			StartActivity(i);
		}

		private bool TestConection()
		{
			if (new ConfigController().TestServerConnection() != true)
			{
				cvINDCONN.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("Red"));
				return false;
			}
			else
			{
				cvINDCONN.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("Green"));
				return true;
			}
		}
		protected override void OnDestroy()
		{
			timer.Stop();
			base.OnDestroy();
		}

		private void versionController()
		{
			if (new ConfigController().TestServerConnection("elosoftware.dyndns.org", 8560))
			{
				try
				{
					DateTime curretnVersion = DateTime.Parse(new FTPController().getCurrentVersion());
					DateTime apkVersion = DateTime.Parse(new FTPController().getApkVersion()).AddHours(3);
					DateTime appVersion = FTPController.getAppVersion(out string version).AddHours(3);

					if (curretnVersion > appVersion)
					{
						linearLayout7.Visibility = ViewStates.Visible;
						relativeLayoutScroll.SetPadding(0, 0, 0, 150);
					}
					else
					{
						linearLayout7.Visibility = ViewStates.Invisible;
						relativeLayoutScroll.SetPadding(0, 0, 0, 0);
					}
				}
				catch
				{
					Log.Error("", "");
					//Activity activity = CrossCurrentActivity.Current.Activity;
					//Android.Views.View view = activity.FindViewById(Android.Resource.Id.Content);
					//Snackbar.Make(view, "HÁ UMA NOVA ATUALIZAÇÃO DISPONÍVEL !", 100000).Show() ;
				}
			}

			//if (new ConfigController().TestServerConnection("elosoftware.dyndns.org", 8560))
			//{
			//	//DateTime nVersion = DateTime.Parse(new FTPController().getCurrentVerson(), CultureInfo.CreateSpecificCulture("pt-BR"));

			//	//DateTime cVersion = DateTime.Parse(new FTPController().getAppVersion()).AddHours(3);

			//	string nVersion = new FTPController().getCurrentVerson();
			//	string cVersion = new FTPController().getAppVersion();

			//	if (!nVersion.IsEmpty() && !cVersion.IsEmpty())
			//	{
			//		if (DateTime.Parse(nVersion, CultureInfo.CreateSpecificCulture("pt-BR")) > DateTime.Parse(cVersion).AddHours(3))
			//		{
			//			linearLayout7.Visibility = ViewStates.Visible;
			//			relativeLayoutScroll.SetPadding(0, 0, 0, 150);
			//		}
			//		else
			//		{
			//			linearLayout7.Visibility = ViewStates.Invisible;
			//			relativeLayoutScroll.SetPadding(0, 0, 0, 0);
			//		}
			//	}
			//}
		}
		private bool downLoadNewVersion()
		{
			bool result = false;
			if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
				ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 8);
			if (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
				ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 9);

			if (new ConfigController().TestServerConnection("elosoftware.dyndns.org", 8560))
			{
				if (new FTPController().updateApp())
					result = true;
			}

			return result;
		}

	}

}