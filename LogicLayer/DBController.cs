using Data;
using Microsoft.EntityFrameworkCore;
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
            Journal journal = new Journal { Comment = comment,Date=DateTime.Now };


            foreach (var item in RRlist)
            {
                Measurement measurement = new Measurement { mV = item};
                journal.Measurements.Add(measurement);
            }

            Patient patient = new Patient { FirstName = firstname, LastName = lastname, CPR = cpr};

            patient.Journals.Add(journal);
            context.Add(patient);
            context.SaveChanges();
        }

        public Patient LoadPatientFromDB(int identifier)
        {
            using (var context = new DBContextClass())
            {
                var patient = context.Patients
                    .Include(j => j.Journals)
                        .ThenInclude(m => m.Measurements)
                    .FirstOrDefault(p => p.Id == identifier);

                return patient;
            }
        }

        public List<Patient> SearchDBForPatients(string searchText)
        {
            using DBContextClass context = new DBContextClass();
            var patients = context.Patients.Where(x => x.CPR.Contains(searchText) || x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).ToList();
            if (patients != null)
            {
                return patients;
            }
            else
            {
                return new List<Patient>();
            }
        }

        public Journal LoadJournalFromDB(int identifier)
        {
            using (var context = new DBContextClass())
            {
                var journal = context.Journals
                    .Include(m => m.Measurements)
                    .FirstOrDefault(p => p.Id == identifier);

                return journal;
            }
        }

        public List<Journal> GetPatientJournals(Patient patient)
        {
            using DBContextClass context = new DBContextClass();
            var patientJournals = context.Journals.Where(j => j.PatientId == patient.Id).ToList();
            return patientJournals;
        }

        public List<Measurement> GetJournalMeasuremens(Journal journal)
        {
            using DBContextClass context = new DBContextClass();
            var journalMeasurements= context.Measurements.Where(m => m.JournalId == journal.Id).ToList();
            return journalMeasurements;
        }
    }
}
