using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EloComandas.Controller;
using EloComandas.Entites;
using EloComandas.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EloComandas.View
{
    [Activity(Label = "ConfigView")]
    public class ConfigView : Activity
    {
        private LinearLayout secret;
        private EditText txDNSInterno, txDNSExterno;
        private SwitchCompat swDNS;
        private RelativeLayout RLayout;
        private Button btnTest, btnlogout, btnFechar, btnSalvar;
        private TextView lbTest;
        private ImageButton btnReturn;

        private int Count = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_config);
            LoadView();

            btnReturn.Click += (s, a) => Finish();

            secret.Click += (s, a) =>
            {
                if (Count == 0)
                    Task.Run(() => { Thread.Sleep(TimeSpan.FromSeconds(1.5)); Count = 0; });

                Count++;

                if (Count == 5)
                    RLayout.Visibility = ViewStates.Visible;
            };

            btnFechar.Click += (s, a) => RLayout.Visibility = ViewStates.Invisible;

            btnlogout.Click += (s, a) =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("AVISO DO SISTEMA");
                builder.SetMessage("CONFIRMA A REINICIALIZAÇÃO DO APLICATIVO ?");
                builder.SetPositiveButton("SIM", (s, e) =>
                {
                    Database.GetConnection().RunInTransaction(() =>
                    {
                        Intent i = new Intent(this, typeof(LoginView));
                        StartActivity(i);
                    });
                });
                builder.SetNegativeButton("CANCELAR", (s, e) => { return; });
                AlertDialog dialog = builder.Create();
                dialog.Show();
            };

            btnTest.Click += (s, a) =>
            {
                lbTest.Text = "";
                lbTest.Visibility = Android.Views.ViewStates.Visible;

                string host = "";
                string port = "";

                if (swDNS.Checked)
                {
                    if (!string.IsNullOrEmpty(txDNSExterno.Text))
                    {
                        if (txDNSExterno.Text.Contains(":"))
                        {
                            host = txDNSExterno.Text.Split(":")[0];
                            port = txDNSExterno.Text.Split(":")[1];
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(txDNSInterno.Text))
                        if (txDNSInterno.Text.Contains(":"))
                        {
                            host = txDNSInterno.Text.Split(":")[0];
                            port = txDNSInterno.Text.Split(":")[1];
                        }
                }

                if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port))
                {

                    if (new ConfigController().TestServerConnection(host, int.Parse(port)))
                    {
                        lbTest.Text = "OK";
                        lbTest.SetTextColor(Android.Graphics.Color.ParseColor("#005500"));
                    }
                    else
                    {
                        lbTest.Text = "SEM CONEXÃO";
                        lbTest.SetTextColor(Android.Graphics.Color.ParseColor("#550000"));
                    }
                }
                else
                {
                    lbTest.Text = "SEM CONEXÃO";
                    lbTest.SetTextColor(Android.Graphics.Color.ParseColor("#550000"));
                }
            };

            btnSalvar.Click += (s, a) =>
            {
                string host = "";
                string port = "";

                if (swDNS.Checked)
                {
                    if (txDNSExterno.Text != "" && txDNSExterno.Text.Contains(":"))
                    {
                        host = txDNSExterno.Text.Split(":")[0];
                        port = txDNSExterno.Text.Split(":")[1];
                    }
                }
                else
                {
                    if (txDNSInterno.Text != "" && txDNSInterno.Text.Contains(":"))
                    {
                        host = txDNSInterno.Text.Split(":")[0];
                        port = txDNSInterno.Text.Split(":")[1];
                    }
                }

                if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port))
                    if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(port))
                    {
                        DNS dns = new DNS();
                        dns.Host = host;
                        dns.Port = int.Parse(port);

                        Config config = new ConfigController().GetConfig();
                        if (swDNS.Checked)
                        {
                            config.DNSEXT = $"{dns.Host}:{dns.Port}";
                            config.INDDNS = true;
                        }
                        else
                        {
                            config.DNSINT = $"{dns.Host}:{dns.Port}";
                            config.INDDNS = false;
                        }
                        if (new ConfigController().Save(config))
                            Toast.MakeText(this, "SALVO COM SUCESSO !", ToastLength.Long).Show();
                    }
            };
        }
        public void LoadView()
        {
            secret = FindViewById<LinearLayout>(Resource.Id.linearLayout);
            RLayout = FindViewById<RelativeLayout>(Resource.Id.relativeLayout);
            txDNSInterno = FindViewById<EditText>(Resource.Id.txDNSInterno);
            txDNSExterno = FindViewById<EditText>(Resource.Id.txDNSExterno);
            swDNS = FindViewById<SwitchCompat>(Resource.Id.swDNS);
            btnTest = FindViewById<Button>(Resource.Id.btnTest);
            btnlogout = FindViewById<Button>(Resource.Id.btnlogout);
            btnFechar = FindViewById<Button>(Resource.Id.btnFechar);
            btnSalvar = FindViewById<Button>(Resource.Id.btnSalvar);
            lbTest = FindViewById<TextView>(Resource.Id.lbTest);
            btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);

            RLayout.Visibility = ViewStates.Invisible;

            Config config = new ConfigController().GetConfig();
            if (config != null)
            {
                txDNSExterno.Text = config.DNSEXT;
                txDNSInterno.Text = config.DNSINT;
                if (config.INDDNS)
                    swDNS.Checked = true;
                else
                    swDNS.Checked = false;
            }

        }

    }
}