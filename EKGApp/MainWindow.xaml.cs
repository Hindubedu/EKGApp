﻿using FileShare;
using global::RPI;
using global::RPI.Controller;
using global::RPI.Display;
using global::RPI.Heart_Rate_Monitor;
using global::RPI.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using LogicLayer;

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
        Loader loader = new Loader();

        DBController dbController = new DBController();



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

        int Rtak_old = 0;
        int Rtak_new = 0;
        double sample = 500;
        double diff;
        double threshold = 0.6; //Set carefully
        bool belowThreshold = true;

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
        }

        private void LoadNewestButton_Click(object sender, RoutedEventArgs e)
        {
            EKGLine.Values.Clear();
            RRList.Clear();

            var values = loader.LoadNewestFromCloud();

            EKGLine.Values.AddRange(values);
            RRList.AddRange(values.Cast<double>());
            fileLoaded = true;
        }

        private void CommentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CommentTextBox.Text = "";
        }

        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
        }

        private void CPRTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CPRTextBox.Text = "";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadioButton = (RadioButton)sender;

            if (selectedRadioButton == DBRadioButton)
            {
                EKGMeasurementCombobox.Items.Clear();
                DBSearchTextBox.Visibility=Visibility.Visible;
                PatientComboBox.Visibility=Visibility.Visible;
                EKGMeasurementCombobox.Visibility = Visibility.Hidden;
                PatientComboBox.IsDropDownOpen=true;


            }
            else if (selectedRadioButton == CloudRadioButton)
            {
                EKGMeasurementCombobox.Items.Clear();
                DBSearchTextBox.Visibility = Visibility.Hidden;
                PatientComboBox.Visibility = Visibility.Hidden;
                loader.GetFileNames().ForEach(x => EKGMeasurementCombobox.Items.Add(x));
            }
        }

        private void DBSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DBSearchTextBox.Text = "";
        }

        private void SaveToDBButton_Click(object sender, RoutedEventArgs e)
        {
            dbController.SavePatientToDB();
        }

        private void DBSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = DBSearchTextBox.Text;
            List<string> dbSearchResults = dbController.SearchDBForPatients(searchText);
            PatientComboBox.ItemsSource = dbSearchResults;
        }

        private void PatientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientComboBox.SelectedItem != null)
            {
                string selectedResult = PatientComboBox.SelectedItem.ToString();
                dbController.LoadPatientFromDB(selectedResult);
                EKGMeasurementCombobox.Visibility = Visibility.Visible;
                EKGMeasurementCombobox.ItemsSource = dbController.GetPatientJournals();
            }
        }
    }
}
