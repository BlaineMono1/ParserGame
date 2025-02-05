using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Business.Data.Iterfaces
{
    public interface IBaseEntity
    {
        // <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        Guid Guid { get; set; }

        /// <summary>
        /// Флаг удаления
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        bool IsDelete { get; set; }
    }
}
