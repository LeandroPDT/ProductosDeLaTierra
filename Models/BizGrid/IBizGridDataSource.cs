using System.Collections.Generic;

namespace System.Web.Helpers {
    internal interface IBizGridDataSource {
        int TotalRowCount { get; }

        IList<BizGridRow> GetRows(SortInfo sortInfo, int pageIndex);
    }
}
