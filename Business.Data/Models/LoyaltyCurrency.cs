using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Баланс Joy и JoyPlus пользователя
    /// </summary>
    public class LoyaltyCurrency:BaseEntity
    {
        #region поля 
        /// <summary>
        /// Joy
        /// </summary>
        public decimal BalanceJoy { get; set; } = default(decimal);
        /// <summary>
        /// JoyPlus
        /// </summary>
        public decimal BalanceJoyPlus {  get; set; } = default(decimal);
        #endregion

        #region связи 
        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }
        #endregion
    }
}
