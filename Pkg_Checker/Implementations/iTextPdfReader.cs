using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Pkg_Checker.Interfaces;
using Pkg_Checker.Entities;
using Pkg_Checker.Helpers;
using System.Text.RegularExpressions;

namespace Pkg_Checker.Implementations
{
    public class iTextPdfReader : IPdfReader
    {
        #region Properties
        public PdfReader Reader { get; set; }
        public AcroFields Fields { get; set; }
        public float SCRTolerance { get { return 0.001F; } }  // SCR is in XXXX.XX form

        public String ReviewPackageName { get; set; }
        public bool PackageIsLocked { get; set; }
        public bool HasTraceChecklist { get; set; }
        public String Aircraft { get; set; }
        public float F_ReviewID { get; set; }
        public float MasterSCR { get; set; }
        public char F_DO178Level { get; set; }
        public String F_ACMProject { get; set; }
        public String F_ACMSubProject { get; set; }
        public String F_FuncArea { get; set; }
        public int F_RevParticipants { get; set; }
        public int F_ProducerTechDefectCount { get; set; }
        public int F_ProducerNontechDefectCount { get; set; }
        public int F_ProducerProcessDefectCount { get; set; }
        public String F_ModStamp { get; set; }
        public String F_ReviewLocation { get; set; }
        public String F_WorkProductType { get; set; }
        public String F_Lifecycle { get; set; }
        public String F_ReviewStatus { get; set; }
        public String F_ProducerLocation { get; set; }
        public String F_CTP_Justification { get; set; }
        public String F_Trace_Justification { get; set; }

        public int TotalAcceptedDefectCount { get; set; }

        public List<SCRReport> SCRReports { get; set; }
        public List<CheckedInFile> CheckedInFiles { get; set; }
        public List<float> SCRs { get; set; }   // SCRs of "Work products under review"
        public List<String> BaseFileNames { get; set; }
        public List<String> ExtFileNames { get; set; }
        public List<String> PrintedFiles { get; set; }
        public List<String> Defects { get; set; }
        #endregion Properties

        /// <summary>
        /// Call this method before any checking methods to populate necessary data
        /// </summary>
        /// <param name="filePath"></param>
        public void Init(string filePath)
        {
            try
            {
                Defects = new List<String>();
                Reader = new PdfReader(filePath);
                Fields = Reader.AcroFields;
                ReviewPackageName = Path.GetFileName(filePath).ToUpper().Trim();
            }

            catch (Exception)
            {
                Reader = null;
                Fields = null;
                return;
            }            
        }

        public bool IsValidReviewPackage()
        {
            // 通过判断 Coversheet 的 Review ID 字段是否存在来判断是否为有效的 Review Package            
            return (null != Reader && null != Fields &&
                null != Reader.AcroFields.GetFieldItem(FormFields.F_ReviewID));            
        }

