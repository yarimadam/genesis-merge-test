using System;
using System.Collections.Generic;

namespace CoreType.Types
{
    public class PaginationWrapper<T>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public IList<T> List { get; set; } = new List<T>();
    }

    public class Pagination
    {
        public int CurrentPage { get; set; } = 1;
        public int MaxPage { get; set; }

        private int? _rowOffset;

        public int RowOffset
        {
            get => _rowOffset ??= (Math.Max(CurrentPage, 1) - 1) * MaxRowsPerPage;
            set => _rowOffset = value;
        }

        private int _maxRowsPerPage = Constants.ROW_COUNT_PER_PAGE;

        public int MaxRowsPerPage
        {
            get => _maxRowsPerPage;
            set
            {
                _maxRowsPerPage = Math.Min(Math.Max(value, 1), Constants.MAX_ROW_COUNT_PER_PAGE);
                _rowOffset = null;
            }
        }

        public int ResultRowCount { get; set; }

        private int _totalRowCount;

        public int TotalRowCount
        {
            get => _totalRowCount;
            set
            {
                _totalRowCount = value;
                MaxPage = (int) Math.Ceiling(_totalRowCount / (double) MaxRowsPerPage);
                if (TotalRowCount > 0 && ResultRowCount <= 0)
                    CurrentPage = MaxPage;
            }
        }
    }
}