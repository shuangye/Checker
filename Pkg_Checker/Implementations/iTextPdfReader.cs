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

        public String ReviewPackageName { get; set; }
        public bool PackageIsLocked { get; set; }
        public String Aircraft { get; set; }
        public float MasterSCR { get; set; }
        public char F_DO178Level { get; set; }
        public String F_ACMProject { get; set; }
        public String F_ACMSubProject { get; set; }
        public String F_FuncArea { get; set; }
        public int F_RevParticipants { get; set; }
        public String F_ModStamp { get; set; }
        public String F_ReviewLocation { get; set; }
        public String F_WorkProductType { get; set; }
        public String F_Lifecycle { get; set; }
        public String F_ReviewStatus { get; set; }

        public List<CheckedInFile> CheckedInFiles { get; set; }
        public List<float> SCRs { get; set; }
        public List<String> BaseFileNames { get; set; }
        public List<String> ExtFileNames { get; set; }

        public List<String> Defects { get; set; }
        #endregion Properties

        /// <summary>
        /// Call this method before any checking methods to populate necessary data
        /// </summary>
        /// <param name="pdfPath"></param>
        public void Read(string pdfPath)
        {
            try
            {
                Defects = new List<String>();
                Reader = new PdfReader(pdfPath);
                Fields = Reader.AcroFields;
            }

            catch (Exception)
            {
                Reader = null;
                Fields = null;
                return;
            }

            if (null != Fields)
            {
                const int maxFileCount = 40;
                String val = "";

                ReviewPackageName = Path.GetFileName(pdfPath).ToUpper();

                val = ReviewPackageName.Strip(@"_REVIEW_PACKET.PDF").Strip(@"FMS2000_A350_A380_")
                    .Strip(@"FMS2000_A3XX_").Strip(@"FMS2000_MDXX_").Strip(@"B7E7_B7E7FMS_");
                try
                { MasterSCR = float.Parse(val.Replace('_', '.')); }
                catch
                { MasterSCR = 0.0F; }

                if (ReviewPackageName.Contains(@"A350"))
                    Aircraft = @"A350";  // A350
                else if (ReviewPackageName.Contains(@"A3XX"))
                    Aircraft = @"A3XX";  // A380, A340, A320
                else if (ReviewPackageName.Contains(@"B7E7"))
                    Aircraft = @"B7E7";  // B787
                else if (ReviewPackageName.Contains(@"MD"))
                    Aircraft = @"MDXX";  // MD11

                AcroFields.Item f = Reader.AcroFields.GetFieldItem(FormFields.F_ReviewID);
                if (null != f)
                {
                    var n = f.GetMerged(0).GetAsNumber(PdfName.FF);
                    PackageIsLocked = null != n && (n.IntValue & PdfFormField.FF_READ_ONLY) > 0;
                }                

                F_FuncArea = Fields.GetField(FormFields.F_FuncArea);
                val = Fields.GetField(FormFields.F_DO178Level);
                F_DO178Level = String.IsNullOrEmpty(val) ? ' ' : char.Parse(val);
                F_ACMProject = Fields.GetField(FormFields.F_ACMProject);
                F_ACMSubProject = Fields.GetField(FormFields.F_ACMSubProject);
                val = Fields.GetField(FormFields.F_RevParticipants);
                F_RevParticipants = String.IsNullOrWhiteSpace(val) ? 0 : int.Parse(val);
                F_ModStamp = Fields.GetField(FormFields.F_ModStamp);
                F_ReviewLocation = Fields.GetField(FormFields.F_ReviewLocation);
                F_WorkProductType = Fields.GetField(FormFields.F_WorkProductType);
                F_Lifecycle = Fields.GetField(FormFields.F_Lifecycle);
                F_ReviewStatus = Fields.GetField(FormFields.F_ReviewStatus);

                #region checked in files
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

                        CheckedInFile checkedInFile = new CheckedInFile
                        {
                            SCR = float.Parse(scr),
                            FileName = fileName.ToUpper(),
                            CheckedInVer = int.Parse(checkedInVer),
                            ApprovedVer = int.Parse(approvedVer)
                        };
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
                #endregion checked in files
            }
        }

        public bool IsValidReviewPackage()
        {
            // 通过判断 Coversheet 的 Review ID 字段是否存在来判断是否为有效的 Review Package            
            if (null == Reader || null == Fields ||
                String.IsNullOrWhiteSpace(Fields.GetField(FormFields.F_ReviewID)))
            {
                Defects.Add(@"Unable to open the file or it is not a valid review package");
                return false;
            }
            else
                return true;
        }

        public void CheckCommonFields()
        {
            if (null == Fields)
                return;

            const int maxRevParticipants = 38;
            String val = "";
            
            val = Fields.GetField(FormFields.F_ReviewID);
            if (String.IsNullOrWhiteSpace(val) || Math.Abs(float.Parse(val) - MasterSCR) > 0.001)
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

            if (String.IsNullOrWhiteSpace(F_ModStamp) || !"Yes".Equals(F_ModStamp))
                Defects.Add(@"Moderator did not stamp on the package.");
            
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
            // 检查 Review status, 所填写的 defects 个数，以及实际有多少个 comments
            if (PackageIsLocked)
            {
                // Accepted as is: no defects
                if (FormFields.F_ReviewStatus_Val_Accepted.Equals(F_ReviewStatus))
                {

                }

                // Revise (no further review): at least 1 defect
                else if (FormFields.F_ReviewStatus_Val_Revised.Equals(F_ReviewStatus))
                { 

                }

                else
                    Defects.Add(@"Review status is not valid.");
            }

            else
                Defects.Add(@"Review package is not locked.");
        }

        public void CheckWorkProductType()
        {
            List<String> workProductTypes;

            if (null == Fields)
                return;

            if (String.IsNullOrWhiteSpace(F_WorkProductType))
                Defects.Add(@"Work Product Type is empty.");
            else
            {
                char[] sep = { ',' };
                workProductTypes = F_WorkProductType.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList<String>();

                // CTP. CTP 没必要一定有 TDF, 也可能只更新了 ZIP
                if ("Low-Level Test Procedures".Equals(F_Lifecycle))
                {
                    if (!F_WorkProductType.Contains("Component Test"))
                        Defects.Add(@"Work Product Type does not match Low-Level Test Procedure");

                    // CTP review package 不能包含 SLTP 相关的内容
                    if (F_WorkProductType.Contains("Software Test") || ExtFileNames.Contains(@".TST"))
                        Defects.Add(@"Low-Level Test Procedure should not contain SLTP related files or Work Product Types");
                }

                // SLTP
                if ("High-Level Test Procedures".Equals(F_Lifecycle))
                {
                    if (!F_WorkProductType.Contains(@"Software Test") || !ExtFileNames.Contains(@".TST"))
                        Defects.Add(@"Work Product Type does not match High-Level Test Procedure");

                    // SLTP review package 不能包含 CTP 相关的内容
                    if (F_WorkProductType.Contains("Component Test") || ExtFileNames.Contains(@".TDF"))
                        Defects.Add(@"High-Level Test Procedure should not contain CTP related files or Work Product Types");
                }

                // Trace
                if (F_WorkProductType.Contains(@"Trace Data") ^ ExtFileNames.Contains(".TRT"))
                    Defects.Add(@"Tracing: Work Product Type and Checked in files do not match");
                if (ExtFileNames.Contains(".TRT") && String.IsNullOrWhiteSpace(Fields.GetField(FormFields.F_TraceCheckList)))
                    Defects.Add(@"Missing Trace Check List");
            }
        }

        public void CheckCheckList()
        {
            const int checkListItemCount = 45;
            List<int> itemsNotChecked = new List<int>();
            List<KeyValuePair<int, bool>> itemsNoOrNA = new List<KeyValuePair<int, bool>>();  // <item, isDisposed>
            // Dictionary<int, bool> itemsNoOrNA = new Dictionary<int, bool>();            

            if (null == Fields)
                return;

            for (int i = 1; i <= checkListItemCount; ++i)
            {
                // "Yes", "No", "NA", ""
                String fieldVal = Fields.GetField("CTP." + i);
                if (String.IsNullOrWhiteSpace(fieldVal))
                    itemsNotChecked.Add(i);
                else if (fieldVal.Contains('N'))
                    // itemsNoOrNA.Add(i, false);
                    itemsNoOrNA.Add(new KeyValuePair<int, bool>(i, false));                    
            }

            if (itemsNotChecked.Count() > 0)
            {
                string temp = @"Item(s) ";
                foreach (int i in itemsNotChecked)
                    temp += i + " ";
                Defects.Add(temp + "is/are not checked.");
            }

            // Justifications
            if (itemsNoOrNA.Count() > 0)
            {
                String justifications = Fields.GetField(FormFields.F_Justifications_1);
                if (String.IsNullOrWhiteSpace(justifications))
                    justifications = Fields.GetField(FormFields.F_Justifications_2);
                if (String.IsNullOrWhiteSpace(justifications))
                {
                    Defects.Add(@"No justifications for NO/NA items.");
                    return;
                }

                List<int> notDisposedItems = new List<int>();
                foreach (var item in itemsNoOrNA)
                    notDisposedItems.Add(item.Key);

                Match match;
                foreach (var item in itemsNoOrNA)
                {
                    foreach (var line in justifications.Split("\r\n".ToCharArray()))
                    {
                        String[] itemNumbers;

                        // For item(s) 12 - 15
                        // 匹配这种形式的，也会匹配下面一种形式。故把这种形式放在前面。
                        match = Regex.Match(line, @"For items*\s*(\d{1,2}\s*-\s*\d{1,2})+", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            itemNumbers = match.Value.ToUpper().Strip("FOR ITEMS").Strip("FOR ITEM").Trim()
                                .Split("- ".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                            if (null != itemNumbers && itemNumbers.Length > 1)
                                if (item.Key >= int.Parse(itemNumbers[0]) &&
                                    item.Key <= int.Parse(itemNumbers[1]))
                                    // itemsNoOrNA.Remove(item);                                    
                                    notDisposedItems.Remove(item.Key);
                            // 匹配了 For item(s) 1 - 3 就不能再匹配 Form item(s) 1, 2, 3... 了
                            continue;
                        }

                        // For item(s) 1
                        // For item 12, 13...
                        match = Regex.Match(line, @"For items*\s*(\d{1,2}[\s,]*)+", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            itemNumbers = match.Value.ToUpper().Strip("FOR ITEMS").Strip("FOR ITEM").Trim()
                                .Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (var itemNumber in itemNumbers)
                                if (item.Key == int.Parse(itemNumber))
                                    notDisposedItems.Remove(item.Key);
                                    // itemsNoOrNA[item.Key] = true;  // disposed
                                    // the Value property is readonly, so set it to a new KeyValuePair
                                    // itemsNoOrNA[item.Key] = new KeyValuePair<int, bool>(item.Key, true);
                                    // itemsNoOrNA.Remove(item);                           
                        }                        
                    }
                }

                if (notDisposedItems.Count > 0)
                {
                    String temp = "";
                    foreach (var item in notDisposedItems)
                        temp += item + " ";
                    Defects.Add(@"No justification for NO or N/A item " + temp + ".");
                }
            }
        }

        /// <summary>        
        /// 为了只遍历一次，把检查 SCR report, 是否存在 .TRT 文件，以及有多少 comments 放在了一个函数里
        /// </summary>
        public void TraverseWholeFile()
        {
            if (null == Reader)
                return;

            // is SCR report found for one specific SCR? (consider checking in / inserting more than one SCRs)
            List<KeyValuePair<float, bool>> SCRReportFound = new List<KeyValuePair<float, bool>>();
            // is the TRT file present for one specific CTP? (consider more than one CTPs in one review package)
            List<KeyValuePair<String, bool>> TRTFileFound = new List<KeyValuePair<string, bool>>();

            Match match;
            String KEYWORKDS;

            for (int page = 1; page <= Reader.NumberOfPages; ++page)
            {                
                SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                String pageText = PdfTextExtractor.GetTextFromPage(Reader, page, strategy);
                if (String.IsNullOrWhiteSpace(pageText))
                    continue;

                #region Parse SCR Report
                // SCR report 可能被打印成了多页，而它自身的 page # of # 和最终打印的 PDF 又不严格对应
                // 故只能根据特征字符串定位 SCR Report, 之后从中提取信息
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
                        match = Regex.Match(SCRPageContent, @"Change Category:\s*PROBLEM\s*SCR No.:\s*[A-Z]\s*\d{3,}\.\d{2}");
                        if (match.Success)
                            report.SCRNumber = float.Parse(
                                Regex.Match(match.Value.Strip(@"Change Category:").Strip("PROBLEM").Strip("SCR No.:").Trim(),
                                @"\d{3,}\.\d{2}").Value);

                        // SCR Status: SEC
                        match = Regex.Match(SCRPageContent, @"SCR Status:\s*[A-Z]{3}");
                        if (match.Success)
                            report.Status = match.Value.Strip("SCR Status:").Trim();

                        // Affected Area: VGUIDE
                        match = Regex.Match(SCRPageContent, @"Affected Area:\s*\S{2,}");
                        if (match.Success)
                            report.AffectedArea = match.Value.Strip("Affected Area:").Trim();

                        // Target Configuration: A350_CERT1_TST_X04
                        match = Regex.Match(SCRPageContent, @"Target Configuration:\s*\S{2,}");
                        if (match.Success)
                            report.TargetConfig = match.Value.Strip("Target Configuration:").Trim();

                        // Elements Affected
                        #region Elements Affected
                        // Begin: Elements Affected:
                        // End: Closure Category:
                        // 考虑了这一区域位于 PDF 多个页面中的情况
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
                        }

                        // 定位出了 Affected Elements 这一区域，下面解析其中的信息
                        if (!String.IsNullOrWhiteSpace(elementsAffectedArea))
                        {
                            report.AffectedElements = new List<CheckedInFile>();
                            CheckedInFile checkedInFile = null;
                            foreach (String line in elementsAffectedArea.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            {                                
                                match = Regex.Match(line, @"\S{1,}\.\S{1,}");
                                if (match.Success)
                                {
                                    checkedInFile = new CheckedInFile();
                                    checkedInFile.SCR = report.SCRNumber;
                                    checkedInFile.FileName = match.Value;

                                    // 把文件名与版本合在一起匹配是因为文件名中也可能含有数字，如 A350, MD11
                                    match = Regex.Match(line, checkedInFile.FileName + @"\s*\d{1,}");
                                    if (match.Success)
                                        checkedInFile.CheckedInVer = int.Parse(match.Value.Strip(checkedInFile.FileName));

                                    report.AffectedElements.Add(checkedInFile);
                                }                                    
                            }                            
                        }
                        #endregion Elements Affected

                        // Closed in Config.: MD11_922_TST
                        match = Regex.Match(SCRPageContent, @"Closed in Config.:\s*\S{2,}");
                        if (match.Success)
                        {
                            report.ClosedConfig = match.Value.Strip("Closed in Config.:").Trim();
                            break;  // reach the end of the SCR report
                        }
                    }                                       
                }
                #endregion Parse SCR Report

                #region Check Prerequisite Files
                // 如果 TRT 被更新了，则它至少应出现2次；否则至少应出现1次。
                // 若一个 review package 里包含了多个 CTP 呢？

                #endregion Check Prerequisite Files
            }
        }

        public void WorkWithAnnot()
        {
            if (null == Reader)
                return;

            int acceptedDefectCount = 0;

            // 如果只是检查签章，则不必把整个文件读完，只为签章只出现在前几页
            for (int page = 1; page <= Reader.NumberOfPages; ++page)
            {
                PdfDictionary pdfDict = Reader.GetPageN(page);  // read one page once
                if (null != pdfDict)
                {
                    PdfArray annotArray = pdfDict.GetAsArray(PdfName.ANNOTS);
                    if (null != annotArray)
                    {
                        for (int i = 0; i < annotArray.Size; ++i)
                        {
                            PdfDictionary curAnnot = annotArray.GetAsDict(i);
                            // SUBTYPE: /Widget, /Text, /Popup, /StrikeOut, /Stamp
                            PdfName annotSubtype = (PdfName)curAnnot.Get(PdfName.SUBTYPE);
                            PdfString annotState = (PdfString)curAnnot.Get(PdfName.STATE);
                            PdfString annotStateType = (PdfString)curAnnot.Get(new PdfName("StateModel"));

                            // PdfName.STATE: ND, ST, Accepted, Minor, Unmarked, In Work, Work Completed, Verified Complete, 
                            // StateModel: DefectType, Resolution Status, DefectSeverity, Is Defect State, Marked

                            if (PdfName.WIDGET != annotSubtype)
                            {
                                Defects.Add(null == annotSubtype ? "<null>" : "PdfName.SUBTYPE: " + annotSubtype.ToString() +
                                "; PdfName.STATE: " + annotState + "; StateModel: " + annotStateType);
                            }

                            if (PdfName.STAMP == annotSubtype)
                            {
                                PdfString str = (PdfString)curAnnot.Get(PdfName.AUTHOR);
                                if (null != str)
                                    Defects.Add("Stamp: " + str.ToString());
                            }

                            if (PdfName.TEXT == annotSubtype)
                            {
                                // PdfName.SUBTYPE: /Text; PdfName.STATE: Accepted; StateModel: Is Defect State
                                if (null != annotStateType && annotStateType.ToString().IndexOf("Is Defect State") >= 0 &&
                                    null != annotState && annotState.ToString().IndexOf("Accepted") >= 0)
                                    ++acceptedDefectCount;
                            }
                        }
                    }
                }
            }
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