        /// <summary>        
        /// To avoid multiple pass traverses, collected all needed info in one pass.
        /// Collects SCR reports, .TRT file existance, and comments.
        /// </summary>
        public void ReadWholeFile()
        {
            if (null == Reader || null == Fields)
                return;

            const int maxFileCount = 40;
            Match match;
            String val = "";

            #region Parse Info from Review Package Name

            val = ReviewPackageName.Strip(@"FMS2000_A350_A380_")
                .Strip(@"FMS2000_A3XX_").Strip(@"FMS2000_MDXX_").Strip(@"B7E7_B7E7FMS_");
            try
            {
                match = Regex.Match(val, @"\d{1,}_\d{2}");
                if (match.Success)
                    MasterSCR = float.Parse(match.Value.Replace('_', '.'));
            }
            catch { MasterSCR = 0.0F; }

            if (ReviewPackageName.Contains(@"A350"))
                Aircraft = @"A350";  // A350
            else if (ReviewPackageName.Contains(@"A3XX"))
                Aircraft = @"A3XX";  // A380, A340, A320
            else if (ReviewPackageName.Contains(@"B7E7"))
                Aircraft = @"B7E7";  // B787
            else if (ReviewPackageName.Contains(@"MD"))
                Aircraft = @"MDXX";  // MD11

            #endregion Parse Info from Review Package Name

            #region Coversheet Fields

            AcroFields.Item reviewIDField = Reader.AcroFields.GetFieldItem(FormFields.F_ReviewID);
            if (null != reviewIDField)
            {
                var n = reviewIDField.GetMerged(0).GetAsNumber(PdfName.FF);
                PackageIsLocked = null != n && (n.IntValue & PdfFormField.FF_READ_ONLY) > 0;
            }

            HasTraceChecklist = null != Reader.AcroFields.GetFieldItem(FormFields.F_TraceCheckList);

            F_FuncArea = Fields.GetField(FormFields.F_FuncArea);
            val = Fields.GetField(FormFields.F_DO178Level);
            F_DO178Level = String.IsNullOrEmpty(val) ? ' ' : char.Parse(val);
            F_ACMProject = Fields.GetField(FormFields.F_ACMProject);
            F_ACMSubProject = Fields.GetField(FormFields.F_ACMSubProject);
            F_ModStamp = Fields.GetField(FormFields.F_ModStamp);
            F_ReviewLocation = Fields.GetField(FormFields.F_ReviewLocation);
            F_WorkProductType = Fields.GetField(FormFields.F_WorkProductType);
            F_Lifecycle = Fields.GetField(FormFields.F_Lifecycle);
            F_ReviewStatus = Fields.GetField(FormFields.F_ReviewStatus);
            F_ProducerLocation = Fields.GetField(FormFields.F_ProducerLocation);
            F_CTP_Justification = Fields.GetField(FormFields.F_CTP_Justification_1);
            if (String.IsNullOrWhiteSpace(F_CTP_Justification))
                F_CTP_Justification = Fields.GetField(FormFields.F_CTP_Justification_2);
            F_Trace_Justification = Fields.GetField(FormFields.F_Trace_Justification);

            // Review participants field may be filled as 3.0
            match = Regex.Match(Fields.GetField(FormFields.F_RevParticipants), @"\d{1,}");
            if (match.Success)
                F_RevParticipants = int.Parse(match.Value);

            // Review ID fields may be filled as 3912.02;4080.02
            try
            {
                F_ReviewID = float.Parse(Fields.GetField(FormFields.F_ReviewID)
                   .Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
            }
            catch { F_ReviewID = 0.0F; }
            try { F_ProducerTechDefectCount = int.Parse(Fields.GetField(FormFields.F_ProducerTechDefectCount)); }
            catch { F_ProducerTechDefectCount = 0; }
            try { F_ProducerNontechDefectCount = int.Parse(Fields.GetField(FormFields.F_ProducerNontechDefectCount)); }
            catch { F_ProducerNontechDefectCount = 0; }
            try { F_ProducerProcessDefectCount = int.Parse(Fields.GetField(FormFields.F_ProducerProcessDefectCount)); }
            catch { F_ProducerProcessDefectCount = 0; }

            #endregion Coversheet Fields

            #region Checked in Files

            CheckedInFiles = new List<CheckedInFile>();
            ExtFileNames = new List<string>();
            BaseFileNames = new List<string>();
            SCRs = new List<float>();
            String scr, fileName, checkedInVer, approvedVer;
            for (int i = 1; i <= maxFileCount; ++i)
            {
                scr = Fields.GetField("F" + i + ".SCR");
                fileName = Fields.GetField("F" + i + ".Name");
                checkedInVer = Fields.GetField("F" + i + ".Ver");
                approvedVer = Fields.GetField("F" + i + ".ApprovedVer");
                if (!String.IsNullOrWhiteSpace(scr))
                {
                    if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(checkedInVer)
                        || String.IsNullOrWhiteSpace(approvedVer))
                    {
                        Defects.Add(@"Incomplete info for checked in file " + i);
                        continue;
                    }

                    CheckedInFile checkedInFile = new CheckedInFile { FileName = fileName.Trim().ToUpper() };
                    try { checkedInFile.SCR = float.Parse(scr); }
                    catch { checkedInFile.SCR = 0.0F; }
                    try { checkedInFile.CheckedInVer = int.Parse(checkedInVer); }
                    catch { checkedInFile.CheckedInVer = 0; }
                    try { checkedInFile.ApprovedVer = int.Parse(approvedVer); }
                    catch { checkedInFile.ApprovedVer = 0; }

                    if (checkedInFile.ApprovedVer < checkedInFile.CheckedInVer)
                        Defects.Add(@"Approved ver is less than checked in ver for file " + checkedInFile.FileName);

                    CheckedInFiles.Add(checkedInFile);
                    String ext = Path.GetExtension(checkedInFile.FileName);
                    ExtFileNames.Add(ext);
                    BaseFileNames.Add(checkedInFile.FileName.Substring(0, checkedInFile.FileName.LastIndexOf(ext)));
                    SCRs.Add(checkedInFile.SCR);
                }
            }
            ExtFileNames = ExtFileNames.Distinct().ToList();
            BaseFileNames = BaseFileNames.Distinct().ToList();
            SCRs = SCRs.Distinct().ToList();

            #endregion Checked in Files

            // is SCR report found for one specific SCR? (consider checking in / inserting more than one SCRs)
            // List<KeyValuePair<float, bool>> SCRReportFound = new List<KeyValuePair<float, bool>>();
            // is the TRT file present for one specific CTP? (consider more than one CTPs in one review package)
            // List<KeyValuePair<String, bool>> TRTFileFound = new List<KeyValuePair<string, bool>>();

            SCRReports = new List<SCRReport>();
            PrintedFiles = new List<string>();            
            String KEYWORKDS;

            for (int page = 1; page <= Reader.NumberOfPages; ++page)
            {
                SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                String pageText = PdfTextExtractor.GetTextFromPage(Reader, page, strategy);
                if (String.IsNullOrWhiteSpace(pageText))
                    continue;

                #region Parse SCR Report
                // SCR report and "Affected Elements" therein may span multiple pages,
                // and the page number of SCR report is not one-to-one mapping with the PDF page.
                // So locate the SCR report by keywords.
                // Begin: SYSTEM CHANGE REQUEST Page # of #
                // End: Closed in Config.: ***                
                match = Regex.Match(pageText, @"SYSTEM CHANGE REQUEST\s*Page\s*\d+\s*of\s*\d+");
                if (match.Success)
                {
                    SCRReport report = new SCRReport();
                    report.BeginPageInPackage = page;
                    String SCRPageContent = "";
                    for (report.EndPageInPackage = page; report.EndPageInPackage <= Reader.NumberOfPages; ++report.EndPageInPackage)
                    {
                        SCRPageContent = PdfTextExtractor.GetTextFromPage(Reader, report.EndPageInPackage, strategy);

                        // Change Category: PROBLEM SCR No.: P 17011.01                        
                        // Change Category: INITIAL DEVELOPMENT SCR No.:  08982.04                        
                        match = Regex.Match(SCRPageContent, @"Change Category:\s*\w{1,}\s*SCR No.:\s*[A-Z]*\s*\d{3,}\.\d{2}");
                        if (match.Success)
                            report.SCRNumber = float.Parse(
                                Regex.Match(match.Value.Strip(@"Change Category:").Strip("PROBLEM").Strip("SCR No.:").Trim(),
                                @"\d{3,}\.\d{2}").Value);

                        // SCR Status: SEC
                        match = Regex.Match(SCRPageContent, @"SCR Status:\s*[A-Z]{3}");
                        if (match.Success)
                            report.Status = match.Value.Strip("SCR Status:").Trim();

                        // Affected Area: VGUIDE
                        match = Regex.Match(SCRPageContent, @"Affected Area:\s*\w{2,}");
                        if (match.Success)
                            report.AffectedArea = match.Value.Strip("Affected Area:").Trim();

                        // Target Configuration: A350_CERT1_TST_X04
                        match = Regex.Match(SCRPageContent, @"Target Configuration:\s*\w{2,}");
                        if (match.Success)
                            report.TargetConfig = match.Value.Strip("Target Configuration:").Trim();

                        #region Elements Affected
                        // Begin: Elements Affected:
                        // End: Closure Category:
                        // Note: The Elements Affected area may span multiple pages
                        String elementsAffectedArea = "";
                        KEYWORKDS = @"Elements Affected:";
                        int elementsAffectedBeginLocation = -1;
                        int elementsAffectedEndLocation = -1;
                        if (SCRPageContent.IndexOf(KEYWORKDS) >= 0)
                        {
                            elementsAffectedBeginLocation = SCRPageContent.IndexOf(KEYWORKDS) + KEYWORKDS.Length;
                            KEYWORKDS = @"Closure Category:";
                            if (SCRPageContent.IndexOf(KEYWORKDS) >= 0)
                            {
                                // Elements Affected 在一页内显示完整
                                elementsAffectedEndLocation = SCRPageContent.IndexOf(KEYWORKDS);
                                elementsAffectedArea += SCRPageContent.Substring(elementsAffectedBeginLocation,
                                    elementsAffectedEndLocation - elementsAffectedBeginLocation);
                            }
                            else
                                elementsAffectedArea += SCRPageContent.Substring(elementsAffectedBeginLocation);
                        }
                        else
                        {
                            KEYWORKDS = @"Closure Category:";
                            if (SCRPageContent.IndexOf(KEYWORKDS) >= 0)
                                elementsAffectedArea += SCRPageContent.Substring(0, SCRPageContent.IndexOf(KEYWORKDS));
                            else
                                elementsAffectedArea += SCRPageContent;
                        }

                        // Having located the "Affected Elements" area, it is time now to parse info therein
                        if (!String.IsNullOrWhiteSpace(elementsAffectedArea))
                        {
                            report.AffectedElements = new List<CheckedInFile>();
                            CheckedInFile checkedInFile = null;
                            foreach (String line in elementsAffectedArea.ToUpper().Split("\r\n".ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries))
                            {
                                // C:\A350_TEST\9311_00.LIS 3/28/2014, 8:35:25 PM
                                // Filter the file name like 9311_00.LIS in printed SCR report header
                                if (line.IndexOfAny(@":\/,".ToCharArray()) >= 0)
                                    continue;

                                // match a line like "TEST CTP_A350_FMCI_EVENT_HANDLER.ZIP 4"
                                match = Regex.Match(line, @"\w{1,}\s*\w{5,}\.\w{3}\s*\d{1,}");
                                if (match.Success)
                                {
                                    // match a file name like TEST CTP_A350_FMCI_EVENT_HANDLER.ZIP
                                    match = Regex.Match(line, @"\w{3,}\.\w{3}");
                                    if (match.Success)
                                    {
                                        checkedInFile = new CheckedInFile();
                                        checkedInFile.SCR = report.SCRNumber;
                                        checkedInFile.FileName = match.Value;

                                        // File name may also contain digits, e.g., A350, MD11
                                        match = Regex.Match(line, checkedInFile.FileName + @"\s*\d{1,}");
                                        if (match.Success)
                                            checkedInFile.CheckedInVer = int.Parse(match.Value.Strip(checkedInFile.FileName));

                                        report.AffectedElements.Add(checkedInFile);
                                    }
                                }
                            }
                        }
                        #endregion Elements Affected

                        // Closed in Config.: MD11_922_TST
                        match = Regex.Match(SCRPageContent, @"Closed in Config.:\s*\w{2,}");
                        if (match.Success)
                        {
                            report.ClosedConfig = match.Value.Strip("Closed in Config.:").Trim();
                            break;  // reach the end of the SCR report
                        }
                    }
                    SCRReports.Add(report);
                    // page += report.EndPageInPackage - report.EndPageInPackage;  // skip this SCR report
                    continue;
                }
                #endregion Parse SCR Report

                #region Collect Prerequisite Files
                // .TRT files                
                foreach (Match matchItem in Regex.Matches(pageText, @"TRACE FILENAME\s*:\s*\S{1,}\.TRT", RegexOptions.IgnoreCase))
                    PrintedFiles.Add(matchItem.Value.ToUpper().Strip(@"TRACE FILENAME").Trim().Strip(@":").Trim());
                #endregion Collect Prerequisite Files

                #region Collect Comments

                #region Annotation state model

                //// Standard Defect State Model setup - 
                //// This code defines the Annotation state models in support of Review Defect Statusing.
                //// It defines Two models:
                //// 1. Defect Review State.  This provides a state of defect acceptance.
                //// 2. Defect Status.  This provides a status of the defect resolution.
                //// 3. Defect Status (default Adobe Review state is deleted)

                //// First, delete any old state models,

                //// Defect type model
                //try { Collab.removeStateModel("DefectType");    } catch (e) {}
                //// Qualifier model
                //try { Collab.removeStateModel("QualifierType"); } catch (e) {}
                //// Impact model
                //try { Collab.removeStateModel("Impact");        } catch (e) {}
                //// Delete Adobe default state model - don't need it anymore
                //try { Collab.removeStateModel("Review");        } catch (e) {}

                //// Create the new state models

                //// Defect Review State model
                //var myIsDefectType = new Object;
                //myIsDefectType["None"] = {cUIName: "None"};
                //myIsDefectType["Accepted"] = {cUIName: "Accepted"};
                //myIsDefectType["Nondefect"] = {cUIName: "Not a Defect"};
                //myIsDefectType["Duplicate"] = {cUIName: "Duplicate"};
                //Collab.addStateModel({cName: "Is Defect State", cUIName: "IsDefectState",oStates: myIsDefectType, Default: "Accepted"});

                //// Defect Status model
                //var myResolutionStatusType = new Object;
                //myResolutionStatusType["None"] = {cUIName: "None"};
                //myResolutionStatusType["In Work"] = {cUIName: "In Work"};
                //myResolutionStatusType["Work Completed"] = {cUIName: "Work Completed"};
                //myResolutionStatusType["Need Additional Rework"] = {cUIName: "Need Additional Rework"};
                //myResolutionStatusType["Verified Complete"] = {cUIName: "Verified Complete"};
                //Collab.addStateModel({cName: "Resolution Status", cUIName: "ResolutionStatus",oStates: myResolutionStatusType, Default: "In Work"});

                //// Defect Classification state model
                //var myDefectType = new Object;
                //myDefectType["DR"] = {cUIName: "Driving Requirement"};
                //myDefectType["FN"] = {cUIName: "Functionality"};
                //myDefectType["IF"] = {cUIName: "Interface"};
                //myDefectType["LA"] = {cUIName: "Language"};
                //myDefectType["LO"] = {cUIName: "Logic"};
                //myDefectType["MN"] = {cUIName: "Maintainability"};
                //myDefectType["PF"] = {cUIName: "Performance"};
                //myDefectType["ST"] = {cUIName: "Standards"};
                //myDefectType["OT"] = {cUIName: "Other"};
                //myDefectType["ND"] = {cUIName: "Documentation:NT"};
                //myDefectType["PD"] = {cUIName: "Documentation:P"};
                //myDefectType["TE"] = {cUIName: "Incomplete Test Execution:T"};
                //myDefectType["TI"] = {cUIName: "Incorrect Stubbing:T"};
                //myDefectType["PR"] = {cUIName: "Review Packet Deficiency:P"};
                //myDefectType["TS"] = {cUIName: "Structural Coverage:T"};
                //myDefectType["NS"] = {cUIName: "Structural Coverage:NT"};
                //myDefectType["TC"] = {cUIName: "Test case:T"};
                //myDefectType["NC"] = {cUIName: "Test case:NT"};
                //myDefectType["PG"] = {cUIName: "Test Generation System warnings:P"};
                //myDefectType["TT"] = {cUIName: "Trace:T"};
                //myDefectType["NT"] = {cUIName: "Trace:NT"};
                //myDefectType["PT"] = {cUIName: "Trace:P"};
                //Collab.addStateModel({cName: "DefectType", cUIName: "Defect Type",oStates: myDefectType});

                //// Defect Classification state model
                //var myDefectSeverity = new Object;
                //myDefectSeverity["Minor"] = {cUIName: "Minor"};
                //myDefectSeverity["Major"] = {cUIName: "Major"};
                //Collab.addStateModel({cName: "DefectSeverity", cUIName: "Defect Severity",oStates: myDefectSeverity});

                #endregion Annotation state model

                // How to collect all state models that belong to one sticky note?
#warning One comment may be accepted by multiple people
                PdfDictionary pdfDict = Reader.GetPageN(page);  // read one page once
                if (null != pdfDict)
                {
                    PdfArray annotArray = pdfDict.GetAsArray(PdfName.ANNOTS);
                    if (null != annotArray)
                    {
                        for (int i = 0; i < annotArray.Size; ++i)
                        {
                            PdfDictionary curAnnot = annotArray.GetAsDict(i);

                            // SUBTYPE values: /Widget, /Text, /Popup, /StrikeOut, /Stamp
                            PdfName annotSubtype = (PdfName)curAnnot.Get(PdfName.SUBTYPE);

                            // StateModel values: DefectType, Resolution Status, DefectSeverity, Is Defect State, Marked
                            PdfString annotStateType = (PdfString)curAnnot.Get(new PdfName("StateModel"));

                            // PdfName.STATE values: ND, ST, NC, Accepted, Minor, Unmarked, In Work, Work Completed, Verified Complete, 
                            PdfString annotState = (PdfString)curAnnot.Get(PdfName.STATE);

                            if (PdfName.TEXT == annotSubtype)
                            {
                                if (null != annotState && annotState.ToString().IndexOf("Accepted") >= 0)
                                    ++TotalAcceptedDefectCount;
                            }
                        }
                    }
                }

                #endregion collect comments
            }
        }

