namespace Spi;

using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

class CopyP 
{
    static bool OpenFileForReading(string filename, out FileStream? fs)
    {
        try
        {
            fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read );
            return true;
        }
        catch (Exception ex)
        {
            fs = null;
            Console.Error.WriteLine($"E: open src: {filename} {ex.Message}");
            return false;
        }
    }
    static bool OpenFileForWriting(string filename, out FileStream? fs)
    {
        for(;;)
        {
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read );
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                fs = null;
                var dir = Path.GetDirectoryName(filename);
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {
                    continue;
                }
            }
        }
    }

    //public static async Task Run(ChannelReader<FileInfo> toCopy, CancellationToken cts)
    public static async Task Start(string srcBaseDir, string dstBaseDir, IAsyncEnumerable<string> srcFilesRelative, CancellationToken cts)
    {
        int srcBaseDirLength = srcBaseDir.EndsWith(Path.DirectorySeparatorChar) 
            ? srcBaseDir.Length
            : srcBaseDir.Length + 1;

        await Parallel.ForEachAsync(
            source: srcFilesRelative,
            parallelOptions : new ParallelOptions() { MaxDegreeOfParallelism = 16, CancellationToken = cts },
            body : async (string srcFilename, CancellationToken cts) =>
            {
                string relativeFilename = srcFilename.Substring(srcBaseDirLength);
                string dstFilename = System.IO.Path.Combine(dstBaseDir, relativeFilename );

                Console.WriteLine($"{srcFilename,-40} -> {dstFilename} ({relativeFilename})");

                     if ( !OpenFileForReading(srcFilename, out FileStream? srcStream))
                {
                }
                else if ( !OpenFileForWriting(dstFilename, out FileStream? dstStream))
                {
                }
                else if ( srcStream != null && dstStream != null )
                {
                    using (srcStream)
                    using (dstStream)
                    {
                        await srcStream.CopyToAsync(dstStream);
                    }
                }
            });
    }
}