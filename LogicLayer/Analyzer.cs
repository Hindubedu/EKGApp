using EKGApp;
using System.Diagnostics.Metrics;

namespace LogicLayer
{
    public class Analyzer
    {
        private List<double> RRList { get; set; }
        public Analyzer(List<double> list)
        {
            RRList = list;
        }

        public bool DetectedSTElevation()
        {
            // Find baseline
            Histogram histogram = new Histogram();
            double baseline = histogram.FindBaseLine(RRList); // Beregner baseline-værdien for RRList ved at kalde metoden FindBaseLine på Histogram-objektet

            // Lav listen med målinger om til liste af koordinater (x = index/nr. måling, y=måling)
            List<Coordinates> signal = new(); // Opretter en ny liste med Coordinates-objekter kaldet signal
            for (int index = 0; index < RRList.Count; index++) // Gennemløber RRList
            {
                signal.Add(new Coordinates(index, RRList[index])); // Tilføjer et nyt Coordinates-objekt til signal for hver måling i RRList
            }

            // Udregn half_peak_width til 0,5% af målinger. Dette er halvdelen af et peaks bredde.
            // Dette bruges til at fjerne målinger omkring et peak, når det er fundet, så man ikke finder samme peak flere gange.
            // Jo mindre % her, desto mere sensitiv er detektoren for peaks.
            int half_peak_width = (int)(signal.Count * 0.005); // Beregner halvdelen af et peaks bredde baseret på antallet af målinger i signal

            //Find R- og S-takker
            PeakDetector peakDetector = new PeakDetector();
            List<Coordinates> rPeaks = peakDetector.DetectRPeaks(signal, baseline, half_peak_width);
            List<Coordinates> sPeaks = peakDetector.DetectSPeaks(signal, rPeaks, half_peak_width);

            // TODO returner true, hvis st-elevation er til stede.
            return true;
        }

        public class Coordinates
        {
            public double X, Y;

            public Coordinates(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }
        
        internal class PeakDetector
        {
            // Metode til at detektere r-peaks
            public List<Coordinates> DetectRPeaks(List<Coordinates> signal, double baseline, int half_peak_width)
            {
                // Fjern alle målinger, hvor målingen (y-aksen) er under tærskel (0,5 mV over baseline)
                double threshold = 0.5 + baseline;
                List<Coordinates> rPeaks = signal.Where(m => m.Y >= threshold).ToList();

                // Lav en tom liste, hvor vi gemmer fundne peaks
                List<Coordinates> foundRPeaks = new();
                
                // Mens listen med potentielle peaks (alle målinger over tærskel) er større end 2 x halv peak bredde, gør følgende
                while (rPeaks.Count > half_peak_width*2)
                {
                    // Find det største peak 
                    Coordinates peak = rPeaks.MaxBy(measurement => measurement.Y);
                    // Gem det på listen over fundne peaks
                    foundRPeaks.Add(peak);
                    // I listen over potentielle peaks, fjern X antal målinger (half_peak_width) til højre og venstre fra peaket 
                    rPeaks.RemoveAll(measurement => measurement.X < peak.X + half_peak_width && measurement.X > peak.X - half_peak_width);
                }

                return foundRPeaks;
            }

            // Metode til at detektere s-peaks 
            public List<Coordinates> DetectSPeaks(List<Coordinates> signal, List<Coordinates> rPeaks, int half_peak_width)
            {
                List<Coordinates> foundSPeaks = new();

                // Algoritme til s-takker 
                // For hver R-peak: find den laveste y-værdi mellem r-peak og r-peak + half_peak_width    

                foreach (Coordinates rPeak in rPeaks)
                {
                    // FInd det laveste punkt mellem r-peak og r-peak + halvdelen af peak-bredde på signalet (y-aksen)
                    Coordinates sPeak = signal.Where(m => m.X > rPeak.X && m.X < rPeak.X + half_peak_width).MinBy(c => c.Y);
                    // Tilføj fundet s-peak til listen af fundne s-peaks
                    foundSPeaks.Add(sPeak);
                }
                return foundSPeaks;
            }
        }

    }
}