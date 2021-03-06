﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Web.UI.WebControls;

namespace ToSic.SexyContent
{
    public partial class EditTemplateFile : SexyControlEditBase
    {

		//private const string AdditionalSystemTokens = "[Content:Toolbar],ContentToolbar";
		//private const string ListAdditionalSystemTokens = "[List:Index],ListIndex;[List:Index1],ListIndex1;[List:Count],ListCount;[List:IsFirst],ListIsFirst;[List:IsLast],ListIsLast;[List:Alternator2],ListAlternator2;[List:Alternator3],ListAlternator3;[List:Alternator4],ListAlternator4;[List:Alternator5],ListAlternator5;[ListContent:Toolbar],ListToolbar;<repeat>...</repeat>,ListRepeat";

		//private const string AdditionalSystemRazor = "@Content.Toolbar,ContentToolbar";
		//private const string ListAdditionalSystemRazor = "@ListContent.Toolbar,ListToolbar";

        private bool UserMayEdit
        {
            get
            {
                return UserInfo.IsSuperUser ||
                    (Template.Location == SexyContent.TemplateLocations.PortalFileSystem && !Template.IsRazor && UserInfo.IsInRole(PortalSettings.AdministratorRoleName));
            }
        }

        private Template Template
        {
            get {
                int TemplateID = int.Parse(Request.QueryString["TemplateID"]);
                return Sexy.TemplateContext.GetTemplate(TemplateID);
            }
        }

        private string TemplatePath
        {
            get {
                return Server.MapPath(System.IO.Path.Combine(SexyContent.GetTemplatePathRoot(Template.Location, Sexy.App), Template.Path));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserMayEdit)
            {
                btnUpdate.Visible = false;
                txtTemplateContent.Enabled = false;
            }

            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId);

            if (IsPostBack)
                return;

            if (Template == null)
                return;

            if (File.Exists(TemplatePath))
                txtTemplateContent.Text = File.ReadAllText(TemplatePath);

            // Get name of the current template and write it to lblTemplate
            lblTemplate.Text = Template.Name;
            lblTemplateLocation.Text = @"/" + TemplatePath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, String.Empty).Replace('\\','/');
            lblEditTemplateFileHeading.Text = String.Format(LocalizeString("lblEditTemplateFileHeading.Text"), Template.Name);


            var defaultLanguageID = Sexy.ContentContext.GetLanguageId(PortalSettings.DefaultLanguage);
            var languageList = defaultLanguageID.HasValue ? new[] {defaultLanguageID.Value} : new[] { 0 };
            
            var templateDefaults = Sexy.GetTemplateDefaults(Template.TemplateID).Where(t => t.ContentTypeID.HasValue);
            string formatString;

            if (Template.Type == "Token")
                formatString = "[{0}:{1}]";
            else
                formatString = "@{0}.{1}";

            foreach(var templateDefault in templateDefaults)
            {
	            var contentTypeId = templateDefault.ContentTypeID.Value;
	            AddHelpForAContentType(contentTypeId, formatString, templateDefault, languageList);
            }

			// todo: add AppResources and AppSettings help

