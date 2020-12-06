using JewishCalculationWPF.Classes;
using System.Windows;

namespace JewishCalculationWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        public AddProduct()
        {
            InitializeComponent();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Models.products.Add(new Models.Product
            {
                Name = tbName.Text,
                Price = double.Parse(tbPrice.Text),
                Quantity = double.Parse(tbQuantity.Text),
                Sum = double.Parse(tbPrice.Text) * double.Parse(tbQuantity.Text)
            });
        }
    }
}
