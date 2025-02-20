namespace Petit.Core.Exception
{
    /// <summary>
    /// 実行エラー
    /// </summary>
    public class RuntimeErrorException : System.Exception
    {
        public RuntimeErrorException(string message)
            : base(message)
        {
        }
    }
}
