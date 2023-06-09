using Data;
using DataModels;

namespace DataMapping
{
    public static class PatientMapper
    {
        public static PatientModel ToModel(this Patient patient)
        {
            var patientmodel = new PatientModel();
            patientmodel.Id = patient.Id;
            patientmodel.CPR = patient.CPR;
            patientmodel.FullName = $"{patient.FirstName} {patient.LastName}";
            patientmodel.Journals = patient.Journals.ToModels();
            return patientmodel;
        }

        public static JournalModel ToModel(this Journal journal)
        {
            var journalmodel = new JournalModel();
            journalmodel.Id = journal.Id;
            journalmodel.Date = journal.Date;
            journalmodel.Comment = journal.Comment;
            journalmodel.Measurements = journal.Measurements.ToModels();
            return journalmodel;
        }

        public static MeasurementModel ToModel(this Measurement measurement)
        {
            var measurementmodel = new MeasurementModel();
            measurementmodel.Id = measurement.Id;
            measurementmodel.mV = measurement.mV;
            return measurementmodel;
        }
    }
}
