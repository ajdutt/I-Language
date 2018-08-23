using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core
{
   using Token = Dictionary<string, object>;

   /// <summary>
   /// Creates tokens using <see cref="InputStream"/>
   /// </summary>
   public class Tokenizer
   {
      private Token _curr = new Token( );
      private InputStream _input = null;

      private const string _kws =
         " if else char int long float double string def namesp class true false ";

      private bool IsKw(string x)
         => _kws.IndexOf($" {x} ") >= 0;

      private bool IsDigit(string c)
         => Regex.IsMatch(c, "[0-9]");

      private bool IsIdStart(string c)
         => Regex.IsMatch(c, "[a-bA-B_]");

      private bool IsId(string c)
         => Regex.IsMatch(c, "[a-bA-B0-9_]");

      private bool IsOper(string c)
         => Regex.IsMatch(c, "[+-*/%=&|<>!]");

      private bool IsPunc(string c)
         => Regex.IsMatch(c, @"[,.;(){}\[\]]");

      private bool IsWs(string c)
         => Regex.IsMatch(c, "[ \t\n]");

      private string RdWhile(Func<string, bool> pred)
      {
         string s = string.Empty;
         while (!_input.Eof( ) && pred(_input.Peek( ).ToString( )))
         {
            s += _input.Next( );
         }
         return s;
      }

      private Token RdNum()
      {
         bool dot = false;
         Func<string, bool> f = (c) =>
         {
            if (c == ".")
            {
               if (dot)
                  return false;
               dot = true;
               return true;
            }
            return IsDigit(c);
         };
         string num = RdWhile(f);

         var ret = new Token( );
         ret["type"] = "num";
         ret["value"] = num;

         return ret;
      }

      private Token RdId()
      {
         var id = RdWhile(IsId);

         var ret = new Token( );
         ret["type"] = IsKw(id) ? "kw" : "var";
         ret["value"] = id;

         return ret;
      }

      private string RdEsc(char end)
      {
         var esc = false;
         var str = string.Empty;
         _input.Next( );
         while (!_input.Eof( ))
         {
            var c = _input.Next( );
            if (esc)
            {
               str += c;
               esc = false;
            }

            else if (c == '\\')
               esc = true;

            else if (c == end)
               break;

            else
               str += c;
         }
         return str;
      }


   }
}
