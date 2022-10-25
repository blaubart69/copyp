using System.Threading;
using System;
namespace Spi;

class Stats
{
    public UInt64 BytesCopied => _bytesCopied; 
    public UInt64 FilesCopied => _filesCopied;

    public UInt64 ErrorsCopying =>_errorsCopying;

    private UInt64 _bytesCopied;
    private UInt64 _filesCopied;
    private UInt64 _errorsCopying;
    public void IncrementFilesCopied(UInt64 filesize)
    {
        Interlocked.Increment(ref _filesCopied);
        Interlocked.Add(ref _bytesCopied, filesize);
    }
    public void IncrementErrorsCopying()
    {
        Interlocked.Increment(ref _errorsCopying);
    }
}