using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class Product
    {
        #region поля
       
        /// <summary>
        /// Тип объекта Издания/AddOn/Подписки
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Цена в гривнах
        /// </summary>
        public decimal? PriceUa { get; set; }
      
        /// <summary>
        /// Цена в лирах
        /// </summary>
        public decimal? PriceTr {  get; set; }
      
        /// <summary>
        /// Процент скидки
        /// </summary>
        public string DiscountPercent {  get; set; }

        /// <summary>
        /// Длительность скидки 
        /// </summary>
        public DateTime? DiscountDate { get; set; }


        #endregion

        #region связи
  
        #endregion

    }
}
