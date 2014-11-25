console.println("FMS Toolbar Extensions Installed - June 9, 2005");
//toolBar.js
// Add to menu
app.addSubMenu({ cName: "E-Reviews", cParent: "Tools", nPos: 0 })

app.addSubMenu({ cName: "Lock", cParent: "E-Reviews" });
app.addMenuItem({ cName: "All", cParent: "Lock", cExec:"rr_lock(this)"});

app.addSubMenu({ cName: "UnLock", cParent: "E-Reviews" });
app.addMenuItem({ cName: "All", cParent: "UnLock", cExec:"rr_unlock(this)"});

//EDIT 9/27/2006 by Robert Gee, added more new menu options
app.addSubMenu({ cName: "E-mail Packet", cParent: "E-Reviews" });
app.addMenuItem({ cName: "Send For Review", cParent: "E-mail Packet", cExec:"sendForReview(this)" });
app.addMenuItem({ cName: "Return Comments", cParent: "E-mail Packet", cExec:"returnToAuthor(this)" });
app.addSubMenu({ cName: "Update n Check", cParent: "Tools", nPos: 1 });
app.addMenuItem({ cName: "AutoUpdate", cParent: "Update n Check", cExec:"AutoUpdateChecklist(this)" });
app.addMenuItem({ cName: "AutoCheck", cParent: "Update n Check", cExec:"ReviewChecklistCheck(this)" });
