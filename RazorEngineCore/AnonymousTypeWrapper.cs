using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;

namespace RazorEngineCore
{
    [RequiresDynamicCode("Creating a call site may require dynamic code generation.")]
    public class AnonymousTypeWrapper : DynamicObject
    {
        private readonly object model;

        public AnonymousTypeWrapper(object model)
        {
            this.model = model;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var type = model.GetType();
            var propertyInfo = type.GetProperty(binder.Name);

            if (propertyInfo == null)
            {
                result = null;
                return false;
            }

            result = propertyInfo.GetValue(model, null);

            if (result == null)
            {
                return true;
            }

            if (result.IsAnonymous())
            {
                result = new AnonymousTypeWrapper(result);
            }

            if (result is IDictionary dictionary)
            {
                foreach(var key in dictionary.Keys)
                {
                    var value = dictionary[key];
                    if (value != null && value.IsAnonymous())
                    {
                        dictionary[key] = new AnonymousTypeWrapper(value);
                    }
                }
            }
            else if (result is IEnumerable enumerable and not string)
            {
                result = enumerable.Cast<object>()
                        .Select(e => e.IsAnonymous() ? new AnonymousTypeWrapper(e) : e)
                        .ToList();
            }
            
            return true;
        }
    }
}