using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class GroupAddOn:BaseEntity
    {
        #region поля
        /// <summary>
        /// Наименование подборки Дополнений 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Для загрузки изображения
        /// </summary>
        public string FilePathImage { get; set; }
        #endregion


        #region связи
        /// <summary>
        /// Дополнения 
        /// </summary>
        public List<AddOn>? AddOns { get; set; }
        #endregion
    }
}
