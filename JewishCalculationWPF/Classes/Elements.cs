using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewishCalculationWPF.Classes
{
    public class Elements
    {
        public class Phone
        {
            public string Name { get; set; }
            public string Price { get; set; }
            public override string ToString()
            {
                return $"Смартфон {this.Name}; цена: {this.Price}";
            }
        }
    }
}
