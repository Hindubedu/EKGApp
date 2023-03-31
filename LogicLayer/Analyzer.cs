using EKGApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Analyzer
    {
        private List<double> RRList  { get; set; }
        public Analyzer(List<double> list)
        {
            RRList = list;
        }

        public bool DetectedSTElevation()
        {
            Histogram histogram = new Histogram();
            double baseline = histogram.FindBaseLine(RRList);
            //Analyse for ST elevation
            return false;
        }
    }
}
