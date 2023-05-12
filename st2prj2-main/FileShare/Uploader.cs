namespace FileShare;

public class Uploader
{
    private readonly StorageHandler handler;

    public Uploader(string groupNum)
    {
        handler = new StorageHandler(groupNum);
    }

    public string Save(string filename, FileStream @in)
    {
        String result = "";
        // var task = handler.Upload(filename, @in);

        var task = handler.Upload(filename, @in);
        Task.Run(() =>
        {
            try
            {
                result = TaskEnd(task);
            } 
            catch (Exception)
            {
                result = TaskEnd(task);
            }
        }).Wait();
        
        return result;
    }

    private string TaskEnd(object task)
    {
        var t = task as Task<string?>;
        var content = t?.Result;

        return content ?? "";
    }

}