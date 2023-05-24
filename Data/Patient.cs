namespace Data
{
    public class Patient
    {
        public int Id { get; set; }
        public string CPR { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public ICollection<Journal> Journals { get; set; }= new List<Journal>();
    }
}
