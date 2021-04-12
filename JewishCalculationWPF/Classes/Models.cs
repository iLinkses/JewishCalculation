using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JewishCalculationWPF.Classes
{
    class Models
    {
        //internal class Person : IPerson
        //{
        //    int ID { get; set; }
        //    string IPerson.FIO { get ; set; }
        //}
        //internal class Product : IProduct
        //{
        //    int ID { get; set; }
        //    string IProduct.Name { get; set; }
        //    double IProduct.Price { get; set; }
        //    double IProduct.Quantity { get; set; }
        //}
        //internal class Consumption : IConsumption
        //{
        //    IPerson IConsumption.person { get; set; }
        //    List<IProduct> IConsumption.products { get; set; }
        //}
        internal class Person : INotifyPropertyChanged
        {
            private string fio;

            #region Свойства
            public string FIO
            {
                get { return fio; }
                set
                {
                    fio = value;
                    OnPropertyChanged("FIO");
                }
            }
            #endregion

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
        internal class Product : INotifyPropertyChanged
        {
            private string name;
            private double price;
            private double quantity;
            private double sum;

            #region Свойства
            //public string Name { get; set; }
            public string Name
            {
                get { return name; }
                set
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
            //public double Price { get; set; }
            public double Price
            {
                get { return price; }
                set
                {
                    price = value;
                    OnPropertyChanged("Price");
                }
            }
            //public double Quantity { get; set; }
            public double Quantity
            {
                get { return quantity; }
                set
                {
                    quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
            //public double Sum { get; set; }
            public double Sum
            {
                get { return sum; }
                set
                {
                    sum = value;
                    OnPropertyChanged("Sum");
                }
            }
            #endregion

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
        internal class Consumption : INotifyPropertyChanged
        {
            private Person _person;
            private List<Product> _products;

            #region Свойства
            //public Person person { get; set; }
            public Person person
            {
                get { return _person; }
                set
                {
                    _person = value;
                    OnPropertyChanged("person");
                }
            }
            //public List<Product> products { get; set; }
            public List<Product> products
            {
                get { return _products; }
                set
                {
                    _products = value;
                    OnPropertyChanged("products");
                }
            }
            #endregion

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }

            //public double Quantity { get; set; }
        }
        internal static List<Person> persons = new List<Person>();
        internal static List<Product> products = new List<Product>();
        internal static List<Consumption> consumptions = new List<Consumption>();
    }
}
