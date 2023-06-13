namespace DataModels;

public interface IPatientModel
{
    int Id { get; set; }
    string CPR { get; set; }
    ICollection<JournalModel> Journals { get; set; }
}