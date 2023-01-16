using System.Collections.Generic;
using CoreType.Types;

namespace CoreData.Common
{
    public static class SessionExtensions
    {
        public static int GetUserId(this SessionContext session) => session?.CurrentUser?.UserId ?? 0;
        public static int GetTenantId(this SessionContext session) => session?.CurrentUser?.XTenantId ?? session?.CurrentUser?.TenantId ?? 0;
        public static int? GetTenantType(this SessionContext session) => session?.CurrentUser?.XTenantId != null ? session.CurrentUser?.XTenantType : session?.CurrentUser?.TenantType;
        public static int? GetParentTenantId(this SessionContext session) => session?.CurrentUser?.XParentTenantId ?? session?.CurrentUser?.ParentTenantId;

        public static List<int> GetSubTenantIds(this SessionContext session) =>
            (session?.CurrentUser?.XTenantId != null ? session.CurrentUser?.XSubTenantIds : session?.CurrentUser?.SubTenantIds) ?? new List<int>();
    }
}