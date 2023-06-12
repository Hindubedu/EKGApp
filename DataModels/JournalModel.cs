namespace DataModels
{
    public class JournalModel
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public DateTime Date { get; set; }
        public ICollection<MeasurementModel> Measurements { get; set; } = new List<MeasurementModel>();
    }
}