        public void CheckCommonFields()
        {
            if (null == Fields)
                return;

            const int maxRevParticipants = 38;

            if (Math.Abs(F_ReviewID - MasterSCR) > SCRTolerance)
                Defects.Add(@"Review ID does not match the review package name.");

            // DO-178 level
            if (("A350".Equals(Aircraft) || "B7E7".Equals(Aircraft) || "MDXX".Equals(Aircraft))
                && F_DO178Level != 'B')
                Defects.Add(@"DO-178 Level is " + F_DO178Level + "; expected B.");
#warning "A380 也是 A3XX, 但是 DO178 level B"
            else if ("A3XX".Equals(Aircraft) && F_DO178Level != 'C')
                Defects.Add(@"DO-178 Level is " + F_DO178Level + "; expected C.");

            // ACM project and subproject
            if (@"A350".Equals(Aircraft) && (!@"FMS2000".Equals(F_ACMProject) || !@"A350_A380".Equals(F_ACMSubProject)))
                Defects.Add(@"ACM project is " + F_ACMProject + ", subproject is " + F_ACMSubProject
                    + "; expected FMS2000 and A350_A380, respectively.");
            if (@"A3XX".Equals(Aircraft) && (!@"FMS2000".Equals(F_ACMProject) || !@"A3XX".Equals(F_ACMSubProject)))
                Defects.Add(@"ACM project is " + F_ACMProject + ", subproject is " + F_ACMSubProject
                    + "; expected FMS2000 and A3XX, respectively.");
            if (@"B7E7".Equals(Aircraft) && (!@"B7E7".Equals(F_ACMProject) || !@"B7E7FMS".Equals(F_ACMSubProject)))
                Defects.Add(@"ACM project is " + F_ACMProject + ", subproject is " + F_ACMSubProject
                    + "; expected B7E7 and B7E7FMS, respectively.");
            if (@"MDXX".Equals(Aircraft) && (!@"FMS2000".Equals(F_ACMProject) || !@"MDXX".Equals(F_ACMSubProject)))
                Defects.Add(@"ACM project is " + F_ACMProject + ", subproject is " + F_ACMSubProject
                    + "; expected FMS2000 and MDXX, respectively.");

            // Review location
            if (String.IsNullOrWhiteSpace(F_ReviewLocation) || !F_ReviewLocation.Equals(FormFields.F_ReviewLocation_Val))
                Defects.Add(@"Review Location is " + F_ReviewLocation + "; expected " + FormFields.F_ReviewLocation_Val);

            // Moderator stamp
            if (String.IsNullOrWhiteSpace(F_ModStamp) || !"Yes".Equals(F_ModStamp))
                Defects.Add(@"Moderator did not stamp on the package.");

            // Review participants
            int revParticipants = 0;
            for (int i = 1; i <= maxRevParticipants; ++i)
            {
                if ("Yes".Equals(Fields.GetField("E" + i + ".Sign")))
                    ++revParticipants;
            }
            if (F_RevParticipants != revParticipants)
                Defects.Add(@"Number of review participants is " + F_RevParticipants + ", but there is/are " + revParticipants + " stamp(s).");
            if (3 > revParticipants)
                Defects.Add(@"Review participants shoud be at least three.");
        }

