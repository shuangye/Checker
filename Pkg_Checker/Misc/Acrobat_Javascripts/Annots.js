var AcroPlugins = app.plugIns;
var annotsPresent = false;

for (var iAcroPlugins = 0; iAcroPlugins < AcroPlugins.length ; iAcroPlugins++)
	if (AcroPlugins[iAcroPlugins].name.toLowerCase() == "annots")
	{
		annotsPresent = true;
		break;
	}

if (annotsPresent)
{
	var strsToExport = new Array (
		"IDS_SUM_TITLE1",
		"IDS_SUM_TITLE2",
		"IDS_UNNAMED",
		"IDS_SUM_DATE1",
		"IDS_SUM_DATE2",
		"IDS_SUM_AUTHOR1",
		"IDS_SUM_AUTHOR2",
		"IDS_SUM_SUBJ1",
		"IDS_SUM_SUBJ2",
		"IDS_SUM_LABEL1",
		"IDS_SUM_LABEL2",
		"IDS_SUM_PAGE1",
		"IDS_SUM_PAGE2",
		"IDS_SUM_TYPE1",
		"IDS_SUM_TYPE2",
		"IDS_SUM_SEQ1",
		"IDS_SUM_SEQ2",
		"IDS_SUM_NO_ANNOTS1",
		"IDS_SUM_NO_ANNOTS2",
		"IDS_STORE_WEB_DISCUSSIONS",
		"IDS_STORE_DAVFDF",
		"IDS_STORE_FSFDF",
		"IDS_STORE_DATABASE",
		"IDS_STORE_NONE",
		"IDS_PROGRESS_SUMMARIZE",
		"IDS_PROGRESS_SORTING",
		"IDS_PROGRESS_FETCHING",
		"IDS_PROGRESS_FETCHING_BIG",
		"IDS_PROGRESS_ADDING",
		"IDS_PROGRESS_DELETING",
		"IDS_PROGRESS_CHANGING",
		"IDS_ANNOTS_JS_BUILTIN",
		"IDS_DATE_INDETERMINATE",
		"IDS_EMAIL_PLEASE",
		"IDS_EMAIL_BLANKNAME",
		"IDS_EMAIL_VERIFY",
		"IDS_EMAIL_TITLE",
		"IDS_EMAIL_LABEL",
		"IDS_SEND_FOR_REVIEW_SUBJ",
		"IDS_SEND_FOR_REVIEW_MSG",
		"IDS_SEND_FOR_REVIEW_MSG_LABEL",
		"IDS_SEND_FOR_REVIEW",
		"IDS_SEND_FOR_REVIEW_TITLE",
		"IDS_SEND_FOR_REVIEW_TITLE_NONAME",
		"IDS_SEND_FOR_REVIEW_INST1",
		"IDS_SEND_FOR_REVIEW_INST2",
		"IDS_SEND_FOR_REVIEW_GET_ADDRS_CAPTION",
		"IDS_SEND_FOR_REVIEW_VERSION_ERR",
		"IDS_SEND_FOR_REVIEW_CONFIRM_MSG",
		"IDS_SEND_FOR_REVIEW_CONFIRM_TIP",
		"IDS_SEND_FOR_REVIEW_CONFIRM_TITLE",
		"IDS_SEND_FOR_REVIEW_PROBLEM",
        "IDS_SEND_FOR_REVIEW_NO_RECIPIENT",
		"IDS_SEND_COMMENTS_TO_AUTHOR_TITLE",
		"IDS_SEND_COMMENTS_TO_AUTHOR_TO",
		"IDS_SEND_COMMENTS_TO_AUTHOR_SUBJ",
		"IDS_SEND_COMMENTS_TO_AUTHOR_MSG",
		"IDS_SEND_COMMENTS_TO_AUTHOR_MSG_LABEL",
		"IDS_SEND_COMMENTS_TO_AUTHOR_INST1",
		"IDS_SEND_COMMENTS_TO_AUTHOR_INST2",
		"IDS_SEND_COMMENTS_TO_AUTHOR_CONFIRM",
		"IDS_SEND_COMMENTS_TO_AUTHOR",
		"IDS_COLLAB_TITLE",
		"IDS_ACTION_REVIEW",
		"IDS_STATE_NONE",
		"IDS_STATE_ACCEPT",
		"IDS_STATE_REJECT",
		"IDS_STATE_CANCELLED",
		"IDS_STATE_COMPLETED",
		"IDS_ACTION_COLLAB",
		"IDS_STATE_COLLAB_ACTIVE",
		"IDS_STATE_COLLAB_COMPLETED",
		"IDS_ACTION_MARKED",
		"IDS_STATE_MARKED",
		"IDS_STATE_UNMARKED",
		"IDS_SUM_FOOTER1",
		"IDS_SUM_FOOTER2",
		"IDS_SUM_STATUS_TITLE",
		"IDS_SEND_FOR_REVIEW_DOC_IS_DIRTY",
		"IDS_EMAIL_INVALID",
		"IDS_SEND_COMMENTS_ATTACHMENT",
		"IDS_SEND_FOR_REVIEW_ATTACHMENT",
		"IDS_ATTACH_FOR_EMAIL_REVIEW"			//Adobe 9
	);

	for(var nToExport = 0; nToExport < strsToExport.length; nToExport++)
	{
		var strID = strsToExport[nToExport];

		eval(strID + " = " + app.getString("Annots", strID).toSource());
	}

	console.println(IDS_ANNOTS_JS_BUILTIN);

	/* for debugging */
	function debugExcept(e)
	{
		if((typeof app._DEBUG != "undefined") && app._DEBUG)
		  console.println(e)
	}

	/* Sort methods */
	ANSB_None = 0;
	ANSB_Page = 1;
	ANSB_Seq = 2;
	ANSB_Author = 3;
	ANSB_ModDate = 4;
	ANSB_Type = 5;
	ANSB_Subject = 6;

	ANFB_ShouldPrint = 0;
	ANFB_ShouldView = 1;
	ANFB_ShouldEdit = 2;
	ANFB_ShouldAppearInPanel = 3;
	ANFB_ShouldSummarize = 4;
	ANFB_ShouldExport = 5;
	ANFB_ShouldCollaborate = 6;
	ANFB_ShouldNone = 7;

	/* Field to summary functions by property name */
	ANsums =
	[
	/* None */		function(a){return "*None*";},
	/* Page */		function(a){return IDS_SUM_PAGE1+a.doc.getPageLabel(a.page)+IDS_SUM_PAGE2;},
	/* Sequence */	function(a, s){return IDS_SUM_SEQ1+(s ? s : a.seqNum)+IDS_SUM_SEQ2;},
	/* Author */	function(a){return IDS_SUM_AUTHOR1+a.author+IDS_SUM_AUTHOR2;},
	/* ModDate */	function(a){
		var d = a.modDate; 
		return IDS_SUM_DATE1+ (d ? util.printd(2, a.modDate) : IDS_DATE_INDETERMINATE )+IDS_SUM_DATE2;
		},
	/* Type */		function(a){return IDS_SUM_TYPE1+a.uiType+IDS_SUM_TYPE2;},
	/* Subject */	function(a){
		var s = a.subject;
		return s ? IDS_SUM_SUBJ1+s+IDS_SUM_SUBJ2 : "";
		},
	];

	/* Order of summary fields */
	ANsumorder = [ ANSB_Page, ANSB_Author, ANSB_Subject, ANSB_ModDate ];

	/* binary insertion into sorted list */
	function binsert(a, m)
	{
		var nStart = 0, nEnd = a.length - 1;

		while(nStart < nEnd)
		{
			var nMid = Math.floor((nStart + nEnd) / 2);

			if(m.toString() < a[nMid].toString())
				nEnd = nMid - 1;
			else
				nStart = nMid + 1;
		}
		if((nStart < a.length) && (m.toString() >= a[nStart].toString()))
			a.splice(nStart + 1, 0, m);
		else
			a.splice(nStart, 0, m);
	}

	/* perform a worst case n log ( n ) sort with status */
	function isort(a, status)
	{
		var i;
		var aNew = new Array();

		if(status)
		{
			app.thermometer.begin();
			app.thermometer.duration = a.length;
			app.thermometer.text = status;
		}
		for(i = 0; i < a.length; i++)
		{
			if(status)
				app.thermometer.value = i;
			binsert(aNew, a[i]);
		}
		if(status)
			app.thermometer.end();
		return aNew;
	}

    function ANstateful(annot)
	{
	  return annot &&
	    typeof annot.state == "object" &&
	    typeof annot.state.state != "undefined" &&
		annot.state.state;
	}

	function ANsumFlatten(a, m, i, s)
	{
		var result = [];

		if(a)
		{
			if(s)
				/* if we're sorting, sort by creation date */
				a.sort(function(a,b){
					return a.creationDate.getTime() - b.creationDate.getTime();
				});

			for(var n = 0; n < a.length; n++)
			{
				var item = a[n];

				result.push(item); /* push on the item */
				result.push(i); /* the indent level */

                // don't indent if this one is stateful
				var sub = ANsumFlatten(m[item.name], m, i + (ANstateful(item) ? 0 : 1), true);

				for(var j = 0; j < sub.length; j++)
					result.push(sub[j]); /* and the sub stuff */
			}
		}
		return result;
	}

    function ANsummAnnot(annot, scale, doc, r, p, seqNum)
	{
	  var assoc = true;

	  r.size = 1 * scale;

   	  if(seqNum && !annot.inReplyTo)
		r.writeText(ANsums[ANSB_Seq](annot, seqNum), doc, annot.page, annot.rect, false, "" + seqNum, annot.containedPopupHeelPoint);

   	  for(j = 0; j < ANsumorder.length; j++)
   		  if(ANsumorder[j] != p)
   		  {
   			  var s = (ANsums[ANsumorder[j]])(annot);
   
   			  if(s)
   			  {
  			    if(assoc)
  				{
  				  assoc = false;
   				  if(!annot.inReplyTo && !seqNum)
					r.writeText(s, doc, annot.page, annot.rect);
   				  else
   					r.writeText(s, doc, annot.page);
  				}
  				else
  				  r.writeText(s, doc, annot.page);
   			  }
   		  }

	  var contents = annot.richContents;

	  if(contents)
	  {
		  r.style = "DefaultNoteText";
		  r.indent({nAmount: 16 * scale, oIcon: annot.uiIcon, color: annot.strokeColor});
		  r.writeText(contents, doc, annot.page);
		  r.writeText(" ", doc, annot.page);
		  r.outdent(16 * scale);
	  }
	  else
	  {
	      r.indent({nAmount: 16 * scale, oIcon: annot.uiIcon, color: annot.strokeColor});
		  r.writeText(" ", doc, annot.page);
		  r.writeText(" ", doc, annot.page);
		  r.outdent(16 * scale);
	  }

	  // Add the state info
	  var models = Collab.getStateModels(false);
	  for(i = 0; i < models.length; i++)
	  {
		  var states = annot.getStateInModel(models[i].name);

		  if(states.length > 0)
		  {
		  	r.writeText("" + IDS_SUM_STATUS_TITLE, doc, annot.page);

		  	for(j = 0; j < states.length; j++)
		  	{
				var d = util.printd(2, states[j].modDate);
				var s = states[j].state;
				var a = states[j].author;

		  		r.writeText(a + " " + s + " " + d, doc, annot.page);
			}
		  }
	  }
	}

	function ANsummarize(doc, title, p, r, dest, fs, print, twoUp, useSeqNum, scale, noAssocDoc, filter)
	{	/* Summarize annotations sorted primarily by property p */
		if(!scale)
			scale = 1;

		var thermoUp = true;
		app.thermometer.begin();

		try
		{
			app.thermometer.text = IDS_PROGRESS_SUMMARIZE;

			if(!ANsums[p])
				p = ANSB_Page;
			if(!title)
				title = IDS_UNNAMED;

			/* make sure we have all annots */
			this.syncAnnotScan();

			/* Get all summarizable annots on all pages sorted in the given manner */
			var a = [];

			for(var n = 0; n < xfa.host.numPages; n++)	//12-Nov
			{
			  var a2 = doc.getAnnots(n, p, r, typeof filter == "undefined" ? ANFB_ShouldSummarize : filter);

			  for(var n2 = 0; a2 && n2 < a2.length; n2++)
			  {
				  // If it's hidden, or a state annot (or both) don't show it
				  var curAnnot = a2[n2];

				  if(!curAnnot.hidden && (!curAnnot.state || !curAnnot.state.state))
				    a.push(curAnnot);
			  }
			}

			if(a && a.length > 0) /* Put in thread order */ 
			{
				app.thermometer.duration = a.length * 4;

				var t = {};

				for(var n = 0; n < a.length; n++, app.thermometer.value = n * 2)
				{
					var item = a[n];

					if(!t[item.inReplyTo])
						t[item.inReplyTo] = [ item ];
					else
						t[item.inReplyTo].push(item);
				}


				/* don't sort the top level 'cuz it's already sorted */
				a = ANsumFlatten(t[""], t, 0, false);

				/* make the indents differential */
				for(var j = a.length - 1; j > 2; j -= 2)
					a[j] -= a[j - 2];
			}

			var t;
			var r = new Report();
			var assocDoc = noAssocDoc ? null : doc;

			r.ignoreAnnotLayers = (filter == ANFB_ShouldNone);
			r.joinAssocs = twoUp;
			r.style = "NoteTitle";
			r.size = 3 * scale;
			t = IDS_SUM_TITLE1 + title + IDS_SUM_TITLE2;
			r.writeText(t);
			r.divide(3.5 * scale);

			var i, j, contents;
			var oldHeading;
			var lastAnnotPage;
			var curFooterText = "";
			var seqNum = 1;

			if(a && a.length > 0)
			{
			  for(i = 0; i < a.length; i += 2)
			  {
				app.thermometer.value = a.length + i;

				// update the indent level
				var ind = a[i + 1];
				var curAnnot = a[i];
				var footerText = "";

				// update the footer
				  r.style = "NoteTitle";
				  r.size = 2 * scale;
				if((typeof lastAnnotPage != "undefined") && (curAnnot.page != lastAnnotPage))
				{
					footerText = curFooterText = "";
					r.setFooterText();
					r.breakPage();
					seqNum = 1;
				}

				footerText = "\r" + IDS_SUM_FOOTER1 + doc.getPageLabel(curAnnot.page) + IDS_SUM_FOOTER2; 

				if(footerText != curFooterText)
				{
					curFooterText = footerText;
					r.setFooterText(footerText);
				}

				for(; ind < 0; ind++)
					{ r.outdent(16 * scale); r.outdent(16 * scale); }
				for(; ind > 0; ind--)
					{ r.indent(16 * scale); r.indent(16 * scale); }

				// maybe do the heading
				  var heading = (ANsums[p])(curAnnot);
				  if(heading != oldHeading)
				  {
					if(typeof oldHeading != "undefined")
					  r.writeText(" ");
					r.writeText(heading, assocDoc, curAnnot.page);
					oldHeading = heading;
					r.divide();
				  }

				if(useSeqNum)
					ANsummAnnot(curAnnot, scale, assocDoc, r, p, seqNum);
				else
					ANsummAnnot(curAnnot, scale, assocDoc, r, p);

				if(!curAnnot.inReplyTo)
					seqNum++;

				  r.divide(1 * scale);
				  lastAnnotPage = curAnnot.page;
			  }
			  r.setFooterText();
			}
			else
			  r.writeText(IDS_SUM_NO_ANNOTS1 + title + IDS_SUM_NO_ANNOTS2);

			if(thermoUp)
			{
				thermoUp = false;
				app.thermometer.end();
			}

			if (typeof dest != "undefined")
				r.save(dest, fs);
			else if(print)
				r.print();
			else
				r.open(t);
		}
		catch(e)
		{
			app.alert({cMsg: e["message"], oDoc: doc});
		}
		if(thermoUp)
			app.thermometer.end();

		return a ? a.length / 2 : 0;
	}
}

