using Azure;
using Azure.Data.Tables;
using Microsoft.VisualBasic;

namespace ABCRetail.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string CustomerID { get; set; }
        public DateAndTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
    }
}
