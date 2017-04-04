<%@ Page Title="Document Upload" 
         Language="C#" 
         MasterPageFile="~/MasterPages/SiteLNav.Master" 
         AutoEventWireup="true" 
         CodeBehind="DocUpload.aspx.cs" 
         Inherits="EPF.DocUpload.DocUpload" %>

<asp:Content ID="Content1" 
             ContentPlaceHolderID="HeadContent" 
             runat="server">

    <link href="/Styles/jquery-ui-1.8.17.custom.css" 
          rel="stylesheet" 
          type="text/css" />
    <link href="/Styles/TimePicker.css" 
          rel="stylesheet" 
          type="text/css" />
    
    <script language="JavaScript"
            type="text/javascript" 
            src="../Scripts/GridView.js">
    </script>
    
	<script language="JavaScript" 
            type="text/javascript" 
            src="../Scripts/jquery-1.7.1.min.js"> 
    </script>
    <script language="JavaScript" 
            type="text/javascript" 
            src="../Scripts/jquery-ui-1.8.16.custom.min.js">
    </script>
    <script language="JavaScript" 
            type="text/javascript" 
            src="../Scripts/jquery-ui-timepicker-addon.js">
    </script>
    <script language="JavaScript" 
            type="text/javascript" 
            src="../Scripts/jquery-ui-sliderAccess.js">
    </script>
    <script language="JavaScript" 
            type="text/javascript">

        $(function () {
            // Effective Date Picker
            $(document.getElementById('txtEffDate')).datepicker(
            {
                changeMonth: true,
                changeYear: true,
                selectOtherMonths: true,
                showOtherMonths: true,
                showButtonPanel: true
            });
            // Due Date Picker
            $(document.getElementById('txtDueDate')).datepicker(
            {
                changeMonth: true,
                changeYear: true,
                selectOtherMonths: true,
                showOtherMonths: true,
                showButtonPanel: true
            });
        });

        // Calculation for 1000 MB (1 GB)
        var validFileSize = 1000 * 1024 * 1024;
        var maxFileSize = "1000 MB";

        function checkfilesize(source, arguments) {

            var version = 999;
            if (navigator.appVersion.indexOf("MSIE") != 1)
                version = parseFloat(navigator.appVersion.split("MSIE")[1]);

            arguments.IsValid = false;

            if (version > 9) {
                var size = document.getElementById("uplZipFile").files[0].size;
            }
            else {
                var axo = new ActiveXObject("Scripting.FileSystemObject");
                thefile = axo.getFile(arguments.Value);

                var size = thefile.size;
            }

            var mb_size = parseInt(size / 1048576);

            if (size > validFileSize) {
                arguments.IsValid = false;
                alert("Uploaded file size is: " + mb_size + " MB, which exceeds the " + maxFileSize + " size limit.");
            }
            else {
                arguments.IsValid = true;
            }
        }

    </script>

    <style type="text/css">
        .style1
        {
            height: 73px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" 
             ContentPlaceHolderID="MainContent" 
             runat="server">

        <h3>
            Document Upload
        </h3>

        <table cellpadding="1" 
               cellspacing="1" 
               width="99%">
            <tr>  <%-- Project Name --%>
			    <td valign="top" 
                    align="left" 
                    nowrap="nowrap">
				    Project Name <br />

                    <asp:HiddenField ID="hdnProcessorId" 
                                     runat="server" />

				    <asp:SqlDataSource ID="sdsProjects" 
                                       runat="server" 
                                       CancelSelectOnNullParameter="False"
                                       ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                       OnSelected="sdsProjects_Selected"
                                       SelectCommand="Get_Projects_For_DocUpload" 
                                       SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="hdnProcessorId" 
                                                  DbType="String"
                                                  Name="ProcessorId" 
                                                  PropertyName="Value" />
                        </SelectParameters>
                    </asp:SqlDataSource>
				    <asp:DropDownList ID="ddlProjects" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsProjects" 
								      DataTextField="Project_Name"
                                      DataValueField="Project_Id" 
                                      OnSelectedIndexChanged="ddlProjects_SelectedIndexChanged" >
					    <asp:ListItem>-- Select Project --</asp:ListItem>
                        <asp:ListItem>Add a New Project</asp:ListItem>
				    </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="rfvProjects" 
                                                runat="server" 
                                                ControlToValidate="ddlProjects" 
                                                Display="Dynamic" 
                                                ErrorMessage="Projects"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvProjects" 
                                          runat="server" 
                                          ErrorMessage="Project Name"
                                          ControlToValidate="ddlProjects"  
                                          ValueToCompare="-- Select Project --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
			    </td>
                <td  id="tdNewProject" runat="server" align="left" nowrap="nowrap">
                    New Project Name:<br />
                    <asp:TextBox ID="txtNewProjectName" 
                                 runat="server" 
                                 Width="300px"></asp:TextBox>
 				    <asp:RequiredFieldValidator ID="rfvNewProjectName" 
                                                runat="server" 
                                                ControlToValidate="txtNewProjectName" 
                                                Display="Dynamic" 
                                                ErrorMessage="Project Name"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
               </td>
            </tr>

            <tr>
                <td><br /></td>
            </tr>

            <%-- Upload Criteria --%>
            <tr id="trUploadCriteria" runat="server">  
			    <td valign="top" 
                    align="left" 
                    nowrap="nowrap">
				    Upload Criteria <br />
                    
                    <asp:SqlDataSource ID="sdsUploadCriteria" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                       OnSelected="sdsUploadCriteria_Selected"
                                       SelectCommand="Get_Upload_Criteria" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>
                    <asp:DropDownList ID="ddlUploadCriteria" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsUploadCriteria" 
								      DataTextField="Upload_Criteria_Name"
                                      DataValueField="Upload_Criteria_Id"
                                      OnSelectedIndexChanged="ddlUploadCriteria_SelectedIndexChanged">
                            <asp:ListItem>-- Select Upload Criteria --</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvUploadCriteria" 
                                                runat="server" 
                                                ErrorMessage="Upload Criteria" 
                                                ControlToValidate="ddlUploadCriteria"
                                                Display="Dynamic" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" 
                                                Visible="False" />
                    <asp:CompareValidator ID="cvUploadCriteria" 
                                          runat="server" 
                                          ErrorMessage="Upload Criteria"
                                          ControlToValidate="ddlUploadCriteria"  
                                          ValueToCompare="-- Select Upload Criteria --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
            </tr>

            <tr>
                <td><br /></td>
            </tr>

            <tr id="trBatchFile" runat="server">
                <td align="left" nowrap="nowrap">
                    Zip File Upload<br />
                    <asp:FileUpload ID="uplZipFile" 
                                    runat="server"
                                    ToolTip="Browse to find your Zip File" 
                                    Width="250px" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:RequiredFieldValidator ID="rfvZipFile" 
                                                runat="server" 
                                                ErrorMessage="Upload Zip File" 
                                                ControlToValidate="txtZipFile"
                                                Display="Dynamic" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" 
                                                Visible="True" />
                    <asp:CompareValidator ID="cvZipFile" 
                                          runat="server" 
                                          ErrorMessage="Complete Upload Process"
                                          ControlToValidate="uplZipFile"  
                                          ValueToCompare=""
                                          Operator="Equal"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                     <asp:Button ID="btnUploadZipFile" 
                                 runat="server" 
                                 Text="Upload Zip" 
                                 ToolTip="Upload the Zip File"  
                                 onclick="btnUploadZipFile_Click"
                                 ValidationGroup="UploadSize" 
                                 CausesValidation="true" />
                     <asp:CustomValidator ID="cvUploadSize" 
                                          runat="server"
                                          Text="*" 
                                          ToolTip="Tool Tip"
                                          ErrorMessage="File Uploaded exceeds the file size limit." 
                                          ValidationGroup="UploadSize"
                                          ControlToValidate="uplZipFile" 
                                          ClientValidationFunction = "checkfilesize">
                     </asp:CustomValidator>
               </td>
                <td align="left" nowrap="nowrap">
                    Control File Upload<br />
                    <asp:FileUpload ID="uplControlFile" 
                                    runat="server" 
                                    ToolTip="Browse to find your Control File" 
                                    Width="250px"/>&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:RequiredFieldValidator ID="rfvControlFile" 
                                                runat="server" 
                                                ErrorMessage="Upload Control File" 
                                                ControlToValidate="txtControlFile"
                                                Display="Dynamic" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" 
                                                Visible="True" />
                    <asp:CompareValidator ID="cvControlFile" 
                                          runat="server" 
                                          ErrorMessage="Complete Upload Process"
                                          ControlToValidate="uplControlFile"  
                                          ValueToCompare=""
                                          Operator="Equal"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                    <asp:Button ID="btnUploadControlFile" 
                                runat="server" 
                                Text="Upload Control" 
                                ToolTip="Upload the Control File" 
                                onclick="btnUploadControlFile_Click"
                                CausesValidation="False" />
                </td>
            </tr>

            <tr id="trBatchUpload" runat="server">
                <td align="left">
                    <asp:TextBox ID="txtZipFile" 
                                 runat="server"
                                 Width="500px" 
                                 BorderStyle="None" 
                                 Enabled="False">
                    </asp:TextBox>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtControlFile" 
                                 runat="server"
                                 Width="500px" 
                                 BorderStyle="None" 
                                 Enabled="False">
                    </asp:TextBox>
                </td>
            </tr>

            <tr id="trSingleDoc" runat="server">
                <td align="left" nowrap="nowrap">
                    PDF File Upload<br />
                    <asp:FileUpload ID="uplPDFFile" 
                                    runat="server" 
                                    ToolTip="Browse to find your single PDF File" 
                                    Width="250px"/>&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:RequiredFieldValidator ID="rfvPDFFile" 
                                                runat="server" 
                                                ErrorMessage="Upload PDF File" 
                                                ControlToValidate="txtPDF"
                                                Display="Dynamic" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" 
                                                Visible="True" />
                    <asp:CompareValidator ID="cvPDFFile" 
                                          runat="server" 
                                          ErrorMessage="Complete Upload Process"
                                          ControlToValidate="uplPDFFile"  
                                          ValueToCompare=""
                                          Operator="Equal"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                    <asp:Button ID="btnUploadPDFFile" 
                                runat="server"
                                Text="Upload PDF" 
                                ToolTip="Upload the PDF File" 
                                onclick="btnUploadPDFFile_Click" 
                                CausesValidation="False" />
                </td>
                <td align="left" nowrap="nowrap">
                    &nbsp;<br />
                    <asp:Button ID="btnSelectionCriteria" 
                                runat="server" 
                                Text="Selection Criteria" 
                                OnClientClick="javascript:openPopUp('Criteria.aspx','Criteria', 900, 600); return false;" />
                </td>
            </tr>
            <tr id="trSingleDocUpload" runat="server">
                <td align="left" nowrap="nowrap">
                    <asp:TextBox ID="txtPDF" 
                                 runat="server" 
                                 Width="500px"
                                 BorderStyle="None" 
                                 Enabled="False">
                    </asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td><br /></td>
            </tr>

            <tr id="trDates" runat="server">
			    <td valign="top" 
                    align="left" 
                    nowrap="nowrap">
				    Effective Date <br />
                    
                    <asp:TextBox ID="txtEffDate" 
                                 runat="server"
                                 ClientIDMode="Static" 
                                 MaxLength="20"
                                 Width="100px" />
                    <asp:RequiredFieldValidator ID="rfvEffDate" 
                                                runat="server" 
                                                ControlToValidate="txtEffDate" 
                                                Display="Dynamic" 
                                                ErrorMessage="Effective Date" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True" 
                                                Text="*" />
                    <asp:CompareValidator ID="cvEffDate" 
                                          runat="server" 
                                          ControlToValidate="txtEffDate"
                                          Display="Dynamic" 
                                          ErrorMessage="Invalid Date" 
                                          ForeColor="Red" 
                                          Operator="DataTypeCheck"
                                          SetFocusOnError="True"
                                          Text="*" 
                                          Type="Date" />
                    <asp:CompareValidator ID="cvEffDate2" 
                                          runat="server" 
                                          ErrorMessage="Effective date can not be in the past."
                                          ControlToValidate="txtEffDate" 
                                          ControlToCompare="txtTodaysDate"
                                          Operator="GreaterThanEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="Date" 
                                          SetFocusOnError="true"
                                          Text="*" />
               </td>
                <td valign="top" align="left">
                    Acknowledgement Due Date <br />
                    
                    <asp:TextBox ID="txtDueDate" 
                                 runat="server"
                                 ClientIDMode="Static" 
                                 MaxLength="20"
                                 Width="100px" />
                    <asp:RequiredFieldValidator ID="rfvDueDate" 
                                                runat="server" 
                                                ControlToValidate="txtDueDate" 
                                                Display="Dynamic" 
                                                ErrorMessage="Due Date" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True" 
                                                Text="*" />
                    <asp:CompareValidator ID="cvDueDate" 
                                          runat="server" 
                                          ControlToValidate="txtDueDate"
                                          Display="Dynamic" 
                                          ErrorMessage="Invalid Date" 
                                          ForeColor="Red" 
                                          Operator="DataTypeCheck"
                                          SetFocusOnError="True"
                                          Text="*" 
                                          Type="Date" />
                    <asp:CompareValidator ID="cvDate2" 
                                          runat="server" 
                                          ErrorMessage="Due Date must be later than Effective Date."
                                          ControlToValidate="txtDueDate"
                                          ControlToCompare="txtEffDate"
                                          Operator="GreaterThan"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="Date"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
            </tr>

            <tr>
                <td><br /></td>
            </tr>

            <tr id="trBarcodes" runat="server">
                <td align="left" nowrap="nowrap" class="style1">
                    Primary Barcode <br />

                    <asp:DropDownList ID="ddlPrimaryBarcode" 
                                      runat="server"
                                      AppendDataBoundItems="True" 
                                      onselectedindexchanged="ddlPrimaryBarcode_SelectedIndexChanged" 
                                      DataSourceID="sdsPrimaryBarcode" 
                                      DataTextField="Display" 
                                      DataValueField="Barcode">
                        <asp:ListItem Text="-- Select Primary Barcode --" Value="" />
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="sdsPrimaryBarcode" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                       OnSelected="sdsPrimaryBarcode_Selected" 
                                       SelectCommand="Get_Barcodes_For_DocUpload" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>
				    <asp:RequiredFieldValidator ID="rfvPrimaryBarcode" 
                                                runat="server" 
                                                ControlToValidate="ddlPrimaryBarcode" 
                                                Display="Dynamic" 
                                                ErrorMessage="Primary Barcode"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvPrimaryBarcode" 
                                          runat="server" 
                                          ErrorMessage="Primary Barcode"
                                          ControlToValidate="ddlPrimaryBarcode"  
                                          ValueToCompare="-- Select Primary Barcode --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
                <td align="left" nowrap="nowrap" class="style1">
                    Secondary Barcode <br />

                    <asp:DropDownList ID="ddlSecondaryBarcode" 
                                      runat="server" 
                                      AppendDataBoundItems="True" 
                                      onselectedindexchanged="ddlSecondaryBarcode_SelectedIndexChanged" 
                                      DataSourceID="sdsSecondaryBarcode" 
                                      DataTextField="Display" 
                                      DataValueField="Barcode">
                        <asp:ListItem Text="-- Select Secondary Barcode --" Value="" />
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="sdsSecondaryBarcode" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                       OnSelected="sdsSecondaryBarcode_Selected" 
                                       SelectCommand="Get_Barcodes_For_DocUpload" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>
				    <asp:RequiredFieldValidator ID="rfvSecondaryBarcode" 
                                                runat="server" 
                                                ControlToValidate="ddlSecondaryBarcode" 
                                                Display="Dynamic" 
                                                ErrorMessage="Secondary Barcode"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvSecondaryBarcode" 
                                          runat="server" 
                                          ErrorMessage="Secondary Barcode"
                                          ControlToValidate="ddlSecondaryBarcode"  
                                          ValueToCompare="-- Select Secondary Barcode --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
            </tr>

            <tr>
                <td><br /></td>
            </tr>

            <tr id="trEmails" runat="server">
                <td align="left" nowrap="nowrap">
                    Send Email Notification <br />

                    <asp:DropDownList ID="ddlEmailNotify" 
                                      runat="server" 
                                      AppendDataBoundItems="True" 
                                      onselectedindexchanged="ddlEmailNotify_SelectedIndexChanged">
                        <asp:ListItem Text="-- Select Email Notify --" Value="-- Select Email Notify --" />
                        <asp:ListItem Text="Yes" Value="Y" />
                        <asp:ListItem Text="No" Value="N" />
                    </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="rfvEmailNotify" 
                                                runat="server" 
                                                ControlToValidate="ddlEmailNotify" 
                                                Display="Dynamic" 
                                                ErrorMessage="Email Notification"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvEmailNotify" 
                                          runat="server" 
                                          ErrorMessage="Email Notification"
                                          ControlToValidate="ddlEmailNotify"  
                                          ValueToCompare="-- Select Email Notify --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
                <td align="left" nowrap="nowrap">
                    Send Email Reminder <br />

                    <asp:DropDownList ID="ddlEmailReminder" 
                                      runat="server" 
                                      AppendDataBoundItems="True" 
                                      onselectedindexchanged="ddlEmailReminder_SelectedIndexChanged">
                        <asp:ListItem Text="-- Select Email Reminder --" Value="" />
                        <asp:ListItem Text="Yes" Value="Y" />
                        <asp:ListItem Text="No" Value="N" />
                    </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="rfvEmailReminder" 
                                                runat="server" 
                                                ControlToValidate="ddlEmailReminder" 
                                                Display="Dynamic" 
                                                ErrorMessage="Email Reminder"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvEmailReminder" 
                                          runat="server" 
                                          ErrorMessage="Email Reminder"
                                          ControlToValidate="ddlEmailReminder"  
                                          ValueToCompare="-- Select Email Reminder --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
            </tr>
             <tr>
                <td><br /></td>
            </tr>

            <tr id="trButtons" runat="server">
                <td  align="center" colspan="3">
                    <asp:Button ID="btnSave" 
                                runat="server" 
                                Text="    Save    " 
                                onclick="btnSave_Click" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancel" 
                                runat="server" 
                                Text="   Cancel   " 
                                onclick="btnCancel_Click" 
                                CausesValidation="False" />
                </td>
            </tr>

        </table>
    <div style="display:none">
        <asp:TextBox ID="txtTodaysDate" 
                     runat="server" />
    </div>
    <%--     Validation Summary - Displays a MessageBox     --%>
    <asp:ValidationSummary ID="ValidationSummary1" 
                           runat="server" 
                           HeaderText="Please enter the following information:" 
                           ShowMessageBox="True" 
                           ShowSummary="False" />

    <asp:ValidationSummary ID="ValidationSummary2" 
                           runat="server" 
                           ValidationGroup="UploadSize"
                           HeaderText="Please correct the following information:" 
                           ShowMessageBox="True" 
                           ShowSummary="False" />
</asp:Content>
