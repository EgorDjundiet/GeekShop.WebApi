using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Domain.Exceptions
{
    public class GeekShopValidationException : GeekShopException
    {
        public GeekShopValidationException(string message) : base(message)
        {
            
        }
    }
}
