using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Patient
    {
        public int Id { get; set; }
        public string CPR { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public ICollection<Journal>? Journals { get; set; }
    }
}
