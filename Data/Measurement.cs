using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Measurement
    {
        public int Id { get; set; }
        public double mV { get; set; }

        public int JournalId { get; set; }

        public Journal Journal { get; set; } = null!;
    }
}
