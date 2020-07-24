namespace Shop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IOrder
    {
        int Id { get; set; }

        int Quantity { get; set; }

        float Price { get; set; }
    }
}
