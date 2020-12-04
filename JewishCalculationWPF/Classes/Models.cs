using System;
using System.Collections.Generic;
using System.Linq;
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
       internal class Person
        {
            //internal int ID { get; set; }
            public string FIO { get; set; }
        }
        internal class Product
        {
            //internal int ID { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public double Quantity { get; set; }
        }
        internal class Consumption
        {
            public Person person { get; set; }
            public List<Product> products { get; set; }
        }
        internal static List<Person> persons = new List<Person>();
        internal static List<Product> products = new List<Product>();
        internal static List<Consumption> consumptions = new List<Consumption>();
    }
}
