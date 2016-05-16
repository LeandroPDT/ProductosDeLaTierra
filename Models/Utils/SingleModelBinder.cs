using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Web.WebPages;


namespace Site.Models {
    public class SingleModelBinder : DefaultModelBinder {

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            dynamic valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null) {
                return base.BindModel(controllerContext, bindingContext);
            }

            return ((string)valueProviderResult.AttemptedValue).TryCSingle();
        }

    }


}