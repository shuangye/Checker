#undef COMMENT_COLLECTION_METHOD_1
#undef COMMENT_COLLECTION_METHOD_2
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
using Pkg_Checker.Integration;

namespace Pkg_Checker.Implementations
{
    public class iTextPdfReader : IPdfReader
    {
        #region Properties
        public PdfReader Reader { get; set; }
        public AcroFields Fields { get; set; }
        public float SCRTolerance { get { return 0.001F; } }  // SCR is in XXXX.XX form

        public String ReviewPackageName { get; set; }  // in upper case
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
        public List<AnnotGroup> AnnotGroups { get; set; }
        public List<SCRReport> SCRReportsFromPackage { get; set; }
        public List<SCRReport> SCRReportsFromCM21 { get; set; }
        public List<CheckedInElement> CheckedInElements { get; set; }
        public List<float> SCRs { get; set; }   // SCRs of "Work products under review"
        public List<String> BaseFileNames { get; set; }
        public List<String> ExtFileNames { get; set; }
        public List<String> PrintedFiles { get; set; }
        public List<String> Defects { get; set; }
        public List<String> Warnings { get; set; }
        #endregion Properties

        public iTextPdfReader(String filePath)
        {
            Defects = new List<String>();
            Warnings = new List<String>();
            Reader = new PdfReader(filePath);
            Fields = Reader.AcroFields;
            ReviewPackageName = Path.GetFileName(filePath).ToUpper();
        }

        ~iTextPdfReader()
        {
            if (null != Reader)
                Reader.Close();
        }

        /// <summary>
        /// Call this method before any checking methods to populate preliminary data
        /// </summary>
        /// <param name="filePath"></param>
        [Obsolete]
        private bool Init(string filePath)
        {
            bool result = true;

            try
            {
                Defects = new List<String>();
                Reader = new PdfReader(filePath);
                Fields = Reader.AcroFields;
                ReviewPackageName = Path.GetFileName(filePath).ToUpper();
            }

            catch (Exception)
            {
                Reader = null;
                Fields = null;
                result = false;
            }

            return result;
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

            SCRReportsFromPackage = new List<SCRReport>();
            PrintedFiles = new List<string>();
            AnnotGroups = new List<AnnotGroup>();            
            const int maxFileCount = 40;            
            Match match;            
            String val = "";

            #region Parse Info from Review Package Name
            match = Regex.Match(ReviewPackageName, @"_(" + FormFields.PATTERN_SCR_NUMBER_2 + ")_");
            if (match.Success)
                MasterSCR = float.Parse(match.Groups[1].Value.Replace('_', '.'));

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
            match = Regex.Match(Fields.GetField(FormFields.F_RevParticipants), @"\d+");
            if (match.Success)
                F_RevParticipants = int.Parse(match.Value);
            
            // Review ID fields may be filled as 3912.02;4080.02
            match = Regex.Match(Fields.GetField(FormFields.F_ReviewID), FormFields.PATTERN_SCR_NUMBER);
            if (match.Success)
                F_ReviewID = float.Parse(match.Value);

            match = Regex.Match(Fields.GetField(FormFields.F_ProducerTechDefectCount), @"\d+");
            if (match.Success)
                F_ProducerTechDefectCount = int.Parse(match.Value);

            match = Regex.Match(Fields.GetField(FormFields.F_ProducerNontechDefectCount), @"\d+");
            if (match.Success)
                F_ProducerNontechDefectCount = int.Parse(match.Value);

            match = Regex.Match(Fields.GetField(FormFields.F_ProducerProcessDefectCount), @"\d+");
            if (match.Success)
                F_ProducerProcessDefectCount = int.Parse(match.Value);

            // try { F_ProducerProcessDefectCount = int.Parse(Fields.GetField(FormFields.F_ProducerProcessDefectCount)); }
            // catch { F_ProducerProcessDefectCount = 0; }

            #endregion Coversheet Fields

            #region Checked in Files

            CheckedInElements = new List<CheckedInElement>();
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

                    CheckedInElement checkedInFile = new CheckedInElement { FileName = fileName.Trim().ToUpper() };
                                        
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
                    foreach (Match item in Regex.Matches(scr, FormFields.PATTERN_SCR_NUMBER))
                    {
                        float SCRNumber = float.Parse(item.Value);
                        SCRs.Add(SCRNumber);
                        CheckedInElements.Add(new CheckedInElement
                        { 
                            FileName = checkedInFile.FileName,
                            SCR = SCRNumber,
                            ApprovedVer = checkedInFile.ApprovedVer,
                            CheckedInVer = checkedInFile.CheckedInVer
                        });                        
                    }
                    
                    String ext = Path.GetExtension(checkedInFile.FileName);
                    ExtFileNames.Add(ext);
                    BaseFileNames.Add(checkedInFile.FileName.Substring(0, checkedInFile.FileName.LastIndexOf(ext)));
                    // SCRs.Add(checkedInFile.SCR);
                }
            }
            ExtFileNames = ExtFileNames.Distinct().ToList();
            BaseFileNames = BaseFileNames.Distinct().ToList();
            SCRs = SCRs.Distinct().ToList();
            if (SCRs.Count() <= 0)
                Defects.Add("Coversheet should contain at least one SCR number (in ####.## format).");

