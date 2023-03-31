using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EKGApp
{
    public class Histogram
    {
        public double FindBaseLine(List<double> RRList)
        {
            //Opretter en tom dictionary til at gemme RRList værdier og hvor mange gange de optræder
            Dictionary<double, int> RRDict = new Dictionary<double, int>();
            
            //Finder min og max værdier i RRList
            double min = RRList.Min();
            double max = RRList.Max();
            
            //Går gennem RRList og indsætter værdier i dictionary. Hvis samme værdi optræder flere gange optæller value
            foreach (double RR in RRList)
            {

                //Hvis dictionary allerede indeholder RR som key, øger vi den værdi med 1

                if (RRDict.ContainsKey(RR))
                {
                    RRDict[RR]++;

                }
                
                //Ellers tilføjer vi RR som en ny nøgle med værdien 1

                else
                {
                    RRDict.Add(RR, 1);
                }
            }

            //Finder den key i dictionary, der har den højeste værdi, den mest almindelige RR-værdi. Hvis x's værdi er større end y's værdi, så returnerer x, ellers returnerer y
           //evt. skal de tre værdier der er flest af tages gennemsnittet af og bruges til baseline
            double baseline = RRDict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            //Returnerer baseline-værdien

            return baseline;
            

            
        }
    }
}
