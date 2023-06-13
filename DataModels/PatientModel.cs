namespace DataModels
{
    public class PatientModel: IPatientModel
    {
        public int Id { get; set; }
        public string CPR { get; set; }
        public string FullName { get; set; }
        public ICollection<JournalModel> Journals { get; set; } = new List<JournalModel>();
    }
}