        public void CheckReviewStatus()
        {
            if (!PackageIsLocked)
            {
                Defects.Add(@"Review package is not locked.");
                return;
            }

            int filledDefect = F_ProducerTechDefectCount | F_ProducerNontechDefectCount | F_ProducerProcessDefectCount;

            // Accepted as is
            if (FormFields.F_ReviewStatus_Val_Accepted.Equals(F_ReviewStatus))
            {
                if (0 != filledDefect)
                    Defects.Add(@"According to the coversheet fillings, there is at least one defect, but the package is closed as "
                        + F_ReviewStatus);

                foreach (var checkedInFile in CheckedInFiles)
                {
                    if (checkedInFile.CheckedInVer != checkedInFile.ApprovedVer)
                        Defects.Add(@"Review status is " + F_ReviewStatus
                            + @", but the approved ver is not equal to the checked in ver of file "
                            + checkedInFile.FileName);
                }
            }

            // Revise (no further review): at least 1 defect
            else if (FormFields.F_ReviewStatus_Val_Revised.Equals(F_ReviewStatus) ||
                FormFields.F_ReviewStatus_Val_Rereview.Equals(F_ReviewStatus))
            {
                if (0 == filledDefect)
                    Defects.Add(@"According to the coversheet fillings, there are no defects, but the package is closed as"
                        + F_ReviewStatus);

                if (TotalAcceptedDefectCount != F_ProducerNontechDefectCount + F_ProducerTechDefectCount
                    + F_ProducerProcessDefectCount)
                    Defects.Add(@"Total defects count is not equal to actual accepted comments count.");

                bool reworkFound = false;
                foreach (var checkedInFile in CheckedInFiles)
                {
                    if (checkedInFile.CheckedInVer < checkedInFile.ApprovedVer)
                    {
                        reworkFound = true;
                        break;
                    }
                }
                if (!reworkFound)
                    Defects.Add(@"Review status is " + F_ReviewStatus + @", but there seems to be no rework.");
            }

            else
                Defects.Add(@"Review status is not valid.");
        }

