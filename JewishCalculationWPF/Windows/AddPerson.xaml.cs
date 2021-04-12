using JewishCalculationWPF.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JewishCalculationWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPerson.xaml
    /// </summary>
    public partial class AddPerson : Window
    {
        public AddPerson()
        {
            InitializeComponent();
            DataContext = new ViewModels.PersonViewModel();
        }
        private void AddPerson_Click(object sender, RoutedEventArgs e)
        {
            if (tbSecondName.Text.Length.Equals(0) && tbFirstName.Text.Length.Equals(0) && tbLastName.Text.Length.Equals(0))
            {
                MessageBox.Show("Для добавления введите данные пользователя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Models.persons.Add(new Models.Person
            {
                FIO = $"{(!tbSecondName.Text.Length.Equals(0) ? tbSecondName.Text : "")} {(!tbFirstName.Text.Length.Equals(0) ? tbFirstName.Text.Substring(0, 1) : "")}.{(!tbLastName.Text.Length.Equals(0) ? tbLastName.Text.Substring(0,1) : "")}"
            });
            if (MessageBox.Show("Пользователь добавлен!\nДобавить еще пользователя?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                tbSecondName.Clear();
                tbFirstName.Clear();
                tbLastName.Clear();
            }
            else this.Close();
        }
    }
}
