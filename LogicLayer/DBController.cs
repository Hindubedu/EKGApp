using Bogus;
using Data;
using Microsoft.EntityFrameworkCore;

namespace LogicLayer
{
    public class DBController
    {
        public void SavePatient(string firstname, string lastname, string cpr, string comment, List<double> RRlist)
        {
            using DBContextClass context = new DBContextClass();

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
        }

        public Patient LoadPatient(int id)
        {
            using var context = new DBContextClass();

            var patient = context.Patients
                .Include(j => j.Journals)
                    .ThenInclude(m => m.Measurements)
                .FirstOrDefault(p => p.Id == id);
            return patient;
        }

        public List<Patient> SearchForPatients(string searchText) ///Searches DB for patients but only returns first 100 otherwise GUI is too slow 
        {
            using DBContextClass context = new DBContextClass();
            var patients = context.Patients.Where(x => x.CPR.Contains(searchText) || x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).Take(100).ToList();
            if (patients != null)
            {
                return patients;
            }
            else
            {
                return new List<Patient>();
            }
        }

        public Journal LoadJournal(int identifier)
        {
            try
            {
                using (var context = new DBContextClass())
                {
                    var journal = context.Journals
                        .Include(m => m.Measurements)
                        .FirstOrDefault(p => p.Id == identifier);
                    return journal;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while loading the journal: " + ex.Message);
                return null;
            }
        }
        public bool EditPatient(int patientId, int journalId, string firstname, string lastname, string cpr, string comment, List<double> RRlist) //could be seperated into smaller methods
        {
            using var context = new DBContextClass();
            var existingPatient = context.Patients
                .Include(p => p.Journals)
                .ThenInclude(j => j.Measurements)
                .FirstOrDefault(p => p.Id == patientId);

            if (existingPatient == null)
            {
                return false;
            }

            existingPatient.FirstName = firstname;
            existingPatient.LastName = lastname;
            existingPatient.CPR = cpr;

            var existingJournal = existingPatient.Journals.FirstOrDefault(j => j.Id == journalId);

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
            return true;
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

        public async Task SaveJournalToPatient(int patientId, string comment, List<double> RRlist)
        {
            using var db = new DBContextClass();
            var patientExists = await db.Patients.AnyAsync(p => p.Id == patientId);

            if (!patientExists)
            {
                return;
            }

            var newJournal = new Journal { Comment = comment, PatientId = patientId, Date = DateTime.Now };
            var mes = RRlist.Select(milliVolt => new Measurement { mV = milliVolt, Journal = newJournal });
            await db.Measurements.AddRangeAsync(mes);
            await db.SaveChangesAsync();
        }

        public bool IsDatabaseEmpty()
        {
            using var context = new DBContextClass();
            return !context.Patients.Any(); //returns true if database is empty notice the !
        }
    }


    public static class FakeDataGenerator
    {
        public static void PopulateDB()
        {
            var patients = GeneratePeople();
            using (DBContextClass context = new DBContextClass())
            {
                context.Patients.AddRange(patients);
                context.SaveChanges();
            }
        }

        private static List<Patient> GeneratePeople()
        {
            Randomizer.Seed = new Random(1);

            var model = new List<Patient>();

            var faker = new Faker<Patient>()
                .RuleFor(x => x.FirstName, y => y.Name.FirstName())
                .RuleFor(x => x.LastName, z => z.Name.LastName())
                .RuleFor(x => x.CPR, z => z.Random.Replace("##########"));

            var result = faker.Generate(10000);
            result.ForEach(x => model.Add(x));
            return model;
        }
    }
}
