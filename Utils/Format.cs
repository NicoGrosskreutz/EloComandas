using System;
using System.Text.RegularExpressions;

namespace EloComandas.Utils
{
    public class Format
    {
        public static string RemoveAccents(string text)
        {
            if (text.IsEmpty() || text.IsEmpty())
                return text;

            var a = "[àáãâä]";
            var A = "[ÀÁÃÂÄ]";
            var e = "[èéêë]";
            var E = "[ÈÉÊË]";
            var i = "[ìíîï]";
            var I = "[ÌÍÎÏ]";
            var o = "[òóôõºö]";
            var O = "[ÒÓÔÕÖ]";
            var u = "[ùúûü]";
            var U = "[ÙÚÛÜ]";

            text = Regex.Replace(text, a, "a");
            text = Regex.Replace(text, A, "A");
            text = Regex.Replace(text, e, "e");
            text = Regex.Replace(text, E, "E");
            text = Regex.Replace(text, i, "i");
            text = Regex.Replace(text, I, "I");
            text = Regex.Replace(text, o, "o");
            text = Regex.Replace(text, O, "O");
            text = Regex.Replace(text, u, "u");
            text = Regex.Replace(text, U, "U");

            return text;
        }
    }
}