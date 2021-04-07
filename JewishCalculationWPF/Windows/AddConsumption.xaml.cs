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

            List<Models.Product> pList = new List<Models.Product>();

            foreach (Models.Product p in Models.products)
            {
                pList.Add(new Models.Product
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = 0,
                    Sum = p.Sum
                });
            }

            dgProducts.ItemsSource = pList;//Models.products;
        }

        private void AddConsumption_Click(object sender, RoutedEventArgs e)
        {
            List<Models.Product> products;
            if (cbPersons.Text.Equals(""))
            {
                MessageBox.Show("Для добавления выберите персону!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dgProducts.Items.OfType<Models.Product>().Count(p => !p.Quantity.Equals(0)) == 0)
            {
                MessageBox.Show("Для добавления введите потребление (количество) товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Models.consumptions.Count.Equals(0) && Models.consumptions.Count(c => c.person.FIO.Equals(cbPersons.Text)) > 0)
            {
                MessageBox.Show("Потребление пользователя уже было добавлено!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                //Добавить условие для выбора редактирования потребления пользователя.
            }

            products = dgProducts.Items.OfType<Models.Product>().Where(p => !p.Quantity.Equals(0)).ToList();//Условие для отбора тех продуктов, к которым персона имеет дело
            Models.consumptions.Add(new Models.Consumption
            {
                person = new Models.Person { FIO = Models.persons.Select(p => p.FIO = cbPersons.Text).FirstOrDefault() },
                products = products
            });

        }
    }
}
