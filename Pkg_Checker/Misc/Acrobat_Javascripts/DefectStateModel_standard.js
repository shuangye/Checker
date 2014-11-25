console.println("FMS Standard Defect State Model Installed - May 16, 2005");
// Standard Defect State Model setup - 
// This code defines the Annotation state models in support of Review Defect Statusing.
// It defines Two models:
//       1. Defect Review State.  This provides a state of defect acceptance.
//       2. Defect Status.  This provides a status of the defect resolution.
//       3. Defect Status (default Adobe Review state is deleted)

// First, delete any old state models,

// Defect type model
try { Collab.removeStateModel("DefectType");    } catch (e) {}
// Qualifier model
try { Collab.removeStateModel("QualifierType"); } catch (e) {}
// Impact model
try { Collab.removeStateModel("Impact");        } catch (e) {}
// Delete Adobe default state model - don't need it anymore
try { Collab.removeStateModel("Review");        } catch (e) {}

// Create the new state models

// Defect Review State model
var myIsDefectType = new Object;
myIsDefectType["None"] = {cUIName: "None"};
myIsDefectType["Accepted"] = {cUIName: "Accepted"};
myIsDefectType["Nondefect"] = {cUIName: "Not a Defect"};
myIsDefectType["Duplicate"] = {cUIName: "Duplicate"};
Collab.addStateModel({cName: "Is Defect State", cUIName: "IsDefectState",oStates: myIsDefectType, Default: "Accepted"});

// Defect Status model
var myResolutionStatusType = new Object;
myResolutionStatusType["None"] = {cUIName: "None"};
myResolutionStatusType["In Work"] = {cUIName: "In Work"};
myResolutionStatusType["Work Completed"] = {cUIName: "Work Completed"};
myResolutionStatusType["Need Additional Rework"] = {cUIName: "Need Additional Rework"};
myResolutionStatusType["Verified Complete"] = {cUIName: "Verified Complete"};
Collab.addStateModel({cName: "Resolution Status", cUIName: "ResolutionStatus",oStates: myResolutionStatusType, Default: "In Work"});

// Defect Classification state model
var myDefectType = new Object;
myDefectType["DR"] = {cUIName: "Driving Requirement"};
myDefectType["FN"] = {cUIName: "Functionality"};
myDefectType["IF"] = {cUIName: "Interface"};
myDefectType["LA"] = {cUIName: "Language"};
myDefectType["LO"] = {cUIName: "Logic"};
myDefectType["MN"] = {cUIName: "Maintainability"};
myDefectType["PF"] = {cUIName: "Performance"};
myDefectType["ST"] = {cUIName: "Standards"};
myDefectType["OT"] = {cUIName: "Other"};
myDefectType["ND"] = {cUIName: "Documentation:NT"};
myDefectType["PD"] = {cUIName: "Documentation:P"};
myDefectType["TE"] = {cUIName: "Incomplete Test Execution:T"};
myDefectType["TI"] = {cUIName: "Incorrect Stubbing:T"};
myDefectType["PR"] = {cUIName: "Review Packet Deficiency:P"};
myDefectType["TS"] = {cUIName: "Structural Coverage:T"};
myDefectType["NS"] = {cUIName: "Structural Coverage:NT"};
myDefectType["TC"] = {cUIName: "Test case:T"};
myDefectType["NC"] = {cUIName: "Test case:NT"};
myDefectType["PG"] = {cUIName: "Test Generation System warnings:P"};
myDefectType["TT"] = {cUIName: "Trace:T"};
myDefectType["NT"] = {cUIName: "Trace:NT"};
myDefectType["PT"] = {cUIName: "Trace:P"};
Collab.addStateModel({cName: "DefectType", cUIName: "Defect Type",oStates: myDefectType});

// Defect Classification state model
var myDefectSeverity = new Object;
myDefectSeverity["Minor"] = {cUIName: "Minor"};
myDefectSeverity["Major"] = {cUIName: "Major"};
Collab.addStateModel({cName: "DefectSeverity", cUIName: "Defect Severity",oStates: myDefectSeverity});

//End of the State Function Modifications

