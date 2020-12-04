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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JewishCalculationWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void AddPerson_Click(object sender, RoutedEventArgs e)
        {
            AddPerson addPerson = new AddPerson();
            addPerson.Owner = this;
            //addPerson.Topmost = true;
            addPerson.ShowDialog();
        }
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct addProduct = new AddProduct();
            addProduct.Owner = this;
            //addProduct.Topmost = true;
            addProduct.ShowDialog();
        }
        private void btAddProduct_Click(object sender, RoutedEventArgs e)
        {
            Classes.GetProductsFromCheck getProductsFromCheck = new Classes.GetProductsFromCheck();
            DateTime dateTime = new DateTime(dpdateTime.SelectedDate.Value.Year, dpdateTime.SelectedDate.Value.Month, dpdateTime.SelectedDate.Value.Day, int.Parse(tbH.Text), int.Parse(tbM.Text), 0);
            getProductsFromCheck.GetCheck(tbfiscal_mark.Text, tbstate_number.Text, double.Parse(tbsum.Text), dateTime);
        }
        private void AddConsumption_Click(object sender, RoutedEventArgs e)
        {
            AddConsumption addConsumption = new AddConsumption();
            addConsumption.Owner = this;
            addConsumption.ShowDialog();
        }
    }
}
