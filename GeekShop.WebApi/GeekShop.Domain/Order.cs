using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? PhoneNumber { get; set; }
        public List<OrderDetails> Details { get; set; } = new List<OrderDetails>();
        public decimal? TotalCost // DO not save it to database and do not read 
        {
            get => Details.Sum(detail => detail?.Product?.Price * detail?.Quantity);
        }
    }
}
