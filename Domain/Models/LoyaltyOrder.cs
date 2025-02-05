using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Ордер на покупку Joy 
    /// </summary>
    public class LoyaltyOrder:BaseEntity
    {
        #region поля
        /// <summary>
        /// UserID tg
        /// </summary>
        public string TgUserId { get; set; }
        /// <summary>
        /// Кол-во покупаемых Joy
        /// </summary>
        public decimal CountProductJoy {  get; set; }
        /// <summary>
        /// Кол-во оплаты в рублях
        /// </summary>
        public decimal? AmountByRub {  get; set; }
        /// <summary>
        /// Кол-во оплаты в JouPlus
        /// </summary>
        public decimal? AmountByJoyPlus { get; set; }
        /// <summary>
        /// Номер заказа 
        /// </summary>
        public string CodeOrder {  get; set; }

        #endregion

        #region связи 
        /// <summary>
        /// История покупок Joy пользователя 
        /// </summary>
        public Guid LoyaltyTransactionHistoryId {  get; set; }
        [ForeignKey("LoyaltyTransactionHistoryId")]
        public LoyaltyTransactionHistory LoyaltyTransactionHistory { get; set; }
        #endregion
    }
}
