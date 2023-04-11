using System.Collections;
using System.Reflection;

namespace ConversionUtilities
{
    public class EntityMappingService
    {

        /// <summary>
        /// Create a new record of type T mapping sources properties to target properties of same name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public T? Clone<T>(object source) where T : class, new()
        {
            return CloneLogic(source, typeof(T)) as T;
        }


        /// <summary>
        /// Apply properties from one object to the other. Objects must be of same type
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ApplyProperties(object source, object target)
        {
            List<PropertyInfo> targetProperties = target.GetType().GetProperties().ToList();
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList();


            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = targetProperties.SingleOrDefault(pi => pi.Name.ToLower() == sourceProperty.Name.ToLower());
                if (targetProperty != null)
                    targetProperty.SetValue(target, sourceProperty.GetValue(source));

            }
        }

        /// <summary>
        ///  Create a new record of type targetType mapping sources properties to target properties of same name
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object Clone(object source, Type targetType)
        {
            return CloneLogic(source, targetType);
        }


        /// <summary>
        /// Actual conversion logic 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object CloneLogic(object source, Type targetType)
        {
            object result = Activator.CreateInstance(targetType);
            List<PropertyInfo> targetProperties = targetType.GetProperties().ToList();
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList();


            //Loop through the object's properties
            foreach (var sourceProperty in sourceProperties)
            {

                var targetProperty = targetProperties.SingleOrDefault(pi => pi.Name.ToLower() == sourceProperty.Name.ToLower());
                var sourceValue = sourceProperty.GetValue(source);

                if (sourceValue == null || targetProperty == null)
                    continue;

                //If the source property is a List<> of any kind
                if (IsList(sourceProperty))
                {

                    //Create an instance of the target list
                    //var targetList = Activator.CreateInstance(targetProperty.GetValue(result).GetType());
                    var targetList = Activator.CreateInstance(targetProperty.PropertyType);

                    if (targetList == null)
                        continue;

                    //Convert the source value (object) to a List<object> so we can iterate through it
                    List<object> listItems = ((IEnumerable)sourceValue).Cast<object>().ToList();


                    var addMethod = targetList.GetType().GetMethod("Add");

                    if (addMethod == null)
                        throw new Exception($"{sourceProperty.Name}: List<> is not implementing Add()!");

                    foreach (var item in listItems)
                    {
                        var targetItem = Clone(item, GetListUnderlyingType(targetList));
                        addMethod.Invoke(targetList, new object[] { targetItem });
                    }

                    targetProperty.SetValue(result, targetList);
                }
                else
                {
                    //If the property value is primitive (scalar)
                    if (IsScalar(sourceValue))
                    {
                        targetProperty.SetValue(result, sourceValue);
                    }

                    //If the property value is an object
                    if (sourceValue.GetType().IsClass && sourceValue.GetType() != typeof(string))
                    {
                        var targetItem = Clone(sourceValue, targetProperty.PropertyType);
                        targetProperty.SetValue(result, targetItem);
                    }
                }

            }

            return result;
        }

       

        private bool IsScalar(object value)
        {
            var typeCode = Convert.GetTypeCode(value);
            var type = value.GetType();

            if (type == typeof(Guid) || type == typeof(DateTime))
                return true;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.String:
                    // scalar type
                    return true;
                default:
                    return false;
            }
        }


        /// <summary>
        /// Gets the Type with which a List<> was instanciated for.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Type GetListUnderlyingType(object list)
        {
            return list.GetType().UnderlyingSystemType.GenericTypeArguments.First();
        }

        /// <summary>
        /// Checks if input object is a List<> of any kind
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool IsList(PropertyInfo property)
        {
            return typeof(IEnumerable<object>).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string);
        }

        


    }
}
