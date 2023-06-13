using Data;
using DataModels;

namespace DataMapping
{
    public static class CollectionMapper
    {
        public static List<PatientModel> ToModels(this List<Patient> patients)
        {
            return patients.Select(p => p.ToModel()).ToList();
        }
        public static List<PatientEditModel> ToEditModels(this List<Patient> patients)
        {
            return patients.Select(p => p.ToEditModel()).ToList();
        }
        public static ICollection<JournalModel> ToModels(this ICollection<Journal> journals)
        {
            return journals.Select(j => j.ToModel()).ToList();
        }
        public static ICollection<MeasurementModel> ToModels(this ICollection<Measurement> measurements)
        {
            return measurements.Select(m => m.ToModel()).ToList();
        }
    }
}
