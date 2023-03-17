using System.Diagnostics;
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
        String Result = "";
        var task = handler.Upload(filename, @in);

        try
        {
            task.ContinueWith(t =>
            {
                if (t.IsCompleted)
                {
                    Result = TaskEnd(t);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        catch (Exception ex)
        {
            Result = TaskEnd(task);
        }

        return Result;
    }

    private String TaskEnd(object task)
    {
        var t = task as Task<String?>;
        var content = t.Result;

        return (content == null ? "" : content);
    }

}