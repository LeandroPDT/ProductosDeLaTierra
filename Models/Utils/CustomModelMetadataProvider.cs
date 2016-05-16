using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Site.Models
{
    public class CustomModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            ModelMetadata metadata = base.CreateMetadata(attributes,
                containerType,
                modelAccessor,
                modelType,
                propertyName);

            //Add MaximumLength to metadata.AdditionalValues collection
            var stringLengthAttribute = attributes.OfType<StringLengthAttribute>().FirstOrDefault();
            if (stringLengthAttribute != null)
                metadata.AdditionalValues.Add("MaxLength", stringLengthAttribute.MaximumLength);

            return metadata;
        }
    }
}