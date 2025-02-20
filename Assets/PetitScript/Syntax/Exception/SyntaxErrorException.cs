namespace Petit.Syntax.Exception
{
    /// <summary>
    /// 構文エラー
    /// </summary>
    public class SyntaxErrorException : System.Exception
    {
        public SyntaxErrorException(string message, int line, int column)
            : base(message)
        {
            Line = line;
            Column = column;
        }
        public readonly int Line;
        public readonly int Column;

        public override string Message => $"{base.Message} (line:{Line}, {Column})";
    }
}
