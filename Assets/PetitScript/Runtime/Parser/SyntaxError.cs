namespace Petit.Parser
{
    /// <summary>
    /// 構文エラー
    /// </summary>
    readonly struct SyntaxError
    {
        public SyntaxError(string message, int line, int column)
        {
            Message = message;
            Line = line;
            Column = column;
        }
        public readonly string Message;
        public readonly int Line;
        public readonly int Column;

        public override string ToString() 
        {
            return $"{Message} (line:{Line}, {Column})";
        }
    }
}
