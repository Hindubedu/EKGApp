using Bogus;
using Data;
using Microsoft.EntityFrameworkCore;

namespace LogicLayer
{
    public class DBController
    {
        public List<List<double>> RRList { get; set; }
        public Patient currentPatient { get; set; }
        public int currentJournalId { get; set; }

        public DBController()
        {

        }

        public bool SavePatientToDB(string firstname, string lastname, string cpr, string comment, List<double> RRlist)
        {
            using DBContextClass context = new DBContextClass();
            if (currentPatient == null)
            {
                Journal journal = new Journal { Comment = comment, Date = DateTime.Now };


                foreach (var item in RRlist)
                {
                    Measurement measurement = new Measurement { mV = item };
                    journal.Measurements.Add(measurement);
                }

                Patient patient = new Patient { FirstName = firstname, LastName = lastname, CPR = cpr };

                patient.Journals.Add(journal);
                context.Add(patient);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Patient LoadPatientFromDB(int identifier)
        {
            using (var context = new DBContextClass())
            {
                var patient = context.Patients
                    .Include(j => j.Journals)
                        .ThenInclude(m => m.Measurements)
                    .FirstOrDefault(p => p.Id == identifier);
                currentPatient = patient;
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
                currentJournalId = journal.Id;
                return journal;
            }
        }
        public bool EditPatient(string firstname, string lastname, string cpr, string comment, List<double> RRlist)
        {
            using DBContextClass context = new DBContextClass();
            {
                var existingPatient = context.Patients
                .Include(p => p.Journals)
                .ThenInclude(j => j.Measurements)
                .FirstOrDefault(p => p.Id == currentPatient.Id);

                if (existingPatient != null)
                {
                    existingPatient.FirstName = firstname;
                    existingPatient.LastName = lastname;
                    existingPatient.CPR = cpr;

                    var existingJournal = existingPatient.Journals.FirstOrDefault(j => j.Id == currentJournalId);

                    if (existingJournal != null)
                    {
                        existingJournal.Comment = comment;
                        existingJournal.Measurements.Clear();
                        foreach (var item in RRlist)
                        {
                            Measurement m = new Measurement();
                            m.mV = item;
                            existingJournal.Measurements.Add(m);
                        }

                    }
                    context.SaveChanges();
                    currentPatient = null;
                    currentJournalId = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ClearAllPatients()
        {
            using (DBContextClass context = new DBContextClass())
            {

                var allPatients = context.Patients.ToList();

                foreach (var patient in allPatients)
                {
                    foreach (var journal in patient.Journals)
                    {
                        journal.Measurements.Clear();
                    }

                    patient.Journals.Clear();
                }

                // Remove all patients from the database
                context.Patients.RemoveRange(allPatients);

                // Save changes to the database
                context.SaveChanges();
            }
        }

        public void SaveJournalToPatient(string comment, List<double> RRlist)
        {
            using (DBContextClass context = new DBContextClass())
            {
                var existingPatient = context.Patients
                .Include(p => p.Journals)
                .ThenInclude(j => j.Measurements)
                .FirstOrDefault(p => p.Id == currentPatient.Id);

                Journal newJournal = new Journal();
                newJournal.Comment = comment;

                foreach (var item in RRlist)
                {
                    Measurement m = new Measurement();
                    m.mV = item;
                    newJournal.Measurements.Add(m);
                }
                existingPatient.Journals.Add(newJournal);

                context.SaveChanges();
                currentPatient = null;
                currentJournalId = 0;
            }
        }

        public void CreateBogusDB()
        {
            var patients = GeneratePeople();
            using (DBContextClass context = new DBContextClass())
            {
                context.Patients.AddRange(patients);
                context.SaveChanges();
            }
        }

        private List<Patient> GeneratePeople()
        {
            Randomizer.Seed = new Random(1);

            List<Patient> model = new List<Patient>();

            var faker = new Faker<Patient>()
                .RuleFor(x => x.FirstName, y => y.Name.FirstName())
                .RuleFor(x => x.LastName, z => z.Name.LastName())
                .RuleFor(x => x.CPR, z => z.Random.Replace("##########"));

            var result = faker.Generate(2);
            result.ForEach(x => model.Add(x));
            return model;
        }
    }
}
