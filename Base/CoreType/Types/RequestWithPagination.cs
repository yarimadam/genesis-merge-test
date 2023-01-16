using System.Collections.Generic;

namespace CoreType.Types
{
    public class RequestWithPagination<T> where T : new()
    {
        private Pagination _pagination;
        public Pagination Pagination => _pagination ??= new Pagination();

        public T Criteria { get; set; } = new T();

        public GridCriterias GridCriterias { get; set; } = new GridCriterias();
    }

    public class GridCriterias
    {
        public List<SortModel> SortModel { get; set; } = new List<SortModel>();
        public List<FilterModel> FilterModel { get; set; } = new List<FilterModel>();
    }

    public class SortModel
    {
        public string PropertyName { get; set; }
        public string Order { get; set; }
    }

    public class FilterModel
    {
        public string PropertyName { get; set; }
        public string FilterType { get; set; }
        public string Operator { get; set; }
        public List<Condition> Conditions { get; set; } = new List<Condition>();
    }

    public class Condition
    {
        //public string FilterType { get; set; }
        public string TypeName { get; set; }
        public ConditionType Type { get; set; }
        public object[] Values { get; set; }
    }
}