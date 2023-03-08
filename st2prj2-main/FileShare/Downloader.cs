namespace FileShare;

public class Downloader
{
    private StorageHandler _handler;

    public Downloader(string groupNum)
    {
        _handler = new StorageHandler(groupNum);
    }

    public void Load(string filename, FileStream @out)
    {
        var task = _handler.Download(filename);
        var content = task.Result;

        using var streamWriter = new StreamWriter(@out);
        streamWriter.Write(content);
    }
}