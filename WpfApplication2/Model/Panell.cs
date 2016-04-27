using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Model;

namespace WpfApplication2
{
    public class Panell : List<Cela>
    {

        public Panell()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Add(new Cela(this, col, row));
                }
            }
        }

    }
}
