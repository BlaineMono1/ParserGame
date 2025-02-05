using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// История Покупок Joy Пользователя 
    /// </summary>
    public class LoyaltyTransactionHistory:BaseEntity
    {
        #region поля 
        #endregion

        #region связи
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Ордера пользователя 
        /// </summary>
        public List<LoyaltyOrder> LoyaltyOrders { get; set; }
        #endregion
    }
}
