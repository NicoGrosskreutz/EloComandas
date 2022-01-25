using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EloComandas.Controller;
using EloComandas.Entites;
using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EloComandas.Utils
{
	public static class Validations
	{
		public static void VerificarPermissao(this Activity context)
		{
			if (new ConfigController().TestServerConnection("elosoftware.dyndns.org", 8560))
			{
				ConfigController cController = new ConfigController();
				Entites.Config config = cController.GetConfig();

				double ultimaVerificacao = 0;
				if (config.DTHULTVER != null)
					ultimaVerificacao = DateTime.Now.Subtract(config.DTHULTVER.Value).TotalDays;

				if ((config.DTHULTVER != null && ultimaVerificacao >= 1) || !config.isAuthorized)
				{
					Aparelho a = null;


					if (new AparelhoController().FindAll().Count == 0)
					{
						var device = CrossDeviceInfo.Current;
						a = new Aparelho();
						a.ID_APARELHO = device.Id;
						a.DSCAPAR = device.DeviceName;
						a.NOMOPER = new OperadorController().GetOperador().USROPER;
						a.IDTPESS = new EmpresaController().GetEmpresa().NROCNPJ;
						a.NROVERS = "0";
						a.TIPSAPAR = "2";
						a.INDINAT = "0";
						a.DTHULTAT = DateTime.Now;
						a.USRULTAT = new OperadorController().GetOperador().USROPER;

						if (new AparelhoController().Insert(a))
							new AparelhoController().syncDevice(a);
					}
					else
					{
						a = new AparelhoController().GetAparelho();
						if (!a.INDSYNC)
							new AparelhoController().syncDevice(a);
					}

					if (a.INDSYNC)
					{
						bool permission = new ConfigController().applicationPermission(out string error);
						if (permission)
						{
							config.isAuthorized = true;
							config.DTHULTVER = DateTime.Now;
							cController.Save(config);
						}
						else
						{
							context.RunOnUiThread(() =>
							{
								Android.Support.V7.App.AlertDialog.Builder bd = new Android.Support.V7.App.AlertDialog.Builder(context);
								bd.SetTitle("AVISO DO SISTEMA !");
								bd.SetMessage("SISTEMA ATUALMENTE BLOQUEADO \n" + error + "\n" + "CONTATE O ADMINISTRADOR DO SISTEMA");
								bd.SetPositiveButton("OK", (s, a) =>
								{
									config.DSCRERRO = error;
									config.isAuthorized = false;
									cController.Save(config);
									context.Finish();
								});
								bd.SetCancelable(false);
								Android.Support.V7.App.AlertDialog alert = bd.Create();
								alert.Show();

							});
						}
					}
					else if (!config.isAuthorized)
					{
						context.RunOnUiThread(() =>
						{
							Android.Support.V7.App.AlertDialog.Builder bd = new Android.Support.V7.App.AlertDialog.Builder(context);
							bd.SetTitle("AVISO DO SISTEMA !");
							bd.SetMessage("SISTEMA ATUALMENTE BLOQUEADO \n" + "APARELHO NÃO CADASTRADO" + "\n" + "CONTATE O ADMINISTRADOR DO SISTEMA");
							bd.SetPositiveButton("OK", (s, a) =>
							{
								context.Finish();
							});
							bd.SetCancelable(false);
							Android.Support.V7.App.AlertDialog alert = bd.Create();
							alert.Show();

						});
					}
				}
			}
		}
	}
}