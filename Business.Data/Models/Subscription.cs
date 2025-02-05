using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class Subscription:BaseEntity
    {
        #region поля
        /// <summary>
        /// Куса код Подписки
        /// </summary>
        public string CusaCode { get; set; }
        /// <summary>
        /// Наименование Подписки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Тип объекта(Subscription)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Url изображения
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Платформа игры (0 - ps4, 1- ps5, 2- ps4/ps5)
        /// </summary>
        public int Platform { get; set; }


        #endregion

        #region связи
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public PriceSettingSubscription PriceSettingSubscription { get; set; }
        #endregion


    }
}
