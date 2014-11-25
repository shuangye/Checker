/******************************************************************************************************
**
**  Declarations used by the CoverSheet
**
*******************************************************************************************************
*/
// declarations for Cover Sheet
//folder level declaration
var numFormFiles = 130;		//To support Big coversheet, SCR elements>70 (up to 130)
var numMaxReviewers = 38;
var minNumReviewers = 2;
var maxNumChecklists = 100;
var maxFileNameLength = 42;  // ACM and Pathworks limitation //
var aff_aircraft;

ChecklistArray = {
    "Checklist" : null,                          // questions //
    "System Requirements" : "SRS",               // 13 //
    "Software Requirements" : "SRD",             // 54 //
    "Software Design (level 1)" : "SDD_1",       //  4 //
    "Software Design (level 2)" : "SDD_2",       //  5 //
    "Software Design (level 3)" : "SDD_3",       //  5 //
    "Software Design (full)" : "SDD058",         //  8  - must follow levels//
    "Software Design (legacy)" : "SDD",          // 13 //
    "Ada Source" : "Ada",                        // 14 //
    "Rose UML Design Checklist" : "UML",         // 32 //
    "C++ Code Checklist" : "CPLUS",              // 45 //
    "DF File Checklist" : "DF",                  // 20 //
    "System Level Test Procedure" : "SLTP",      // 22 //
    "Component Test Procedure" : "CTP",          // 20 //
    "System Validation Test Procedure" : "ATP",  //  7 //
    "Trace Checklist" : "Trace",                 //  4 //
    "CIT HMI Checklist" : "cit",                 // 29 //
    "Regression Analysis Guide" : "RA",          // 24 //
    "Regression Analysis & Test" : "drat",       // 15 //
    "PDR Checklist" : "pdr",                     // 32 //
    "CDR Checklist" : "cdr",                     // 27 //
    "PDR/CDR Wrap Up Checklist" : "wrap",        //  7 //
    "Process Checklist" : "chk",                 //  7 //
    "Pre-Publication" : "PrePub"                 //  9 //
    }
UnitArray = {   //include each support ACM Subproject DOC type
    "Extension" : null,
    "SDS"   : "anchors", 
    "SRS"   : "anchors", 
    "SRD"   : "anchors", 
    "FMFSDS": "anchors", 
    "FMFSRD": "anchors", 
    "T3"    : "anchors", 
    "SES"   : "anchors", 
    "IFC"   : "pages",
    "SDD"   : "pages",
    "SRC"   : "LOC",
    "C"     : "LOC",
    "ADA"   : "LOC",
    "TRR"   : "LOC",
    "TRC"   : "LOC",
    "ATP"   : "pages",
    "SLTP"  : "anchors",
    "CTP"   : "LOC",
    "PROC"  : "pages",
    "TOOL"  : "LOC",
    "DATA"  : "LOC",
    "IOP"   : "pages",
    "PSAC"  : "pages",
    "SAFE"  : "pages",
    "AS"    : "pages",
    "SCMP"  : "pages",
    "SQAP"  : "pages",
    "SCD"   : "pages",
    "SDP"   : "pages",
    "DOC"   : "pages",
    "DRAT"  : "LOC"
    }    
//end declarations
/******************************************************************************************************
**  function: Oversight Requested
**
**  Purpose: to allow user to request for oversight review
**  Default oversight requested check box is selected, if oversight name present in
**  Oversight_Reviewers.txt it appears in the Oversight row of Participants block
**
********************************************************************************************************
*/
function OversightRequested(doc)
{
	// Opens the Oversight requested checkbox field
	var f = doc.getField("R.oversight");
	var k = doc.getField("R.SupplierLocation");

	var m = doc.getField("E3.Name");
	var x1 = doc.getField("DT.Supplier");
	var x2 = doc.getField("DNT.Supplier");
	var x3 = doc.getField("DP.Supplier");
	var y1 = doc.getField("DT.Total");
	var y2 = doc.getField("DNT.Total");
	var y3 = doc.getField("DP.Total");

	var os_name;
	
	if (f.isBoxChecked(0))
	{
		f.checkThisBox(0,true);
		m.readonly = false;
		k.readonly = false;
		x1.readonly = false;
		x2.readonly = false;
		x3.readonly = false;
		y1.readonly = false;
		y2.readonly = false;
		y3.readonly = false;
		
		k.value = "Select Employer";	
		k.readonly = false;
	}
	else
	{	
		f.checkThisBox(0,false);
		k.value = "Honeywell - Phoenix";
		os_name = m.value
		os_name = os_name.replace(/\s+/g,"");
		m.readonly = true;

		y1.readonly = true;
		y2.readonly = true;
		y3.readonly = true;
	
		var annots = this.getAnnots({nPage:0}); 
		for (var i = 0; i < annots.length; i++) 
		if (annots[i].author == m.value) annots[i].destroy();
	}
}
/******************************************************************************************************
**  function: ModClosure
**
**  Purpose: to allow user to run different checks as chosen from the Moderator
**	     closure checkbox
**
**	1. InspectionClosure: runs PreDistribution, ReviewMeeting, CompletionOfRework, and 
**         InspectionClosure checks (adds moderator Stamp)
**      2. ClosureChecks:    runs PreDistribution, ReviewMeeting, CompletionOfRework, and 
**         InspectionClosure, and CloseAndLock checks (Locks Inspection)
**      3. RemoveStamp:      removes moderator stamp and date complete. (must rerun checks to add again)
********************************************************************************************************
*/
function ModClosure(doc)
{
	// Opens the Moderator checkbox field
	var f = doc.getField("R.modChk");
	
	if (f == null)
	{
		app.alert({cMsg: "Problem adding stamp.", nIcon: 1});
	}

	//Let choice decide if checkbox changes
	if (f.isBoxChecked(0))
	{
		f.checkThisBox(0,false);
	}
	else
	{
		f.checkThisBox(0,true);
	}

	//Creates a menu when the checkbox is clicked on
	var cChoice = app.popUpMenuEx
	(
		{cName: "Inspection Closure and Add Stamp (Moderator)", cReturn: "InspectionClosure"},
		{cName: "Remove Moderator Stamp", cReturn: "Delete"},
		{cName: "Close and Lock Inspection", cReturn: "CloseAndLock"}
	)
		if (cChoice == "InspectionClosure")
		{	//Perform Moderator checks
			try{
				completenessChecks(doc, cChoice);
			} catch (e){app.alert("Completeness Checks Failed");}
		}
		else if(cChoice == "CloseAndLock")
		{
			//Perform closure checks
			try {
				// Remove blank or unchecked wp under review
				completenessChecks(doc, cChoice);
			}catch (e){app.alert("Closure Checks Failed(modclos)");}
		}
		else if(cChoice == "Delete")
		{	//Delete the Moderator Stamp and Date Complete
			ModStamp(doc, cChoice);
			doc.getField("R.CompleteDate").value = "";
			doc.getField("R.modChk").checkThisBox(0,false);
		}
		else
		{       // if no choice made checkbox returns to previous state
			if(f.isBoxChecked(0))
			{
				f.checkThisBox(0, true);	
			}
			else
			{
				f.checkThisBox(0, false);
			}
		}
}

/*******************************************************************************************************
**
**  Functions: completenessChecks and all helping functions
**
**  Purpose:   performs all checking PreDistribution, ReviewMeeting, CompletionOfRework, and 
**             InspectionClosure, and CloseAndLock checks
**
********************************************************************************************************
*/
function completenessChecks(doc, chkType)
{
	// BEGIN DECLARATIONS
	
	//PRE DISTRIBUTION	
	// At least two participants
	var part_missing = false;
	// Participants have assigned Function and Role
	var func_missing = false;  // each has a function assigned
	var role_missing = false;       // each has a role assigned	
	// At least one work product under review
	var wp_missing = false;
	// All elements have size/units
	var size_missing = false;
	var unit_missing = false;
	// Site selected where work product type under review produced.
	var noSite = false;             // site where material produced defined
	// Supplier Location is entered
	var supplierLocation = false;	
	// Work Product Type selected
	var wpType_missing = false;	
	// At least 1 checklist in record
	var chklst_missing = false;
	// At least one name on each checklist
	var chklistName_missing = false;
	// If Review type is Meeting, have meeting date, time
	var Meetingmissing = false;	
	// Producer is present in participants
	var producer_missing = false;
	// Moderator present in participants
	var moderator_missing = false;
	// Oversight present in participants
	var oversight_missing = false;
	// Review missing from participants
	var reviewer_missing = false;
	// Review ID missing
	var reviewid_missing = false;
	// ACM project missing
	var acm_project_missing = false;
	// ACM subproject missing
	var acm_subproj_missing = false;
	
	
	var warningResults = false;  // Used to see if warning checks failed
	var badPreDistribResults = false;  // ROLL UP OF PRE DISTRIBUTION RESULTS
	
	//REVIEW MEETING	
	// If indicate Participant Attended meeting, have non-zero prep time recorded.
	var time_missing = false;       // Has each meeting participant recorded prep time
	// At least 2 (minNumReviewers) participants recorded review time > 0
	var lowPartCnt = false;         // Check if 2 or more Participants (prep time recorded)
	// Stamps must be in place for each participant
	var part_stamp_missing = false;
	// Checklist(s) completed
	var chklst_incompl = false;
	// If Meeting, # of Meeting Participants not blank or zero
	var numMeetingParts_missing = false;                              
	// If Meeting, Meeting Duration not missing
	var meetingDuration_missing = false; 				
	// If Meeting, moderator and at least one reviewer attended
	var modrevAttend_missing = false;
	// Record has valid Review Status
	var badStatus =  false;         // Check Review Status valid
	
	var badReviewMeetingResults = false;  //ROLL UP OF REVIEW MEETING RESULTS
	
	//COMPLETION OF REWORK		

	// If Revise or ReReview, has rework time
	var rework_missing = false;


	var badCompletionOfReworkResults = false;  // ROLL UP OF COMPLETION OF REWORK RESULTS	
	
	//INSPECTION CLOSURE CHECKS
	// Approved version entered
	var appVer_incompl = false;
	// Based on status (if "Revise ..") is rework time recorded and at least 1 file changed
	var reviseStatusIssue = false;
	// If "Accepted .." there is no rework time, and no differing file versions
	var acceptStatusIssue = false;
	// Closure time recorded
	var closureTime_missing = false;

	var badInspectionClosureResults = false;  // ROLL UP OF INSPECTION CLOSURE CHECK RESULTS
	
	//CLOSE AND LOCK CHECKS
	var badCloseAndLockResults = false;  // ROLL UP OF CLOSE AND LOCK CHECK RESULTS
	
	//WARNING LIST
	warningList = new Array(); // Keeps a list of warnings come across in the checks
	var warningMessage = ""; //Formatted list of errors in the form that can be printed
	
	//ERROR LIST		  
	errorList = new Array(); // Keeps a list of errors come across in the checks
	var errorMessage = ""; //Formatted list of errors in the form that can be printed 

	//END DECLARATIONS


	// BEGIN FORM CHECKING
	
	//if not current version skip checks (IN CODE TO ALLOW PACKETS WITH OLDER COVERSHEETS TO BE USED)
	if(doc.getField("FormVersion").value != "Automated")
	{
		if(chkType == "moderator")
		{
			// Opens the Moderator checkbox field
			var f = doc.getField("R.modChk");

			if (f == null)
			{
				app.alert({cMsg: "Problem adding stamp.", nIcon: 1});
			}

			//Let choice decide if checkbox changes
			if (f.isBoxChecked(0))
			{
				f.checkThisBox(0,false);
			}
			else
			{
				f.checkThisBox(0,true);
			}

			var reply = app.popUpMenu("Inspection Closure and Add Stamp (Moderator)",
						  "Remove Moderator Stamp",
						  "Close and Lock Inspection");

			if (reply == "Inspection Closure and Add Stamp (Moderator)")
				badInspectionClosureResults = modClosureComplete(doc);
			else if (reply == "Remove Moderator Stamp")
			{
				ModStamp(doc, "Delete");
				doc.getField("R.CompleteDate").value = "";
				doc.getField("R.modChk").checkThisBox(0,false);
			}
			else if (reply == "Close and Lock Inspection")
			{
				deleteUnusedPages(doc);
				closeOldVersion(doc);
			}
			else
			{       // if no choice made checkbox returns to previous state
				if(f.isBoxChecked(0))
				{
					f.checkThisBox(0, true);	
				}
				else
				{
					f.checkThisBox(0, false);
				}
			}
		}
			
		return;
	}
				
	// If Status = Abort add Moderator Stamp, no closure time, lock and save form
	if(doc.getField("R.Status").value == "Abort")
	{
		statusAbort(doc);
		return;
	}
	
	// uncheck unused keep boxes so only checked filenames are evaluated
	uncheckKeepBoxes(doc);
	
	//BEGIN PRE DISTRIBUTION CHECKS
	//check if participants missing
	part_missing = participantMissing(doc);
	//check if function missing
	func_missing = checkPartFunctionMissing(doc);
	//check if role missing
	role_missing = checkPartRoleMissing(doc);
	//check if work product missing
	wp_missing = workProductMissing(doc);
	//check if size missing
	size_missing = checkElementSizeMissing(doc);
	//check if units missing
	unit_missing = checkElementUnitMissing(doc);
	//check site selected
	noSite = checkSiteProducedMissing(doc);
		
	//check work product type selected
	wpType_missing = checkWPTypeMissing(doc);
	//check at least 1 checklist
	chklst_missing = checklistMissing(doc);
	//check if all checklists have at least one name
	chklistName_missing = checklistNameMissing(doc);
	//check if review type meeting, has date, time, place
	Meetingmissing = checkMeetingValuesMissing(doc);
	//check if producer is present in participants
	producer_missing = producerMissing(doc);
	//check if moderator is present in participants
	moderator_missing = moderatorMissing(doc);
	
	//check if reviewer is present in participants
	reviewer_missing = reviewerMissing(doc);
	//check if review ID is entered
	reviewid_missing = reviewIdMissing(doc);
	//check  if ACM project missing
	acm_project_missing = acmProjCheck(doc);
	//check if ACM subproject missing
	acm_subproj_missing = acmSubProjCheck(doc);
	//check if Oversight checkbox is on, then check for Oversight Reviewer missing
	var f = doc.getField("R.oversight");
	if (f.isBoxChecked(0))
	{
	oversight_missing = oversightMissing(doc);	
	}
	else
	{
	//app.alert("Not checked");
	}
	//check if Oversight checkbox is on, then check for SupplierLocation missing
	var g = doc.getField("R.oversight");
	if (g.isBoxChecked(0))
	{
	supplierLocation = supplierLocationMissing(doc);
	}
	else
	{
	//app.alert("supplierLocation Not checked");
	} 
	
	

	// TO CHANGE A CHECK FROM A WARNING TO AN ERROR, MOVE VARIABLE FROM warningResults TO 
	// badPreDistributionResults AND CHANGE warningList.push() to errorList.push() IN THE IF
	// STATEMENT SECTION BELOW
	warningResults = ((chklst_missing)| (reviewer_missing)| (reviewid_missing)|
			  (acm_project_missing)| (acm_subproj_missing)| (oversight_missing)| (supplierLocation))

	//check if any of previous checks came out true
	badPreDistribResults = ((part_missing)| (func_missing)| (role_missing)| (wp_missing)| 
			        (size_missing)| (unit_missing)| (noSite)| 
				(wpType_missing)| (chklistName_missing)| (Meetingmissing)| 
				(producer_missing)| (moderator_missing)); 
			    
	//if any check returned true add error message to errorList		    
	if (badPreDistribResults || warningResults) 
	{
		app.beep(5);

		if (part_missing)
			errorList.push("Must have at least two participants.");
		if (func_missing) 
			errorList.push("Must supply a function assignment for each Participant. (Except Producer)");
		if (role_missing) 
			errorList.push("Must supply a role assignment for each Participant.");
		if (wp_missing)
			errorList.push("Must have at least one work product under review. \n" + 
				       "      (Must have Problem Report and File Name)");
		if (size_missing) 
			errorList.push("Each file must have a Review Size.");
		if (unit_missing)
			errorList.push("Each file must have  Size Units.")
		if (noSite) 
			errorList.push("Must identify site Where the material under review was Produced\n"+ 
				       "      (where last modified).");
		
		if (wpType_missing) 
			errorList.push("Must have Work Product Type selected.");
		if (chklst_missing) 
			warningList.push("Checklist was not found.");
		if (chklistName_missing)
			errorList.push("Every checklist must have at least one name entered.");
		if (Meetingmissing) 
			errorList.push("You must provide meeting information (e.g. data, time, place).");
		if (producer_missing)
			errorList.push("Producer missing from list of participants.");
		if (moderator_missing)
			errorList.push("Moderator missing from list of participants.");
		//check if Oversight is present in participants
		/*if (oversight_missing)
			errorList.push("Oversight Reviewer missing from list of participants.");*/
		if (reviewer_missing)
			warningList.push("Reviewer missing from list of participants.");
		if (reviewid_missing)
			warningList.push("Review ID field is blank.");
		if (acm_project_missing)
			warningList.push("No ACM Project identified.");
		if (acm_subproj_missing)
			warningList.push("No ACM Subproject identified.");
		if (supplierLocation)
			warningList.push("No Producer Employer selected.");
			//warningList.push("No Supplier Location selected.");
	}
	if (warningList.length > 0)
	{
		app.alert("WARNING:\n" + getErrorList(warningList));
	}
	if (chkType == "PreDistribution")
	{ // if badPreDistribResults Check exit
		if(!badPreDistribResults && errorList.length == 0)
		{			
			app.alert("PreDistribution Checks Successful");
		}
		else
		{
			app.alert("ERRORS:\n" + getErrorList(errorList));
			return false;
		}
		return;
	}
	//END PRE DISTRIBUTION CHECKS	
	
	//BEGIN REVIEW MEETING CHECKS
	//check attend has non zero prep time
	time_missing = checkAttendedTimeMissing(doc);
	//check at least 2 participants > 0 review time
	lowPartCnt = checkPartCountMissing(doc);
	//check if participant stamps missing
	part_stamp_missing = partStampMissing(doc);
	//check if checklists completed
	chklst_incompl = checklistIncomplete(doc);	
	//check if # of Meeting Participants blank
	numMeetingParts_missing = numMeetingPartMissing(doc);
	//check if Meeting Duration Field blank
	meetingDuration_missing = meetingDurationMissing(doc);
	// check if moderator and a reviewer attended meeting
	modrevAttend_missing = checkAttendMissing(doc);
	//check valid review status
	badStatus = checkReviewTypeInvalid(doc);
	
	
	//check if any of previous checks came out true
	badReviewMeetingResults = ((time_missing)| (lowPartCnt)| (part_stamp_missing)| (chklst_incompl)|
				   (numMeetingParts_missing)| (meetingDuration_missing)| 
				   (badStatus)| (modrevAttend_missing));

	//if any check returned true add error message to errorList		    
	if (badReviewMeetingResults) 
	{
		app.beep(5);
		
		if (time_missing) 
			errorList.push("Must have Review Time for each who Attend the meeting.\n" +
				       "      (Except Producer)");
		if (lowPartCnt)
			errorList.push("At least " + minNumReviewers + " participants must recorded review time");
		if (part_stamp_missing)
			errorList.push("Must have a participant Stamp for every participant. (Except Producer)");
		if (chklst_incompl) 
			errorList.push("Checklist(s) found to be incomplete. Need minimum of one stamp on each\n" +
				       "      checklist. (Or all yes, no , na questions have been answered)");
		if (numMeetingParts_missing)
			errorList.push("If Review Type Meeting, need to complete # of Meeting Participants field.");
		if (meetingDuration_missing)
			errorList.push("If Review Type Meeting, need to complete the Meeting Duration field.");
		if (modrevAttend_missing)
			errorList.push("If Review Type Meeting, the moderator and at least one reviewer must\n" +
				       "      have attended the meeting.");
		if (badStatus) 
			errorList.push("Invalid Review Status.");
	}
	if (chkType == "ReviewMeeting")
	{ // if ReviewMeeting Check exit
		if(!badReviewMeetingResults && errorList.length == 0)
			app.alert("Review Meeting Checks Successful");
		else
		{
			app.alert("ERRORS:\n" + getErrorList(errorList));
		}
		return;
	}
	//END REVIEW MEETING CHECKS
	
	//BEGIN COMPLETION OF REWORK CHECKS	
	//check if status is revise and look for rework time if true
	rework_missing = reworkTimeMissing(doc);
	
	//check if any previous checks came out true
	badCompletionOfReworkResults = ((rework_missing)| (reviseStatusIssue)| (acceptStatusIssue));
			       
	if (badCompletionOfReworkResults) 
	{
		app.beep(5);

		if (rework_missing)
			errorList.push("If Status is Revise or ReReview, must enter rework time.");
	}
	
	if (chkType == "CompletionOfRework")
	{ // if PostDistribution Check exit
		if(!badCompletionOfReworkResults && errorList.length == 0)
			app.alert("Completion of Rework Checks Successful");
		else
		{
			app.alert("ERRORS:\n" + getErrorList(errorList));
		}
		return;
	}
	//END COMPLETION OF REWORK CHECKS
	
	//BEGIN INSPECTION CLOSURE CHECKS
	
	//check if status = revise, no issues present
	reviseStatusIssue = checkReviseStatus(doc);
	//check if status = accept, no issues present
	acceptStatusIssue = checkAcceptStatus(doc);
	//check approved version present
	appVer_incompl = checkApprovedVersionMissing(doc);
	//check if closure effort missing
	closureTime_missing = closureTimeMissing(doc);
	
	
	if (appVer_incompl) 
		errorList.push("Approved file version(s) missing.");
	if (reviseStatusIssue) 
		errorList.push("Review Status 'Revise...' must have at least 1 file changed unless work\n" +
			       "      deferral box or versions unchanged box is checked. (for deferral box\n" +
			       "      all SCRs and deferral resolutions must be defined in comments field.)");
	if (acceptStatusIssue) 
		errorList.push("Review Status 'Accepted' should not have differing file versions.");
	if (closureTime_missing)
		errorList.push("Closure Effort must be greater than zero");

  //Added by Fei Wang, Dec 06,2010
  if(meetingDateMissing(doc))//check if meeting date is empty
    errorlist.push("Must have meeting date filled!");
		
  if(meetingTimeMissing(doc))//check if meeting time is empty
		errorList.push("Must have meeting time filled!");
		
  if(meetingDurationMissing(doc))//check if meeting duration is less than zero
		errorList.push("Entered duration time must more than 0!");
		
  if(meetingNumberMissing(doc))//check if meeting participants is less than zero
		errorList.push("Entered meeting participants must more than 0!");
			
  if(FAreaMissing(doc))//check if Function Area is Unknown
		errorList.push("Function Area must be selected!");

  if(LoadMissing(doc))//check if Load Info is N/A,TBD or undefined
		errorList.push("Load Information must be selected!");

  if(AircraftMissing(doc))//check if Aircraft Info is error
    errorList.push("Aircraft Affected must be selected!");
  //End checks added by Fei Wang, Dec 06,2010

	if(errorList.length > 0)
	{
		app.alert("ERRORS:\n" + getErrorList(errorList));
		return;
	}
	
	//inspection closure and stamp placement
	badInspectionClosureResults = modClosureComplete(doc);
	if(badInspectionClosureResults)
		return;
	if(chkType == "InspectionClosure")
		return;	
	//END INSPECTION CLOSURE CHECKS

	//BEGIN CLOSE AND LOCK INSPECTION CHECKS
	badCloseAndLockResults = closureComplete(doc);
	if(badCloseAndLockResults)
	{
		return;
	}
	if(chkType == "CloseAndLock")
		return;	
	//END CLOSE AND LOCK INSPECTION CHECKS	
	//END FORM CHECKING
}

