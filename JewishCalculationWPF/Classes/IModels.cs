using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewishCalculationWPF.Classes
{
    internal interface IPerson
    {
       string FIO { get; set; }
    }
    interface IProduct
    {
        string Name { get; set; }
        double Price { get; set; }
        double Quantity { get; set; }
    }
    interface IConsumption
    {
        IPerson person { get; set; }
        List<IProduct> products { get; set; }
    }
}
