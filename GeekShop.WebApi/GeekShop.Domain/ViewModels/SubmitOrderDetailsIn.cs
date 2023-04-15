using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitOrderDetailsIn
    {
        public int ProductId { get; set; }

        public int ProductQuantity { get; set; }
    }
}
