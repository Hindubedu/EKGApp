// See https://aka.ms/new-console-template for more information

using FileShare;

// To upload (on RaspBerry)
/*
Uploader uploader = new Uploader("gruppe_10"); // Create an Uploader instance with a group name
FileStream localFileStream = new FileStream("data_6.csv", FileMode.Open); // Open a filestream to data
string filename = uploader.Save("data.csv", localFileStream); // Upload data to a file
Console.WriteLine(filename); // Prints the filename the data is saved in - can change if you try to use same filename
*/


Downloader downloader = new Downloader("gruppe_10");
var filenames = downloader.GetFilenames();
foreach (var name in filenames)
{
    Console.WriteLine(name);
}


// To download (on PC)
// try
// {
//     Downloader downloader = new Downloader("F23_Gruppe_05"); // Create a Downloader instance with the same group name
//     FileStream newLocalStream = new FileStream("data_6.csv", FileMode.Create); // Create a new file to save data in
//     downloader.Load("data_6.csv",
//         newLocalStream); // Get data from the file specified (should match filename returned from uploader) 
//
// }
// catch (Exception)
// {
//     Console.WriteLine("here");
// }