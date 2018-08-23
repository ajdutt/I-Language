using System;
using System.IO;

namespace Core
{
   public class InputStream
   {
      private string _file = string.Empty;

      private int _pos = 0,
                  _col = 0, _ln = 1;

      public InputStream(string path)
      {
         if (File.Exists(path))
            _file = File.ReadAllText(path);
         else throw new FileNotFoundException("Could not find a file in path: " + path);
      }

      public char? Peek()
      {
         if (Eof( ))
            return null;
         return _file[_pos];
      }

      public char? Next()
      {
         var c = Peek( );
         ++_pos;
         return c;
      }

      public bool Eof( )
         => _pos >= _file.Length;

      public Exception Croak(string msg)
      {
         return new Exception($"{msg} ({_col}:{_ln}");
      }
   }
}