//////  BEGIN INITIAL FORM CHECK FUNCTIONS  //////

// places the Moderator Stamp and Closes Form with Moderator Approval
function closeOldVersion(doc)
{
	// Confirm placement of the Moderator stamp.
	var g = doc.getField("R.modChk");
	if(g != null)
		if(!g.isBoxChecked(0))
		{
			modClosureComplete(doc);
			closureComplete(doc);
		}
		else
			closureComplete(doc);		
}

// If status is Abort, adds Moderator Stamp, clears closure effort, locks and saves the form
function statusAbort(doc)
{
	var x4Moderator = app.alert({
					cMsg: "Abort will lock and save the form as is. \nNo changes may be made.\nDo you wish to continue?",
					cTitle: "Abort - Lock and Save Form",
					nIcon: 2, nType: 2
	});
	if (x4Moderator == 4) 
	{ //  If "Yes"  Lock and Save form
		try 
		{
			// Displays Inspection Complete and hides buttons
			doc.getField("Btn_Complete").display = display.noPrint;

			// Adds moderator stamp and checks box
			doc.getField("R.modChk").checkThisBox(0,true);
			ModStamp(doc,"Add");

			// Clears closure effort
			doc.getField("R.CloseEffort").value = "";

			// locks the form
			chgLockState(doc);
			doc.getField("R.modChk").readonly = true;
			doc.getField("R.CompleteDate").readonly = true;

			hideAllCoverButtons(doc);
			hideChecklistButtons(doc);
			hideKeepBoxes(doc);
		} catch(e) {app.alert("Abort Method Failed");}

		// saves the locked form
		wpList(doc);
		deleteUnusedPages(doc);
		app.execMenuItem("Save");
	}
}

//////  END INITIAL FORM CHECK FUNCTIONS    //////

//////  BEGIN PRE DISTRIBUTION FUNCTIONS    //////

// Check to make sure at least two participants
function participantMissing(doc)
{
	var partCnt = 0;
	
	for(var i=0; i<numMaxReviewers; i++)
	{
		var f = doc.getField("E"+i+".Name");
		if(f != null)
			if(f.value.length >= 1)
				partCnt += 1;
	}
	
	if(partCnt < minNumReviewers)
		return true;
	else
		return false;
}

// Checks if all Participants have a Function assigned returns true if no function assigned
function checkPartFunctionMissing(doc)
{
	for (var iCnt = 1; iCnt <= numMaxReviewers; iCnt++) 
	{
		if(doc.getField("E"+iCnt+".Name") != null)
		{
			if (doc.getField("E"+iCnt+".Name").value.length >=1 && doc.getField("E"+iCnt+".Name").value != "Name here keeps page")
			{
				if(doc.getField("E"+iCnt+".Role").value != "Producer")
				{
					if (doc.getField("E"+iCnt+".Function").value == " ")
					{
						return true;
					}
					if (doc.getField("E"+iCnt+".Function").value == "")
					{
						return true;
					}
				}
			}
		}
	}
	return false;
}

// Checks if all Participants have a Role assigned returns true if no role assigned
function checkPartRoleMissing(doc)
{
	//for (var iCnt = 1; iCnt <= numMaxReviewers; iCnt++) 
	for (var iCnt = 4; iCnt <= numMaxReviewers; iCnt++) 	// first three dedicated rows need not be checked.
	{
		if(doc.getField("E"+iCnt+".Name") != null)
			if (doc.getField("E"+iCnt+".Name").value.length >=1 && doc.getField("E"+iCnt+".Name").value != "Name here keeps page")
				if (doc.getField("E"+iCnt+".Role").currentValueIndices <= 0 || doc.getField("E"+iCnt+".Role").currentValueIndices >= 7) 	//Updated for safety focal
				{
					//app.alert(" Value " + doc.getField("E"+iCnt+".Role").currentValueIndices);
					return true;
				}
	}
	return false;
}

// Check to make sure at least one WP under review is filled in
function workProductMissing(doc)
{
	for(var i=1; i <= numFormFiles; i++)
	{
		var f = doc.getField("keepBox." + i);
		if(f != null)
			if(f.isBoxChecked(0))
				if(doc.getField("F" + i + ".SCR").value != "" && 
				   doc.getField("F" + i + ".Name").value != "")
					return false;
	}
	return true;
}

// Checks if any Elements missing size, returns true if size missing
function checkElementSizeMissing(doc)
{
	try
	{
		for (var iCnt = 1; iCnt <= numFormFiles; iCnt++) 
		{
			var daBox = doc.getField("keepBox." + iCnt);
			if (daBox == null)
				break;
			if (daBox.isBoxChecked(0))
				if(doc.getField("F" + iCnt + ".Units").value != "undefined")
					if (doc.getField("F" + iCnt + ".Size").value <= 0)
						return true;
		}
		return false;
	}catch(e) {app.alert("Error Checking Size")}
}

// Checks if any Element missing units, returns true if units missing
function checkElementUnitMissing(doc)
{
	try
	{
		for (var iCnt = 1; iCnt <= numFormFiles; iCnt++) 
		{
			var daBox = doc.getField("keepBox." + iCnt);
			if (daBox == null)
				break;
			if (daBox.isBoxChecked(0))
				if (doc.getField("F" + iCnt + ".Units").value == "invalid")
					return true;
		}
		return false;
	}catch(e) {app.alert("Error Checking Units")}	
}

// Returns true if Site Produced Field set as invalid
function checkSiteProducedMissing(doc)
{
	if (doc.getField("R.ArtifactProduced").value == "invalid") 
		return true;
	else
		return false;
	
}


// Returns true is WP.Artifacts Field blank
function checkWPTypeMissing(doc)
{
	if(doc.getField("WP.Artifacts").value == "")
		return true;
	else
		return false;
}

// checks for at least one check list in the document, returns true if missing
function checklistMissing(doc) 
{
	try 
	{ 
		for(var i = 0 ; i < maxNumChecklists; i++)
		{
			var f = doc.getField("CHK_" + i + ".ID");
			if (f != null) 
			{
				return false;
			}
		}
		
		//////////////////////////////////////////////////////////////////////////////////
		////////////////////////  BEGIN CODE FOR FMS CHECKLISTS  /////////////////////////
		//////////////////////////////////////////////////////////////////////////////////
			
		//evaluate PDF to find first checklist question, and if found, 
		//all checklist questions have been answered (Yes, No, or NA)
		var firstpass = 0;
		for (var i in ChecklistArray)
		{ 
			if (firstpass == 0) 
			{ 
				firstpass = 1; 
			}
			else 
			{
				try 
				{ 
					var f = doc.getField(ChecklistArray[i]+".ChecklistType");
					if (f != null) 
					{
						return false;
					}
				} catch(e) { console.println(ChecklistArray[i]+" catch"); }
			}
		}
		
		//////////////////////////////////////////////////////////////////////////////////
		////////////////////////  END CODE FOR FMS CHECKLISTS  ///////////////////////////
		//////////////////////////////////////////////////////////////////////////////////
		
		
	} catch(e) { app.alert("checklist missing catch"); }
	//return true;						--Commented on 21-Apr-10 to avoid warning msg "Checklist was not found."
	//Previously checklist had an ID number which was not followed over a period of time and the tool users always got the annoying warning message "Checklist was not found."
}

// Check to make sure at least one name on every checklist
function checklistNameMissing(doc)
{
	try
	{
		// for each possible checklist
		for(var i = 0; i < maxNumChecklists; i++)
		{
			// try to open checklist field
			var f = doc.getField("CHK_" + i + ".ID");

			// if field not null
			if(f != null)
			{
				if(f.value == 0)   // Stamped Checklist
				{
					if(!stampedNameFieldComplete(doc, i))
						return true;
				}
			}
		}

		return false;
	}
	catch(e) { app.alert("Error in checklistIncomplete method")};
}

// Checks to see if name entered into current checklist
function stampedNameFieldComplete(doc, chkListNo)
{
	for(var i = 1; i <=5; i++)
	{
		if(doc.getField("CHK_" + chkListNo + ".Part." + i).value != "enter name" 
			&& doc.getField("CHK_" + chkListNo + ".Part." + i).value.length >= 1)
			return true;
	}
	return false;
}

// Returns true if Review Type Meeting and missing date, time, or location
function checkMeetingValuesMissing(doc)
{
	if (doc.getField("R.Type").value == "Meeting")  
	{
		if (doc.getField("M.Date").value == "") 
			return true;
		if (doc.getField("M.Time").value == "") 
			return true;
		if (doc.getField("M.Location").value == "")
			return true;
	}
	return false;
}

// Checks if the Producer is present in list of participants, returns true if missing
function producerMissing(doc)
{
	for(var iCnt = 1; iCnt < numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Role") != null)
			if(doc.getField("E"+iCnt+".Role").value == "Producer" && doc.getField("E"+iCnt+".Name").value.length > 0)
				return false;
	}
	return true;
}

// Checks if the Moderator is present in list of participants, returns true if missing
function moderatorMissing(doc)
{
	for(var iCnt = 1; iCnt < numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Role") != null)
			if(doc.getField("E"+iCnt+".Role").value == "Moderator" && doc.getField("E"+iCnt+".Name").value.length > 0)
				return false;
	}
	return true;
}

// Checks if the Oversight Reviewer is present in list of participants, returns true if missing
function oversightMissing(doc)
{
	for(var iCnt = 1; iCnt < numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Role") != null)
			if(doc.getField("E"+iCnt+".Role").value == "Oversight" && doc.getField("E"+iCnt+".Name").value.length > 0)
				return false;
	}
	return true;
}
// Returns true if Supplier Location field blank
function supplierLocationMissing(doc)
{
	if (doc.getField("R.SupplierLocation").value == "Select Employer") 
		return true;
	else
		return false;
}

// Displays a warning to the moderator if no participant has the role of Reviewer
function reviewerMissing(doc)
{
	var numReviewers = 0;

	for(var iCnt = 1; iCnt < numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Role") != null)
			if(doc.getField("E"+iCnt+".Role").value == "Reviewer" && doc.getField("E"+iCnt+".Name").value.length > 0)
				numReviewers += 1;
	}

	if(numReviewers == 0)
		return true;
	else
		return false;
}

function reviewIdMissing(doc)
{
	var f = doc.getField("R.Ref_ID");
	if(f != null)
		if(f.value == "")
			return true;
	
	return false;
}

// Returns true if ACM Project field blank
function acmProjCheck(doc)
{
	var eList = new Array();
	
	var f = doc.getField("R.Project");
	
	
	if(f != null)
		if(f.value == "")
			return true;
			
	return false;	
}

// Returns true if ACM Subproject field blank
function acmSubProjCheck(doc)
{
	var g = doc.getField("R.SubProject");
	
	if(g != null)
		if(g.value == "")
			return true;
			
	return false;

}

//////  END PRE DISTRIBUTION FUNCTIONS      //////

//////  BEGIN REVIEW MEETING FUNCTIONS      //////

// Checks if attend checked, then there must be review time checked. If none then returns true
function checkAttendedTimeMissing(doc)
{
	for (var iCnt = 1; iCnt <= numMaxReviewers; iCnt++) 
	{
		if(doc.getField("E"+iCnt+".Attend") != null)
			if (doc.getField("E"+iCnt+".Attend").value == "Yes") 
				if(doc.getField("E"+iCnt+".Role").value != "Producer")
					if (doc.getField("E"+iCnt+".Time").value <= 0)  
						return true;
	}
	return false;
}

