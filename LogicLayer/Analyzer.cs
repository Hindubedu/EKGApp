using EKGApp;

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
            Histogram histogram = new Histogram();
            double baseline = histogram.FindBaseLine(RRList);
            
            // TODO Analyse for ST elevation
            
            return false;
        }
    }
}
