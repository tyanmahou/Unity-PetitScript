using System;

namespace Petit.Runtime
{
    public class Reference
    {
        internal Reference(System.Func<Value> get, Action<Value> set)
        {
            _get = get;
            _set = set;
        }
        public Value Indirection
        {
            get => _get?.Invoke() ?? Value.Invalid;
            set => _set.Invoke(value);
        }

        readonly System.Func<Value> _get;
        readonly Action<Value> _set;
    }
}
