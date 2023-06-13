using LogicLayer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EKGApp
{
    /// <summary>
    /// Interaction logic for NewPatientWindow.xaml
    /// </summary>
    public partial class NewPatientWindow : Window
    {
        private DBController _dbController = new DBController();

        public NewPatientWindow()
        {
            InitializeComponent();
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

        private void NameTextBoxes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\P{L}+"); // Should prohibit to any letter
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CPRTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckuserInput()==false)
            {
                return;
            }
            var saved = _dbController.SavePatient(FirstNameTextBox.Text, LastNameTextBox.Text, CPRTextBox.Text.Trim());
            if (saved==false)
            {
               ShowMessage("CPR already exists");
               return;
            }
            ShowMessage("PatientSaved");
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            CPRTextBox.Text = "";
        }

        private async void ShowMessage(string message)
        {
            MessageTextBox.Text = message;

            await Task.Delay(3000);

            MessageTextBox.Text = "";
        }

        private bool CheckuserInput()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text)||string.IsNullOrWhiteSpace((LastNameTextBox.Text)))
            {
                ShowMessage("Please fill out name");
                return false;
            }

            if (CPRTextBox.Text.Trim().Length!=10)
            {
                ShowMessage("CPR must be 10 digits");
                return false;
            }

            if (FirstNameTextBox.Text=="First name"||LastNameTextBox.Text=="Last name")
            {
                ShowMessage("Please fill out name");
                return false;
            }

            return true;
        }
    }
}
