using System;
using System.IO;

namespace Core
{
   /// <summary>
   /// Allows to move across the file
   /// </summary>
   public class InputStream
   {
      /// <summary>
      /// String, where the file's content is stored
      /// </summary>
      private string _file = string.Empty;

      /// <summary>
      /// Position of the current character
      /// </summary>
      private int _pos = 0,
                  _col = 0, _ln = 1;

      /// <summary>
      /// Default constructor
      /// </summary>
      public InputStream()
      { }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="path">Path to the file</param>
      public InputStream(string path)
      {
         if (File.Exists(path))
            _file = File.ReadAllText(path);
         else throw new FileNotFoundException("Could not find a file in path: " + path);
      }

      /// <summary>
      /// Peek the current character
      /// </summary>
      /// <returns>Current character</returns>
      public char? Peek()
      {
         if (Eof( ))
            return null;
         return _file[_pos];
      }

      /// <summary>
      /// Moves to next character
      /// </summary>
      /// <returns>Last character</returns>
      public char? Next()
      {
         var c = Peek( );
         ++_pos;
         return c;
      }

      /// <summary>
      /// Indicates if there is no more characters in the file
      /// </summary>
      public bool Eof( )
         => _pos >= _file.Length;

      /// <summary>
      /// Exception to be thrown. Adds the position of the error
      /// </summary>
      /// <param name="msg">Message of the exception</param>
      /// <returns>Exception</returns>
      public Exception Croak(string msg)
      {
         return new Exception($"{msg} ({_col}:{_ln}");
      }
   }
}
