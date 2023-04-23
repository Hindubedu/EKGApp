using EKGApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            {
                Histogram histogram = new Histogram();
                double baseline = histogram.FindBaseLine(RRList);

                //Find R- og S-takker
                PeakDetector peakDetector = new PeakDetector();
                List<Coordinates> rPeaks = peakDetector.DetectRPeaks(RRList, baseline);
                //List<double> sPeaks = peakDetector.DetectSPeaks(RRList, rPeaks);

                return true;
            }
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
            public List<Coordinates> DetectRPeaks(List<double> signal, double baseline)
            {

                // Lav listen med målinger om til liste af koordinater (x = index/nr. måling, y=måling)
                List<Coordinates> rPeaks = new();
                for(int index = 0; index < signal.Count; index++)
                {
                    rPeaks.Add(new Coordinates(index, signal[index]));
                }

                // Fjern alle målinger, hvor målingen (y-aksen) er under tærskel (0,5 mV over baseline)
                double threshold = 0.5 + baseline;
                rPeaks.RemoveAll(m => m.Y < threshold);
                
                // Udregn half_peak_width til 5% af målinger. Dette er halvdelen af et peaks bredde.
                // Dette bruges til at fjerne målinger omkring et peak, når det er fundet, så man ikke finder samme peak flere gange.
                int half_peak_width = (int)(signal.Count * 0.01);

                // Lav en tom liste, hvor vi gemmer funde peaks
                List<Coordinates> foundRPeaks = new();
                
                // Mens listen med potentielle peaks (alle målinger over tærskel) er større end 2 x halv peak bredde, gør følgende
                while (rPeaks.Count > half_peak_width*2)
                {
                    // Find det største peak 
                    Coordinates peak = rPeaks.MaxBy(p => p.Y);
                    // Gem det på listen over fundne peaks
                    foundRPeaks.Add(peak);
                    // I listen over potentielle peaks, fjern X antal målinger (half_peak_width) til højre og venstre fra peaket 
                    rPeaks.RemoveAll(p => p.X < peak.X - half_peak_width || p.X > peak.X + half_peak_width);
                }

                return foundRPeaks;
            }

            public List<double> DetectSPeaks(List<double> signal, List<(double, double)> rPeaks)
            {
                List<double> foundSPeaks = new List<double>();

                // Algoritme til s-takker 
                // For hver R-peak: find den laveste y-værdi mellem r-peak og r-peak + half_peak_width    
                
                //foreach (double rPeak in rPeaks)
                //{
                //    //signal.GetRange(rPeaks)
                //}


                return foundSPeaks;
            }
        }

        //internal class PulseDetector
        //{
        //    public static double CalculatePulse(List<double> RRList)
        //    {
        //        bool belowThreshold = true;
        //        int Rtak_old = 0;
        //        int Rtak_new = 0;
        //        double threshold = 0.5; //Antager at threshold er 0.5
        //        double sample = 0.002; //Antager at sample rate er 0.002

        //        List<double> RPeaks = new List<double>();

        //        for (int i = 0; i < RRList.Count; i++)
        //        {
        //            double RR = RRList[i];

        //            if (RR < threshold && belowThreshold)
        //            {
        //                belowThreshold = false;
        //                Rtak_new = i;

        //                if (Rtak_old != 0)
        //                {
        //                    RPeaks.Add((Rtak_new - Rtak_old) * sample * 60);
        //                }
        //                Rtak_old = Rtak_new;
        //            }
        //            else if (RR >= threshold && !belowThreshold)
        //            {
        //                belowThreshold = true;
        //            }
        //        }

        //        double pulse = RPeaks.Count > 0 ? RPeaks.Average() : 0;
        //        return pulse;
        //    }
        //}
    }
}



