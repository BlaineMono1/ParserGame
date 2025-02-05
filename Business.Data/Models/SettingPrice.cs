using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Настройка наценки для товара To Do Сделать наценку на разные регионы по разному 
    /// </summary>
    public class SettingPrice:BaseEntity
    {
        #region поля 
        /// <summary>
        /// От какой цены будет процент
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Наценка в процентах 
        /// </summary>
        public decimal Percent { get; set; }
        #endregion
    }
}
