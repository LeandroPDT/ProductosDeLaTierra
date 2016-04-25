using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Globalization;

namespace Site.Models {
    public class IntegerModelBinder : DefaultModelBinder {

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            dynamic valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null) {
                return base.BindModel(controllerContext, bindingContext);
            }


            return ((string)valueProviderResult.AttemptedValue).AsInt();
        }

    }


}