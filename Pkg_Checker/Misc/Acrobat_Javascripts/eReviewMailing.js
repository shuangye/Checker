/************************************************************************************************/
//SEND FOR REVIEW
//Added by Fei Wang 9/29/2010
//
//Modified by Xinghua Liu 07/July/2011
// Update folder path for A350.
//
//Modified by Xinghua Liu 06/Sep/2011
// 1. The Function Area field in PDF should be keep up with SCR Report(i.e TEST_Function Name)
//      but in server, the folder name is by function name(i.e Function Name)
//      so convert the function name.
//      It avoid to copy file error.
// 2. The name should be consist with SCR report.
//      creat a Name list PDF due to record user name and update code for cover user name to CM name.
//
//Modified by Xinghua Liu 07/Feb/2012
// Add Switch logic for fix the issue, that when display CM name for Author, Moderator.., send for eReview function can't find
// the e-mail address in the next cycle.
// 
/************************************************************************************************/

function sendForReview(doc)
{
  if(app.viewerVersion >= 7)  //fix for 7.0 compatability
    app.beginPriv();

  // is there a valid e-mail address attached to the doc?
  var eaddr = identity.email;

  if(eaddr != null) //as long as we have an email address
  {
    var reviewIDField = doc.getField("R.Ref_ID");
    var project = doc.getField("R.Project");
    var subProject = doc.getField("R.SubProject");
    var fArea = doc.getField("R.Farea");
    var reviewType = doc.getField("R.Type");
    var faircraft = doc.getField("R.Aircraft");
    var MyEmailAddresses = "";
    var QAEmailAddresses = "";
    var emailSubject = "eReview " + faircraft.value + " /";
    var emailMessage = "";

    //define array for CM Name, Mail Name and EID
    var vCMName = new Array();
    var vMailName = new Array();
    var vEID = new Array();

    var AutName = doc.getField("E1.Name").value;
    var ModName = doc.getField("E2.Name").value;
    var InsName = doc.getField("E4.Name").value;
    var SysName = doc.getField("E5.Name").value;
    var PQ1Name = doc.getField("E6.Name").value;
    var PQ2Name = doc.getField("E7.Name").value;

    var AutEID = "";
    var ModEID = "";
    var InsEID = "";
    var SysEID = "";
    var PQ1EID = "";
    var PQ2EID = "";

    //define path for packet save as.
    var packetPath = "\\159.99.234.164\\FMS";

    var namelistPath = "/Ch71w3001/FMS/A380/Tools/Generate_Packet/NameList.PDF";

    //Remove "TEST_" from function area due to copy file to server path.
    var TempArea = fArea.value.replace("TEST_","");
    //fArea.value = fArea.value.replace("TEST_","");

    // convert Mail name to CM name and get EID
    try
    {
      var oDoc = app.openDoc({cPath: namelistPath, bHidden: true});
      //app.alert("Open DOC is OK!");
      for(var vCnt = 1; vCnt <= 44; vCnt++)
      {
        vCMName[vCnt]   = oDoc.getField("E" + vCnt + ".Name");
        vMailName[vCnt] = oDoc.getField("E" + vCnt + ".Mail");
        vEID[vCnt]      = oDoc.getField("E" + vCnt + ".EID");

        if ((vCMName[vCnt] == null) || (vEID[vCnt] == null))
        { break;  }

        if ((vCMName[vCnt].value == "") || (vEID[vCnt].value == ""))
        { break;  }
      }

      for(var i = 1; i < vMailName.length; i++)
      {
        switch(vMailName[i].value)  // switch for Mail name
        {
          case AutName :
            doc.getField("E1.Name").value = vCMName[i].value;
            AutName = vCMName[i].value;
            AutEID = vEID[i].value;

            break;

          case ModName :
            doc.getField("E2.Name").value = vCMName[i].value;
            ModName = vCMName[i].value;
            ModEID = vEID[i].value;

            break;

          case InsName :
            doc.getField("E4.Name").value = vCMName[i].value;
            InsName = doc.getField("E4.Name").value;
            InsEID = vEID[i].value;

            break;

          case SysName :
            doc.getField("E5.Name").value = vCMName[i].value;
            SysName = vCMName[i].value;
            SysEID = vEID[i].value;

            break;

          case PQ1Name :
            doc.getField("E6.Name").value = vCMName[i].value;
            PQ1Name = vCMName[i].value;
            PQ1EID = vEID[i].value;

            break;

          case PQ2Name :
            doc.getField("E7.Name").value = vCMName[i].value;
            PQ2Name = vCMName[i].value;
            PQ2EID = vEID[i].value;

            break;

          default :
            break;

        } // switch for Mail name

        switch(vCMName[i].value) // switch for CM name
        {
          case AutName :
            AutEID = vEID[i].value;
            break;

          case ModName :
            ModEID = vEID[i].value;
            break;

          case InsName :
            InsEID = vEID[i].value;
            break;

          case SysName :
            SysEID = vEID[i].value;
            break;

          case PQ1Name :
            PQ1EID = vEID[i].value;
            break;

          case PQ2Name :
            PQ2EID = vEID[i].value;
            break;

          default :
            break;

        } // switch for CM name
      }
    }
    catch(e){
        app.alert("Open Cover Name Error!" + e);
    }

    oDoc.closeDoc(true);
    //app.alert("Close Doc is OK!");

    // build E-Mail Subject, body
    if ((faircraft.value == "") || (faircraft.value == "Aircraft Affected"))
    {
      app.alert("Miss Aircraft Info.");
    }
    else
    {
      try
      {
        emailSubject = emailSubject + project.value + " / " + subProject.value + " / " + TempArea
                                    + " / " + reviewIDField.value + " / " + reviewType.value + " / CTP";

        emailMessage = "Hi all,\n\n"  + "Please Review the packet for SCR " + reviewIDField.value + ".\n\n"
                                      + "\\" + packetPath + "\\" + faircraft.value + "\\E-review_Packets\\" + TempArea
                                      + "\\" + doc.documentFileName + "\n\n"
                                      + "Author         : " + doc.getField("E1.Name").value + "\n"
                                      + "Moderator      : " + doc.getField("E2.Name").value + "\n"
                                      + "Sw Reviewer    : " + doc.getField("E4.Name").value + "\n"
                                      + "Sys Reviewer   : " + doc.getField("E5.Name").value + "\n"
                                      + "PDQE           : " + doc.getField("E6.Name").value + "\n"
                                      + "PDQE           : " + doc.getField("E7.Name").value + "\n\n"
                                      + "Review Meeting : " + doc.getField("M.Date").value + ", " + doc.getField("M.Time").value + ".\n\n"
                                      + "And let me know if you have any comments.\n\n"
                                      + "Best Regards,\n" + doc.getField("E1.Name").value;
      }//end try
      catch(e)
      {
        app.alert("Error while composing e-mail message\n" + e);
      }

      // read the user's Email address files (if it exists)
      try
      {
        QAEmailAddresses = PQ1EID + ";" + PQ2EID + ";" + SysEID;
        MyEmailAddresses = ModEID + ";" + InsEID;
      }
      catch(e)
      {
        app.alert("Error while building send list\n" + e);
      }

      //save PDF to server
      doc.saveAs("/159.99.234.164/FMS/" + faircraft.value + "/E-review_Packets/" + TempArea + "/" + doc.documentFileName);

      //set up the confirmation dialouge box
      try{
        app.mailMsg({
                     bUI: true,
                     cTo: MyEmailAddresses,
                     cCc: QAEmailAddresses,
                     cSubject: emailSubject,
                     cMsg: emailMessage
                   });
      }
      catch(e){
        app.alert("Send Mail error " + e);
      }
    }
  }
}

