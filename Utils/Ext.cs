using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EloComandas.Utils
{
	public static class Ext 
	{
        public static long ToLong(this string value)
        {
            return long.TryParse(value, out long aux) ? aux : 0;
        }
        /// <summary>
        ///  Converte bytes com codificação UTF7 para string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string UTF7ToString(this byte[] bytes)
        {
            return Encoding.UTF7.GetString(bytes);
        }

        public static int ToInt(this string value)
        {
            return int.TryParse(value, out int aux) ? aux : 0;
        }

        public static short ToShort(this string value)
        {
            return short.TryParse(value, out short aux) ? aux : (short)0;
        }

        public static double ToDouble(this string value)
        {
            return double.TryParse(value, NumberStyles.AllowDecimalPoint, new CultureInfo("pt-BR"), out double aux) ? aux : 0;
        }

        public static bool ToBool(this string value)
        {
            return bool.TryParse(value, out bool aux) ? aux : false;
        }

        public static void Msg(this Android.App.Activity activity, string message)
		   => activity.RunOnUiThread(
			   () => Toast.MakeText(activity, message, ToastLength.Long).Show());

		public static bool IsEmpty(this string value) => string.IsNullOrEmpty(value);

		/// <summary>
		///  Converte a string para bytes com codificação UTF8
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] ToUTF8(this string value, bool RemoveAccents = false)
		{
			return Encoding.UTF8.GetBytes(RemoveAccents ? Format.RemoveAccents(value) : value);
		}
	}

}