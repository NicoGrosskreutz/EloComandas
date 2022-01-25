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
using FluentFTP;
using Android.Util;
using PCLExt;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using System.IO;
using EloComandas.Entites;
using EloComandas.Controller;
using Xamarin.Essentials;
using Android.Content.PM;

namespace EloComandas.Controller
{
	public class FTPController
	{
		public bool updateApp()
		{
			bool result = false;
			ConfigController cController = new ConfigController();
			FtpClient client = new FtpClient();
			DNS dns = cController.GetDNS();
			FileInfo fileInfo;

			try
			{
				if (cController.TestServerConnection("elosoftware.dyndns.org", 8560))
				{
					client.Host = "elosoftware.dyndns.org";
					client.Credentials = new System.Net.NetworkCredential("usuario", "penasoft");
					client.Connect();

					string localPath = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloComandas.EloComandas.apk";
					string remotePath = "/AtServidor/EloComandas.EloComandas.apk";
					client.DownloadFile(localPath, remotePath);

					result = true;
				}

			}
			catch (FtpException e)
			{
				Log.Error("FTP_ERROR", e.ToString());
			}
			finally
			{
				client.Disconnect();
			}
			return result;
		}

		//public string getCurrentVerson()
		//{
		//	ConfigController cController = new ConfigController();
		//	FtpClient client = new FtpClient();
		//	DNS dns = cController.GetDNS();
		//	string data = "";
		//	try
		//	{
		//		if (cController.TestServerConnection("elosoftware.dyndns.org", 8560))
		//		{
		//			client.Host = "elosoftware.dyndns.org";
		//			client.Credentials = new System.Net.NetworkCredential("usuario", "penasoft");
		//			client.Connect();

		//			data = client.GetModifiedTime("/AtServidor/EloComandas.EloComandas.apk").ToString("dd/MM/yyyy HH:mm:ss");
		//		}

		//	}
		//	catch (Exception e)
		//	{
		//		Log.Error("FTP_ERROR", e.ToString());
		//	}
		//	finally
		//	{
		//		client.Disconnect();
		//	}
		//	return data;
		//}

		//public string getAppVersion()
		//{
		//	FileInfo fileInfo;
		//	string data = "";
		//	string path = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloComandas.EloComandas.apk";
		//	try
		//	{
		//		fileInfo = new FileInfo(path);
		//		data = fileInfo.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss");
		//	}
		//	catch (Exception e)
		//	{
		//		Log.Error("FILE_ERROR", e.ToString());
		//	}
		//	return data;
		//}

		public string getCurrentVersion()
		{
			ConfigController cController = new ConfigController();
			FtpClient client = new FtpClient();
			DNS dns = cController.GetDNS();
			string data = "";
			try
			{
				client.Host = "elosoftware.dyndns.org";
				client.Credentials = new System.Net.NetworkCredential("usuario", "penasoft");
				client.Connect();

				data = client.GetModifiedTime("/AtServidor/EloComandas.EloComandas.apk").ToString("dd/MM/yyyy HH:mm:ss");
			}
			catch (Exception e)
			{
				Log.Error("FTP_ERROR", e.ToString());
			}
			finally
			{
				client.Disconnect();
			}
			return data;
		}

		public string getApkVersion()
		{
			FileInfo fileInfo;
			string data = "";
			string path = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "EloSoftware/EloComandas.EloComandas.apk";
			try
			{
				fileInfo = new FileInfo(path);
				data = fileInfo.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss");
			}
			catch (Exception e)
			{
				Log.Error("FILE_ERROR", e.ToString());
			}
			return data;
		}

		public static DateTime getAppVersion(out string version)
		{
			try
			{
				VersionTracking.Track();

				PackageInfo info = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0);
				long unixDate = info.LastUpdateTime;
				DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				DateTime date = start.AddMilliseconds(unixDate).ToLocalTime();

				version = VersionTracking.CurrentVersion;
				return date;
			}
			catch
			{
				version = string.Empty;
				return new DateTime();
			}
		}
	}
}