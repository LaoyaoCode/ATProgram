using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailDisposeConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string result;
            Console.WriteLine(result = ConfigurationData.ReadConfigurationData());
            MailDispose.SendVerifyMail("2471129157@qq.com", "Laoyao", 6666);
            Console.Read();
        }
    }
}
