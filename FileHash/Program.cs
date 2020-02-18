using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHash
{
    class Program
    {

        static void Main(string[] args)
        {
            IFileHash fileHash = new FileHash();
            string result;

            result = fileHash.getFilenameAsHash("hello.png");
            Console.WriteLine(result);

            fileHash.setFileHash("bye.pdf");
            fileHash.setFileHash("dog.bmp");
            Console.WriteLine(fileHash.isFileHashed("bye.pdf"));
            Console.WriteLine(fileHash.isFileHashed("wolf.pdf"));
            Console.WriteLine(fileHash.isFileHashed("cat.bmp"));
            Console.WriteLine(fileHash.isFileHashed("dog.bmp"));

            Console.WriteLine(fileHash.getFilenameAsHash("cat.bmp"));
            Console.WriteLine(fileHash.getFilenameAsHash("dog.bmp"));

            fileHash.removeFileHash("cat.bmp");
            fileHash.removeFileHash("dog.bmp");
        }
    }
}
