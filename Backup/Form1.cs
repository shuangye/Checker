using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading; 


namespace demo_readpdf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Collect_Defect_Click(object sender, EventArgs e)
        {
            // Thread thread = new Thread(new ThreadStart(Process_Collect)); 
            // thread.Start(); 

            int num = 1;
            

            if (tbPath.Text == "")
            {
                MessageBox.Show("Please input file folder ");
            }
            else
            {
                string[] strFile = Directory.GetFiles(tbPath.Text);
                foreach (string s in strFile)
                {
                    List<string> lsFile = strFile.ToList();

                    PdfDictionary pagedic;
                    PdfObject refs, fs;
                    PdfString pdfannots;
                    string strannots = "";
                    int count = 0;
                    List<string> lsAnnot = new List<string>();
                    List<PdfIndirectReference> lsoldannot = new List<PdfIndirectReference>();
                    List<int> lsPosition = new List<int>();


                    string strRole = "";
                    string strFunction = "";
                    string strName = "";
                    string strAuthor = "";
                    string strModerator = "";
                    string strReviewer = "";
                    string strSystem = "";
                    string strQA1 = "";
                    string strQA2 = "";
                    string strPacket = "";

                    //get packet name
                    string[] nameSplit = s.Split('\\');
                    List<string> lsName = nameSplit.ToList();
                    strPacket = lsName[lsName.Count - 1];
                    string[] arrFiletype = strPacket.Split('.');
                    List<string> lsFiletype = arrFiletype.ToList();
                    string strFiletype = lsFiletype[lsFiletype.Count - 1];

                    List<string> strComments = new List<string>();
                    List<string> strMigration = new List<string>();
                    List<string> strDefectType = new List<string>();
                    List<string> strIsDefectStatus = new List<string>();
                    List<string> strResolutionStatus = new List<string>();
                    List<string> strDefectSeverity = new List<string>();
                    List<string> strReply = new List<string>();
                    List<string> strPage = new List<string>();

                    if (strFiletype.Contains("PDF") || strFiletype.Contains("pdf"))
                    {
                        PdfReader pReader = new PdfReader(s);

                        //Get review packet status
                        AcroFields.Item fieldStatus = pReader.AcroFields.GetFieldItem("R.Status");
                        PdfDictionary dicStatus = fieldStatus.GetValue(0);
                        string R_Status = dicStatus.Get(PdfName.V).ToString();
                        if (R_Status != "Accepted"&& R_Status != "Revise" &&R_Status != "ReReview")
                        {
                            R_Status = "InReview";
                        }

                        //Get CloseDate
                        AcroFields.Item fieldDate = pReader.AcroFields.GetFieldItem("R.CompleteDate");
                        PdfDictionary dicDate = fieldDate.GetValue(0);

                        string R_CompleteDate = "";
                        if (dicDate.Get(PdfName.V) != null)
                        {
                            R_CompleteDate = dicDate.Get(PdfName.V).ToString();
                        }

                        //Read FArea
                        AcroFields.Item fieldFArea = pReader.AcroFields.GetFieldItem("F1.Name");
                        PdfDictionary dicFArea = fieldFArea.GetValue(0);
                        string R_FArea = "";
                        if (dicFArea.Get(PdfName.V) != null)
                        {
                            string strFArea = dicFArea.Get(PdfName.V).ToString();
                            string[] arrFArea = strFArea.Split('_');

                            if (arrFArea[0].Contains("CTP") || arrFArea[0].Contains("ctp"))
                            {
                                if (arrFArea[1].Contains("B7") || arrFArea[1].Contains("A3") || arrFArea[1].Contains("MD") || arrFArea[1].Contains("b7") || arrFArea[1].Contains("a3") || arrFArea[1].Contains("md"))
                                {
                                    R_FArea = arrFArea[2];
                                }
                                else
                                {
                                    R_FArea = arrFArea[1];
                                }
                            }
                            else
                            {
                                R_FArea = arrFArea[0];
                            }
                            if (R_FArea.Contains("þÿ"))
                            {
                                R_FArea = R_FArea.Replace("þÿ", "");
                            }
                            if (R_FArea.Contains("\0"))
                            {
                                R_FArea = R_FArea.Replace("\0", "");
                            }
                            R_FArea = R_FArea.ToUpper();
                        }

                        //Get Author/Moderator info
                        for (int i = 1; i < 39; i++)
                        {
                            string fName = 'E' + i.ToString() + ".Name";
                            string fRole = 'E' + i.ToString() + ".Role";
                            string fFunction = 'E' + i.ToString() + ".Function";
                            AcroFields.Item fieldName = pReader.AcroFields.GetFieldItem(fName);
                            AcroFields.Item fieldRole = pReader.AcroFields.GetFieldItem(fRole);
                            AcroFields.Item fieldFunction = pReader.AcroFields.GetFieldItem(fFunction);

                            if (fieldName != null)
                            {
                                PdfDictionary dicName = (PdfDictionary)fieldName.GetValue(0);

                                if (dicName != null)
                                {
                                    PdfObject dicNameValue = dicName.Get(PdfName.V);
                                    if (dicNameValue != null)
                                    {
                                        strName = dicNameValue.ToString();
                                        if (strName.Contains("þÿ"))
                                        {
                                            strName =strName.Replace("þÿ","");
                                        }
                                        if(strName.Contains("\0"))
                                        {
                                            strName = strName.Replace("\0", "");
                                        }
                                    }

                                    PdfDictionary dicRole = (PdfDictionary)fieldRole.GetValue(0);
                                    PdfString psOPT = dicRole.GetAsString(PdfName.V);
                                    if (psOPT != null)
                                    {
                                        strRole = psOPT.ToString();
                                        if (strRole.Contains('R') && strRole.Contains('c') )
                                        {
                                            strRole = "Recorder";
                                        }
                                        else if (strRole.Contains('A'))
                                        {
                                            strRole = "Auditor";
                                        }
                                        else if(strRole.Contains('S'))
                                        {
                                            strRole = "Safety Focal";
                                        }
                                        else if (strRole.Contains('R') && strRole.Contains('v'))
                                        {
                                            strRole = "Reviewer";
                                        }
                                        else if (strRole.Contains('P'))
                                        {
                                            strRole = "Producer";
                                        }
                                        else if (strRole.Contains('M'))
                                        {
                                            strRole = "Moderator";
                                        }
                                        else if (strRole.Contains('O'))
                                        {
                                            strRole = "Oversight";
                                        }
                                    }
                                    
                                    PdfDictionary dicFunction = (PdfDictionary)fieldFunction.GetValue(0);
                                    PdfString psFun = dicFunction.GetAsString(PdfName.V);
                                    if (psFun != null)
                                    {
                                        strFunction = psFun.ToString();

                                        if (strFunction.Contains('S') && strFunction.Contains('E'))
                                        {
                                            strFunction = "S/W Engineering";
                                        }
                                        else if(strFunction.Contains('S') && strFunction.Contains('T'))
                                        {
                                            strFunction = "S/W Test";
                                        }
                                        else if (strFunction.Contains('S') && strFunction.Contains('y'))
                                        {
                                            strFunction = "Systems";
                                        }
                                        else if (strFunction.Contains('P')&& strFunction.Contains('D'))
                                        {
                                            strFunction = "PDQE";
                                        }
                                        else if (strFunction.Contains('P') && strFunction.Contains('S'))
                                        {
                                            strFunction = "Process";
                                        }
                                        else if (strFunction.Contains('D') && strFunction.Contains('E'))
                                        {
                                            strFunction = "DER";
                                        }
                                    }

                                    if (strRole == "Producer")
                                    {
                                        strAuthor = strName;
                                    }
                                    else if (strRole == "Moderator")
                                    {
                                        strModerator = strName;
                                    }
                                    else if ( strRole == "Reviewer")
                                    {
                                        strReviewer = strName;
                                    }
                                    else if (strFunction == "Systems")
                                    {
                                        strSystem = strName;
                                    }
                                    else if (strFunction == "PDQE")
                                    {
                                        if (strQA1 == "")
                                        {
                                            strQA1 = strName;
                                        }
                                        else
                                        {
                                            strQA2 = strName;
                                        }
                                    }
                                }
                            }
                        }

                        if (R_Status == "Revise" || R_Status == "Accepted")
                        {

                            for (int page = 1; page <= pReader.NumberOfPages; page++)
                            {
                                 List<string> lsMigrationAuthor = new List<string>();
                                 List<string> lsDefectTypeAuthor = new List<string>();
                                 List<string> lsIsDefectStatusAuthor = new List<string>();
                                 List<string> lsResolutionStatusAuthor = new List<string>();
                                 List<string> lsDefectSeverityAuthor = new List<string>();

                                //Read page
                                pagedic = pReader.GetPageN(page);

                                fs = pagedic.Get(PdfName.ANNOTS);
                                if (fs != null)
                                {
                                    //Get annot array list
                                    PdfArray annotarray = (PdfArray)PdfReader.GetPdfObject(fs);

                                    if (annotarray != null)
                                    {
                                        //Read annot array list
                                        foreach (PdfIndirectReference annot in annotarray.ArrayList)
                                        {

                                            //Get the dictionary of each annot
                                            PdfDictionary annotationDic = (PdfDictionary)PdfReader.GetPdfObject(annot);

                                            //Get the pre link
                                            refs = annotationDic.Get(PdfName.IRT);
                                            PdfDictionary prefs = (PdfDictionary)PdfReader.GetPdfObject(refs);

                                            //Get subtype
                                            PdfName annotSubType = (PdfName)annotationDic.Get(PdfName.SUBTYPE);

                                            //Get Status
                                            PdfString annotStaus = (PdfString)annotationDic.Get(PdfName.STATE);

                                            //Get Statustype
                                            PdfName STATEMODEL = new PdfName("StateModel");
                                            PdfString annotStausType = (PdfString)annotationDic.Get(STATEMODEL);

                                            PdfName Highlight = new PdfName("Highlight");
                                            if (annotStaus == null && (annotSubType.Equals(PdfName.TEXT) || annotSubType.Equals(Highlight)) && prefs == null)
                                            {
                                                strPage.Add(page.ToString());
                                                pdfannots = annotationDic.GetAsString(PdfName.CONTENTS);
                                                strannots = pdfannots.ToString();
                                                //add comments and status
                                                strComments.Add(strannots);
                                                strMigration.Add("");
                                                strDefectType.Add("");
                                                strDefectSeverity.Add("");
                                                strIsDefectStatus.Add("");
                                                strResolutionStatus.Add("");
                                                strReply.Add("");

                                                lsAnnot.Add(strannots);
                                                lsoldannot.Add(annot);
                                                lsPosition.Add(count);
                                                count++;
                                            }
                                            if (prefs != null)
                                            {
                                                for (int i = lsoldannot.Count - 1; i >= 0; i--)
                                                {
                                                    PdfDictionary oldannot = (PdfDictionary)PdfReader.GetPdfObject(lsoldannot[i]);
                                                    if (prefs.Equals(oldannot))
                                                    {
                                                        if (annotStaus != null)
                                                        {
                                                            lsAnnot[lsPosition[i]] += '\n' + annotStaus.ToString();

                                                            PdfString content = annotationDic.GetAsString(PdfName.CONTENTS);
                                                            string status = content.ToString();
                                                            string strStausType = annotStausType.ToString();

                                                            if (strStausType == "MigrationStatus")
                                                            {
                                                                PdfString author = annotationDic.GetAsString(PdfName.T);
                                                                Boolean bReset = false;
                                                                if (author != null)
                                                                {
                                                                    for (int authorname = 0; authorname < lsMigrationAuthor.Count;authorname++ )
                                                                    {
                                                                        if (lsMigrationAuthor[authorname] == author.ToString())
                                                                        {
                                                                            bReset = true;
                                                                            string[] strSplit = strMigration[lsPosition[i]].Split(';');
                                                                            List<string> lsSplit = strSplit.ToList();
                                                                            if (authorname < lsSplit.Count)
                                                                            {
                                                                                strSplit[authorname] = status;
                                                                            }
                                                                            strMigration[lsPosition[i]] = "";
                                                                            for (int numSplit = 0; numSplit < lsSplit.Count; numSplit++)
                                                                            {
                                                                                if (strSplit[numSplit] != "")
                                                                                {
                                                                                    strMigration[lsPosition[i]] += strSplit[numSplit] + ';';
                                                                                }
                                                                            }
                                                                            break;
                                                                        }
                                                                        
                                                                    }
                                                                }
                                                                if (bReset == false)
                                                                {
                                                                    strMigration[lsPosition[i]] += status + ';';
                                                                    lsMigrationAuthor.Add(author.ToString());
                                                                }
                                                                
                                                            }
                                                            else if (strStausType == "DefectType")
                                                            {
                                                                PdfString author = annotationDic.GetAsString(PdfName.T);
                                                                Boolean bReset = false;
                                                                if (author != null)
                                                                {
                                                                    for (int authorname = 0; authorname < lsDefectTypeAuthor.Count; authorname++)
                                                                    {
                                                                        if (lsDefectTypeAuthor[authorname] == author.ToString())
                                                                        {
                                                                            bReset = true;
                                                                            string[] strSplit = strDefectType[lsPosition[i]].Split(';');
                                                                            List<string> lsSplit = strSplit.ToList();
                                                                            if (authorname < lsSplit.Count)
                                                                            {
                                                                                strSplit[authorname] = status;
                                                                            }

                                                                            strDefectType[lsPosition[i]] = "";
                                                                            for (int numSplit = 0; numSplit < lsSplit.Count; numSplit++)
                                                                            {
                                                                                if (strSplit[numSplit] != "")
                                                                                {
                                                                                    strDefectType[lsPosition[i]] += strSplit[numSplit] + ';';
                                                                                }
                                                                            }
                                                                            break;
                                                                        }

                                                                    }
                                                                }
                                                                if (bReset == false)
                                                                {
                                                                    strDefectType[lsPosition[i]] += status + ';';
                                                                    lsDefectTypeAuthor.Add(author.ToString());
                                                                }
                                                            }
                                                            else if (strStausType == "Is Defect State")
                                                            {
                                                                PdfString author = annotationDic.GetAsString(PdfName.T);
                                                                Boolean bReset = false;
                                                                if (author != null)
                                                                {
                                                                    for (int authorname = 0; authorname < lsIsDefectStatusAuthor.Count; authorname++)
                                                                    {
                                                                        if (lsIsDefectStatusAuthor[authorname] == author.ToString())
                                                                        {
                                                                            bReset = true;
                                                                            string[] strSplit = strIsDefectStatus[lsPosition[i]].Split(';');
                                                                            List<string> lsSplit = strSplit.ToList();

                                                                            if (authorname < lsSplit.Count)
                                                                            {
                                                                                strSplit[authorname] = status;
                                                                            }
                                                                            strIsDefectStatus[lsPosition[i]] = "";
                                                                            for (int numSplit = 0; numSplit < lsSplit.Count; numSplit++)
                                                                            {
                                                                                if (strSplit[numSplit] != "")
                                                                                {
                                                                                    strIsDefectStatus[lsPosition[i]] += strSplit[numSplit] + ';';
                                                                                }
                                                                            }
                                                                            break;
                                                                        }

                                                                    }
                                                                }
                                                                if (bReset == false)
                                                                {
                                                                    strIsDefectStatus[lsPosition[i]] += status + ';';
                                                                    lsIsDefectStatusAuthor.Add(author.ToString());
                                                                }
                                                            }
                                                            else if (strStausType == "Resolution Status")
                                                            {
                                                                PdfString author = annotationDic.GetAsString(PdfName.T);
                                                                Boolean bReset= false;
                                                                if (author != null)
                                                                {
                                                                    for (int authorname = 0; authorname < lsResolutionStatusAuthor.Count; authorname++)
                                                                    {
                                                                        if (lsResolutionStatusAuthor[authorname] == author.ToString())
                                                                        {
                                                                            bReset = true;
                                                                            string[] strSplit = strResolutionStatus[lsPosition[i]].Split(';');
                                                                            List<string> lsSplit = strSplit.ToList();
                                                                            if (authorname < lsSplit.Count)
                                                                            {
                                                                                strSplit[authorname] = status;
                                                                            }

                                                                            strResolutionStatus[lsPosition[i]] = "";
                                                                            for (int numSplit = 0; numSplit < lsSplit.Count; numSplit++)
                                                                            {
                                                                                if (strSplit[numSplit] != "")
                                                                                {
                                                                                    strResolutionStatus[lsPosition[i]] += strSplit[numSplit] + ';';
                                                                                }
                                                                             }
                                                                            break;
                                                                        }

                                                                    }
                                                                }
                                                                if (bReset == false)
                                                                {
                                                                    strResolutionStatus[lsPosition[i]] += status + ';';
                                                                    lsResolutionStatusAuthor.Add(author.ToString());
                                                                }
                                                            }
                                                            else if (strStausType == "DefectSeverity")
                                                            {                          
                                                                PdfString author = annotationDic.GetAsString(PdfName.T);
                                                                Boolean bReset=false;
                                                                if (author != null)
                                                                {
                                                                    for (int authorname = 0; authorname < lsDefectSeverityAuthor.Count; authorname++)
                                                                    {
                                                                        if (lsDefectSeverityAuthor[authorname] == author.ToString())
                                                                        {
                                                                            bReset = true;
                                                                            string[] strSplit = strDefectSeverity[lsPosition[i]].Split(';');
                                                                            List<string> lsSplit = strSplit.ToList();
                                                                            if (authorname < lsSplit.Count)
                                                                            {
                                                                                strSplit[authorname] = status;
                                                                            }

                                                                            strDefectSeverity[lsPosition[i]] = "";
                                                                            for (int numSplit = 0; numSplit < lsSplit.Count; numSplit++)
                                                                            {
                                                                                if (strSplit[numSplit] != "")
                                                                                {
                                                                                    strDefectSeverity[lsPosition[i]] = strSplit[numSplit] + ';';
                                                                                }
                                                                            }
                                                                            break;
                                                                        }

                                                                    }
                                                                }
                                                                if (bReset == false)
                                                                {
                                                                    strDefectSeverity[lsPosition[i]] += status + ';';
                                                                    lsDefectSeverityAuthor.Add(author.ToString());
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            pdfannots = annotationDic.GetAsString(PdfName.CONTENTS);
                                                            strannots = pdfannots.ToString();
                                                            strReply[lsPosition[i]] += strannots + ' ';
                                                            lsAnnot[lsPosition[i]] += '\n' + strannots;
                                                        }
                                                        lsoldannot.Add(annot);
                                                        lsPosition.Add(lsPosition[i]);
                                                    }

                                                }

                                            }

                                        }

                                    }
                                }
                            }
                            if (0 == strComments.Count)
                            {
                                dgvData.Rows.Add(num, strPacket, R_FArea, R_Status,R_CompleteDate, strAuthor, strModerator, strReviewer, "", "", "", "", "", "", "", "");
                            }
                            else
                            {
                                for (int m = 0; m < strComments.Count; m++)
                                {
                                    dgvData.Rows.Add(num, strPacket, R_FArea, R_Status, R_CompleteDate,strAuthor, strModerator, strReviewer, strPage[m], strComments[m], strMigration[m], strDefectType[m], strIsDefectStatus[m], strResolutionStatus[m], strDefectSeverity[m], strReply[m]);
                                }
                            }
                        }
                        else
                        {
                            dgvData.Rows.Add(num, strPacket, R_FArea, R_Status, strAuthor, R_CompleteDate,strModerator, strReviewer, "", "", "", "", "", "", "", "");
                        }
                        num++;
                    }
                }
            }

        }
        private void Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Please select folder";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                tbPath.Text = foldPath;
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            dgvData.Rows.Clear();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";

            saveFileDialog.FilterIndex = 0;

            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.CreatePrompt = true;

            saveFileDialog.Title = "Export Excel File To";


            saveFileDialog.ShowDialog();


            Stream myStream;

            myStream = saveFileDialog.OpenFile();

            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

            string str = "";

            try
            {

                for (int i = 0; i < dgvData.ColumnCount; i++)
                {

                    if (i > 0)
                    {

                        str += "\t";

                    }

                    str += dgvData.Columns[i].HeaderText;

                }


                sw.WriteLine(str);


                for (int j = 0; j < dgvData.Rows.Count; j++)
                {

                    string tempStr = "";

                    for (int k = 0; k < dgvData.Columns.Count; k++)
                    {

                        if (k > 0)
                        {

                            tempStr += "\t";

                        }
                        string strCell = "";

                        if (dgvData.Rows[j].Cells[k].Value != null)
                        {
                            strCell = dgvData.Rows[j].Cells[k].Value.ToString();

                            if (strCell.Contains('\r') || strCell.Contains('\n'))
                            {
                                if (strCell.Contains('\r'))
                                {
                                    strCell = strCell.Replace('\r', ' ');
                                }

                                else if (strCell.Contains('\n'))
                                {
                                    strCell = strCell.Replace('\n', ' ');
                                }
                            }
                        }
                        tempStr += strCell;

                    }



                    sw.WriteLine(tempStr);

                }

                sw.Close();

                myStream.Close();

            }

            finally
            {

                sw.Close();

                myStream.Close();

            }
        }
    }
}

