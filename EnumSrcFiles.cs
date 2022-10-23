namespace Spi;

using System.Threading.Tasks;
using System.IO;

using System.Collections.Generic;
using System.Threading.Tasks;

class EnumSrcFiles
{
    public static async IAsyncEnumerable<string> Start(string rootdir)
    {
        foreach ( string filename in Directory.EnumerateFiles(
            rootdir, 
            "*", 
            new EnumerationOptions () { 
                AttributesToSkip = 0,
                ReturnSpecialDirectories = false,
                RecurseSubdirectories = true }) )
        {
            yield return await Task.FromResult(filename);
        }
    }
}