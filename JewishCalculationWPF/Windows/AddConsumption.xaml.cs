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
    /// Логика взаимодействия для AddConsumption.xaml
    /// </summary>
    public partial class AddConsumption : Window
    {
        public AddConsumption()
        {
            InitializeComponent();
        }

        private void AddConsumption_Load(object sender, RoutedEventArgs e)
        {
            cbPersons.DisplayMemberPath = "FIO";
            cbPersons.ItemsSource = Models.persons;

            cbPersons.SelectedIndex = -1;

            dgProducts.ItemsSource = Models.products;
        }
    }
}
