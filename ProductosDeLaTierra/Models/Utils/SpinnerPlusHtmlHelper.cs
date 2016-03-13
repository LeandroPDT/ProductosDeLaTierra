using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Models {
    public static class SpinnerPlusHtmlHelper {        

        public static IHtmlString SpinnerPlus(string NombreControl) {
            string retval = string.Format(@"                   
                <a id='{0}_spinnerPlus' class='spinnerPlus icon-plus'> </a>",NombreControl);
            return MvcHtmlString.Create(retval);
        }

    }
}