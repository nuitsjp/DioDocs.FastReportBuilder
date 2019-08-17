using System;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    internal class JTokenAccessor
    {
        private readonly Func<JToken, object> _converter;

        private static readonly JTokenAccessor StringAccessor = new JTokenAccessor(x => x?.Value<string>());

        private static readonly JTokenAccessor DoubleAccessor = new JTokenAccessor(x => x?.Value<double>());

        private static readonly JTokenAccessor DateTimeAccessor = new JTokenAccessor(x => x?.Value<DateTime>());

        private JTokenAccessor(Func<JToken, object> converter)
        {
            _converter = converter;
        }


        internal object Get(JToken jToken)
        {
            return _converter(jToken);
        }

        internal static JTokenAccessor GetConverter(string type)
        {
            switch (type)
            {
                case "string":
                    return StringAccessor;
                case "double":
                    return DoubleAccessor;
                case "DateTime":
                    return DateTimeAccessor;
                default:
                    throw new NotSupportedException($"{type} is not supported. Supported type is string, double and DateTime.");
            }

        }
    }
}