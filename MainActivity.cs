using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using EloComandas.Controller;
using Android.Content;
using EloComandas.View;
using Android.Views;
using AlertDialog = Android.App.AlertDialog;
using System;
using EloComandas.Persistence;

namespace EloComandas
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
		private Button btnlogout;

		private System.Timers.Timer timer = null;

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

			btnlogout = FindViewById<Button>(Resource.Id.btnlogout);
			btnlogout.Visibility = ViewStates.Invisible;

			btnlogout.Click += (sender, args) =>
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("AVISO");
				builder.SetMessage("GOSTARIA DE REINICIAR O SISTEMA ? \n(TODOS OS DADOS SERÃO PERDIDOS !)");
				builder.SetPositiveButton("SIM", (s, a) =>
				{
					Database.Drop();

					Intent i = new Intent(this, typeof(LoginView));
					StartActivity(i);
					Finish();
				});
				builder.SetNegativeButton("CANCELAR", (s, a) => { return; });
				AlertDialog dialog = builder.Create();
				dialog.Show();

			};


			timer = new System.Timers.Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
			timer.Elapsed += (s, a) =>
			{
				endSplashView();
				timer.Stop();
			};
			timer.Enabled = true;
			timer.AutoReset = false;
			timer.Start();



		}
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


		private void endSplashView()
		{
			if (new ConfigController().GetConfig() != null)
			{
				Intent i = new Intent(Application.Context, typeof(PedidosView));
				StartActivity(i);
				Finish();
			}
			else
			{
				Intent i = new Intent(Application.Context, typeof(LoginView));
				StartActivity(i);
				Finish();
			}


			//if (new ConfigController().GetConfig() != null)
			//{
			//	if (new ConfigController().TestServerConnection("elosoftware.dyndns.org", 8560))
			//	{
			//		if (new AparelhoController().FindAll().Count == 0)
			//		{
			//			var device = CrossDeviceInfo.Current;
			//			CG_APARELHO a = new CG_APARELHO();
			//			a.DSCAPAR = device.DeviceName;
			//			a.NOMOPER = new OperadorController().GetOperador().NOMOPER;
			//			a.IDTPESS = new EmpresaController().GetEmpresa().NROCNPJ;
			//			a.NROVERS = "0";
			//			a.TIPSAPAR = "3";
			//			a.INDINAT = "0";
			//			a.DTHULTAT = DateTime.Now;
			//			a.USRULTAT = new OperadorController().GetOperador().NOMOPER;

			//			if (new AparelhoController().Insert(a))
			//				new AparelhoController().syncDevice(a);
			//		}
			//		else
			//		{
			//			CG_APARELHO aparelho = new AparelhoController().GetAparelho();
			//			if (!aparelho.INDSYNC)
			//				new AparelhoController().syncDevice(aparelho);
			//		}

			//		bool permission = new ConfigController().applicationPermission();
			//		if (permission)
			//		{
			//			Intent i = new Intent(Application.Context, typeof(Views.PedidoView));
			//			StartActivity(i);
			//			Finish();
			//		}
			//		else
			//		{
			//			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			//			builder.SetTitle("AVISO DO SISTEMA !");
			//			builder.SetMessage("SISTEMA ATUALMENTE BLOQUEADO \nCONTATE O ADMINISTRADOR DO SISTEMA");
			//			builder.SetNeutralButton("OK", (sender, args) => { return; });
			//			AlertDialog dialog = builder.Create();
			//			dialog.Show();

			//			btnlogout.Visibility = ViewStates.Visible;
			//		}
			//	}
			//	else
			//	{
			//		Intent i = new Intent(Application.Context, typeof(Views.PedidoView));
			//		StartActivity(i);
			//		Finish();
			//	}
			//}
			////Senão sincroniza dados atravéz desta intent
			//else
			//{
			//	Intent i = new Intent(Application.Context, typeof(Views.LoginView));
			//	StartActivity(i);
			//	Finish();
			//}
		}
	}
}