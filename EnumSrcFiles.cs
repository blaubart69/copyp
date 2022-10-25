namespace Spi;

using System.Threading.Tasks;
using System.IO;

using System.Collections.Generic;

class EnumSrcFiles
{
    public static async IAsyncEnumerable<FileInfo> Start(DirectoryInfo dir)
    {
        foreach ( var fi in dir.EnumerateFiles(
            "*",
            new EnumerationOptions () { 
                AttributesToSkip = 0,
                ReturnSpecialDirectories = false,
                RecurseSubdirectories = true }) )
        {
            yield return await Task.FromResult(fi);
        }
    }
}