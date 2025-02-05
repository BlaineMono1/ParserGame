using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Business.Data.Iterfaces;

namespace Business.Data.BaseEntities
{
    public class BaseEntity : Entity, IBaseEntity
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Флаг удаления
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public bool IsDelete { get; set; }
    }
}
