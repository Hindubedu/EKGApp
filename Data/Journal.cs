using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Journal
    {
        public int Id { get; set; }
        
        public string? Comment { get; set; }

        public int PatientId { get; set; }

        public Patient Patient { get; set; } = null!;
        public DateTime Date { get; set; }

        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

    }
}