        public void CheckWorkProducts()
        {
            if (null == Fields)
                return;

            #region Work Product Type

            if (String.IsNullOrWhiteSpace(F_WorkProductType))
                Defects.Add(@"Work Product Type is empty.");
            else
            {
                // CTP. A CTP update may only contain .ZIP updates, and not .TDF updates.
                if ("Low-Level Test Procedures".Equals(F_Lifecycle))
                {
                    if (!F_WorkProductType.Contains("Component Test"))
                        Defects.Add(@"Work Product Type does not match Low-Level Test Procedure.");

                    // CTP review package 不能包含 SLTP 相关的内容
                    if (F_WorkProductType.Contains("Software Test") || ExtFileNames.Contains(@".TST"))
                        Defects.Add(@"Low-Level Test Procedure should not contain SLTP related files or Work Product Types.");
                }

                // SLTP
                if ("High-Level Test Procedures".Equals(F_Lifecycle))
                {
                    if (!F_WorkProductType.Contains(@"Software Test") || !ExtFileNames.Contains(@".TST"))
                        Defects.Add(@"Work Product Type does not match High-Level Test Procedure");

                    // SLTP review package 不能包含 CTP 相关的内容
                    if (F_WorkProductType.Contains("Component Test") || ExtFileNames.Contains(@".TDF"))
                        Defects.Add(@"High-Level Test Procedure should not contain CTP related files or Work Product Types.");
                }

                // Trace
                if (F_WorkProductType.Contains(@"Trace Data") ^ ExtFileNames.Contains(".TRT"))
                    Defects.Add(@"Tracing: Work Product Type and checked in files do not match.");
                // if (ExtFileNames.Contains(".TRT") ^ HasTraceChecklist)
                //     Defects.Add(@"Missing Trace Check List, or the .TRT file is not updated but the Trace check list presents.");
                if (ExtFileNames.Contains(".TRT") && !HasTraceChecklist)
                    Defects.Add(@"Missing Trace Checklist.");
                if (!ExtFileNames.Contains(".TRT") && HasTraceChecklist)
                    Defects.Add(@"The Trace Checklist is redundant because no trace file is updated.");
            }

            #endregion Work Product Type

            #region Prerequisite Files
            // .TRT files
            foreach (var item in BaseFileNames)
            {
                if (!PrintedFiles.Contains(item + @".TRT"))
                    Defects.Add(item + @".TRT is not printed to the review package.");
            }
            #endregion Prerequisite Files

            #region ACM Info vs. Coversheet Info

            // Approved ver >= checked in ver has been checked inside Read() method
            // Now check Checked in ver == max ver in ACM, and take the files in coversheet as a standard.
            foreach (var checkedInFile in CheckedInFiles)
            {
                SCRReport matchingSCRReport = SCRReports.FirstOrDefault(
                    x => Math.Abs(x.SCRNumber - checkedInFile.SCR) < SCRTolerance);
                if (null != matchingSCRReport)
                {
                    IEnumerable<CheckedInFile> matchingFiles =
                        matchingSCRReport.AffectedElements.Where(x => x.FileName.Equals(checkedInFile.FileName));
                    if (null != matchingFiles && matchingFiles.Count() > 0)
                    {
                        int maxACMver = matchingFiles.OrderByDescending(x => x.CheckedInVer).FirstOrDefault().CheckedInVer;
                        if (maxACMver != checkedInFile.CheckedInVer)
                            Defects.Add(@"The coversheet indicates that the checked in ver of file "
                                + checkedInFile.FileName + " is " + checkedInFile.CheckedInVer
                                + ", but the max ver in SCR report is " + maxACMver
                                + ". SCR: " + checkedInFile.SCR);
                    }
                    else
                        Defects.Add(@"The SCR report indicates that file " + checkedInFile.FileName
                            + " is not checked into SCR " + checkedInFile.SCR);
                }
            }

            #endregion ACM Info vs. Coversheet Info
        }

