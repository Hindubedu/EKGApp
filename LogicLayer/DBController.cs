using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //Helper methods from before i discovered include

        //public List<Journal> GetPatientJournals(Patient patient)
        //{
        //    using DBContextClass context = new DBContextClass();
        //    var patientJournals = context.Journals.Where(j => j.PatientId == patient.Id).ToList();
        //    return patientJournals;
        //}

        //public List<Measurement> GetJournalMeasuremens(Journal journal)
        //{
        //    using DBContextClass context = new DBContextClass();
        //    var journalMeasurements= context.Measurements.Where(m => m.JournalId == journal.Id).ToList();
        //    return journalMeasurements;
        //}
    }
}
