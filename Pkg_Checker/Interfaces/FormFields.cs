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
        public const string F_DO178Level = @"R.DOLevel";
        public const string F_Lifecycle = @"R.Lifecycle";
        public const string F_ACMProject = @"R.Project";
        public const string F_ACMSubProject = @"R.SubProject";
        public const string F_RevParticipants = @"M.Number";
        public const string F_ModStamp = @"R.modChk";
        public const string F_ReviewStatus = @"R.Status";
        public const string F_LockStatus = @"lockStatus";
        public const string F_ReviewLocation = @"M.Location";
        public const string F_ProducerTechDefectCount = @"DT.Supplier";
        public const string F_ProducerNontechDefectCount = @"DNT.Supplier";
        public const string F_ProducerProcessDefectCount = @"DP.Supplier";
        public const string F_WorkProductType = @"WP.Artifacts";
        public const string F_ProducerLocation = @"R.SupplierLocation";                
        public const string F_TraceCheckList = @"CkList.1";        
        public const string F_CTP_Justification_1 = @"1.Text1"; // 有些 coversheet 中是这个名字
        public const string F_CTP_Justification_2 = @"Text1"; // 有些 coversheet 中却是这个名字
        public const string F_SLTP_Justification_1 = @"SLTP_Text1";
        public const string F_SLTP_Justification_2 = @"Text1";
        public const string F_Trace_Justification = @"Text_N_NA_Justification_12";        
        #endregion Coversheet fields

        #region Coversheet fields values
        public const string F_N_ReviewLocation_Val = @"No Meeting";
        public const string F_Lifecycle_Val_CTP = @"Low-Level Test Procedures";
        public const string F_Lifecycle_Val_SLTP = @"High-Level Test Procedures";
        public const string F_ReviewStatus_Val_Accepted = "Accepted";
        public const string F_ReviewStatus_Val_Revised = "Revise";
        public const string F_ReviewStatus_Val_Rereview = "ReReview";
        public const string F_ProducerLocation_Val = @"Shanghai (Avionics excl EDS)";
        #endregion Coversheet fields values

        #region regex patterns
        public const String PATTERN_SCR_NUMBER = @"\d{3,}\.\d{2}";
        public const String PATTERN_SCR_NUMBER_2 = @"\d{3,}_\d{2}";
        // FMS2000 : A350_A380 - SYSTEM CHANGE REQUEST
        public const String PATTERN_SCR_REPORT_BEGIN_MARK = @"(\w+)\s*:\s*(\w+)\s*-\s*SYSTEM CHANGE REQUEST";
        public const String PATTERN_SCR_REPORT_END_MARK = @"Closed in Config\.:\s*(\w{1,})";
        #endregion regex patterns
    }

    public enum CheckListType
    {
        CTP = 1,
        SLTP = 2,
        Trace = 3
    }
}
