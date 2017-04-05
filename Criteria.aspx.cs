using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using EPF.BusinessObjects;
using EPF.DataAccess;
using EPF.Utilities;

//------------------------------------------------------------------------------
//                                    Criteria.aspx.cs
//
//      This is the code behind for the Criteria screen
// 
//------------------------------------------------------------------------------
// 
//                          Modification Control Log                           
//                                                                             
//    Date     By                 Description                                
//  --------  ---  -------------------------------------------------------------
//  04-10-15  JV   Initial creation of program                               
//  08/11/15  JV   Add Session.Remove("dtJobCodes") to InitializeScreen()
//                 Remove ListItem("All", "All") in GetProject() was causing duplicates 
//------------------------------------------------------------------------------

namespace EPF.DocUpload
{
    public partial class Criteria : System.Web.UI.Page
    {
        #region Member Variables

        // Module Level Variables
        HRSCLogsDA _HRSCLogsDA = null;

        #endregion

　
        // ON SAVE BTN, below logic
        // NOTES: Criteria = Employee ID, Write 1 rcd to Other_Criteria w/ Proj id, criteria = "Employee ID" and nothing else but mod date , user,
        //                                Write 1 rcd to Upload_Doc_TM_Status for every emplid selected.
        //        Criteria = Other Criteria, Write 1 rcd to Other_Criteria w/ Proj id, criteria = "Other Criteria" and all other columns based on DropDownLists & Textboxes.
        //                                   Write 1 record to Other_Criteria_Job_Codes for EACH job code selected.
        //                                   Write 1 record to Other_Criteria_Work_State for EACH work state selected.
        //                                   Write 1 rcd to Upload_Doc_TM_Status for every emplid based on the Other_Criteria table values.  Those values
        //                                          will be used as parameters for the selection criteria against the Full_Roster DB table.

　
        // If the project Effective Date has been triggered, do not allow chgs on the Criteria screen. Display only..........................

        /// <summary>
        /// Page Load   
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize the Logging Class
            _HRSCLogsDA = new HRSCLogsDA();

            if (!IsPostBack)
            {
                trMultiView.Visible = false;

                // get the Session Variables
                string emplId = Session["EmplId"].ToString();

                // Initialize the screen
                InitializeScreen();
            }

            // Clear out the TM count label
            lblTMCount.Text = string.Empty;

            // Check for Session Variables
            if (Session.SessionID != null)
            {
                // Check for JobCodes
                if (Session["JobCodes"] != null)
                {
                    // Retrieve the Session Variable
                    txtJobCodes.Text = Session["JobCodes"].ToString();
                }

                // Check for WorkState
                if (Session["TempWorkState"] != null)
                {
                    // Retrieve the Session Variable
                    txtWorkState.Text = Session["TempWorkState"].ToString();
                }
            }

            if (ddlCriteria.SelectedItem.Text == "Other Criteria")
            {
                Hide_Dropdownlist();
            }
        }

　
        /// <summary>
        /// Cancel button Click   
        /// </summary>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlCriteria.SelectedItem.Text == "Other Criteria")
                {
                    Session["TempWorkState"] = string.Empty;
                    Session["Jobcodes"] = null;
                }
                
                //  Log the Action
                _HRSCLogsDA.Insert("Criteria Screen - Cancel button Clicked");

