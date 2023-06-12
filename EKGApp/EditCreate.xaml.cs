using DataModels;
using LogicLayer;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EKGApp
{
    /// <summary>
    /// Interaction logic for EditCreate.xaml
    /// </summary>
    public partial class EditCreate : Window
    {
        private DBController _dbController = new DBController();
        public int CurrentJournalId { get; private set; }
        public int CurrentPatientId { get; private set; }
        public EditCreate()
        {
            InitializeComponent();

        }

        private void SearchDBTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var patients = _dbController.SearchForPatients(SearchDBTextBox.Text);

            if (patients.Count > 0)
            {
                SearchDBDropDownComboBox.ItemsSource = PatientNameAndCPR(patients);
                SearchDBDropDownComboBox.IsDropDownOpen = true;
            }
        }

        private List<ComboBoxItem> PatientNameAndCPR(List<PatientModel> patients)
        {
            var patientItems = patients.Select(patient =>
            {
                var item = new ComboBoxItem
                {
                    Content = $"{patient.FullName} CPR: {patient.CPR}",
                    Tag = patient.Id // Set the Tag property to the patient ID
                };
                return item;
            });
            return patientItems.ToList();
        }

        private void TextBox_ClearText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Tag == null)
                {
                    textBox.Text = string.Empty;
                    textBox.Tag = "Focused";
                }
            }
        }

        private void SearchDBDropDownComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedPatientItem = (ComboBoxItem)comboBox.SelectedItem;
            if (selectedPatientItem != null)
            {
                var selectedPatientID = (int)selectedPatientItem.Tag;
                var patient = _dbController.LoadPatient(selectedPatientID);
                if (patient == null)
                {
                    return;
                }
                CurrentPatientId = patient.Id;
                CurrentJournalId = 0;
                JournalListBox.ItemsSource = SetJournalInfo(patient.Journals.ToList());
                CurrentPatientTextBlock.Text = $"{patient.FullName} {patient.CPR}";
                CommentTextBox.IsEnabled=false;
            }
        }

        private List<ListBoxItem> SetJournalInfo(List<JournalModel> journals)
        {
            var journalItems = journals.Select(journal =>
            {
                var item = new ListBoxItem
                {
                    Content = $"Date: {journal.Date}",
                    Tag = journal.Id
                };
                return item;
            });
            return journalItems.ToList();
        }

        private void JournalListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox)sender;
            var selectedJournalItem = (ListBoxItem)listbox.SelectedItem;
            if (selectedJournalItem != null)
            {
                var selectedJournalID = (int)selectedJournalItem.Tag;
                var journal = _dbController.LoadJournal(selectedJournalID);
                CurrentJournalId = journal.Id;
                CommentTextBox.Text= journal.Comment;
                CommentTextBox.IsEnabled = true;
            }
        }

        private void CPRTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void NameTextBoxes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\P{L}+"); // Should prohibit to any letter
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveToDBButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPatientId==0)
            {
                ShowMessage("Currently No Patient Loaded");
                return;
            }

            if (IsUserInputCorrect(CPRTextBox.Text.Trim())==false)
            {
                return;
            }
            var saved = _dbController.EditPatient(CurrentPatientId, FirstNameTextBox.Text, LastNameTextBox.Text, CPRTextBox.Text);
            ShowMessage(saved? "Changes Saved":"No Patient Loaded");
            if (saved)
            {
                ResetUI();
            }
        }
        private void DeletePatient_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrentPatientId==0)
            {
                ShowMessage("Currently No Patient Loaded");
                return;
            }
            var saved = _dbController.DeletePatient(CurrentPatientId);
            ShowMessage(saved ? "Changes Saved" : "Currently No Patient Loaded");
            if (saved)
            {
                ResetUI();
            }
        }

        private void DeleteJournal_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrentJournalId==0)
            {
                ShowMessage("Currently No Journal Loaded");
                return;
            }
            var saved = _dbController.DeleteJournal(CurrentJournalId);
            ShowMessage(saved ? "Changes Saved" : "Currently No Journal Loaded");
            if (saved)
            {
                CurrentJournalId = 0;
                CommentTextBox.IsEnabled=false;
            }
        }

        private bool IsUserInputCorrect(string cleanCPR)
        {
            if (cleanCPR.Length != 10)
            {
                ShowMessage("CPR must be 10 digits");
                return false;
            }
            return true;
        }

        private async void ShowMessage(string message)
        {
            MessageTextBox.Text = message;

            await Task.Delay(3000);

            MessageTextBox.Text = "";
        }
        private void ResetUI()
        {
            CurrentPatientTextBlock.Text = "No Patient";
            CommentTextBox.IsEnabled = false;
            CurrentPatientId = 0;
            CurrentJournalId = 0;
        }
    }
}
