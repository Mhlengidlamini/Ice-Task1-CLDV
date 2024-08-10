﻿using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class CustomerProfile : ITableEntity
    {
        public string PartitionKey { get; set ; }
        public string RowKey { get ; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
