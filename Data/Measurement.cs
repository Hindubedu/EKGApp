namespace Data
{
    public class Measurement
    {
        public int Id { get; set; }
        public double mV { get; set; }
        public int JournalId { get; set; }
        public Journal Journal { get; set; } = null!;
    }
}
