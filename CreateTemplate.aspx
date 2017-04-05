<%@ Page Title="Create Template" 
         Language="C#" 
         MasterPageFile="~/MasterPages/SiteCAWNav.Master" 
         AutoEventWireup="true" 
         CodeBehind="CreateTemplate.aspx.cs" 
         Inherits="EPF.Maintenance.CreateTemplate" %>

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
        });

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
             <h2>
            Maintenance - Create Template
        </h2>

        <table cellpadding="1" 
               cellspacing="1" 
               width="99%">

               <%-- Template Name --%>
               <tr>
               <%--<td valign="top" 
                    align="left" 
                    nowrap="nowrap">
				    Template Name <br />--%>

                    <%--<asp:HiddenField ID="hdnProcessorId" 
                                     runat="server" />--%>

                    <%--<asp:SqlDataSource ID="sdsCreateTemplate" 
                                       runat="server" 
                                       CancelSelectOnNullParameter="False"
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsCreateTemplate_Selected"
                                       SelectCommand="Get_Template_Name" 
                                       SelectCommandType="StoredProcedure">--%>
                        <%--<SelectParameters>
                            <asp:ControlParameter ControlID="hdnProcessorId" 
                                                  DbType="String"
                                                  Name="ProcessorId" 
                                                  PropertyName="Value" />
                        </SelectParameters>--%>
                   <%-- </asp:SqlDataSource>

                    <asp:DropDownList ID="ddlCreateTemplate" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsCreateTemplate" 
								      DataTextField="Name"
                                      DataValueField="Template_Id" 
                                      OnSelectedIndexChanged="ddlCreateTemplate_SelectedIndexChanged" >
					    <asp:ListItem>-- Select Template --</asp:ListItem>
                        <asp:ListItem>Add a New Template</asp:ListItem>
				    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvTemplates" 
                                                runat="server" 
                                                ControlToValidate="ddlCreateTemplate" 
                                                Display="Dynamic" 
                                                ErrorMessage="Templates"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvTemplates" 
                                          runat="server" 
                                          ErrorMessage="Template Name"
                                          ControlToValidate="ddlCreateTemplate"  
                                          ValueToCompare="-- Select Template --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
			    </td>--%>

                <td  id="tdNewTemplate" 
                     runat="server" 
                     align="left" 
                     nowrap="nowrap">
                    New Template Name:<br />
                    <asp:TextBox ID="txtNewTemplateName" 
                                 runat="server" 
                                 Width="300px"></asp:TextBox>
 				    <asp:RequiredFieldValidator ID="rfvNewTemplateName" 
                                                runat="server" 
                                                ControlToValidate="txtNewTemplateName" 
                                                Display="Dynamic" 
                                                ErrorMessage="Template Name"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
               </td>
            </tr>

            <tr>
                <td><br /></td>
            </tr>

            <%-- Upload Template --%>

            <tr id="trUploadTemplate" runat="server">
                <td align="left" nowrap="nowrap">
                    DOCX File Upload<br />
                    <asp:FileUpload ID="uplDOCXFile" 
                                    runat="server" 
                                    ToolTip="Browse to find your Word DOCX File" 
                                    Width="250px"/>&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:RequiredFieldValidator ID="rfvDOCXFile" 
                                                runat="server" 
                                                ErrorMessage="Upload DOCX File" 
                                                ControlToValidate="txtDOCXFile"
                                                Display="Dynamic" 
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" 
                                                Visible="True" />
                    <asp:CompareValidator ID="cvDOCXFile" 
                                          runat="server" 
                                          ErrorMessage="Complete Upload Process"
                                          ControlToValidate="uplDOCXFile"  
                                          ValueToCompare=""
                                          Operator="Equal"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                    <asp:Button ID="btnUploadDOCXFile" 
                                runat="server"
                                Text="Upload DOCX" 
                                ToolTip="Upload the Word DOCX File" 
                                onclick="btnUploadDOCXFile_Click"
                                CausesValidation="False" />
                </td>

                <td align="left" nowrap="nowrap">
                    &nbsp;<br />
                    <asp:Button ID="btnValidateTemplate" 
                                runat="server" 
                                Text="Validate Template" 
                                OnClientClick="javascript:openPopUp('ValidateTemplate.aspx','Validate', 900, 600); return false;" />
                </td>
            </tr>

            <tr id="trDOCXUpload" runat="server">
                <td align="left" nowrap="nowrap">
                    <asp:TextBox ID="txtDOCXFile" 
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

             <tr id="trEffDate" runat="server">
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
            </tr>

            <tr>
                <td><br /></td>
            </tr>

            <tr id="trCategories" runat="server">
                <td align="left" nowrap="nowrap" class="style1">
                    Category <br />

                    <asp:SqlDataSource ID="sdsCategories" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsCategories_Selected"
                                       SelectCommand="Get_Categories" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>

                    <asp:DropDownList ID="ddlCategories" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsCategories" 
								      DataTextField="Category"
                                      DataValueField="Category_Id"
                                      OnSelectedIndexChanged="ddlCategories_SelectedIndexChanged">
                            <asp:ListItem>-- Select Category --</asp:ListItem>
                    </asp:DropDownList>

				    <asp:RequiredFieldValidator ID="rfvCategories" 
                                                runat="server" 
                                                ControlToValidate="ddlCategories" 
                                                Display="Dynamic" 
                                                ErrorMessage="Category"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvCategories" 
                                          runat="server" 
                                          ErrorMessage="Category"
                                          ControlToValidate="ddlCategories"  
                                          ValueToCompare="-- Select Category --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
                <td align="left" nowrap="nowrap" class="style1">
                    Subcategory <br />
                    <asp:SqlDataSource ID="sdsSubcategories" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsSubcategories_Selected"
                                       SelectCommand="Get_Subcategories" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>

                    <asp:DropDownList ID="ddlSubcategories" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsSubcategories" 
								      DataTextField="Subcategory"
                                      DataValueField="Subcategory_Id"
                                      OnSelectedIndexChanged="ddlSubcategories_SelectedIndexChanged">
                            <asp:ListItem>-- Select Subcategory --</asp:ListItem>
                    </asp:DropDownList>

				    <asp:CompareValidator ID="cvSubcategories" 
                                          runat="server" 
                                          ErrorMessage="Subcategory"
                                          ControlToValidate="ddlSubcategories"  
                                          ValueToCompare="-- Select Subcategory --"
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

            <tr id="trTypeExec" runat="server">
                <td align="left" nowrap="nowrap" class="style1">
                    Type <br />

                    <asp:SqlDataSource ID="sdsTypes" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsTypes_Selected"
                                       SelectCommand="Get_Template_Types" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>

                    <asp:DropDownList ID="ddlTypes" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsTypes" 
								      DataTextField="Type"
                                      DataValueField="Type_Id"
                                      OnSelectedIndexChanged="ddlTypes_SelectedIndexChanged">
                            <asp:ListItem>-- Select Type --</asp:ListItem>
                    </asp:DropDownList>

				    <asp:RequiredFieldValidator ID="rfvTypes" 
                                                runat="server" 
                                                ControlToValidate="ddlTypes" 
                                                Display="Dynamic" 
                                                ErrorMessage="Type"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />
                    <asp:CompareValidator ID="cvTypes" 
                                          runat="server" 
                                          ErrorMessage="Type"
                                          ControlToValidate="ddlTypes"  
                                          ValueToCompare="-- Select Type --"
                                          Operator="NotEqual"
                                          ForeColor="Red" 
                                          Font-Bold="true"
                                          Type="String"
                                          SetFocusOnError="true"
                                          Text="*" />
                </td>
                <td align="left" nowrap="nowrap" class="style1">
                    Executive Code <br />
                    <asp:SqlDataSource ID="sdsExecCode" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsExecCode_Selected"
                                       SelectCommand="Get_Exec_Codes" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>

                    <asp:DropDownList ID="ddlExecCode" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
								      AutoPostBack="True" 
                                      DataSourceID="sdsExecCode" 
								      DataTextField="Display"
                                      DataValueField="Exec_Cd"
                                      OnSelectedIndexChanged="ddlExecCode_SelectedIndexChanged">
                            <asp:ListItem>-- Select Executive Code --</asp:ListItem>
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvExecCode" 
                                                runat="server" 
                                                ControlToValidate="ddlExecCode" 
                                                Display="Dynamic" 
                                                ErrorMessage="Executive Code"
                                                ForeColor="Red" 
                                                SetFocusOnError="True"
                                                Text="*" />

				    <asp:CompareValidator ID="cvExecCode" 
                                          runat="server" 
                                          ErrorMessage="Executive Code"
                                          ControlToValidate="ddlExecCode"  
                                          ValueToCompare="-- Select Executive Code --"
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
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsPrimaryBarcode_Selected" 
                                       SelectCommand="Get_CAW_Barcodes" 
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
                                       ConnectionString="<%$ ConnectionStrings:CAW %>" 
                                       OnSelected="sdsSecondaryBarcode_Selected" 
                                       SelectCommand="Get_CAW_Barcodes" 
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

</asp:Content>
