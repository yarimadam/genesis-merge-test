namespace CoreType.DBModels
{
    public partial class CoreDepartment
    {
        public int DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public string DepartmentName { get; set; }
        public int? ParentDepartmentId { get; set; }
        public int? DepHeadUserId { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }

        public virtual CoreCompany Company { get; set; }
        public virtual CoreUsers DepHeadUser { get; set; }
    }
}