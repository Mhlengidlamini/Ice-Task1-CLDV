using System.ComponentModel.DataAnnotations;

namespace ABCRetail.ViewModel
{
    public class ProductViewModel
    {
        public string Id { get; set; }


        public string ProductName { get; set; }

   
        public string Description { get; set; }

  
        public double Price { get; set; }

        public int StockLevel { get; set; }
    }
}
