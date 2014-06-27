using System;
using Acrobat;
using AFORMAUTLib;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Pkg_Checker
{
    public class Checker
    {
        #region Form Fields

        // public String R_Load { get; set; }
        // public String R_Farea { get; set; }
        // public Char R_DOLevel { get; set; }
        // public String ACM_Project { get; set; }
        // public String ACM_Subproject { get; set; }
        // public String R_ArtifactProduced { get; set; }
        // public String R_SupplierLocation { get; set; }
        // public String R_Aircraft { get; set; }
        // public float M_Duration { get; set; }  // >= 0.5
        // public int M_Number { get; set; }  // expected >= 3

        // public String R_Support { get; set; }
        // public String R_Oversight { get; set; }  // expected Yes       
        public String WP_Artifacts { get; set; }
        public String R_LifeCycle { get; set; }
        public String R_ModChk { get; set; }  // Yes/Off        
        public String R_Status { get; set; }
        public String AuthorName { get; set; }
        public String ModeratorName { get; set; }

        #endregion  // Form Fields


        /// <summary>
        /// Core checking logics
        /// </summary>
        public void Check(String pdfPath, bool saveChanges, ref bool errorOccurred, String resultPath = @".\Pkg_Checker_Result.txt")
        {
            AcroAVDoc avDoc = null;    // A view of a PDF document in a window
            AcroPDDoc pdDoc = null;   // underlying representation of a pdf file            
            IAFormApp formApp = null;
            IFields myFields = null;
            StreamWriter SW = null;
            const String WARNING = "[Warning] ";
            const String INFO = "[Info] ";
            List<int> CTP_Checklist = new List<int>();  // CTP checklist            
            List<String> WorkProductsFileName = new List<String>();            
            int DefectCount = 0;
            int FixedDefectCount = 0;
            
            #region Try to pen the pdf document and get the form fields

            try
            {
                CAcroApp acroApp = new AcroAppClass();
                // acroApp.Show();
                                
                avDoc = new Acrobat.AcroAVDocClass();
                avDoc.Open(pdfPath, "");
                avDoc.SetTitle(pdfPath);

                pdDoc = (AcroPDDoc)avDoc.GetPDDoc();

                // Get the IFields object associated with the form
                formApp = new AFormAppClass();
                myFields = (IFields)formApp.Fields;                
            }

            catch (Exception ex)
            {
                MessageBox.Show("Please ensure that Adobe Acrobat is properly installed and the path is correct. " + ex.Message);
                errorOccurred = true;
                return;
            }

            #endregion // Try to pen the pdf document and get the form fields

            #region Open the result file for writting

            try
            {
                SW = new StreamWriter(resultPath, true, Encoding.Default);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            #endregion Open the result file for writting

            // Write result header                
            SW.WriteLine(System.DateTime.Now.ToString());
            SW.WriteLine("Begin to check " + pdfPath);

            try
            {
                // Get the IEnumerator object for myFields
                IEnumerator myEnumerator = myFields.GetEnumerator();

                while (myEnumerator.MoveNext())
                {
                    // Get the IField object
                    IField myField = (IField)myEnumerator.Current;                    

                    #region Fields Based on Configuration

                    if (myField.Name == "R.Load")
                    {
                        if (Pkg_Checker.Configuration.R_Load != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Load\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.R_Load);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {                                
                                myField.Value = Pkg_Checker.Configuration.R_Load;
                                SW.WriteLine(INFO + "Cover sheet \"Load\" changed to " + Pkg_Checker.Configuration.R_Load);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.Farea")
                    {
                        if (Pkg_Checker.Configuration.R_Farea != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"FAREA\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.R_Farea);
                            ++DefectCount;
                                                        
                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.R_Farea;
                                SW.WriteLine(INFO + "Cover sheet \"FAREA\" changed to " + Pkg_Checker.Configuration.R_Farea);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.DOLevel")
                    {
                        if (Pkg_Checker.Configuration.R_DOLevel != Convert.ToChar(myField.Value))
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"DO 178 Level\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.R_DOLevel);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.R_DOLevel.ToString();
                                SW.WriteLine(INFO + "Cover sheet \"DO 178 Level\" changed to " + Pkg_Checker.Configuration.R_DOLevel);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.Project")
                    {
                        if (Pkg_Checker.Configuration.ACM_Project != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"ACM Project\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.ACM_Project);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.ACM_Project;
                                SW.WriteLine(INFO + "Cover sheet \"ACM Project\" changed to " + Pkg_Checker.Configuration.ACM_Project);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.SubProject")
                    {
                        if (Pkg_Checker.Configuration.ACM_Subproject != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"ACM SubProject\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.ACM_Subproject);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.ACM_Subproject;
                                SW.WriteLine(INFO + "Cover sheet \"ACM SubProject\" changed to " + Pkg_Checker.Configuration.ACM_Subproject);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.ArtifactProduced")
                    {
                        if (Pkg_Checker.Configuration.R_ArtifactProduced != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Produced\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.R_ArtifactProduced);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.R_ArtifactProduced;
                                SW.WriteLine(INFO + "Cover sheet \"Produced\" changed to " + Pkg_Checker.Configuration.R_ArtifactProduced);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.Aircraft")
                    {
                        if (Pkg_Checker.Configuration.R_Aircraft != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Aircraft\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.R_Aircraft);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.R_Aircraft;
                                SW.WriteLine(INFO + "Cover sheet \"Aircraft\" changed to " + Pkg_Checker.Configuration.R_Aircraft);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.SupplierLocation")
                    {
                        if (Pkg_Checker.Configuration.R_SupplierLocation != myField.Value.Trim())
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Producer Employer\" is " + myField.Value + "; expected " + Pkg_Checker.Configuration.R_SupplierLocation);
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.R_SupplierLocation;
                                SW.WriteLine(INFO + "Cover sheet \"Producer Employer\" changed to " + Pkg_Checker.Configuration.R_SupplierLocation);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "M.Duration")
                    {
                        if (Pkg_Checker.Configuration.Min_M_Duration > Convert.ToSingle(myField.Value))
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Meeting Duration\" is invalid.");
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.Min_M_Duration.ToString();
                                SW.WriteLine(INFO + "Cover sheet \"Meeting Duration\" changed to " + Pkg_Checker.Configuration.Min_M_Duration);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "M.Number")
                    {
                        if (Pkg_Checker.Configuration.Min_Review_Participants > Convert.ToInt32(myField.Value))
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Review Participant Number\" is invalid.");
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = Pkg_Checker.Configuration.Min_Review_Participants.ToString();
                                SW.WriteLine(INFO + "Cover sheet \"Review Participant Number\" changed to " + Pkg_Checker.Configuration.Min_Review_Participants);
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    #endregion // Fields Based on Configuration

                    #region General Checkings

                    if (myField.Name == "R.oversight")
                    {
                        if (myField.Value != "Yes")
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Oversight Oversight\" is NOT checked.");
                            ++DefectCount;

                            // Fix this defect
                            if (saveChanges)
                            {
                                myField.Value = "Yes";
                                SW.WriteLine(INFO + "Cover sheet \"Oversight Oversight\" checked.");
                                ++FixedDefectCount;
                            }
                        }
                        continue;
                    }

                    if (myField.Name == "R.Support")
                    {
                        if (String.IsNullOrWhiteSpace(myField.Value))
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Supporting Material(s)/Comments\" is NOT provided.");
                            ++DefectCount;
                        }
                        continue;
                    }

                    #endregion // General Checkings

                    #region Get Field Value for Further Checking

                    if (myField.Name == "R.Lifecycle")
                    {
                        this.R_LifeCycle = myField.Value.Trim();
                        continue;
                    }

                    if (myField.Name == "R.modChk")
                    {
                        this.R_ModChk = myField.Value;
                        continue;
                    }

                    if (myField.Name == "R.Status")
                    {
                        this.R_Status = myField.Value.Trim();
                        continue;
                    }

                    if (myField.Name == "WP.Artifacts")
                    {
                        this.WP_Artifacts = myField.Value.Trim();
                        if (String.IsNullOrWhiteSpace(this.WP_Artifacts))
                        {
                            SW.WriteLine(WARNING + "Cover sheet \"Work Product Type(s)\" is NOT provided.");
                            ++DefectCount;
                        }
                        continue;
                    }

                    if (myField.Name == "E1.Name")
                    {
                        this.AuthorName = myField.Value;
                        continue;
                    }

                    if (myField.Name == "E2.Name")
                    {
                        this.ModeratorName = myField.Value;
                        continue;
                    }

                    // Files
                    for (int i = 1; i <= 40; ++i)
                    {
                        // skip item 11 because this field's value is always "Something here keeps page from being deleted on Save."
                        if (i == 11)
                            continue;

                        if (myField.Name == "F" + i.ToString() + ".Name" && !String.IsNullOrWhiteSpace(myField.Value))
                        {
                            WorkProductsFileName.Add(myField.Value);
                            break;
                        }
                        continue;
                    }

                    // CTP check list                    
                    for (int i = 1; i < 46; ++i)
                    {
                        if (myField.Name == "CTP." + i.ToString() && myField.Value == "Off")
                        {
                            CTP_Checklist.Add(i);
                            break;
                        }
                        continue;
                    }

                    #endregion // Get Field Value for Further Checking
                }

                if (WorkProductsFileName.Count <= 0)
                {
                    SW.WriteLine(WARNING + "No files under review.");
                    ++DefectCount;
                }

                if (CTP_Checklist.Count > 0)
                {
                    String s = "";
                    foreach (var item in CTP_Checklist)
                        s += item.ToString() + ",";
                    SW.WriteLine(WARNING + "CTP check list item " + s + " NOT checked.");
                    ++DefectCount;
                }

                #region Checks basing on configuration

                /*
                if (this.R_Load != Pkg_Checker.Configuration.R_Load)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"Load\" is NOT " + Pkg_Checker.Configuration.R_Load);
                    ++DefectCount;
                }

                if (this.R_Farea != Pkg_Checker.Configuration.R_Farea)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"FAREA\" is NOT " + Pkg_Checker.Configuration.R_Farea);
                    ++DefectCount;
                }

                if (this.R_DOLevel != Pkg_Checker.Configuration.R_DOLevel)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"DO-178 Level\" is NOT " + Pkg_Checker.Configuration.R_DOLevel);
                    ++DefectCount;
                }

                if (this.ACM_Project != Pkg_Checker.Configuration.ACM_Project)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"ACM Project\" is NOT " + Pkg_Checker.Configuration.ACM_Project);
                    ++DefectCount;
                }

                if (this.ACM_Subproject != Pkg_Checker.Configuration.ACM_Subproject)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"ACM Subproject\" is NOT " + Pkg_Checker.Configuration.ACM_Subproject);
                    ++DefectCount;
                }

                if (this.R_Aircraft != Pkg_Checker.Configuration.R_Aircraft)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"Aircraft\" is NOT " + Pkg_Checker.Configuration.R_Aircraft);
                    ++DefectCount;
                }

                if (this.R_ArtifactProduced != Pkg_Checker.Configuration.R_ArtifactProduced)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"Produced\" is NOT " + Pkg_Checker.Configuration.R_ArtifactProduced);
                    ++DefectCount;
                }

                if (this.R_SupplierLocation != Pkg_Checker.Configuration.R_SupplierLocation)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"Producer Employer\" is NOT " + Pkg_Checker.Configuration.R_SupplierLocation);
                    ++DefectCount;
                }

                if (this.M_Duration < Pkg_Checker.Configuration.Min_M_Duration)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"Meeting Duration\" is less than " + Pkg_Checker.Configuration.Min_M_Duration);
                    ++DefectCount;
                }

                if (this.M_Number < Pkg_Checker.Configuration.Min_Review_Participants)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"# Min_Review Participants\" is less than " + Pkg_Checker.Configuration.Min_Review_Participants);
                    ++DefectCount;
                }
                */

                #endregion Checks basing on configuration

                #region Work Products Matching

                // checking for Trace Data and .TRT files
                // .TRT file(s) and "Trace Data" in Work Product Type(s) must occur/not occur at the same time
                // So these 2 conditions can be XOR-ed
                bool TRT_Found = false;
                bool Trace_Data_Found = false;

                if (this.WP_Artifacts != null && this.WP_Artifacts.Contains("Trace"))
                    Trace_Data_Found = true;

                foreach (var item in WorkProductsFileName)
                {
                    if (System.IO.Path.GetExtension(item).ToUpper() == (".TRT"))
                    {
                        TRT_Found = true;
                        break;
                    }
                }

                if (TRT_Found ^ Trace_Data_Found)
                {
                    SW.WriteLine(WARNING + "\"Trace Data\" is chosen but no .TRT file(s) found in \"Work Products Under Review\", or .TRT file(s) present but \"Trace Data\" is not chosen.");
                    ++DefectCount;
                }

                if (this.R_LifeCycle == null)
                {
                    SW.WriteLine(WARNING + "Cover sheet \"Life Cycle\" is NOT provided.");
                    ++DefectCount;
                }

                else
                {
                    if (this.R_LifeCycle == "Low-Level Test Procedures" &&
                        (this.WP_Artifacts == null || !this.WP_Artifacts.Contains("Component Test")))
                    {
                        SW.WriteLine(WARNING + "Chose \"Low-Level Test Procedures\" but no \"Component Test\" found in \"Work Products Types\".");
                        ++DefectCount;
                    }

                    if (this.R_LifeCycle == "High-Level Test Procedures" &&
                        (this.WP_Artifacts == null || !this.WP_Artifacts.Contains("Software Test")))
                    {
                        SW.WriteLine(WARNING + "Chose \"High-Level Test Procedures\" but no \"Software Test\" found in \"Work Products Types\".");
                        ++DefectCount;
                    }
                }

                #endregion // Work Products Matching

                # region if the package is closed

                if (this.R_ModChk == "Yes")
                {
                    if (this.R_Status != "Accepted" && this.R_Status != "Revise")
                    {
                        SW.WriteLine(WARNING + "Cover sheet \"Review Status\" should be \"Accepted As Is\" or \"Revise (No Further Review)\"");
                        ++DefectCount;
                    }
                }

                #endregion

                # region if the package is NOT closed

                else
                {
                    if (this.R_Status == "Accepted" || this.R_Status == "Revise")
                    {
                        SW.WriteLine(WARNING + "Cover sheet \"Review Status\" is " + this.R_Status + " but the package has not been closed.");
                        ++DefectCount;
                    }
                }

                #endregion

                #region Annotations
                
                /*
                AcroPDPage pdPage = null;  // A single page in the PDF representation of a document
                AcroPDAnnot pdAnnot = null;  // An annotation on a page in a PDF file
                String annotContent = "";
                int TotalPages = pdDoc.GetNumPages();
                int TotalAnnot = 0;

                // first page number is 0
                for (int i = 0; i < TotalPages; ++i)
                {
                    pdPage = (AcroPDPage)pdDoc.AcquirePage(i);
                    if (pdPage != null)
                    {
                        TotalAnnot = pdPage.GetNumAnnots();
                        for (int j = 0; j < TotalAnnot; ++j)
                        {
                            pdAnnot = (AcroPDAnnot)pdPage.GetAnnot(j);
                            if (pdAnnot != null)
                            {
                                annotContent = pdAnnot.GetContents();
                                if (pdAnnot.GetSubtype() != "Widget")
                                    MessageBox.Show("Title: " + pdAnnot.GetTitle() + Environment.NewLine
                                        + "Content:" + annotContent + Environment.NewLine
                                        + "Subtype: " + pdAnnot.GetSubtype() + Environment.NewLine
                                        + "Date: " + pdAnnot.GetDate() + Environment.NewLine
                                        + "Color: " + pdAnnot.GetColor() + Environment.NewLine
                                        + "Rect: " + pdAnnot.GetRect() + Environment.NewLine
                                        );
                            }
                        }
                    }
                }
                */ 

                #endregion // Annotations
            }

            catch
            {
                errorOccurred = true;
            }

            finally
            {
                if (SW != null)
                {
                    // Write result end         
                    SW.WriteLine("{0} warning(s) found and {1} of which fixed.", DefectCount, FixedDefectCount);
                    SW.WriteLine("Author: " + AuthorName);
                    SW.WriteLine("Moderator: " + ModeratorName);
                    SW.WriteLine("Checking of " + pdfPath + " ended.");
                    SW.WriteLine("--------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                    SW.Close();
                }

                if (saveChanges && pdDoc != null)
                    pdDoc.Save((short)(PDSaveFlags.PDSaveFull | PDSaveFlags.PDSaveLinearized
                        | PDSaveFlags.PDSaveCollectGarbage), pdfPath);

                // close the pdf file
                // If a positive number, the document is closed without saving it.
                // If 0 and the document has been modified, the user is asked whether or not the file should be saved.
                if (avDoc != null)
                    avDoc.Close(saveChanges ? 0 : 1);
            }
        }
    }
}
