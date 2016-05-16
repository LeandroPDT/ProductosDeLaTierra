using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Web.Helpers {
    /// <summary>
    /// Source wrapper for data provided by the user that is already sorted and paged. The user provides the BizGrid the rows to bind and additionally the total number of rows that 
    /// are available.
    /// </summary>
    internal sealed class PreComputedBizGridDataSource : IBizGridDataSource {
        private readonly int _totalRows;
        private readonly IList<BizGridRow> _rows;

        public PreComputedBizGridDataSource(BizGrid grid, IEnumerable<dynamic> values, int totalRows) {
            Debug.Assert(grid != null);
            Debug.Assert(values != null);

            _totalRows = totalRows;
            _rows = values.Select((value, index) => new BizGridRow(grid, value: value, rowIndex: index)).ToList();
        }

        public int TotalRowCount {
            get {
                return _totalRows;
            }
        }

        public IList<BizGridRow> GetRows(SortInfo sortInfo, int pageIndex) {
            // Data is already sorted and paged. Ignore parameters.
            return _rows;
        }
    }
}
