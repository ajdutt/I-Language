namespace Core
{
   /// <summary>
   /// Parses the tokens made by the <see cref="Tokenizer"/>
   /// </summary>
   public class Parser
   {
      private Tokenizer _input = null;

      /// <summary>
      /// Default constructor
      /// </summary>
      public Parser( )
      { }

      public Parser(string path)
         => _input = new Tokenizer(path);

      
   }
}
