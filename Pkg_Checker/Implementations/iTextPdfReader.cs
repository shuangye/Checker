using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using Pkg_Checker.Interfaces;
using System.IO;
using iTextSharp.text.pdf.parser;

namespace Pkg_Checker.Implementations
{
    public class iTextPdfReader : IPdfReader
    {
        #region Properties
        public PdfReader Reader { get; set; }
        public AcroFields Fields { get; set; }
        public String F_FuncArea { get; set; }
        public int F_RevParticipants { get; set; }
        public String F_ModStamp { get; set; }
        public String F_ReviewLocation { get; set; }
        public String F_WorkProductType { get; set; }
        public String F_Lifecycle { get; set; }
        public String F_ReviewStatus { get; set; }

        public List<String> FullFileNames { get; set; }
        public List<String> BaseFileNames { get; set; }
        public List<String> Defects { get; set; }
        #endregion Properties

        /// <summary>
        /// Call this method before any checking methods to prepare data
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
                String val;

                F_FuncArea = Fields.GetField(FormFields.F_FuncArea);
                val = Fields.GetField(FormFields.F_RevParticipants);                
                F_RevParticipants = String.IsNullOrWhiteSpace(val) ? 0 : int.Parse(val);
                F_ModStamp = Fields.GetField(FormFields.F_ModStamp);
                F_ReviewLocation = Fields.GetField(FormFields.F_ReviewLocation);
                F_WorkProductType = Fields.GetField(FormFields.F_WorkProductType);
                F_Lifecycle = Fields.GetField(FormFields.F_Lifecycle);
                F_ReviewStatus = Fields.GetField(FormFields.F_ReviewStatus);
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
        }

        public void CheckWorkProductType()
        {
            const int maxFileCount = 40;
            List<String> workProductTypes;

            if (null == Fields)
                return;

            if (String.IsNullOrWhiteSpace(F_WorkProductType))
                Defects.Add(@"Work Product Type is empty.");
            else
            {
                FullFileNames = new List<string>();
                BaseFileNames = new List<String>();
                List<String> extensions = new List<String>();

                char[] sep = { ',' };
                workProductTypes = F_WorkProductType.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList<String>();

                for (int i = 1; i < maxFileCount; ++i)
                {
                    if (11 == i)
                        continue; // 因为第11个总是点位符

                    String fileName = Fields.GetField("F" + i + ".Name");
                    if (!String.IsNullOrWhiteSpace(fileName))
                    {
                        String ext = Path.GetExtension(fileName);
                        BaseFileNames.Add(fileName.Substring(0, fileName.IndexOf(ext)).ToUpper());
                        extensions.Add(ext.ToUpper());
                        FullFileNames.Add(fileName.ToUpper());
                    }

                    // 顺带着检查版本
                    String fileVer = Fields.GetField("F" + i + ".Ver");
                    String approvedVer = Fields.GetField("F" + i + ".ApprovedVer");
                    if (String.IsNullOrWhiteSpace(fileVer) ^ String.IsNullOrWhiteSpace(approvedVer))
                        Defects.Add(@"File version or Approved version for file " + i + "is empty.");
                    else if (!String.IsNullOrWhiteSpace(fileVer) && !String.IsNullOrWhiteSpace(approvedVer) &&
                        int.Parse(fileVer) > int.Parse(approvedVer))
                    {
                        Defects.Add(@"File version or Approved version for file " + i + "is not valid.");
                    }
                }

                BaseFileNames = BaseFileNames.Distinct().ToList();
                extensions = extensions.Distinct().ToList();

                // CTP. CTP 没必要一定有 TDF, 也可能只更新了 ZIP
                if ("Low-Level Test Procedures".Equals(F_Lifecycle))
                {
                    if (!F_WorkProductType.Contains("Component Test"))
                        Defects.Add(@"Work Product Type does not match Low-Level Test Procedure");

                    // CTP review package 不能包含 SLTP 相关的内容
                    if (F_WorkProductType.Contains("Software Test") || extensions.Contains(@".TST"))
                        Defects.Add(@"Low-Level Test Procedure should not contain SLTP related files or Work Product Types");
                }

                // SLTP
                if ("High-Level Test Procedures".Equals(F_Lifecycle))
                {
                    if (!F_WorkProductType.Contains(@"Software Test") || !extensions.Contains(@".TST"))
                        Defects.Add(@"Work Product Type does not match High-Level Test Procedure");

                    // SLTP review package 不能包含 CTP 相关的内容
                    if (F_WorkProductType.Contains("Component Test") || extensions.Contains(@".TDF"))
                        Defects.Add(@"High-Level Test Procedure should not contain CTP related files or Work Product Types");
                }

                // Trace
                if (F_WorkProductType.Contains(@"Trace Data") ^ extensions.Contains(".TRT"))
                    Defects.Add(@"Tracing: Work Product Type and Checked in files do not match");
                if (extensions.Contains(".TRT") && String.IsNullOrWhiteSpace(Fields.GetField(FormFields.F_TraceCheckList)))
                    Defects.Add(@"Missing Trace Check List");
            }
        }

        public void CheckCheckList()
        {
            const int checkListItemCount = 45;
            List<int> itemsNotChecked = new List<int>();
            List<int> itemsNoOrNA = new List<int>();

            if (null == Fields)
                return;

            for (int i = 1; i <= checkListItemCount; ++i)
            {
                // "Yes", "No", "NA", ""
                String fieldVal = Fields.GetField("CTP." + i);
                if (String.IsNullOrWhiteSpace(fieldVal))
                    itemsNotChecked.Add(i);
                else if (fieldVal.Contains('N'))
                    itemsNoOrNA.Add(i);
            }

            if (itemsNotChecked.Count() > 0)
            {
                string temp = @"Item(s) ";
                foreach (int i in itemsNotChecked)
                    temp += i + " ";
                Defects.Add(temp + "is/are not checked.");
            }

            // Justification
            if (itemsNoOrNA.Count() > 0)
            {
                String justifications = Fields.GetField(FormFields.F_Justifications_1);
                if (String.IsNullOrWhiteSpace(justifications))
                    justifications = Fields.GetField(FormFields.F_Justifications_2);
                if (String.IsNullOrWhiteSpace(justifications))
                    Defects.Add(@"No justifications for No/NA items.");

                String temp = "";
                foreach (var item in itemsNoOrNA)
                {
                    // 如果写成 for items 41 - 43 这样的形式，就不能如此检查了
                    if (!justifications.Contains(item.ToString()))
                        temp += item + " ";
                }

                if (!String.IsNullOrWhiteSpace(temp))
                    Defects.Add(@"Potentially no justification for item " + temp + ".");
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
                                // PdfName.STATE: ND, ST, Accepted, Minor, Unmarked, In Work, Work Completed, Verified Complete, 
                                // StateModel: DefectType, Resolution Status, DefectSeverity, Is Defect State, Marked
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
