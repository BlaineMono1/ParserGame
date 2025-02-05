using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Настройка скидки при оплате в Joy
    /// </summary>
    public class LoyaltySetting:BaseEntity
    {
        #region поля 
        /// <summary>
        /// От какой цены будет считаться скидка
        /// </summary>
        public decimal PriceValue { get; set; }
        /// <summary>
        /// Процент скидки 
        /// </summary>
        public decimal DiscountPercent { get; set; }

        #endregion

        #region связи
        #endregion
    }
}
