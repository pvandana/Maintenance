using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPF.DataAccess;
using EPF.BusinessObjects;
using EPF.Utilities;
using Ionic.Zip;

//------------------------------------------------------------------------------
//                                    DocUpload.aspx.cs
//
//      This is the code behind for the Document Upload screen 
// 
//------------------------------------------------------------------------------
// 
//                          Modification Control Log                           
//                                                                             
//    Date     By                 Description                                
//  --------  ---  -------------------------------------------------------------
//  04-09-15  JV   Initial creation of program                               
//  08-07-15  JV   On ASPX page chgd SP for ddlBarcodes from Get_Barcodes to 
//                    Get_Barcodes_For_DocUpload.  Old SP was returning every barcode
//                    and not just the barcodes selected in the Maint Screen.
//  05-03-16  RB   Modified to retrieve the "Other Criteria" data for previously saved projects
//------------------------------------------------------------------------------

namespace EPF.DocUpload
{
    public partial class DocUpload : System.Web.UI.Page
    {
        #region Member Variables

        // Module Level Variables
        HRSCLogsDA _HRSCLogsDA = null;
        string _emplId = string.Empty;

        #endregion

　
        /// <summary>
        /// Page Load   
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize the Logging Class
            _HRSCLogsDA = new HRSCLogsDA();

            _emplId = Session["EmplId"].ToString();

            if (!IsPostBack)
            {
                //// Filter to only show projects the LOB created
                //if ((bool)Session["HasAckLOBRole"])
                //{
                //    hdnProcessorId.Value = _emplId;
                //}

                RemoveSessionVariables();

                // Initialize the screen
                InitializeScreen();

                // Used for validation and set as a hidden field
                txtTodaysDate.Text = DateTime.Today.ToShortDateString();
            }
        }
        

        /// <summary>
        /// Cancel button Click   
        /// </summary>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //  Log the Action
            _HRSCLogsDA.Insert("Document Upload Screen - Cancel button Clicked");

