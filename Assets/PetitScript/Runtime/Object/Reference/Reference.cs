
namespace Petit.Runtime
{
    public class Reference
    {
        public Reference(Value value)
        {
            _value = value;
        }
        public Value Indirection
        {
            get => _value;
            set => _value = value;
        }

        Value _value;
    }
}
