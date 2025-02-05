using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Настройка кешбэка при покупке Joy-ями
    /// </summary>
    public class LoyaltyCashback:BaseEntity
    {
        #region поля
        /// <summary>
        /// Процент при оплате Joy-ями
        /// </summary>
        public decimal Percent { get; set; }
        #endregion


        #region связи
        #endregion
    }
}
