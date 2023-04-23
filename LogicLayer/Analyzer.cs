using EKGApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                List<double> rPeaks = peakDetector.DetectRPeaks(RRList, baseline);

                foreach(double rPeak in rPeaks)
                {
                    Console.WriteLine("rPeak: " + rPeak);
                }

                return true;
            }
        }

        internal class PeakDetector
        {
            public List<double> DetectRPeaks(List<double> signal, double baseline)
            {
                List<double> foundRPeaks = new List<double>();

                // Algoritme til R-takker
                // 1. Tag værdier fra signal hvor y-værdien er større end eller lig tærskel (0.5 mV over baseline som udgangspunkt)
                // 2. Gem det højeste punkt på peakliste 
                // 3. Fjern X målinger fra hver side af peak (X = 5% af antal målinger) 
                // 4. Hvis signallisten har flere målinger, så gentag fra 2.

                // 1. Tag værdier fra signal hvor y-værdien er større end eller lig tærskel (0.5 mV over baseline som udgangspunkt)
                double threshold = 0.5 + baseline;
                List<double> rPeaks = signal.Where(y => y >= threshold).ToList();

                // Udregn half_peak_width til 5% af målinger 
                int half_peak_width = (int)(signal.Count * 0.01);

                // 4. Hvis der kan slettes en peaks bredde fra rPeaks (målinger over tærksel), så gentag fra 2.
                while (rPeaks.Count > half_peak_width*2)
                {
                    // 2. Gem det højeste punkt på peakliste
                    // Find højeste Y-værdi i  listen
                    double peak = rPeaks.Max();
                    // Gem peak i "fundet peaks" liste
                    foundRPeaks.Add(peak);

                    // 3. Fjern halv peak bredde målinger fra hver side af peak (half_peak_width = 5% af antal målinger)
                    // Find x-koordinat for peak
                    int peakXCord = rPeaks.IndexOf(peak);
                    // Fjern bredden af peak fra liste. Først højre, så venstre
                    rPeaks.RemoveRange(peakXCord, half_peak_width);
                    // Sletningen kan begynde før nul (altså i negative tal), så derfor bruger vi "Math.Clamp" til at indgrænse værdien til 0, hvis den er negativ.
                    int deleteStart = Math.Clamp(peakXCord - half_peak_width, 0, int.MaxValue);
                    rPeaks.RemoveRange(deleteStart, half_peak_width);
                }
              
                return foundRPeaks;
            }

            public List<int> DetectSPeaks(List<double> signal)
            {
                List<int> sPeaks = new List<int>();

                //TODO Implementer logik for at finde S-takker i signalet

                return sPeaks;
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



