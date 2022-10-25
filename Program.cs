using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System;

namespace Spi;

class Program
{
    public static int Main(string[] args)
    {
        string src = args[0];
        string dst = args[1];

        DirectoryInfo srcDir = new DirectoryInfo(src);

        if ( ! Directory.Exists(src) )
        {
            Console.Error.WriteLine($"src directory does not exists. {src}");
            return 8;
        }
 
        CancellationTokenSource cts = new CancellationTokenSource();
        Stats stats = new Stats();
        var filesToCopy = Spi.EnumSrcFiles.Start(new DirectoryInfo(src));
        var CopyTask =  Spi.CopyP.Start(src, dst, filesToCopy, stats, cts.Token);
        
        while ( ! CopyTask.Wait( TimeSpan.FromSeconds(1) ) )
        {
            Console.WriteLine($"copied: files {stats.FilesCopied}, bytes {stats.BytesCopied}, errors {stats.ErrorsCopying}");
        }

        return 0;
    }
}

