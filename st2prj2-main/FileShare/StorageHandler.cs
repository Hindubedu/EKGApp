using Firebase.Storage;

namespace FileShare;

internal class StorageHandler
{
    private readonly string _groupNum;
    private FirebaseStorage _firebaseStorage;

    public StorageHandler(string groupNum)
    {
        _groupNum = groupNum;
        var options = new FirebaseStorageOptions();
        _firebaseStorage = new FirebaseStorage("st2prj2-fileshare.appspot.com", options);
    }

    internal async Task<string> Upload(string filename, FileStream fs)
    {
        var filenameToUse = await FindNextLegalFileName(filename);
        
        await _firebaseStorage.Child(_groupNum).Child(filenameToUse).PutAsync(fs)!;
        var firebaseStorageReference = await _firebaseStorage.Child(_groupNum).Child(filenameToUse).GetMetaDataAsync();
        return filenameToUse;
    }

    // internal async Task<List<string>> List()
    // {
    //     var firebaseMetaData = await _firebaseStorage.Child(_groupNum).GetMetaDataAsync();
    //     _firebaseStorage.Child(_groupNum).
    // }

   
    private async Task<string> FindNextLegalFileName(string filename)
    {
        if (! await IsNameUsed(filename))
        {
            return filename;
        }

        var i = 1;
        while (true)
        {
            var filenameNumbered = GetFileName(filename, i);
            if (! await IsNameUsed(filenameNumbered))
            {
                return filenameNumbered;
            }
            i++;
        }
    }

    private async Task<bool> IsNameUsed(string filename)
    {
        try
        {
            await _firebaseStorage.Child(_groupNum).Child(filename).GetMetaDataAsync()!;
            return true;
        }
        catch (Exception)
        {
            return false;
        }        
    }

    private string GetFileName(string filename, int i)
    {
        if (filename.Contains("."))
        {
            var index = filename.LastIndexOf(".", StringComparison.Ordinal);
            return $"{filename.Substring(0, index)}_{i}.{filename.Substring(index + 1)}";
        }

        return $"{filename}_{i}";
    }

    internal async Task<string> Download(string filename)
    {
        try
        {
            var url = await _firebaseStorage.Child(_groupNum).Child(filename).GetDownloadUrlAsync();
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        } catch (Exception) {
            throw new ArgumentException($"File '{filename}' do not exists. Has file been created");
        }
     
    }
}

