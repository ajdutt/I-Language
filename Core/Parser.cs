using System;
using System.Collections.Generic;

namespace Core
{
   using Parsen = Dictionary<string, object>;

   /// <summary>
   /// Parses the tokens made by the <see cref="Tokenizer"/>
   /// </summary>
   public class Parser
   {
      /// <summary>
      /// Input <see cref="Tokenizer"/>
      /// </summary>
      private Tokenizer _input = null;

      private static Dictionary<string, int> PRECENDENCE = new Dictionary<string, int>( )
      {
         { "=", 1 }, { "->", 1 },
         { "||", 2 },
         { "&&", 3 },
         { "<", 7 }, { ">", 7 }, { "<=", 7 }, { ">=", 7 },
         { "+", 10 }, { "-", 10 },
         { "*", 20 }, { "/", 20 }, { "%", 20 }
      };

      /// <summary>
      /// Default constructor
      /// </summary>
      public Parser( )
      { }

      /// <summary>
      /// Constructor. Creates an input <see cref="Tokenizer"/>
      /// </summary>
      /// <param name="path">Path to the file</param>
      public Parser(string path)
         => _input = new Tokenizer(path);

      private bool IsPunc(char? c)
      {
         var tok = _input.Peek( );
         return tok != null && (string)tok["type"] == "punc" && (c == null || (char)tok["value"] == c);
      }

      private bool IsKw(string kw)
      {
         var tok = _input.Peek( );
         return tok != null && (string)tok["type"] == "kw" && (kw == null || (string)tok["value"] == kw);
      }

      private bool IsOp(string op)
      {
         var tok = _input.Peek( );
         return tok != null && (string)tok["type"] == "op" && (op == null || (string)tok["value"] == op);
      }

      private void SkipPunc(char c)
      {
         if (IsPunc(c))
            _input.Next( );
         else
            throw new Exception( );
      }

      private void SkipKw(string kw)
      {
         if (IsKw(kw))
            _input.Next( );
         else
            throw new Exception( );
      }

      private void SkipOp(string op)
      {
         if (IsOp(op))
            _input.Next( );
         else
            throw new Exception( );
      }

      private Parsen MaybeBinary(Parsen left, int myPrec)
      {
         var tok = _input.Peek( );

         if (IsOp(null))
         {
            var hisPrec = PRECENDENCE[(string)tok["value"]];
            if (hisPrec > myPrec)
            {
               _input.Next( );
               return MaybeBinary(
                  new Parsen( )
                     {
                     { "type", (string)tok["value"] == "=" || (string)tok["value"] == "->" ? "assign" : "binary"},
                     { "oper", (string)tok["value"]},
                     { "left", left },
                     { "right", MaybeBinary(ParseAtom( ), hisPrec) } }, myPrec
                  );
            }
         }
         return left;
      }

      private List<object> Delimited(char start, char stop, char separator, Func<object> parser)
      {
         var a = new List<object>( );
         var first = true;

         SkipPunc(start);
         while (!_input.Eof( ))
         {
            if (IsPunc(stop))
               break;

            if (first)
               first = false;
            else
               SkipPunc(separator);

            if (IsPunc(stop))
               break;

            a.Add(parser( ));
         }
         SkipPunc(stop);
         return a;
      }

      private Parsen ParseCall(Func<object> func)
      {
         return new Parsen( )
         {
            { "type", "call" },
            { "func", func },
            { "args", Delimited('(', ')', ',', ParseExpr) }
         };
      }

      private string ParseVarname( )
      {
         var name = _input.Next( );
         if ((string)name["type"] != "var")
            throw new Exception( );
         return (string)name["value"];
      }

      private Parsen ParseIf( )
      {
         SkipKw("if");
         var cond = ParseExpr( );
         if (!IsPunc('{'))
            throw new Exception( );

         var then = ParseExpr( );
         var ret = new Parsen( )
         {
            { "type", "if" },
            { "cond", cond },
            { "then", then }
         };
         
         if (IsKw("else"))
         {
            _input.Next( );
            ret["else"] = ParseExpr( );
         }

         return ret;
      }

      private Parsen ParseDef( )
         => new Parsen( )
            {
               { "type", "def" },
               { "vars", Delimited('(', ')', ',', ParseVarname) },
               { "body", ParseExpr( ) }
            };

      private Parsen ParseBool( )
         => new Parsen( )
         {
            { "type", "bool" },
            { "value", (string)_input.Next()["value"] == "true" }
         };

      private Parsen MaybeCall(Func<Parsen> expr)
      {
         var e = expr( );
         return IsPunc('(') ? ParseCall(expr) : e;
      }

      private Parsen ParseAtom( )
         => MaybeCall(( ) =>
         {
            if (IsPunc('('))
            {
               _input.Next( );
               var exp = ParseExpr( );
               SkipPunc(')');
               return exp;
            }
            if (IsPunc('{'))
               return ParseProg( );

            if (IsKw("if"))
               return ParseIf( );

            if (IsKw("true") || IsKw("false"))
               return ParseBool( );

            if (IsKw("def"))
            {
               _input.Next( );
               return ParseDef( );
            }

            if (IsKw("int") || IsKw("int") || IsKw("int"))
         });
   }
}
