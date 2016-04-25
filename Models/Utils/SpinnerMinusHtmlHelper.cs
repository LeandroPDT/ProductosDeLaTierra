using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Models {
    public static class SpinnerMinusHtmlHelper {        

        public static IHtmlString SpinnerMinus(string NombreControl) {
            string retval = string.Format(@"                   
                <a id='{0}_spinnerMinus' class='spinnerMinus icon-minus'> </a>",NombreControl);
            return MvcHtmlString.Create(retval);
        }

    }
}