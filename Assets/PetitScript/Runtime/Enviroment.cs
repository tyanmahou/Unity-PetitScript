namespace Petit
{
    class Enviroment
    {
        public Variables Variables
        {
            get => _variables;
            set => _variables = value;
        }
        Variables _variables = new();
    }
}
