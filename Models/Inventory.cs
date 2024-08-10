using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Inventory : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string ProductID { get; set; }
        public string StockLevel { get; set; }
        public string Location{ get; set; }
    }
}