            #endregion Checked in Files

            String targetArea = "";            
            bool isEndMarkFound = false;
            bool isOpeningTargetArea = false;
            for (int page = 1; page <= Reader.NumberOfPages; ++page)
            {
                // sticky notes does not depend on page text, so put it before pageText checking
                #region Collect Comments
                // CollectComments(page);
                switch (Parser.ParseComments(Reader, page, AnnotGroups))
                {
                    case 1:
                        Defects.Add(String.Format("The comment on page {0} is empty.", page));
                        break;
                    case 2:
                        Defects.Add(String.Format("The comment on page {0} has no reply.", page));
                        break;
                    default:
                        break;
                }

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
#elif COMMENT_COLLECTION_METHOD_2
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
                            // PdfObject _annotIRT = annotDict.Get(PdfName.IRT);
                            PdfIndirectReference _annotIRT = (PdfIndirectReference)annotDict.Get(PdfName.IRT);
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

                // String pageText = PdfTextExtractor.GetTextFromPage(Reader, page, new SimpleTextExtractionStrategy());                
                // pageText = Encoding.UTF8.GetString(ASCIIEncoding.Convert
                //     (Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(pageText)));
                String pageText = PdfTextExtractor.GetTextFromPage(Reader, page);
                if (String.IsNullOrWhiteSpace(pageText))
                    continue;  // early filtering

                #region Locate an SCR Report Area
                // Locate the SCR report by keywords.
                // Begin: FMS2000 : A350_A380 - SYSTEM CHANGE REQUEST (Proj : Subproj - SYSTEM CHANGE REQUEST)
                // End: Closed in Config.: ***     
                // This algorithm assumes both the begin mark and end mark exist.
                match = Regex.Match(pageText, FormFields.PATTERN_SCR_REPORT_BEGIN_MARK, RegexOptions.IgnoreCase);
                if (match.Success)                
                    isOpeningTargetArea = true;                
                match = Regex.Match(pageText, FormFields.PATTERN_SCR_REPORT_END_MARK);
                if (match.Success)                
                    isEndMarkFound = true;                    

                if (isOpeningTargetArea)
                {                    
                    targetArea += pageText;
                    if (isEndMarkFound)
                    {                        
                        SCRReport report = Parser.ParseSCRReport(targetArea);
                        if (null != report)
                            SCRReportsFromPackage.Add(report);
                        // reset flags
                        isOpeningTargetArea = false;
                        isEndMarkFound = false;
                        targetArea = "";
                    }
                }
                #endregion
                                
                #region Parse SCR Report (Obsolete)
                /*
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
                                            @"Change Category:.*SCR No\.:\s*[A-Z]*\s*(" + FormFields.PATTERN_SCR_NUMBER + ")",
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
                                match = Regex.Match(line, @"\w{1,}\s*\w{5,}\.\w{3,}\s*\d{1,}");
                                if (match.Success)
                                {
                                    // match a file name like TEST CTP_A350_FMCI_EVENT_HANDLER.ZIP
                                    match = Regex.Match(line, @"\w{3,}\.\w{3,}");
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
                            report.ClosedConfig = match.Groups[1].Value;
                            break;  // reach the end of the SCR report
                        }
                    }
                    SCRReports.Add(report);
                    // page += report.EndPageInPackage - report.EndPageInPackage;  // skip this SCR report
                    continue;
                }
                */
                #endregion Parse SCR Report
                
                #region Collect Prerequisite Files
                // .TRT files                
                foreach (Match item in Regex.Matches(pageText, @"TRACE\s*FILENAME\s*:\s*(\w+\.TRT)", RegexOptions.IgnoreCase))
                    PrintedFiles.Add(item.Groups[1].Value.ToUpper());
                #endregion Collect Prerequisite Files                                
            }
        }
                
        /// <summary>
        /// this method has been refactored to Parser.ParseComments
        /// </summary>
        /// <param name="page"></param>
        [Obsolete]
        private void CollectComments(int page)
        {
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

                PdfIndirectReference _annotP = (PdfIndirectReference)annotDict.Get(PdfName.P);  // possibly means Place
                PdfIndirectReference _annotIRT = (PdfIndirectReference)annotDict.Get(PdfName.IRT);  // IRT: In Reply To
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
                if (null == _annotState && null == _annotStateModel)
                {
                    bool isEmptyContent = null == _annotContents || String.IsNullOrWhiteSpace(_annotContents.ToString());
                    // Sticky note has no IRT value. This can be the basis to distinguish it from its reply.
                    if (null == _annotIRT)  // the sticky note
                    {
                        // Create a annotation group only when a sticky note is met.
                        // Its associated attributes all rely on (IRT) the sticky note (the root),
                        // so the order is guaranteed.
                        AnnotGroup annotGroup = new AnnotGroup { 
                            AnnotP = _annotP, Root = annot, Page = page, Numbers = new List<int>(),
                            IsDefectAccepted = false, IsDefectTypeFound = false, IsDefectSeverityFound = false,
                            IsAuthorWorkCompleted = false, IsModeratorVerifyCompleted = false                             
                        }; 
                        annotGroup.Numbers.Add(annot.Number);
                        AnnotGroups.Add(annotGroup);
                        if (isEmptyContent)
                            Defects.Add(String.Format("The comment on page {0} is empty.", page));
                    }
                    else  // the REPLY to this sticky note
                    {
                        AnnotGroup annotGroup = AnnotGroups.FirstOrDefault(x => x.AnnotP.Number == _annotP.Number && x.Numbers.Contains(_annotIRT.Number));
                        if (null != annotGroup)
                            annotGroup.Numbers.Add(annot.Number);
                        if (isEmptyContent)
                            Defects.Add(String.Format("The comment on page {0} has no reply.", page));
                    }                    
                }
                else  // sticky note's associated attributes
                {
                    if (null == _annotIRT)
                        continue;

                    // Attributes 的 IRT 值位于本 annotation 组其他元素的 Number 集合中。
                    AnnotGroup annotGroup = AnnotGroups.FirstOrDefault(x => x.AnnotP.Number == _annotP.Number && x.Numbers.Contains(_annotIRT.Number));
                    if (null != annotGroup)
                    {
                        annotGroup.Numbers.Add(annot.Number);
                        String annotState = null == _annotState ? "" : _annotState.ToString().ToUpper();
                        String annotStateModel = null == _annotStateModel ? "" : _annotStateModel.ToString().ToUpper();
                        if (annotState.Contains("ACCEPTED"))
                            annotGroup.IsDefectAccepted = true;
                        if (annotState.Contains("WORK COMPLETED"))
                            annotGroup.IsAuthorWorkCompleted = true;
                        if (annotState.Contains("VERIFIED COMPLETE"))
                            annotGroup.IsModeratorVerifyCompleted = true;
                        if (annotStateModel.Contains("DEFECTTYPE"))
                            annotGroup.IsDefectTypeFound = true;
                        if (annotStateModel.Contains("DEFECTSEVERITY"))
                            annotGroup.IsDefectSeverityFound = true;
                    }                    
                }                                
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

                foreach (var checkedInFile in CheckedInElements)
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
                
                bool reworkFound = false;
                foreach (var checkedInFile in CheckedInElements)
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

        public void CheckComments()
        {
            int filledDefectCount = F_ProducerNontechDefectCount + F_ProducerTechDefectCount
                    + F_ProducerProcessDefectCount;
            foreach (var annotGroup in AnnotGroups)
            {
                if (annotGroup.IsDefectAccepted)
                {
                    ++TotalAcceptedDefectCount;
                    if (!annotGroup.IsDefectTypeFound)
                        Defects.Add(String.Format("The comment on page {0} has no defect type set.", annotGroup.Page));
                    if (!annotGroup.IsDefectSeverityFound)
                        Defects.Add(String.Format("The comment on page {0} has no defect severity set.", annotGroup.Page));
                    if (!annotGroup.IsAuthorWorkCompleted)
                        Defects.Add(String.Format("The comment on page {0} has no resolution status - work complete set.", annotGroup.Page));
                    if (!annotGroup.IsModeratorVerifyCompleted)
                        Defects.Add(String.Format("The comment on page {0} has no resolution status - verify complete set.", annotGroup.Page));
                }
            }

            if (TotalAcceptedDefectCount != filledDefectCount)
                Defects.Add(String.Format(@"Total defects count filled in coversheet is {0}, but actual accepted comments count is {1}.",
                    filledDefectCount, TotalAcceptedDefectCount));
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
            // foreach (var checkedInFile in CheckedInElements)
            // {
            //     SCRReport matchingSCRReport = SCRReportsFromPackage.FirstOrDefault(
            //         x => Math.Abs(x.SCRNumber - checkedInFile.SCR) < SCRTolerance);
            //     if (null != matchingSCRReport)
            //     {
            //         if (!"SEC".Equals(matchingSCRReport.Status))
            //             Defects.Add(String.Format(@"SCR report {0} is in {1} status; ecptcted SEC.", 
            //                                       matchingSCRReport.SCRNumber, matchingSCRReport.Status));
            // 
            //         IEnumerable<AffectedElement> matchingFiles = null;
            //         if (null != matchingSCRReport.AffectedElements)
            //             matchingFiles =
            //                 matchingSCRReport.AffectedElements.Where(x => x.FileName.Equals(checkedInFile.FileName));
            //         if (null != matchingFiles && matchingFiles.Count() > 0)
            //         {
            //             int maxACMver = matchingFiles.OrderByDescending(x => x.CheckedInVer).FirstOrDefault().CheckedInVer;
            //             if (maxACMver != checkedInFile.CheckedInVer)
            //                 Defects.Add(@"The coversheet indicates that the checked in ver of file "
            //                     + checkedInFile.FileName + " is " + checkedInFile.CheckedInVer
            //                     + ", but the max ver in SCR report is " + maxACMver
            //                     + ". SCR: " + checkedInFile.SCR);
            //         }
            //         else
            //             Defects.Add(@"The SCR report indicates that file " + checkedInFile.FileName
            //                 + " is not checked into SCR " + checkedInFile.SCR);
            //     }
            //     else
            //         Defects.Add("Missing report for SCR " + checkedInFile.SCR);
            // }
            #endregion ACM Info vs. Coversheet Info
            CheckSCRReports(false);
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

            // List<int> notDisposedItems = ParseJustifications(justification, itemsNoOrNA, checkListType);
            List<int> notDisposedItems = Parser.ParseJustifications(justification, itemsNoOrNA, checkListType);
            if (null != notDisposedItems && notDisposedItems.Count > 0)
            {
                String items = "";
                foreach (var i in notDisposedItems)
                    items += (checkListType == CheckListType.SLTP ? Utilities.ProgramToHuman(i) : i) + " ";
                Defects.Add(@"No justification for NO or N/A item(s) " + items
                    + " in " + typeName + " check list, or the justification cannot be recognized.");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="password"></param>
        /// <param name="outputPath"></param>
        /// <param name="timeout">in seconds</param>
        public void CheckWithCM21(String EID, String password, String outputPath, int timeout)
        {
            timeout = timeout * 1000;
            String filePath;
            List<float> SCRsToFetch = new List<float>();

            if (String.IsNullOrWhiteSpace(EID) || String.IsNullOrWhiteSpace(outputPath) || !Directory.Exists(outputPath))
                return;

            // avoid duplicate fetching
            foreach(var item in SCRs)
            {                
                filePath = Path.Combine(outputPath, item.ToString("0.00"));
                if (!File.Exists(filePath))
                    SCRsToFetch.Add(item);
            }

            // fetch SCR report
            if (SCRsToFetch.Count > 0)
            {
                try
                {
                    CM21 cm21 = new CM21(EID, password, F_ACMProject, F_ACMSubProject, outputPath, timeout);
                    cm21.FetchSCRReport(SCRsToFetch, timeout);
                    cm21.Exit(timeout);
                }
                catch (Exception ex)
                {
                    Warnings.Add(ex.Message);
                    return;
                }
            }
                    
            // parse SCR report
            SCRReportsFromCM21 = new List<SCRReport>();
            StreamReader sr;
            foreach(var item in SCRs)
            {                
                filePath = Path.Combine(outputPath, item.ToString("0.00"));
                try
                {
                    sr = new StreamReader(filePath);
                }
                catch(Exception ex)
                {
                    Warnings.Add(String.Format("Cannot open SCR report file {0} for reading: {1}", filePath, ex.Message));
                    continue;
                }

                SCRReport reportFromCM21 = Parser.ParseSCRReport(sr.ReadToEnd());
                sr.Close();
                if (null != reportFromCM21)
                    SCRReportsFromCM21.Add(reportFromCM21);
                else
                    Warnings.Add("Cannot parse SCR report file " + filePath);
            }

            CheckSCRReports(true);
        }

        public List<String> GetDefects()
        {
            return Defects.Distinct().ToList();
        }

        public List<String> GetWarnings()
        {
            return Warnings.Distinct().ToList();
        }

        #region Refactored Methods
        private void CheckSCRReports(bool isFromCM21)
        {
            List<SCRReport> reports;
            String expectedSCRStatus;
            String SCRReportSource;
            String comparedTo;
            if (isFromCM21)
            {
                reports = SCRReportsFromCM21;
                expectedSCRStatus = "VER";
                SCRReportSource = "from CM21";
                comparedTo = "approved";
            }
            else
            {
                reports = SCRReportsFromPackage;
                expectedSCRStatus = "SEC";
                SCRReportSource = "in review package";
                comparedTo = "checked in";
            }

            if (null == reports)
                return;

            foreach (var checkedInFile in CheckedInElements)
            {                
                SCRReport matchingSCRReport = reports.FirstOrDefault(
                    x => Math.Abs(x.SCRNumber - checkedInFile.SCR) < SCRTolerance);
                if (null != matchingSCRReport)
                {
                    if (!expectedSCRStatus.Equals(matchingSCRReport.Status))
                        Defects.Add(String.Format(@"The SCR report {0} {1} is in {2} status; ecptcted {3}.",
                                                  matchingSCRReport.SCRNumber.ToString("0.00"), SCRReportSource,
                                                  matchingSCRReport.Status, expectedSCRStatus));

                    IEnumerable<AffectedElement> matchingFiles = null;
                    if (null != matchingSCRReport.AffectedElements)
                        matchingFiles = matchingSCRReport.AffectedElements.Where(x => x.FileName.Equals(checkedInFile.FileName));
                    if (null != matchingFiles && matchingFiles.Count() > 0)
                    {
                        // compare checked in version with SCR report in review package
                        // compare approved version with SCR report from CM21
                        int maxACMver = matchingFiles.OrderByDescending(x => x.CheckedInVer).First().CheckedInVer;
                        int comparedVer = isFromCM21 ? checkedInFile.ApprovedVer : checkedInFile.CheckedInVer;                                                
                        if (maxACMver != comparedVer)
                            Defects.Add(String.Format(@"The review package coversheet indicates that the {0} version of file {1} is {2}, "
                                + "but the max ver in SCR report {3} {4} is {5}.", comparedTo, checkedInFile.FileName,
                                comparedVer, checkedInFile.SCR.ToString("0.00"), SCRReportSource, maxACMver));                        
                    }
                    else
                        Defects.Add(String.Format(@"The SCR report {0} indicates that file {1} is not checked into SCR {2}.",
                            SCRReportSource, checkedInFile.FileName, checkedInFile.SCR.ToString("0.00")));
                }
                else
                    Defects.Add(String.Format("Missing report for SCR {0} {1}.", checkedInFile.SCR.ToString("0.00"), SCRReportSource));
            }
        }
        #endregion Refactored Methods
    }
}
