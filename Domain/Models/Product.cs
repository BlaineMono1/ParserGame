using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class Product:BaseEntity
    {
        #region поля
        /// <summary>
        /// Id Издания/AddOn/Подписки
        /// </summary>
        public Guid TypeId { get; set; }
        /// <summary>
        /// Тип объекта Издания/AddOn/Подписки
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Цена в гривнах
        /// </summary>
        public decimal? PriceUa { get; set; }
        /// <summary>
        /// Цена по скидке в гривнах
        /// </summary>
        public decimal? DiscountUa { get; set; }
        /// <summary>
        /// Цена в лирах
        /// </summary>
        public decimal? PriceTr {  get; set; }
        /// <summary>
        ///  Цена по скидке в лирах
        /// </summary>
        public decimal? DiscountTr { get; set; }
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
        public AddOn AddOn { get; set; }
        public Subscription Subscription { get; set; }
        public Edition Edition { get; set; }
        #endregion

    }
}
