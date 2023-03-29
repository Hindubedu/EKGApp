using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKGApp
{
    public class Histogram
    {
        List<double> RRList; //deklarer listen som en variabel i klassen

        public Histogram(List<double> list) //opretter en konstruktør
        {
            RRList = list; //tildeler listen til variablen
        }
    }
}
