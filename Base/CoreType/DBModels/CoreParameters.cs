using CoreType.Types;

namespace CoreType.DBModels
{
    public partial class CoreParameters
    {
        public int ParameterId { get; set; }
        public string KeyCode { get; set; }
        public int ParentValue { get; set; }
        public string Value { get; set; }
        public int? OrderIndex { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }
        public Translations Translations { get; set; }
    }
}