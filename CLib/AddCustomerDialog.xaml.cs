using System.Windows;

namespace CLib
{
    public partial class AddCustomerDialog : Window
    {
        public Customers Customer { get; set; }

        public AddCustomerDialog()
        {
            InitializeComponent();
            Customer = new Customers();
            this.DataContext = Customer;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
