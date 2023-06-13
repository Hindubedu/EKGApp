using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class PatientEditModel : IPatientModel
    {
        public int Id { get; set; }
        public string CPR { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public ICollection<JournalModel> Journals { get; set; } = new List<JournalModel>();

    }
}
