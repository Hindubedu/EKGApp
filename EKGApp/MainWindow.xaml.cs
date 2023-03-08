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

        bool fileLoaded = false;

        public MainWindow()
        {

            InitializeComponent();
            MyCollection = new SeriesCollection();
            EKGLine = new LineSeries();
            EKGLine.Values = new ChartValues<double> { };
            EKGLine.Fill = Brushes.Transparent;
            EKGLine.PointGeometry = null;
            MyCollection.Add(EKGLine);
            DataContext = this;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EKGLine.Values.Clear();
            RRList.Clear();
            int index = 0;
            // To download (on PC)
            Downloader downloader = new Downloader("F23_Gruppe_02"); // Create a Downloader instance with group name
            FileStream newLocalStream = new FileStream("pc_data.csv", FileMode.Create); // Create a new file
            downloader.Load("NormaltEKG.csv", newLocalStream); // Get data from the file specified 

            using (StreamReader reader = new StreamReader("pc_data.csv")) // Same procedure as last year? (Get data from the file)
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Console.WriteLine(line);

                    if (index > 1)
                    {
                        var splitLine = line.Trim().Split(','); //do we need to trim?
                        if (splitLine.Length == 2)
                        {
                            EKGLine.Values.Add(splitLine[1]); 
                        }
                        else
                        {
                            Debug.WriteLine($"Error in line: {index}, linesplit not working: {splitLine.Length} lines after split");
                        }
                    }
                    index++;
                }
            }
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)///Updates PulsTextBlock with a measurement of pulses/min (heartrate) if an EKG has been loaded 
        {
            int Rtak_old = 0;
            int Rtak_new = 0;
            double sample = 0.002;
            double diff;
            double threshold = 600; //Set carefully
            bool belowThreshold = true;

            if (fileLoaded== true)
            {
                RRList.Clear(); //Important otherwise your list is accumulative with each click and the time diff from one reading to next will do funky things.
                for (int i = 0; i < EKGLine.Values.Count; i++)
                {
                    if ((double)EKGLine.Values[i] > threshold && belowThreshold == true) //Obs Important to choose correct threshold otherwise with i.e. 1000 only the point triggering threshold will be recorded and hence skip a peak.
                    {
                        Rtak_new = i;

                        diff = (Rtak_new - Rtak_old) * sample; //samplerate 0.002 samples /s
                        RRList.Add(diff);
                        Rtak_old = i;
                        Debug.WriteLine($"Current line: {i}, Value: {EKGLine.Values[i].ToString()} Diff: {diff}");
                    }
                    if ((double)EKGLine.Values[i] < threshold)
                    {
                        belowThreshold = true;
                    }
                    else
                    {
                        belowThreshold = false;
                    }
                }
                RRList.RemoveAt(0);
                Debug.WriteLine($"Pulses recorded: {RRList.Count}");
                double Puls = 60 / RRList.Average();
                Puls = Math.Round(Puls);
                PulsTextBlock.Text = Puls.ToString(); 
            }
            else
            {
                PulsTextBlock.Text = "Please load an EKG";
            }
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            EKGLine.Values.Clear();
            RRList.Clear();
            int index = 0;

            using (StreamReader reader = new StreamReader("NormaltEKG.csv")) // Same procedure as every year...
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (index > 1)
                    {
                        var splitLine = line.Trim().Split(','); 
                        if (splitLine.Length == 2)
                        {
                            EKGLine.Values.Add(Double.Parse(splitLine[1]));
                        }
                        else
                        {
                            Debug.WriteLine($"Error in line: {index}, linesplit not working: {splitLine.Length} lines after split");
                        }
                    }
                    index++;
                }
            }
            fileLoaded = true;
        }
    }
}