                // Close the Window and Refresh the page
                ClientScript.RegisterStartupScript(Page.GetType(), "CloseWindow", "CloseWindow();", true);
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - btnCancel_Click Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Cancel button logic.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// New button Click   
        /// </summary>
        protected void btnNew_Click(object sender, CommandEventArgs e)
        {
            hdnNewRecord.Value = "True";

            // Set the GridView to the first page
            gvEmplId.PageIndex = 0;

            // Sort by EmplID Ascending
            gvEmplId.Sort("EmplId", SortDirection.Ascending);

            // Set the first row of GridView to be Editable
            gvEmplId.EditIndex = 0;

            // Place the cursor on the first field in the first DataRow
            hdnSelectedCell.Value = "txtEmplId";
        }

　
        /// <summary>
        /// Reset Button Click   
        /// </summary>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            // Reset selection criteria to the default values (like it is a brand new project for the first time)
            try
            {
                if (ddlCriteria.SelectedItem.Text == "Other Criteria")
                {
                    // Clear out the dropdownlists and textboxes
                    ddlExec.SelectedIndex = 0;
                    ddlExec.DataBind();
                    ddlOrg.SelectedIndex = 0;
                    ddlOrg.DataBind();
                    ddlGroup.SelectedIndex = 0;
                    ddlGroup.DataBind();
                    ddlRegion.SelectedIndex = 0;
                    ddlRegion.DataBind();
                    ddlSection.SelectedIndex = 0;
                    ddlSection.DataBind();
                    ddlArea.SelectedIndex = 0;
                    ddlArea.DataBind();

                    Session["dtJobCodes"] = string.Empty;
                    txtJobCodes.Text = string.Empty;

                    Session["TempWorkState"] = string.Empty;
                    txtWorkState.Text = string.Empty;

                    //FLSA dropdown
                    ListItem allListItem = new ListItem("All", "All");
                    ddlFLSA.Items.Add(allListItem);
                    ddlFLSA.SelectedValue = "All";
                    ddlFLSA.SelectedIndex = 0;
                    ddlFLSA.DataBind();

                    Hide_Dropdownlist();

                    //  Log the Action
                    _HRSCLogsDA.Insert("Criteria reset button clicked for the Other Criteria controls");
                }
                else
                {
                    // Clear out the Employee Id datatable and reload the gridview
                    Session["dtEmplIds"] = null;
                    gvEmplId.DataBind();

                    //  Log the Action
                    _HRSCLogsDA.Insert("Criteria reset button clicked for the Employee Ids gridview");
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - btnReset_Click Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Reset button logic.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Save button Click   
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // LOAD Session variables for DB Update/Insert in DocUpload.aspx.cs

            try
            {
                // Let WageNotice\JobCodes.apsx know we are coming from DocUpload\Criteria.aspx
                // All done, set to "No"
                Session["CriteriaJobCode"] = "No";

                if (ddlCriteria.SelectedItem.Text == "Other Criteria")
                {
                    Session["Exec"]    = ddlExec.SelectedValue.ToString();
                    Session["Org"]     = ddlOrg.SelectedValue.ToString();
                    Session["Group"]   = ddlGroup.SelectedValue.ToString();
                    Session["Region"]  = ddlRegion.SelectedValue.ToString();
                    Session["Section"] = ddlSection.SelectedValue.ToString();
                    Session["Area"]    = ddlArea.SelectedValue.ToString();

                    Session["FLSA"]    = ddlFLSA.SelectedValue.ToString();
                    
                    Session["SavedJobCodes"]  = txtJobCodes.Text.ToString();            // blank means all Jobcodes or csv format xxxxxx, xxxxxx, xxxxxx
                    Session["WorkState"] = txtWorkState.Text.ToString();                // blank means all States or csv format AZ, FL, CA
                    
                    Session["Criteria"] = ddlCriteria.SelectedItem.Text.ToString();

                    //Cleanup
                    Session["dtEmplIds"] = null;
                    Session["dtEmplIdsFromDocUpload"] = null;

                    //  Log the Action
                    _HRSCLogsDA.Insert("Criteria Screen - Save button Clicked");

                    // Close the Window and Refresh the page
                    ClientScript.RegisterStartupScript(Page.GetType(), "CloseWindow", "CloseWindow();", true);
                }
                else
                {
                    // Session["dtEmplIds"] is loaded in EmployeeIdDA.cs so just load into the session variable from DocUpload.aspx.cs
                    //  Type DataTable.

                    DataTable wrkEmplIds = (DataTable)Session["dtEmplIds"];

                    //Check to see if the DataTable has any EmplIds in it.
                    if (wrkEmplIds.Rows[0]["EmplId"].ToString() == "" || wrkEmplIds.Rows[0]["EmplId"].ToString() == "00000000000")
                    {
                        // Display a Messagebox
                        AlertMessage.Show("Please add at least 1 Employee Id", this.Page);
                    }
                    else  
                    {
                        // Save the data
                        Session["dtEmplIdsFromDocUpload"] = ((DataTable)Session["dtEmplIds"]).Copy();
                        Session["Criteria"] = ddlCriteria.SelectedItem.Text.ToString();

                        //Cleanup
                        Session["Exec"]     = string.Empty;
                        Session["Org"]      = string.Empty;
                        Session["Group"]    = string.Empty;
                        Session["Region"]   = string.Empty;
                        Session["Section"]  = string.Empty;
                        Session["Area"]     = string.Empty;
                        Session["FLSA"]     = string.Empty;

                        Session["dtJobCodes"]    = null;
                        Session["SavedJobCodes"] = string.Empty;   
                        Session["WorkState"]     = string.Empty;   
                        Session["TempWorkState"] = string.Empty;   

                        //  Log the Action
                        _HRSCLogsDA.Insert("Criteria Screen - Save button Clicked");

                        // Close the Window and Refresh the page
                        ClientScript.RegisterStartupScript(Page.GetType(), "CloseWindow", "CloseWindow();", true);
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - btnSave_Click Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Save button logic.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Team Member Count button Click   
        /// </summary>
        protected void btnTMCount_Click(object sender, EventArgs e)
        {
            try
            {
                Doc_Upload_Single_TM_StatusBO duBO = new Doc_Upload_Single_TM_StatusBO();

                if (ddlExec.SelectedValue.ToString() != "All")
                {
                    duBO.ExecCode = ddlExec.SelectedValue.ToString();
                }
                if (ddlOrg.SelectedValue.ToString() != "All")
                {
                    duBO.OrgCode = ddlOrg.SelectedValue.ToString();
                }
                if (ddlGroup.SelectedValue.ToString() != "All")
                {
                    duBO.GroupCode = ddlGroup.SelectedValue.ToString();
                }
                if (ddlRegion.SelectedValue.ToString() != "All")
                {
                    duBO.RegionCode = ddlRegion.SelectedValue.ToString();
                }
                if (ddlSection.SelectedValue.ToString() != "All")
                {
                    duBO.SectionCode = ddlSection.SelectedValue.ToString();
                }
                if (ddlArea.SelectedValue.ToString() != "All")
                {
                    duBO.AreaCode = ddlArea.SelectedValue.ToString();
                }
                if (ddlFLSA.SelectedValue.ToString() != "All")
                {
                    duBO.FLSAStatus = ddlFLSA.SelectedValue.ToString();
                }

                duBO.JobCodes = txtJobCodes.Text.ToString();             // blank means all Jobcodes
                duBO.WorkStates = txtWorkState.Text.ToString();          // blank means all States

                DocumentUploadTMStatusDA tmDA = new DocumentUploadTMStatusDA();
                DataTable dtTMCount = tmDA.GetPotentialEmployeeCount(duBO);

                DataRow drTMackCount = dtTMCount.Rows[0];
                Int32 tmCount = Convert.ToInt32(drTMackCount["Total_Count"].ToString());
                lblTMCount.Text = tmCount.ToString("N0") + " Team Members will receive a document";
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - btnTMCount_Click Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Team Mbr button logic.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        
        /// <summary>
        /// ddlArea - SelectedIndexChanged   
        /// </summary>
        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Area selected: {0}",
                                           ddlArea.SelectedItem);
                _HRSCLogsDA.Insert(msg);
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlExec_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Executive drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlCriteria - SelectedIndexChanged   
        /// </summary>
        protected void ddlCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Criteria selected: {0}",
                                           ddlCriteria.SelectedItem);
                _HRSCLogsDA.Insert(msg);

                if (ddlCriteria.SelectedItem.Text == "Employee Id")
                {
                    trMultiView.Visible = true;
                    mvCriteria.ActiveViewIndex = 0;
                    gvEmplId.DataBind();

                    // Clear out all ddl, textboxes from "other criteria".............
                    ddlExec.SelectedValue = "All";
                    ddlExec.SelectedIndex = 0;

                    ddlOrg.SelectedValue = "All";
                    ddlOrg.SelectedIndex = 0;

                    ddlGroup.SelectedValue = "All";
                    ddlGroup.SelectedIndex = 0;

                    ddlRegion.SelectedValue = "All";
                    ddlRegion.SelectedIndex = 0; 

                    ddlSection.SelectedValue = "All";
                    ddlSection.SelectedIndex = 0;

                    ddlArea.SelectedValue = "All";
                    ddlArea.SelectedIndex = 0;

                    ListItem allListItem = new ListItem("All", "All");
                    ddlFLSA.Items.Add(allListItem);
                    ddlFLSA.SelectedValue = "All";
                    ddlFLSA.SelectedIndex = 0;

                    Session["dtJobCodes"] = null;
                    Session["JobCodes"] = null; 
                    txtJobCodes.Text = String.Empty;

                    Session["TempWorkState"] = string.Empty;
                    txtWorkState.Text = String.Empty;
                }
                else if (ddlCriteria.SelectedItem.Text == "Other Criteria")
                {
                    trMultiView.Visible = true;
                    mvCriteria.ActiveViewIndex = 1;

                    //Clear out the EmplIds
                    Session["dtEmplIds"] = null;

                    // Let WageNotice\JobCodes.apsx know we are coming from DocUpload\Criteria.aspx
                    Session["CriteriaJobCode"] = "Yes";
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - Criteria DropDown List() SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem with the Criteria DropDown List on the Document Upload Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlExec - SelectedIndexChanged   
        /// </summary>
        protected void ddlExec_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Executive selected: {0}",
                                           ddlExec.SelectedItem);
                _HRSCLogsDA.Insert(msg);

                Initialize_Dropdownlists();

                Hide_Dropdownlist();
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlExec_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Executive drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlFLSA - SelectedIndexChanged   
        /// </summary>
        protected void ddlFLSA_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Document Upload Screen - FLSA selected: {0}",
                                           ddlFLSA.SelectedItem);
                _HRSCLogsDA.Insert(msg);
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlFLSA_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the FLSA drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlGroup - SelectedIndexChanged   
        /// </summary>
        protected void ddlGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Group selected: {0}",
                                           ddlGroup.SelectedItem);
                _HRSCLogsDA.Insert(msg);

                // Clear out other drop down Lists
                ddlRegion.Items.Clear();
                ddlSection.Items.Clear();
                ddlArea.Items.Clear();

                // Add the first list item to the drop down lists
                ListItem allListItem = new ListItem("All", "All");
                ddlRegion.Items.Add(allListItem);
                ddlSection.Items.Add(allListItem);
                ddlArea.Items.Add(allListItem);

                // Select the first item in the lists
                ddlRegion.SelectedIndex = -1;
                ddlSection.SelectedIndex = -1;
                ddlArea.SelectedIndex = -1;

                Hide_Dropdownlist();
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlGroup_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Group drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlOrg - SelectedIndexChanged   
        /// </summary>
        protected void ddlOrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Org selected: {0}",
                                           ddlOrg.SelectedItem);
                _HRSCLogsDA.Insert(msg);

                // Clear out other drop down Lists
                ddlGroup.Items.Clear();
                ddlRegion.Items.Clear();
                ddlSection.Items.Clear();
                ddlArea.Items.Clear();

                // Add the first list item to the drop down lists
                ListItem allListItem = new ListItem("All", "All");
                ddlGroup.Items.Add(allListItem);
                ddlRegion.Items.Add(allListItem);
                ddlSection.Items.Add(allListItem);
                ddlArea.Items.Add(allListItem);

                // Select the first item in the lists
                ddlGroup.SelectedIndex = -1;
                ddlRegion.SelectedIndex = -1;
                ddlSection.SelectedIndex = -1;
                ddlArea.SelectedIndex = -1;

                Hide_Dropdownlist();
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlOrg_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Organization drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlRegion - SelectedIndexChanged   
        /// </summary>
        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Region selected: {0}",
                                           ddlRegion.SelectedItem);
                _HRSCLogsDA.Insert(msg);

                // Clear out other drop down Lists
                ddlSection.Items.Clear();
                ddlArea.Items.Clear();

                // Add the first list item to the drop down lists
                ListItem allListItem = new ListItem("All", "All");
                ddlSection.Items.Add(allListItem);
                ddlArea.Items.Add(allListItem);

                // Select the first item in the lists
                ddlSection.SelectedIndex = -1;
                ddlArea.SelectedIndex = -1;

                Hide_Dropdownlist();
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlRegion_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Region drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlSection - SelectedIndexChanged   
        /// </summary>
        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Log the Action
                string msg = string.Format("Criteria Screen - Section selected: {0}",
                                           ddlSection.SelectedItem);
                _HRSCLogsDA.Insert(msg);

                // Clear out other drop down Lists
                ddlArea.Items.Clear();

                // Add the first list item to the drop down lists
                ListItem allListItem = new ListItem("All", "All");
                ddlArea.Items.Add(allListItem);

                // Select the first item in the lists
                ddlArea.SelectedIndex = -1;

                Hide_Dropdownlist();
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - ddlSection_SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Section drop down list control.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Gridview - Edit First Row   
        /// </summary>
        private void EditFirstRow()
        {
            hdnNewRecord.Value = "True";

            // Creating the first row of GridView to be Editable
            gvEmplId.EditIndex = 0;

            hdnSelectedCell.Value = "txtEmplId";
        }

　
        /// <summary>
        /// // Load screen data from session variables passed in from DocUpload.aspx.cs
        /// </summary>
        private void GetProjectInfo()
        {
            try
            {
                int projectId = Convert.ToInt32(Session["ProjectId"]);

                if (Session["Criteria"] == null)
                {
                    Session["dtEmplIds"] = null;
                }
                else if (Session["Criteria"].ToString() == "Other Criteria")                     //Other Criteria
                {
                    Session["dtEmplIds"] = null;

                    trMultiView.Visible = true;
                    mvCriteria.ActiveViewIndex = 1;
                    ddlCriteria.SelectedIndex = 2;
                    ddlCriteria.DataBind();
                    txtJobCodes.Text = string.Empty;
                    txtWorkState.Text = string.Empty;

                    // Let WageNotice\JobCodes.apsx know we are coming from DocUpload\Criteria.aspx  ***WE NEED THIS***
                    Session["CriteriaJobCode"] = "Yes";

                    if (Session["SavedJobCodes"] != null)
                    {
                        Session["dtJobCodes"] = null;

                        JobCodeBO jcBO = new JobCodeBO();
                        JobCodeDA jcDA = new JobCodeDA();

                        string[] jobCodes = null;
                        jobCodes = Session["SavedJobCodes"].ToString().Split(',');

                        foreach (string jc in jobCodes)
                        {
                            jcBO.JobCode = jc;
                            jcDA.Insert(jcBO);
                        }

                        txtJobCodes.Text = Session["SavedJobCodes"].ToString();
                    }
                    else if (Session["dtJobCodesFromDocUpload"] != null)
                    {
                        // Prevents duplicates
                        Session["dtJobCodes"] = null;

                        // JobCodes for JobCodes screen
                        JobCodeBO jcBO = new JobCodeBO();
                        JobCodeDA jcDA = new JobCodeDA();

                        // Get the Employee Id Datatable
                        DataTable dtJobCodes = (DataTable)Session["dtJobCodesFromDocUpload"];
                        if (dtJobCodes.Rows.Count > 0)
                        {
                            // Load the txtJobCodes
                            foreach (DataRow dr in dtJobCodes.Rows)
                            {
                                if (txtJobCodes.Text.Length == 0)
                                {
                                    txtJobCodes.Text = string.Format("{0}",
                                                        dr["JobCode"].ToString());
                                }
                                else
                                {
                                    txtJobCodes.Text = string.Format("{0},{1}",
                                                        txtJobCodes.Text,
                                                        dr["JobCode"].ToString());
                                }

                                // Try to populate Session["dtJobCodes"] using JobCodesDA.cs Insert() from here....
                                // This is needed for an existing Project Id so if JobCodes were loaded they will be available
                                //  to pass along to the original JobCodes.aspx page.
                                jcBO.JobCode = dr["JobCode"].ToString();
                                jcDA.Insert(jcBO);
                            }
                        }
                    }
                    
                    // WorkState
                    if (Session["WorkState"] != null)
                    {
                        // Retrieve the Session Variable
                        txtWorkState.Text = Session["WorkState"].ToString();
                        Session["TempWorkState"] = txtWorkState.Text;
                    }
                    else if (Session["dtWorkStateFromDocUpload"] != null)
                    {
                        // Get the Employee Id Datatable
                        DataTable dtWorkState = (DataTable)Session["dtWorkStateFromDocUpload"];
                        if (dtWorkState.Rows.Count > 0)
                        {
                            // Load the txtWorkState
                            foreach (DataRow dr in dtWorkState.Rows)
                            {
                                if (txtWorkState.Text.Length == 0)
                                {
                                    txtWorkState.Text = string.Format("{0}",
                                                        dr["State_Code"].ToString());
                                }
                                else
                                {
                                    txtWorkState.Text = string.Format("{0},{1}",
                                                        txtWorkState.Text,
                                                        dr["State_Code"].ToString());
                                }
                            }

                            // TempWorkState is used in WorkState.aspx.cs
                            Session["TempWorkState"] = txtWorkState.Text.ToString();
                        }
                    }

                    //DropDownLists

                    // The "== sting.Empty" if statements will handle the values from the DB and then the ocBO
                    //  converting NULL values from the DB into string.empty in the BO class.
                    if (Session["Exec"].ToString() == string.Empty)   //EXEC 
                    {
                        ddlExec.SelectedValue = "All";
                        ddlExec.SelectedIndex = 0;
                        ddlExec.DataBind();
                    }
                    else
                    {
                        ddlExec.SelectedValue = Session["Exec"].ToString();   //EXEC value
                        ddlExec.DataBind();

                        if (Session["Org"].ToString() == string.Empty)        //ORG 
                        {
                            ddlOrg.SelectedValue = "All";
                            ddlOrg.SelectedIndex = 0;
                            ddlOrg.DataBind();
                        }
                        else
                        {
                            ddlOrg.SelectedValue = Session["Org"].ToString();  //ORG value
                            ddlOrg.DataBind();

                            if (Session["Group"].ToString() == string.Empty)   //Group 
                            {
                                ddlGroup.SelectedValue = "All";
                                ddlGroup.SelectedIndex = 0;
                                ddlGroup.DataBind();
                            }
                            else
                            {
                                ddlGroup.SelectedValue = Session["Group"].ToString();    //Group value
                                ddlGroup.DataBind();

                                if (Session["Region"].ToString() == string.Empty)      //Region 
                                {
                                    ddlRegion.SelectedValue = "All";
                                    ddlRegion.SelectedIndex = 0;
                                    ddlRegion.DataBind();
                                }
                                else                                              //Region value
                                {
                                    ddlRegion.SelectedValue = Session["Region"].ToString();
                                    ddlRegion.DataBind();

                                    if (Session["Section"].ToString() == string.Empty)   //Section 
                                    {
                                        ddlSection.SelectedValue = "All";
                                        ddlSection.SelectedIndex = 0;
                                        ddlSection.DataBind();
                                    }
                                    else                                                //Section value      
                                    {                                                  
                                        ddlSection.SelectedValue = Session["Section"].ToString();
                                        ddlSection.DataBind();

                                        if (Session["Area"].ToString() == string.Empty)   //Area 
                                        {
                                            ddlArea.SelectedValue = "All";
                                            ddlArea.SelectedIndex = 0;
                                            ddlArea.DataBind();
                                        }
                                        else                                              //Area value
                                        {                                                  
                                            ddlArea.SelectedValue = Session["Area"].ToString();
                                            ddlArea.DataBind();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //FLSA DropDownList
                    if (Session["FLSA"].ToString() == string.Empty)   
                    {
                        ListItem allListItem = new ListItem("All", "All");
                        ddlFLSA.Items.Add(allListItem);
                        ddlFLSA.SelectedValue = "All";
                        ddlFLSA.SelectedIndex = 0;
                        ddlFLSA.DataBind();
                    }
                    else
                    {
                        ddlFLSA.SelectedValue = Session["FLSA"].ToString();
                        ddlFLSA.DataBind();
                    }
                }
                else if (Session["Criteria"].ToString() == "Employee Id")
                {
                    trMultiView.Visible = true;
                    mvCriteria.ActiveViewIndex = 0;
                    ddlCriteria.SelectedIndex = 1;
                    ddlCriteria.DataBind();

                    if (Session["dtEmplIdsFromDocUpload"] != null)
                    {
                        // Prevents duplicates
                        Session["dtEmplIds"] = null;

                        if (((DataTable)Session["dtEmplIdsFromDocUpload"]).Rows.Count > 0)
                        {
                            // Load the odsEmplId
                            EmployeeIdBO emplIdBO = new EmployeeIdBO();
                            EmployeeIdDA emplIdDA = new EmployeeIdDA();

                            foreach (DataRow dr in ((DataTable)Session["dtEmplIdsFromDocUpload"]).Rows)
                            {
                                emplIdBO.EmplId = dr["EmplId"].ToString();
                                emplIdDA.Insert(emplIdBO);
                            }
                        }
                    }
                }

                // For existing Projects, If EffectiveDate is < Todays date, allow edits,
                // otherwise, all controls are display only because the Project is active.
                if (Session["EffectiveDate"] != null)
                {
                    if (DateTime.Today >= Convert.ToDateTime(Session["EffectiveDate"]))
                    {
                        pnlQueryCriteria.Enabled = false;
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - Criteria existing project id GetRequestInfo() Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem retrieving the Project record for the Criteria page.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// gvEmplId - Data Bound   
        /// </summary>
        protected void gvEmplId_DataBound(object sender, EventArgs e)
        {
            try
            {
                // Check if the Session Info is in the Session object
                if (Session.SessionID != null && Session["dtEmplIds"] != null)
                {
                    // Get the Employee Id Datatable
                    DataTable dtEmplIds = (DataTable)Session["dtEmplIds"];

                    if (dtEmplIds.Rows.Count == 0 || (decimal)((DataRow)dtEmplIds.Rows[0])["Id"] == 0)
                    {
                        EditFirstRow();
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - gvEmplId_DataBound Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem creating a row in the grid.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// gvEmplId - Page Index Changing   
        /// </summary>
        protected void gvEmplId_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            RemoveNewRecord();
        }

　
        /// <summary>
        /// gvEmplId - Row Canceling Edit   
        /// </summary>
        protected void gvEmplId_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            RemoveNewRecord();
        }

　
        /// <summary>
        /// gvEmplId - Row Created   
        /// </summary>
        protected void gvEmplId_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // Process Header
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    // Create a New Record button
                    Button btnNew = new Button();
                    btnNew.ID = "btnNew";
                    btnNew.Text = "Add";
                    btnNew.Command += new CommandEventHandler(btnNew_Click);
                    e.Row.Cells[0].Controls.Add(btnNew);
                }

                // Process DataRow
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItem != null)
                    {
                        DataRowView drv = (DataRowView)e.Row.DataItem;

                        // Check for New Record
                        //if ((int)drv["Id"] == 0)
                        if ((decimal)drv["Id"] == 0)
                        {
                            // Change the Text on the Update Button to Insert for a New Record
                            ((Button)e.Row.Cells[0].Controls[0]).Text = "Insert";
                        }
                    }

                    // Check for Edit Button
                    if (((Button)e.Row.Cells[0].Controls[0]).Text == "Edit")
                    {
                        // Hide the Edit Button
                        ((Button)e.Row.Cells[0].Controls[0]).Style["display"] = "None";
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - gvEmplId Row Created Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem creating a row in the grid.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// gvEmplId - Row Data Bound   
        /// </summary>
        protected void gvEmplId_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // Process DataRow
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Button b = (Button)e.Row.Cells[0].Controls[2];
                    DataRowView drv = (DataRowView)e.Row.DataItem;

                    // Check for Delete Button
                    if (b.Text == "Delete")
                    {
                        // Add a confirmation to the Delete Button
                        b.Attributes["onclick"] = "javascript:if (! confirm('Are you sure you want to delete this record " + drv["EmplId"] + "?')) " +
                                                  "{return false;}";
                    }

                    // Check if row is NOT in Edit mode
                    // Check if row is in edit mode
                    if (Session["EffectiveDate"] != null)
                    {
                        if (DateTime.Today < Convert.ToDateTime(Session["EffectiveDate"]))
                        {
                            if (e.Row.RowState != DataControlRowState.Edit
                                && e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate))
                            {
                                // Undate each cell in the row
                                for (int i = 1; i < e.Row.Cells.Count; i++)
                                {
                                    // Change the Background Color of the cell during Hoover
                                    GridFormatting.GridViewCellStyle(e.Row.Cells[i]);

                                    // Set OnClick event to save the Id of the cell selected
                                    e.Row.Cells[i].Attributes["onclick"] = "saveClientId('txtEmplId'); " + Page.ClientScript.GetPostBackClientHyperlink(gvEmplId, "Edit$" + e.Row.RowIndex);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.Row.RowState != DataControlRowState.Edit
                            && e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate))
                        {
                            // Undate each cell in the row
                            for (int i = 1; i < e.Row.Cells.Count; i++)
                            {
                                // Change the Background Color of the cell during Hoover
                                GridFormatting.GridViewCellStyle(e.Row.Cells[i]);

                                // Set OnClick event to save the Id of the cell selected
                                e.Row.Cells[i].Attributes["onclick"] = "saveClientId('txtEmplId'); " + Page.ClientScript.GetPostBackClientHyperlink(gvEmplId, "Edit$" + e.Row.RowIndex);
                            }
                        }
                    }
                }

                
                if (e.Row.RowState == DataControlRowState.Edit || e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Alternate))
                {
                    // Get the fields from the row with special Edit Templates
                    DataRowView drv = (DataRowView)e.Row.DataItem;
                    string emplId = drv["EmplId"].ToString();

                    // Retrieve the controls from the row 
                    TextBox txtEmplId = (TextBox)e.Row.FindControl("txtEmplId");

                    // Populate the controls
                    txtEmplId.Text = emplId;
                }
                    
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - gvEmplId Row Data Bound Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem creating a row in the grid.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// gvEmplId - Row Deleted   
        /// </summary>
        protected void gvEmplId_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            RemoveNewRecord();
        }

　
        /// <summary>
        /// GridView - Row Editing   
        /// </summary>
        protected void gvEmplId_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Check for New Record
            if (hdnNewRecord.Value == "True")
            {
                // Remove the New Record
                RemoveNewRecord();

                // Adjust for removing the New Record
                e.NewEditIndex = e.NewEditIndex - 1;
            }
        }

　
        /// <summary>
        /// gvEmplId - Row Updated   
        /// </summary>
        protected void gvEmplId_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            hdnNewRecord.Value = "False";
        }

　
        /// <summary>
        /// gvEmplId - Row Updating   
        /// </summary>
        protected void gvEmplId_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                // Retrieve the row being edited.
                GridViewRow row = gvEmplId.Rows[e.RowIndex];

                // Retrieve the controls from the row
                TextBox txtEmplId = (TextBox)row.FindControl("txtEmplId");

                e.NewValues["EmplId"] = txtEmplId.Text;
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - gvEmplId Row Updating Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem updating a row in the grid.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Hides the lower level Dropdownlists when All is selected
        /// </summary>
        private void Hide_Dropdownlist()
        {
            try
            {
                // Hide the Organization drop down lists when All Executives are selected
                if (ddlExec.SelectedIndex == 0)
                {
                    lblOrg.Visible = false;
                    ddlOrg.Visible = false;
                    lblGroup.Visible = false;
                    ddlGroup.Visible = false;
                    lblRegion.Visible = false;
                    ddlRegion.Visible = false;
                    lblSection.Visible = false;
                    ddlSection.Visible = false;
                    lblArea.Visible = false;
                    ddlArea.Visible = false;
                }
                else
                {
                    lblOrg.Visible = true;
                    ddlOrg.Visible = true;

                    // Hide the Group drop down lists when All Organizations are selected
                    if (ddlOrg.SelectedIndex == 0)
                    {
                        lblGroup.Visible = false;
                        ddlGroup.Visible = false;
                        lblRegion.Visible = false;
                        ddlRegion.Visible = false;
                        lblSection.Visible = false;
                        ddlSection.Visible = false;
                        lblArea.Visible = false;
                        ddlArea.Visible = false;
                    }
                    else
                    {
                        lblGroup.Visible = true;
                        ddlGroup.Visible = true;

                        // Hide the Region drop down lists when All Groups are selected
                        if (ddlGroup.SelectedIndex == 0)
                        {
                            lblRegion.Visible = false;
                            ddlRegion.Visible = false;
                            lblSection.Visible = false;
                            ddlSection.Visible = false;
                            lblArea.Visible = false;
                            ddlArea.Visible = false;
                        }
                        else
                        {
                            lblRegion.Visible = true;
                            ddlRegion.Visible = true;

                            // Hide the Section drop down lists when All Regions are selected
                            if (ddlRegion.SelectedIndex == 0)
                            {
                                lblSection.Visible = false;
                                ddlSection.Visible = false;
                                lblArea.Visible = false;
                                ddlArea.Visible = false;
                            }
                            else
                            {
                                lblSection.Visible = true;
                                ddlSection.Visible = true;

                                // Hide the Area drop down lists when All Sections are selected
                                if (ddlSection.SelectedIndex == 0)
                                {
                                    lblArea.Visible = false;
                                    ddlArea.Visible = false;
                                }
                                else
                                {
                                    lblArea.Visible = true;
                                    ddlArea.Visible = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - Hide_Dropdownlist Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem with the Criteria controls.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Initialize the Dropdown lists   
        /// </summary>
        private void Initialize_Dropdownlists()
        {
            try
            {
                // Clear out other drop down Lists
                ddlOrg.Items.Clear();
                ddlGroup.Items.Clear();
                ddlRegion.Items.Clear();
                ddlSection.Items.Clear();
                ddlArea.Items.Clear();

                // Add the first list item to the drop down lists
                ListItem allListItem = new ListItem("All", "All");
                ddlOrg.Items.Add(allListItem);
                ddlGroup.Items.Add(allListItem);
                ddlRegion.Items.Add(allListItem);
                ddlSection.Items.Add(allListItem);
                ddlArea.Items.Add(allListItem);

                // Select the first item in the lists
                ddlOrg.SelectedIndex = 0;
                ddlGroup.SelectedIndex = 0;
                ddlRegion.SelectedIndex = 0;
                ddlSection.SelectedIndex = 0;
                ddlArea.SelectedIndex = 0;
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - Initialize_Dropdownlists Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem Initializing the Criteria controls.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Initializes the screen the first time
        /// </summary>
        private void InitializeScreen()
        {
            try
            {
                Session["CriteriaScreen"] = "Opened";    //Used in DocUpload.aspx.cs

                // Load screen data from session variables passed in from DocUpload.aspx.cs 
                GetProjectInfo();
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - Criteria Class Initialize Screen Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem initializing the Criteria Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// odsEmplId - Deleted   
        /// </summary>
        protected void odsEmplId_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - odsEmplId Deleted Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);
                }

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// odsEmplId - Selected   
        /// </summary>
        protected void odsEmplId_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - odsEmplId Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);
                }

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// odsEmplId - Updated   
        /// </summary>
        protected void odsEmplId_Updated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - odsEmplId Updated Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);
                }

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// Remove new record from the gridview   
        /// </summary>
        private void RemoveNewRecord()
        {
            try
            {
                // Check for New Record
                if (hdnNewRecord.Value == "True")
                {
                    hdnNewRecord.Value = "False";

                    // Delete the New Record
                    EmployeeIdDA employeeIdDA = new EmployeeIdDA();
                    EmployeeIdBO employeeIdBO = new EmployeeIdBO();
                    employeeIdBO.Id = 0;

                    employeeIdDA.Delete(employeeIdBO);
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - RemoveNewRecord Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem removing a row in the grid.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// sdsArea - Record Selected   
        /// </summary>
        protected void sdsArea_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsArea Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Area drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsExec - Record Selected   
        /// </summary>
        protected void sdsExec_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsExec Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Executive drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsCriteria - Record Selected   
        /// </summary>
        protected void sdsCriteria_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsCriteria Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Criteria drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsFLSA - Record Selected   
        /// </summary>
        protected void sdsFLSA_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsFLSA Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the FLSA drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsGroup - Record Selected   
        /// </summary>
        protected void sdsGroup_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsGroup Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Group drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsOrg - Record Selected   
        /// </summary>
        protected void sdsOrg_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsOrg Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Organization drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsRegion - Record Selected   
        /// </summary>
        protected void sdsRegion_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsRegion Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Region drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

　
        /// <summary>
        /// sdsSection - Record Selected   
        /// </summary>
        protected void sdsSection_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Retrieve the Error Message from the Session Variable
                    errMsg = ((InternalException)inner).UserFriendlyMsg;
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsSection Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);
                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);

                    // Save a User Friendly Error Message in Session Variable
                    errMsg = "There was a problem selecting from the Section drop down list.  If the problem persists, please contact Technical Support.";
                }

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

    }
}
