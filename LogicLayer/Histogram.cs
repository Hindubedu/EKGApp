namespace EKGApp
{
    public class Histogram
    {
        /// <summary>
        /// Opdeler data i bins af størrelsen 0,1 mV og returnerer det bin med flest observationer
        /// </summary>
        /// <param name="RRList">Listen af observationer</param>
        /// <returns>Baseline (bin med flest observationer)</returns>
        public double FindBaseLine(List<double> RRList)
        {
            //Opretter en tom SortedDictionary til at gemme RRList værdier og hvor mange gange de optræder
            SortedDictionary<double, int> RRDict = new();

            //Går gennem RRList og indsætter værdier i dictionary. Hvis samme værdi optræder flere gange optæller value
            foreach (double RR in RRList)
            {
                // Afrund RR til nærmste 0.1 mV (nærmeste decimal)
                double val = Math.Round(RR, 1);

                //Hvis dictionary allerede indeholder RR som key, øger vi den værdi med 1
                //Ellers tilføjer vi RR som en ny nøgle med værdien 1
                if (RRDict.ContainsKey(val))
                {
                    RRDict[val]++;
                }
                else
                {
                    RRDict.Add(val, 1);
                }
            }
            // Returner det bin med flest observationer
            return RRDict.OrderByDescending(x => x.Value).First().Key;
        }
    }
}
