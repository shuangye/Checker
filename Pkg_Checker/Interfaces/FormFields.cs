using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    public static class FormFields
    {
        #region Coversheet fields
        public const string F_LockStatus = @"lockStatus";
        public const string F_ReviewLocation = @"M.Location";
        public const string F_WorkProductType = @"WP.Artifacts";
        public const string F_Lifecycle = @"R.Lifecycle";
        public const string F_TraceCheckList = "TRC_CHK.ID";
        public const string F_ReviewStatus = "R.Status";
        #endregion Coversheet fields

        #region Coversheet fields values
        public const string F_ReviewLocation_Val = @"Moderator Cubicle";
        public const string F_Lifecycle_Val_CTP = @"R.Lifecycle";
        public const string F_Lifecycle_Val_SLTP = @"R.Lifecycle";
        public const string F_ReviewStatus_Val_Accepted = "Accepted";
        public const string F_ReviewStatus_Val_Revised = "Revise";
        #endregion Coversheet fields values
    }
}
