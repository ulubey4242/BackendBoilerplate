using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Core.Entities;

namespace Core.Infrastructure.Mapper
{
    public static class PropertyMapper
    {
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        public sealed class IgnoreMappingAttribute : Attribute { public IgnoreMappingAttribute() { } }

        private static readonly Dictionary<Type, IList<PropertyInfo>> typeDictionary = new Dictionary<Type, IList<PropertyInfo>>();

        private static IList<PropertyInfo> GetPropertiesFor(Type type)
        {
            if (!typeDictionary.ContainsKey(type))
                typeDictionary.Add(type, type.GetProperties().ToList());

            return typeDictionary[type];
        }
        private static IList<PropertyInfo> GetPropertiesFor<T>()
        {
            return GetPropertiesFor(typeof(T));
        }
        private static IList<PropertyInfo> GetPropertiesForEntity(IEntity entity)
        {
            return GetPropertiesFor(entity.GetType());
        }
        private static IList<PropertyInfo> GetPropertiesForModel(IDto model)
        {
            return GetPropertiesFor(model.GetType());
        }

        public static TModel ToDto<TModel>(this IEntity entity) where TModel : IDto
        {            
            return CreateDtoFromEntity<TModel>(entity);
        }
        public static TEntity ToEntity<TEntity>(this IDto model) where TEntity : IEntity
        {
            return CreateEntityFromDto<TEntity>(model);
        }

        private static TModel CreateDtoFromEntity<TModel>(IEntity entity) where TModel : IDto
        {
            TModel item = (TModel)Activator.CreateInstance(typeof(TModel));

            IList<PropertyInfo> sourceProperties = GetPropertiesForEntity(entity);
            IList<PropertyInfo> targetProperties = GetPropertiesFor<TModel>();

            foreach (var property in targetProperties)
            {
                var sourceProperty = sourceProperties
                                        .Where(x => x.Name == property.Name)
                                        .FirstOrDefault();

                if (sourceProperty == null)
                    continue;

                var sourceValue = sourceProperty.GetValue(entity);

                SetPropertyValue(item, property , sourceValue);
            }

            return item;
        }
        private static TEntity CreateEntityFromDto<TEntity>(IDto model) where TEntity : IEntity
        {
            TEntity item = (TEntity)Activator.CreateInstance(typeof(TEntity));

            IList<PropertyInfo> sourceProperties = GetPropertiesForModel(model);
            IList<PropertyInfo> targetProperties = GetPropertiesFor<TEntity>();

            foreach (var property in targetProperties)
            {
                var sourceProperty = sourceProperties
                                        .Where(x => x.Name == property.Name)
                                        .FirstOrDefault();

                if (sourceProperty == null)
                    continue;

                var sourceValue = sourceProperty.GetValue(model);

                SetPropertyValue(item, property, sourceValue);
            }

            return item;
        }

        private static void SetPropertyValue(object item, PropertyInfo property, object sourceValue)
        {
            var atr = property.GetCustomAttribute(typeof(IgnoreMappingAttribute));

            if (atr == null && sourceValue != null)
            {
                try
                {
                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        DateTime.TryParse(sourceValue.ToString(), out DateTime date);

                        if (sourceValue.ToString() != "")
                            property.SetValue(item, date, null);
                        else
                            property.SetValue(item, "", null);
                    }
                    else if (sourceValue.GetType() == typeof(DBNull))
                        property.SetValue(item, "", null);
                    else if (property.PropertyType == typeof(string))
                        property.SetValue(item, sourceValue.ToString(), null);
                    else
                        property.SetValue(item, sourceValue, null);
                }
                catch { }
            }
        }
    }
}
