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
        }
        private void AddPerson_Click(object sender, RoutedEventArgs e)
        {
            Models.persons.Add(new Models.Person
            {
                FIO = $"{tbSecondName.Text} {tbFirstName.Text.Substring(0, 1)}.{tbLastName.Text.Substring(0,1)}"
            });
        }
    }
}
