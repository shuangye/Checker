//Autoupdate the review checklist and trace checklist
function AutoUpdateChecklist(doc)
{
	TraceChecklistUpdate(doc);
	ReviewChecklistUpdate(doc);
	WorkProductsUpdate(doc);
}

//Auto update the check list items which the value are always equal to N/A and 
//fill the information in the N N/A Justification Box.
function ReviewChecklistUpdate(doc)
{
	try
	{
		var Tf = doc.getField("TRC_CHK.ID");//get the trace_checklist sheet.
		for (var i = 1; i<= 45; i++)
		{
			var f = doc.getField("CTP." + i);
			if(f != null)
			{
				if ((i == 4) && (Tf == null) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 22) && (f.value != "Yes" && f.value != "No" && f.value != "NA")) //Items 22, 29, 30, 33, 34, 40, 41, 42, 43 have been set to N/A. 
				{
					f.value = "NA";
				}
				else if ((i == 29) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 30) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 33) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 34) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 40) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 41) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 42) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 43) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}
				else if ((i == 45) && (f.value != "Yes" && f.value != "No" && f.value != "NA"))
				{
					f.value = "NA";
				}				
				else if (f.value != "Yes" && f.value != "No" && f.value != "NA") //Other items keep the value which set by user.
				{
					f.value = "Yes";
				}
			}
		}
	}
	catch(e)
	{
		app.alert("Error while updating Review Check list.\n" + e);
	}
	
	//Update the N N/A Justification Box
	try
	{
		if (doc.getField("1.Text1").value == "")
		{
			doc.getField("1.Text1").value = "N/A:\n";
			if (Tf == null)
			{
				doc.getField("1.Text1").value += "For item 4 : TRT file is not updated.\n";			
			}
			doc.getField("1.Text1").value += "For item 22 : there is no bad trace.\n"
						+ "For item 29,30 : this CTP test follows Level B standard.\n"
						+ "For item 33,34 : there are no \"complex algorithms\" or \"complex requirements\" for this CTP.\n"
						+ "For item 40 : there are no failures in this CTP.\n"
						+ "For item 41,42,43 : there is no C++ code or C++ stub code.\n"
						+ "For item 45 : there is no defect found during review";

		}
	}
	catch(e)
	{
		app.alert("Error while updating the N N/A Justification Box.\n" + e);
	}
	
	//Alert whether the value of items 38, 39 should be set to N/A.
	try
	{
		if ((doc.getField("CTP.38").value != "NA") || (doc.getField("CTP.39").value != "NA"))
		{
			app.alert(
						{
							cMsg: "Note:\n" + "For item 38,39\n" 
											+ "Make sure all of the requirements were completely tested in this CTP,\n" 
											+ "if true these items should be set to N/A.",
							cTitle: "Check For Items 38&39",
							nIcon: 3
						}
					);	
		}
	}
	catch(e)
	{
		app.alert("Error while checking point 38,39.\n" + e);
	}
}

//Auto updated trace checklist if it is exist.
function TraceChecklistUpdate(doc)
{
	try
	{	
		f = doc.getField("TRC_CHK.ID");
		if (f != null)
		{
			var a = doc.getField("T.Project");
			a.readonly = false;
			a.value = doc.getField("R.Project").value;
			
			var b = doc.getField("T.Subproject");
			b.readonly = false;
			b.value = doc.getField("R.SubProject").value;
			
			var c = doc.getField("T.ScrNum");
			c.readonly = false;
			c.value = doc.getField("F1.SCR").value;
			
			var d = doc.getField("T.AffectArea");
			d.readonly = false;
			d.value = doc.getField("R.Farea").value;

			for (var i = 1; i <= 8; i++)
			{
				var Cf = doc.getField("CkList." + i);
				if (Cf != null && Cf.value != "Yes" && Cf.value != "No" && Cf.value != "NA")
				{
					Cf.value = "Yes";
				}
			}
		}
	}
	catch(e)
	{
		app.alert("Error while updating the Trace_Checklist\n" + e);
	}
}