			// add standard help
	        AddHelpFragments();
        }

		/// <summary>
		/// Create a help-table showing all the tokens/placeholders for a specific content type
		/// </summary>
		/// <param name="contentTypeId"></param>
		/// <param name="FormatString"></param>
		/// <param name="TemplateDefault"></param>
		/// <param name="LanguageList"></param>
	    private void AddHelpForAContentType(int contentTypeId, string FormatString, TemplateDefault TemplateDefault,
		    int[] LanguageList)
	    {
		    Eav.AttributeSet Set = Sexy.ContentContext.GetAttributeSet(contentTypeId);

		    var DataSource = Sexy.ContentContext.GetAttributes(Set, true).Select(a => new
		    {
			    StaticName = String.Format(FormatString, TemplateDefault.ItemType.ToString("F"), a.StaticName),
			    DisplayName =
				    (Sexy.ContentContext.GetAttributeMetaData(a.AttributesInSets.FirstOrDefault().AttributeID)).ContainsKey("Name")
					    ? (Sexy.ContentContext.GetAttributeMetaData(a.AttributesInSets.FirstOrDefault().AttributeID))["Name"][LanguageList]
					    : a.StaticName + " (static)"
		    }).ToList();

		    AddFieldGrid(DataSource, TemplateDefault.ItemType.ToString("F"));
	    }

	    /// <summary>
		/// Add helper infos to the editor, common tokens, razor snippets etc.
		/// </summary>
	    private void AddHelpFragments()
	    {
			// 2014-07-18 2dm - new
		    var lookupKey = Template.IsRazor ? "Razor" : "Token";
		    var additionalHelpers = LocalizeString("Additional" + lookupKey + "Sets.Text");
		    if (!Template.UseForList)
			    additionalHelpers = additionalHelpers.Replace("List" + lookupKey + ",", "");
		    foreach (var helperSet in additionalHelpers.Split(','))
		    {
			    var setText = LocalizeString(helperSet + ".List");
			    var hasEncodedStuff = (setText.IndexOf("»") > 0);
			    var splitFilter1 = hasEncodedStuff ? new string[] {"»\r\n", "»\n"} : new string[] {"\r\n", "\n"};
			    var splitFilter2 = hasEncodedStuff ? new string[] {"«"} : new string[] {"="};
			    var data =
				    setText.Split(splitFilter1, StringSplitOptions.None)
					    .Select(
						    d =>
							    new
							    {
								    StaticName = d.Split(splitFilter2, StringSplitOptions.None)[0],
								    DisplayName = d.Split(splitFilter2, StringSplitOptions.None)[1]
							    })
					    .ToList();
			    if (hasEncodedStuff)
				    data =
					    data.Select(x => new {StaticName = EncodeCode(x.StaticName), DisplayName = EncodeComment(x.DisplayName)}).ToList();
			    // todo: bug - cannot pass in translated title, the grid always tries to re-look it up in another resx
			    //var title = LocalizeString(tokenSet + ".Title");
			    AddFieldGrid(data, helperSet);
		    }


		    //// Add Token Help Tables
		    //if (!Template.IsRazor)
		    //{
		    //	// 2014-07-18 2dm - removed
		    //	// AddFieldGrid(AdditionalSystemTokens.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text")}), "System");
		    //	//if (Template.UseForList)
		    //	//	AddFieldGrid(ListAdditionalSystemTokens.Split(';').Select(d => new { StaticName = HttpUtility.HtmlEncode(d.Split(',')[0]), DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "ListSystem");

		    //}
		    //// Add Razor Help Tables
		    //else
		    //{
		    //	AddFieldGrid(AdditionalSystemRazor.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "System");

		    //	if(Template.UseForList)
		    //		AddFieldGrid(ListAdditionalSystemRazor.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "ListSystem");
		    //}
	    }

	    protected string EncodeCode(string original)
	    {
		    //return "<pre>" + original + "</pre>";
		    return Server.HtmlEncode(original)
			    .Replace("\r\n", "<br/>")
			    .Replace("\t", "&nbsp;");
	    }

	    protected string EncodeComment(string original)
	    {
			return Server.HtmlEncode(original)
				.Replace("\r\n", "<br/>")
				.Replace("\t", "&nbsp;");
		}

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if(!UserMayEdit)
                return;

            if (File.Exists(TemplatePath))
            {
                File.WriteAllText(TemplatePath, txtTemplateContent.Text);
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId));
            }
        }

        protected void AddFieldGrid(object dataSource, string headerText)
        {
            var gridControl = (TemplateHelpGrid)LoadControl(Path.Combine(TemplateSourceDirectory, "TemplateHelpGrid.ascx"));
            var grid = gridControl.Grid;


            // DataBind the GridView with the Tokens
            DnnGridBoundColumn tokenColumn = ((DnnGridBoundColumn)grid.Columns.FindByUniqueName("StaticName"));
            tokenColumn.HeaderText = headerText;
            
            grid.DataSource = dataSource;
            grid.DataBind();
            
            phGrids.Controls.Add(gridControl);
        }
    }
}