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
      /// <summary>
      /// Current token
      /// </summary>
      private Token _curr = new Token( );
      /// <summary>
      /// <see cref="InputStream"/> for the file
      /// </summary>
      private InputStream _input = null;

      /// <summary>
      /// Keywords of our language
      /// </summary>
      private const string _kws =
         " if else char int long float double string def namesp class true false ";

      /// <summary>
      /// Check if the parameter is a keyword
      /// </summary>
      /// <param name="x">Possible keyword</param>
      /// <returns>Yes or no</returns>
      private bool IsKw(string x)
         => _kws.IndexOf($" {x} ") >= 0;

      /// <summary>
      /// Check if the character is a digit
      /// </summary>
      /// <param name="c">Character</param>
      /// <returns>Yes or no</returns>
      private bool IsDigit(string c)
         => Regex.IsMatch(c, "[0-9]");

      /// <summary>
      /// Check if the character is a letter or an underscore
      /// </summary>
      /// <param name="c">Character</param>
      /// <returns>Yes or no</returns>
      private bool IsIdStart(string c)
         => Regex.IsMatch(c, "[a-bA-B_]");

      /// <summary>
      /// Check if the character is a letter, a digit or an underscore
      /// </summary>
      /// <param name="c">Character</param>
      /// <returns>Yes or no</returns>
      private bool IsId(string c)
         => Regex.IsMatch(c, "[a-bA-B0-9_]");

      /// <summary>
      /// Check if the character is an operator
      /// </summary>
      /// <param name="c">Character</param>
      /// <returns>Yes or no</returns>
      private bool IsOper(string c)
         => Regex.IsMatch(c, "[+-*/%=&|<>!]");

      /// <summary>
      /// Check if the character is a punctuation
      /// </summary>
      /// <param name="c">Character</param>
      /// <returns>Yes or no</returns>
      private bool IsPunc(string c)
         => Regex.IsMatch(c, @"[,.;(){}\[\]]");

      /// <summary>
      /// Check if the character is a whitespace
      /// </summary>
      /// <param name="c">Character</param>
      /// <returns>Yes or no</returns>
      private bool IsWs(string c)
         => Regex.IsMatch(c, "[ \t\n]");

      /// <summary>
      /// Creates the string that fulfills the predicate function
      /// </summary>
      /// <param name="pred">Predicate function</param>
      /// <returns>A string that fulfills the predicate function</returns>
      private string RdWhile(Func<string, bool> pred)
      {
         string s = string.Empty;
         while (!_input.Eof( ) && pred(_input.Peek( ).ToString( )))
         {
            s += _input.Next( );
         }
         return s;
      }

      /// <summary>
      /// Finds a number
      /// </summary>
      /// <returns>Token of that number</returns>
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

      /// <summary>
      /// Finds an ID
      /// </summary>
      /// <returns>Token of that ID</returns>
      private Token RdId()
      {
         var id = RdWhile(IsId);

         var ret = new Token( );
         ret["type"] = IsKw(id) ? "kw" : "var";
         ret["value"] = id;

         return ret;
      }

      /// <summary>
      /// Reads the in-programming-language string
      /// </summary>
      /// <param name="end">End character</param>
      /// <returns>String</returns>
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
