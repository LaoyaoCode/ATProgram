using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            FTPDispose.UploadFile("TestImage.jpg", "UploadImage.jpg",(double progress , FTPDispose.DownLoadOrUplaodState state)=>
                {
                    Console.WriteLine("Progress : {0} , state : {1}" , progress  , state);
                });
            */

            FTPDispose.DownLoadFile("TestTxt.txt", "test.txt");
            Console.WriteLine("DownLoad Finished!");
            Console.Read();
        }
    }
}
