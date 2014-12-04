#undef DEBUG_VERBOSE

using iTextSharp.text.pdf;
using Pkg_Checker.Entities;
using Pkg_Checker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pkg_Checker.Helpers
{
    public class Parser
    {
        /// <summary>
        /// Parse the justifications to find items that are not disposed
        /// </summary>
        /// <param name="justifications">Justifications to be parsed</param>
        /// <param name="itemsNoOrNA">A list of NO or N/A items</param>
        /// <returns>A list of not disposed items</returns>
        public static List<int> ParseJustifications(String justifications, List<int> itemsNoOrNA, CheckListType checkListType)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="page"></param>
        /// <param name="annotGroups">manipulate parsed comment here</param>
        /// <returns>status: negatives mean error; 0 means OK; positives mean defect types</returns>
        public static int ParseComments(PdfReader reader, int page, List<AnnotGroup> annotGroups)
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

            // Returning an integer, rather than a list that directly contains the defects. 
            // This can alleviate memory usage and improve performance.
            int result = 0;  // 0: OK; 1: the sticky note on this page is empty; 2: the REPLY to this reply is empty.

            if (null == reader || null == annotGroups)
                return -1;

            PdfObject pageObj = reader.GetPageN(page).Get(PdfName.ANNOTS);
            if (null == pageObj)
                return 0;

            PdfArray annotArray = (PdfArray)PdfReader.GetPdfObject(pageObj);
            if (null == annotArray)
                return 0;

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
                PdfObject _annotSubtype = annotDict.Get(PdfName.SUBTYPE);
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
                String info;
                if (_annotSubtype == PdfName.TEXT)
                    // Defects.Add(_annotC.ToString() + "; "
                    info = (_annotC.ToString() + ""
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
                                // + _annotState.ToString() + "; "
                                // + _annotStateModel.ToString()
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
                        AnnotGroup annotGroup = new AnnotGroup
                        {
                            AnnotP = _annotP,
                            Root = annot,
                            Page = page,
                            Numbers = new List<int>(),
                            IsDefectAccepted = false,
                            IsDefectTypeFound = false,
                            IsDefectSeverityFound = false,
                            IsAuthorWorkCompleted = false,
                            IsModeratorVerifyCompleted = false
                        };
                        annotGroup.Numbers.Add(annot.Number);
                        annotGroups.Add(annotGroup);
                        if (isEmptyContent)
                            // defects.Add(String.Format("The comment on page {0} is empty.", page));
                            result = 1;
                    }
                    else  // the REPLY to this sticky note
                    {
                        AnnotGroup annotGroup = annotGroups.FirstOrDefault(x => x.AnnotP.Number == _annotP.Number && x.Numbers.Contains(_annotIRT.Number));
                        if (null != annotGroup)
                            annotGroup.Numbers.Add(annot.Number);
                        if (isEmptyContent)
                            // defects.Add(String.Format("The comment on page {0} has no reply.", page));
                            result = 2;
                    }
                }
                else  // sticky note's associated attributes
                {
                    if (null == _annotIRT)
                        continue;

                    // Attributes 的 IRT 值位于本 annotation 组其他元素的 Number 集合中。
                    AnnotGroup annotGroup = annotGroups.FirstOrDefault(x => x.AnnotP.Number == _annotP.Number && x.Numbers.Contains(_annotIRT.Number));
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
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contents">The content to be parsed</param>
        /// <returns></returns>
        public static SCRReport ParseSCRReport(String contents)
        {
            // SCR report and "Affected Elements" therein may span multiple pages,
            // and the page number of SCR report is not one-to-one mapping with the PDF page.
            // So locate the SCR report by keywords.
            // BEGIN: SYSTEM CHANGE REQUEST Page # of #
            // END: Closed in Config.: *** 

            Match match = Regex.Match(contents, FormFields.PATTERN_SCR_REPORT_BEGIN_MARK, RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;
            else
            {
                SCRReport report = new SCRReport { Project = match.Groups[1].Value, SubProject = match.Groups[2].Value };

                // Change Category: PROBLEM SCR No.: P 17011.01                        
                // Change Category: INITIAL DEVELOPMENT SCR No.:  08982.04                        
                match = Regex.Match(contents,
                                    @"Change Category:.*SCR No\.:\s*[A-Z]*\s*(" + FormFields.PATTERN_SCR_NUMBER + ")",
                                    RegexOptions.IgnoreCase);
                if (match.Success)
                    report.SCRNumber = float.Parse(match.Groups[1].Value);

                // SCR Status: SEC
                match = Regex.Match(contents, @"SCR Status:\s*([A-Z]{3})");
                if (match.Success)
                    report.Status = match.Groups[1].Value.ToUpper();

                // Affected Area: VGUIDE
                match = Regex.Match(contents, @"Affected Area:\s*(\w{2,})");
                if (match.Success)
                    report.AffectedArea = match.Groups[1].Value;

                // Target Configuration: A350_CERT1_TST_X04
                match = Regex.Match(contents, @"Target Configuration:\s*(\w{2,})");
                if (match.Success)
                    report.TargetConfig = match.Groups[1].Value;

                #region Elements Affected
                String targetArea = contents.SubString(@"Elements Affected:", @"Closure Category:", false, false);
                if (!String.IsNullOrWhiteSpace(targetArea))
                {                    
                    // Having located the "Affected Elements" area, it is time now to parse info therein
                    // match lines like "TEST CTP_A350_FMCI_EVENT_HANDLER.ZIP 4"                    
                    MatchCollection matches = Regex.Matches(targetArea,
                                                            @"^\s*(\w+)\s+(\w{5,}\.\w{3,})\s+(\d+)\s*$",
                                                            RegexOptions.Multiline);
                    if (matches.Count > 0)
                    {
                        report.AffectedElements = new List<CheckedInFile>();
                        foreach (Match item in matches)
                        {
                            report.AffectedElements.Add(new CheckedInFile
                            {
                                SCR = report.SCRNumber,
                                FileName = item.Groups[2].Value.ToUpper(),
                                CheckedInVer = int.Parse(item.Groups[3].Value)
                            });
                        }
                    }
                }
                #endregion Elements Affected

                // Closed in Config.: MD11_922_TST
                match = Regex.Match(contents, FormFields.PATTERN_SCR_REPORT_END_MARK);
                if (match.Success)
                    report.ClosedConfig = match.Groups[1].Value;

                return report;
            }
        }
    }
}
