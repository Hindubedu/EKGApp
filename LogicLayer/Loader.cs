using FileShare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Loader
    {
        Downloader downloader = new Downloader("F23_Gruppe_02");



        public List<object> LoadNewestFromCloud()
        {
           return LoadChoiceFromCloud(GetFileNames().First());
        }

        public List<string> GetFileNames()
        {
            List<string> downloadStrings = new List<string>();
            return downloader.GetFilenames();
        }

        public List<object> LoadChoiceFromCloud(string loadPath)
        {
            FileStream newLocalStream = new FileStream("Files/pc_data3.csv", FileMode.Create); // Create a new file
            downloader.Load(loadPath, newLocalStream); // Get data from the file specified NormaltEKG_6.csv
            var stream2 = new FileStream("Files/pc_data3.csv", FileMode.Open); // Create a new file

            var values = new List<object>();
            using (StreamReader reader = new StreamReader(stream2)) // Get data from the file 
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    values.Add(Double.Parse(line));
                }
            }
            return values;
        }
    }
}
