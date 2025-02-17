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
        public string CusaCodeUa { get; set; }
        public string CusaCodeTr { get; set; }


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
