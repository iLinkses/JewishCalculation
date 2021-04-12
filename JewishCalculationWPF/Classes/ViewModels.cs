using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JewishCalculationWPF.Classes
{
    class ViewModels
    {
        internal class PersonViewModel : INotifyPropertyChanged
        {
            private Models.Person person;
            public ObservableCollection<Models.Person> Persons { get; set; }
            public Models.Person Person
            {
                get { return person; }
                set
                {
                    person = value;
                    OnPropertyChanged("Person");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
        internal class ProductViewModel : INotifyPropertyChanged
        {
            private Models.Product product;
            public ObservableCollection<Models.Product> Products { get; set; }
            public Models.Product Product
            {
                get { return product; }
                set
                {
                    product = value;
                    OnPropertyChanged("Product");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
        internal class Consumption : INotifyPropertyChanged
        {
            private Models.Consumption _consumption;
            public ObservableCollection<Models.Consumption> Consumptions { get; set; }
            public Models.Consumption consumption
            {
                get { return _consumption; }
                set
                {
                    _consumption = value;
                    OnPropertyChanged("Consumption");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
