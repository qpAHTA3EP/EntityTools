using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EntityTools.Tools.ItemFilter
{
    public interface IFilterEntry : IXmlSerializable, IEquatable<IFilterEntry>, IComparable<IFilterEntry>
    {
        [Description("Тип идентификатора предмета")]
        ItemFilterEntryType EntryType { get; set; }

        [Description("Способ интерпретации строки, задающей идентификатор предмета:\n\r" +
            "Simple:\tПростой текст, допускающий подстановочный символ '*' в начале и в конце\n\r" +
            "Regex:\tРегулярное выражение")]
        ItemFilterStringType StringType { get; set; }

        [Description("Тип фильтра:\n\r" +
            "Include: предмет, попадающий под фильтр, подлежит обработке\n\r" +
            "Exclude: предмет, попадающий под фильтр, НЕ подлежит обработке")]
        ItemFilterMode Mode { get; set; }

        [Description("Шаблон с которым производится сопоставление выбранного признака предмета")]
        string Pattern { get; set; }

        [Description("Номер группы фильтров, в которую входит текущий")]
        uint Group { get; set; }

        bool IsMatch(InventorySlot slot);
        bool IsMatch(Item item);

        bool IsForbidden(InventorySlot slot);
        bool IsForbidden(Item item);

#if !event_PropertyChanged
        [Browsable(false)]
        Action<object, string> PropertyChanged { get; set; } 
#endif
        IFilterEntry AsReadOnly();

        IFilterEntry Clone();
    }
}
