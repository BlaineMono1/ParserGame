using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// История покупки пользователя 
    /// </summary>
    public class ProductTransactionItem:BaseEntity
    {
        #region связи
        /// <summary>
        /// История покупок пользователя 
        /// </summary>
        public Guid ProductTransactionHistoryId {  get; set; }
        [ForeignKey("ProductTransactionHistoryId")]
        public ProductTransactionHistory ProductTransactionHistory { get; set; }
        /// <summary>
        /// Ордера пользователя 
        /// </summary>
        public List<Order> Orders { get; set; }
        #endregion
    }
}
