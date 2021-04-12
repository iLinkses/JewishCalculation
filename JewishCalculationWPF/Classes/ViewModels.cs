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
            //private Models.Person person;
            private string secondName;
            private string firstName;
            private string lastName;
            private RelayCommand addCommand;

            #region Свойства
            public ObservableCollection<Models.Person> Persons { get; set; }
            public RelayCommand AddCommand
            {
                get
                {
                    return addCommand ?? (addCommand = new RelayCommand(obj =>
                    {
                        Models.Person person = new Models.Person
                        {
                            FIO = $"{(!secondName.Length.Equals(0) ? secondName : "")} {(!firstName.Length.Equals(0) ? firstName.Substring(0, 1) : "")}.{(!lastName.Length.Equals(0) ? lastName.Substring(0, 1) : "")}"
                        };
                        Persons.Add(person);
                    }));
                }
            }
            public string SecondName
            {
                get
                {
                    return secondName;
                }
                set
                {
                    secondName = value;
                    OnPropertyChanged("SecondName");
                }
            }
            public string FirstName
            {
                get
                {
                    return firstName;
                }
                set
                {
                    firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
            public string LastName
            {
                get
                {
                    return lastName;
                }
                set
                {
                    lastName = value;
                    OnPropertyChanged("LastName");
                }
            }
            #endregion

            public PersonViewModel()
            {
                Persons = new ObservableCollection<Models.Person>();
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
