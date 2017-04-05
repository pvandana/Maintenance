<%@ Page Title="Criteria" 
         Language="C#" 
         MasterPageFile="~/MasterPages/PopUp.Master" 
         AutoEventWireup="true" 
         CodeBehind="Criteria.aspx.cs" 
         Inherits="EPF.DocUpload.Criteria" %>

<asp:Content ID="Content1" 
             ContentPlaceHolderID="HeadContent" 
             runat="server">
    <link href="/Styles/jquery-ui-1.8.17.custom.css" 
          rel="stylesheet" 
          type="text/css" />
    <link href="/Styles/Grid.css" 
          rel="stylesheet" 
          type="text/css" />
    <script language="JavaScript"
            type="text/javascript" 
            src="../Scripts/GridView.js">
    </script>

    <script type="text/javascript">

        function focusOnExec() {
            document.getElementById("<%= ddlExec.ClientID %>").focus();
            return false;
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" 
             ContentPlaceHolderID="MainContent" 
             runat="server">

    <%--     Hidden Fields     --%>
     <asp:HiddenField ID="hdnNewRecord" 
                     runat="server" 
                     ClientIDMode="Static" />
    <asp:HiddenField ID="hdnSelectedCell" 
                     runat="server" 
                     ClientIDMode="Static" />

    <h3>
        Criteria
    </h3>

    <asp:Panel ID="pnlQueryCriteria" 
               runat="server" 
               CssClass="Panel" 
               GroupingText="Query Criteria" Font-Size="Small"> 
        <table cellpadding="1" 
               cellspacing="1" 
               width="99%">
            <tr>
                <td align="center" colspan="5">
                    <asp:DropDownList ID="ddlCriteria" 
                                      runat="server" 
                                      AppendDataBoundItems="True"
                                      AutoPostBack="True" 
                                      DataTextField="Single_Document_Selection_Criteria_Name"
                                      DataValueField="Single_Document_Selection_Criteria_Id" 
                                      onselectedindexchanged="ddlCriteria_SelectedIndexChanged" 
                                      DataSourceID="sdsCriteria">
                        <asp:ListItem Text="-- Select Criteria --" Value="" />
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="sdsCriteria" 
                                       runat="server" 
                                       ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                       OnSelected="sdsCriteria_Selected"
                                       SelectCommand="Get_Single_Document_Selection_Criteria" 
                                       SelectCommandType="StoredProcedure">
                    </asp:SqlDataSource>
                </td>
            </tr>
            <tr>
                <td>
                    <br /><br /><br />
                </td>
            </tr>
            <tr runat="server" id="trMultiView">
                <td align="center" colspan="5">
                    <asp:MultiView ID="mvCriteria" 
                                   runat="server"
                                   ActiveViewIndex="0">
                        <asp:View ID="vwEmplID" 
                                  runat="server">     
                            <table cellpadding="1" 
                                   cellspacing="1">
                                    <tr>
                                        <td>
                                            <asp:ObjectDataSource ID="odsEmplId" 
                                                                  runat="server"
                                                                  TypeName="EPF.DataAccess.EmployeeIdDA"
                                                                  DataObjectTypeName="EPF.BusinessObjects.EmployeeIdBO" 
                                                                  DeleteMethod="Delete"
                                                                  InsertMethod="Insert" 
                                                                  SelectMethod="Get"
                                                                  UpdateMethod="Update" 
                                                                  OnDeleted="odsEmplId_Deleted"
                                                                  OnSelected="odsEmplId_Selected" 
                                                                  OnUpdated="odsEmplId_Updated">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="hdnNewRecord" 
                                                                              DefaultValue="False" 
                                                                              Name="newRecord"
                                                                              PropertyName="Value" 
                                                                              Type="String" />
                                                    </SelectParameters>
                                            </asp:ObjectDataSource>
    
                                                    <asp:GridView ID="gvEmplId" 
                                                                  runat="server" 
                                                                  AllowPaging="True"
                                                                  AllowSorting="True"
                                                                  AutoGenerateColumns="False" 
                                                                  CellPadding="4"
                                                                  DataKeyNames="Id"
                                                                  OnDataBound="gvEmplId_DataBound"
                                                                  OnPageIndexChanging="gvEmplId_PageIndexChanging"
                                                                  OnRowCancelingEdit="gvEmplId_RowCancelingEdit"
                                                                  OnRowCreated="gvEmplId_RowCreated"
                                                                  OnRowDeleted="gvEmplId_RowDeleted" 
                                                                  OnRowDataBound="gvEmplId_RowDataBound"
                                                                  OnRowEditing="gvEmplId_RowEditing"
                                                                  OnRowUpdated="gvEmplId_RowUpdated"
                                                                  OnRowUpdating="gvEmplId_RowUpdating"
                                                                  PagerSettings-Mode="NumericFirstLast"
                                                                  PageSize="5" 
                                                                  DataSourceID="odsEmplId"> 
        
                                                        <%--     GridView Columns     --%>
                                                        <Columns>
                                                            <%--     Delete Button     --%>
                                                            <asp:CommandField ButtonType="Button" 
                                                                              ShowDeleteButton="True"
                                                                              ShowEditButton="True"
                                                                              ShowHeader="True" />
                                                            <asp:BoundField DataField="Id" 
                                                                            Visible="False" />
                            
                                                            <%--     Employee Id     --%>
                                                            <asp:TemplateField HeaderText="Employee ID" 
                                                                               ItemStyle-HorizontalAlign="Center"
                                                                               ControlStyle-Width="80px" 
                                                                               SortExpression="EmplId">
                                                                <ItemTemplate>
                                                                    <%# Eval("EmplId")%>
                                                                </ItemTemplate>

                                                                <EditItemTemplate>
                                                                    <asp:TextBox ID="txtEmplId" 
                                                                                 runat="server" 
                                                                                 ClientIDMode="Static" 
                                                                                 MaxLength="11" />
                                                                    <asp:RegularExpressionValidator ID="revEmplId" 
                                                                                                    runat="server" 
                                                                                                    ControlToValidate="txtEmplId" 
                                                                                                    Display="Dynamic" 
                                                                                                    ErrorMessage="Employee ID must be numeric"
                                                                                                    ForeColor="Red" 
                                                                                                    SetFocusOnError="True"
                                                                                                    Text="*"
                                                                                                    ValidationExpression="^\d+$" />
                                                                </EditItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>

                                                        <%--     GridView Styles     --%>
                                                        <AlternatingRowStyle CssClass="gvAlternatingRow" />
                                                        <EditRowStyle CssClass="gvEditRow" />
                                                        <FooterStyle CssClass="gvFooter" />
                                                        <HeaderStyle CssClass="gvHeader" />
                                                        <PagerStyle CssClass="gvPager" />
                                                        <SelectedRowStyle  CssClass="gvSelectedRow" />
                                                    </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </asp:View>

                        <asp:View ID="vwOther" 
                                  runat="server">         
                            <table>
                                <tr>
                                    <td align="center">
                                        Executive
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblOrg" runat="server" Text="Organization" />
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblGroup" runat="server" Text="Group" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlExec" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True" 
                                                          DataTextField="Exec_Descr"
                                                          DataValueField="Exec_Cd" 
                                                          OnSelectedIndexChanged="ddlExec_SelectedIndexChanged" 
                                                          DataSourceID="sdsExec" 
                                                          Width="200px">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsExec" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsExec_Selected" 
                                                           SelectCommand="Get_Exec_Codes" 
                                                           SelectCommandType="StoredProcedure">
                                        </asp:SqlDataSource>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlOrg" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True" 
                                                          DataTextField="Org_Descr"
                                                          DataValueField="Org_Cd" 
                                                          DataSourceID="sdsOrg"
                                                          OnSelectedIndexChanged="ddlOrg_SelectedIndexChanged"
                                                          Width="200px">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsOrg" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsOrg_Selected"
                                                           SelectCommand="Get_Org_Codes" 
                                                           SelectCommandType="StoredProcedure">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlExec" 
                                                                          Name="ExecCode" 
                                                                          PropertyName="SelectedValue" 
                                                                          Type="String" />
                                                </SelectParameters>
                                         </asp:SqlDataSource>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlGroup" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True" 
                                                          DataTextField="Group_Descr"
                                                          DataValueField="Group_Cd" 
                                                          DataSourceID="sdsGroup"
                                                          OnSelectedIndexChanged="ddlGroup_SelectedIndexChanged"
                                                          Width="200px">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsGroup" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsGroup_Selected"
                                                           SelectCommand="Get_Group_Codes" 
                                                           SelectCommandType="StoredProcedure">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlOrg" 
                                                                          Name="OrgCode" 
                                                                          PropertyName="SelectedValue" 
                                                                          Type="String" />
                                                </SelectParameters>
                                         </asp:SqlDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br /><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblRegion" runat="server" Text="Region" />
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblSection" runat="server" Text="Section" />
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblArea" runat="server" Text="Area" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlRegion" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True" 
                                                          DataTextField="Region_Descr"
                                                          DataValueField="Region_Cd" 
                                                          DataSourceID="sdsRegion"
                                                          OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged"
                                                          Width="200px">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsRegion" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsRegion_Selected"
                                                           SelectCommand="Get_Region_Codes" 
                                                           SelectCommandType="StoredProcedure">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlGroup" 
                                                                          Name="GroupCode" 
                                                                          PropertyName="SelectedValue" 
                                                                          Type="String" />
                                                </SelectParameters>
                                         </asp:SqlDataSource>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSection" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True" 
                                                          DataTextField="Section_Descr"
                                                          DataValueField="Section_Cd" 
                                                          DataSourceID="sdsSection"
                                                          OnSelectedIndexChanged="ddlSection_SelectedIndexChanged"
                                                          Width="200px">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsSection" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsSection_Selected"
                                                           SelectCommand="Get_Section_Codes" 
                                                           SelectCommandType="StoredProcedure">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlRegion" 
                                                                          Name="RegCode" 
                                                                          PropertyName="SelectedValue" 
                                                                          Type="String" />
                                                </SelectParameters>
                                         </asp:SqlDataSource>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlArea" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True"  
                                                          DataTextField="Area_Descr"
                                                          DataValueField="Area_Cd" 
                                                          DataSourceID="sdsArea"
                                                          OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"
                                                          Width="200px">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsArea" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsArea_Selected"
                                                           SelectCommand="Get_Area_Codes" 
                                                           SelectCommandType="StoredProcedure">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlSection" 
                                                                          Name="SecCode" 
                                                                          PropertyName="SelectedValue" 
                                                                          Type="String" />
                                                </SelectParameters>
                                         </asp:SqlDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br /><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblJobcodes" runat="server" Text="Job Code(s)" />
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblWorkstates" runat="server" Text="Work State(s)" />
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblFLSA" runat="server" Text="FLSA Status" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtJobCodes" 
                                                     runat="server" 
                                                     ReadOnly="True"  
                                                     OnFocus="openPopUp('/WageNotice/JobCodes.aspx','JobCodeWindow', 650, 600); focusOnExec();" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtWorkState" 
                                                     runat="server" 
                                                     ReadOnly="True" 
                                                     OnFocus="openPopUp('WorkState.aspx','WorkStateWindow', 550, 750); focusOnExec();" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlFLSA" 
                                                          runat="server" 
                                                          AppendDataBoundItems="True"
                                                          AutoPostBack="True"  
                                                          DataTextField="FLSA_Status"
                                                          DataValueField="FLSA_Status" 
                                                          OnSelectedIndexChanged="ddlFLSA_SelectedIndexChanged"
                                                          Width="75px" 
                                                          DataSourceID="sdsFLSA">
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="sdsFLSA" 
                                                           runat="server" 
                                                           ConnectionString="<%$ ConnectionStrings:PROCESS360 %>" 
                                                           OnSelected="sdsFLSA_Selected"
                                                           SelectCommand="Get_FLSA_Status" 
                                                           SelectCommandType="StoredProcedure">
                                         </asp:SqlDataSource>
                                    </td>
                                </tr>
                                <tr>  
                                    <td><br /><br />
                                         <asp:Button ID="btnTMCount" 
                                                     runat="server" 
                                                     Text="TM Count" 
                                                     onclick="btnTMCount_Click"
                                                     ToolTip="Get a count of how many Team Members this criteria will send a document to." />
                                    </td>
                                    <td  align="left" colspan='2' valign="bottom">
                                        <asp:Label ID="lblTMCount" runat="server" Width="300px"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:View>
                    </asp:MultiView>
                </td>
            </tr>
            <tr>
                <td>
                    <br /><br />
                    <%--     Validation Summary - Displays a MessageBox     --%>
                    <asp:ValidationSummary ID="ValidationSummary1" 
                                           runat="server" 
                                           ShowMessageBox="True" 
                                           ShowSummary="False" 
                                           HeaderText="Please enter the following information:" />
                </td>
            </tr>
            <tr>
                <td align="center" colspan="5">
                    <asp:Button ID="btnSave" 
                                runat="server" 
                                Text="    Save    " 
                                onclick="btnSave_Click" 
                                ToolTip="Save your entered criteria and close the screen" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancel" 
                                runat="server" 
                                onclick="btnCancel_Click" 
                                ToolTip="Cancel any changes made and close the screen" 
                                Text="   Cancel   " />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnReset" 
                                runat="server" 
                                Text="Reset" 
                                onclick="btnReset_Click"
                                ToolTip="Reset the selection criteria to the default values." />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
