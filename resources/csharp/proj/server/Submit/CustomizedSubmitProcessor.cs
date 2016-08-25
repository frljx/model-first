﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Restier.Core.Submit;

namespace %s.Submit
{
    public class CustomizedSubmitProcessor : IChangeSetItemFilter
    {
        private IChangeSetItemFilter Inner { get; set; }

        public Task OnChangeSetItemProcessingAsync(SubmitContext context, ChangeSetItem item, CancellationToken cancellationToken)
        {
            return Inner.OnChangeSetItemProcessingAsync(context, item, cancellationToken);
        }

        public Task OnChangeSetItemProcessedAsync(SubmitContext context, ChangeSetItem item, CancellationToken cancellationToken)
        {
            var dataModificationItem = item as DataModificationItem;
            if (dataModificationItem != null)
            {
                object myEntity = dataModificationItem.Resource;
                string entitySetName = dataModificationItem.ResourceSetName;
                var operation = dataModificationItem.DataModificationItemAction;

                // In case of insert, the request URL has no key, and request body may not have key neither as the key may be generated by database
                var keyAttrbiutes = new Dictionary<string, object>();
                var keyConvention = new Dictionary<string, object>();

                var entityTypeName = myEntity.GetType().Name;
                PropertyInfo[] properties = myEntity.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                        as KeyAttribute;
                    var propName = property.Name;
                    // This is getting key with Key attribute defined
                    if (attribute != null) // This property has a KeyAttribute
                    {
                        // Do something, to read from the property:
                        object val = property.GetValue(myEntity);
                        keyAttrbiutes.Add(propName, val);
                    }
                    // This is getting key based on convention
                    else if (propName.ToLower().Equals("id") || propName.ToLower().Equals(entityTypeName.ToLower() + "id"))
                    {
                        object val = property.GetValue(myEntity);
                        keyConvention.Add(propName, val);
                    }
                }
                if (keyAttrbiutes.Count > 0)
                {
                    // Use property with key attribute as keys    
                }
                else if (keyConvention.Count > 0)
                {
                    // Key is defined based on convention
                }
            }
            return Inner.OnChangeSetItemProcessedAsync(context, item, cancellationToken);
        }
    }
}