using Data;
using DataModels;

namespace DataMapping
{
    public static class CollectionMapper
    {
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
