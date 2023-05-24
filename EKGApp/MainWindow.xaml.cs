using FileShare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;
using LogicLayer;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Data;

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
        List<double> RRDiff = new List<double>();
        public string SaveString { get; set; } = "Save patient";
        public string EditString { get; set; } = "Edit patient";

        Loader loader = new Loader();
        DBController dbController = new DBController();
        double sample = 500;

        private bool fileLoaded = false;
        public Func<double, string> labelformatter { get; set; }
        public Func<double, string> labelformatter1 { get; set; }

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
            //dbController.CreateBogusDB();
        }

        private void LoadInitialCloudFiles()
        {
            EKGMeasurementCombobox.ItemsSource = loader.GetFileNames();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e) //Change
        {
            EKGLine.Values.Clear();
            RRList.Clear();
            int index = 0;
            // To download (on PC)
            //LoadFilesFromCLoud(List<double> RRList);
            Downloader downloader = new Downloader("F23_Gruppe_02"); // Create a Downloader instance with group name F23_Gruppe_02
            FileStream newLocalStream = new FileStream("Files/pc_data3.csv", FileMode.Create); // Create a new file
            downloader.Load("NormaltEKG_9.csv", newLocalStream); // Get data from the file specified NormaltEKG_6.csv
            var stream2 = new FileStream("Files/pc_data3.csv", FileMode.Open); // Create a new file

            var values = new List<object>();
            using (StreamReader reader = new StreamReader(stream2)) // Get data from the file 
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
                            values.Add(Double.Parse(doubleValues));
                        }
                        else
                        {
                            Debug.WriteLine($"Error in line: {index}, linesplit not working: {doubleValues.Length} lines after split");
                        }
                    }
                    index++;
                }
            }
            EKGLine.Values.AddRange(values);
            RRList.AddRange(values.Cast<double>());
            fileLoaded = true;
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)///Updates PulsTextBlock with a measurement of pulses/min (heartrate) if an EKG has been loaded 
        {
            Analyzer analyzer = new Analyzer(RRList);
            bool STElevation = analyzer.DetectedSTElevation();
            PulsTextBlock.Text = $"ST: Elevation: {STElevation}";
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
                        }
                        else
                        {
                            Debug.WriteLine($"Error in line: {index}, linesplit not working: {doubleValues.Length} lines after split");
                        }
                    }
                    index++;
                }
            }
            fileLoaded = true;
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            //To upload (on RaspBerry)
            //Uploader uploader = new Uploader("F23_Gruppe_02"); // Create an Uploader instance with a group name
            //FileStream localFileStream = new FileStream("NormaltEKG.csv", FileMode.Open); // Open a filestream to data
            //string filename = uploader.Save("NormaltEKG.csv", localFileStream); // Upload data to a file
            //Debug.WriteLine(filename); // Prints the filename the data is saved in - can change if you try to use same filename
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Graf.AxisX[0].MinValue = 0;
            Graf.AxisX[1].MinValue = 0;
            Graf.AxisX[0].MaxValue = sample;
            Graf.AxisX[1].MaxValue = sample;

            Graf.AxisX[0].Separator.Step = 0.04 / (1 / sample);
            Graf.AxisX[1].Separator.Step = 0.2 / (1 / sample);

            Graf.AxisY[0].MinValue = -1;
            Graf.AxisY[1].MinValue = -1;
            //Skal indstilles til 1.5 mV, eller hvad der cirka passer. Skal justeres og tilpasses senere 
            //når vi har målinger vi kan teste på
            Graf.AxisY[0].MaxValue = 1.5;
            Graf.AxisY[1].MaxValue = 1.5;
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
                fileLoaded = true;
            }
            dbController.currentJournalId = 0;
        }

        private void LoadNewestButton_Click(object sender, RoutedEventArgs e) ///Loads the newest measurement from the cloud and shows it on the graf, also updates the EKGmeasurementscombobox
        {
            EKGLine.Values.Clear();
            RRList.Clear();

            var cloudfilenames = loader.GetFileNames();
            EKGMeasurementCombobox.ItemsSource = cloudfilenames;

            var newest = loader.LoadChoiceFromCloud(cloudfilenames.First());

            EKGLine.Values.AddRange(newest);
            RRList.AddRange(newest.Cast<double>());
            fileLoaded = true;
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

        private void SaveToDBButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToDB();
        }

        private void NameTextBoxes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\P{L}+"); // Should prohibit to any letter
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CPRTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void SearchDBTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var patients = dbController.SearchDBForPatients(SearchDBTextBox.Text);

            if (patients.Count > 0)
            {
                SearchDBDropDownComboBox.ItemsSource = PatientNameAndCPR(patients);
                SearchDBDropDownComboBox.IsDropDownOpen = true;
            }
        }

        private List<ComboBoxItem> PatientNameAndCPR(List<Patient> patients)
        {
            var patientItems = patients.Select(patient =>
            {
                var item = new ComboBoxItem
                {
                    Content = $"{patient.FirstName} {patient.LastName} CPR: {patient.CPR}",
                    Tag = patient.Id // Set the Tag property to the patient ID
                };
                return item;
            });

            return patientItems.ToList();
        }

        private List<ListBoxItem> SetJournalInfo(List<Journal> journals)
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

        //Comment only enable if a journal is loaded
        //Journals clear with the change of a patient so does the enable of comments
        //Reset clears journals
        //Journal selected enables comments


        private void SearchDBDropDownComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditPatientButton.Content = EditString;
            CommentTextBox.IsEnabled=false;
            var comboBox = (ComboBox)sender;
            var selectedPatientItem = (ComboBoxItem)comboBox.SelectedItem;
            if (selectedPatientItem != null)
            {
                var selectedPatientID = (int)selectedPatientItem.Tag;
                var patient = dbController.LoadPatientFromDB(selectedPatientID);

                UpdatePatientUIInfo(patient);
                JournalListBox.ItemsSource = SetJournalInfo(patient.Journals.ToList());
                HideSaveButton(true);
            }
        }

        private void UpdatePatientUIInfo(Patient patient)
        {
            FirstNameTextBox.Text = patient.FirstName;
            LastNameTextBox.Text = patient.LastName;
            CPRTextBox.Text = patient.CPR;
        }

        private void UpdateJournalUIInfo(Journal journal)
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
                var journal = dbController.LoadJournalFromDB(selectedJournalID);
                UpdateJournalUIInfo(journal);
                UpdateGraph();
                CommentTextBox.IsEnabled = true;
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

        private void EditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditPatientButton.Content.ToString().Trim() == EditString)
            {
                FirstNameTextBox.IsEnabled = true;
                LastNameTextBox.IsEnabled = true;
                CPRTextBox.IsEnabled = true;
                AddJournalButton.IsEnabled = true;
                CommentTextBox.IsEnabled =true;
                EditPatientButton.Content = SaveString;
            }
            else if (EditPatientButton.Content.ToString() == SaveString)
            {
                EditPatientInDB();
            }
        }

        private void HideSaveButton(bool hideSaveButton) /// Method to switch 
        {
            if (hideSaveButton == true)
            {
                SaveToDBButton.IsEnabled = false;
                SaveToDBButton.Visibility = Visibility.Hidden;
                EditPatientButton.IsEnabled = true;
                EditPatientButton.Visibility = Visibility.Visible;

                FirstNameTextBox.IsEnabled = false;
                LastNameTextBox.IsEnabled = false;
                CPRTextBox.IsEnabled = false;
                CommentTextBox.IsEnabled = false;
                AddJournalButton.IsEnabled = false;
            }
            else
            {
                SaveToDBButton.IsEnabled = true;
                SaveToDBButton.Visibility = Visibility.Visible;
                EditPatientButton.IsEnabled = false;
                EditPatientButton.Visibility = Visibility.Hidden;
                EditPatientButton.Content = SaveString;

                FirstNameTextBox.IsEnabled = true;
                LastNameTextBox.IsEnabled = true;
                CPRTextBox.IsEnabled = true;
                CommentTextBox.IsEnabled = true;
            }
        }

        private void SaveToDB()
        {
            string cleanedCPR = CPRTextBox.Text.Replace(" ", string.Empty);
            if (IsUserInputCorrect(cleanedCPR))
            {
                bool isSaved = dbController.SavePatientToDB(FirstNameTextBox.Text, LastNameTextBox.Text, cleanedCPR, CommentTextBox.Text, RRList);
                if (isSaved == true)
                {
                    ShowMessage("Patient saved...");
                    HideSaveButton(false);
                }
                else
                {
                    ShowMessage("Patient bot saved...");
                }
            }
            
        }

        private void EditPatientInDB()
        {
            string cleanedCPR = CPRTextBox.Text.Replace(" ", string.Empty);
            bool isSaved = false;
            if (IsUserInputCorrect(cleanedCPR))
            {
               isSaved = dbController.EditPatient(FirstNameTextBox.Text, LastNameTextBox.Text, cleanedCPR, CommentTextBox.Text, RRList);

            }
            if (isSaved)
            {
                ResetUI();
                ShowMessage("Changes Saved");
            }
            else
            {
                ShowMessage("Cannot find patient");
            }

        }

        private bool IsUserInputCorrect(string cleanCPR)
        {

            if (cleanCPR.Length == 10) //EditPatientButton.Content
            {
                return true;
            }
            else if (RRList.IsNullOrEmpty())
            {
                ShowMessage("Please Load a file to save");
                return false;
            }
            else if (cleanCPR.Length != 10)
            {
                ShowMessage("CPR must be 10 digits");
                return false;
            }
            else
            {
                ShowMessage("Something Went wrong");
                return false;
            }
        }
        private void ResetUI()
        {
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            CPRTextBox.Clear();
            CommentTextBox.Clear();
            RRList.Clear();
            EKGLine.Values.Clear();
            LoadInitialCloudFiles();
            SearchDBTextBox.Text = "";
            SearchDBDropDownComboBox.ItemsSource = null;
            HideSaveButton(false);
            EditPatientButton.Content = EditString;
            AddJournalButton.IsEnabled = false;
            dbController.currentJournalId = 0;
            dbController.currentPatient = null;
        }

        private void AddJournalButton_Click(object sender, RoutedEventArgs e)
        {
            if (dbController.currentPatient != null && RRList.Count > 0 && dbController.currentJournalId==0)
            {
                CommentTextBox.IsEnabled = true;
                CommentTextBox.Text = "";
                dbController.SaveJournalToPatient(CommentTextBox.Text,RRList);
                ShowMessage("Journal Saved");
                AddJournalButton.IsEnabled = false;
                EditPatientButton.Content = EditString;
            }
            else
            {
                ShowMessage("Please choose a patient or new EKG");
            }
        }
        //If currentpatient !=null
    }
}
