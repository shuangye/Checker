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
        public String ReviewPackageName { get; set; }
        public float MasterSCR { get; set; }
        public PdfReader Reader { get; set; }
        public AcroFields Fields { get; set; }
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
                MasterSCR = float.Parse(val.Replace('_', '.'));

                F_FuncArea = Fields.GetField(FormFields.F_FuncArea);
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

            AcroFields.Item f = Reader.AcroFields.GetFieldItem(FormFields.F_ReviewID);
            var n = f.GetMerged(0).GetAsNumber(PdfName.FF);
            if (!(null != n && (n.IntValue & PdfFormField.FF_READ_ONLY) > 0))
                Defects.Add(@"Package is not locked.");

            val = Fields.GetField(FormFields.F_ReviewID);
            if (String.IsNullOrWhiteSpace(val) || Math.Abs(float.Parse(val) - MasterSCR) > 0.001)
                Defects.Add(@"Review ID does not match the review package name.");

            if (String.IsNullOrWhiteSpace(F_ReviewLocation) || !F_ReviewLocation.Equals(FormFields.F_ReviewLocation_Val))
                Defects.Add(@"Review Location is " + F_ReviewLocation + "; expected " + FormFields.F_ReviewLocation_Val);

            if (String.IsNullOrWhiteSpace(F_ModStamp) || !"Yes".Equals(F_ModStamp))
                Defects.Add(@"Moderator did not stamp on the package.");

            if (String.IsNullOrWhiteSpace(F_ReviewStatus) ||
                (!F_ReviewStatus.Equals(FormFields.F_ReviewStatus_Val_Accepted)) &&
                 !F_ReviewStatus.Equals(FormFields.F_ReviewStatus_Val_Revised))
                Defects.Add(@"Review Status is not valid.");

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

                            // 一行只可能匹配 Form item(s) 1, 2, 3... 和 For item(s) 1 - 3 之一，故不必再判断是否匹配另一种形式了
                            continue;
                        }

                        // For item(s) 12 - 15
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
        /// Call CheckWorkProductType() prior to this method, because the FullFileNames and 
        /// BaseFileNames properties are populated there.
        /// </summary>
        public void CheckSCRReportAndPrerequisiteFiles()
        {
            if (null == Reader)
                return;

            bool reportFound = false;  // is SCR report found?
            bool trtFound = false; // is the .TRT file found?

            for (int page = 1; page <= Reader.NumberOfPages; ++page)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                String text = PdfTextExtractor.GetTextFromPage(Reader, page, strategy);
                if (String.IsNullOrWhiteSpace(text))
                    continue;

                #region Check SCR Report
                // 根据特征字符串定位到 SCR Report, 之后从中提取信息
                // 若 check in / insert 到了多个 SCR 呢？
                if (!reportFound &&
                    text.Contains(@"SYSTEM CHANGE REQUEST") &&
                    text.Contains(@"Change Category"))
                {
                    String KEY;
                    String EXPECTEDVALUE;

                    #region extract SCR Status
                    KEY = @"SCR Status:";
                    EXPECTEDVALUE = @"SEC";
                    int startIndex = text.IndexOf(KEY) + KEY.Length;
                    int endIndex = text.IndexOf("SCR Status Date:", startIndex);
                    String status = text.Substring(startIndex, endIndex - startIndex);
                    if (!String.IsNullOrWhiteSpace(status) && !status.Trim().ToUpper().Equals(EXPECTEDVALUE))
                        Defects.Add(@"SCR status is " + status.Trim() + " in SCR Report; expected" + EXPECTEDVALUE);
                    #endregion extract SCR Status

                    #region Function Area
                    KEY = @"Affected Area:";
                    startIndex = text.IndexOf(KEY) + KEY.Length;
                    endIndex = text.IndexOf("Customer No.:", startIndex);
                    String funcArea = text.Substring(startIndex, endIndex - startIndex);
                    if (!String.IsNullOrWhiteSpace(funcArea) && !String.IsNullOrWhiteSpace(F_FuncArea) &&
                        !funcArea.Trim().ToUpper().Equals(F_FuncArea.Trim().ToUpper()))
                        Defects.Add(@"Affected area is " + funcArea.Trim() + " in SCR report, but in coversheet is " + F_FuncArea);
                    #endregion Function Area

                    reportFound = true;
                }
                #endregion Check SCR Report

                #region Check Prerequisite Files
                // 如果 TRT 被更新了，则它至少应出现2次；否则至少应出现1次。
                // 若一个 review package 里包含了多个 CTP 呢？

                #endregion Check Prerequisite Files
            }

            if (!reportFound)
                Defects.Add("Missing SCR Report.");
            if (!trtFound)
                Defects.Add("Missing TRT file.");
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
