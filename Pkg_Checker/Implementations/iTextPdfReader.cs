using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using Pkg_Checker.Interfaces;
using System.IO;

namespace Pkg_Checker.Implementations
{
    public class iTextPdfReader : IPdfReader
    {
        #region Properties
        public AcroFields Fields { get; set; }
        public List<String> Defects { get; set; }
        #endregion Properties

        public void Read(string pdfPath)
        {
            PdfReader reader = null;

            try
            {
                Defects = new List<String>();
                reader = new PdfReader(pdfPath);
                Fields = reader.AcroFields;

                PdfDictionary pdfDict = reader.GetPageN(1);
                PdfArray annotArray = pdfDict.GetAsArray(PdfName.ANNOTS);
                for (int i = 0; i < annotArray.Size; ++i)
                {
                    PdfDictionary curAnnot = annotArray.GetAsDict(i);
                    // int someType = myCodeToGetAnAnnotsType(curAnnot);
                    
                }
            }
            
            catch(Exception)
            {
                Fields = null;
            }            
        }
        
        public void CheckCommonFields()
        {            
            if (null != Fields)
            {
                String reviewLocation = Fields.GetField(FormFields.F_ReviewLocation);
                if (String.IsNullOrWhiteSpace(reviewLocation) || !reviewLocation.Equals(FormFields.F_ReviewLocation_Val))                                    
                    Defects.Add(@"Review Location is " + reviewLocation + "; expected " + FormFields.F_ReviewLocation_Val);

                String reviewStatus = Fields.GetField(FormFields.F_ReviewStatus);
                if (String.IsNullOrWhiteSpace(reviewStatus) || 
                    (!reviewStatus.Equals(FormFields.F_ReviewStatus_Val_Accepted)) &&
                     !reviewStatus.Equals(FormFields.F_ReviewStatus_Val_Revised))
                    Defects.Add(@"Review Status is not valid.");
            }                        
        }

        public void CheckWorkProductType()
        {            
            const int maxFileCount = 30;
            List<String> workProductTypes;

            if (null != Fields)
            {
                String workProductType = Fields.GetField(FormFields.F_WorkProductType);                

                if (String.IsNullOrWhiteSpace(workProductType))                
                    Defects.Add(@"Work Product Type is empty.");                
                else
                {
                    List<String> extensions = new List<String>();

                    char[] sep = { ',' };
                    workProductTypes = workProductType.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList<String>();

                    for (int i = 1; i < maxFileCount; ++i)
                    {
                        String fileName = Fields.GetField("F" + i + ".Name");
                        if (!String.IsNullOrWhiteSpace(fileName))
                            extensions.Add(Path.GetExtension(fileName).ToUpper());

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

                    extensions = extensions.Distinct().ToList();

                    // CTP. CTP 没必要一定有 TDF, 也可能只更新了 ZIP
                    if ("Low-Level Test Procedures".Equals(Fields.GetField(FormFields.F_Lifecycle)))
                    {
                        if (!workProductType.Contains("Component Test"))
                            Defects.Add(@"Work Product Type does not match Low-Level Test Procedure");                        

                        // CTP review package 不能包含 SLTP 相关的内容
                        if (workProductType.Contains("Software Test") || extensions.Contains(@".TST"))
                            Defects.Add(@"Low-Level Test Procedure should not contain SLTP related files or Work Product Types");                        
                    }

                    // SLTP
                    if ("High-Level Test Procedures".Equals(Fields.GetField(FormFields.F_Lifecycle)))
                    { 
                        if (!workProductType.Contains(@"Software Test") || !extensions.Contains(@".TST"))
                            Defects.Add(@"Work Product Type does not match High-Level Test Procedure");                        

                        // SLTP review package 不能包含 CTP 相关的内容
                        if (workProductType.Contains("Component Test") || extensions.Contains(@".TDF"))
                            Defects.Add(@"High-Level Test Procedure should not contain CTP related files or Work Product Types");                        
                    }

                    // Trace
                    if (workProductType.Contains(@"Trace Data") ^ extensions.Contains(".TRT"))
                        Defects.Add(@"Tracing: Work Product Type and Checked in files do not match");
                    if (extensions.Contains(".TRT") && String.IsNullOrWhiteSpace(Fields.GetField(FormFields.F_TraceCheckList)))
                        Defects.Add(@"Missing Trace Check List");
                }
            }
        }

        public void CheckCheckList()
        {
            const int checkListItemCount = 45;
            List<int> items = new List<int>();

            for (int i = 1; i <= checkListItemCount; ++i)
            {
                // "Yes", "No", "NA", ""
                if (String.IsNullOrWhiteSpace(Fields.GetField("CTP." + i)))
                    items.Add(i);
            }

            if (items.Count() > 0)
            {
                string temp = @"Item(s) ";
                foreach (int i in items)
                    temp += i + " ";
                Defects.Add(temp + "is/are not checked.");
            }
                
        }

        public List<String> GetDefects()
        {
            return Defects;
        }
    }
}
