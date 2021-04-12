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
            if (tbName.Text.Length.Equals(0) || tbPrice.Text.Length.Equals(0) || tbQuantity.Text.Length.Equals(0))
            {
                MessageBox.Show("Для добавления введите данные товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Models.Products.Add(new Models.Product
            {
                Name = tbName.Text,
                Price = double.Parse(tbPrice.Text),
                Quantity = double.Parse(tbQuantity.Text),
                Sum = double.Parse(tbPrice.Text) * double.Parse(tbQuantity.Text)
            });
            if (MessageBox.Show("Товар добавлен!\nДобавить еще товар?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                tbName.Clear();
                tbPrice.Clear();
                tbQuantity.Clear();
            }
            else this.Close();
        }
    }
}
