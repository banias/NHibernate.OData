﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Metadata;
using NHibernate.Type;

namespace NHibernate.OData
{
    internal class MappedClassMetadata
    {
        private readonly IDictionary<string, DynamicComponentProperty> caseSensitiveDynamicProperties = new Dictionary<string, DynamicComponentProperty>(StringComparer.Ordinal);
        private readonly IDictionary<string, DynamicComponentProperty> caseInsensitiveDynamicProperties = new Dictionary<string, DynamicComponentProperty>(StringComparer.OrdinalIgnoreCase);

        public MappedClassMetadata(IClassMetadata classMetadata)
        {
            Require.NotNull(classMetadata, "classMetadata");

            for (int i = 0; i < classMetadata.PropertyNames.Length; i++)
                BuildDynamicComponentPropertyList(classMetadata.PropertyNames[i], classMetadata.PropertyTypes[i]);
        }

        public DynamicComponentProperty FindDynamicComponentProperty(string fullPath, bool caseSensitive)
        {
            Require.NotNull(fullPath, "fullPath");

            var dictionary = caseSensitive ? caseSensitiveDynamicProperties : caseInsensitiveDynamicProperties;
            DynamicComponentProperty dynamicProperty;

            dictionary.TryGetValue(fullPath, out dynamicProperty);

            return dynamicProperty;
        }

        private void BuildDynamicComponentPropertyList(string name, IType type)
        {
            ComponentType component = type as ComponentType;
            if (component == null)
                return;

            bool isDynamicComponent = component.ReturnedClass == typeof(IDictionary);

            for (int i = 0; i < component.PropertyNames.Length; i++)
            {
                string fullName = string.Concat(name, ".", component.PropertyNames[i]);

                if (isDynamicComponent)
                {
                    var dynamicProperty = new DynamicComponentProperty(component.PropertyNames[i], component.Subtypes[i].ReturnedClass);

                    caseInsensitiveDynamicProperties[fullName] = dynamicProperty;
                    caseSensitiveDynamicProperties[fullName] = dynamicProperty;
                }

                BuildDynamicComponentPropertyList(fullName, component.Subtypes[i]);
            }
        }
    }
}
