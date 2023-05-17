using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class DBController
    {
        public List<List<double>> RRList { get; set; }
        public string Name { get; set; }
        public string CPR { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public DBController()
        {

        }

        public void SavePatientToDB(string firstname, string lastname,string cpr, string comment, List<double> RRlist)
        {
            using DBContextClass context = new DBContextClass();
            Patient patient = new Patient { FirstName = firstname, LastName = lastname, CPR = cpr, };
            Journal journal = new Journal { Comment = comment,Date=DateTime.Now };

            foreach (var item in RRlist)
            {
                Measurement measurement = new Measurement { mV = item};
                journal.Measurements.Add(measurement);
            }

            patient.Journals.Add(journal);
            context.Add(patient);

            context.SaveChanges();
        }

        public void LoadPatientFromDB(string identifier)
        {

        }

        public List<string> SearchDBForPatients(string searchText)
        {
            return null;
        }

        public List<string> GetPatientJournals()
        {
            List<string> journals = new List<string>();
            return journals;
        }
        public List<double> LoadSelectedJournal(string identifier)
        {
            List<double> ekgmeasurement = new List<double>();
            return ekgmeasurement;
        }
    }
}
