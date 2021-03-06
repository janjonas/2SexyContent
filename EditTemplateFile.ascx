﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTemplateFile.ascx.cs" Inherits="ToSic.SexyContent.EditTemplateFile" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<div class="dnnForm dnnEditTemplateFile dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelSexyContentEditTemplateFile">
        <a href="#"><asp:Label runat="server" ID="lblEditTemplateFileHeading"></asp:Label></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <div class="dnnClear">
                <asp:TextBox CssClass="sc-txt-templatecontent" runat="server" ID="txtTemplateContent" TextMode="MultiLine" Rows="20" Wrap="false" Width="97%"></asp:TextBox>
            </div>
        </div>
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" ID="btnUpdate" ResourceKey="btnUpdate" OnClick="btnUpdate_Click" CssClass="dnnPrimaryAction"></asp:LinkButton></li>
            <li><asp:HyperLink runat="server" ID="hlkCancel" ResourceKey="hlkCancel" CssClass="dnnSecondaryAction"></asp:HyperLink></li>
        </ul>
    </fieldset>

    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelTemplateHelp"><a href="#">
        <asp:Label runat="server" ID="lblTemplateHelpHeader" ResourceKey="lblTemplateHelpHeader"></asp:Label></a>
        </h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="plCurrentTemplate" ResourceKey="plCurrentTemplate" HelpKey="plCurrentTemplate.HelpText" ControlName="lblTemplate" Suffix=":" />
            <asp:Label runat="server" ID="lblTemplate"></asp:Label>
        </div>
        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="plTemplateLocation" ResourceKey="plTemplateLocation" HelpKey="plTemplateLocation.HelpText" ControlName="lblTemplate" Suffix=":" />
            <asp:Label runat="server" ID="lblTemplateLocation"></asp:Label>
        </div>
        <div class="dnnFormItem">
            <asp:Label runat="server" ID="lblTableDescription" ResourceKey="lblTableDescription"></asp:Label>
        </div>

        <asp:PlaceHolder runat="server" ID="phGrids"></asp:PlaceHolder>
    </fieldset>
</div>

<script type="text/javascript">
    jQuery(function ($) {
        var setupModule = function () {
            $('.dnnEditTemplateFile').dnnPanels();
            if ($(".dnnForm fieldset:visible").size() != 2) {
                $("<a />").dnnExpandAll({
                    targetArea: '.dnnEditTemplateFile'
                }).trigger("click");
            }
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });

    $(document).delegate('.sc-txt-templatecontent', 'keydown', function (e) {
        var keyCode = e.keyCode || e.which;

        if (keyCode == 9) {
            e.preventDefault();
            var start = $(this).get(0).selectionStart;
            var end = $(this).get(0).selectionEnd;

            // set textarea value to: text before caret + tab + text after caret
            $(this).val($(this).val().substring(0, start)
                        + "\t"
                        + $(this).val().substring(end));

            // put caret at right position again
            $(this).get(0).selectionStart =
            $(this).get(0).selectionEnd = start + 1;
        }
    });
</script>

<style type="text/css">
    .grdFields { margin-bottom: 10px; }
    .sc-txt-templatecontent { max-width:none!important;font-family: "Monaco", "Menlo", "Ubuntu Mono", "Consolas", "source-code-pro", monospace; -moz-tab-size:4;-ms-tab-size: 4; -o-tab-size:4; tab-size:4; }
</style>