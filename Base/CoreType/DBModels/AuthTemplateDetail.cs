namespace CoreType.DBModels
{
    public partial class AuthTemplateDetail
    {
        public int AuthTemplateDetailId { get; set; }
        public int AuthTemplateId { get; set; }
        public int ResourceId { get; set; }
        public int ActionId { get; set; }
        public int Status { get; set; }

        public virtual AuthTemplate AuthTemplate { get; set; }
    }
}