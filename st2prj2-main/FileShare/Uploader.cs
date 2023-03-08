using System.Reflection.Metadata;

namespace FileShare;

public class Uploader
{
    private StorageHandler handler;

    public Uploader(string groupNum)
    {
        handler = new StorageHandler(groupNum);
    }

    public string Save(string filename, FileStream @in)
    {
        return handler.Upload(filename, @in).Result;
    }
    
}