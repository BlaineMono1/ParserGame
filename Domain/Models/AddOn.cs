using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class AddOn:BaseEntity
    {
        #region поля
        /// <summary>
        /// Куса код аддона
        /// </summary>
        public string CusaCode { get; set; }
        /// <summary>
        /// Наименование Аддона
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Тип объекта(AddOn)
        /// </summary>
        public string Type {  get; set; }
        /// <summary>
        /// Url изображения
        /// </summary>
        public string Image {  get; set; }
        /// <summary>
        /// Платформа игры (0 - ps4, 1- ps5, 2- ps4/ps5)
        /// </summary>
        public int Platform { get; set; }
      
        #endregion

        #region связи
        /// <summary>
        /// Id игры концепт
        /// </summary>
        public Guid GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        /// <summary>
        /// Id товара 
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        #endregion


    }
}
