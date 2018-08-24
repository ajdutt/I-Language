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
      private bool IsKw(string x)
         => _kws.IndexOf($" {x} ") >= 0;

      /// <summary>
      /// Check if the character is a digit
      /// </summary>
      /// <param name="c">Character</param>
      private bool IsDigit(string c)
         => Regex.IsMatch(c, "[0-9]");

      /// <summary>
      /// Check if the character is a letter or an underscore
      /// </summary>
      /// <param name="c">Character</param>
      private bool IsIdStart(string c)
         => Regex.IsMatch(c, "[a-bA-B_]");

      /// <summary>
      /// Check if the character is a letter, a digit or an underscore
      /// </summary>
      /// <param name="c">Character</param>
      private bool IsId(string c)
         => Regex.IsMatch(c, "[a-bA-B0-9_]");

      /// <summary>
      /// Check if the character is an operator
      /// </summary>
      /// <param name="c">Character</param>
      private bool IsOper(string c)
         => Regex.IsMatch(c, "[+-*/%=&|<>!]");

      /// <summary>
      /// Check if the character is a punctuation
      /// </summary>
      /// <param name="c">Character</param>
      private bool IsPunc(string c)
         => Regex.IsMatch(c, @"[,.;(){}\[\]]");

      /// <summary>
      /// Check if the character is a whitespace
      /// </summary>
      /// <param name="c">Character</param>
      private bool IsWs(string c)
         => Regex.IsMatch(c, "[ \t\n]");

      /// <summary>
      /// Creates the string that fulfills the predicate function
      /// </summary>
      /// <param name="pred">Predicate function</param>
      /// <returns>A string that fulfills the predicate function</returns>
      private string RdWhile(Func<string, bool> pred)
      {
         var s = string.Empty;
         while (!_input.Eof( ) && pred(_input.Peek( ).ToString( )))
         {
            s += _input.Next( );
         }
         return s;
      }

      /// <summary>
      /// Deafult constructor
      /// </summary>
      public Tokenizer( )
      { }

      /// <summary>
      /// Constructor. Initializes the <see cref="InputStream"/>
      /// </summary>
      /// <param name="path"></param>
      public Tokenizer(string path)
         => _input = new InputStream(path);

      /// <summary>
      /// Reads a number
      /// </summary>
      /// <returns>Token of that number</returns>
      private Token RdNum()
      {
         var dot = false;
         var num = RdWhile(
            (c) =>
         {
            if (c == ".")
            {
               if (dot)
                  return false;
               dot = true;
               return true;
            }
            return IsDigit(c);
         });

         return new Token( )
         {
            { "type", "num" },
            { "value", num }
         };
      }

      /// <summary>
      /// Reads an ID
      /// </summary>
      /// <returns>Token of that ID</returns>
      private Token RdId()
      {
         var id = RdWhile(IsId);

         return new Token( )
         {
            { "type", IsKw(id) ? "kw" : "var" },
            { "value", id }
         };
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

      /// <summary>
      /// Reads a string
      /// </summary>
      /// <returns>Token of that string</returns>
      private Token RdStr( )
         => new Token( )
            {
               { "type", "str" },
               { "value", RdEsc('"') }
            };

      /// <summary>
      /// Reads a character
      /// </summary>
      /// <returns>Token of that character</returns>
      private Token RdChar( )
         => new Token( )
            {
               { "type", "char" },
               { "value", RdEsc('\'') }
            };

      /// <summary>
      /// Skips comment
      /// </summary>
      private void SkipComm( )
      {
         RdWhile((c) => c != "\n");
         _input.Next( );
      }

      /// <summary>
      /// Core function of the <see cref="Tokenizer"/>
      /// </summary>
      /// <returns>Proper token</returns>
      private Token RdNext( )
      {
         RdWhile(IsWs);
         if (_input.Eof( ))
            return null;

         var c = _input.Peek( );
         if (c == '#')
         {
            SkipComm( );
            return RdNext( );
         }
         if (c == '"')
            return RdStr( );
         if (IsDigit(c.ToString( )))
            return RdNum( );
         if (IsIdStart(c.ToString( )))
            return RdId( );
         if (IsPunc(c.ToString( )))
            return new Token( )
            {
               { "type", "punc" },
               { "value", _input.Next( ) }
            };
         if (IsOper(c.ToString( )))
            return new Token( )
            {
               { "type", "oper" },
               { "value", RdWhile(IsOper) }
            };

         throw _input.Croak("Cannot handle character: " + c);
      }

      /// <summary>
      /// Returns the current token
      /// </summary>
      public Token Peek( )
      {
         if (_curr == null)
            _curr = RdNext( );
         return _curr;
      }

      /// <summary>
      /// Moves to the next token
      /// </summary>
      /// <returns>Last token</returns>
      public Token Next( )
      {
         var t = _curr;
         _curr = null;
         return t;
      }

      /// <summary>
      /// Indicates if there is no more tokens
      /// </summary>
      public bool Eof( )
         => Peek( ) == null;
   }
}
