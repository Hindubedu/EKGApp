// See https://aka.ms/new-console-template for more information

using FileShare;

 //To upload (on RaspBerry)
Uploader uploader = new Uploader("F23_Gruppe_02"); // Create an Uploader instance with a group name   F23_Gruppe_02
FileStream localFileStream = new FileStream("NormaltEKG.csv", FileMode.Open); // Open a filestream to data
string filename = uploader.Save("NormaltEKG.csv", localFileStream); // Upload data to a file
Console.WriteLine(filename); // Prints the filename the data is saved in - can change if you try to use same filename


//// To download (on PC)
//Downloader downloader = new Downloader("gruppe_10"); // Create a Downloader instance with the same group name
//FileStream newLocalStream = new FileStream("pc_data.csv", FileMode.Create); // Create a new file to save data in
//downloader.Load("data.csv", newLocalStream); // Get data from the file specified (should match filename returned from uploader) 