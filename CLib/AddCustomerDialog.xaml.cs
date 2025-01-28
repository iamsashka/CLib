using System.Text.RegularExpressions;
using System.Windows;

namespace CLib
{
    public partial class AddCustomerDialog : Window
    {
        public Customers Customer { get; set; }
        /// <summary>
        /// Конструктор класса, который инициализирует компоненты окна и создает новый объект Customer типа Customers. 
        /// Также устанавливает контекст данных окна, связывая его с объектом Customer.
        /// </summary>
        public AddCustomerDialog()
        {
            InitializeComponent();
            Customer = new Customers();
            this.DataContext = Customer;
        }
        /// <summary>
        /// Метод, который выполняется при нажатии кнопки "Сохранить". Он проверяет корректность формата телефонного номера с помощью метода IsPhoneValid.
        /// Если номер некорректен, появляется сообщение об ошибке. Если номер корректен, устанавливается результат диалога в true, и окно закрывается.
        /// sender: объект, который вызывает этот метод (в данном случае кнопка, на которую нажали).
        /// e: данные события, содержащие информацию о событии(например, параметры события нажатия кнопки).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPhoneValid(Customer.Phone))
            {
                MessageBox.Show("Неверный формат номера телефона. Укажите номер в формате +7XXXXXXXXXX или 8XXXXXXXXXX.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// Метод, который выполняется при нажатии кнопки "Отмена". Он устанавливает результат диалога в false и закрывает окно.
        /// sender: объект, который вызывает этот метод (кнопка "Отмена").
        /// e: данные события, содержащие информацию о событии(например, параметры события нажатия кнопки).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        /// <summary>
        /// Метод, который проверяет корректность формата телефонного номера. 
        /// Он принимает строку и возвращает true, если номер соответствует шаблону формата "+7XXXXXXXXXX" или "8XXXXXXXXXX". 
        /// В противном случае возвращает false.
        /// phone: строка, представляющая телефонный номер, который требуется проверить.
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>Метод возвращает true, если номер телефона соответствует формату, и false в противном случае.</returns>
        private bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            
            var phonePattern = @"^(?:\+7|8)\d{10}$";
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}
