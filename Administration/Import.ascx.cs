﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    public partial class Import : SexyControlAdminBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkExport.NavigateUrl =  EditUrl(TabId, SexyContent.ControlKeys.Export, true, "mid=" + ModuleId + "&" + SexyContent.AppIDString + "=" + AppId);
            //pnlGettingStartedTemplates.Visible = IsContentApp && !Sexy.GetVisibleTemplates(PortalSettings.PortalId).Any();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                ImportFromStream(fileUpload.FileContent, fileUpload.FileName.EndsWith(".zip"));
            }
        }

        protected void ImportFromStream(Stream importStream, bool isZip)
        {
            var messages = new List<ExportImportMessage>();
            var success = false;

            if (isZip)
            {
                success = new ZipImport(ZoneId.Value, AppId.Value, UserInfo.IsSuperUser).ImportZip(importStream, Server, PortalSettings, messages, false);
            }
            else
            {
                string Xml = new StreamReader(importStream).ReadToEnd();
                var import = new XmlImport();
                success = import.ImportXml(ZoneId.Value, AppId.Value, Xml);
                messages = import.ImportLog;
            }

            lstvSummary.DataSource = messages;
            lstvSummary.DataBind();
            pnlSummary.Visible = true;
            pnlUpload.Visible = false;
        }

        protected void lstvSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ExportImportMessage.MessageTypes MessageType = ((ExportImportMessage)e.Item.DataItem).MessageType;
                Panel Pnl = (Panel)e.Item.FindControl("pnlMessage");

                if (MessageType == ExportImportMessage.MessageTypes.Error)
                    Pnl.CssClass += " dnnFormValidationSummary";
                if (MessageType == ExportImportMessage.MessageTypes.Information)
                    Pnl.CssClass += " dnnFormSuccess";
                if (MessageType == ExportImportMessage.MessageTypes.Warning)
                    Pnl.CssClass += " dnnFormWarning";
            }
        }

        protected void btnInstallGettingStarted_Click(object sender, EventArgs e)
        {
            var messages = new List<ExportImportMessage>();
            new GettingStartedImport(ZoneId.Value, AppId.Value).ImportGettingStartedTemplates(UserInfo, messages);

            //var release = Releases.Element("SexyContentReleases").Elements("Release").FirstOrDefault(p => p.Attribute("Version").Value == SexyContent.ModuleVersion);
            //var starterPackageUrl = release.Elements("RecommendedPackages").Elements("Package").First().Attribute("PackageUrl").Value;

            //var tempDirectory = new DirectoryInfo(Server.MapPath(SexyContent.TemporaryDirectory));
            //if (tempDirectory.Exists)
            //    Directory.CreateDirectory(tempDirectory.FullName);

            //var destinationPath = Path.Combine(tempDirectory.FullName, Path.GetRandomFileName() + ".zip");

            //var client = new WebClient();

            //client.DownloadFile(starterPackageUrl, destinationPath);

            //using(var file = File.OpenRead(destinationPath))
            //    ImportFromStream(file, true);

            //File.Delete(destinationPath);
        }
    }
}