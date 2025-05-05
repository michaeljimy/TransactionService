using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Transaction_Service.Models
{
    public class TransactionTypeModel
    {
        public string TransactionTypeName { get; set; }
        public string TransactionTypeDescription { get; set; }

        public string TransactionTypeCode { get; set; }
    }

}
