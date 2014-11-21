#undef COMMENT_COLLECTION_METHOD_1
#undef DEBUG_VERBOSE

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
        // public bool PackageIsLocked { get; set; }
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
        public String F_SLTP_Justification { get; set; }
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

        ~iTextPdfReader()
        {
            if (null != Reader)
                Reader.Close();
        }

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
            const String SCR_PATTERN = @"\d{3,}\.\d{2}";
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

            // AcroFields.Item reviewIDField = Reader.AcroFields.GetFieldItem(FormFields.F_ReviewID);
            // if (null != reviewIDField)
            // {
            //     var n = reviewIDField.GetMerged(0).GetAsNumber(PdfName.FF);
            //     PackageIsLocked = null != n && (n.IntValue & PdfFormField.FF_READ_ONLY) > 0;
            // }

            HasTraceChecklist = null != Reader.AcroFields.GetFieldItem(FormFields.F_TraceCheckList);

            F_FuncArea = Fields.GetField(FormFields.F_FuncArea);
            val = Fields.GetField(FormFields.F_DO178Level);
            F_DO178Level = String.IsNullOrEmpty(val) ? ' ' : char.Parse(val);
            F_ACMProject = Fields.GetField(FormFields.F_ACMProject);
            if (null != F_ACMProject) F_ACMProject = F_ACMProject.ToUpper();
            F_ACMSubProject = Fields.GetField(FormFields.F_ACMSubProject);
            if (null != F_ACMSubProject) F_ACMSubProject = F_ACMSubProject.ToUpper();
            F_ModStamp = Fields.GetField(FormFields.F_ModStamp);
            F_ReviewLocation = Fields.GetField(FormFields.F_ReviewLocation);
            F_WorkProductType = Fields.GetField(FormFields.F_WorkProductType);
            F_Lifecycle = Fields.GetField(FormFields.F_Lifecycle);
            F_ReviewStatus = Fields.GetField(FormFields.F_ReviewStatus);
            F_ProducerLocation = Fields.GetField(FormFields.F_ProducerLocation);
            F_CTP_Justification = Fields.GetField(FormFields.F_CTP_Justification_1);
            if (String.IsNullOrWhiteSpace(F_CTP_Justification))
                F_CTP_Justification = Fields.GetField(FormFields.F_CTP_Justification_2);
            F_SLTP_Justification = Fields.GetField(FormFields.F_SLTP_Justification_1);
            if (String.IsNullOrWhiteSpace(F_SLTP_Justification))
                F_SLTP_Justification = Fields.GetField(FormFields.F_SLTP_Justification_2);
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
                                        
                    // parse checked in version
                    match = Regex.Match(checkedInVer, @"\d+");
                    if (match.Success)
                        checkedInFile.CheckedInVer = int.Parse(match.Value);

                    // parse approved version
                    match = Regex.Match(approvedVer, @"\d+");
                    if (match.Success)
                        checkedInFile.ApprovedVer = int.Parse(match.Value);

                    if (checkedInFile.ApprovedVer < checkedInFile.CheckedInVer)
                        Defects.Add(@"Approved ver is less than checked in ver for file " + checkedInFile.FileName);

                    // parse under which SCR is this file checked in                    
                    // one field may contain multiple SCRs, e.g., 123.45, 678.90
                    foreach (Match item in Regex.Matches(scr, SCR_PATTERN))
                    {
                        CheckedInFiles.Add(new CheckedInFile { 
                            FileName = checkedInFile.FileName,
                            SCR = float.Parse(item.Value),
                            ApprovedVer = checkedInFile.ApprovedVer,
                            CheckedInVer = checkedInFile.CheckedInVer
                        });                        
                    }
                    
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
                match = Regex.Match(pageText, @"SYSTEM CHANGE REQUEST\s*Page\s*\d+\s*of\s*\d+", RegexOptions.IgnoreCase);
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
                        match = Regex.Match(SCRPageContent, 
                                            @"Change Category:.*SCR No\.:\s*[A-Z]*\s*(" + SCR_PATTERN + ")",
                                            RegexOptions.IgnoreCase);
                        if (match.Success)                            
                            report.SCRNumber = float.Parse(match.Groups[1].Value);

                        // SCR Status: SEC
                        match = Regex.Match(SCRPageContent, @"SCR Status:\s*([A-Z]{3})");
                        if (match.Success)                            
                            report.Status = match.Groups[1].Value;

                        // Affected Area: VGUIDE
                        match = Regex.Match(SCRPageContent, @"Affected Area:\s*(\w{2,})");
                        if (match.Success)                            
                            report.AffectedArea = match.Groups[1].Value;

                        // Target Configuration: A350_CERT1_TST_X04
                        match = Regex.Match(SCRPageContent, @"Target Configuration:\s*(\w{2,})");
                        if (match.Success)                            
                            report.TargetConfig = match.Groups[1].Value;

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
                                        match = Regex.Match(line, checkedInFile.FileName + @"\s*(\d{1,})");
                                        if (match.Success)
                                            // checkedInFile.CheckedInVer = int.Parse(match.Value.Strip(checkedInFile.FileName));
                                            checkedInFile.CheckedInVer = int.Parse(match.Groups[1].Value);

                                        report.AffectedElements.Add(checkedInFile);
                                    }
                                }
                            }
                        }
                        #endregion Elements Affected

                        // Closed in Config.: MD11_922_TST
                        match = Regex.Match(SCRPageContent, @"Closed in Config.:\s*(\w{1,})");
                        if (match.Success)
                        {
                            // report.ClosedConfig = match.Value.Strip("Closed in Config.:").Trim();
                            report.ClosedConfig = match.Groups[1].Value;
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

#if COMMENT_COLLECTION_METHOD_1
                #region method 1
                PdfDictionary pdfDict = Reader.GetPageN(page);  // read one page once                
                if (null != pdfDict)
                {
                    PdfArray annotArray = pdfDict.GetAsArray(PdfName.ANNOTS);
                    // How to collect all state models that belong to one sticky note?
#warning assume there is only ONE comment on one page
                    bool isDefectAccepted = false;
                    bool defectSeverityFound = false;
                    bool defectTyeFound = false;
                    bool authorWorkCompletedFound = false;
                    bool moderatorVerifyCompletedFound = false;
                    for (int i = 0; null != annotArray && i < annotArray.Size; ++i)
                    {
                        PdfDictionary curAnnot = annotArray.GetAsDict(i);                        
                        // SUBTYPE values: /Widget, /Text, /Popup, /StrikeOut, /Stamp
                        if (PdfName.TEXT == (PdfName)curAnnot.Get(PdfName.SUBTYPE))
                        {
                            // The KEY
                            // StateModel values: DefectType, Resolution Status, DefectSeverity, Is Defect State, Marked
                            PdfString KEY = (PdfString)curAnnot.Get(new PdfName("StateModel"));

                            // The VALUE
                            // PdfName.STATE values: ND, ST, NC, Accepted, Minor, Unmarked, In Work, Work Completed, Verified Complete, 
                            PdfString VALUE = (PdfString)curAnnot.Get(PdfName.STATE);
                            
                            if (null != KEY && null != VALUE)
                            {
                                if (KEY.ToString().Contains("Is Defect State") &&
                                    VALUE.ToString().Contains("Accepted"))
                                {                                    
                                    isDefectAccepted = true;
                                    continue;
                                }
                                if (KEY.ToString().Contains("DefectSeverity"))
                                {
                                    defectSeverityFound = true;
                                    continue;
                                }
                                if (KEY.ToString().Contains("DefectType"))
                                {
                                    defectTyeFound = true;
                                    continue;
                                }
                                if (KEY.ToString().Contains("Resolution Status") && VALUE.ToString().Contains("Work Completed"))
                                {
                                    authorWorkCompletedFound = true;
                                    continue;
                                }
                                if (KEY.ToString().Contains("Resolution Status") && VALUE.ToString().Contains("Verified Complete"))
                                {
                                    moderatorVerifyCompletedFound = true;
                                    continue;
                                }
                            }
                        }
                    }
                    if (isDefectAccepted)
                    {
                        ++TotalAcceptedDefectCount;
                        if (!(defectSeverityFound && defectTyeFound && authorWorkCompletedFound && moderatorVerifyCompletedFound))
                            Defects.Add("Incomplete attributes for the comment on page " + page);
                    }
                }
                #endregion method 1
#else
                #region method 2
                PdfObject pageObj = Reader.GetPageN(page).Get(PdfName.ANNOTS);
                List<PdfIndirectReference> commentsInOnePage = new List<PdfIndirectReference>();
                if (null != pageObj)
                {
                    PdfArray annotArray = (PdfArray)PdfReader.GetPdfObject(pageObj);
                    if (null != annotArray)
                    {
                        foreach (PdfIndirectReference annot in annotArray.ArrayList)
                        {
                            bool isDefectAccepted = false;
                            bool defectSeverityFound = false;
                            bool defectTyeFound = false;
                            bool authorWorkCompletedFound = false;
                            bool moderatorVerifyCompletedFound = false;

                            #region retrieve info from an annotation

                            PdfDictionary annotDict = (PdfDictionary)PdfReader.GetPdfObject(annot);
                            
                            PdfName _annotSubtype = (PdfName)annotDict.Get(PdfName.SUBTYPE);
                                                                               
                            // IRT: In Reply To
                            PdfObject _annotIRT = annotDict.Get(PdfName.IRT);
                            PdfDictionary prefs = (PdfDictionary)PdfReader.GetPdfObject(_annotIRT);
                            
                            // Work Completed, Verified Complete, ND, Accepted, Minor, etc.
                            PdfString _annotState = (PdfString)annotDict.Get(PdfName.STATE);
                            String annotState = null == _annotState ? "" : _annotState.ToString();

                            // "MigrationStatus", "DefectType", "Is Defect State", "Resolution Status", "DefectSeverity"
                            PdfString _annotStateModel = ((PdfString)annotDict.Get(new PdfName("StateModel")));
                            String annotStateModel = null == _annotStateModel ? "" : _annotStateModel.ToString();

                            PdfString _annotContents = annotDict.GetAsString(PdfName.CONTENTS);
                            String annotContents = null == _annotContents ? "" : _annotContents.ToString();
                            
                            #endregion retrieve info from an annotation

                            // Guess: this is the sticky note
                            if (null == _annotState && null == prefs &&
                                (_annotSubtype.Equals(PdfName.TEXT) || _annotSubtype.Equals(new PdfName("Highlight"))))
                            {
                                // check the contents of this comment, by moderator.
                                // such as "Should be TC 3. Note: this fails checklist 45."

                                // annot.Number property: 6181
                                // annot.Generation property: 0
                                // annotDict Keys:
                                // AP                {Dictionary}
                                // C                 {[1.0, 1.0, 0.0]}
                                // Contents          "Should be TC 3.\rNote: this fails checklist 45."
                                // CreationDate      {D:20141120162918+08'00'}
                                // F                 {28}
                                // M                 {D:20141120163832+08'00'}
                                // NM                {1a9e49e9-e5ce-424d-bfd2-09ed4c6e6e4b}
                                // Name              {/Comment}
                                // P                 {4193 0 R}
                                // Popup             {6182 0 R}
                                // RC                {<?xml version="1.0"?><body xmlns="http://www.w3.org/1999/xhtml" xmlns:xfa="http://www.xfa.org/schema/xfa-data/1.0/" xfa:APIVersion="Acrobat:9.0.0" xfa:spec="2.0.2" ><p dir="ltr"><span dir="ltr" style="font-size:10.0pt;text-align:left;color:#000000;font-weight:normal;font-style:normal">Should be TC 3.&#13;Note: this fails checklist 45.</span></p></body>}
                                // Rect              {[51.3388, 606.718, 71.3388, 624.718]}
                                // Rotate            {90}
                                // Subj              null
                                // Subtype           {/Text}
                                // T                 {E461456}
                                // Type              {/Annot}                                

                                if (String.IsNullOrWhiteSpace(annotContents))
                                    Defects.Add(String.Format(@"The content of the comment on page {0} is empty.", page));
                                commentsInOnePage.Add(annot);
                            }
                                                        
                            for (int i = 0; i < commentsInOnePage.Count; ++i)
                            {                                
                                if (null != prefs && prefs.Equals((PdfDictionary)PdfReader.GetPdfObject(commentsInOnePage[i]))) 
                                {                                       
                                    commentsInOnePage.Add(annot);
                                    if (null != _annotState)
                                    {
                                        // annot.Number property: 6337
                                        // annot.Generation property: 0
                                        // annotDict Keys:
                                        // AP               {Dictionary}
                                        // C                {[1.0, 1.0, 0.0]}
                                        // Contents         {ND set by E461456}
                                        // CreationDate     {D:20141120162951+08'00'}
                                        // F                {30}
                                        // + IRT            {6181 0 R}
                                        // M                {D:20141120162951+08'00'}
                                        // NM               {737023fb-77fa-47a0-aa70-05dcd67e4243}
                                        // Name             {/Comment}
                                        // P                {4193 0 R}
                                        // Popup            {6338 0 R}
                                        // RC               {<?xml version="1.0"?><body xmlns="http://www.w3.org/1999/xhtml" xmlns:xfa="http://www.xfa.org/schema/xfa-data/1.0/" xfa:APIVersion="Acrobat:9.0.0" xfa:spec="2.0.2" ><p>ND set by E461456</p></body>}
                                        // Rect             {[100.0, 82.0, 120.0, 100.0]}
                                        // + State          {ND}
                                        // + StateModel     {DefectType}
                                        // Subj             null
                                        // Subtype          {/Text}
                                        // T                {E461456}
                                        // Type             {/Annot}

                                        if ("DefectType".Equals(annotStateModel))
                                            defectTyeFound = true;
                                        else if ("Is Defect State".Equals(annotStateModel) && annotState.Contains("Accepted"))
                                            isDefectAccepted = true;
                                        else if ("Resolution Status".Equals(annotStateModel))
                                        {
                                            if (annotState.Contains("Work Completed"))
                                                authorWorkCompletedFound = true;
                                            if (annotState.Contains("Verified Complete"))
                                                moderatorVerifyCompletedFound = true;
                                        }
                                        else if ("DefectSeverity".Equals(annotStateModel))
                                            defectSeverityFound = true;
                                    }
                                    else // null == annotState
                                    {
                                        // author's reply
                                        if (String.IsNullOrWhiteSpace(annotContents))
                                            Defects.Add(String.Format(@"The comment on page {0} has no reply.", page));
                                    }                                    
                                }                                 
                            }  
                            if (isDefectAccepted)
                            {
                                ++TotalAcceptedDefectCount;
                                if (!(defectSeverityFound && defectTyeFound && authorWorkCompletedFound && moderatorVerifyCompletedFound))
                                    Defects.Add("Incomplete attributes for the comment on page " + page);
                            }
                        }                        
                    }
                }
                #endregion method 2
#endif
                #endregion collect comments
            }
        }

        private void CollectComments(int page)
        {
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

            // Summary:
            // KEY = Resolution Status, VALUE = None, In Work, Work Completed, Need Additional Rework, Verified Complete
            // KEY = Is Defect State, VALUE = None, Accepted, Nondefect, Duplicate
            // KEY = DefectType, VALUE = NT, TT, NC...
            // KEY = DefectSeverity, VALUE = Minor, Major

            #endregion Annotation state model

            PdfObject pageObj = Reader.GetPageN(page).Get(PdfName.ANNOTS);
            if (null == pageObj)
                return;

            PdfArray annotArray = (PdfArray)PdfReader.GetPdfObject(pageObj);
            if (null == annotArray)
                return;

            foreach (PdfIndirectReference annot in annotArray.ArrayList)
            {
                PdfDictionary annotDict = (PdfDictionary)PdfReader.GetPdfObject(annot);
                                
                if (PdfName.TEXT != (PdfName)annotDict.Get(PdfName.SUBTYPE))
                    continue; // early filter: sticky notes and their attributes are all of subtype TEXT

                PdfObject _annotP = annotDict.Get(PdfName.P);  // possibly means Place
                PdfObject _annotIRT = annotDict.Get(PdfName.IRT);  // IRT: In Reply To
                PdfString _annotContents = annotDict.GetAsString(PdfName.CONTENTS);
                // PdfObject _annotT = annotDict.Get(PdfName.T);  // Title, usually indicates the author who created this annot

                // Work Completed, Verified Complete, ND, Accepted, Minor, etc.
                PdfString _annotState = (PdfString)annotDict.Get(PdfName.STATE);                

                // "MigrationStatus", "DefectType", "Is Defect State", "Resolution Status", "DefectSeverity"
                PdfString _annotStateModel = ((PdfString)annotDict.Get(new PdfName("StateModel")));                
                
#if DEBUG_VERBOSE
                PdfObject _annotAP = annotDict.Get(PdfName.AP);
                PdfObject _annotC = annotDict.Get(PdfName.C);
                PdfObject _annotCreationDate = annotDict.Get(PdfName.CREATIONDATE);
                PdfObject _annotF = annotDict.Get(PdfName.F);
                PdfObject _annotM = annotDict.Get(PdfName.M);
                PdfObject _annotNM = annotDict.Get(PdfName.NM);
                PdfObject _annotName = annotDict.Get(PdfName.NAME);                
                PdfObject _annotPopup = annotDict.Get(PdfName.POPUP);
                PdfObject _annotRC = annotDict.Get(PdfName.RC);
                PdfObject _annotRect = annotDict.Get(PdfName.RECT);
                PdfObject _annotRotate = annotDict.Get(PdfName.ROTATE);
                PdfObject _annotSubj = annotDict.Get(PdfName.SUBJECT);
                PdfObject _annotT = annotDict.Get(PdfName.T);
                PdfObject _annotType = annotDict.Get(PdfName.TYPE);

                // C; Contents; F; NM; Name; P; Popup; T; Type; Number; IRT; State; StateModel
                if (_annotSubtype == PdfName.TEXT)
                    Defects.Add(_annotC.ToString() + "; "
                                + _annotContents.ToString() + "; "
                                + _annotF.ToString() + "; "
                                + _annotNM.ToString() + "; "
                                + _annotName.ToString() + "; "
                                + _annotP.ToString() + "; "
                                + _annotPopup.ToString() + "; "                                            
                                + _annotT.ToString() + "; "
                                + _annotType.ToString() + "; "
                                + annot.Number + "; "
                                + (_annotIRT == null ? "null; " : _annotIRT.ToString() + "; ")
                                + annotState + "; "
                                + annotStateModel
                                );
#endif

                #region Parse annotation info
                // Sticky note and its reply have no State and StateModel values.
                // This can be the basis to distinguish them from other attributes.
                AnnotGroup annotGroup = new AnnotGroup { AnnotP = null };
                if (null == _annotState && null == _annotStateModel)
                {
                    if (null == _annotContents || String.IsNullOrWhiteSpace(_annotContents.ToString()))
                    {
                        // Sticky note has no IRT value. This can be the basis to distinguish it from its reply.
                        if (null == _annotIRT)
                            Defects.Add(String.Format("The comment on page {0} is empty.", page));
                        else
                            Defects.Add(String.Format("The comment on page {0} has no reply.", page));
                    }
                }
                                
                // P 值指的是文档中的一个位置；不同的 annotation 组可能有相同的 P 值，同一个 annotation 组的 P 值是相同的。
                // Attributes 的 IRT 值位于本 annotation 组其他元素的 Number 值集合中。
                // 结合以上两点可定位出一个 annotation 组。

                #endregion Parse annotation info
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
            if (String.IsNullOrWhiteSpace(F_ReviewLocation) || F_ReviewLocation.ToUpper().Contains("INVALID") ||
                F_ReviewLocation.Contains(FormFields.F_N_ReviewLocation_Val))
                Defects.Add(@"Review Location is invalid.");

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
            // if (!PackageIsLocked)
            // {
            //     Defects.Add(@"Review package is not locked.");
            //     return;
            // }

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

                int filledDefectCount = F_ProducerNontechDefectCount + F_ProducerTechDefectCount
                    + F_ProducerProcessDefectCount;
                if (TotalAcceptedDefectCount != filledDefectCount)
                    Defects.Add(String.Format(@"Total defects count filled in coversheet is {0}, but actual accepted comments count is {1}.",
                        filledDefectCount, TotalAcceptedDefectCount));

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
                    if (!F_WorkProductType.Contains(@"Software Test"))
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
                // if (!ExtFileNames.Contains(".TRT") && HasTraceChecklist)
                //     Defects.Add(@"The Trace Checklist is redundant because no trace file is updated.");
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
                else
                    Defects.Add("Missing report for SCR " + checkedInFile.SCR);
            }

            #endregion ACM Info vs. Coversheet Info
        }

        /// <summary>
        /// Parse the justifications to find items that are not disposed
        /// </summary>
        /// <param name="justifications">Justifications to be parsed</param>
        /// <param name="itemsNoOrNA">A list of NO or N/A items</param>
        /// <returns>A list of not disposed items</returns>
        private List<int> ParseJustifications(String justifications, List<int> itemsNoOrNA, CheckListType checkListType)
        {
            if (String.IsNullOrWhiteSpace(justifications) || itemsNoOrNA.Count < 1)
                return itemsNoOrNA;

            const int SpecialSLTPItemNumber = 5;  // SLTP check list item 5 displays as 4.1
            List<int> notDisposedItems = new List<int>(itemsNoOrNA);
            Match match;
            foreach (var item in itemsNoOrNA)
            {
                // Special case: SLTP check list item 5 displays as 4.1
                if (checkListType == CheckListType.SLTP && item == SpecialSLTPItemNumber)
                {
                    match = Regex.Match(justifications, @"(items?|points?|#)+\s*(\d{1,2}\.\d)", RegexOptions.IgnoreCase);
                    if (match.Success && Math.Abs(4.1 - float.Parse(match.Groups[2].Value)) < 0.01)
                    {
                        notDisposedItems.Remove(item);
                        continue;
                    }
                }               

                foreach (var line in justifications.Split("\r\n".ToCharArray()))
                {
                    // For item(s) 12 - 15 or For point(s) 12-15
                    // 匹配下一种形式的，也会匹配这种形式。这种形式更 specific, 故把它放在前面。                                        
                    match = Regex.Match(line, @"(items?|points?|#)+\s*(\d{1,2})\s*-+\s*(\d{1,2})+", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        int lowerBound = int.Parse(match.Groups[2].Value);
                        int upperBound = int.Parse(match.Groups[3].Value);
                        // 1. consider the special SLTP case first
                        if (checkListType == CheckListType.SLTP && item > SpecialSLTPItemNumber &&
                            // item >= lowerBound + 1 && item <= upperBound + 1)
                            item >= Utilities.HumanToProgram(lowerBound) && item <= Utilities.HumanToProgram(upperBound))
                            notDisposedItems.Remove(item);
                        // 2. then normal CTP check list or trace check list
                        else if (item >= lowerBound && item <= upperBound)
                            notDisposedItems.Remove(item);
                        // One line can only match one form, so continue to next line
                        continue;
                    }

                    // For item 1 2 3
                    // For items 12, 13...                    
                    match = Regex.Match(line, @"(items?|points?|#)\s*((\d{1,2}[\s,;:]+)+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        foreach (var itemNumber in match.Groups[2].Value.Split(" ,;:".ToCharArray(),
                                                                               StringSplitOptions.RemoveEmptyEntries))
                        {
                            int temp = int.Parse(itemNumber);
                            // again, consider the special SLTP case first
                            if (checkListType == CheckListType.SLTP && item > SpecialSLTPItemNumber &&
                                // item == temp + 1)
                                item == Utilities.HumanToProgram(temp))
                                notDisposedItems.Remove(item);
                            // then normal CTP check list or trace check list
                            else if (item == temp)
                                notDisposedItems.Remove(item);
                        }
                    }
                }
            }

            return notDisposedItems;
        }

        public void CheckCheckList(CheckListType checkListType)
        {            
            int checkLiteItemCount = 1;
            String fieldPrefix = "";
            String typeName = "";
            String justification = "";
            List<int> itemsNotChecked = new List<int>();
            List<int> itemsNoOrNA = new List<int>();            

            if (null == Fields)
                return;

            switch (checkListType)
            {
                case CheckListType.CTP:
                    checkLiteItemCount = 45;
                    fieldPrefix = @"CTP.";                    
                    typeName = "CTP";
                    justification = F_CTP_Justification;
                    break;
                case CheckListType.SLTP:                    
                    checkLiteItemCount = 25;
                    fieldPrefix = @"SLTP.";
                    typeName = "SLTP";
                    justification = F_SLTP_Justification;
                    break;
                case CheckListType.Trace:
                    checkLiteItemCount = 8;
                    fieldPrefix = @"CkList.";
                    typeName = "trace";
                    justification = F_Trace_Justification;
                    break;
                default:
                    break;
            }

            for (int i = 1; i <= checkLiteItemCount; ++i)
            {
                // "Yes", "No", "NA", ""
                String fieldVal = Fields.GetField(fieldPrefix + i);
                if (String.IsNullOrWhiteSpace(fieldVal))
                    itemsNotChecked.Add(i);
                else if (!fieldVal.ToUpper().Contains('Y'))
                    itemsNoOrNA.Add(i);
            }

            if (itemsNotChecked.Count() > 0)
            {
                string items = @"Item(s) ";
                foreach (int i in itemsNotChecked)
                    items += (checkListType == CheckListType.SLTP ? Utilities.ProgramToHuman(i) : i) + " ";
                Defects.Add(items + "is/are not checked in " +  typeName + " check list.");
            }

            List<int> notDisposedItems = ParseJustifications(justification, itemsNoOrNA, checkListType);
            if (null != notDisposedItems && notDisposedItems.Count > 0)
            {
                String items = "";
                foreach (var i in notDisposedItems)
                    items += (checkListType == CheckListType.SLTP ? Utilities.ProgramToHuman(i) : i) + " ";
                Defects.Add(@"No justification for NO or N/A item(s) " + items
                    + " in " + typeName + " check list, or the justification cannot be parsed.");
            }
        }

        /// <summary>
        /// Check CTP, SLTP check list and trace check list.
        /// </summary>
        public void CheckCheckList()
        {
            if (null != Reader.AcroFields.GetFieldItem(@"CTP.1"))
                CheckCheckList(CheckListType.CTP);
            if (null != Reader.AcroFields.GetFieldItem(@"SLTP.1"))
                CheckCheckList(CheckListType.SLTP);
            if (HasTraceChecklist)
                CheckCheckList(CheckListType.Trace);            
        }

        public List<String> GetDefects()
        {
            return Defects;
        }
    }
}