if(typeof Collab != "undefined")
{
	/* flags used by collaboration
	*/
	CBFNiceTableName = 1;
	CBFNiceDBName = 2;
	CBFDBPerDoc = 4;

	function CBgetTableDesc(doc, author)
	{
	  var frag = Collab.URL2PathFragment(doc.URL);
	  var DBName;
	  var tableName;

	  if(doc.collabDBFlags & CBFDBPerDoc)
	  {
		DBName = frag;
		tableName = author;
	  }
	  else
	  {
		DBName = "";
		tableName = frag;
	  }

	  if(doc.collabDBFlags & CBFNiceTableName)
		tableName = Collab.hashString(tableName);
	  if(doc.collabDBFlags & CBFNiceDBName)
		DBName = Collab.hashString(DBName);
	  return {DBName: DBName ? doc.collabDBRoot + DBName : doc.collabDBRoot,
		tableName: tableName,
		URL: doc.URL,
		user: author,
		flags: doc.collabDBFlags};
	}

	function CBgetTableConnect(desc)
	{
	  var e;

	  try
	  {
		var conn = ADBC.newConnection(desc.DBName);
		var stmt = conn.newStatement();

		return {conn: conn,
		  stmt: stmt,
		  tableName: desc.tableName,
		  user: desc.user,
		  flags: desc.flags};
	  }
	  catch(e) { debugExcept(e); return false; }
	}

	function CBgetInfo(conn, name)
	{
	  var e;

	  try
	  {
		conn.stmt.execute("select CONTENTS from \"" + conn.tableName + "\" where AUTHOR like ?;",
		  "~" + name + "~");
		conn.stmt.nextRow();
		return conn.stmt.getColumn("CONTENTS").value;
	  }
	  catch(e) { debugExcept(e); return false; }
	}

	function CBsetInfo(conn, name, value)
	{
	  var e;

	  /* add the field */
	  try { return conn.stmt.execute("insert into \"" + conn.tableName + "\" (AUTHOR, CONTENTS) values (?, ?);",
		  "~" + name + "~",
		  value); }
	  catch(e) { debugExcept(e); return false; }
	}

	function CBcreateTable(desc)
	{
	  var e;

	  try
	  {
		var conn = ADBC.newConnection(desc.DBName);
		var stmt = conn ? conn.newStatement() : null;

		/* come up with the SQL query to do it */
		var sql1 = "create table \"" + desc.tableName + "\" (AUTHOR varchar(64), PAGE integer, NAME varchar(64), CONTENTS text, DATA image);";
		var sql2 = "create table \"" + desc.tableName + "\" (AUTHOR varchar(64), PAGE integer, NAME varchar(64), CONTENTS clob, DATA blob);";

		var conn = {conn: conn,
		  stmt: stmt,
		  tableName: desc.tableName,
		  user: desc.user,
		  flags: desc.flags};

		// first try...
		try
		{
		  stmt.execute(sql1);
		} catch(e) { debugExcept(e); }
		// second try...
		try
		{
		  stmt.execute(sql2);
		} catch(e) { debugExcept(e); }
		// these will throw if the table wasn't created
		CBsetInfo(conn, "URL", desc.URL);
		CBsetInfo(conn, "creator", desc.user);
		return conn;
	  }
	  /* we failed... */
	  catch(e) { debugExcept(e); return false; }
	}

	function CBconnect(desc, bDoNotCreate)
	{
	  var conn = CBgetTableConnect(desc);
	  var e;

	  /* if we can't get the URL from it, it doesn't exist */
	  if(!CBgetInfo(conn, "URL"))
	  {
		if (!bDoNotCreate)
		  conn = CBcreateTable(desc);
		else
		  return false;
	  }

	  /* here it is! */
	  return conn;
	}

	/* mapping of annot types to data properties */
	CBannotdata =
	{
		FileAttachment:	function(p) { return "FSCosObj" },
		Sound:			function(p) { return "SCosObj" },
		Stamp:			function(p) { return /^\#/.exec(p.AP) ? "APCosObj" : false }
	};

	/* returns the data fork for an annot */
	function CBannotData(annot)
	{
	  var prop = CBannotdata[annot.type];
	  if(prop != null)
	  {
	  	prop = prop(annot);
	  	var stm = prop ? Collab.cosObj2Stream(annot[prop]) : null;

	  	if(stm && typeof ADBC != "undefined") 
			stm.type = ADBC.SQLT_LONGVARBINARY;
	  	return stm;
	  }
	}

	/* sets the data fork of an annot */
	function CBannotSetData(annot, data)
	{
	  var prop = CBannotdata[annot.type];
	  if(prop)
	  {
	  	prop = prop(annot);
	  	if(prop) annot[prop] = data;
	  }
	}


	/* recursive function that deletes a reply chain */
	function CBDeleteReplyChain(disc)
	{
		var replies = Discussions.getDiscussions(disc);

		if (replies && (replies.length == 1))
		{
			var currentReply = replies[0];
			var looper = 1;
			while (looper)
			{
				/*
				** There better only be one reply 
				*/
				var saveChild = Discussions.getDiscussions(currentReply);

	//			console.println("Delete reply");
				currentReply.Delete();

				if (saveChild && (saveChild.length == 1))
					currentReply = saveChild[0];
				else
					looper = 0;
			}
		}

	}

	/* gets the reply chain, stuffs it in a stream */
	/* and then puts it in the annot */
	function CBGetReplyChain(dstAnnot, discussion)
	{
		var discList = Discussions.getDiscussions(discussion);

		var cos = Collab.newWrStreamToCosObj();

		var data = 0;
		while (discList && (discList.length > 0))
		{
			data = 1;
			cos.write(discList[0].Text);
			//console.println("Write to cos stream " + discList[0].Text.length + " characters");

			discList = Discussions.getDiscussions(discList[0]);
		}

		if (data == 1)
			CBannotSetData(dstAnnot, cos.getCosObj());
	}

	/* get the stream and puts the data as replies */
	function CBPutReplyChain(discussion, bookmark, srcAnnot)
	{
		var cosStream = CBannotData(srcAnnot);

		if(cosStream)
		{
			var s = cosStream.read(Collab.wdBlockSize);

			while (discussion && (s.length > 0))
			{
				discussion = Discussions.addDiscussion(discussion, "Data", s, bookmark);

				s = null;
			
				s = cosStream.read(Collab.wdBlockSize);
			}
		}
	}

	/* ADBC based annot enumerator constructor
	*/
	function ADBCAnnotEnumerator(parent, sorted)
	{
	  /* store away parameters */
	  this.parent = parent;
	  this.sorted = sorted;
	  /* add enumeration method */
	  this.next = function()
	  {
		var e;

		try
		{
		  if(!this.conn)
		  {
			this.conn = CBconnect(this.parent.desc, true);
			this.conn.stmt.execute("select CONTENTS from \"" + this.parent.desc.tableName + "\" where AUTHOR not like '~%~'" +
			  (this.sorted ? " order by PAGE, NAME;" : ";"));
		  }
		  this.conn.stmt.nextRow();
		  return eval(this.conn.stmt.getColumn("CONTENTS").value);
		}
		catch(e) { debugExcept(e); return false; }
	  }
	}

	function CBStrToLongColumnThing(s)
	{
	  return { type: ADBC.SQLT_LONGVARCHAR, value: s, size: s.length };
	}

	/* ADBC based annot store constructor
	*/
	function ADBCAnnotStore(doc, user)
	{
	  this.desc = CBgetTableDesc(doc, user);
	  this.enumerate = function(sorted)
	  {
		return new ADBCAnnotEnumerator(this, sorted);
	  }
	  this.complete = function(toComplete)
	  {
		var i;
		var conn = CBconnect(this.desc,true);

		if (conn) 
			{
		  for(i = 0; toComplete && i < toComplete.length; i++)
		  {
			if(CBannotdata[toComplete[i].type])
			{
			  var e;
  
			  try
			  {
				conn.stmt.execute("select DATA from \"" + this.desc.tableName + "\" where PAGE = ? and NAME like ?;",
				  toComplete[i].page, toComplete[i].name);
				conn.stmt.nextRow();
				var cos = Collab.newWrStreamToCosObj();

				conn.stmt.getColumn("DATA", ADBC.Binary | ADBC.Stream, cos);
				CBannotSetData(toComplete[i], cos.getCosObj());
			  }
			  catch(e) 
			  { 
			  	debugExcept(e);
				return false;
			  }
			}
			  }
		} else return false;
		return true;
	  }
	  this.update = function(toDelete, toAdd, toUpdate)
	  {
		var i;
		var e;
		var conn = CBconnect(this.desc);
		if(conn == null) return false;

		for(i = 0; toDelete && i < toDelete.length; i++)
		{
		  try
		  {
			conn.stmt.execute("delete from \"" + this.desc.tableName + "\" where PAGE = ? and NAME like ?;",
			  toDelete[i].page, toDelete[i].name);
		  }
		  catch(e) 
		  { 
		  	debugExcept(e);
			return false;
		  }
		}
		for(i = 0; toAdd && i < toAdd.length; i++)
		{
		  try
		  {
			conn.stmt.execute("insert into \"" + this.desc.tableName + "\" (AUTHOR, PAGE, NAME, CONTENTS, DATA) values (?, ?, ?, ?, ?);",
			  toAdd[i].author, toAdd[i].page, toAdd[i].name, CBStrToLongColumnThing(toAdd[i].toSource()), CBannotData(toAdd[i]));
		  }
		  catch(e) 
		  { 
		  	debugExcept(e);
			return false;
		  }
		}
		for(i = 0; toUpdate&& i < toUpdate.length; i++)
		{
		  try
		  {  	
			conn.stmt.execute("update \"" + this.desc.tableName + "\" set CONTENTS = ?, DATA = ? where PAGE = ? and NAME like ?;",
			  CBStrToLongColumnThing(toUpdate[i].toSource()), CBannotData(toUpdate[i]), toUpdate[i].page, toUpdate[i].name);
		  }
		  catch(e) 
		  { 
		  	debugExcept(e);
			return false;
		  }
		}
		return true;
	  }
	}

	/* Munge an URL such that Web Discussions won't put our data in the discussions pane
	*/
	function WDmungeURL(url)
	{
		return url + "/ACData";
	}

	/* Web discussions based annot enumerator constructor
	*/
	function WDAnnotEnumerator(parent, sorted)
	{
	//  console.println("WDAnnotEnumerator(): Begin");

	  this.parent = parent;
	  this.sorted = sorted;
	  this.next = function()
	  {
		try
		{
			app.thermometer.begin();
			app.thermometer.text = IDS_PROGRESS_FETCHING;

			if(!this.discussions)
			{
		  		this.discussions = Discussions.getDiscussions(WDmungeURL(this.parent.doc.URL));

				// always sort as our completion callback relies on a sorted list
		  		if(this.discussions)
		  		{
					this.discussions = isort(this.discussions, IDS_PROGRESS_SORTING);
					app.thermometer.duration = this.discussions.length;
		  		}
		  		this.index = 0;
			}

			// skip non-Acro discussions
			while(this.discussions && this.index < this.discussions.length && this.discussions[this.index] == "[Discussion]")
		  		app.thermometer.value = this.index++;

		  	app.thermometer.end();

			if(!this.discussions || this.index >= this.discussions.length)
			{
		  		return false;
			}
			return eval(this.discussions[this.index++].Text);
	   	}

		catch(e)
		{
	  		debugExcept(e);
		  	app.thermometer.end();
			return false;
		}
	  } // next
	}

	/* Web discussion based annot store constructor
	*/
	function WDAnnotStore(doc, user)
	{
	//  console.println("WDAnnotStore(): Begin");

	  this.doc = doc;
	  this.user = user;
	  this.enumerate = function(sorted)
	  {
	//	console.println("WDAnnotStore.enumerate(): Begin");
		return new WDAnnotEnumerator(this, sorted);
	  }
	  this.complete = function(toComplete)
	  {
	//	console.show();
	//	console.println("WDAnnotStore.toComplete(): Begin");

		var i,j;
	//	console.println("get discussions for "+WDmungeURL(this.doc.URL));
		var discussions = Discussions.getDiscussions(WDmungeURL(this.doc.URL));

		if (discussions == null) return false;

		if (discussions.length) 
		{
			// sort them to perform fast searches
			// JS sort is a SLOW qsort... use our worst case N log ( N )
			discussions = isort(discussions, IDS_PROGRESS_SORTING);
 
			try
			{
				app.thermometer.begin();
				app.thermometer.text = IDS_PROGRESS_FETCHING_BIG;
				app.thermometer.duration = toComplete.length;

				for(i = 0, j = 0; discussions && (i < toComplete.length) && (j < discussions.length); app.thermometer.value = ++i)
				{
					// create a string that'll look like the corresponding discussion
					var discString = Discussions.makeDiscussionString(toComplete[i].page, toComplete[i].name);

					// keep skipping annots while they are "less" than the current one
					while(discString > discussions[j])
						j++;

					// if we found it
					if(discString == discussions[j])
					{
						//console.println("found it - Annot to Complete " + i + " is in discussion slot " + j);
						//console.println("subject "+discussions[j].Subject);

						/*
						** We found the discussion, now gather replys which will
						** contain the "data" for the stream
						*/
						if (CBannotdata[toComplete[i].type])
							CBGetReplyChain(toComplete[i], discussions[j]);

					}

				}
				app.thermometer.end();
			}
			catch(e)
			{
	  			debugExcept(e);
				app.thermometer.end();
				return false;
			}
		}
		return true;
	  }
	  this.update = function(toDelete, toAdd, toUpdate)
	  {
	  	var result = true;

	//	console.println("WDAnnotStore.update(): Begin");

		// get the list of discussions
	//	console.println("WDAnnotStore.update(): get discussions "+WDmungeURL(this.doc.URL();
		var discussions = Discussions.getDiscussions(WDmungeURL(this.doc.URL));
		var i, j;

		// if we got any...
		if(discussions && discussions.length)
		{
	//		console.println("WDAnnotStore.update(): got some " + discussions.length);
			// sort them to perform fast searches
			discussions = isort(discussions, IDS_PROGRESS_SORTING);

			// if we've got any to update
			if(toUpdate && toUpdate.length)
			{
				try
				{
					app.thermometer.begin();
					app.thermometer.text = IDS_PROGRESS_CHANGING;
					app.thermometer.duration = toUpdate.length;

					for(i = 0, j = 0; i < toUpdate.length && j < discussions.length; app.thermometer.value = ++i)
					{
				  		// create a string that'll look like 
						// the corresponding discussion
				  		var discString = Discussions.makeDiscussionString(toUpdate[i].page, toUpdate[i].name);

				  		// keep skipping annots while they are 
						// "less" than the current one
				  		while(discString > discussions[j]) j++;

				  		// if we found it
				  		if(discString == discussions[j])
				  		{
							// then update it!
							CBDeleteReplyChain(discussions[j]);
							discussions[j].Delete();

							var bookmark = Discussions.makeBookmark(toUpdate[i].page, toUpdate[i].name);
							discussions[j] = Discussions.addDiscussion(WDmungeURL(this.doc.URL), "Markup", toUpdate[i].toSource(), bookmark);
							CBPutReplyChain(discussions[j], bookmark, toUpdate[i]);
							j++;
				  		}
					}
					app.thermometer.end();
				}

				catch(e)
				{
					app.thermometer.end();
					return false;
				}
			}

			// delete is just like update
			if(toDelete && toDelete.length) 
			{
				try
				{
					app.thermometer.begin();
					app.thermometer.text = IDS_PROGRESS_DELETING;
					app.thermometer.duration = toDelete.length;
					for(i = 0, j = 0; i < toDelete.length && j < discussions.length; app.thermometer.value = ++i)
					{
				  		var discString = Discussions.makeDiscussionString(toDelete[i].page, toDelete[i].name);

				  		while(discString > discussions[j])
							j++;

				  		if(discString == discussions[j])
				  		{
							CBDeleteReplyChain(discussions[j]);
							discussions[j].Delete();
							j++;
				  		}
					}
					app.thermometer.end();
				}

				catch(e)
				{
					app.thermometer.end();
					return false;
				}
			}
		}
		if(toAdd && toAdd.length)
		{
			try
			{
				app.thermometer.begin();
				app.thermometer.text = IDS_PROGRESS_ADDING;
				app.thermometer.duration = toAdd.length;

				for(i = 0; toAdd && i < toAdd.length; app.thermometer.value = ++i)
				{
			  		var bookmark = Discussions.makeBookmark(toAdd[i].page, toAdd[i].name);

			  		var discussion = Discussions.addDiscussion(WDmungeURL(this.doc.URL), "Markup", toAdd[i].toSource(), bookmark);

					if(discussion == null)
					{
						result = false;
						break;
					}

			  		if (CBannotdata[toAdd[i].type])
					{
						CBPutReplyChain(discussion, bookmark, toAdd[i]);
					}

				}
				app.thermometer.end();
			}

			catch(e)
			{
				app.thermometer.end();
				return false;
			}

		}
		return result;
	  }
	}

	/* Set up default annot stores */
	Collab.addAnnotStore("NONE", IDS_STORE_NONE,
		{create: function(doc, user, settings){ return null; }});
	Collab.setStoreNoSettings("NONE", true);
	if(typeof Discussions != "undefined")
	{
	  Collab.addAnnotStore("WD", IDS_STORE_WEB_DISCUSSIONS,
			{create: function(doc, user, settings){ return new WDAnnotStore(doc, user); }});
		Collab.setStoreNoSettings("WD", true);
	}
	if(typeof ADBC != "undefined")
		Collab.addAnnotStore("DB", IDS_STORE_DATABASE,
			{create: function(doc, user, settings){ doc.collabDBRoot = settings; doc.collabDBFlags = CBFNiceTableName; return (settings && settings != "") ? new ADBCAnnotStore(doc, user) : null; }});
	Collab.addAnnotStore("DAVFDF", IDS_STORE_DAVFDF,
		{create: function(doc, user, settings){ return (settings && settings != "") ? new FSAnnotStore(doc, user, settings + doc.Collab.docID + "/", "CHTTP") : null; }});
	Collab.addAnnotStore("FSFDF", IDS_STORE_FSFDF,
		{create: function(doc, user, settings){ return (settings && settings != "") ? new FSAnnotStore(doc, user, settings + doc.Collab.docID + "/") : null; }});
	Collab.setStoreFSBased("FSFDF", true);

	// Web Discussion data block size
	Collab.wdBlockSize = 16384;

	// Add default state handlers -- this should go in a seperate file.
	Collab.addStateModel
	({
		cName: "Review", 
		cUIName: IDS_ACTION_REVIEW, 
		oStates:
		{ 
			"None": IDS_STATE_NONE,
			"Accepted":
			{
				cUIName: IDS_STATE_ACCEPT,
				cIconName: "C_Accept_Md_N.png"
			},
			"Rejected": 
			{
				cUIName: IDS_STATE_REJECT,
				cIconName: "C_Reject_Md_N.png"
			},
			"Cancelled": 
			{
				cUIName: IDS_STATE_CANCELLED,
				cIconName: "C_Cancel_Md_N.png"
			},
			"Completed": 
			{
				cUIName: IDS_STATE_COMPLETED,
				cIconName: "C_Complete_Md_N.png"
			}
		},
		cDefault: "None"
	});

	Collab.addStateModel
	({
		cName: "CollabStatus", 
		cUIName: IDS_ACTION_COLLAB,
		oStates:
		{
			"Modified": IDS_STATE_COLLAB_ACTIVE,
			"Completed": IDS_STATE_COLLAB_COMPLETED
		},
		bHidden: true,
		cDefault: "Modified"
	});

	Collab.addStateModel
	({
		cName: "Marked", 
		cUIName: IDS_ACTION_MARKED,
		oStates:
		{
			"Marked": IDS_STATE_MARKED,
			"Unmarked": IDS_STATE_UNMARKED
		},
		cDefault: "Unmarked",
		bHidden: true,
		bHistory: false
	});
}

/* E-mail ad-hoc workflow stuff */

// Send the current document out for review
function ANSendForReview(doc)
{
	if(!doc.dirty || app.alert({cMsg: IDS_SEND_FOR_REVIEW_DOC_IS_DIRTY, cTitle: IDS_SEND_FOR_REVIEW_TITLE_NONAME, nType: 2, nIcon: 1, oDoc: doc}) == 4) //19Nov09
	{
		// is there a valid e-mail address attached to the doc?
		var eaddr = identity.email;
		var explicitlyEntered = false;

		// Make sure the document gets saved if it's readonly
		if(doc.dirty && !doc.requestPermission(permission.document, permission.remove))
		{
			app.execMenuItem("Save");
			if(doc.dirty)
				return 0; // Cancelled
		}

		// if not, try to get one
		if(!eaddr)
		{
			var emailRE = /^([a-zA-Z0-9_\-\.\/]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;

			// pop the question
			do
			{
				eaddr = app.response({	cQuestion: IDS_EMAIL_PLEASE, 
										cTitle: IDS_EMAIL_TITLE, 
										cDefault: IDS_EMAIL_BLANKNAME, 
										cLabel: IDS_EMAIL_LABEL});
				if(eaddr != null && !eaddr.match(emailRE))
					app.alert({cMsg: IDS_EMAIL_INVALID, cTitle: IDS_SEND_FOR_REVIEW_TITLE_NONAME, nIcon: 1, oDoc: doc});
			} while(eaddr != null && !eaddr.match(emailRE));

			// got a response?	pop it into the author info field
			if(eaddr != null)
				identity.email = eaddr;
		}

		// shall we?
		var name;		
		var match = doc.path.match(/\/([^\/]*$)/);

		if (match && match.length == 2)
			name = match[1];

		if(!name)
			name = doc.path;

		// plop it into a localized string
		name = name.replace(/\.pdf\w*$/i, "");
		name = IDS_SEND_FOR_REVIEW_ATTACHMENT.replace(/\%pdfbase\%/g, name);

		if(eaddr != null)
		{
			var result;
			var info;
			var firstTime = true;
			var keepTrying = true;
			var startDate;

			//
			// If the submitForm fails, allow the user to correct the situation by 
			// bringing them back to our email dialog.  To get out of this while 
			// loop either a successful email must occur or the user must cancel
			// the dialog. 
			//
			while (keepTrying)
			{
				if (firstTime)
				{
					info = doc.Collab.collectEmailInfo({
						subj: IDS_SEND_FOR_REVIEW_SUBJ, 
						msg: IDS_SEND_FOR_REVIEW_MSG,
						inst1: IDS_SEND_FOR_REVIEW_INST1,
						inst2: IDS_SEND_FOR_REVIEW_INST2,
						title: IDS_SEND_FOR_REVIEW_TITLE,
						attach: name,
						reviewStatusDisabled: true,
						helpID: "Dlg_SendEmailReview",
						cCaption: IDS_SEND_FOR_REVIEW_GET_ADDRS_CAPTION, 
						msgLabel: IDS_SEND_FOR_REVIEW_MSG_LABEL
						});
					firstTime = false;

				} else {
					info = doc.Collab.collectEmailInfo({
						to: info[0],
						cc: info[1],
						bcc: info[2],
						subj: info[3],
						msg: info[4],
						inst1: IDS_SEND_FOR_REVIEW_INST1,
						inst2: IDS_SEND_FOR_REVIEW_INST2,
						title: IDS_SEND_FOR_REVIEW_TITLE,
						attach: name,
						reviewStatusDisabled: true,
						helpID: "Dlg_SendEmailReview",
						cCaption: IDS_SEND_FOR_REVIEW_GET_ADDRS_CAPTION, 
						msgLabel: IDS_SEND_FOR_REVIEW_MSG_LABEL
						});
				}

				if (info)
				{
					startDate = new Date();
					var url = "mailto:" + escape(info[0]) + "?" +
						"cc=" + escape(info[1]) + "&" +
						"bcc=" + escape(info[2]) + "&" +
						"subject=" + escape(info[3]) + "&" +
						"body=" + escape(info[4]) + "&" +
						"ui=false";

					var deadSource = info[5] ? ", " + info[5].toSource() : "";
					var script = "if(app.viewerType.match(\"Exchange\") == null || app.viewerVersion < 6) { app.alert({cMsg: \"" + IDS_SEND_FOR_REVIEW_VERSION_ERR + "\", oDoc: doc}) } else { Collab.registerReview(this, " + eaddr.toSource() + ", \"\", " + info[0].toSource() + ", " + info[1].toSource() + ", \"\", " + startDate.toSource() + deadSource + ");}";

					if (app.platform == "WIN")
					{
						if((typeof info[0] == "undefined") || info[0].length == 0)
						{
							app.alert({cMsg: IDS_SEND_FOR_REVIEW_NO_RECIPIENT, oDoc: doc});
							return 0; // Cancelled
						}
					}
		
					var e;
					try
					{
						keepTrying = false;
						result = doc.submitForm({cURL: url, bEmbedForm: true, oJavaScript: { After: script } });
					}
					catch(e)
					{
						app.alert({cMsg: e["message"], oDoc: doc});
						keepTrying = true;
//						app.alert({cMsg: IDS_SEND_FOR_REVIEW_PROBLEM, oDoc: doc});
//						return 0; // Cancelled
					}
				}
				else
					keepTrying = false;
			}

			// Check the return value: submitForm can fail but not throw 
			// if it popped it's own UI.
			if(!result) return 0; // Cancelled

			var fdfName = name.replace(/\.pdf\w*$/i,"\.fdf");
			var alertText = IDS_SEND_FOR_REVIEW_CONFIRM_MSG.replace(/\%docname\%/g, fdfName);
			if (doc.Collab.isOutlook)
			{
				doc.Collab.alertWithHelp(
					{ cMsg: alertText,
					cTip: IDS_SEND_FOR_REVIEW_CONFIRM_TIP,
					cTitle: IDS_SEND_FOR_REVIEW_CONFIRM_TITLE,
					cPref: "Annots:OutgoingEmailNotification"}
				  );
			}

			// note that only the first part of this message is displayed.  If the full text
			// is displayed there is a bug in the alert and the OK button is not located 
			// properly and cannot get focus and thus the dialog cannot be dismissed.
			if(info[5])
				Collab.registerReview(doc, null, "", info[0], info[1], info[2], startDate, info[5]);
			else
				Collab.registerReview(doc, null, "", info[0], info[1], info[2], startDate);

			return 1; // Success

		} else return 0; // Cancelled - if (eaddr is not NULL)
	} else return 0; // Cancelled - if (!docIsDirty)
  return -1; // Failed
}

// Should send for review be enabled?
function ANSendForReviewEnabled(doc)
{
  //event.rc = doc && !doc.external && doc.requestPermission(permission.annot, permission.canExport) && doc.requestPermission(permission.annot, permission.modify);
  event.rc = true;
}

function ANSendCommentsToAuthor(doc)
{
	var eaddr = doc.Collab.initiatorEmail;
	var name = doc.info.title;  // first attempt: title info

	// second attempt: last component of path
	if(!name)
	{
		var match = doc.path.match(/\/([^\/]*$)/);

		if(match && match.length == 2)
		name = match[1];
	}

	// third attempt: path
	if(!name)
		name = doc.path;

	var attachname;
	var amatch = doc.path.match(/\/([^\/]*$)/);
	if(amatch && amatch.length == 2)
		attachname = amatch[1];
	if(!attachname)
		attachname = doc.path;
	var fdfName = attachname.replace(/\.pdf\w*$/i,"\.fdf");

	var dlgTitle = IDS_SEND_COMMENTS_TO_AUTHOR_TITLE.replace(/\{docname\}/g, name);
	var subject = IDS_SEND_COMMENTS_TO_AUTHOR_SUBJ.replace(/\{title\}/g, name);
	var mesg = IDS_SEND_COMMENTS_TO_AUTHOR_MSG.replace(/\{title\}/g, name);
	var firstTime = true;
	var keepTrying = true;
	var info;
	var result;
  
	fdfName = fdfName.replace(/\.fdf\w*$/i, "");
 	fdfName = IDS_SEND_COMMENTS_ATTACHMENT.replace(/\%pdfbase\%/g, fdfName);

	//
	// Keep trying to send to send email until the user cancels the email 
	// review dialog or we successfully send the email
	//
	while (keepTrying)
	{
		if (firstTime)
		{
			info = doc.Collab.collectEmailInfo({
				to: eaddr,
				subj: subject,
				msg: mesg,
				title: dlgTitle,
				attach: fdfName,
				inst1: IDS_SEND_COMMENTS_TO_AUTHOR_INST1,
				inst2: IDS_SEND_COMMENTS_TO_AUTHOR_INST2,
				toDisabled: true,
				ccDisabled: true,
				bccDisabled: true,
				deadDateDisabled: true,
				reviewStatusDisabled: false,
				helpID: "Dlg_ReplyReview",
				cCaption: IDS_SEND_FOR_REVIEW_GET_ADDRS_CAPTION,
				msgLabel: IDS_SEND_COMMENTS_TO_AUTHOR_MSG_LABEL
				});
			firstTime = false;
		} else {

			info = doc.Collab.collectEmailInfo({
				to: info[0],
				cc: info[1],
				bcc: info[2],
				subj: info[3],
				msg: info[4],
				title: dlgTitle,
				attach: fdfName,
				inst1: IDS_SEND_COMMENTS_TO_AUTHOR_INST1,
				inst2: IDS_SEND_COMMENTS_TO_AUTHOR_INST2,
				toDisabled: true,
				ccDisabled: true,
				bccDisabled: true,
				deadDateDisabled: true,
				reviewStatusDisabled: false,
				helpID: "Dlg_ReplyReview",
				cCaption: IDS_SEND_FOR_REVIEW_GET_ADDRS_CAPTION,
				msgLabel: IDS_SEND_COMMENTS_TO_AUTHOR_MSG_LABEL
				});
		}

		if(info)
		{
			var url = "mailto:" + escape(info[0]) + "?" +
				"cc=" + escape(info[1]) + "&" +
				"bcc=" + escape(info[2]) + "&" +
				"subject=" + escape(info[3]) + "&" +
				"body=" + escape(info[4]) + "&" +
				"ui=false";

			var e;
			try
			{
				keepTrying = false;
				result = doc.submitForm({
					aFields: [],
					bAnnotations: true,
					bInclNMKey: true,
					cURL: url,
					bExclFKey: true
					});
			}
			catch(e)
			{
				keepTrying = true;
				app.alert({cMsg: e["message"], oDoc: doc});
			}
		}
		else
			keepTrying = false;
	}

	if(!result) return;

	var alertText = IDS_SEND_FOR_REVIEW_CONFIRM_MSG.replace(/\%docname\%/g, fdfName);
	if (doc.Collab.isOutlook)
	{
		doc.Collab.alertWithHelp(
			{ cMsg: alertText,
			  cTip: IDS_SEND_FOR_REVIEW_CONFIRM_TIP,
			  cTitle: IDS_SEND_FOR_REVIEW_CONFIRM_TITLE,
			  cPref: "Annots:OutgoingEmailNotification"}
			  );
	}
	Collab.setReviewRespondedDate(doc, new Date());
}

function ANSendCommentsToAuthorEnabled(doc)
{
  //return (event.rc = doc && doc.Collab.initiatorEmail && doc.requestPermission(permission.annot, permission.canExport) && doc.requestPermission(permission.annot, permission.modify));
  return(event.rc = true);
}

if(!app.viewerType.match(/Reader/))
{
	// Add the menu item
	app.addMenuItem({
	  cName: "SendForReview",
	  cUser: IDS_SEND_FOR_REVIEW,
	  cParent: "File",
	  nPos: "endOptimizeGroup",
	  cExec: "ANSendForReview(event.target);",
	  cEnable: "ANSendForReviewEnabled(event.target);",
	  bPrepend: false
	});

	// Add the menu item
	app.addMenuItem({
	  cName: "SendCommentsToAuthor",
	  cUser: IDS_SEND_COMMENTS_TO_AUTHOR,
	  cParent: "File",
	  nPos: "endSendCommentsMenuItem",
	  cExec: "ANSendCommentsToAuthor(event.target);",
	  cEnable: "ANSendCommentsToAuthorEnabled(event.target);",
	  bPrepend: true
	});
}

function ANDefaultInvite(doc)
{
  if(!doc.external)
  {
    return ANSendForReview(doc);
  }
  // if external, fall through to the default C++ implementation
};

Collab.invite = ANDefaultInvite;

function CBdef(a, b)
{
  return typeof a == "undefined" ? b : a;
}

function Matrix2D(a, b, c, d, h, v)
{
	this.a = CBdef(a, 1);
	this.b = CBdef(b, 0);
	this.c = CBdef(c, 0);
	this.d = CBdef(d, 1);
	this.h = CBdef(h, 0);
	this.v = CBdef(v, 0);
	this.fromRotated = function(doc, page)
	{
		page = CBdef(page, 0);

		var cropBox = doc.getPageBox("Crop", page);
		var mediaBox = doc.getPageBox("Media", page);
		var mbHeight = mediaBox[1] - mediaBox[3];
		var mbWidth = mediaBox[2] - mediaBox[0];
		var rotation = doc.getPageRotation(page);
		var m = new Matrix2D(1, 0, 0, 1, cropBox[0] - mediaBox[0], cropBox[3] - mediaBox[3]);

		if(rotation == 90)
			return this.concat(m.rotate(Math.asin(1.0)).translate(mbHeight, 0));
		else if(rotation == 180)
			return this.concat(m.rotate(2.0 * -Math.asin(1.0)).translate(mbWidth, mbHeight));
		else if(rotation == 270)
			return this.concat(m.rotate(-Math.asin(1.0)).translate(0, mbWidth));
		return this.concat(m);
	}
	this.transform = function(pts)
	{
		var result = new Array(pts.length);

		if(typeof pts[0] == "object")
			for(var n = 0; n < pts.length; n++)
				result[n] = this.transform(pts[n]);
		else
			for(var n = 0; n + 1 < pts.length; n += 2)
			{
				result[n] = this.a * pts[n] + this.c * pts[n + 1] + this.h;
				result[n + 1] = this.b * pts[n] + this.d * pts[n + 1] + this.v;
			}
		return result;
	}
	this.concat = function(m)
	{
		return new Matrix2D(
			(this.a * m.a) + (this.b * m.c),
			(this.a * m.b) + (this.b * m.d),
			(this.c * m.a) + (this.d * m.c),
			(this.c * m.b) + (this.d * m.d),
			(this.h * m.a) + (this.v * m.c) + m.h,
			(this.h * m.b) + (this.v * m.d) + m.v);
	}
	this.invert = function()
	{
		var result = new Matrix2D;
		var q = this.b * this.c - this.a * this.d;

		if (q)
		{
			result.a = - this.d / q;
			result.b = this.b / q;
			result.c = this.c / q;
			result.d = - this.a / q;
			result.h = -(this.h * result.a + this.v * result.c);
			result.v = -(this.h * result.b + this.v * result.d);
		}
		return result;
	}
	this.translate = function(dx, dy)
	{
		return this.concat(new Matrix2D(1, 0, 0, 1, CBdef(dx, 0), CBdef(dy, 0)));
	}
	this.scale = function(sx, sy)
	{
		return this.concat(new Matrix2D(CBdef(sx, 1), 0, 0, CBdef(sy, 1), 0, 0));
	}
	this.rotate = function(t)
	{
		t = CBdef(t, 0);
		return this.concat(new Matrix2D(Math.cos(t), Math.sin(t), -Math.sin(t), Math.cos(t), 0, 0));
	}
}