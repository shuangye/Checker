﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pkg_Checker.Helpers;
using Pkg_Checker.Entities;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class Parser
    {
        [TestMethod]
        public void ParseSCRReport()
        {
            String target =
            #region SCR report Content
                            @"文件: C:\A350_TEST\9311_00.LIS 3/28/2014, 8:35:25 PM
                            FMS2000 : A350_A380 - SYSTEM CHANGE REQUEST Page 1
                            of 3
                            Change Category: PROBLEM SCR No.: P
                            09311.00
                            SCR Status: SEC SCR Status Date: 28-MAR-2014 Split SCRs:
                            1
                            Originator: O'Connor, Michael Date Originated:
                            14-FEB-2014
                            Affected Area: TEST_PERF Customer No.:
                            Assignee: Xiong, Sarah Priority: D3
                            Verification Assignee: Lin Ye
                            Found in Configuration: A350_CERT1_SDD_X04 Hardcopy Attachment: None
                            Target Configuration: A350_CERT1_TST_X05
                            Planned Impact: Test
                            Found During: CTP DEV REVIEW
                            Aircraft Affected: A350/A380
                            Task: N/A
                            CR1-F41 Type:
                            Earliest Applicabl: A350 S2
                            Risk of Regression: Low
                            SCR Copied To: < None Entered >
                            SCR Copied From:
                            A/M Project Subproject SCR
                            A FMS2000 A350_A380 9285.00
                            SCR Reissued To: < None Entered >
                            SCR Reissued From: < None Entered >
                            Title: Missing design conditions checks for Psperfreqst
                            Description:
                            ------------------------------------------------------------------
                            [11-Feb-2014] Syed Muqsith - HTSB
                            ----------------------------------
                            This SCR covers problems reported in the below AOs:
                            1. AO_A350_CTP_PERF_A350VNV_2730
                            -----------------------------------------------------------------
                            1. AO_A350_CTP_PERF_A350VNV_2730
                            -
                            Sys_Perf_Interface_Dpkg.Psperfreqst is used as decision condition
                            without relative requirement more than once in following
                            procedures, so there is a coverage hole.
                            The below code files uses the condition check of
                            Sys_Perf_Interface_Dpkg.Psperfreqst which is not mentioned in the
                            design requirement leading to coverage holes.
                            1) Prf_Maxalt_Pkg.Comp_Eo_Gdot_Max_Alt(PRF_MAXALT_PKG_COMP_EO_GDOT_MAX
                            _ALT.ADA; gen=5; PERF_CODE_03736): line 279,301,308.
                            2)
                            PRF_MAXALT_PKG.COMP_EO_GDOT_MAX_ALT.COMP_INITIAL_EO_GDOT_MAX_ALT
                            (PRF_MAXALT_PKG_COMP_EO_GDOT_MAX_ALT.ADA;gen=5): line 163.
                            3) PRF_MAXALT_PKG.EXECUTE (PRF_MAXALT_PKG_EXECUTE.ADA;gen=3;
                            PERF_CODE_2441): line155,159,207,231,239,261,268,314,332.
                            4) PRF_MAXALT_PKG.PREDICT(PRF_MAXALT_PKG_PREDICT.ADA;gen=7;
                            PERF_CODE_1909): line 116,156,193.
                            第 1 页
                            文件: C:\A350_TEST\9311_00.LIS 3/28/2014, 8:35:25 PM
                            5) PRF_MAXALT_PKG.CALC_REC_MAX_ALT(PRF_MAXALT_PKG_CALC_REC_MAX_ALT.ADA;
                            gen=2; PERF_CODE_1908): line 70,76.
                            第 2 页
                            文件: C:\A350_TEST\9311_00.LIS 3/28/2014, 8:35:25 PM
                            < Description field continued > SCR No. 09311.00 Page 2 of
                            3
                            6) PRF_HM_PKG.PREDICT (PRF_HM_PKG_PREDICT.ADA; gen=6;
                            PERF_CODE_2401): line 1035,1045,1058.
                            7) PRF_HM_PKG.PREDICT.PXDOPREDS (PRF_HM_PKG_PREDICT.ADA; gen=6): line 1014.
                            8) PRF_PREDTOALT_PKG_FIND_ALT_CSTR.ADA(gen=1,buildA01418) : line 113,293
                            9) PRF_LEGDIST_PKG_GET_GLEG_DATA.ADA(gen=27,A01418) : line
                            1694,1699,1772,1777,1819,1823,1863,1922,2065,2103,2229,2233,2276,
                            2331,2495,2836,2886,2920,2927,2950,2992
                            But only 4 anchors are mentioned with Sys_Perf_Interface_Dpkg.Psperfreqst:
                            PERF_SDD_0593, PERF_SDD_07872_INT, PERF_SDD_5726_INT, PERF_SDD_4807_INT
                            10)PRF_BKGND_PKG_PUT_BK_DATA.ADA(gen=8,A01418) : line 367,416
                            --
                            Additionally this issue applies to all the other SDD where in the
                            code Sys_Perf_Interface_Dpkg.Psperfreqst is used as a decision
                            condition.
                            Note: This issue was basically raised in
                            AO_A350_CTP_PDB_A350VNV_2501 but the issues in it was not captured
                            in SCR 8316.00.
                            ---------------------------------
                            System Impact: Incomplete/Incorrect Design, will cause a coverage hole
                            --
                            SRB Reviewed By: O'Connor, Michael Date: 14-FEB-2014
                            Analysis/Solution:
                            ---
                            Probable Solution:
                            A common design requirement can be written mentioning the condition
                            requirements of Sys_Perf_Interface_Dpkg.Psperfreqst when it is false
                            and when it is true.
                            Then all the modules including this checks can be traced to common
                            design requirement. This will avoid updating each and every design
                            requirment which maps to the code having the checks for that
                            variable.
                            ---
                            --------------------------------------
                            <27-Mar-2014>[E806250-HTSC]
                            Update the following zip file per for A350 on Class C0 build A01426.
                            CTP_A350_PERF_SYS_PERF_INTERFACE (ZIP:7)
                            CTP_A350_PERF_MAXALT (ZIP:12)
                            CTP_A350_PERF_HM_PKG (ZIP:10)
                            CTP_A350_PERF_CURMODE_PREDICT (ZIP:16)
                            CTP_A350_PERF_LEGDIST_GET_GLEG_DATA (ZIP:9)
                            --------------------
                            Reviewed with 9499.00
                            -------------------------------
                            <27-Mar-2014>[E806250-HTSC]
                            Self reword the following zip file per for A350 on Class C0 build A01426.
                            CTP_A350_PERF_MAXALT (ZIP:13)
                            CTP_A350_PERF_CURMODE_PREDICT (ZIP:17)
                            Elements Affected:
                            Doc. Element Generation
                            TEST CTP_A350_PERF_CURMODE_PREDICT.ZIP 16
                            TEST CTP_A350_PERF_CURMODE_PREDICT.ZIP 17
                            第 3 页
                            文件: C:\A350_TEST\9311_00.LIS 3/28/2014, 8:35:25 PM
                            TEST CTP_A350_PERF_HM_PKG.ZIP 10
                            第 4 页
                            文件: C:\A350_TEST\9311_00.LIS 3/28/2014, 8:35:25 PM
                            < Elements Affected field continued > SCR No. 09311.00 Page 3
                            of 3                            
                            ASSIGNEE: Xiong, Sarah Date:
                            28-MAR-2014
                            VERIFIER: Date:
                            CCB COORDINATOR:
                            Date:
                            Closure Category: Fixed/Added Duplicate SCR No.: 00000.00
                            Project Status: Done
                            Addendum:
                            Review Info:
                            Cert Concern: 250 - CC3/S3 DER Concern Item/Level 3
                            Cust Notification: 160 - CN3 Fix Commitment (Via Telex Or Management)
                            Inservice Incident: 0 - I1 None
                            FDE Distraction: 0 - FD1 None
                            Pilot Input: 0 - P1 None
                            Workload Wrkaround: 0 - W1 No Workaround Necessary
                            Must Fix: 0 - MF1 Use Score
                            Score/Comment: 410
                            Cause: N/A
                            Closed in Config.: A350_CERT1_TST_X05
                            第 5 页
                            ";
            #endregion SCR report Content
            SCRReport report = new SCRReport
            {
                SCRNumber = 9311.0f,
                Status = "SEC",
                AffectedArea = "TEST_PERF",
                TargetConfig = "A350_CERT1_TST_X05",
                ClosedConfig = "A350_CERT1_TST_X05",
                AffectedElements = new List<AffectedElement>()
            };
            report.AffectedElements.Add(new AffectedElement { 
                SCR = 9311.0f, CheckedInVer = 16,
                FileName = "CTP_A350_PERF_CURMODE_PREDICT.ZIP"
            });
            report.AffectedElements.Add(new AffectedElement
            {
                SCR = 9311.0f,
                CheckedInVer = 17,
                FileName = "CTP_A350_PERF_CURMODE_PREDICT.ZIP"
            });
            report.AffectedElements.Add(new AffectedElement
            {
                SCR = 9311.0f,
                CheckedInVer = 10,
                FileName = "CTP_A350_PERF_HM_PKG.ZIP"
            });

            SCRReport actualReport = Pkg_Checker.Helpers.Parser.ParseSCRReport(target);
            // maybe I need to implement the Equal method myself
            Assert.AreEqual(report, actualReport);
        }
    }
}