if(app.viewerVersion >= 7) //if we are working with acrobat 7
{
  console.println("Implementing Acrobat 7 Fix for Send Review");
  app.trustedFunction(sendForReview);  //this line of code will ONLY run in acrobat 7
}

function returnToAuthor(doc)
{
  if(app.viewerVersion >= 7)  //fix for 7.0 compatability
    app.beginPriv();

  var reviewIDField = doc.getField("R.Ref_ID");
  var project = doc.getField("R.Project");
  var subProject = doc.getField("R.SubProject");
  var fArea = doc.getField("R.Farea");
  var reviewType = doc.getField("R.Type");
  var faircraft = doc.getField("R.Aircraft");

  //Remove "TEST_" from function area due to copy file to server path.
  var TempArea = fArea.value.replace("TEST_","");
  //fArea.value = fArea.value.replace("TEST_","");

  var emailSubject = "RE: eReview " + faircraft.value + "/ " + project.value + " / " + subProject.value + " / " + TempArea
                                    + " / " + reviewIDField.value + " / " + reviewType.value + " / CTP";

  var emailMessage = "";
  var TOaddr = "";
  var CCaddr = "";

  //define array for CM Name, Mail Name and EID
  var vCMName = new Array();
  var vEID = new Array();

  var AutName = doc.getField("E1.Name").value;
  var ModName = doc.getField("E2.Name").value;
  var InsName = doc.getField("E4.Name").value;
  var SysName = doc.getField("E5.Name").value;
  var PQ1Name = doc.getField("E6.Name").value;
  var PQ2Name = doc.getField("E7.Name").value;

  var AutEID = "";
  var ModEID = "";
  var InsEID = "";
  var SysEID = "";
  var PQ1EID = "";
  var PQ2EID = "";

  var namelistPath = "/Ch71w3001/FMS/A380/Tools/Generate_Packet/NameList.PDF";

  //  get EID
  try
  {
    var oDoc = app.openDoc({cPath: namelistPath, bHidden: true});

    for(var vCnt = 1; vCnt <= 44; vCnt++)
    {
      vCMName[vCnt]   = oDoc.getField("E" + vCnt + ".Name");
      vEID[vCnt]      = oDoc.getField("E" + vCnt + ".EID");

      if ((vCMName[vCnt] == null) || (vEID[vCnt] == null))
      { break;  }

      if ((vCMName[vCnt].value == "") || (vEID[vCnt].value == ""))
      { break;  }
    }

    for(var i = 1; i < vCMName.length; i++)
    {
      switch(vCMName[i].value)
      {
        case AutName :
          AutEID = vEID[i].value;
          break;

        case ModName :
          ModEID = vEID[i].value;
          break;

        case InsName :
          InsEID = vEID[i].value;
          break;

        case SysName :
          SysEID = vEID[i].value;
          break;

        case PQ1Name :
          PQ1EID = vEID[i].value;
          break;

        case PQ2Name :
          PQ2EID = vEID[i].value;
          break;

        default :
          break;
      }
    }
  }
  catch(e){
      app.alert("Open Cover Name Error!" + e);
  }

  oDoc.closeDoc(true);

  // reply mail for respective user.
  if(identity.loginName == AutEID)
  {
    TOaddr = ModEID;
    CCaddr = InsEID;
    emailMessage = "Hi,\n\n" + "Rework done for the Review packet for SCR " + reviewIDField.value + "\n\n"
                             + "\\" + "\\159.99.234.164\\FMS\\" + faircraft.value + "\\E-review_Packets\\" + TempArea
                             + "\\" + doc.documentFileName + ".\n\n Please check, any other concern please let me know.\n\n"
                             + "Best Regards,\n" + doc.getField("E1.Name").value;
  }
  else
  {
    if(identity.loginName == ModEID)
    {
      TOaddr = AutEID;
      CCaddr = InsEID;
      emailMessage = "Hi,\n\n" + "Review packet for SCR " + reviewIDField.value + " has been reviewed." + "\n\n"
                               + "\\" + "\\159.99.234.164\\FMS\\" + faircraft.value + "\\E-review_Packets\\" + TempArea
                               + "\\" + doc.documentFileName + ".\n\n Please check, any other concern please let me know.\n\n"
                               + "Best Regards,\n" + doc.getField("E2.Name").value;
    }
    else
    {
      TOaddr = AutEID + ";" + ModEID + ";";
      emailMessage = "Hi,\n\n" + "Review packet for SCR " + reviewIDField.value + " has been reviewed." + "\n\n"
                               + "\\" + "\\159.99.234.164\\FMS\\" + faircraft.value + "\\E-review_Packets\\" + TempArea
                               + "\\" + doc.documentFileName + ".\n\n Please check, any other concern please let me know.\n\n"
                               + "Best Regards,\n";
    }
  }

  // Send E-mail
  try{
    app.mailMsg({
                bUI: true,
                cTo: TOaddr,
                cCc: CCaddr,
                cSubject: emailSubject,
                cMsg: emailMessage
               });
  }
  catch(e){
    app.alert("Send Mail error " + e);
  }
}

if(app.viewerVersion >= 7) //if we are working with acrobat 7
{
  console.println("Implementing Acrobat 7 Fix for Return Review Comments");
  app.trustedFunction(returnToAuthor);  //this line of code will ONLY run in acrobat 7
}