//Auto updated Work Products when there are more than one SCR number.
function WorkProductsUpdate(doc)
{
	try
	{
		f = doc.getField("F1.SCR").value;
		if ((f != "") && (f.length > 7))
		{
			m = f.split(",");
			var i = 0;
			var j = 1;
			var x = 0;
			var z = 0;
			while (m[i] != null)
			{
				i++;
			}

			while ((doc.getField("F" + j + ".SCR").value != ""))
			{
				x++;
				j++;
			}
			
			for (var y = 1; y <= i*x; y++)
			{
				doc.getField("F" + y + ".SCR").value = m[z];
				if ((y%x) == 0)
				{
					z++;
				}
				if ((y+x) <= i*x)
				{
					doc.getField("F" + (y+x) + ".Name").value = doc.getField("F" + y + ".Name").value;
					doc.getField("F" + (y+x) + ".Ver").value = doc.getField("F" + y + ".Ver").value;
					doc.getField("F" + (y+x) + ".Size").value = doc.getField("F" + y + ".Size").value;
					doc.getField("F" + (y+x) + ".Units").value = doc.getField("F" + y + ".Units").value;
				}
			}
		}
	}
	catch(e)
	{
		app.alert("Error while updating the Work Products\n" + e);
	}
}

//check packet info
var ErrorList = ""; 
var ListCheckErr = false; 
var InfoCheckErr = false;
var SupMaterCheck = false;
var SiUsCheck = false;
var TrclistCheckErr = false;
var ParticipantsCheckErr = false;

function ReviewChecklistCheck(doc)
{
	ErrorList = "";
	SupportMaterialCheck(doc);
	SizeUnitsCheck(doc);
	NanInfoCheck(doc);
	AllReviewItemSelect(doc);
	TraceChecklistCheck(doc);
	ReviewParticipantsCheck(doc);
	
	
	if (InfoCheckErr || SupMaterCheck || SiUsCheck || ListCheckErr || TrclistCheckErr || ParticipantsCheckErr)
	{
		app.alert(ErrorList + "Please check again and make sure all items right!\n");			
	}
	else
	{
		app.alert(
					{
						cMsg: "Check Successfully!",
						cTitle: "Check Point",
						nIcon: 3
					}
				);	
	}	
	
    ListCheckErr = false; 
    InfoCheckErr = false;
    SupMaterCheck = false;
    SiUsCheck = false;
    TrclistCheckErr = false;	
	ParticipantsCheckErr = false;
}

//Check Reviewlist to make sure that all of the items have been set value.
function AllReviewItemSelect(doc)
{
	try
	{
		for (var i = 1; i<= 45; i++)
		{
			var f = doc.getField("CTP." + i);
			if (f != null)
			{
				//recond the missing selection items
				if (f.value != "Yes" && f.value != "No" && f.value != "NA")
				{
					ErrorList += "Missing selection of the checklist item " + i + "\n"; 
					ListCheckErr = true;
				}
			}
		}
	}
	catch(e)
	{
		app.alert("Error while checking the reviewlist!" + e);
	}
}

//Also check the N/A and N information in N,N/A Justification Box.
function NanInfoCheck(doc)
{
	var Info = doc.getField("1.Text1").value;
	try
	{
		//check justification info according to items value
		for (var i = 1; i<= 45; i++)
		{
			var f = doc.getField("CTP." + i);
			if (f != null)
			{
				if (f.value == "No" || f.value == "NA")
				{
					//var re = "/\D" + i;
					var index = Info.search(i);
					if (index == -1)
					{
						ErrorList += "Missing excuse of the checklist item " + i + " in the Justification Box\n"; 
						InfoCheckErr = true;
					}
				}
			}
		}
	}
	catch(e)
	{
		app.alert("Error while checking the reviewlist!" + e);
	}
}

