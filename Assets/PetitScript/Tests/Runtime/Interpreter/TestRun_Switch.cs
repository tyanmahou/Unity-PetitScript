using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_Switch : TestRunBase
    {
        [Test]
        public void TestSwitch()
        {
            string code = @"
text="""";
switch(a) {
case 0:
   text += ""0"";
case  1:
   text += ""1"";
case  2:
case  3:
   text += ""23"";
　break;    
default:
   text += ""d"";
case  4:
   text += ""4"";
   break;
}
return text;
";
            {
                var env = Environment.New();
                env["a"] = 0;
                RunString(code, "0123", env);
            }
            {
                var env = Environment.New();
                env["a"] = 1;
                RunString(code, "123", env);
            }
            {
                var env = Environment.New();
                env["a"] = 2;
                RunString(code, "23", env);
            }
            {
                var env = Environment.New();
                env["a"] = 3;
                RunString(code, "23", env);
            }
            {
                var env = Environment.New();
                env["a"] = 4;
                RunString(code, "4", env);
            }
            {
                var env = Environment.New();
                env["a"] = 5;
                RunString(code, "d4", env);
            }
        }
    }
}
