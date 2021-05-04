using JewishCalculationWPF.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            JObject siJ = GetWhoisJO();
            lblCountry.Text = $"ip: {siJ["ip"]}; страна: {siJ["country"]}; город: {siJ["city"]}; валюта: {siJ["currency_code"]}";
            ImageSourceConverter isc = new ImageSourceConverter();
        }
        private JObject GetWhoisJO()
        {
            var locationResponse = new WebClient().DownloadString($"https://ipwhois.app/json/{(new WebClient().DownloadString("https://api.ipify.org"))}");
            return JObject.Parse(locationResponse);
        }
        private void Menu_Click(object sender, RoutedEventArgs e)//Не всегда срабатывает
        {
            if (Models.Products.Count == 0 || Models.Persons.Count == 0)
            {
                AddConsumption.IsEnabled = false;
            }
            else
            {
                AddConsumption.IsEnabled = true;
                if (Models.Consumptions.Count == 0) CreateXLSX.IsEnabled = false;
                else CreateXLSX.IsEnabled = true;
            } 
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
            DateTime dateTime = new DateTime(dpdateTime.SelectedDate.Value.Year, dpdateTime.SelectedDate.Value.Month, dpdateTime.SelectedDate.Value.Day, int.Parse(tbH.Text), int.Parse(tbM.Text), 0);


            //Check check = new CheckFromCheckRU(tbfiscal_mark.Text, tbstate_number.Text, double.Parse(tbsum.Text), dateTime, tbiCh.Text);
            //FromCheck fromCheck = check.GetFromCheck();

            //if (check.Done)
            //{
            //    MessageBox.Show("Данные с чека добавлены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //else MessageBox.Show("При загрузке данных с чека возникла ошибка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);


            GetProductsFromCheck getProductsFromCheck = new GetProductsFromCheck();
            getProductsFromCheck.GetCheck(tbfiscal_mark.Text, tbstate_number.Text, double.Parse(tbsum.Text), dateTime, tbiCh.Text);

            if (getProductsFromCheck.Done)
            {
                MessageBox.Show("Данные с чека добавлены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else MessageBox.Show("При загрузке данных с чека возникла ошибка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Menu_Click(sender, e);
        }
        private void AddConsumption_Click(object sender, RoutedEventArgs e)
        {
            AddConsumption addConsumption = new AddConsumption();
            addConsumption.Owner = this;
            addConsumption.ShowDialog();
            //if (Models.consumptions != null && Models.consumptions.Count != 0)
            //{

            //}
        }

        private void CreateXLSX_Click(object sender, RoutedEventArgs e)
        {
            ToExcel toExcel = new ToExcel();
            toExcel.GetExcel();
        }
    }
}
