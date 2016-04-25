using System;

namespace System.Web.Helpers {
    public class BizGridColumn {

        public bool CanSort { get; set; }
        public string ColumnName { get; set; }
        public Func<dynamic, object> Format { get; set; }
        public string Header { get; set; }
        public string Style { get; set; }
        public string SummaryOperation { get; set; }
        public string StandardStyle { get; set; }
        public bool IsHeaderCheckbox { get; set; }

    }

}
