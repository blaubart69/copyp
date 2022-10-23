using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System;

namespace spi;

class Program
{
    public static int Main(string[] args)
    {
        string src = args[0];
        string dst = args[1];

        if ( ! Directory.Exists(src) )
        {
            Console.Error.WriteLine($"src directory does not exists. {src}");
            return 8;
        }
 
        CancellationTokenSource cts = new CancellationTokenSource();
        var filesToCopy = Spi.EnumSrcFiles.Start(src);
        var CopyTask =  Spi.CopyP.Start(src, dst, filesToCopy, cts.Token);
        
        while ( ! CopyTask.Wait( TimeSpan.FromSeconds(1) ) )
        {
            
        }

        return 0;
    }
}