function checkPartCountMissing(doc)
{
	var totParts = 0;
	
	for(var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Name") != null)
			if(doc.getField("E"+iCnt+".Name").value.length >= 1)
				totParts += 1;
	}
	
	if(totParts < minNumReviewers)
		return true;
	if(totParts == minNumReviewers)
	{
		var prod = false;
		var mod  = false;
	
		for(var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
		{
			if(doc.getField("E"+iCnt+".Time") != null)
				if(doc.getField("E"+iCnt+".Time").value > 0)
				{
					if(doc.getField("E"+iCnt+".Role").value == "Producer")
						prod = true;
					if(doc.getField("E"+iCnt+".Role").value == "Moderator")
						mod = true;
				}
		}		
		
		if(prod && mod)
			return false;
		else
			return true;
	}
	if(totParts > minNumReviewers)
	{
		var partCnt = 0;

		for (var iCnt = 1; iCnt <= numMaxReviewers; iCnt++) 
		{
			try 
			{
				if(doc.getField("E"+iCnt+".Time") != null)
					if (doc.getField("E"+iCnt+".Time").value > 0)  
						partCnt = partCnt  + 1;
			} catch(e) {}
		}
		if (partCnt < minNumReviewers) 
			return true;
		else
			return false;		
	}
	
}

// Checks if participant stamp is present for every participant, returns true if stamp missing
function partStampMissing(doc)
{
	for(var iCnt = 1; iCnt < numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Name") != null)
			if (doc.getField("E"+iCnt+".Name").value.length >= 1 && doc.getField("E"+iCnt+".Name").value != "Name here keeps page")
				if(doc.getField("E"+iCnt+".Role").value != "Producer")
					if(doc.getField("E"+iCnt+".Attend").isBoxChecked(0))
						if(!doc.getField("E"+iCnt+".Sign").isBoxChecked(0))
							return true;
	}
	return false;
}

// returns true if no stamp or all questions not answered
function checklistIncomplete(doc) 
{
	try
	{
		// for each possible checklist
		for(var i = 0; i < maxNumChecklists; i++)
		{
			// try to open checklist field
			var f = doc.getField("CHK_" + i + ".ID");

			// if field not null
			if(f != null)
			{
				if(f.value == 0)   // Stamped Checklist
				{
					if(!stampedChecklistComplete(doc, i))
						return true;
				}
				else	// Y_N_NA checklist
				{
					if(!ynnaChecklistComplete(doc, i, f.value))
						return true;
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////
		//////////////////  BEGIN CODE FOR FMS CHECKLISTS /////////////////////////////
		///////////////////////////////////////////////////////////////////////////////
		
		var firstpass = 0;
		var foundChkLst = false;
		for (var i in ChecklistArray)
		{ 
			if (firstpass == 0) 
			{ 
				firstpass = 1; 
			}
			else 
			{
				try 
				{ 
					var f = doc.getField(ChecklistArray[i]);
					if (f != null) 
					{
						// if y_n_na checklist
						if(doc.getField(ChecklistArray[i]+".ChecklistType").value == "Y_N_NA")
						{
							foundChkLst = true;
							for (var x = 1; x < 60; x++) 
							{ 
								try 
								{ 
									var q = doc.getField((ChecklistArray[i])+"."+x);
									if (q != null) 
									{
										if (q.value == "Off") 
										{
											console.println("Found checklist "+(ChecklistArray[i])+" question not answered.");
											return true;
										}
									}
								} catch(e) { }
							}
						}	
					}
				} catch(e) { console.println(ChecklistArray[i]+" catch"); }
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////
		//////////////////  END CODE FOR FMS CHECKLISTS ///////////////////////////////
		///////////////////////////////////////////////////////////////////////////////

		return false;
	}
	catch(e) { app.alert("Error in checklistIncomplete method")};
}

// Returns true if at least one stamp appears on the Stamped checklist
function stampedChecklistComplete(doc, chkListNo)
{
	for(var i = 1; i <=5; i++)
	{
		if(doc.getField("CHK_" + chkListNo + "." + i).isBoxChecked(0))
			return true;
	}	
	return false;
}

// Returns true if all of the questions on the Y_N_NA checklist have been answered
function ynnaChecklistComplete(doc, chkListNo, numOfQuestions)
{
	for(var i = 0; i < numOfQuestions; i++)
	{
		var q = doc.getField("CHK_"+ chkListNo + "_Radio." + i);
		if(q != null)
			if(q.value == "Off")
				return false;
	}
	return true;
}

// Returns true if no number entered in the # of Meeting Participants Field
function numMeetingPartMissing(doc)
{
	if(doc.getField("R.Type") != null)
		if(doc.getField("R.Type").value == "Meeting")
			if(doc.getField("M.Number") != null)
				if(doc.getField("M.Number").value == "")
					return true;
	
	return false;
}

// Returns true if Meeting Duration field blank
function meetingDurationMissing(doc)
{
	if(doc.getField("R.Type") != null)
		if(doc.getField("R.Type").value == "Meeting")
			if(doc.getField("M.Duration") != null)
				if(doc.getField("M.Duration").value == "")
					return true;
			
	return false;
}

// Checks if Review Type Meeting, that moderator and at least one reviewer attended meeting
function checkAttendMissing(doc)
{
	if(doc.getField("R.Type") != null)
		if(doc.getField("R.Type").value != "Meeting")
			return false;

	var totParts = 0;
	
	for(var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
	{
		if(doc.getField("E"+iCnt+".Name") != null)
			if(doc.getField("E"+iCnt+".Name").value.length >= 1)
				totParts += 1;
	}
	
	
	if(totParts < minNumReviewers)
		return true;
	if(totParts == minNumReviewers)
	{
		var prod = false;
		var mod  = false;
	
		for(var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
		{
				if(doc.getField("E"+iCnt+".Attend") != null)
					if(doc.getField("E"+iCnt+".Attend").value == "Yes")
					{
						if(doc.getField("E"+iCnt+".Role").value == "Producer")
							prod = true;
						if(doc.getField("E"+iCnt+".Role").value == "Moderator")
							mod = true;
					}
		}		
		
		if(prod && mod)
			return false;
		else
			return true;		
	}
	if(totParts > minNumReviewers)
	{
		var rev = false;
		var mod  = false;
	
		for(var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
		{
				if(doc.getField("E"+iCnt+".Attend") != null)
					if(doc.getField("E"+iCnt+".Attend").value == "Yes")
					{
						if(doc.getField("E"+iCnt+".Role").value == "Reviewer")
							rev = true;
						if(doc.getField("E"+iCnt+".Role").value == "Moderator")
							mod = true;
					}
		}		
		
		if(rev && mod)
			return false;
		else
			return true;	
	}
}

// Returns true if Review Type is invalid
function checkReviewTypeInvalid(doc)
{
	if (doc.getField("R.Status").value == "invalid") 
		return true;
	else
		return false;
}

//////  END REVIEW MEETING FUNCTIONS        //////

//////  BEGIN COMPLETION OF REWORK FUNCTIONS//////

// If Status = Revise, return true if no rework time
function reworkTimeMissing(doc)
{
	if (doc.getField("R.Status").value == "Revise" || doc.getField("R.Status").value == "ReReview") 
	{
		//check for rework time
		if(doc.getField("R.Rework") != null)
			if(doc.getField("R.Rework").value <= 0) 
				return true;
	}
	return false;
}

//////  END COMPLETION OF REWORK FUNCTIONS  //////

//////  BEGIN INSPECTION CLOSURE FUNCTIONS  //////

// If Status = Revise, if deferred checkbox or versions unchanged
// checkbox not checked makes sure there is at least one file change
function checkReviseStatus(doc)
{
	//reviseStatusIssue check for rework time and a difference in file versions
	if (doc.getField("R.Status").value == "Revise") 
	{
		if(versionBlank(doc))
			return false;
		

		if(doc.getField("R.VUnchanged").isBoxChecked(0))
		{
			var x4Moderator = app.alert({
							cMsg: "The Version(s) Unchanged box is checked. Only the Moderator should check this box.  Do you want to continue?",
							cTitle: "Version(s) Unchanged",
							nIcon: 2, nType: 2
			});
			if (x4Moderator == 4) 
			{ //  If "Yes" then return false;
				return false;
			}
			else
			{
				return true;
			}
		}

		//counter for at least one different file version
		var difFiles = 0;
		//check for at least one file changed unless deferred checkbox checked
		if(!doc.getField("R.deferred").isBoxChecked(0))
		{
			for (var iCnt = 1; iCnt <= numFormFiles; iCnt++) 
			{						
				var daBox = doc.getField("keepBox."+iCnt);
				if (daBox != null) 
					if (daBox.isBoxChecked(0))
					{
						var f = doc.getField("F"+iCnt+".Name");
						if (f != null)
							if(f.value.length != 0)	
								if(doc.getField("F"+iCnt+".ApprovedVer").value != doc.getField("F"+iCnt+".Ver").value)
									difFiles += 1;
					}
			}
			if(difFiles == 0)
				return true;
		}
		else
		{
			var x4Moderator = app.alert({
							cMsg: "The work deferred box is checked. Are all SCR's and deferral resolutions defined in Supporting Material(s)/Comments Field?",
							cTitle: "Work Deferred",
							nIcon: 2, nType: 2
			});
			if (x4Moderator == 4) 
			{ //  If "Yes" then return false;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
	return false;
}

// If Status = Accept, returns true if there are differing file versions
function checkAcceptStatus(doc)
{
	//acceptStatusIssue check for no rework time and no difference in file versions
	if (doc.getField("R.Status").value == "Accepted")
	{
	
		// Auto populate the Approved Version from the Version field
		for(var i = 1; i <= numFormFiles; i++)
		{
			if(doc.getField("F"+i+".ApprovedVer") != null)
				doc.getField("F"+i+".ApprovedVer").value = doc.getField("F"+i+".Ver").value;
		}

		if(versionBlank(doc))
			return false;

		//check for no changes in file
		for (var iCnt = 1; iCnt <= numFormFiles; iCnt++) 
		{					
			var daBox = doc.getField("keepBox."+iCnt);
			if (daBox != null) 
				if (daBox.isBoxChecked(0))
				{
					var f = doc.getField("F"+iCnt+".Name");
					if (f != null)
						if(f.value.length != 0)			
							if(doc.getField("F"+iCnt+".ApprovedVer").value != doc.getField("F"+iCnt+".Ver").value)
								return true;
				}	
		}
	}
	return false;
}

// If all versions fields are blank, return true
function versionBlank(doc)
{
	for(var iCnt = 1; iCnt <= numFormFiles; iCnt++)
	{
		var f = doc.getField("F"+iCnt+".Ver");
		
		var g = doc.getField("keepBox."+iCnt);
		
		if(g != null)
			if(g.isBoxChecked(0))
				if(f != null)
					if(f.value != "")
						return false;
	}
	return true;
}

// Returns true if Approved Version Field not filled in
function checkApprovedVersionMissing(doc)
{
	try
	{
		for (var iCnt = 1; iCnt <= numFormFiles; iCnt++) 
		{
			var daBox = doc.getField("keepBox." + iCnt);
			if (daBox == null)
				break;
			if (daBox.isBoxChecked(0))
				if (doc.getField("F" + iCnt + ".ApprovedVer").value <= 0)
					return true;
		}
		return false;
	}catch(e) {app.alert("Error Checking Approved Version")}	
}

// Return true if Closure time is not greater than zero
function closureTimeMissing(doc)
{
	var f = doc.getField("R.CloseEffort");
	if(f != null)
		if(f.value <= 0)
			return true;
			
	return false;
}

// Performs the moderator closure and adds stamp if successful
function modClosureComplete(doc)
{
	
	// Check if Moderator Stamp already on the Form
	var modBox = doc.getField("R.modChk");
	if (modBox != null)
		if(!modBox.isBoxChecked(0))
		{	

			app.beep(5);    
			//  If "Pass" all checks then present Yes/No message with any manual checks required.
			var x4Moderator = app.alert({
							cMsg: "Automated checks pass. \n\nHave other manual checks passed?",
							cTitle: "Add Moderator Stamp Now",
							nIcon: 2, nType: 2
			});
			if (x4Moderator == 4) 
			{ //  If "Yes" (manual checks completed) then 
			  //     Checkbox and add Moderator Stamp.
				doc.getField("R.modChk").checkThisBox(0,true);
				ModStamp(doc,"Add");
				// rollup wp list and delete unused pages
				wpList(doc);
				deleteUnusedPages(doc);
				//     Set Complete Date.
				try 
				{ 
					doc.getField("R.CompleteDate").visible = true;
					doc.getField("R.CompleteDate").readonly = false;
					var d = new Date();
					var cDate = util.printd("m/d/yy", d);
					doc.getField("R.CompleteDate").value = cDate;
					doc.getField("R.CompleteDate").readonly = true;
				} catch(e) {}
				//Also make "Mark Complete" button visible.
				try 
				{
					var f = doc.getField("FormTools");
					f.display = display.noPrint;
				} catch(e) {}
				return false;
			} 
			else 
			{ // If "No" then remove Moderator Stamp, Hide "Mark Complete" button
				doc.getField("R.modChk").checkThisBox(0,false);
				ModStamp(doc,"Delete");
				var f = doc.getField("Btn_Complete");
				f.display = display.hidden;
				return true;
			}
		}
	
}

//////  END INSPECTION CLOSURE FUNCTIONS    //////

//////  BEGIN CLOSE AND LOCK FUNCTIONS      //////

// Performs Closure checks, if successful save and lock form
function closureComplete(doc)
{
	// check if Moderator Checkbox is checked (Stamp is on form)
	if(doc.getField("R.modChk").isBoxChecked(0))
	{ 
		var x4Moderator = app.alert({
						cMsg: "No changes may be made after form has been locked.\n\nDo you want to close and lock the form?\n",
						cTitle: "Close and Lock Form",
						nIcon: 2, nType: 2
		});
		if (x4Moderator == 4) 
		{ //  If "Yes" (close and lock form) 
			try 
			{
				var f = doc.getField("Btn_Complete");
				if(f != null)
					f.display = display.noPrint;
				hideAllCoverButtons(doc);
				hideChecklistButtons(doc);
				hideKeepBoxes(doc);
				chgLockState(doc);
				doc.getField("R.modChk").readonly = true;
				doc.getField("R.CompleteDate").readonly = true;
				f.value = "INSPECTION COMPLETE";

			} catch(e) {app.alert("Error in ClosureComplete");}
			app.execMenuItem("Save");
			return false;
		}
	}
	return true;
}

// hides the keep boxes for the work products under review
function hideKeepBoxes(doc)
{
	for(var i = 1; i <= numFormFiles; i++)
	{
		var f = doc.getField("keepBox." + i);
		if(f != null)
			f.display = display.hidden;
	}
}

//  hides all buttons on checklists and locks answers or stamps
function hideChecklistButtons(doc)
{
	for(var i = 0; i < maxNumChecklists; i++)
	{

		var f = doc.getField("CHK_"+i+".ID");

		if(f != null)
			if(f.value == 0)
			{
				for(var s = 1; s <= 5; s++)
				{
					var j = doc.getField("CHK_" + i + "." + s);
					if (j!= null)
						doc.getField("CHK_" + i + "." + s).readonly = true;
						
					j = doc.getField("CHK_" + i + ".Part." + s);			
					if (j != null)
						doc.getField("CHK_" + i + ".Part." + s).readonly = true;
				}
			}
			else
			{
				var g = doc.getField("CHK_" + i + "_AllYes");
				if(g != null)
					g.display = display.hidden;
				
				g = doc.getField("CHK_" + i + "_Clear");
				if(g != null)
					g.display = display.hidden;
					
				for(var s = 0; s < f.value; s++)
					doc.getField("CHK_" + i + "_Radio." + s).readonly = true;
			}
	}
}

// hides all the buttons located on the coversheet
function hideAllCoverButtons(doc) 
{
	// make all buttons hidden
	if(doc.getField("FormTools") != null)
		doc.getField("FormTools").display = display.hidden;
	if(doc.getField("lockStatus") != null)
		doc.getField("lockStatus").display = display.hidden;
	if(doc.getField("FormMakePdf") != null)
		doc.getField("FormMakePdf").display = display.hidden;
	if(doc.getField("AutoSave") != null)
		doc.getField("AutoSave").display = display.hidden;
	if(doc.getField("2Top") != null)
		doc.getField("2Top").display = display.hidden;
	if(doc.getField("Help.Start") != null)
		doc.getField("Help.Start").display = display.hidden;
	if(doc.getField("Help.Closure") != null)
		doc.getField("Help.Closure").display = display.hidden;
	if(doc.getField("Help.WP") != null)
		doc.getField("Help.WP").display = display.hidden;
	if(doc.getField("Help.Support") != null)
		doc.getField("Help.Support").display = display.hidden;
	if(doc.getField("Help.Participant") != null)
		doc.getField("Help.Participant").display = display.hidden;
	if(doc.getField("Help.Status") != null)
		doc.getField("Help.Status").display = display.hidden;
	if(doc.getField("R.OSdBstatus") != null)
		doc.getField("R.OSdBstatus").display = display.hidden;
	if(doc.getField("QuerydB") != null)
		doc.getField("QuerydB").display = display.hidden;		
}

// delete the unused pages of wp and participant data
function deleteUnusedPages(doc)
{
	var f = doc.getField("E6.Name"); //5-Oct-09
	if(f != null)
		if(f.value == "" || f.value == "Name here keeps page")
			doc.deletePages(f.page);
			
	f = doc.getField("F11.Name");
	if(f != null)
		if(f.value == "" || f.value == "Something here keeps page from being deleted on Save.")
			doc.deletePages(f.page);
			
	f = doc.getField("F41.Name");
	if(f != null)
		if(f.value == "" || f.value == "Something here keeps page from being deleted on Save.")
			doc.deletePages(f.page);
}

// runs through the list of WP under review and removes unchecked or empty lines
function wpList(doc)
{
	//creates an array to store each checked line
	aArray = new Array();
	
	//try all rows up to the max number of 70
	for(var i = 1; i <= numFormFiles; i++)
	{
		var f = doc.getField("keepBox."+i);
		
		//try to open current keepbox
		if(f != null)
		{
	            if(f.isBoxChecked(0)) // if the box is checked
	            {
	            	row = new Array(7);

	            	//store each value into the array
	                row[0] = doc.getField("F"+i+".SCR").valueAsString;
	                row[1] = doc.getField("F"+i+".Name").value;
	                row[2] = doc.getField("F"+i+".Ver").value;
	                row[3] = doc.getField("F"+i+".Type").value;
	                row[4] = doc.getField("F"+i+".Size").value;
	                row[5] = doc.getField("F"+i+".Units").value;
	                row[6] = doc.getField("F"+i+".ApprovedVer").value;
	            	//add the current row to the other checked lines
	                aArray.push(row);
	   		
	   		// clear the current row
	   		clearWpRow(doc, i);
		    }
		    else // if the box unchecked
		    {
		    	//clear the current row
		    	clearWpRow(doc, i);
		    }
	        } 	       
	}		
	// populate the form with data from the array
	repopulateWp(doc, aArray);
}

// clears the current wp row 
function clearWpRow(doc, row)
{
	doc.getField("F"+row+".SCR").value = "";
	doc.getField("F"+row+".Name").value = "";
	doc.getField("F"+row+".Ver").value = "";
	doc.getField("F"+row+".Type").value = "";
	doc.getField("F"+row+".Size").value = "";
	doc.getField("F"+row+".Units").value = "invalid";
	doc.getField("F"+row+".ApprovedVer").value = "";
	doc.getField("keepBox."+row).checkThisBox(0, false);
}

//populates wp under review list with the contents of aArray
function repopulateWp(doc, aArray)
{
	// Repopulate the rows with only the checked rows
	for(var i = 0; i < aArray.length; i++)
	{
	    var iCnt = i + 1;
	    
	    // add each value back to the current row and check the keepbox
	    doc.getField("F"+iCnt+".SCR").value = aArray[i][0];
	    doc.getField("F"+iCnt+".Name").value = aArray[i][1];
	    doc.getField("F"+iCnt+".Ver").value = aArray[i][2];
	    doc.getField("F"+iCnt+".Type").value = aArray[i][3];
	    doc.getField("F"+iCnt+".Size").value = aArray[i][4];
	    doc.getField("F"+iCnt+".Units").value = aArray[i][5];
	    doc.getField("F"+iCnt+".ApprovedVer").value = aArray[i][6]; 
	    doc.getField("keepBox."+iCnt).checkThisBox(0, true);
	}
	
	for(var r = aArray.length + 1; r < numFormFiles; r++)
	{
	    var q = doc.getField("keepBox." + r);
	    if(q != null)
	    	q.checkThisBox(0, false);
	}
}

//////  END CLOSE AND LOCK FUNCTIONS        //////

//////  BEGIN MISC FUNCTIONS                //////

// Clears check marks from keep boxes where Problem Report Field blank
function uncheckKeepBoxes(doc)
{
	for(var w = 1; w <= numFormFiles; w++)
	{
	    var kBox = doc.getField("keepBox." + w);

	    if(kBox != null)
		if(doc.getField("F"+w+".Name") != null)
		    if (doc.getField("F"+w+".Name").value.length == 0 || doc.getField("F"+w+".Name").value == "Something here keeps page from being deleted on Save.")
			kBox.checkThisBox(0, false);
	}
}
	
// Given an array of error messages, formats messages into one numbered list
function getErrorList(list)
{
	var messageString = "";
	
	for(var i = 0; i < list.length ; i++)
	{
		var num = i + 1;
		
		if(num < 10)
			messageString = messageString + num + ".   " + list[i] + "\n";
		else
			messageString = messageString + num + ". " + list[i] + "\n";
	}
	return messageString;
}

//////  END MISC FUNCTIONS                 ///////


/*******************************************************************************************************
**
**  Functions: timer, autosave
**
**  Purpose: Timer used in the autosave feature
**
********************************************************************************************************
*/

//saves the file after interval
function incrTimer() {
	//app.alert("Saving...")   // Uncomment to test Saving Function (if uncommented forces save after every interval completed)
	if (this.dirty) {
		app.execMenuItem("Save");
		//app.alert("Saved")  // Uncomment to test Saving Function
	}
}

//Start the timer to run for given interval
function start_timer() {
	runt = app.setInterval("incrTimer()", 300000 );  // 5 minutes 
}

//Stops the timer by clearing the interval
function stopTimer() {
	try {
		app.clearInterval(runt);
	}catch (e) {}
}

//toggles the autosave button to start and stop the timer
function chgAutosaveStatus(doc)
{
	try
	{
		//open field
		var f = doc.getField("AutoSave");
		if(f != null)
		{
			//get caption of the autosave button
    			var test = f.buttonGetCaption();
    
    			//if disabled start timer and change caption
    			if(test == "Autosave Is Off")
    			{
        			f.buttonSetCaption("Autosave Is On");
        			f.strokeColor = color.white;
				f.fillColor = color.brGreen; 
				start_timer();
    			}
    			//if enabled stop timer and change caption
    			else
    			{
        			f.buttonSetCaption("Autosave Is Off");
				f.strokeColor = color.white;
				f.fillColor = color.red;
				stopTimer();
    			}
		}
	}
	catch (e) {app.alert({nIcon: 1, cMsg: "Setup incomplete."}); }


}

/*******************************************************************************************************
**
**  Functions: Lock and Unlocking
**
**  Purpose: functions that lock and unlock the fields of the form.
**
********************************************************************************************************
*/


///////////////////////////////////////////////////////////////////////
//////////////// Only Locks an FMS Checklist  /////////////////////////
///////////////////////////////////////////////////////////////////////

// Purpose: Lock checklist fields
//          Attempts to lock fields for every possible checklist.
//          When checklist is added, update this function.
//   send alert - only called from toolbar
//---------------------------------
function LockChecklists(doc)
{
var firstpass = 0;
for (var i in ChecklistArray)
   { if (firstpass == 0) { firstpass = 1; }
      else {
         try { 
            var f = doc.getField(ChecklistArray[i]);
            if (f != null) f.readonly = true;
         } catch(e) { console.println(ChecklistArray[i]+" catch"); }
     }
   }
   app.alert({cMsg: "Checklists locked.", nIcon: 2});
}


///////////////////////////////////////////////////////////////////////
////////////////  Only UnLocks an FMS Checklist  //////////////////////
///////////////////////////////////////////////////////////////////////

// Purpose: Lock checklist fields
//          Attempts to lock fields for every possible checklist.
//          When checklist is added, update this function.
//   send alert - only called from toolbar
//---------------------------------
function UnLockChecklists(doc)
{
var firstpass = 0
for (var i in ChecklistArray)
    { if (firstpass == 0) { firstpass = 1; }
      else {
         try { 
             var f = doc.getField(ChecklistArray[i]);
             if (f != null) f.readonly = false;
         } catch(e) { console.println(ChecklistArray[i]+" catch"); }
      }
    }
   app.alert({cMsg: "Checklists unlocked.", nIcon: 2});
}

// Purpose: Set ACM Project, Subproject, SCR, Filename, GENeration, 
//          and Element Type form fields ReadOnly attribute to true.
//          This ensures form data is retained when the PDF file is saved.
//   send alert - only called from toolbar
//---------------------------------
function LockFiles(doc)
{
   try 
     {
       var f = doc.getField("R.Project")
       if (f != null) f.readonly = true;
       var f = doc.getField("R.SubProject")
       if (f != null) f.readonly = true;
   } catch(e) {}
   try 
     {
      for (var iCnt = 1; iCnt <= numFormFiles; iCnt++)
        {
        try { 
           var f = doc.getField("F"+iCnt);
           if (f != null) f.readonly = true;
        } catch(e) { console.println("F"+iCnt+" catch"); }
        }
   } catch(e) {}
   app.alert({cMsg: "Files locked.", nIcon: 2});
}

function tb_UnLockFiles(doc)
{
	unLockFiles(doc);
	app.alert({cMsg: "Files unlocked.", nIcon: 2});
}


// Purpose: Set ACM Project, Subproject, SCR, Filename, GENeration, 
//          and Element Type form fields ReadOnly attribute to false.
//          This ensures form data is retained when the PDF file is saved.
//   send alert - only called from toolbar
//---------------------------------
function unLockFiles(doc)
{
   try 
     {
       var f = doc.getField("R.Project")
       if (f != null) f.readonly = false;
       var f = doc.getField("R.SubProject")
       if (f != null) f.readonly = false;
   } catch(e) {}
   for (var iCnt = 1; iCnt <= numFormFiles; iCnt++)
   {
        try { 
           var f = doc.getField("F"+iCnt);
           if (f != null) f.readonly = false;
        } catch(e) { console.println("F"+iCnt+" catch"); }
   }
}

//if readonly then change button to Locked, otherwise change to unlock.
function chgLockStatus(doc)
{
	try { 
	    var locked_record = false;
	    var f = doc.getField("R.Status");
	    if (f != null) if (f.readonly) locked_record = true;
	    var f = doc.getField("lockStatus");
	    if (f != null) {
		    f.buttonSetCaption("Unlocked");
		    f.strokeColor = color.black;
		    f.fillColor = color.brGreen;
		    if (locked_record) {
				f.buttonSetCaption("Locked");
				f.strokeColor = color.white;
				f.fillColor = color.red; 
			}
	    }
   } catch(e) {}
}

//   send alert - only called from toolbar
function tb_lock(doc)
{ 
	rr_lock(doc);
	WpUnderReview(doc);
	chgLockStatus(doc);
	app.alert({cMsg: "Review Record locked.", nIcon: 2});
}

// Purpose: Lock form data so stays with PDF/FDF file.
function rr_lock(doc)
{
     // Lock all fields starting with M
     try {
       var f = doc.getField("M");
       if (f != null) f.readonly = true;
         } catch(e) {}
     // Lock all fields starting with R
     try {
       var f = doc.getField("R");
       if (f != null) f.readonly = true;
         } catch(e) {}
     // Lock all fields starting with WP
     try {
       var f = doc.getField("WP");
       if (f != null) f.readonly = true;
         } catch(e) {}
     // Lock all Participant Data (E#)
     for (var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
         {
            try { 
               var f = doc.getField("E"+iCnt);
               if (f != null) f.readonly = true;
            } catch(e) { console.println("E"+iCnt+" catch"); }
          }
     // Lock FormStarted field
     try {
          var f = doc.getField("FormStarted");
          if (f != null) f.readonly = true;
         } catch(e) {}
     // Lock all WP under Review data (F#)
     for (var iCnt = 1; iCnt <= numFormFiles; iCnt++)
         {
            try { 
               var f = doc.getField("F"+iCnt);
               if (f != null) f.readonly = true;
            } catch(e) { console.println("F"+iCnt+" catch"); }
          }
     // Lock all FMS Checklists if any         
	var firstpass = 0;
	for (var i in ChecklistArray)
	   { if (firstpass == 0) { firstpass = 1; }
	      else {
		 try { 
		       var f = doc.getField(ChecklistArray[i]);
		       if (f != null) f.readonly = true;
		       f = doc.getField(ChecklistArray[i]+"_AllYes");
		       if(f != null) f.display = display.hidden;
		       f = doc.getField(ChecklistArray[i]+"_Clear");
		       if(f != null) f.display = display.hidden;
		 } catch(e) { console.println(ChecklistArray[i]+" catch"); }
	     }
	   }
         
  	// Lock all stamped checklist names and stamp check boxes
	for(var q = 0; q < maxNumChecklists; q++)
	{
		try
		{
			var f = doc.getField("CHK_"+q+".ID");
			if(f != null)
				if(f.value == 0)
					for(var s = 1; s <= 5; s++){
						var j = doc.getField("CHK_" + q + "." + s);
						if (j!= null)
							doc.getField("CHK_" + q + "." + s).readonly = true;
						
						j = doc.getField("CHK_" + q + ".Part." + s);			
						if (j != null)
							doc.getField("CHK_" + q + ".Part." + s).readonly = true;
					} 
		}catch(e) {console.println("Checklist " + q + " catch"); }
	}
       
}

 //   send alert - only called from toolbar
function tb_unlock(doc)
{
	rr_unlock(doc);
	chgLockStatus(doc);
	app.alert({cMsg: "Review Record unlocked.", nIcon: 2});
}

// Purpose: Unlock form data in PDF/FDF file.
// If file extension is PDF, then unlock fields for Author and/or meeting
// Always unlock Participant Data and Checklist
function rr_unlock(doc)
{
	// Unlock all fields starting with M
	try 
	{
		var f = doc.getField("M");
		if (f != null) 
			f.readonly = false;
	} catch(e) {}
	// Unlock all fields starting with R
	try 
	{
		var f = doc.getField("R");
		if (f != null) 
			f.readonly = false;
	} catch(e) {}
	// Unlock all fields starting with WP
	try 
	{
		var f = doc.getField("WP");
		if (f != null) 
			f.readonly = false;
	} catch(e) {}
	// Unlock participant fields
	for (var iCnt = 1; iCnt <= numMaxReviewers; iCnt++)
	{
		try 
		{ 
			var f = doc.getField("E"+iCnt);
			if (f != null) 
				f.readonly = false; 
		} catch(e) { console.println("E"+iCnt+" catch"); }
	}
	
	// Unlock WP fields
     	for (var iCnt = 1; iCnt <= numFormFiles; iCnt++)
        {
        	try 
        	{ 
               		var f = doc.getField("F"+iCnt);
               			if (f != null) 
               				f.readonly = false;
            	} catch(e) { console.println("F"+iCnt+" catch"); }
        }
        
        // If FMS Checklist attached, unlock answers and buttons	
	var firstpass = 0
	for (var i in ChecklistArray)
	{ 
		if (firstpass == 0) 
		{ 
			firstpass = 1;
		}
		else 
		{
			try
			{ 
				var f = doc.getField(ChecklistArray[i]);
				if (f != null) f.readonly = false;
				
				f = doc.getField(ChecklistArray[i]+"_AllYes");
				if (f != null) 
					f.display = display.noPrint;
				if (f != null) 
					f.readonly = false; 
				f = doc.getField(ChecklistArray[i]+"_Clear");
				if (f != null) 
					f.display = display.noPrint;
				if (f != null) 
					f.readonly = false; 	
				
			} catch(e) { console.println(ChecklistArray[i]+" catch"); }
		}
	}
	
	// Unlock Stamped Checklist names and check boxes
	for(var q = 0; q < maxNumChecklists; q++)
	{
		try
		{
			var f = doc.getField("CHK_"+q+".ID");
			if(f != null)
				if(f.value == 0)
					for(var s = 1; s <= 5; s++)
					{
						var j = doc.getField("CHK_" + q + "." + s);
						if (j!= null)
							doc.getField("CHK_" + q + "." + s).readonly = false;
						
						j = doc.getField("CHK_" + q + ".Part." + s);			
						if (j != null)
							doc.getField("CHK_" + q + ".Part." + s).readonly = false;
					} 
		}catch(e) {console.println("Checklist " + q + " catch"); }
	}
	
	var isFDF = ANSendCommentsToAuthorEnabled(doc);
	if (isFDF != true)
	{// not FDF file
		if (doc.path.search(/.pdf/i) > 0) //file is PDF
		{
			try {
				var f = doc.getField("M");
				if (f != null) f.readonly = false;
			} catch(e) {}
			try {
				var f = doc.getField("WP");
				if (f != null) f.readonly = false;
			} catch(e) {}
			try {
				var f = doc.getField("R");
				if (f != null) f.readonly = false;
			} catch(e) {}
			for (var i in ChecklistArray)
			{
				try { 
					var f = doc.getField(ChecklistArray[i]);
					if (f != null) f.readonly = false;
				} catch(e) { console.println(ChecklistArray[i]+" catch"); }
			}
		} // if PDF
	}
	//after "Create PDF from Multiple Files" file name starts with "Binder*"
	//Remind user of naming convention
	if (doc.path.search(/binder/i) > 0) 
	  {
		try { //hide FormMakePdf ("Create PDF form multiple") button
			var f = doc.getField("FormMakePdf");
			if (f != null) f.display = display.hidden;
		} catch(e) {  }
		try { //show FormSaveAs ("SaveAs") Button
			var f = doc.getField("FormSaveAs");
			if (f != null) f.display = display.noPrint;
		} catch(e) {  }
		// hide files unchecked (won't be reviewed)
		//lockWpElements(doc);			////////////////////Need a different methods here
	  }
	  if (doc.numPages <= 2) unLockFiles(doc);
}

//if readonly then Locked, otherwise unlock record.
function chgLockState(doc) {
	try { 
		var locked_record = false;
		var f = doc.getField("R.Status");
		if (f != null) if (f.readonly) locked_record = true;
		if (locked_record) {
			rr_unlock(doc)
		} else {
			rr_lock(doc)
		}
		chgLockStatus(doc);
	} catch(e) {}
}

/*******************************************************************************************************
**
**  Functions:  Stamps
**
**  Purpose: To add/remove participant, moderator, and checklist stamps from the form
**
********************************************************************************************************
*/
// Adds or Deletes the Moderator Stamp to/from the Form
function ModStamp(doc,daAction)
{
	// creates new date object
	var d = new Date();
	doc.syncAnnotScan();
	if (daAction == "Add") 
	{  // #DApproved - Adds the stamp
		try 
		{
			var annot = doc.addAnnot({
				page: 0,
				type: "Stamp",
				author: "Moderator Stamp",
				//name: "Mod Stamp - " +util.printd("HHMMss", d),
				name: "Mod Stamp - " +util.printd("yyyy-mm-dd HH:MM:ss", d),		//changed for mysql dB
				rect: [460.6, 643.7, 576.2, 667.3] , 
				AP: "#DApproved"
			});
			//473, 663, 561, 687
		} catch(e) {}
	}
	if (daAction == "Delete") 
	{ // Deletes the stamp
		var annots = doc.getAnnots({nPage:0 });  //look only on first page
		try 
		{ 
			for (var i = 0; i < annots.length; i++) 
			{
				if (annots[i].author == "Moderator Stamp") annots[i].destroy();
			}
		} catch(e) {}
	}
}


// Adds or Deletes the Participant Stamp to/from the form
function addPartStamp (doc,row, page)
{ // Dependent on installation of Dynamic.pdf which comes with Acrobat
  // Assume delete all stamps for row, not just stamp on top.
   if(page == 0)
  		position = new Array(510.04,190.52,591.23,207.52);
  		else
  		position = new Array(510.67,707.77,591.87,724.77);
  		
	var f = doc.getField("E"+row+".Sign");
	if (f == null) 
	{
		app.alert({cMsg: "Problem adding stamp.", nIcon: 1});
		return;
	}
	
	// Let choice decide of checkbox changes or not
	if (f.isBoxChecked(0))
	{
		f.checkThisBox(0,false);
	}
	else
	{ 
		f.checkThisBox(0,true);
	}


	var cChoice = app.popUpMenuEx
	(
		{cName: "Add Stamp", cReturn: "Add"},
		{cName: "Remove Stamp", cReturn: "Delete"}
	)
		if (cChoice=="Delete")
		{// Deletes the Participant stamp from the row
			doc.syncAnnotScan();
			var annots = this.getAnnots({nPage:page});  
			if(annots != null)
			{
				try 
				{
					for (var i = 0; i < annots.length; i++) 
					{
						//if (annots[i].author == "E"+row+" Stamp") annots[i].destroy(); 
						if (annots[i].author == doc.getField("E"+row+".Name").value) annots[i].destroy();//1-oct-09
					}
				} catch(e) {}
			}
			f.checkThisBox(0,false);
		} 
		else if (cChoice=="Add") 
		{ // add stamp
				
			// make sure no existing stamp
			if(!f.isBoxChecked(0))
			{

				var d = new Date();
				// warn if no time logged
				var t = doc.getField("E"+row+".Time");
				if (t != null) 
				{
					if (doc.getField("E"+row+".Time").value <=0 )
					{
						app.alert({cMsg: "Need to fill in Review Time", nIcon: 1});
						return;
					}
				}
				
				if(page == 0)
				{
					// add the stamp					
					var annot = doc.addAnnot({
						page: page,
						type: "Stamp",
						//author: "E"+row+" Stamp", 						
						author: doc.getField("E"+row+".Name").value, //1-oct-09
						//name: "Reviewer Stamp - " +util.printd("HHMMss", d),
						name: "Reviewer Stamp - " +util.printd("yyyy-mm-dd HH:MM:ss", d),		//changed for mysql dB
						rect: [position[0],position[1]-((row+1)*18.5),position[2],position[3]-((row+1)*18.5)] , 
						AP: "#DReviewed"
					});
					//500,206-(row*18),600,223-(row*18)
					//532,206-(row*18),600,224-(row*18)
					//505,182-((row-1)*18),597,201-((row-1)*18)
					// display a checkmark in box	
					f.checkThisBox(0,true);
					supportUpdate(doc);
				}
				else
				{
				// add the stamp
					var annot = doc.addAnnot({
						page: page,
						type: "Stamp",
						//author: "E"+row+" Stamp", 
						author: doc.getField("E"+row+".Name").value, //1-oct-09
						//name: "Reviewer Stamp - " +util.printd("HHMMss", d),
						name: "Reviewer Stamp - " +util.printd("yyyy-mm-dd HH:MM:ss", d),		//changed for mysql dB
						rect: [position[0],position[1]-((row-6)*18.5),position[2],position[3]-((row-6)*18.5)] , 
						AP: "#DReviewed"
					});
					// display a checkmark in box	
					f.checkThisBox(0,true);
				}
				
			}
			else
			{
				app.alert("Participant stamp already in place!\n To remove use 'Remove Stamp' option.");
			}
		}
		// if no choice (add/delete) is made checkbox goes to previous state
		else 
		{
			if(f.isBoxChecked(0))
			{
				f.checkThisBox(0, true);	
			}
			else
			{
				f.checkThisBox(0, false);
			}	
		}
        //add function by Xinghua.Liu
        //when auchor stamp on, it will delete unuse page and update checklist auto.
        //deleteUnusedPages(doc);
		checklistUpdate(doc);

}


// adds or deletes the stamps from a Stamped Checklist
function addCheckStamp (doc, chkListNo, chkStampNo, myPage)
{ // Dependent on installation of Dynamic.pdf which comes with Acrobat
  // chkStampNo string end with single digit used to determine stamp location

  	if(chkStampNo == 1)
  		placement = new Array(48.778793,14.478867,140.135132,33.607651);
  	else if (chkStampNo == 2)
  		placement = new Array(152.828827,13.207535,250.257538,33.607788);
  	else if (chkStampNo == 3)
  		placement = new Array(262.778046,13.207535,360.206757,33.607788);
  	else if (chkStampNo == 4)
  		placement = new Array(373.415253,13.207535,470.843964,33.607788);
  	else
  		placement = new Array(483.33316,13.975906,576.458191,33.475021);
  		
  
  
	var f = doc.getField("CHK_" + chkListNo + "." + chkStampNo);
	if (f == null) {
		app.alert({cMsg: "Problem adding stamp.", nIcon: 1});
		return;
	}
	if (f.isBoxChecked(0))
		f.checkThisBox(0,false);
	else 
		f.checkThisBox(0,true);


	var reply = app.popUpMenuEx
	(
		{cName: "Add Stamp", cReturn: "Add Stamp"},
		{cName: "Remove Stamp", cReturn: "Remove Stamp"}
	)
	
	if (reply != null) 
		{
		//"Remove Stamp" 
		if (reply == "Remove Stamp") 
			{
			doc.syncAnnotScan();
			var annots = doc.getAnnots({nPage:myPage }); 
			try {
				for (var i = 0; i < annots.length; i++) 
				{
					if (annots[i].name == "Automated Checklist Stamp"+chkStampNo) 
						if (annots[i].author == chkStampNo+" Stamp") 
							annots[i].destroy();
					
				}
			f.checkThisBox(0,false);
			} catch(e) {}
		} else if (reply == "Add Stamp")
			{ // "Add Stamp" 
			var annot = doc.addAnnot({
				page: myPage,
				type: "Stamp",
				author: chkStampNo+" Stamp",
				name: "Automated Checklist Stamp"+chkStampNo,
				rect: [placement[0],placement[1],placement[2],placement[3]] , 
				AP: "#DReviewed"
			});
			f.checkThisBox(0,true);
		} else 
		  {
			if(f.isBoxChecked(0))
			{
				f.checkThisBox(0, true);	
			}
			else
			{
				f.checkThisBox(0, false);
			}
		  }
	}

}

/*******************************************************************************************************
**
**   Functions:  Work Product Type
**
**   Purpose:    Adds the work product type to the form
**
********************************************************************************************************
*/

function SetWpGlobals()
{

	var cChoice = app.popUpMenuEx
	(
		{cName: "System Requirements   ", bMarked:global.bSRS},
		{cName: "Software Requirements ", bMarked:global.bSRD},
		{cName: "Software Design       ", bMarked:global.bSDD},
		{cName: "Source                ", bMarked:global.bSRC},
		{cName: "Interface             ", bMarked:global.bIFC},
		{cName: "Model                 ", bMarked:global.bMDL},
		{cName: "Trace Data            ", bMarked:global.bTRC},
		{cName: "-"},
		{cName: "System Test (ATP)     ", bMarked:global.bATP},
		{cName: "Software Test (SLTP)  ", bMarked:global.bSLTP},
		{cName: "Component Test (CTP)  ", bMarked:global.bCTP},
		{cName: "Regression Analysis   ", bMarked:global.bDRAT},
		{cName: "Process Document      ", bMarked:global.bPCD},
		{cName: "-"},
		{cName: "Other - See comments  ", bMarked:global.bOthr}
	);

	//app.alert("You chose the \"" + cChoice + "\" menu item");

	if(cChoice=="System Requirements   ") global.bSRS = ! (global.bSRS);
	if(cChoice=="Software Requirements ") global.bSRD = ! (global.bSRD);
	if(cChoice=="Software Design       ") global.bSDD = ! (global.bSDD);
	if(cChoice=="Source                ") global.bSRC = ! (global.bSRC);
	if(cChoice=="Interface             ") global.bIFC = ! (global.bIFC);
	if(cChoice=="Model                 ") global.bMDL = ! (global.bMDL);
	if(cChoice=="Trace Data            ") global.bTRC = ! (global.bTRC);
	if(cChoice=="System Test (ATP)     ") global.bATP = ! (global.bATP);
	if(cChoice=="Software Test (SLTP)  ") global.bSLTP = ! (global.bSLTP);
	if(cChoice=="Component Test (CTP)  ") global.bCTP = ! (global.bCTP);
	if(cChoice=="Regression Analysis   ") global.bDRAT = ! (global.bDRAT);
	if(cChoice=="Process Document      ") global.bPCD  = ! (global.bPCD);
	if(cChoice=="Other - See comments  ") global.bOthr = ! (global.bOthr);

}


function SetWpArtifacts(doc)
{
	var WP = "";
	var bFirst = true;

	if (global.bSRS) { 
		WP = "System Requirements";
		bFirst = false; }
	if (global.bSRD) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Software Requirements";
	}
	if (global.bSDD) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Software Design";
	}
	if (global.bSRC) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Source";
	}
	if (global.bIFC) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Interface";
	}
	if (global.bMDL) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Model";
	}
	if (global.bTRC) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Trace Data";
	}
	if (global.bATP) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"System Test";
	}
	if (global.bSLTP) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Software Test";
	}
	if (global.bCTP) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Component Test";
	}
	if (global.bDRAT) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Regression Analysis";
	}
	if (global.bPCD) {
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Process Document";
	}
	if (global.bOthr) { 
		if (bFirst) bFirst = false;
		else WP = WP+", ";
		WP = WP+"Other";
	}
	//app.alert("Status :"+WP);
		var f = doc.getField("WP.Artifacts");
		if (f == null) {
		} else {
		f.value = WP;
		}
}


function InitializeWpGlobals(doc)
{
	var f = doc.getField("WP.Artifacts");
	if (f == null) { return; } 
	try {
		var strWP = f.value;
		strWP = strWP.split(",");
		} catch(e) { return; }
	var totCnt = strWP.length
	for (var cnt = 1; cnt < totCnt; cnt++) {
		console.println(strWP(cnt));
		if(strWP(cnt)=="System Requirements") global.bSRS = true;
		if(strWP(cnt)=="Software Requirements") global.bSRD = true;
		if(strWP(cnt)=="Software Design") global.bSDD = true;
		if(strWP(cnt)=="Source") global.bSRC = true;
		if(strWP(cnt)=="Interface") global.bIFC = true;
		if(strWP(cnt)=="Model") global.bMDL = true;
		if(strWP(cnt)=="Trace Data") global.bTRC = true;
		if(strWP(cnt)=="System Test") global.bATP = true;
		if(strWP(cnt)=="Software Test") global.bSLTP = true;
		if(strWP(cnt)=="Component Test") global.bCTP = true;
		if(strWP(cnt)=="Regression Analysis") global.bDRAT = true;
		if(strWP(cnt)=="Other") global.bOthr = true;
	}
}

/*******************************************************************************************************
**
**  Function:  Help
**
**  Purpose:  Adds help alerts to the help buttons located on the form
**
********************************************************************************************************
*/

//Adds content to the Help buttons located on the Cover Sheet
function showHelp(field)
{
	if(field == "Status")
	{
		app.alert("Accepted As Is - \n" +
			  "     Work product is accepted as is. No fixes are required.\n\n" +
			  "Revise -\n" + 
			  "     Work product is subject to verification of defect\n" + 
			  "     correction by the moderator or assignee before final\n" + 
			  "     acceptance.\n\n" +
			  "Re-Inspect-\n" + 
			  "     Work product is not accepted. Rework is needed. All or\n" +
			  "     portions of the work product must be re-inspected.\n\n" +
			  "Abort-\n" + 
			  "     At judgment of the moderator and participants, it is a\n" +
			  "     waste of time to continue inspection. Once material has\n" +
			  "     been reworked, a complete new inspection must be\n" + 
			  "     performed.", 3);
	}
	if(field == "Closure")
	{
		app.alert("By clicking the Moderator Closure checkbox you may choose:\n" +
			  "     - Moderator Checks and Add Stamp\n" +
			  "          Performs all closure checks and if successful\n" +
			  "          places the Moderator Stamp and Date Complete on\n" +
			  "          the form.\n" +
			  "     - Closure Checks and Lock Form\n" +
			  "          Performs all closure checks and if successful\n" +
			  "          places the Moderator Stamp and Date Complete on\n" +
			  "          the form. The form is then saved and locked so no\n" +
			  "          further changes may be made.\n" +
			  "     - Remove Stamp\n" +
			  "          Removes the Moderator Stamp and Date Complete\n" +
			  "          from the form.", 3);
	}
	if(field == "Participants")
	{
		app.alert("For each participant in the list:\n" +
		          "     - Name: First and Last, eMail address is also acceptable\n" +
		          "     - Function (discipline)/Responsibility:\n" +
		          "            -Review Time (hours): Participants enter the time\n" +
		          "             they spend on this inspection, before they return\n" +
		          "             their markups\n" +
		          "            -Role in review: Select from the drop down, must\n" +
		          "             satisfy process\n" +
		          "            -Attend: Identifies those that attended a review meeting\n" +
		          "            -Will Close: Box is checked by a review participant,\n" +
		          "             indicating that they want to see the product before the\n" +
		          "             review is complete.\n" +
		          "            -Signature check complete: Checked by the participant\n" +
		          "             while they are logged into the machine and before they\n" +
		          "             send their markups to the producer\n" , 3);
	}
	if(field == "Support")
	{
		app.alert("This field is provides supporting materials and\n" +
			  "comments that help explain what happened during the\n" +
			  "the inspection process.", 3);
	}
	if(field == "Start")
	{
		app.alert("Needs to be added");
	}
	if(field == "WP")
	{
		app.alert("For each file name in Work Products under review:\n" +
			  "     - Provide Problem Report ID\n" +
			  "     - Provide the name of the files that are under review only\n" + 
			  "     - Provide the version of the file that is under review\n" +
			  "     - Provide the size of the material under review\n" +
			  "     - Provide the units for the Review size\n" + 
			  "     - When defect corrections are verified, provide the version\n" +
			  "     - of the file that is approved", 3);
	}
}

/*******************************************************************************************************
**
**  Function:  tool button menu
**
**  Purpose:   adds the options for the tool button menu
**
********************************************************************************************************
*/
//creates the functionality for the toolMenu for the tool button
function doc_util(doc)
{ 
  var lc = doc.getField("R.Lifecycle").value;		//Life Cycle
// Purpose:  Tool button menu
//---------------------------------
  //Complete Review is not required if Lifecycle is High-Level Test Procedures or Low-Level Test Procedures or Structural Coverage Analysis.
  if(lc == "High-Level Test Procedures" || lc == "Low-Level Test Procedures" || lc == "Structural Coverage Analysis")
  {
  	var reply = app.popUpMenu("Pre-Distribution Checks (Producer)",
				"Review Meeting Checks (Moderator)",
				"Completion of Rework Checks (Producer)",
				"Update Participant Data");
  }
  else
  {
	var reply = app.popUpMenu("Pre-Distribution Checks (Producer)",
				"Review Meeting Checks (Moderator)",
				"Completion of Rework Checks (Producer)",
				"Update Participant Data",
				["Complete Review","Review Update","Producer Defects Update","Oversight Defects Update"]);
  }
		    
  if (reply == "Review Update")
  	UpdateOversightdB(doc);  
  if (reply == "Oversight Defects Update")
	oversight_defects(doc);
  if (reply == "Producer Defects Update")
	supplier_defects(doc);
  
  if (reply == "Pre-Distribution Checks (Producer)")
  	completenessChecks(doc,"PreDistribution");
  if (reply == "Review Meeting Checks (Moderator)")
  	completenessChecks(doc,"ReviewMeeting");
  if (reply == "Completion of Rework Checks (Producer)")
	completenessChecks(doc,"CompletionOfRework");
  if (reply == "Update Participant Data")
  {
  	UpdatePartData(doc);
  	UpdateChecklistData(doc);
  }  
}
/*******************************************************************************************************
**
**  Function:   UpdateOversightdB 
**
**  Purpose:    After review is complete the oversight related data will be entered into the oversight dB
**
********************************************************************************************************
*/
function UpdateOversightdB(doc)
{   
 var a = doc.getField("R.Ref_ID").valueAsString;	//Review ID	--Updated on 15-Jul-10
 var b = doc.getField("R.Project").value;		//ACM Project
 var c = doc.getField("R.SubProject").value;		//ACM Subproject
 var d = doc.getField("R.oversight");			//Oversight Eligible 
 var e = doc.getField("R.SupplierLocation").value;	//Supplier Location	
 var f = doc.getField("DT.Supplier").value;		//# of  supplier technical defects
 var g = doc.getField("DNT.Supplier").value;		//# of  supplier non-technical defects
 var h = doc.getField("DP.Supplier").value;		//# of  supplier process defects
 var i = doc.getField("DT.Total").value;		//# of Oversight Focal technical defects
 var j = doc.getField("DNT.Total").value;		//# of Oversight Focal non-technical defects
 var k = doc.getField("DP.Total").value;		//# of Oversight Focal process defects
 var l = doc.getField("R.DOLevel").value;   		//DO-178 Level
 var m = doc.getField("WP.Artifacts").value;		//Work Product Type(s)
 var n = doc.getField("E3.Name").value;			//Oversight Reviewer Name
 var o = doc.getField("E3.Sign");			//Date Oversight Reviewer Stamp done
 var p = doc.getField("E3.Time").value;			//Oversight Time Spent
 var q = doc.getField("R.Lifecycle").value;		//Life Cycle
 var s = doc.getField("R.Farea").value;		        //Functional Area
 var t = doc.getField("E1.Time").value;			//Review Time Spent
 var u = doc.getField("R.Load").value;		        //Load Info
 var v = doc.getField("E1.Name").value;			//Producer Name
 var w = doc.getField("M.Number").value;		//# Meeting Participants
 var x = doc.getField("M.Duration").value;		//Meeting Duration
 var y = doc.getField("R.Rework").value;		//Rework effort
 var z = doc.getField("R.CloseEffort").value;		//Closure effort
 var z1 = doc.getField("R.CompleteDate");		//Date Complete
 //var y1 = doc.getField("R.Support").valueAsString;	//Supporting Material(s)/Comments 		//Removed 15Nov10 - We do not need to collect the comments. The comments field in the oversight dB will be just for oversight related comments. 
 var t1 = doc.getField("E2.Name").value;		//Moderator Name
 var s1 = doc.getField("E1.Sign");			//ReviewTime 
 var p1 = doc.getField("R.ArtifactProduced").value;	//Where Produced 
 var modchk = doc.getField("R.modChk");			//Moderator closure 
 var ind = doc.getField("R.OSdBstatus");		//OversightdB update indication
 var dBq = doc.getField("QuerydB");			//Run Query button
 
 //Flags to capture oversight related fields value for dB update
 var o1,r1,w1,v1,u1 = 0,chkflg = 0,pflg = 0;
 //Default NULL vars
 var reviewtimestamp = 0,oversighttimestamp = 0,closedatestamp = 0,packassigneeid = 0,indassigneeid = 0,osassigneeid = 0;
 var oversightstatus = 0,dBloadid = 0,asuppliersid = 0,afareaid = 0,lifecycleid = 0;			
 var tmp,tmp1,tmp2,tmp3 = 0,reviewincomplete = 0,us_flg = 0, os_notdone = 0, done_flg1 = 0, done_flg2 = 0, not_os = 0, chk_osflg = 0;	

 //Note: Try-catch block is used for every query to avoid failure to update dB when the coversheet fields are corrupted, user needs to send mail to update dB manually 
 
 //For Insert query to work properly when oversight eligible is unchecked
 if(d.isBoxChecked(0))
 {
   chkflg = 0;
 }
 else
 {
   chkflg = 1;  
   p = 0;						//overviewer time set to 0 if oversight eligible checkbox is unchecked.
 }
 
  //Done for display purpose of Names/Users
  tmp = v;
  tmp1 = t1;
  tmp2 = n;
  
 //OversightStatus 
 if(chkflg == 1)
 {
    w1 = "PENDING";
    oversightstatus = null;
    reviewincomplete = 1;
 }
 else
 {
    if(o.isBoxChecked(0))
    {
	w1 = "DONE";
	v1 = true;
    }
    else
    {
	w1 = "PENDING";
	v1 = false;
    }
 } 

   //To extract the date and time from the Producer signature stamp - ReviewTime 
   doc.syncAnnotScan();
   var annots = doc.getAnnots({nPage:0 });
   try
   {
	//Checkbox against the producer signature
	if(s1.isBoxChecked(0))
	{	
		pflg = 1;
		s1.checkThisBox(0,true);		
		for (var acnt = 0; acnt < annots.length; acnt++)
		{
			if (annots[acnt].author == doc.getField("E1.Name").value) 		
				s1 = annots[acnt].name;
		}
		s1 = s1.replace(/Reviewer Stamp -/,"");
		
		//If ReviewTime is not in the format yyyy-mm-dd HH:MM:ss error message     ----need to chk if auto correct possible
		if (s1.length != 20)
		{				
			app.alert("Producer signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB");
			s1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
			reviewtimestamp = null;						//To Update NULL in dB	
			reviewincomplete = 1;
		}	
	}
	//When Producer sign is not available during Review update current date and time is used to update
	else
	{
		s1.checkThisBox(0,false);
		s1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());
		reviewtimestamp = null;							//To Update NULL in dB	
		reviewincomplete = 1;
	}
   }  
   catch(e)
   {
	//If ReviewTime is not in the format yyyy-mm-dd HH:MM:ss error message     ----need to chk if auto correct possible
   	app.alert("Producer signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB");
	s1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());				//To avoid query fail
	reviewtimestamp = null;								//To Update NULL in dB	
	reviewincomplete = 1;
   }

    //To get the date and time from Oversight timestamp - OverviewTime
    doc.syncAnnotScan();
    var annots = doc.getAnnots({nPage:0 });    
    try
    {
	//Check if oversight eligible checkbox is checked    
	if(chkflg == 1)							//When oversight eligible checkbox is unchecked
	{
    		o1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());	//to avoid query from failing provide current time to query2 variable	
		oversighttimestamp = null;				//To Update NULL in dB	
		reviewincomplete = 1;
		not_os = 1;
		
	}
	//If oversight is done then get date and time from row 3 sign timestamp
	else
	{
	   //Oversight eligible is checked but Oversight reviewer name is entered chk timestamp
	   if (doc.getField("E3.Name").value.length >=1)
	   {
		if(v1 == true)							//v1 = flag, used as the checkbox value gets modified in many places.
		{	
			for (var acnt = 0; acnt < annots.length; acnt++)
			{   
				if (annots[acnt].author == doc.getField("E3.Name").value)
					o1 = annots[acnt].name;	 
			}
			o1 = o1.replace(/Reviewer Stamp -/,"");	
	
			//If OverviewTime is not in the format yyyy-mm-dd HH:MM:ss error message     ----need to chk if auto correct possible
			if (o1.length != 20)
			{	
				app.alert("Oversight signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB");
				o1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
				oversighttimestamp = null;					//To Update NULL in dB	
				reviewincomplete = 1;
			}
		}
		//oversight eligible checked and oversight timestamp not done
		else
		{
			o1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
			oversighttimestamp = null;					//To Update NULL in dB		
			reviewincomplete = 1;
			os_notdone = 1;
			chk_osflg = 1;
		}
           }
	   //Oversight eligible is checked but Oversight reviewer name is not entered no timestamp chk required
	   else
	   {
	   	o1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
		oversighttimestamp = null;
		reviewincomplete = 0;
		chk_osflg = 1;
	   }
	}
    }
    catch(e)
    {	
	o1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());				//To avoid query fail
	oversighttimestamp = null;							//To Update NULL in dB	
	if (doc.getField("E3.Name").value.length >=1)
	{
		app.alert("Oversight signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB");
		reviewincomplete = 1;
	}
	else
	{
		reviewincomplete = 0;
		chk_osflg = 1;
	}
    }
    
  //Close Date
   doc.syncAnnotScan();
   var annots = doc.getAnnots({nPage:0 });  						//look only on first page
   try
   {
	if(modchk.isBoxChecked(0))
	{
		modchk.checkThisBox(0,true);
		for (var i = 0; i < annots.length; i++) 
		{
			if (annots[i].author == "Moderator Stamp") 
				z1 = annots[i].name;	 			
		}
		z1 = z1.replace(/Mod Stamp -/,"");	

		if (z1.length != 20)
		{	
			app.alert("Moderator Closure signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB for Date Complete");
			z1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
			closedatestamp = null;						//To Update NULL in dB	
			reviewincomplete = 1;
		}
	}
	else
	{
		modchk.checkThisBox(0,false);
		z1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());
		closedatestamp = null;						//To Update NULL in dB	
		reviewincomplete = 1;
	}
   }
   catch(e)
   {
   	app.alert("Moderator Closure signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB for Date Complete");
	z1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
	closedatestamp = null;						//To Update NULL in dB	
	reviewincomplete = 1;
   }
 
 //Overviewertime, Producer, NumReviewers, Meeting Duration default value is set to 0
 if(p == "")
 {
    p = 0;
 }
 if(t == "")
 {
    t = 0;
 }
 if(w == "")
 {
    w = 1;
 }
 if(x == "")
 {
    x = 0;
 }
 if(y == "")			//Actually not permitted, required if write to dB is done before review complete
 {
    y = 0;
 }
  if(z == "")			//Actually not permitted, required if write to dB is done before review complete
 {
    z = 0;
 }
 
 //IndyReviewerTime: sum of all reviewer time read when role = reviewer from column Review time 
 var getrow = doc.getField("E6.Name"); 
 //When coversheet template pages are deleted, to get Review time from reviewers - need to have 2 checks (otherwise it hangs)
 if(getrow != null)
 {
	for (var rCnt = 1; rCnt <= numMaxReviewers; rCnt++) 
	{
		if(doc.getField("E"+rCnt+".Role").value == "Reviewer" || doc.getField("E"+rCnt+".Role").value == "Moderator")
		{
			if(doc.getField("E"+rCnt+".Time").value != "")
				u1 += doc.getField("E"+rCnt+".Time").value;
		}
	}
 }
 else
 {
	for (var rCnt = 1; rCnt <= 5; rCnt++) 
	{
		if(doc.getField("E"+rCnt+".Role").value == "Reviewer" || doc.getField("E"+rCnt+".Role").value == "Moderator")
		{
			if(doc.getField("E"+rCnt+".Time").value != "")
				u1 += doc.getField("E"+rCnt+".Time").value;
		}
	}
 } 
 
  //Normal Factor: sum of all Review Size under Work Products Under Review table 
 var p1getrow = doc.getField("F11.SCR"); 	//10 rows on page1
 var p3getrow = doc.getField("F41.SCR"); 	//upto count 40 on page3
 var p4getrow = doc.getField("F71.SCR"); 	//upto count 70 on page4
 var p5getrow = doc.getField("F101.SCR"); 	//upto count 100 on page5
 r1 = 0;
 //When coversheet template pages are deleted, to get Review Size - a breakup is done at each page
 if(p1getrow == null)
 {
 	for (var rCnt = 1; rCnt <= 10; rCnt++) 
	{
		if(doc.getField("F" + rCnt + ".SCR").value != "")
		{
			//when units is undefined the review size column is blank and for dB update the normal factor is taken as 1.
			if(doc.getField("F" + rCnt + ".Units").value != "undefined")
				r1 += doc.getField("F"+rCnt+".Size").value;
			else
				r1 += 1;
		}
	}
 }
 else if(p3getrow == null)
 {
 	for (var rCnt = 1; rCnt <= 40; rCnt++) 
	{
		if(doc.getField("F" + rCnt + ".SCR").value != "")
		{
			//when units is undefined the review size column is blank and for dB update the normal factor is taken as 1.
			if(doc.getField("F" + rCnt + ".Units").value != "undefined")
				r1 += doc.getField("F"+rCnt+".Size").value;
			else
				r1 += 1;
		}
	}
 }
 else if(p4getrow == null)
 {
  	for (var rCnt = 1; rCnt <= 70; rCnt++) 
	{
		if(doc.getField("F" + rCnt + ".SCR").value != "")
		{
			//when units is undefined the review size column is blank and for dB update the normal factor is taken as 1.
			if(doc.getField("F" + rCnt + ".Units").value != "undefined")
				r1 += doc.getField("F"+rCnt+".Size").value;
			else
				r1 += 1;
		}
	}
 }
 else if(p5getrow == null)
 {
  	for (var rCnt = 1; rCnt <= 100; rCnt++) 
	{
		if(doc.getField("F" + rCnt + ".SCR").value != "")
		{
			//when units is undefined the review size column is blank and for dB update the normal factor is taken as 1.
			if(doc.getField("F" + rCnt + ".Units").value != "undefined")
				r1 += doc.getField("F"+rCnt+".Size").value;
			else
				r1 += 1;
		}
	}
 }
 else
 {
	for (var rCnt = 1; rCnt <= numFormFiles; rCnt++) 
	{
		if(doc.getField("F" + rCnt + ".SCR").value != "")
		{
			//when units is undefined the review size column is blank and for dB update the normal factor is taken as 1.
			if(doc.getField("F" + rCnt + ".Units").value != "undefined")
				r1 += doc.getField("F"+rCnt+".Size").value;
			else
				r1 += 1;
		}
	}
 }

 //Variables declaration for MySQL database
 var arrItem = new Array();
 var row,row1,nrow,nrow1,nrow2,nrow3,nrow4,nrow5,nrow6,nrow7,nrow8,nrow9,nrow10,nrow11,nrow12,row3,row4,re_row,re_row1,re_row2;
 var statement,statement1,statement2,nstatement,nstatement1,nstatement2,nstatement3,nstatement4;
 var nstatement5,nstatement6,nstatement7,nstatement8,nstatement9,nstatement10,statement3,statement4;
 var nstatement11,nstatement12,statement5,re_statement,re_statement1,re_statement2,u_statement,m_statement,os_statement;
 var query,query1,query2,nquery,nquery1,nquery2,nquery3,nquery4,nquery5,nquery6,u_query,m_query,os_query;
 var nquery7,nquery8,nquery9,nquery10,nquery11,nquery12,query3,query4,re_query,re_query1,re_query2; 
 var cnt = 0,ncnt = 0,ncnt1 = 0,cnt3,pass = 0;
 var progid,projid,loadid,supid,asupid,faid,afaid,lcid,prodid,osid,review_projID;
 var nextconcatanate,val_c,uname_p,val_v,IDflag = 0,IDflag1 = 0,IDflag2 = 0,val_t1,uname_m,uname_os,val_n,fupflg = 0;
 var wpt_query2,statement_wpt2,wpt_row2,wpt_query1,statement_wpt1,wpt_row1,wpt_id;
 var wptcnt,wpt_query3,statement_wpt3,wpt_row3,rev_cnt,wpt_query,statement_wpt;
 
 try
 {                                               
    var conn   = ADBC.newConnection("OversightdB");    
    statement  = conn.newStatement();
    statement1 = conn.newStatement();
    statement2 = conn.newStatement();
    statement3 = conn.newStatement();
    statement4 = conn.newStatement();
    
    nstatement  = conn.newStatement();
    nstatement1 = conn.newStatement();
    nstatement2 = conn.newStatement();
    nstatement3 = conn.newStatement();
    nstatement4 = conn.newStatement();
    nstatement5 = conn.newStatement();   
    nstatement6 = conn.newStatement();
    nstatement7 = conn.newStatement();
    nstatement8 = conn.newStatement();
    nstatement9 = conn.newStatement();    
    nstatement10 = conn.newStatement();    
    nstatement11 = conn.newStatement();   
    nstatement12 = conn.newStatement(); 
    
    re_statement  = conn.newStatement(); 
    re_statement1 = conn.newStatement(); 
    re_statement2 = conn.newStatement(); 
    
    u_statement = conn.newStatement();
    m_statement = conn.newStatement();
    os_statement = conn.newStatement();  
    
    statement_wpt = conn.newStatement();
    statement_wpt2 = conn.newStatement();  
    statement_wpt1 = conn.newStatement();  
    statement_wpt3 = conn.newStatement(); 

    var query4 = "SELECT ProjectID from reviews where ReviewName = \'"+a+"\';" ;

   //Get number of rows in database
   //reviews table
   query = "SELECT count(*) as count FROM reviews;";
   statement.execute(query);    
   statement.nextRow();
   row1 = statement.getRow();
   cnt = row1.count.value;			//gives the no of rows with data in the database
   
   //Check for ProgramID in programs table - Programs.Name = ACMProject
   try
   {
	nquery1 = "SELECT ID FROM programs WHERE Name = \'"+b+"\';";
	nstatement1.execute(nquery1);    
	nstatement1.nextRow();
	nrow1 = nstatement1.getRow();
	progid = nrow1.ID.value;
	b = progid;
   }
   catch(e)
   {
	app.alert("ACM Project not added in dB. Please send an email to DLFMSeReview@honeywell.com");
	//15Nov10 Update/Change
	//b = 1;							//arbitrarily set to 1 to avoid insert query from failing , assumption this will never happen
	tmp3 = 1;
   }
   
   //Check for ProjectID in projects table - Projects.Name = ACMSubProject
   try
   {
	nquery3 = "SELECT ID FROM projects WHERE Name = \'"+c+"\';";
	nstatement3.execute(nquery3);     
	nstatement3.nextRow();
	nrow3 = nstatement3.getRow();
	projid = nrow3.ID.value;
	c = projid; 
   }
   catch(e)
   {
	app.alert("ACM SubProject not added in dB. Please send an email to DLFMSeReview@honeywell.com");
	//15Nov10 Update/Change
	//c = 1;								//arbitrarily set to 1 to avoid insert query from failing,  assumption this will never happen
	tmp3 = 1;
   }
   
   //Concatenating a and c values as review_projectID, To check duplicate entries to dB.
   review_projectID = a + "" + c;

   //Check for LoadID in Loads table - Projects.Name.ID = ACMSubProject and Load.Name = LoadInfo
   try
   {
	nquery4 = "SELECT ID FROM loads WHERE Name = \'"+u+"\' AND ProjectID = \'"+c+"\';";
	nstatement4.execute(nquery4); 
	nstatement4.nextRow();
	nrow4 = nstatement4.getRow();
	loadid = nrow4.ID.value;  
	u = loadid;  
   }
   catch(e)
   {
      app.alert("Load not added in dB. Please send an email to DLFMSeReview@honeywell.com");      
      u = 7;								//arbitrarily set to 7 to avoid insert query from failing 
      dBloadid = null;							//To Update NULL in dB	
      reviewincomplete = 1;
   }  

   //OS Eligible checkbox unchecked - Supplier Location default set.
   if(chkflg == 1)
   {
	if(e == "Select Employer")		//Updated 15Jul10
		e = "Honeywell - Phoenix";	//some valid value to enable insert query to work when OE is unchecked
   }
   else
   {
	if(e == "Select Employer")		//Updated 15Jul10
		e = "Honeywell - Phoenix";	//when oversight is checked and producer employer is not selected
   }
   
   //Check for ID in suppliers table - Name = SupplierLocation
   try
   {
	nquery5 = "SELECT ID FROM suppliers WHERE AeroName = \'"+e+"\';";
	nstatement5.execute(nquery5); 
	nstatement5.nextRow();
	nrow5 = nstatement5.getRow();
	supid = nrow5.ID.value;  
	e = supid;
    }
    catch(e)
    {
	e = 1;							//arbitrarily set to 1 to avoid insert query from failing 
    }

   //Check for ID in asuppliers table - ProjectID = projid AND SupplierID = supid
   try
   {
	nquery6 = "SELECT ID FROM asuppliers WHERE ProjectID = \'"+c+"\' AND SupplierID = \'"+e+"\';";
	nstatement6.execute(nquery6);  
	nstatement6.nextRow();
	nrow6 = nstatement6.getRow();
	asupid = nrow6.ID.value;  
	e = asupid;
   }
   catch(e)
   {
	app.alert("Producer Employer selected is not added in dB. Please send an email to DLFMSeReview@honeywell.com");
	e = 1;							//arbitrarily set to 1 to avoid insert query from failing 
	asuppliersid = null;					//To Update NULL in dB	
	reviewincomplete = 1;
   }

   //Check for ID in farea table - Name = FunctionalArea
   try
   {
	nquery7 = "SELECT ID FROM farea WHERE Name = \'"+s+"\';";
	nstatement7.execute(nquery7); 
	nstatement7.nextRow();
	nrow7 = nstatement7.getRow();
	faid = nrow7.ID.value;  
	s = faid;
   }
   catch(e)
   {
	s = 1;							//arbitrarily set to 1 to avoid insert query from failing 
   }

   //Check for ID in afareas table - ProgramID = progid AND fareaID = faid
   try
   {
	nquery8 = "SELECT ID FROM afareas WHERE ProgramID = \'"+b+"\' AND FareaID = \'"+s+"\';";
	nstatement8.execute(nquery8);  
	nstatement8.nextRow();
	nrow8 = nstatement8.getRow();
	afaid = nrow8.ID.value;  
	s = afaid;
   }
   catch(e)
   {
   	app.alert("FAREA is not added in dB. Please send an email to DLFMSeReview@honeywell.com");
	s = 1;							//arbitrarily set to 1 to avoid insert query from failing 
	afareaid = null;					//To Update NULL in dB	
	reviewincomplete = 1;
   }

   //Check for ID in lifecycles table - Name = Lifecycle
   try
   {
	nquery9 = "SELECT ID FROM lifecycles WHERE Name = \'"+q+"\';";
	nstatement9.execute(nquery9);  
	nstatement9.nextRow();
	nrow9 = nstatement9.getRow();
	lcid = nrow9.ID.value;  
	q = lcid; 
   }
   catch(e)
   {
      	app.alert("Life Cycle is not added in dB. Please send an email to DLFMSeReview@honeywell.com");
	q = 10;							//arbitrarily set to 1 to avoid insert query from failing - changed to 10
	lifecycleid = null;					//To Update NULL in dB	
	reviewincomplete = 1;
   }


  //Modify the producer name read from coversheet to lookup in users table
   var pcommacnt = 0, prod_spa = 0;
   for (var pc = 0; pc < v.length; pc++)
   {
	if (v.charAt(pc) == ',')		//To check if names are in the format Lastname, Firstname.
	{
        	if (v.charAt(pc-1) == ' ')
		{
			v = v.replace(v.charAt(pc-1),"");
			pc--;
		}
		pcommacnt++;
	}
	else if (v.charAt(pc) == ' ')		// Check for space before and after comma and eliminate
	{
		if (v.charAt(pc-1) == ',')		
		{
			v = v.replace(v.charAt(pc),"");
			pc--;
		}
		else
		{
			prod_spa++;
			if (prod_spa > 1)			//Check for more than one space in between parts of the name
			{
				v = v.replace(v.charAt(pc),"");
				pc--;
				prod_spa--;					
			}
		}
	}
	else
	{
		prod_spa = 0;
	}
   }
   if(pcommacnt > 0)
   {
	var p_lspace = v.replace(/^\s+/g,"");			//Trim leading spaces
	var p_tspace = p_lspace.replace(/\s+$/g,"");		//Trim trailing spaces
	v = p_tspace;
   }
   else
   {	
	var lspace = v.replace(/^\s+/g,"");			//Trim leading spaces
	var tspace = lspace.replace(/\s+$/g,"");		//Trim trailing spaces
	var mspace = tspace.replace("  "," ");		//Trim extra spaces in middle of the string
	v = mspace.replace(/(.+)\s+([\w']+)$/, "$2,$1");	//This will handle user names having more than 2 parts to a name 
   }
   //Check for ID in users table - UserName = E1.Name
   try
   {   
	nquery10 = "SELECT ID FROM users WHERE UserName = \'"+v+"\';";
	nstatement10.execute(nquery10);
	nstatement10.nextRow();
	nrow10 = nstatement10.getRow();
	prodid = nrow10.ID.value;  
	v = prodid; 
	uname_p = 1;							//After Review Complete dB update allowed only for user names update. flg to chk this.
   }
   catch(e)
   {
      app.alert(tmp + " has not been registered as an User. Please log into http://go.honeywell.com/fms_oversight once to register your name");
      v = 1;								//arbitrarily set to 1 to avoid insert query from failing 
      packassigneeid = null;						//set to NULL in dB
      reviewincomplete = 1;
   }      
	
   //Modify the moderator name read from coversheet to lookup in users table
   var mcommacnt = 0, mod_spa = 0;
   for (var mc = 0; mc < t1.length; mc++)
   {
	if (t1.charAt(mc) == ',')		//To check if names are in the format Lastname, Firstname.
	{
		if (t1.charAt(mc-1) == ' ')
		{
			t1 = t1.replace(t1.charAt(mc-1),"");
			mc--;
		}
		mcommacnt++;
	}
	else if (t1.charAt(mc) == ' ')
	{
		if (t1.charAt(mc-1) == ',')		
		{
			t1 = t1.replace(t1.charAt(mc),"");
			mc--;
		}
		else
		{
			mod_spa++;
			if (mod_spa > 1)
			{
				t1 = t1.replace(t1.charAt(mc),"");
				mc--;
				mod_spa--;					
			}
		}
	}
	else
	{
		mod_spa = 0;
	}
   }
   if(mcommacnt > 0)
   {
	var m_lspace = t1.replace(/^\s+/g,"");			//Trim leading spaces
	var m_tspace = m_lspace.replace(/\s+$/g,"");		//Trim trailing spaces
	t1 = m_tspace;
   }
   else
   {	
 	var lspace = t1.replace(/^\s+/g,"");			//Trim leading spaces
	var tspace = lspace.replace(/\s+$/g,"");		//Trim trailing spaces
	var mspace = tspace.replace("  "," ");			//Trim extra spaces in middle of the string
	t1 = mspace.replace(/(.+)\s+([\w']+)$/, "$2,$1");	//This will handle user names having more than 2 parts to a name 
   }
   //Check for ID in users table - UserName = E2.Name
   try
   { 
	nquery11 = "SELECT ID FROM users WHERE UserName = \'"+t1+"\';";
	nstatement11.execute(nquery11);
	nstatement11.nextRow();
	nrow11 = nstatement11.getRow();
	modid = nrow11.ID.value;  
	t1 = modid; 
	uname_m = 1;							//After Review Complete dB update allowed only for user names update. flg to chk this.
   }
   catch(e)
   {
      app.alert(tmp1 + " has not been registered as an User. Please log into http://go.honeywell.com/fms_oversight once to register your name");
      t1 = 1;								//arbitrarily set to 1 to avoid insert query from failing 
      indassigneeid = null;						//set to NULL in dB
      reviewincomplete = 1;
   } 

  //OSAssigneeID valid only when Oversight Eligible is checked, otherwise its NULL.
  //OSAssigneeID is 18 when Oversight Eligible is unchecked
   if(chkflg == 1)
   {
      n = 18;								//Oversight eligible unchecked and produced in US then OSAssignee ID is 18
      osassigneeid = null; 						//set to NULL in dB
      reviewincomplete = 1;
   }
   else
   {
     //Modify the oversight reviewer name read from coversheet to lookup in users table
    //When oversight eligible is checked and name is entered look up users table and fetch the OSAssigneeID
	if (doc.getField("E3.Name").value.length >=1)
	{
		var ocommacnt = 0, os_spa = 0;
		for (var oc = 0; oc < n.length; oc++)
		{
			if (n.charAt(oc) == ',')		//To check if names are in the format Lastname, Firstname.
			{
				if (n.charAt(oc-1) == ' ')
				{
					n = n.replace(n.charAt(oc-1),"");
					oc--;
				}
				ocommacnt++;
			}
			else if (n.charAt(oc) == ' ')
			{
				if (n.charAt(oc-1) == ',')		
				{
					n = n.replace(n.charAt(oc),"");
					oc--;
				}
				else
				{
					os_spa++;
					if (os_spa > 1)
					{
						n = n.replace(n.charAt(oc),"");
						oc--;
						os_spa--;					
					}
				}
			
			}
			else
			{
				os_spa = 0;
			}
		}
		if(ocommacnt > 0)
		{
			var o_lspace = n.replace(/^\s+/g,"");			//Trim leading spaces
			var o_tspace = o_lspace.replace(/\s+$/g,"");		//Trim trailing spaces
			n = m_tspace;
		}
		else
		{	
			var lspace = n.replace(/^\s+/g,"");			//Trim leading spaces
			var tspace = lspace.replace(/\s+$/g,"");		//Trim trailing spaces
			var mspace = tspace.replace("  "," ");		//Trim extra spaces in middle of the string
			n = mspace.replace(/(.+)\s+([\w']+)$/, "$2,$1");	//This will handle user names having more than 2 parts to a name 
		}	 

		//Check for ID in users table - UserName = E3.Name
		try
		{    
			nquery12 = "SELECT ID FROM users WHERE UserName = \'"+n+"\' AND IsOverseer = 'Y';";
			nstatement12.execute(nquery12);
			nstatement12.nextRow();
			nrow12 = nstatement12.getRow();
			osid = nrow12.ID.value;  
			n = osid; 
			uname_os = 1;							//After Review Complete dB update allowed only for user names update. flg to chk this.
		}
		catch(e)
		{
			app.alert(tmp2 + " has not been registered as an Overseer. Please log into http://go.honeywell.com/fms_oversight once to register your name");
			n = 17;								//arbitrarily set to 1 to avoid insert query from failing 
			osassigneeid = null; 						//set to NULL in dB
			reviewincomplete = 1;
			os_notdone = 1;
		}  
	}
	//When oversight eligible is checked and name is not provided OSAssigneeID is 17.
	else	
	{
		n = 17;
		reviewincomplete = 0;		
	}
   } 

   //ID column of reviews table - Auto increment column does not work?
   if(cnt != 0)
   {
   query3 = "SELECT * FROM reviews WHERE  ID = (SELECT MAX(ID) FROM reviews)"; 
   statement3.execute(query3);   
   statement3.nextRow();
   row3 = statement3.getRow();
   cnt3 = row3.ID.value;
   cnt3 = cnt3 + 1;						//Points to the next available row where data will be written to OversightdB.
  }
   //Check for ReviewName if exists in database
   query1 = "SELECT ReviewName FROM reviews WHERE ReviewName <> \'"+a+"\';"; 
   statement1.execute(query1); 
   
   //OSReady
   //If Oversight Eligible is checked, 
   if(d.isBoxChecked(0))					//Oversight eligible checked
   {
	//d = d.value;
	//Where Produced HTS/Perf SW/RTC
	if(p1 == "HTS - Bangalore" || p1 == "HTS - Beijing" || p1 == "HTS - Madurai" || p1 == "HTS - Shanghai" || p1 == "Perf Sw" || p1 == "RTC")		//Where produced selected
	{     
	   if (doc.getField("E3.Name").value.length >=1)
	   {
		if(v1 == true)					//E3.sign checkbox selected
		{
			d = 'YES';				//OSReady = YES
			w1 = 'DONE';				//Oversightstatus = DONE
		}
		else
		{
			if(pflg == 1)				//E1.Sign checkbox selected, Producer flag set indicated review done
			{
				d = 'YES';			//OSReady = YES
				if(tmp2 == '')			//Oversight Name = not named/empty
					w1 = 'NO';		//Oversightstatus = NO
				else
					w1 = 'ASSIGNED';	//Oversightstatus = ASSIGNED
			}
			else
			{
				d = 'NO';			//OSReady = NO
				w1 = 'PENDING';			//Oversightstatus = PENDING
			}
		}
	   }
	   else
	   {
	   	d = 'YES';			//OSReady = YES
		w1 = 'NO';			//Oversightstatus = NO
	   }
		
	}
   }
   //If Oversight Eligible NOT checked AND if PRODUCED on review form is NOT in the US, OSReady = NO
   else								//Oversight Eligible is unchecked
   {
	if(p1 == "Phx" || p1 == "Olathe" || p1 == "Redmond" || p1 == "Toulouse")		//Where produced selected at Honeywell US
	{ 
	   d = 'NO';						//OSReady = NO
	   w1 = 'NO';						//Oversightstatus = NO
	   us_flg = 1;
	}
   
   //If Oversight Eligible NOT checked AND if PRODUCED on review form is in the US, do not enter review in the database
	/*else
	{	  
	   tmp3 = 1;
	}*/
   }

   //To make the first entry into empty database
   if(cnt == 0)
   {    
        cnt = cnt + 1;
	query2 = "INSERT INTO reviews VALUES (\'"+cnt+"\',\'"+s1+"\',\'"+a+"\',\'"+l+"\',\'"+d+"\',\'"+r1+"\',\'"+w1+"\',\'"+o1+"\',\'"+p+"\',\'"+u1+"\',\'"+t+"\','0',\'"+b+"\',\'"+c+"\',\'"+u+"\',\'"+v+"\',\'"+n+"\',\'"+t1+"\',\'"+e+"\',\'"+q+"\',\'"+w+"\',\'"+x+"\',\'"+y+"\',\'"+z+"\',\'"+z1+"\',\'"+s+"\','','3')"; 		
	statement2.execute(query2);
   }

   for (var icnt = 0; icnt < cnt; icnt++)
   { 
       if(icnt != cnt - 1 )
       {
	statement1.nextRow();
	row = statement1.getRow();
	arrItem[icnt] = row.ReviewName.value;	
	}	
	break;
   }    

   statement4.execute(query4);

   try{
	for (var icnt1 = 0; icnt1 < cnt; icnt1++)
	{
		if(icnt1 != cnt - 1 )
		{
		statement4.nextRow();
		row4 = statement4.getRow();
		val_c = row4.ProjectID.value;
		}
		break;

	}
     }catch(e){} 

 	//Compare the Review form values with database data for duplicate entry of review information.
	nextconcatanate = a + "" + val_c;

	//Check if user name is entered in database for the selected ReviewID and ProjectID
	re_query = "SELECT PackAssigneeID FROM reviews WHERE PackAssigneeID = \'"+v+"\' AND ReviewName = \'"+a+"\' AND ProjectID = \'"+val_c+"\'"; 
	re_statement.execute(re_query);
	try
	{
		for (var icnt2 = 0; icnt2 < cnt; icnt2++)
		{
			if(icnt2 != cnt - 1 )
			{
				re_statement.nextRow();
				re_row = re_statement.getRow();
				val_v = re_row.PackAssigneeID.value;
			}
			break;
		}		
	}
	catch(e){} 
	
	re_query1 = "SELECT IndAssigneeID FROM reviews WHERE IndAssigneeID = \'"+t1+"\' AND ReviewName = \'"+a+"\' AND ProjectID = \'"+val_c+"\'"; 
	re_statement1.execute(re_query1);
	try
	{
		for (var icnt3 = 0; icnt3 < cnt; icnt3++)
		{
			if(icnt3 != cnt - 1 )
			{
				re_statement1.nextRow();
				re_row1 = re_statement1.getRow();
				val_t1 = re_row1.IndAssigneeID.value;
			}
			break;
		}           	
	}
	catch(e){} 
	
	re_query2 = "SELECT OSAssigneeID FROM reviews WHERE OSAssigneeID = \'"+n+"\' AND ReviewName = \'"+a+"\' AND ProjectID = \'"+val_c+"\'"; 
	re_statement2.execute(re_query2);
	try
	{
		for (var icnt4 = 0; icnt4 < cnt; icnt4++)
		{
			if(icnt4 != cnt - 1 )
			{
				re_statement2.nextRow();
				re_row2 = re_statement2.getRow();
				val_n = re_row2.OSAssigneeID.value;
			}
			break;
		}		
	}
	catch(e){} 

	//Update database not allowed when ReviewID and ProjectID are found in oversight database.
	if(nextconcatanate == review_projectID)
	{
		//Further update allowed when OSReady is YES, Oversight Status is ASSIGNED, oversight review time and signature done oversight status changed to DONE and oversight information updated to dB.
		if(uname_os == 1 && chk_osflg != 1)					
		{
			query2 = "UPDATE reviews SET OversightStatus = 'DONE' WHERE ReviewName = \'"+a+"\'"; 		
			statement2.execute(query2);
			query2 = "UPDATE reviews SET OverviewerTime = \'"+p+"\' WHERE ReviewName = \'"+a+"\'"; 		
			statement2.execute(query2);
			query2 = "UPDATE reviews SET OverviewTime = \'"+o1+"\' WHERE ReviewName = \'"+a+"\'"; 		
			statement2.execute(query2);
			pass = 1;
			reviewincomplete = 0;
		}

		//Check if database value and review form value for user name is same and set flag
		if(val_v == v)
		{
			IDflag = 1;
		}
		if(val_t1 == t1)
		{
			IDflag1 = 1;
		}
		if(val_n == n)
		{
			IDflag2 = 1;
		}
		//Further review update allowed 
		else
		{	
			//Further review update not allowed if where produced is in the US.
			if(us_flg == 1)
			{
			}
			//Further review update allowed if where produced is HTS/Perf SW/RTC.
			else
			{
				fupflg = 1;		//To allow further update
			}
		}

		//When review information is available in the database and user name is to be updated.
		if(uname_p == 1 && IDflag != 1)
		{			
			u_query = "UPDATE reviews SET PackAssigneeID = \'"+v+"\' WHERE ReviewName = \'"+a+"\'"; 		
			u_statement.execute(u_query);			
			app.alert("Review update done for user name " + tmp , 1);
			pass = 1;
		}
		if(uname_m == 1 && IDflag1 != 1)
		{			
			m_query = "UPDATE reviews SET IndAssigneeID = \'"+t1+"\' WHERE ReviewName = \'"+a+"\'"; 		
			m_statement.execute(m_query);			
			app.alert("Review update done for user name " + tmp1 , 1);	
			pass = 1;
		}
		if(uname_os == 1 && IDflag2 != 1)
		{			
			os_query = "UPDATE reviews SET OSAssigneeID = \'"+n+"\' WHERE ReviewName = \'"+a+"\'"; 		
			os_statement.execute(os_query);	
			app.alert("Review update done for user name " + tmp2 , 1);	
			//Oversight review time greater than zero update the dB
			if(p != 0)
			{
				os_query = "UPDATE reviews SET OverviewerTime = \'"+p+"\' WHERE ReviewName = \'"+a+"\'"; 		
				os_statement.execute(os_query);	
				done_flg1 = 1;
			}
			//Oversight signature timestamp done update the dB
			if (oversighttimestamp != null)
			{
				os_query = "UPDATE reviews SET OverviewTime = \'"+o1+"\' WHERE ReviewName = \'"+a+"\'"; 		
				os_statement.execute(os_query);
				done_flg2 = 1;
			}
			//All oversight information in dB set oversight status to DONE else NO
			if(done_flg1 == 1 && done_flg2 == 1)
			{
				os_query = "UPDATE reviews SET OversightStatus = 'DONE' WHERE ReviewName = \'"+a+"\'"; 		
				os_statement.execute(os_query);
			}
			else
			{
				os_query = "UPDATE reviews SET OversightStatus = 'NO' WHERE ReviewName = \'"+a+"\'"; 		
				os_statement.execute(os_query);
			}
			//If producer and moderator done update in dB
			if(pflg == 1)
			{
				os_query = "UPDATE reviews SET OSReady = 'YES' WHERE ReviewName = \'"+a+"\'"; 		
				os_statement.execute(os_query);
				if(reviewtimestamp != null)
				{				
					os_query = "UPDATE reviews SET ReviewTime = \'"+s1+"\' WHERE ReviewName = \'"+a+"\'"; 		
					os_statement.execute(os_query);
				}
				if(closedatestamp != null)
				{
					os_query = "UPDATE reviews SET CloseDate = \'"+z1+"\' WHERE ReviewName = \'"+a+"\'"; 		
					os_statement.execute(os_query);
				}				
			}
			pass = 1;			
		}
		//Update the dB allowed 
		if (fupflg == 1)
		{
			//Oversight eligible checked and oversight time stamp is done status is DONE else NO.
			if(os_notdone == 0)
			{
				query2 = "UPDATE reviews SET OversightStatus = 'DONE' WHERE ReviewName = \'"+a+"\'"; 		
				statement2.execute(query2);
			}
			else
			{
				query2 = "UPDATE reviews SET OSReady = 'YES' WHERE ReviewName = \'"+a+"\'"; 		
				statement2.execute(query2);
				if(reviewtimestamp != null)
				{				
					query2 = "UPDATE reviews SET ReviewTime = \'"+s1+"\' WHERE ReviewName = \'"+a+"\'"; 		
					statement2.execute(query2);
				}
				if(closedatestamp != null)
				{
					query2 = "UPDATE reviews SET CloseDate = \'"+z1+"\' WHERE ReviewName = \'"+a+"\'"; 		
					statement2.execute(query2);
				}
				query2 = "UPDATE reviews SET OversightStatus = 'NO' WHERE ReviewName = \'"+a+"\'"; 		
				statement2.execute(query2);
			}
			pass = 1;
			reviewincomplete = 0;
		}
		if(pass == 1)
		{
			app.alert("Review update done", 1);
			if (reviewincomplete == 1)
			{
				ind.display = display.visible;
				ind.fillColor = color.red;
				ind.strokeColor = color.red;
				ind.value = "Review Update Incomplete";
				dBq.display = display.visible;	
				app.alert("Review Update Incomplete. Select File > Save to save the review packet before exiting");
			}
			else
			{
				ind.display = display.visible;
				ind.fillColor = color.green;
				ind.strokeColor = color.green;
				ind.value = "Review Update Complete";
				dBq.display = display.hidden;
				app.alert("Review Update Complete. Select File > Save to save the review packet before exiting",1);
			}
		}		
		else
			app.alert("ReviewName " + a + " Exists in database");	
	}
	else
	{	
		query2 = "INSERT INTO reviews VALUES (\'"+cnt3+"\',\'"+s1+"\',\'"+a+"\',\'"+l+"\',\'"+d+"\',\'"+r1+"\',\'"+w1+"\',\'"+o1+"\',\'"+p+"\',\'"+u1+"\',\'"+t+"\','0',\'"+b+"\',\'"+c+"\',\'"+u+"\',\'"+v+"\',\'"+n+"\',\'"+t1+"\',\'"+e+"\',\'"+q+"\',\'"+w+"\',\'"+x+"\',\'"+y+"\',\'"+z+"\',\'"+z1+"\',\'"+s+"\','','3')"; 		
		statement2.execute(query2);

		if(oversighttimestamp == null)				//Unable to pass null to overview time
		{   
			query2 = "UPDATE reviews SET OverviewTime = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
			if(us_flg == 1 || chk_osflg == 1)
				reviewincomplete = 0;
			if(uname_os == 1 && chk_osflg == 1)
				reviewincomplete = 1;
		}
		if(osassigneeid == null)				//Unable to pass null to OSAssigneeID
		{   
			if(p1 == "Phx" || p1 == "Olathe" || p1 == "Redmond" || p1 == "Toulouse")
			{
				query2 = "UPDATE reviews SET OSAssigneeID = '18' WHERE ID = \'"+cnt3+"\'"; 		
				statement2.execute(query2);
			}
			else
			{
				query2 = "UPDATE reviews SET OSAssigneeID = '17' WHERE ID = \'"+cnt3+"\'"; 		
				statement2.execute(query2);
			}			
			reviewincomplete = 1;
			if(us_flg == 1 || chk_osflg == 1)
				reviewincomplete = 0;
			if(uname_os == 1 && chk_osflg == 1)
				reviewincomplete = 1;
		}
		if(reviewtimestamp == null)				//Unable to pass null to reviewtime column
		{   
			query2 = "UPDATE reviews SET ReviewTime = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}
		if(closedatestamp == null)				//Unable to pass null to closedate column
		{   
			query2 = "UPDATE reviews SET CloseDate = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);	
			reviewincomplete = 1;
		}	    
		if(packassigneeid == null)				//Unable to pass null to packassigneeID
		{   
			query2 = "UPDATE reviews SET PackAssigneeID = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}	
		if(indassigneeid == null)				//Unable to pass null to IndAssigneeID
		{   
			query2 = "UPDATE reviews SET IndAssigneeID = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}	
		if(dBloadid == null)
		{
			query2 = "UPDATE reviews SET LoadID = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}
		if(asuppliersid == null)
		{
			query2 = "UPDATE reviews SET ASupplierID = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}
		if(afareaid == null)
		{
			query2 = "UPDATE reviews SET AFareaID = NULL WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}		
		if(lifecycleid == null)
		{
			query2 = "UPDATE reviews SET LifeCycleID = '10' WHERE ID = \'"+cnt3+"\'"; 		
			statement2.execute(query2);
			reviewincomplete = 1;
		}
		app.alert("Review update done", 1);

		//Added for the review_type and review_type_map tables in the dB.
		//Get number of rows in database table
		wpt_query1 = "SELECT count(*) as count FROM review_type_map;";
		statement_wpt1.execute(wpt_query1);    
		statement_wpt1.nextRow();
		wpt_row1 = statement_wpt1.getRow();
		wptcnt = wpt_row1.count.value;			//gives the no of rows with data in the database
			
		//ID column of review_type_map - Auto increment column does not work?
		if(wptcnt != 0)
		{
			wpt_query3 = "SELECT * FROM review_type_map WHERE  ID = (SELECT MAX(ID) FROM review_type_map)"; 
			statement_wpt3.execute(wpt_query3);   
			statement_wpt3.nextRow();
			wpt_row3 = statement_wpt3.getRow();
			rev_cnt = wpt_row3.ID.value;
			rev_cnt = rev_cnt + 1;						//Points to the next available row where data will be written to OversightdB.
		}
		//Get the ID for review_type from dB
		try
		{
			wpt_query2 = "SELECT ID FROM review_type WHERE review_type = \'"+m+"\';"; 				
			statement_wpt2.execute(wpt_query2);  	
			statement_wpt2.nextRow();
			wpt_row2 = statement_wpt2.getRow();
			wpt_id = wpt_row2.ID.value;	
		}		
		catch(e)
		{
			app.alert("No matching work product type in the database");
			wpt_id = 1;
		}

		//Insert record into review_type_map table of database
		if(wptcnt == 0)
		{    
			wptcnt = wptcnt + 1;
			wpt_query = "INSERT INTO review_type_map VALUES (\'"+wptcnt+"\',\'"+cnt3+"\',\'"+wpt_id+"\')"; 		
			statement_wpt.execute(wpt_query);
		}
		else
		{
			wpt_query = "INSERT INTO review_type_map VALUES (\'"+rev_cnt+"\',\'"+cnt3+"\',\'"+wpt_id+"\')"; 	
			statement_wpt.execute(wpt_query);
		}
		
		//An indication on eReview form for status of oversightdB update : Red indicates Incomplete update and Green indicates complete update this is done one time only.
		if (reviewincomplete == 1)
		{
			ind.display = display.visible;
			ind.fillColor = color.red;
			ind.strokeColor = color.red;
			ind.value = "Review Update Incomplete";
			dBq.display = display.visible;
			app.alert("Review Update Incomplete. Select File > Save to save the review packet before exiting");
		}
		else
		{
			ind.display = display.visible;
			ind.fillColor = color.green;
			ind.strokeColor = color.green;
			ind.value = "Review Update Complete";
			app.alert("Review Update Complete. Select File > Save to save the review packet before exiting",1);
		}
	}
 }                                            
 catch(e)
 {   
    if(tmp3 == 1)
    {
	app.alert("Cannot enter review information to database, Please send an email to DLFMSeReview@honeywell.com for more information");	//16Nov10 - Reviewupdate not allowed if ACM Project and Subproject are not found in dB.
    }
    else
    {
	ind.display = display.visible;
	ind.fillColor = color.red;
	ind.strokeColor = color.red;
	ind.value = "Review Update Incomplete";
	dBq.display = display.visible;
	app.alert("Review Update Incomplete. Select File > Save to save the review packet before exiting");
    }
 } 	 
}   
/*******************************************************************************************************
**
**  Function:   RunQuery 
**
**  Purpose:    Run a javascript code for Query button on the eReview form
**
********************************************************************************************************
*/
function RunQuery(doc)
{
	var a = doc.getField("R.Ref_ID").valueAsString;	//Review ID
	var ind = doc.getField("R.OSdBstatus");		//OversightdB update indication
	var dBq = doc.getField("QuerydB");		//Run Query button
	var modchk = doc.getField("R.modChk");		//Moderator closure 
	var z1 = doc.getField("R.CompleteDate");	//Date Complete
	var closedatestamp = 0;				//Flag for closedate
	
	//Variables for database connection and query
	var conn;
	var dBquery1,dBquery2,dBquery3;
	var dBstatement1,dBstatement2,dBstatement3;
	
	//Close Date
	doc.syncAnnotScan();
	var annots = doc.getAnnots({nPage:0 });  						//look only on first page
	try
	{
		if(modchk.isBoxChecked(0))
		{
			modchk.checkThisBox(0,true);
			for (var i = 0; i < annots.length; i++) 
			{
				if (annots[i].author == "Moderator Stamp") 
				z1 = annots[i].name;	 			
			}
			z1 = z1.replace(/Mod Stamp -/,"");	

			if (z1.length != 20)
			{	
				app.alert("Moderator Closure signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB for Date Complete");
				z1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
				closedatestamp = null;						//To Update NULL in dB					
			}
		}
		else
		{
			modchk.checkThisBox(0,false);
			z1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());
			closedatestamp = null;						//To Update NULL in dB				
		}
	}
	catch(e)
	{
		app.alert("Moderator Closure signature time stamp is not in the format yyyy-mm-dd HH:MM:ss, Send mail to DLFMSeReview@honeywell.com to manually update the dB for Date Complete");
		z1 = util.printd("yyyy-mm-dd HH:MM:ss", new Date());		//To avoid query fail
		closedatestamp = null;						//To Update NULL in dB			
	}

	//Oversight database connection
	conn = ADBC.newConnection("OversightdB");
	dBstatement1 = conn.newStatement();
	dBstatement2 = conn.newStatement();
	dBstatement3 = conn.newStatement();
	
	//ID from Reviews table for selected ReviewID
	dBquery1 = "SELECT ID from reviews where ReviewName = \'"+a+"\';" ;
	dBstatement1.execute(dBquery1);
	dBstatement1.nextRow();
	var dBrow1 = dBstatement1.getRow();
	var dBid = dBrow1.ID.value;
		
	//All review update done, review incomplete for missing moderature signature can be closed by using query to update dB with closedate and complete review.
	if(closedatestamp != null)
	{
		dBquery3 = "UPDATE reviews SET CloseDate = \'"+z1+"\' WHERE ReviewName = \'"+a+"\'"; 		
		dBstatement3.execute(dBquery3);
	}
	
	//Read values in  all columns of the reviews table and check for any value being null
	dBquery2 = "SELECT * from reviews where ID = \'"+dBid+"\';" ;
	dBstatement2.execute(dBquery2);
	dBstatement2.nextRow();
	var dBrevt = dBstatement2.getColumn("ReviewTime",1).value;
	var dBnof = dBstatement2.getColumn("NormalFactor").value;
	var dBost = dBstatement2.getColumn("OversightStatus").value;
	var dBovt = dBstatement2.getColumn("OverviewTime",1).value;
	var dBort = dBstatement2.getColumn("OverviewerTime").value;
	var dBirt = dBstatement2.getColumn("IndyReviewerTime").value;
	var dBrwt = dBstatement2.getColumn("ReviewerTime").value;
	var dBprg = dBstatement2.getColumn("ProgramID").value;
	var dBprj = dBstatement2.getColumn("ProjectID").value;
	var dBlod = dBstatement2.getColumn("LoadID").value;
	var dBpaa = dBstatement2.getColumn("PackAssigneeID").value;
	var dBosa = dBstatement2.getColumn("OSAssigneeID").value;
	var dBina = dBstatement2.getColumn("IndAssigneeID").value;
	var dBasp = dBstatement2.getColumn("ASupplierID").value;
	var dBlic = dBstatement2.getColumn("LifeCycleID").value;
	var dBnur = dBstatement2.getColumn("NumReviewers").value;
	var dBmed = dBstatement2.getColumn("MeetingDuration").value;
	var dBrwe = dBstatement2.getColumn("ReworkEffort").value;
	var dBcle = dBstatement2.getColumn("ClosureEffort").value;
	var dBcld = dBstatement2.getColumn("CloseDate",1).value;
	var dBafa = dBstatement2.getColumn("AFareaID").value;	

	//If any of the column values are null then Review update is incomplete else Review Update Complete.
	if(dBrevt == null || dBnof == null || dBost == null || dBort == null || dBirt == null || dBrwt == null || dBprg == null || dBprj == null || dBlod == null || dBpaa == null || dBosa == null || dBina == null || dBasp == null || dBlic == null || dBnur == null || dBmed == null || dBrwe == null || dBcle == null || dBcld == null || dBafa == null)
	{
		app.alert("Review Update Incomplete. Please send an email to DLFMSeReview@honeywell.com for more information");
	}
	else
	{
		ind.display = display.visible;
		ind.fillColor = color.green;
		ind.strokeColor = color.green;
		ind.value = "Review Update Complete";
		dBq.display = display.hidden;
		app.alert("Review Update Complete. Select File > Save to save the review packet before exiting",1);
	}	
}
/*******************************************************************************************************
**
**  Function:   oversight_defects 
**
**  Purpose:    write to defects table in database the defects logged by oversight on coversheet
**
********************************************************************************************************
*/
function oversight_defects(doc)
{
 var a = doc.getField("R.Ref_ID").valueAsString;	//Review ID
 var c = doc.getField("R.SubProject").value;		//ACM Subproject
 var i = doc.getField("DT.Total").value;		//Total # of technical defects
 var j = doc.getField("DNT.Total").value;		//Total # of non-technical defects
 var k = doc.getField("DP.Total").value;		//Total # of process defects
 var dstatement1,dstatement2,dstatement3,dquery1,dquery2,dquery3,drow1,drow2,query1,statement1,row1; 
 var nstatement3,nquery3,nrow3,projid;
 var d1,d2,d3,d4,d5,revid, statement2,query2;
 var x = doc.getField("R.oversight");
 var y = doc.getField("R.SupplierLocation");

  //Commented all OversightStatus update queries as this is handled in the UpdateOversightdB function 
  
 try
 {                                               
    var conn    = ADBC.newConnection("OversightdB");    
    dstatement1 = conn.newStatement();
    dstatement2 = conn.newStatement();
    dstatement3 = conn.newStatement();
    statement1 = conn.newStatement();
    statement2 = conn.newStatement();
    nstatement3 = conn.newStatement();

  //Defects table
  //ID column of defects table - Auto increment column does not work?
   dquery1 = "SELECT * FROM defects WHERE  ID = (SELECT MAX(ID) FROM defects)"; 
   dstatement1.execute(dquery1);   
   dstatement1.nextRow();
   drow1 = dstatement1.getRow();
   d1 = drow1.ID.value;
   d1 = d1 + 1;   

   //Get ProjectID for the ACM Subproject on the coversheet from projects table.
   nquery3 = "SELECT ID FROM projects WHERE Name = \'"+c+"\';";
   nstatement3.execute(nquery3);     
   nstatement3.nextRow();
   nrow3 = nstatement3.getRow();
   projid = nrow3.ID.value;
   c = projid; 

   //reviews.ID from reviews table for Review ID column
   dquery2 = "SELECT ID FROM reviews WHERE ReviewName = \'"+a+"\' AND ProjectID = \'"+c+"\' ;";
   dstatement2.execute(dquery2); 
   dstatement2.nextRow();
   drow2 = dstatement2.getRow();
   d2 = drow2.ID.value;  				//this gives the ID column value in reviews table for the Review ID and ACM Subproject entered on coversheet.

   //To avoid having more than one entry in the defects table when Update defects table is selected from coversheet
   query1 = "SELECT ReviewID FROM defects WHERE ReviewID = \'"+d2+"\';"; 
   statement1.execute(query1); 
   try{ 
	for (var icnt1 = 0; icnt1 < d1; icnt1++)
	{
		if(icnt1 != d1 - 1 )
		{
			statement1.nextRow();
			row1 = statement1.getRow();
			revid = row1.ReviewID.value;
		}
		break;
	}
     }catch(e){}

     //Compare defects table and reviews table for ReviewID
     if(revid == d2)
     {
	app.alert("Oversight defects data for ReviewID " + a + " exists, to update / modify please send an email to DLFMSeReview@honeywell.com");
     }
     else
     {
	d5 = "TBD"; 
	//Oversight eligible checked
	if (x.isBoxChecked(0))
	{
		x.checkThisBox(0,true);
		//Supplier/Producer location not selected - do nothing - pop-up message to select one location from drop down
		if(y.value == "Select Employer")
		{
			app.alert("Select a Producer Employer");
			//break;
		}
		else
		{
			//Comments column
			d5 = "TBD"; 			//Default value "TBD"
   
			//DefectID - Insert defects count to database
			if(i != "")
			{
				d3 = 1;
				d4 = 1;    
				for(var dcnt = 0;dcnt < i;dcnt++)
				{
					dquery3 = "INSERT INTO defects VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);
					d1 = d1 + 1;				//auto-increment not working?
				}
			}
			if(j != "")
			{
				d3 = 2;
				d4 = 2;
				for(var dcnt = 0;dcnt < j;dcnt++)
				{
					dquery3 = "INSERT INTO defects VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);	
					d1 = d1 + 1;				//auto-increment not working?
				}
			}
			if(k != "")
			{
				d3 = 3;
				d4 = 3;
				for(var dcnt = 0;dcnt < k;dcnt++)
				{
					dquery3 = "INSERT INTO defects VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);
					d1 = d1 + 1;				//auto-increment not working?
				}
			}  
			app.alert("Oversight defects update complete", 1);
			// reviews's oversight status in database should be set to DONE     	
			/*query2 = "UPDATE reviews SET OversightStatus = 'DONE' WHERE ID = \'"+d2+"\'"; 		
			statement2.execute(query2);*/					//Commented on 16Nov10
		}
	}
	else
	{
		//Oversight eligible is unchecked, Supplier/Producer Location is default - Honeywell - Phoenix
		x.checkThisBox(0,false);
		/*y.value = "Honeywell";	
		
		// reviews's oversight status in database should be set to NO   
		query2 = "UPDATE reviews SET OversightStatus = 'NO' WHERE ID = \'"+d2+"\'"; 		
		statement2.execute(query2);*/						//Commented on 16Nov10
	}
    }	//end else
 }                                            
 catch(e)
 {   
   app.alert("Review update not done for ReviewID " + a + " first perform Review update"); 
 }    
 }
/*******************************************************************************************************
**
**  Function:   supplier_defects 
**
**  Purpose:    write to defects_supplier table in database the defects logged by supplier on coversheet
**
********************************************************************************************************
*/
function supplier_defects(doc)
{
 var a = doc.getField("R.Ref_ID").valueAsString;	//Review ID
 var c = doc.getField("R.SubProject").value;		//ACM Subproject
 var f = doc.getField("DT.Supplier").value;		//# of technical defects not found by supplier
 var g = doc.getField("DNT.Supplier").value;		//# of non-technical defects not found by supplier
 var h = doc.getField("DP.Supplier").value;		//# of process defects not found by supplier
 var dstatement1,dstatement2,dstatement3,dquery1,dquery2,dquery3,drow1,drow2,query1,statement1,row1,dBquery2,dBstatement2; 
 var d1,d2,d3,d4,d5,revid,os_query1,os_statement1,os_row1,osid,query2,statement2;
 var nstatement3,nquery3,nrow3,projid;
 var x = doc.getField("R.oversight");
 var y = doc.getField("R.SupplierLocation");
 
 //Commented all OversightStatus update queries as this is handled in the UpdateOversightdB function 
 
 
 try
 {                                               
    var conn    = ADBC.newConnection("OversightdB");
    dstatement1 = conn.newStatement();
    dstatement2 = conn.newStatement();
    dstatement3 = conn.newStatement();
    statement1 = conn.newStatement();
    statement2 = conn.newStatement();
    os_statement1 = conn.newStatement();
    nstatement3 = conn.newStatement();
    
  //Defects table
  //ID column of defects table - Auto increment column does not work?
   dquery1 = "SELECT * FROM defects_supplier WHERE  ID = (SELECT MAX(ID) FROM defects_supplier)"; 
   dstatement1.execute(dquery1);   
   dstatement1.nextRow();
   drow1 = dstatement1.getRow();
   d1 = drow1.ID.value;
   d1 = d1 + 1;   

   //Get ProjectID for the ACM Subproject on the coversheet from projects table.
   nquery3 = "SELECT ID FROM projects WHERE Name = \'"+c+"\';";
   nstatement3.execute(nquery3);     
   nstatement3.nextRow();
   nrow3 = nstatement3.getRow();
   projid = nrow3.ID.value;
   c = projid; 

   //reviews.ID from reviews table for Review ID column
   dquery2 = "SELECT ID FROM reviews WHERE ReviewName = \'"+a+"\' AND ProjectID = \'"+c+"\' ;";
   dstatement2.execute(dquery2); 
   dstatement2.nextRow();
   drow2 = dstatement2.getRow();
   d2 = drow2.ID.value;  							//this gives the ID column value in reviews table for the Review ID and ACM Subproject entered on coversheet.

    //To avoid having more than one entry in the defects table when Update defects table is selected from coversheet
   query1 = "SELECT ReviewID FROM defects_supplier WHERE ReviewID = \'"+d2+"\';"; 
   statement1.execute(query1); 
   try{ 
	for (var icnt1 = 0; icnt1 < d1; icnt1++)
	{
		if(icnt1 != d1 - 1 )
		{
			statement1.nextRow();
			row1 = statement1.getRow();
			revid = row1.ReviewID.value;
		}
		break;
	}
     }catch(e){}

     //Compare defects table and reviews table for ReviewID
     if(revid == d2)
     {
	app.alert("Producer defects data for ReviewID " + a + " exists, to update / modify please send an email to DLFMSeReview@honeywell.com");
	
     }
     else
     {
	d5 = "TBD"; 
 	if (x.isBoxChecked(0))
	{
		x.checkThisBox(0,true);
		if(y.value == "Select Employer")
		{
			app.alert("Select a Producer Employer");
		}
		else
		{    
			//Comments column
			d5 = "TBD"; 			//Default value "TBD"
		   
			//DefectID - Insert defects count to database
			if(f != "")
			{
				d3 = 1;
				d4 = 1;       
				for(var dcnt = 0;dcnt < f;dcnt++)
				{
					dquery3 = "INSERT INTO defects_supplier VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);
					d1 = d1 + 1;				//auto-increment not working?
				}
			}	
			if(g != "")
			{
				d3 = 2;
				d4 = 2;
				for(var dcnt = 0;dcnt < g;dcnt++)
				{
					dquery3 = "INSERT INTO defects_supplier VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);	
					d1 = d1 + 1;				//auto-increment not working?
				}
			}	
			if(h != "")
			{
				d3 = 3;
				d4 = 3;
				for(var dcnt = 0;dcnt < h;dcnt++)
				{
					dquery3 = "INSERT INTO defects_supplier VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);
					d1 = d1 + 1;				//auto-increment not working?
				}
			} 
			app.alert("Producer defects update complete", 1);
			//Commented on 16Nov10----Start
			// reviews's oversight status in database should be set to DONE     	
			/*query2 = "UPDATE reviews SET OversightStatus = 'DONE' WHERE ID = \'"+d2+"\'"; 		
			statement2.execute(query2);*/
			   
			/*os_query1 = "SELECT ReviewID FROM defects WHERE ReviewID = \'"+d2+"\';"; 
			os_statement1.execute(os_query1);
			   try{ 
				for (var icnt1 = 0; icnt1 < d1; icnt1++)
				{
					if(icnt1 != d1 - 1 )
					{
						os_statement1.nextRow();
						os_row1 = os_statement1.getRow();
						osid = os_row1.ReviewID.value;
					}
					break;
				}
			}catch(e){
				query2 = "UPDATE reviews SET OversightStatus = \'"+dBost+"\' WHERE ID = \'"+d2+"\'"; 		
				statement2.execute(query2);
			}*/
			//Commented on 16Nov10-----End
			
		}
	}
	else
	{
		
		x.checkThisBox(0,false);
		//y.value = "Honeywell";
					//Comments column
			d5 = "TBD"; 			//Default value "TBD"
		   
			//DefectID - Insert defects count to database
			if(f != "")
			{
				d3 = 1;
				d4 = 1;       
				for(var dcnt = 0;dcnt < f;dcnt++)
				{
					dquery3 = "INSERT INTO defects_supplier VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);
					d1 = d1 + 1;				//auto-increment not working?
				}
			}	
			if(g != "")
			{
				d3 = 2;
				d4 = 2;
				for(var dcnt = 0;dcnt < g;dcnt++)
				{
					dquery3 = "INSERT INTO defects_supplier VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);	
					d1 = d1 + 1;				//auto-increment not working?
				}
			}	
			if(h != "")
			{
				d3 = 3;
				d4 = 3;
				for(var dcnt = 0;dcnt < h;dcnt++)
				{
					dquery3 = "INSERT INTO defects_supplier VALUES (\'"+d1+"\',\'"+d2+"\',\'"+d3+"\',\'"+d4+"\',\'"+d5+"\')";
					dstatement3.execute(dquery3);
					d1 = d1 + 1;				//auto-increment not working?
				}
			} 
			app.alert("Producer defects update complete", 1);
			// reviews's oversight status in database should be set to DONE     	
			/*query2 = "UPDATE reviews SET OversightStatus = 'NO' WHERE ID = \'"+d2+"\'"; 		
			statement2.execute(query2);*/				//Commented on 16Nov10
	}
    }	//end else
 }                                            
 catch(e)
 {   
   app.alert("Review update not done for ReviewID " + a + " first perform Review update"); 
 }    
}    
/*******************************************************************************************************
**
**  Function:   update Participant data 
**
**  Purpose:    adds the returned participant data to the cover sheet
**
********************************************************************************************************
*/

function UpdatePartData(doc)
{
// Purpose: Process specially marked Note object inserted by
//          ANSendCommentsToAuthor. This note contains 
//          Participant Field Data in an expected format (";" delimited).
// 1) Find Note with author = keyword
// 2) Save Note context into array
// 3) Test for form version and number of records
// 4) Find match reviewer name, or empty Participant Data Row .
// 5) Set form fields to array contents.
// 6) Prompt user to delete special note.  If OKs, delete Note.
// Assumption:
// 1) Empty Participant Data Row exists (only 7 available)
// 2) Participant Name field not changed
//---------------------------------
	console.println("Start UpdatePartData");
	try { doc.gotoNamedDest("Participants"); } catch(e) {}
	doc.syncAnnotScan();
	var annots = doc.getAnnots({nSortBy: ANSB_Page, bReverse: false, nPage: 0}); //only first page
	if (annots == null) return;
	for (var i = 0; i < annots.length; i++) {
		if (annots[i].author == "Automated Participant Data") {
			var arrOrig = annots[i].contents.split(";");
			try {
				var formType = "unknown";
				var totrow = 0;
				var tmp = arrOrig.length+1;
				var tp = arrOrig.length - 1;
				console.println("Number in array : "+tmp);
				if ((tmp % 5) == 0) formType = "Dec04";
				if ((tmp % 7) == 0) formType = "Feb05";
				if ((tp % 8) == 0) formType = "Jul05";
				if (formType == "Dec04") totrow = tmp / 5;
				if (formType == "Feb05") totrow = tmp / 7;
				if (formType == "Jul05") totrow = tp / 8;
				console.println("Records to process : "+totrow);
			} catch(e) { 
				var msg1 = "Could not add participant data";
				if (formType == "unknown") app.alert(msg1+" - Unknown form version.");
				if (totrow == 0) app.alert(msg1+" - Unexpected data /format.");
				return;
			}
			if (formType == "Dec04")
			{// Dec04 Form Version has 5 fields per row
				for (var xrow = 0; xrow < totrow-1; xrow++) {
					try { 
						console.println("Dec 04 Form Version");
						//var pdRow = arrOrig[xrow*5];
						//protect against old annots.js
						//if (pdRow.length < 2) pdRow = "E"+pdRow;
						     //check if name match or is blank
						var goodrow = 0;
						for (var iCnt = 1; iCnt < numMaxReviewers ; iCnt++) //7 participant rows on form
						{
							if (doc.getField("E"+iCnt+".Name").value == (arrOrig[xrow*5+1])) goodrow = iCnt;
						}
						if (goodrow == 0)
						{
							for (var iCnt = 1; iCnt < numMaxReviewers ; iCnt++) 
							{
							if (doc.getField("E"+iCnt+".Name").length == 0) goodrow = iCnt;
							}
						}
						if (goodrow == 0)
						{
							app.alert({cMsg: "No space for Participation data on form.", nIcon: 1});
							return;
						}
						var pdRow = "E"+goodrow;
						doc.getField(pdRow+".Name").value = (arrOrig[xrow*5+1]);
						doc.getField(pdRow+".Function").value = (arrOrig[xrow*5+2]);
						try {
							doc.getField(pdRow+".Time").value = (arrOrig[xrow*5+3]) * 1;
						} catch(e) {}
						doc.getField(pdRow+".Role").value = (arrOrig[xrow*5+4]);
						if ((arrOrig[xrow*5+5]) == "Yes") {
							doc.getField(pdRow+".Attend").checkThisBox(0,true); }
						else {doc.getField(pdRow+".Attend").checkThisBox(0,false); }
						if ((arrOrig[xrow*5+6]) == "Yes") {
							doc.getField(pdRow+".Closer").checkThisBox(0,true); }
						else {doc.getField(pdRow+".Closer").checkThisBox(0,false); }
					} catch(e) { 
						console.println("UpdatePartData Error - Form version "+formType);
						app.alert({cMsg: "Unsuccessful adding Participant Data to cover sheet.", nIcon: 3});
						return;
					}
				}
			} // formType == "Dec04"
			if (formType == "Feb05")
				{//Feb 05 form has 7 fields per row
				for (var xrow = 0; xrow < totrow; xrow++) {
					try { 
						console.println("Feb 05 Form Version");
						//var pdRow = arrOrig[xrow*7];
						//if (pdRow.length < 2) pdRow = "E"+pdRow;
						var goodrow = 0;
						for (var iCnt = 1; iCnt < numMaxReviewers ; iCnt++) //7 participant rows on form
						{
							if (doc.getField("E"+iCnt+".Name") != null)
								if (doc.getField("E"+iCnt+".Name").value == (arrOrig[xrow*7+1])) 
									goodrow = iCnt;
						}
						if (goodrow == 0)
						{
							for (var iCnt = 1; iCnt < numMaxReviewers ; iCnt++) 
							{
								var f = doc.getField("E"+iCnt+".Name");
								if(f != null)
									if(f.length == 0 || f.value == "Name here keeps page")
										goodrow = iCnt;
							}
						}
						if (goodrow == 0)
						{
							app.alert({cMsg: "No space for Participation data on form.", nIcon: 1});
							return;
						}
						var pdRow = "E"+goodrow;
						
						if (doc.getField(pdRow+".Name") != null)
							doc.getField(pdRow+".Name").value = (arrOrig[xrow*7+1]);
						if (doc.getField(pdRow+".Function") != null)
							doc.getField(pdRow+".Function").value = (arrOrig[xrow*7+2]);
						try 
						{
							if (doc.getField(pdRow+".Time") != null)
								doc.getField(pdRow+".Time").value = (arrOrig[xrow*7+3]) * 1;
						} catch(e) {}
						if (doc.getField(pdRow+".Role") != null)
							doc.getField(pdRow+".Role").value = (arrOrig[xrow*7+4]);
						
						
						if ((arrOrig[xrow*7+5]) == "Yes") 
						{
							if (doc.getField(pdRow+".Attend") != null)
								doc.getField(pdRow+".Attend").checkThisBox(0,true); 
						}
						else 
						{
							if (doc.getField(pdRow+".Attend") != null)
								doc.getField(pdRow+".Attend").checkThisBox(0,false); 
						}

						if ((arrOrig[xrow*7+6]) == "Yes") 
						{
							if (doc.getField(pdRow+".Closer") != null)
								doc.getField(pdRow+".Closer").checkThisBox(0,true); 
						}
						else 
						{
							if (doc.getField(pdRow+".Closer") != null)
								doc.getField(pdRow+".Closer").checkThisBox(0,false); 
						}
						
						if ((arrOrig[xrow*7+7]) == "Yes") 
						{
							if (doc.getField(pdRow+".Sign") != null)
								doc.getField(pdRow+".Sign").checkThisBox(0,true); }
							else 
							{ 
								doc.getField(pdRow+".Sign").checkThisBox(0,false); 
							}
					} catch(e) { 
						console.println("UpdatePartData Error - Form version "+formType);
						app.alert({cMsg: "Unsuccessful adding Participant Data to cover sheet.", nIcon: 3});
						return;
					}
				}
			} // formType == "Jul05"
			if (formType == "Jul05")
				{//Jul 05 form has 7 fields per row
				for (var xrow = 0; xrow < totrow; xrow++) {
					try { 
						console.println("Jul 05 Form Version");
						//var pdRow = arrOrig[xrow*7];
						//if (pdRow.length < 2) pdRow = "E"+pdRow;
						var goodrow = "";
						
						
						if (arrOrig[xrow*8+0] != "")
							goodrow = arrOrig[xrow*8+0];
/*
						for (var iCnt = 1; iCnt < numMaxReviewers ; iCnt++) //7 participant rows on form
						{
							if (doc.getField("E"+iCnt+".Name") != null)
								if (doc.getField("E"+iCnt+".Name").value == (arrOrig[xrow*8+1])) 
									goodrow = iCnt;
						}

						if (goodrow == 0)
						{
							for (var iCnt = 1; iCnt < numMaxReviewers ; iCnt++) 
							{
								if (doc.getField("E"+iCnt+".Name").value.length == 0)
								{
									goodrow = "E" + iCnt;
									break;
								}
							}
						}
*/
						if (goodrow == "")
						{
							app.alert({cMsg: "Unknown row number for Participation data on form.", nIcon: 1});
							return;
						}

						var pdRow = goodrow;
						
						if (doc.getField(pdRow+".Name") != null)
							doc.getField(pdRow+".Name").value = (arrOrig[xrow*8+1]);
						if (doc.getField(pdRow+".Function") != null)
							doc.getField(pdRow+".Function").value = (arrOrig[xrow*8+2]);
						try 
						{
							if (doc.getField(pdRow+".Time") != null)
								doc.getField(pdRow+".Time").value = (arrOrig[xrow*8+3]) * 1;
						} catch(e) {}
						if (doc.getField(pdRow+".Role") != null)
							doc.getField(pdRow+".Role").value = (arrOrig[xrow*8+4]);
						
						
						if ((arrOrig[xrow*8+5]) == "Yes") 
						{
							if (doc.getField(pdRow+".Attend") != null)
								doc.getField(pdRow+".Attend").checkThisBox(0,true); 
						}
						else 
						{
							if (doc.getField(pdRow+".Attend") != null)
								doc.getField(pdRow+".Attend").checkThisBox(0,false); 
						}

						if ((arrOrig[xrow*8+6]) == "Yes") 
						{
							if (doc.getField(pdRow+".Closer") != null)
								doc.getField(pdRow+".Closer").checkThisBox(0,true); 
						}
						else 
						{
							if (doc.getField(pdRow+".Closer") != null)
								doc.getField(pdRow+".Closer").checkThisBox(0,false); 
						}
						
						if ((arrOrig[xrow*8+7]) == "Yes") 
						{
							if (doc.getField(pdRow+".Sign") != null)
								doc.getField(pdRow+".Sign").checkThisBox(0,true); 
						}
						else 
						{ 
							doc.getField(pdRow+".Sign").checkThisBox(0,false); 
						}
					} catch(e) { 
						console.println("UpdatePartData Error - Form version "+formType);
						app.alert({cMsg: "Unsuccessful adding Participant Data to cover sheet.", nIcon: 3});
						return;
					}
				}
			} // formType == "Feb05"
			annots[i].destroy();
		}//"Automated Participant Data" note
	}
}

/*******************************************************************************************************
**
**  Function:   update Checklist data 
**
**  Purpose:    adds the returned checklist data to the checklists in the Packet
**
********************************************************************************************************
*/

// Reads the returned note and places checkmarks on the stamped checklists
function UpdateChecklistData(doc)
{
	console.println("Start UpdateChecklistData");

	doc.syncAnnotScan();
	var annots = doc.getAnnots({nSortBy: ANSB_Page, bReverse: false, nPage: 0}); //only first page
	if (annots == null) 
		return;
	for (var i = 0; i < annots.length; i++) 
	{
		// Find note with author "Automated Checklist Data"
		if (annots[i].author == "Automated Checklist Data") 
		{
			// Store contents of the note
			var arrOrig = annots[i].contents.split(";");
			try 
			{
				var formType = "unknown"
				var totlists = 0;
				var tmp = arrOrig.length - 1;
				console.println("Number in array : "+tmp);
				if ((tmp % 6) == 0) formType = "Jul05";
				if (formType == "Jul05") totlists = tmp / 6;
				console.println("Records to process : "+totlists);
			} 
			catch(e) 
			{ 
				var msg1 = "Could not add participant data";
				if (formType == "unknown") app.alert(msg1+" - Unknown form version.");
				if (totrow == 0) app.alert(msg1+" - Unexpected data /format.");
				return;
			}
			
			if (formType == "Jul05")
				{//Jul 05 form
				for (var xlist = 0; xlist < totlists; xlist++) 
				{
					try 
					{ 
						console.println("Jul 05 Form Version");

						// try to open checklist field
						var f = doc.getField(arrOrig[xlist*6+0] + ".ID");

						// if field not null
						if(f != null)
							// if a Stamped Checklist
							if(f.value == 0)
							{
								// If any of the checkboxes (1-5) has a value of "Yes", check the box
								// else remove check mark from check box
								if ((arrOrig[xlist*6+1]) == "Yes") 
								{
									if (doc.getField(arrOrig[xlist*6+0] +".1") != null)
										doc.getField(arrOrig[xlist*6+0] +".1").checkThisBox(0,true); 
								}
								else 
								{ 
									doc.getField(arrOrig[xlist*6+0] +".1").checkThisBox(0,false); 
								}
								if ((arrOrig[xlist*6+2]) == "Yes") 
								{
									if (doc.getField(arrOrig[xlist*6+0] +".2") != null)
										doc.getField(arrOrig[xlist*6+0] +".2").checkThisBox(0,true); 
								}
								else 
								{ 
									doc.getField(arrOrig[xlist*6+0] +".2").checkThisBox(0,false); 
								}
								if ((arrOrig[xlist*6+3]) == "Yes") 
								{
									if (doc.getField(arrOrig[xlist*6+0] +".3") != null)
										doc.getField(arrOrig[xlist*6+0] +".3").checkThisBox(0,true); 
								}
								else 
								{ 
									doc.getField(arrOrig[xlist*6+0] +".3").checkThisBox(0,false); 
								}
								if ((arrOrig[xlist*6+4]) == "Yes") 
								{
									if (doc.getField(arrOrig[xlist*6+0] +".4") != null)
										doc.getField(arrOrig[xlist*6+0] +".4").checkThisBox(0,true); 
								}
								else 
								{ 
									doc.getField(arrOrig[xlist*6+0] +".4").checkThisBox(0,false); 
								}
								if ((arrOrig[xlist*6+5]) == "Yes") 
								{
									if (doc.getField(arrOrig[xlist*6+0] +".5") != null)
										doc.getField(arrOrig[xlist*6+0] +".5").checkThisBox(0,true); 
								}
								else 
								{ 
									doc.getField(arrOrig[xlist*6+0] +".5").checkThisBox(0,false); 
								}
							}
					}
					catch(e){}
				}
		
			} // formType == "Feb05"
			//delete the note
			annots[i].destroy();
		}
	}		
}

/*******************************************************************************************************
**
**  Function:   checklistUpdate
**
**  Purpose:    make sure check list is updated
**
********************************************************************************************************
*/
//EDIT 11/25/2009 by Fei.Wang
function checklistUpdate(doc)
{

  var Project = doc.getField("R.Project");
  var SubProject = doc.getField("R.SubProject");
  var FunctionArea = doc.getField("R.Farea");

  var SCRNum = doc.getField("F1.SCR");

  // add logic for get the different from F.SCR field which will be fill into checklist.
  var x = SCRNum.valueAsString;

  for(var vCnt = 2; vCnt <= 40; vCnt++)
  {

    var Temp = doc.getField("F" + vCnt + ".SCR");

    if ((Temp == null) || (Temp.value == ""))
    {
      break;
    }
    else
    {
      if (SCRNum.value != Temp.value)
      {
        x = x + ";" + Temp.valueAsString;
      }
      SCRNum = Temp;
    }
  }
  // end update

  if(coversheetMissing(doc))//check at least 1 coversheet
  {
    warningList.push("Coversheet was not found.");
  }
  else if(checklistMissing(doc))//check at least 1 checklist
  {
    warningList.push("Checklist was not found.");
  }
  else
  {
    var a = doc.getField("C.Project");
    a.readonly = false;
    a.value = Project.value;

    var b = doc.getField("C.SubProject");
    b.readonly = false;
    b.value = SubProject.value;

    var c = doc.getField("C.SCRNum");
    c.readonly = false;
    //c.value = SCRNum.valueAsString;
    c.value = x;

    var d = doc.getField("C.AffectArea");
    d.readonly = false;
    d.value = FunctionArea.value;
  }
}

// checks for at least one cover sheet in the document, returns true if missing
function coversheetMissing(doc) 
{
  try 
  { 
    var f = doc.getField("FormName");
    if (f != null) 
		{
      return false;
    }
	} catch(e) { app.alert("coversheet missing catch"); }
	return true;
}

// checks for at least one check list in the document, returns true if missing
function checklistMissing(doc) 
{
  try 
  { 
    var f = doc.getField("CHK_CTP.ID");
    if (f != null) 
    {
      return false;
    }
	} catch(e) { app.alert("checklist missing catch"); }
  return true;
}

// Add dummy SCR to Supporting Materials Field
function supportUpdate(doc)
{
  var v = doc.getField("F1.SCR");
  var f = doc.getField("R.Support");
  if(v.value.length > 10 && f.value.indexOf(v.value) < 0 )
  {
    f.value = f.value + "\nSCR " + doc.getField("F1.SCR").value + " are included in this packet.";
	}
}

//Return true if meeting date is empty
function meetingDateMissing(doc)
{
  var f = doc.getField("M.Date");
  if(f != null)
    if(f.value <= 0)
      return true;

  return false;
}

//Return true if meeting time is less than zero
function meetingTimeMissing(doc)
{
  var f = doc.getField("M.Time");
  if(f != null)
    if(f.value <= 0)
      return true;

  return false;
}

//Return true if meeting duration is less than zero
function meetingDurationMissing(doc)
{
  var f = doc.getField("M.Duration");
  if(f != null)
    if(f.value <= 0)
      return true;

  return false;
}

//Return true if meeting number is less than zero
function meetingNumberMissing(doc)
{
  var f = doc.getField("M.Number");
  if(f != null)
    if(f.value <= 0)
      return true;

  return false;
}

//Return true if Function Area is Unknown
function FAreaMissing(doc)
{
  var f = doc.getField("R.Farea");
  if(f != null)
    if(f.value == "Unknown")
      return true;

  return false;
}
//Return true if Load Info is N/A,TBD or undefined
function LoadMissing(doc)
{
  var f = doc.getField("R.Load");
  if(f != null)
    if(f.value == "N/A" || f.value == "TBD" || f.value == "undefined")
      return true;

  return false;
}

//Return true if Aircraft Info is N/A or Aircraft Affected
function AircraftMissing(doc)
{
  var f = doc.getField("R.Aircraft");
  if(f != null)
    if(f.value == "Aircraft Affected" || f.value == "")
      return true;

  return false;
}
