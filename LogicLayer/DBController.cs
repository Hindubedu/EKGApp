using Bogus;
using Data;
using DataMapping;
using DataModels;
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

        public PatientModel? LoadPatient(int id)
        {
            using var context = new DBContextClass();

            var patient = context.Patients
                .Include(j => j.Journals)
                    .ThenInclude(m => m.Measurements)
                .FirstOrDefault(p => p.Id == id);
            if (patient==null)
            {
                return null;
            }
            return patient.ToModel();
        }

        public List<PatientModel> SearchForPatients(string searchText) ///Searches DB for patients but only returns first 100 otherwise GUI is too slow 
        {
            using DBContextClass context = new DBContextClass();
            var patients = context.Patients.Where(x => x.CPR.Contains(searchText) || x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).Take(100).ToList().ToModels();
            if (patients != null)
            {
                return patients;
            }
            else
            {
                return new List<PatientModel>();
            }
        }

        public JournalModel? LoadJournal(int identifier)
        {
            try
            {
                using (var context = new DBContextClass())
                {
                    var journal = context.Journals
                        .Include(m => m.Measurements)
                        .FirstOrDefault(p => p.Id == identifier);
                    if (journal==null)
                    {
                        return null;
                    }
                    return journal.ToModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while loading the journal: " + ex.Message);
                return null;
            }
        }
        public bool EditPatient(int patientId, string fullname, string cpr) //could be seperated into smaller methods
        {
            var context = new DBContextClass();

            var existingPatient = context.Patients
                .Include(j => j.Journals)
                    .ThenInclude(m => m.Measurements)
                .FirstOrDefault(p => p.Id == patientId);

            if (existingPatient == null)
            {
                return false;
            }
            var splitname = fullname.Split(" ");
            existingPatient.FirstName = $"{splitname[0]}";
            existingPatient.LastName = $"{splitname[1]}";
            existingPatient.CPR = cpr;
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