        /// <summary>
        /// Parse the justifications to find items that are not disposed
        /// </summary>
        /// <param name="justifications">Justifications to be parsed</param>
        /// <param name="itemsNoOrNA">A list of NO or N/A items</param>
        /// <returns>A list of not disposed items</returns>
        private List<int> ParseJustifications(String justifications, List<int> itemsNoOrNA)
        {
            if (String.IsNullOrWhiteSpace(justifications) || itemsNoOrNA.Count < 1)
                return itemsNoOrNA;

            List<int> notDisposedItems = new List<int>(itemsNoOrNA);

            Match match;
            foreach (var item in itemsNoOrNA)
            {
                foreach (var line in justifications.Split("\r\n".ToCharArray()))
                {
                    String[] parsedItems;

                    // For item(s) 12 - 15 or For point(s) 12-15
                    // 匹配下一种形式的，也会匹配这一种形式。这种形式更 specific, 故把它放在前面。
                    match = Regex.Match(line, @"(items?|points?)+\s*(\d{1,2}\s*-\s*\d{1,2})+", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        parsedItems = match.Value.ToUpper().Strip("ITEMS").Strip("ITEM").Trim()
                            .Split("- ".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                        if (null != parsedItems && parsedItems.Length > 1)
                            if (item >= int.Parse(parsedItems[0]) &&
                                item <= int.Parse(parsedItems[1]))
                                // itemsNoOrNA.Remove(item);                                    
                                notDisposedItems.Remove(item);
                        // 匹配了 For item(s) 1 - 3 就不能再匹配 Form item(s) 1, 2, 3... 了
                        continue;
                    }

                    // For item(s) 1
                    // For item 12, 13...
                    match = Regex.Match(line, @"(items?|points?)+\s*(\d{1,2}[\s,]*)+", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        parsedItems = match.Value.ToUpper().Strip("ITEMS").Strip("ITEM").Trim()
                            .Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var itemNumber in parsedItems)
                            if (item == int.Parse(itemNumber))
                                notDisposedItems.Remove(item);
                        // itemsNoOrNA[item.Key] = true;  // disposed
                        // the Value property is readonly, so set it to a new KeyValuePair
                        // itemsNoOrNA[item.Key] = new KeyValuePair<int, bool>(item.Key, true);
                        // itemsNoOrNA.Remove(item);                           
                    }
                }
            }

            return notDisposedItems;
        }