//check common_object.c and Apex_traps.o in supportting material
function SupportMaterialCheck(doc)
{
	var f = doc.getField("R.Support").value;
	try
	{
		if (f != null)
		{
			var comm  = f.search(/\.c/i);
			var apex  = f.search(/\.o/i);
			//check common_object.c
			if ((comm == -1))
			{
				ErrorList += "Missing common.c file in supportting material.\n";
				SupMaterCheck = true;
			}
			//check Apex_traps.o
			if ((apex == -1))
			{
				ErrorList += "Missing apex_traps.o file in supportting material.\n";
				SupMaterCheck = true;				
			}
		}
		else
		{
			ErrorList += "Supportting material is null.\n";
			SupMaterCheck = true;
		}
	}
	catch(e)
	{
		app.alert("Error while checking the reviewlist!" + e);		
	}
}

//check Size Units
function SizeUnitsCheck(doc)
{
	try
	{
		var Fnum = 10;
		if (doc.getField("F42.SCR") != null) 
		{
			Fnum = 70;
		}
		else if (doc.getField("F12.SCR") != null)
		{
			Fnum = 40;
		}
		
		for (i=1; i<=Fnum; i++)
		{
			var SCR = doc.getField("F" + i + ".SCR").value;
			var NAM = doc.getField("F" + i + ".Name").value;
			var SUS = doc.getField("F" + i + ".Units").value;
			
			if (((SCR != "") || ((NAM != "") && (NAM != "Something here keeps page from being deleted on Save."))) && ((SUS == "invalid") || (SUS == "")))
			{
				ErrorList += "Size Units " + i + " must be selected.\n";
				SiUsCheck = true;
			}
		}
	}
	catch(e)
	{
		app.alert("Error while checking the Size Units!" + e);			
	}
}

//Auto check trace checklist if it is exist.
function TraceChecklistCheck(doc)
{
	try
	{	
		f = doc.getField("TRC_CHK.ID");
		if (f != null)
		{
			var a = doc.getField("T.Project");
			if ((a.value != doc.getField("R.Project").value) || (a.value == null))
			{
				ErrorList += "ACM Project on Trace Checklist is wrong!\n";
				TrclistCheckErr = true;				
			}
			
			var b = doc.getField("T.Subproject");
			if ((b.value != doc.getField("R.SubProject").value) || (b.value == null))
			{
				ErrorList += "ACM Sub-Project on Trace Checklist is wrong!\n";
				TrclistCheckErr = true;				
			}
			
			var c = doc.getField("T.ScrNum");
			if (c.value == null)
			{
				ErrorList += "SCR Number on Trace Checklist is wrong!\n";
				TrclistCheckErr = true;				
			}	
			
			var d = doc.getField("T.AffectArea");
			if ((d.value != doc.getField("R.Farea").value) || (d.value == null))
			{
				ErrorList += "Affected Area on Trace Checklist is wrong!\n";
				TrclistCheckErr = true;				
			}	
			
			for (var i = 1; i <= 8; i++)
			{
				var Cf = doc.getField("CkList." + i);
				if (Cf != null && Cf.value != "Yes" && Cf.value != "No" && Cf.value != "NA")
				{
					ErrorList += "Missing selection of the TraceList item " + i + "\n";
					TrclistCheckErr = true;
				}
			}
		}
	}
	catch(e)
	{
		app.alert("Error while checking the Trace_Checklist\n" + e);
	}
}

//check Review Participants
function ReviewParticipantsCheck(doc)
{
	try
	{
		var j = 0;
		for (var i = 1; i <= 38; i++)
		{
			if (doc.getField("E"+ i +".Time").value != "")
			{
				j++;
			}
		}
		if (doc.getField("M.Number").value != j)
		{
			ErrorList += "The number of Review Participants are not correct!\n";
			ParticipantsCheckErr = true;
		}
	}
	catch(e)
	{
		app.alert("Error while checking the Review participants!" + e);			
	}
}