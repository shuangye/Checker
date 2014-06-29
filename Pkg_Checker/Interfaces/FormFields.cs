using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    public static class FormFields
    {
        #region Coversheet fields
        public const string F_ReviewID = @"R.Ref_ID";
        public const string F_FuncArea = @"R.Farea";
        public const string F_RevParticipants = @"M.Number";
        public const string F_ModStamp = @"R.modChk";
        public const string F_ReviewStatus = @"R.Status";
        public const string F_LockStatus = @"lockStatus";
        public const string F_ReviewLocation = @"M.Location";
        public const string F_WorkProductType = @"WP.Artifacts";
        public const string F_Lifecycle = @"R.Lifecycle";
        public const string F_TraceCheckList = @"TRC_CHK.ID";        
        public const string F_Justifications_1 = @"1.Text1"; // 有些 coversheet 中是这个名字
        public const string F_Justifications_2 = @"Text1"; // 有些 coversheet 中却是这个名字
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
