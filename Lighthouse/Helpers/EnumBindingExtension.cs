using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Lighthouse.Helpers
{
    class EnumBindingExtension : MarkupExtension
    {
        public Type Type { get; }

        public EnumBindingExtension(Type type)
        {
            if (type == null || !type.IsEnum) throw new Exception();

            Type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(Type);
        }
    }
}