            //// Check for Temp Folder and delete if it exists for this Team Member
            string emplId = Session["EmplId"].ToString();
            string fullFolderName = string.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), emplId);

            if (Directory.Exists(fullFolderName))
            {
                Directory.Delete(fullFolderName, true);

                //  Log the Action
                _HRSCLogsDA.Insert(string.Format("Document Upload Screen - Deleted Directory: {0}", fullFolderName));
            }

            // Reset all controls and session variables
            InitializeScreen();
            RemoveSessionVariables();
        }

        /// <summary>
        /// Save button Click   
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string alertMessage = string.Empty;
            string finalUploadDirectory = string.Empty;
            string projectId = "0";
            string projectName = string.Empty;
            string success = string.Empty;
            string uploadDirectory = string.Empty;

            bool saved = false;

            try
            {
                // via sp.Update_Insert_Document_Upload
                if (ddlUploadCriteria.SelectedItem.Text == "Batch File")
                {
                    if (Session["ProjectId"] != null)
                    {
                        saved = ProcessBatch();
                    }
                }
                else if (ddlUploadCriteria.SelectedItem.Text == "Single Document") //Single Document
                {
                    //Validate that the user chose a Selection Criteria (Employee Id or Other Criteria)
                    if (Session["Criteria"] != null)
                    {
                        //Get Selection Criteria value
                        string selectionCriteria = Session["Criteria"].ToString();

                        //Previously saved project, so delete most data, update data where appropriate
                        if (selectionCriteria == "Other Criteria" && Convert.ToInt32(Session["ProjectId"]) > 0)
                        {
                            //Session Values for Criteria are only available if the user opens the Criteria page
                            if (Session["CriteriaScreen"].ToString() == "Opened")
                            {
                                DocumentUploadDA duDA = new DocumentUploadDA();
                                duDA.Delete(Convert.ToInt32(Session["ProjectId"]));
                            }
                        }

                        if (selectionCriteria == "Other Criteria")
                        {
                            //Insert/Update data in the Doc Upload table
                            UpdateDocUploadTable();

                            //Session Values for Criteria are only available if the user opens the Criteria page
                            if (Session["CriteriaScreen"].ToString() == "Opened")
                            {
                                InsertSingleDocTMStatus(selectionCriteria);
                                InsertSingleDocOtherCriteria();
                                InsertSingleDocJobCodes();
                                InsertSingleDocWorkStates();
                            }
                        }
                        else //EmplId project
                        {
                            UpdateDocUploadTable();

                            if (Convert.ToInt32(Session["ProjectId"]) > 0)
                            {
                                if (Session["CriteriaScreen"].ToString() == "Opened")
                                {
                                    DocumentUploadDA duDA = new DocumentUploadDA();
                                    duDA.Delete(Convert.ToInt32(Session["ProjectId"]));

                                    InsertSingleDocTMStatus(selectionCriteria);
                                }
                            }
                        }

                        uploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), _emplId);

                        if (Directory.Exists(uploadDirectory))
                        {
                            projectId = Session["ProjectId"].ToString();
                            finalUploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), projectId);

                            //Rename the folder as the ProjectId
                            Directory.Move(uploadDirectory, finalUploadDirectory);

                            //  Log the Action
                            _HRSCLogsDA.Insert(string.Format("Document Upload Screen - Renamed Directory: {0} to {1}", uploadDirectory, finalUploadDirectory));
                        }

                        saved = true;
                    }
                    else
                    {
                        AlertMessage.Show("Selection Criteria button must be selected and along with one Criteria.", this.Page);
                    }
                }

                if (saved == true)
                {
                    //Set variable so as to not lose the Session value after calling RemoveSessionVariable in this block of code
                    projectId = Session["ProjectId"].ToString();
                    projectName = txtNewProjectName.Text;

                    // Display a Messagebox
                    AlertMessage.Show(string.Format("Successfully saved project: {0}", projectName), this.Page);

                    // Reset all controls and session variables
                    InitializeScreen();
                    RemoveSessionVariables();

                    //Set value back to same value before entering this block of code.  So it can be used to log message at end of procedure
                    Session["ProjectId"] = projectId;
                }
                else
                {
                    // Display a Messagebox
                    AlertMessage.Show(string.Format("Unable to save project: {0}", txtNewProjectName.Text), this.Page);
                }

                //  Log the Action
                _HRSCLogsDA.Insert(string.Format("Document Upload Screen - Save button Clicked for Project Id: {0}, Project Name: {1}", Session["ProjectId"].ToString(), projectName));
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - btnSave_Click Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Saving the Document Upload Data.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Insert Single Document Upload State Codes
        /// </summary>
        private void InsertSingleDocWorkStates()
        {
            string[] workStates = null;

            try
            {
                OtherCriteriaWorkStateBO ocwsBO = new OtherCriteriaWorkStateBO();
                ocwsBO.Project_Id = Convert.ToInt32(Session["ProjectId"]);
                ocwsBO.Modified_By = _emplId;

                OtherCriteriaWorkStateDA ocwsDA = new OtherCriteriaWorkStateDA();

                if (Session["WorkState"] != null)
                {
                    if (Session["WorkState"].ToString() != string.Empty)
                    {
                        workStates = Session["WorkState"].ToString().Split(',');

                        foreach (string ws in workStates)
                        {
                            ocwsBO.State_Code = ws;
                            ocwsDA.Insert(ocwsBO);
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
                // Database Error
                string errMsg = string.Format("{0} - InsertSingleDocJobCodes Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Inserting Single Document Work State Codes.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Insert Single Document Upload Job Codes
        /// </summary>
        private void InsertSingleDocJobCodes()
        {
            string[] jobCodes = null;

            try
            {
                OtherCriteriaJobCodesBO ocjcBO = new OtherCriteriaJobCodesBO();
                ocjcBO.Project_Id = Convert.ToInt32(Session["ProjectId"]);
                ocjcBO.Modified_By = _emplId;

                OtherCriteriaJobCodesDA ocjcDA = new OtherCriteriaJobCodesDA();

                if (Session["SavedJobCodes"] != null)
                {
                    if (Session["SavedJobCodes"].ToString() != string.Empty)
                    {
                        jobCodes = Session["SavedJobCodes"].ToString().Split(',');

                        foreach (string jc in jobCodes)
                        {
                            ocjcBO.JobCode = jc;
                            ocjcDA.Insert(ocjcBO);
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
                // Database Error
                string errMsg = string.Format("{0} - InsertSingleDocJobCodes Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Inserting Single Document Job Codes.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Insert Single Document Other Criteria values
        /// </summary>
        private void InsertSingleDocOtherCriteria()
        {
            try
            {
                OtherCriteriaBO ocBO = new OtherCriteriaBO();

                ocBO.Project_Id = Convert.ToInt32(Session["ProjectId"]);
                ocBO.Modified_By = _emplId;

                // If user did not select anything, the value will be "All"
                if (Session["Exec"].ToString() != "All")
                {
                    ocBO.Exec_Cd = Session["Exec"].ToString();
                }
                if (Session["Org"].ToString() != "All")
                {
                    ocBO.Org_Cd = Session["Org"].ToString();
                }
                if (Session["Group"].ToString() != "All")
                {
                    ocBO.Group_Cd = Session["Group"].ToString();
                }
                if (Session["Region"].ToString() != "All")
                {
                    ocBO.Region_Cd = Session["Region"].ToString();
                }
                if (Session["Section"].ToString() != "All")
                {
                    ocBO.Section_Cd = Session["Section"].ToString();
                }
                if (Session["Area"].ToString() != "All")
                {
                    ocBO.Area_Cd = Session["Area"].ToString();
                }
                if (Session["FLSA"].ToString() != "All")
                {
                    ocBO.FLSA_Status = Session["FLSA"].ToString();
                }

                //Update/Insert data into Other Criteria table
                OtherCriteriaDA ocDA = new OtherCriteriaDA();
                ocDA.UpdateInsert(ocBO);
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - InsertSingleDocOtherCriteria Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Inserting Single Document Other Criteria rows into the Other Criteria table.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Process Batch type Projects
        /// </summary>
        private bool ProcessBatch()
        {
            string alertMessage = string.Empty;
            string finalUploadDirectory = string.Empty;
            string projectId = string.Empty;
            string result = string.Empty;
            string uploadDirectory = string.Empty;

            bool saved = false;

            try
            {
                projectId = Session["ProjectId"].ToString();

                if (projectId == "0") //New Batch
                {
                    result = validateBatch();

                    if (result == string.Empty)
                    {
                        uploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), _emplId);
                        ZipFile zip = new ZipFile(string.Concat(uploadDirectory, "\\", txtZipFile.Text));
                        zip.ExtractAll(uploadDirectory, ExtractExistingFileAction.OverwriteSilently);

                        //Insert data into the Doc Upload table
                        UpdateDocUploadTable();

                        //Project Id should now have an Id > 0 since a row has been inserted in the Document_Upload table
                        projectId = Session["ProjectId"].ToString();
                        finalUploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), projectId);
                        zip.Dispose();
                        zip = null;

                        //Rename the folder as the ProjectId
                        Directory.Move(uploadDirectory, finalUploadDirectory);

                        InsertBatchTMStatusTable(finalUploadDirectory);

                        saved = true;
                    }
                    else
                    {
                        //Display message to Team Member
                        AlertMessage.Show(result, this.Page);

                        //  Log the Action
                        _HRSCLogsDA.Insert(result);
                    }
                }
                else //Updating a previously save Batch Project
                {
                    //Update data in Document Upload table if effective date is greater than or equal to today's date
                    if (DateTime.Today <= Convert.ToDateTime(txtEffDate.Text))
                    {
                        UpdateDocUploadTable();

                        saved = true;
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
                // Database Error
                string errMsg = string.Format("{0} - ProcessBatch Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Processing the Batch/Control files.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }

            return saved;
        }

        /// <summary>
        /// Insert Single Document Upload TM Status (Creates EmplId rows in table)
        /// </summary>
        private void InsertSingleDocTMStatus(string selectionCriteria)
        {
            try
            {
                DocumentUploadTMStatusDA dsDA = new DocumentUploadTMStatusDA();

                if (selectionCriteria == "Employee Id")
                {
                    DocumentUploadTMStatusBO dsBO = new DocumentUploadTMStatusBO();
                    dsBO.Project_Id = Convert.ToInt32(Session["ProjectId"]);
                    dsBO.Modified_By = _emplId;

                    DataTable dtEmplIds = (DataTable)Session["dtEmplIdsFromDocUpload"];

                    foreach (DataRow dr in dtEmplIds.Rows)
                    {
                        dsBO.File_Name = txtPDF.Text;
                        dsBO.EmplId = dr["EmplId"].ToString();
                        dsDA.Insert(dsBO);
                    }
                }
                else if (selectionCriteria == "Other Criteria") // "Selection may be blank if modifying a project"
                {
                    Doc_Upload_Single_TM_StatusBO duBO = new Doc_Upload_Single_TM_StatusBO();

                    duBO.Project_Id = Convert.ToInt32(Session["ProjectId"]);
                    duBO.File_Name = txtPDF.Text;

                    // If user did not select anything, the value will be "All"
                    if (Session["Exec"].ToString() != "All")
                    {
                        duBO.ExecCode = Session["Exec"].ToString();
                    }
                    if (Session["Org"].ToString() != "All")
                    {
                        duBO.OrgCode = Session["Org"].ToString();
                    }
                    if (Session["Group"].ToString() != "All")
                    {
                        duBO.GroupCode = Session["Group"].ToString();
                    }
                    if (Session["Region"].ToString() != "All")
                    {
                        duBO.RegionCode = Session["Region"].ToString();
                    }
                    if (Session["Section"].ToString() != "All")
                    {
                        duBO.SectionCode = Session["Section"].ToString();
                    }
                    if (Session["Area"].ToString() != "All")
                    {
                        duBO.AreaCode = Session["Area"].ToString();
                    }
                    if (Session["FLSA"].ToString() != "All")
                    {
                        duBO.FLSAStatus = Session["FLSA"].ToString();
                    }

                    duBO.JobCodes = Session["SavedJobCodes"].ToString();
                    duBO.WorkStates = Session["WorkState"].ToString();
                    duBO.Modified_By = _emplId;

                    //Insert data into the Upload Document TM Status table
                    dsDA.InsertSingleDocOtherCriteriaTMStatus(duBO);
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
                string errMsg = string.Format("{0} - InsertSingleDocTMStatus Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Inserting Single Document Upload TM Status table.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Insert Batch Upload TM Status (Creates an EmplId row in table for each EmplId found in the uploaded zip file)
        /// </summary>
        private void InsertBatchTMStatusTable(string finalUploadDirectory)
        {
            string controlFileName = string.Empty;
            string[] row = null;

            int projectId = 0;

            const int emplIdPosition = 0;
            const int fileNamePosition = 1;

            try
            {
                projectId  = Convert.ToInt32(Session["ProjectId"]);

                DocumentUploadTMStatusDA tmDA = new DocumentUploadTMStatusDA();
                DataTable dt = new DataTable();
                dt = tmDA.GetByProjectId(0);
                DataRow dtRow;

                controlFileName = string.Concat(finalUploadDirectory, "\\", Path.GetFileName(txtControlFile.Text));
                
                using (StreamReader readFile = new StreamReader(controlFileName))
                {
                    string line = readFile.ReadLine();
                    string[] header = line.Split(',');

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');

                        dtRow = dt.NewRow();
                        dtRow["TM_Status_Id"] = DBNull.Value;
                        dtRow["Project_Id"] = projectId;
                        dtRow["EmplId"] = row[emplIdPosition].ToString().PadLeft(11,'0');
                        dtRow["File_Name"] = row[fileNamePosition];
                        dtRow["Acknowledged_Date"] = DBNull.Value;
                        dtRow["Viewed_Date"] = DBNull.Value;
                        dtRow["Archived_Date"] = DBNull.Value;
                        dtRow["TM_Delete_Switch"] = DBNull.Value;
                        dtRow["Delete_Id"] = DBNull.Value;
                        dtRow["Delete_Reason_Other"] = DBNull.Value;
                        dtRow["Email_OptOut"] = DBNull.Value;
                        dtRow["Modified_Date"] = DateTime.Now;
                        dtRow["Modified_By"] = _emplId;
                        dt.Rows.Add(dtRow);
                    }
                }

                // Insert a row in the Doc Upload TM Status table for every row in the control file except the header
                tmDA.InsertBatchTMStatus(dt);
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - InsertBatchDocUploadTMStatusTable Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Inserting rows into the Document Upload TM Status table.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Insert Document Upload rows (Primary rows for both types of projects (Batch and Single Document)
        /// </summary>
        private void UpdateDocUploadTable()
        {
            try
            {
                DocumentUploadBO duBO = new DocumentUploadBO();
                duBO.Project_Id = Convert.ToInt32(Session["ProjectId"]);
                if (duBO.Project_Id == 0) //New Batch
                {
                    duBO.Project_Name = txtNewProjectName.Text;
                    duBO.Project_Created_By = _emplId;
                }
                else
                {
                    duBO.Project_Name = ddlProjects.SelectedItem.ToString();
                }

                //Only Single Document Uploads uses Session["Criteria"]
                if (Session["Criteria"] != null)
                {
                    duBO.Criteria_Name = Session["Criteria"].ToString();
                }
                else
                {
                    duBO.Criteria_Name = string.Empty;
                }
                duBO.Upload_Criteria_Id = Convert.ToInt32(ddlUploadCriteria.SelectedValue);
                duBO.Zip_File_Name = Path.GetFileName(txtZipFile.Text);
                duBO.Control_File_Name = Path.GetFileName(txtControlFile.Text);
                duBO.PDF_File_Name = Path.GetFileName(txtPDF.Text);
                duBO.Effective_Date = Convert.ToDateTime(txtEffDate.Text);
                duBO.Ack_Due_Date = Convert.ToDateTime(txtDueDate.Text);
                duBO.Primary_Barcode = ddlPrimaryBarcode.SelectedValue;
                duBO.Secondary_Barcode = ddlSecondaryBarcode.SelectedValue;
                duBO.Email_Notification = ddlEmailNotify.SelectedValue;
                duBO.Email_Reminder = ddlEmailReminder.SelectedValue;
                duBO.Modified_By = _emplId;

                DocumentUploadDA duDA = new DocumentUploadDA();
                duDA.UpdateInsert(duBO);
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - UpdateDocUploadTable Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem Updating the Document Upload table.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Validates the (Batch) Zip and Control files
        /// </summary>
        private string validateBatch()
        {
            string controlFileFullPath = string.Empty;
            string errMessage = string.Empty;
            string fileType = string.Empty;
            string[] row = null;
            string uploadDirectory = string.Empty;
            string zipFileFullPath = string.Empty;

            const int emplIdPosition = 0;
            const int fileNamePosition = 1;
            const string emplId = "emplid";
            const string file_name = "file_name";
            
            try
            {
                uploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), _emplId);
                controlFileFullPath = string.Concat(uploadDirectory, "\\", txtControlFile.Text);
                zipFileFullPath = string.Concat(uploadDirectory, "\\", txtZipFile.Text);

                ZipFile zip = new ZipFile(zipFileFullPath);
                        
                //Does the zip file contents match the number of row in the Control file minus the header row?
                if (zip.Count == File.ReadAllLines(controlFileFullPath).Length - 1)
                {
                    using (StreamReader readFile = new StreamReader(controlFileFullPath))
                    {
                        string line = readFile.ReadLine();
                        string[] header = line.Split(',');

                        //Does the header match what is expected -> EmplId,File_Name
                        if (string.Equals(header[emplIdPosition].Trim(), emplId, StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(header[fileNamePosition].Trim(), file_name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            while ((line = readFile.ReadLine()) != null)
                            {
                                row = line.Split(',');

                                //Do all the file names in the Control file have a .pdf file extension?
                                if (Path.GetExtension(row[fileNamePosition].ToLower()) != ".pdf")
                                {
                                    errMessage = string.Format("All files must be pdf file types, at least one file is type: {0}",
                                            Path.GetExtension(row[fileNamePosition].ToLower()));
                                }
                            }
                        }
                        else
                        {
                            errMessage = string.Format("Header row in Control file is incorrectly named, EmplId is {0} and File_Name is {1}",
                                            header[emplIdPosition].Trim(), header[fileNamePosition].Trim());
                        }
                    }
                }
                else
                {
                    errMessage = string.Format("Number of rows in Zip/Control files are not equal, Zip file = {0} and Control file = {1}",
                                    zip.Count, File.ReadAllLines(controlFileFullPath).Length - 1);
                }

                zip.Dispose();
                zip = null;
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - validateBatch Method Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem validating the batch/control file uploaded.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }

            return errMessage;
        }

        /// <summary>
        /// Initializes the screen the first time
        /// </summary>
        private void InitializeScreen()
        {
            try
            {
                InitializeFields();

                tdNewProject.Visible = false;

                trUploadCriteria.Visible = true;
                ddlUploadCriteria.Enabled = false;

                trBatchFile.Visible = false;
                trBatchUpload.Visible = false;
                uplControlFile.Enabled = false;
                btnUploadControlFile.Enabled = false;

                trSingleDoc.Visible = false;
                trSingleDocUpload.Visible = false;

                txtDueDate.Enabled = false;
                txtEffDate.Enabled = false;

                ddlPrimaryBarcode.Enabled = false;
                ddlSecondaryBarcode.Enabled = false;

                ddlEmailNotify.Enabled = false;
                ddlEmailReminder.Enabled = false;

                trButtons.Visible = true;
                btnSave.Enabled = false;

                //Refresh the dropdown box containing the project names
                ddlProjects.Items.Clear();
                ddlProjects.Items.Add("-- Select Project --");
                ddlProjects.Items.Add("Add a New Project");
                ddlProjects.DataBind(); 
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - Document Upload Initialize Screen Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem initializing the Document Upload Initialize Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Initializes all fields/controls
        /// </summary>
        private void InitializeFields()
        {
            try
            {
                txtNewProjectName.Text = string.Empty;
                ddlUploadCriteria.SelectedIndex = -1;
                txtZipFile.Text = string.Empty;
                txtControlFile.Text = string.Empty;
                txtPDF.Text = string.Empty;
                txtEffDate.Text = string.Empty;
                txtDueDate.Text = string.Empty;
                ddlPrimaryBarcode.SelectedIndex = -1;
                ddlSecondaryBarcode.SelectedIndex = -1;
                ddlEmailNotify.SelectedIndex = -1;
                ddlEmailReminder.SelectedIndex = -1;

                Session["Criteria"] = null;
                Session["EffectiveDate"] = null;
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - Document Upload Initialize Fields Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem initializing the Document Upload Initialize Fields Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }
        
        /// <summary>
        /// btnUploadControlFile - Upload the Control file    
        /// </summary>
        protected void btnUploadControlFile_Click(object sender, EventArgs e)
        {
            Session["ControlFile"] = saveFile("txt", uplControlFile);
            txtControlFile.Text = Path.GetFileName(Session["ControlFile"].ToString());
        }

        /// <summary>
        /// btnUploadPDFFile - Upload the PDF file    
        /// </summary>
        protected void btnUploadPDFFile_Click(object sender, EventArgs e)
        {
            Session["PDFFile"] = saveFile("pdf", uplPDFFile);
            txtPDF.Text = Path.GetFileName(Session["PDFFile"].ToString());
        }

        /// <summary>
        /// btnUploadZipFile - Upload the Zip file   
        /// </summary>
        protected void btnUploadZipFile_Click(object sender, EventArgs e)
        {
            Session["ZipFile"] = saveFile("zip", uplZipFile);
            txtZipFile.Text = Path.GetFileName(Session["ZipFile"].ToString());

            if (txtZipFile.Text.Length > 0)
            {
                uplControlFile.Enabled = true;
                btnUploadControlFile.Enabled = true;
            }
            else
            {
                uplControlFile.Enabled = false;
                btnUploadControlFile.Enabled = false;
            }
        }

        /// <summary>
        /// ddlEmailNotify - SelectedIndexChanged   
        /// </summary>
        protected void ddlEmailNotify_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Document Upload Screen - Email Notify selected: {0}",
                                       ddlEmailNotify.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlEmailReminder - SelectedIndexChanged   
        /// </summary>
        protected void ddlEmailReminder_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Document Upload Screen - Email Reminder selected: {0}",
                                       ddlEmailReminder.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlProjects - SelectedIndexChanged   
        /// </summary>
        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 projectId = 0;

            // Log the Action
            string msg = string.Format("Document Upload Screen - Project selected: {0}",
                                       ddlProjects.SelectedItem);
            _HRSCLogsDA.Insert(msg);

            Session["CriteriaScreen"] = "NotOpened";

            try
            {
                InitializeFields();

                if (ddlProjects.SelectedItem.Text == "Add a New Project")
                {
                    tdNewProject.Visible = true;
                    trUploadCriteria.Visible = true;
                    ddlUploadCriteria.Enabled = true;
                    ddlUploadCriteria.Focus();

                    uplPDFFile.Enabled = true;
                    uplZipFile.Enabled = true;
                    btnUploadPDFFile.Enabled = true;
                    btnUploadZipFile.Enabled = true;

                    Session["ProjectId"] = 0;
                    trBatchFile.Visible = false;
                    trSingleDoc.Visible = false;

                    txtDueDate.Enabled = false;
                    txtEffDate.Enabled = false;

                    ddlPrimaryBarcode.Enabled = false;
                    ddlSecondaryBarcode.Enabled = false;

                    ddlEmailNotify.Enabled = false;
                    ddlEmailReminder.Enabled = false;

                    btnSave.Enabled = true;
                }
                else if (ddlProjects.SelectedItem.Text != "-- Select Project --")
                {
                    // Project exists...store the Project Id
                    projectId = Convert.ToInt32(ddlProjects.SelectedValue);

                    // Existing Project Name was selected, set session variable & go get the record
                    GetProjectInfo(projectId);
                    Session["ProjectId"] = projectId;
                    Session["EffectiveDate"] = txtEffDate.Text;

                    tdNewProject.Visible = false;

                    trUploadCriteria.Visible = true;
                    ddlUploadCriteria.Enabled = false;

                    //// Don't allow editing of these fields once the project has been 'Saved'
                    uplControlFile.Enabled = false;
                    uplPDFFile.Enabled = false;
                    uplZipFile.Enabled = false;
                    btnUploadControlFile.Enabled = false;
                    btnUploadPDFFile.Enabled = false;
                    btnUploadZipFile.Enabled = false;

                    //All fields should be read only if Effective Date is equal to today or later
                    if (DateTime.Today >= Convert.ToDateTime(txtEffDate.Text))
                    {
                        txtDueDate.Enabled = false;
                        txtEffDate.Enabled = false;

                        ddlPrimaryBarcode.Enabled = false;
                        ddlSecondaryBarcode.Enabled = false;

                        ddlEmailNotify.Enabled = false;
                        ddlEmailReminder.Enabled = false;

                        btnSave.Enabled = false;
                    }
                    else
                    {
                        txtDueDate.Enabled = true;
                        txtEffDate.Enabled = true;

                        ddlPrimaryBarcode.Enabled = true;
                        ddlSecondaryBarcode.Enabled = true;

                        ddlEmailNotify.Enabled = true;
                        ddlEmailReminder.Enabled = true;

                        btnSave.Enabled = true;
                    }
                }
                else
                {
                    InitializeScreen();
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
                string errMsg = string.Format("{0} - Projects DropDown List() SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem with the Project Name DropDown List on the Document Upload Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// ddlPrimaryBarcode - SelectedIndexChanged   
        /// </summary>
        protected void ddlPrimaryBarcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Document Upload Screen - Primary Barcode selected: {0}",
                                       ddlPrimaryBarcode.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlSecondaryBarcode - SelectedIndexChanged   
        /// </summary>
        protected void ddlSecondaryBarcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Document Upload Screen - Secondary Barcode selected: {0}",
                                       ddlSecondaryBarcode.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

        /// <summary>
        /// ddlUploadCriteria - SelectedIndexChanged   
        /// </summary>
        protected void ddlUploadCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Document Upload Screen - Upload Criteria selected: {0}",
                                       ddlUploadCriteria.SelectedItem);
            _HRSCLogsDA.Insert(msg);

            try
            {
                if (ddlUploadCriteria.SelectedItem.Text == "Batch File")
                {
                    trBatchFile.Visible = true;
                    trBatchUpload.Visible = true;
                    trSingleDocUpload.Visible = false;
                    trSingleDoc.Visible = false;
                    uplZipFile.Focus();

                    txtDueDate.Enabled = true;
                    txtEffDate.Enabled = true;

                    ddlPrimaryBarcode.Enabled = true;
                    ddlSecondaryBarcode.Enabled = true;

                    ddlEmailNotify.Enabled = true;
                    ddlEmailReminder.Enabled = true;
                }
                else if (ddlUploadCriteria.SelectedItem.Text == "Single Document") //Single Document
                {
                    trSingleDocUpload.Visible = true;
                    trSingleDoc.Visible = true;
                    trBatchFile.Visible = false;
                    trBatchUpload.Visible = false;
                    uplPDFFile.Focus();

                    txtDueDate.Enabled = true;
                    txtEffDate.Enabled = true;

                    ddlPrimaryBarcode.Enabled = true;
                    ddlSecondaryBarcode.Enabled = true;

                    ddlEmailNotify.Enabled = true;
                    ddlEmailReminder.Enabled = true;
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
                string errMsg = string.Format("{0} - UploadCriteria DropDown List() SelectedIndexChanged Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem with the Upload Criteria DropDown List on the Document Upload Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        
        /// <summary>
        /// Get the Selected Project Id Information
        /// </summary>
        private void GetProjectInfo(Int32 projectId)
        {
            // Log the Action
            string msg = string.Format("Document Upload Screen - Get Project Info - Project Id: {0}",
                                       projectId.ToString());
            _HRSCLogsDA.Insert(msg);

            try
            {
                DocumentUploadBO duBO = new DocumentUploadBO();
                DocumentUploadDA duDA = new DocumentUploadDA();
                duBO = duDA.GetByProjectId(projectId);
                
                if (duBO.Upload_Criteria_Name == "Batch File")
                {
                    trBatchFile.Visible = true;
                    trBatchUpload.Visible = true;
                    txtZipFile.Text = duBO.Zip_File_Name;
                    txtControlFile.Text = duBO.Control_File_Name;
                    trSingleDoc.Visible = false;
                    trSingleDocUpload.Visible = false;
                }
                else
                {
                    trSingleDoc.Visible = true;
                    trSingleDocUpload.Visible = true;
                    txtPDF.Text = duBO.PDF_File_Name;
                    trBatchFile.Visible = false;
                    trBatchUpload.Visible = false;
                    Session["Criteria"] = duBO.Criteria_Name;

                    GetCriteriaInfo(projectId);
                }

                ddlUploadCriteria.SelectedValue = duBO.Upload_Criteria_Id.ToString();

                txtEffDate.Text = Convert.ToDateTime(duBO.Effective_Date).ToShortDateString();
                txtDueDate.Text = Convert.ToDateTime(duBO.Ack_Due_Date).ToShortDateString();
                ddlPrimaryBarcode.SelectedValue = duBO.Primary_Barcode.ToString();
                ddlSecondaryBarcode.SelectedValue = duBO.Secondary_Barcode.ToString();
                ddlEmailNotify.SelectedValue = duBO.Email_Notification;
                ddlEmailReminder.SelectedValue = duBO.Email_Reminder;
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - DocUpload existing project GetProjectInfo Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem retrieving the Project record for the Document Upload page.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        private void GetCriteriaInfo(int projectId)
        {
            // Initialize Other Criteria Session variables to "All"
            Session["Exec"] = "All";
            Session["Org"] = "All";
            Session["Group"] = "All";
            Session["Region"] = "All";
            Session["Section"] = "All";
            Session["Area"] = "All";
            Session["FLSA"] = "All";

            try
            {
                // Other Criteria
                OtherCriteriaBO ocBO = new OtherCriteriaBO();
                OtherCriteriaDA ocDA = new OtherCriteriaDA();
                ocBO = ocDA.GetByProjectId(projectId);

                if (ocBO.Exec_Cd != string.Empty)
                {
                    Session["Exec"] = ocBO.Exec_Cd;
                }
                if (ocBO.Org_Cd != string.Empty)
                {
                    Session["Org"] = ocBO.Org_Cd;
                }
                if (ocBO.Group_Cd != string.Empty)
                {
                    Session["Group"] = ocBO.Group_Cd;
                }
                if (ocBO.Region_Cd != string.Empty)
                {
                    Session["Region"] = ocBO.Region_Cd;
                }
                if (ocBO.Section_Cd != string.Empty)
                {
                    Session["Section"] = ocBO.Section_Cd;
                }
                if (ocBO.Area_Cd != string.Empty)
                {
                    Session["Area"] = ocBO.Area_Cd;
                }
                if (ocBO.FLSA_Status != string.Empty)
                {
                    Session["FLSA"] = ocBO.FLSA_Status;
                }

                // JobCodes
                OtherCriteriaJobCodesDA ocjcDA = new OtherCriteriaJobCodesDA();
                DataTable dtJobCodes = ocjcDA.GetByProjectId(projectId);
                Session["dtJobCodesFromDocUpload"] = dtJobCodes;

                // WorkState
                OtherCriteriaWorkStateDA ocwsDA = new OtherCriteriaWorkStateDA();
                DataTable dtWorkState = ocwsDA.GetByProjectId(projectId);
                Session["dtWorkStateFromDocUpload"] = dtWorkState;

                // EmplIds
                DocumentUploadTMStatusDA tmDA = new DocumentUploadTMStatusDA();
                DataTable dtEmplIdsFromDocUpload = tmDA.GetByProjectId(projectId);
                Session["dtEmplIdsFromDocUpload"] = dtEmplIdsFromDocUpload;
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - DocUpload existing project GetCriteriaInfo Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem retrieving the Project record for the Document Upload page.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
　
        /// <summary>
        /// sdsPrimaryBarcode - Selected   
        /// </summary>
        protected void sdsPrimaryBarcode_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = null;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsPrimaryBarcode Selected Error - {1}",
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
        /// sdsProjects - Selected   
        /// </summary>
        protected void sdsProjects_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = null;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsProjects Selected Error - {1}",
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
        /// sdsSecondaryBarcode - Selected   
        /// </summary>
        protected void sdsSecondaryBarcode_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = null;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsSecondaryBarcode Selected Error - {1}",
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
        /// sdsUploadCriteria - Selected   
        /// </summary>
        protected void sdsUploadCriteria_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = null;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsUploadCriteria Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);

                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);
                }

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }

        protected string saveFile(string fileExtension, System.Web.UI.WebControls.FileUpload fileUploads)
        {
            string alertMessage = string.Empty;
            string[] docList = null;
            string emplId = string.Empty;
            string fileName = string.Empty;
            string fileType = string.Empty;
            string msg = string.Empty;
            string projectId = string.Empty;
            string searchCriteria = string.Empty;
            string uploadDirectory = string.Empty;
            string uploadFilePath = string.Empty;

            try
            {
                emplId = Session["EmplId"].ToString();

                if (fileUploads.HasFile)
                {
                    fileName = Path.GetFileName(fileUploads.FileName);
                    fileType = Path.GetExtension(fileName).ToLower();

                    // Check for Supported file types
                    if (fileType == string.Concat(".", fileExtension))
                    {
                        //Build the full file path for the upload directory with file name
                        uploadDirectory = string.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), emplId);
                        uploadFilePath = string.Concat(uploadDirectory, "\\", fileName);

                        if (!Directory.Exists(uploadDirectory))
                        {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        else // Only one file type extension should exist in the directory
                        {
                            searchCriteria = string.Concat("*.", fileExtension);

                            docList = Directory.GetFiles(uploadDirectory, searchCriteria);

                            foreach (string uploadedFile in docList)
                            {
                                File.Delete(uploadedFile);
                            }
                        }

                        fileUploads.PostedFile.SaveAs(uploadFilePath);
                        fileUploads.Dispose();

                        FileInfo fi = new FileInfo(uploadFilePath);

                        //File attribute can't be read only, change to normal
                        if (fi.IsReadOnly)
                        {
                            File.SetAttributes(uploadFilePath, FileAttributes.Normal);
                        }

                        fi = null;
                        fileUploads = null;

                        // Log the Action
                        msg = string.Format("Document Upload Screen - Save File: {0} to {1}",
                                                   fileName, uploadFilePath);
                        _HRSCLogsDA.Insert(msg);
                    }
                    else
                    {
                        //Display message to Team Member
                        msg = string.Concat("Only a ", fileExtension, " file may be Uploaded");
                        alertMessage = "alert('" + msg + "');";
                        ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "Information Message", alertMessage, true);
                    }
                }
                else
                {
                    //Display message to Team Member
                    msg = string.Concat("A File Upload is required to be selected. Please use the Browse button for selecting the file to be uploaded.");
                    alertMessage = "alert('" + msg + "');";
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "Information Message", alertMessage, true);
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
                string errMsg = string.Format("{0} - Upload {1} Click Error - {2}",
                                              GetType().FullName,
                                              fileExtension,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = string.Concat("There was a problem uploading the ", fileExtension, " file.  If the problem persists, please contact Technical Support.");

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
            return uploadFilePath;
        }

        /// <summary>
        /// Remove all Session Variables
        /// </summary>
        private void RemoveSessionVariables()
        {
            Session.Remove("Area");
            Session.Remove("ControlFile");
            Session.Remove("Criteria");
            Session.Remove("CriteriaScreen");
            Session.Remove("dtEmplIds");
            Session.Remove("EffectiveDate");
            Session.Remove("Exec");
            Session.Remove("FLSA");
            Session.Remove("Group");
            Session.Remove("JobCodes");
            Session.Remove("Org");
            Session.Remove("PDFFile");
            Session.Remove("ProjectId");
            Session.Remove("Region");
            Session.Remove("Section");
            Session.Remove("WorkState");
            Session.Remove("ZipFile");

            // Initialize Project list
            ddlProjects.SelectedIndex = -1;
        }
    }
}