        /// <summary>
        /// Check CTP check list and trace check list.
        /// </summary>
        public void CheckCheckList()
        {
            const int CTPCheckListItemCount = 45;
            const int traceCheckListItemCount = 8;
            List<int> itemsNotChecked = new List<int>();
            List<int> itemsNoOrNA = new List<int>();
            List<int> notDisposedItems;
            // List<KeyValuePair<int, bool>> itemsNoOrNA = new List<KeyValuePair<int, bool>>();  // <item, isDisposed>
            // Dictionary<int, bool> itemsNoOrNA = new Dictionary<int, bool>();            

            if (null == Fields)
                return;

            #region CTP Check List

            for (int i = 1; i <= CTPCheckListItemCount; ++i)
            {
                // "Yes", "No", "NA", ""
                String fieldVal = Fields.GetField("CTP." + i);
                if (String.IsNullOrWhiteSpace(fieldVal))
                    itemsNotChecked.Add(i);
                else if (fieldVal.Contains('N'))
                    // itemsNoOrNA.Add(i, false);
                    itemsNoOrNA.Add(i);
            }

            if (itemsNotChecked.Count() > 0)
            {
                string temp = @"Item(s) ";
                foreach (int i in itemsNotChecked)
                    temp += i + " ";
                Defects.Add(temp + "is/are not checked in CTP check list.");
            }

            notDisposedItems = ParseJustifications(F_CTP_Justification, itemsNoOrNA);
            if (null != notDisposedItems && notDisposedItems.Count > 0)
            {
                String temp = "";
                foreach (var item in notDisposedItems)
                    temp += item + " ";
                Defects.Add(@"No justification for NO or N/A item(s) " + temp 
                    + " in CTP check list, or the justification is not in proper form.");
            }

            #endregion CTP Check List

            itemsNotChecked.Clear();
            itemsNoOrNA.Clear();

            #region Trace Check List

            if (HasTraceChecklist)
            {
                for (int i = 1; i <= traceCheckListItemCount; ++i)
                {
                    // "Yes", "No", "NA", ""
                    String fieldVal = Fields.GetField("CkList." + i);
                    if (String.IsNullOrWhiteSpace(fieldVal))
                        itemsNotChecked.Add(i);
                    else if (fieldVal.Contains('N'))
                        // itemsNoOrNA.Add(i, false);
                        itemsNoOrNA.Add(i);
                }

                if (itemsNotChecked.Count() > 0)
                {
                    string temp = @"Item(s) ";
                    foreach (int i in itemsNotChecked)
                        temp += i + " ";
                    Defects.Add(temp + "is/are not checked in Trace check list.");
                }

                notDisposedItems = ParseJustifications(F_Trace_Justification, itemsNoOrNA);
                if (null != notDisposedItems && notDisposedItems.Count > 0)
                {
                    String temp = "";
                    foreach (var item in notDisposedItems)
                        temp += item + " ";
                    Defects.Add(@"No justification for NO or N/A item(s) " + temp
                        + " in Trace check list, or the justification is not in proper form.");
                }
            }

            #endregion Trace Check List
        }

        public List<String> GetDefects()
        {
            return Defects;
        }

        public void Close()
        {
            if (null == Reader)
                return;

            Reader.Close();
        }
    }
}
