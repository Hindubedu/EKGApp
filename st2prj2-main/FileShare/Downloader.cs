using System.Text.Json;

namespace FileShare;

public class Downloader
{
    private readonly StorageHandler _handler;
    private readonly string _url;

    public Downloader(string groupNum)
    {
        _url = "https://us-central1-st2prj2-fileshare.cloudfunctions.net/files?group=" + groupNum;
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

    public List<string> GetFilenames()
    {
        List<string> filenames = new List<string>();
        Task.Run(async () =>
        {
            try
            {
                var httpClient = new HttpClient();
                var data = await httpClient.GetStringAsync(_url);
                var files = JsonSerializer.Deserialize<List<File>>(data) ?? new List<File>(); 
                filenames = files
                    .Where(f => f.Filename != null)
                    .Select(f => f.Filename!)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).Wait();

        return filenames;
    }
    
    

    private void TaskEnd(object task, FileStream @out)
    {
        var t = task as Task<string?>;
        var content = t?.Result;
        var streamWriter = new StreamWriter(@out);
        streamWriter.Write(content);
        streamWriter.Flush();
        streamWriter.Close();
    }
 
}