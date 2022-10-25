namespace Spi;

using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

class CopyP 
{
    static FileStream? OpenFileForReading(string filename)
    {
        try
        {
            return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read );
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"E: open src: {filename} {ex.Message}");
            return null;
        }
    }
    static async ValueTask<FileStream?> OpenFileForWriting(string filename)
    {
        string? dir = Path.GetDirectoryName(filename);
        if ( dir == null)
        {
            Console.Error.WriteLine($"directory is null or root for filename: [{filename}]");
            return null;
        }

        Exception? lastEx = null;

        for(int i=0; i < 3; ++i)
        {
            try
            {
                return new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                    lastEx = ex;
                    continue;
                }
            }
        }

        Console.Error.WriteLine($"E: directory could not be created. {dir} exeption: {lastEx?.Message}");
        return null;
    }

    public static async Task Start(string srcBaseDir, string dstBaseDir, IAsyncEnumerable<FileInfo> srcFiles, Stats stats, CancellationToken cts)
    {
        int srcBaseDirLength = srcBaseDir.EndsWith(Path.DirectorySeparatorChar) 
            ? srcBaseDir.Length
            : srcBaseDir.Length + 1;

        await Parallel.ForEachAsync(
            source: srcFiles,
            parallelOptions : new ParallelOptions() { MaxDegreeOfParallelism = 16, CancellationToken = cts },
            body : async (FileInfo srcFile, CancellationToken cts) =>
            {
                string relativeFilename = srcFile.FullName.Substring(srcBaseDirLength);
                string dstFilename = System.IO.Path.Combine(dstBaseDir, relativeFilename );

                Console.WriteLine($"{srcFile.FullName,-40} -> {dstFilename} ({relativeFilename})");

                FileStream? srcStream;
                FileStream? dstStream;
                     if ( (srcStream =       OpenFileForReading(srcFile.FullName)) == null ) {}
                else if ( (dstStream = await OpenFileForWriting(dstFilename))      == null)  {}
                else 
                {
                    using (srcStream)
                    using (dstStream)
                    {
                        try
                        {
                            await srcStream.CopyToAsync(dstStream,cts).ConfigureAwait(false);
                            stats.IncrementFilesCopied((UInt64)srcFile.Length);
                        }
                        catch (Exception ex)
                        {
                            stats.IncrementErrorsCopying();
                            Console.Error.WriteLine($"E: copy file. src {srcFile.FullName} -> {dstFilename}");
                        }
                        
                    }
                }
            });
    }
}