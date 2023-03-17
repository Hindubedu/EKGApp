using System.Diagnostics;
using System.IO;

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
        Task.Run(() =>
        {
            var task = _handler.Download(filename);
            TaskEnd(task, @out);
        }).Wait();
        
    }

    private void TaskEnd(object task, FileStream @out)
    {
        var t = task as Task<String?>;
        var content = t.Result;
        var streamWriter = new StreamWriter(@out);
        streamWriter.Write(content);
        streamWriter.Flush();
        streamWriter.Close();
    }
 
}