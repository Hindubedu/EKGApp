using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;
using LogicLayer;
using DataModels;

namespace EKGApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection MyCollection { get; set; }
        private LineSeries EKGLine;
        List<double> RRList = new List<double>();
        Loader loader = new Loader();
        private DBController dbController = new DBController();
        double sample = 500;
        public Func<double, string> labelformatter { get; set; }
        public Func<double, string> labelformatter1 { get; set; }
        public int CurrentJournalId { get; private set; }
        public int CurrentPatientId { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            labelformatter = x => (x / sample).ToString();
            labelformatter1 = x => (x.ToString("F1"));
            MyCollection = new SeriesCollection();
            EKGLine = new LineSeries();
            EKGLine.Values = new ChartValues<double> { };
            EKGLine.Fill = Brushes.Transparent;
            EKGLine.PointGeometry = null;
            MyCollection.Add(EKGLine);
            DataContext = this;
            LoadInitialCloudFiles();

            //dbController.ClearAllPatients();
            if (dbController.IsDatabaseEmpty())
            {
                FakeDataGenerator.PopulateDB();
            }
        }

        private void LoadInitialCloudFiles()
        {
            EKGMeasurementCombobox.ItemsSource = loader.GetFileNames();
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)///Updates PulsTextBlock with a measurement of pulses/min (heartrate) if an EKG has been loaded 
        {
            if (!RRList.Any())
            {
                ShowMessage("Please load a file");
                return;
            }
            Analyzer analyzer = new Analyzer(RRList);
            var STElevation = analyzer.DetectedSTElevation();
            PulsTextBlock.Text = $"ST: Elevation: {STElevation.Item1.ToString()} \n Puls: {STElevation.Item2.ToString()}";
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            EKGLine.Values.Clear();
            RRList.Clear();
            int index = 0;

            using (StreamReader reader = new StreamReader("Files/NormaltEKG.csv")) // Same procedure as every year...
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (index > 1)
                    {
                        var splitLine = line.Trim().Split(',');

                        var doubleValues = splitLine[1].Replace('.', ',');

                        if (splitLine.Length == 2)
                        {
                            RRList.Add(Double.Parse(doubleValues));
                            EKGLine.Values.Add(Double.Parse(doubleValues));
                            RRList.Add(Double.Parse(doubleValues));
                        }
                        else
                        {
                            Debug.WriteLine($"Error in line: {index}, line-split not working: {doubleValues.Length} lines after split");
                        }
                    }
                    index++;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Graf.AxisX[0].MinValue = 0;
            Graf.AxisX[1].MinValue = 0;
            Graf.AxisX[0].MaxValue = 3 * sample;
            Graf.AxisX[1].MaxValue = 3 * sample;
            Graf.AxisX[0].Separator.Step = 0.04 / (1 / sample);
            Graf.AxisX[1].Separator.Step = 0.2 / (1 / sample);


            Graf.AxisY[0].MinValue = -0.5;
            Graf.AxisY[1].MinValue = -0.5;
            Graf.AxisY[0].MaxValue = 2.0;
            Graf.AxisY[1].MaxValue = 2.0;
            Graf.AxisY[0].Separator.Step = 0.1;
            Graf.AxisY[1].Separator.Step = 0.5;
        }

        private void LoadFromCombobox_Click(object sender, RoutedEventArgs e)
        {
            string loadPath = EKGMeasurementCombobox.Text;
            if (string.IsNullOrEmpty(loadPath))
            {
                MessageBox.Show($"Please select a measurement to load");
            }
            else
            {
                EKGLine.Values.Clear();
                RRList.Clear();
                var values = loader.LoadChoiceFromCloud(loadPath);
                EKGLine.Values.AddRange(values);
                RRList.AddRange(values.Cast<double>());
                ShowMessage("Loaded");
            }
            CurrentJournalId = 0;
            AddJournalButton.IsEnabled = true;
        }

        private void LoadNewestButton_Click(object sender, RoutedEventArgs e) ///Loads the newest measurement from the cloud and shows it on the graf, also updates the EKGmeasurementscombobox
        {
            EKGLine.Values.Clear();
            RRList.Clear();

            var cloudfilenames = loader.GetFileNames();
            EKGMeasurementCombobox.ItemsSource = cloudfilenames;

            var newest = loader.LoadChoiceFromCloud(cloudfilenames.First()); //does not work

            EKGLine.Values.AddRange(newest);
            RRList.AddRange(newest.Cast<double>());
        }

        private void TextBox_ClearText(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox textBox) return;
            if (textBox.Tag == null)
            {
                textBox.Text = string.Empty;
                textBox.Tag = "Focused";
            }
        }

        private void SearchDBTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var patients = dbController.SearchForPatients(SearchDBTextBox.Text);

            if (patients.Count <= 0) return;
            SearchDBDropDownComboBox.ItemsSource = PatientNameAndCPR(patients);
            SearchDBDropDownComboBox.IsDropDownOpen = true;
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

        private void SearchDBDropDownComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedPatientItem = (ComboBoxItem)comboBox.SelectedItem;
            if (selectedPatientItem != null)
            {
                var selectedPatientID = (int)selectedPatientItem.Tag;
                var patient = dbController.LoadPatient(selectedPatientID);
                if (patient == null)
                {
                    return;
                }
                CurrentPatientId = patient.Id;
                UpdatePatientUIInfo(patient);
                JournalListBox.ItemsSource = SetJournalInfo(patient.Journals.ToList());
                AddJournalButton.IsEnabled = true;
            }
        }

        private void UpdatePatientUIInfo(PatientModel patient)
        {
            NameTextBlock.Text = patient.FullName;
            CPRTextBlock.Text = patient.CPR;
        }

        private void UpdateJournalUIInfo(JournalModel journal)
        {
            CommentTextBox.Text = journal.Comment;
            RRList.Clear();
            RRList.AddRange(journal.Measurements.Select(item => item.mV));
        }

        private void JournalListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox)sender;
            var selectedJournalItem = (ListBoxItem)listbox.SelectedItem;
            if (selectedJournalItem != null)
            {
                var selectedJournalID = (int)selectedJournalItem.Tag;
                var journal = dbController.LoadJournal(selectedJournalID);
                CurrentJournalId = journal.Id;
                UpdateJournalUIInfo(journal);
                UpdateGraph();
            }
        }

        private void UpdateGraph()
        {
            EKGLine.Values.Clear();
            EKGLine.Values.AddRange(RRList.Select(value => (object)value));
        }

        private void ResetUIButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        private async void ShowMessage(string message)
        {
            MessageTextBox.Text = message;

            await Task.Delay(3000);

            MessageTextBox.Text = "";
        }

        private void ResetUI()
        {
            NameTextBlock.Text = "Name";
            CPRTextBlock.Text = "CPR";
            CommentTextBox.Clear();
            RRList.Clear();
            EKGLine.Values.Clear();
            LoadInitialCloudFiles();
            SearchDBTextBox.Text = "";
            SearchDBDropDownComboBox.ItemsSource = null;
            AddJournalButton.IsEnabled = false;
            CurrentJournalId = 0;
            CurrentPatientId = 0;
        }

        private async void AddJournalButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPatientId != 0 && RRList.Count > 0 && CurrentJournalId == 0)
            {
                AddJournalButton.IsEnabled = false;
                await dbController.SaveJournalToPatient(CurrentPatientId,CommentTextBox.Text, RRList);
                ShowMessage("Journal Saved");
                CommentTextBox.Text = "";
                CurrentPatientId = 0;
                CurrentJournalId = 0;
            }
            else
            {
                ShowMessage("Please choose a patient or new EKG");
            }
        }

        private void CreateEditButton_Click(object sender, RoutedEventArgs e)
        {
            EditCreate editCreate = new EditCreate();
            editCreate.Show();
        }

        private void CreateNewPatientButton_Click(object sender, RoutedEventArgs e)
        {
            NewPatientWindow newPatientWindow = new NewPatientWindow();
            newPatientWindow.Show();
        }
    }
}
