using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace Site.Models {
    class DateTimeModelBinder : IModelBinder {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (bindingContext.ModelType == typeof(DateTime?)) {
                if (value == null || value.AttemptedValue == null || value.AttemptedValue == "") {
                    return new DateTime?();
                }
            }

            try {
                var date = value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);

                var holderType = bindingContext.ModelMetadata.ContainerType;
                bool hasFutureDateTimeAttribute = false; bool hasTimeAttribute = false;
                if (holderType != null) {
                    var propertyType = holderType.GetProperty(bindingContext.ModelMetadata.PropertyName);
                    var attributes = propertyType.GetCustomAttributes(true);
                    hasFutureDateTimeAttribute = attributes.Cast<Attribute>().Any(a => a.GetType().IsEquivalentTo(typeof(FutureDateTimeAttribute)));
                    hasTimeAttribute = attributes.Cast<Attribute>().Any(a => a.GetType().IsEquivalentTo(typeof(System.ComponentModel.DataAnnotations.UIHintAttribute)));
                }

                if (!hasFutureDateTimeAttribute && !hasTimeAttribute) {
                    if ((DateTime)date > DateTime.Now) {
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No se puede indicar una fecha mayor a hoy");
                    }
                }

                return date;
            }
            catch (Exception) {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "La fecha no es correcta");
                return value.AttemptedValue;
            }
        }

    }
}

