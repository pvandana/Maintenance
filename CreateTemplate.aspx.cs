using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPF.DataAccess;
using EPF.Utilities;
using System.IO;

//------------------------------------------------------------------------------
//                                    CreateTemplate.aspx.cs
//
//      This is the code behind for the Maintenance screen to create a template 
// 
//------------------------------------------------------------------------------
// 
//                          Modification Control Log                           
//                                                                             
//    Date     By                 Description                                
//  --------  ---  -------------------------------------------------------------
//  03-28-17  VP   Initial creation of program                               
//
//------------------------------------------------------------------------------

namespace EPF.Maintenance
{
    public partial class CreateTemplate : System.Web.UI.Page
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

                //RemoveSessionVariables();

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
            // Log the Action
            _HRSCLogsDA.Insert("Create Template Screen - Cancel button Clicked");

            // Check for Temp folder and delete if it exists for this Team Member
            string emplId = Session["EmplId"].ToString();
            string fullFolderName = string.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), emplId);

            if (Directory.Exists(fullFolderName))
            {
                Directory.Delete(fullFolderName, true);

                // Log the Action
                _HRSCLogsDA.Insert(string.Format("Create Template Screen - Deleted Directory: {0}", fullFolderName));
            }

            // Reset all controls and session variables
            InitializeScreen();
            //RemoveSessionVariables();
        }

        /// <summary>
        /// Save button clicked
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string alertMessage = string.Empty;
            string finalUploadDirectory = string.Empty;
            string templateId = "0";
            string templateName = string.Empty;
            //string success = string.Empty;
            string uploadDirectory = string.Empty;

            bool saved = false;

            try
            {
                if (Session["TemplateId"] != null)
                {
                    saved = ProcessBatch();
                }
            }

            //UpdateDocUploadTable();

            uploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), _emplId);

            if (Directory.Exists(uploadDirectory))
            {
                templateId = Session["TemplateId"].ToString();
                finalUploadDirectory = String.Concat(Server.MapPath(Session["UploadDirectory"].ToString()), templateId);

                //Rename the folder as the TemplateId
                Directory.Move(uploadDirectory, finalUploadDirectory);

                //  Log the Action
                _HRSCLogsDA.Insert(string.Format("Create Template Screen - Renamed Directory: {0} to {1}", uploadDirectory, finalUploadDirectory));
            }

        //    try
        //    {
        //        if (saved == true)
        //        {
        //        //Set variable so as to not lose the Session value after calling RemoveSessionVariable in this block of code
        //        templateId = Session["TemplateId"].ToString();
        //        templateName = txtNewTemplateName.Text;

        //        // Display a Messagebox
        //        AlertMessage.Show(string.Format("Successfully saved Template: {0}", templateName), this.Page);

        //        // Reset all controls and session variables
        //        InitializeScreen();
        //        //RemoveSessionVariables();

        //        //Set value back to same value before entering this block of code.  So it can be used to log message at end of procedure
        //        Session["TemplateId"] = templateId;
        //        }

        //        else
        //        {
        //            // Display a Messagebox
        //            AlertMessage.Show(string.Format("Unable to save Template: {0}", txtNewTemplateName.Text), this.Page);
        //        }

        //        //  Log the Action
        //        _HRSCLogsDA.Insert(string.Format("Create Template Screen - Save button Clicked for Template Id: {0}, Template Name: {1}", Session["TemplateId"].ToString(), templateName));
        //    }

        //    catch (InternalException err)
        //    {
        //        // Display a Messagebox
        //        AlertMessage.Show(err.UserFriendlyMsg, this.Page);
        //    }

        //    catch (Exception err)
        //    {
        //        // Database Error
        //        string errMsg = string.Format("{0} - btnSave_Click Method Error - {1}",
        //                                      GetType().FullName,
        //                                      err.Message);
        //        // Log the Error 
        //        AppLogWrapper.LogError(errMsg);

        //        // Display a User Friendly Error Message 
        //        errMsg = "There was a problem Saving the Create Template Data.  If the problem persists, please contact Technical Support.";

        //        // Display a Messagebox
        //        AlertMessage.Show(errMsg, this.Page);
        //    }
        }

        
        /// <summary>
        /// btnUploadDOCFile - Upload the DOC File
        /// </summary>
        protected void btnUploadDOCXFile_Click(object sender, EventArgs e)
        {
            Session["DOCXFile"] = saveFile("Word DOCX", uplDOCXFile);
            txtDOCXFile.Text = Path.GetFileName(Session["DOCXFile"].ToString());
        }

        
        
        /// <summary>
        /// ddlTypes - SelectedIndexChanged
        /// </summary>
        protected void ddlTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Create Template Screen - Template Type selected: {0}",
                                       ddlTypes.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlCategories - SelectedIndexChanged
        /// </summary>
        protected void ddlCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Create Template Screen - Template Category selected: {0}",
                                       ddlCategories.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlSubcategories - SelectedIndexChanged
        /// </summary>
        protected void ddlSubcategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Create Template Screen - Template Subcategory selected: {0}",
                                       ddlSubcategories.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlExecCode - SelectedIndexChanged
        /// </summary>
        protected void ddlExecCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Create Template Screen - Executive Code selected: {0}",
                                       ddlExecCode.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlPrimaryBarcode - SelectedIndexChanged
        /// </summary>
        protected void ddlPrimaryBarcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Create Template Screen - Primary Barcode selected: {0}",
                                       ddlPrimaryBarcode.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// ddlSecondaryBarcode - SelectedIndexChanged
        /// </summary>
        protected void ddlSecondaryBarcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log the Action
            string msg = string.Format("Create Template Screen - Secondary Barcode selected: {0}",
                                       ddlSecondaryBarcode.SelectedItem);
            _HRSCLogsDA.Insert(msg);
        }

　
        /// <summary>
        /// sdsTypes - Selected
        /// </summary>
        protected void sdsTypes_Selected(object sender, SqlDataSourceStatusEventArgs e)
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
                    errMsg = string.Format("{0} - sdsTypes Selected Error - {1}",
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
        /// sdsCategories - Selected
        /// </summary>
        protected void sdsCategories_Selected(object sender, SqlDataSourceStatusEventArgs e)
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
                    errMsg = string.Format("{0} - sdsCategories Selected Error - {1}",
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
        /// sdsSubcategories - Selected
        /// </summary>
        protected void sdsSubcategories_Selected(object sender, SqlDataSourceStatusEventArgs e)
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
                    errMsg = string.Format("{0} - sdsSubcategories Selected Error - {1}",
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
        /// sdsExecCode - Selected
        /// </summary>
        protected void sdsExecCode_Selected(object sender, SqlDataSourceStatusEventArgs e)
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
                    errMsg = string.Format("{0} - sdsExecCode Selected Error - {1}",
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

　
        protected string saveFile(string fileExtension, System.Web.UI.WebControls.FileUpload fileUploads)
        {
            string alertMessage = string.Empty;
            string[] docList = null;
            string emplId = string.Empty;
            string fileName = string.Empty;
            string fileType = string.Empty;
            string msg = string.Empty;
            string templateId = string.Empty;
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
                        msg = string.Format("Create Template Screen - Save File: {0} to {1}",
                                                   fileName, uploadFilePath);
                        _HRSCLogsDA.Insert(msg);
                    }
                    else
                    {
                        //Display message
                        msg = string.Concat("Only a ", fileExtension, " file may be Uploaded");
                        alertMessage = "alert('" + msg + "');";
                        ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "Information Message", alertMessage, true);
                    }
                }
                else
                {
                    //Display message
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
        /// Initializes the screen the first time
        /// </summary>
        private void InitializeScreen()
        {
            try
            {
                InitializeFields();

                //tdNewProject.Visible = false;

                //trUploadCriteria.Visible = true;
                //ddlUploadCriteria.Enabled = false;

                //trBatchFile.Visible = false;
                //trBatchUpload.Visible = false;
                //uplControlFile.Enabled = false;
                //btnUploadControlFile.Enabled = false;

                //trSingleDoc.Visible = false;
                //trSingleDocUpload.Visible = false;

                //txtDueDate.Enabled = false;
                //txtEffDate.Enabled = false;

                //ddlPrimaryBarcode.Enabled = false;
                //ddlSecondaryBarcode.Enabled = false;

                //ddlEmailNotify.Enabled = false;
                //ddlEmailReminder.Enabled = false;

                trButtons.Visible = true;
                btnSave.Enabled = false;

            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - Create Template Initialize Screen Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem initializing the Create Template Initialize Screen.  If the problem persists, please contact Technical Support.";

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
                txtNewTemplateName.Text = string.Empty;
                txtDOCXFile.Text = string.Empty;
                txtEffDate.Text = string.Empty;
                ddlTypes.SelectedIndex = -1;
                ddlCategories.SelectedIndex = -1;
                ddlSubcategories.SelectedIndex = -1;
                ddlExecCode.SelectedIndex = -1;
                ddlPrimaryBarcode.SelectedIndex = -1;
                ddlSecondaryBarcode.SelectedIndex = -1;                

                //Session["Criteria"] = null;
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
                string errMsg = string.Format("{0} - Create Template Initialize Fields Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem initializing the fields in the Create Template Screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

        /// <summary>
        /// Insert Document Upload rows (Primary rows for both types of projects (Batch and Single Document)
        /// </summary>
        //private void UpdateDocUploadTable()
        //{
        //    try
        //    {
        //        DocumentUploadBO duBO = new DocumentUploadBO();
        //        duBO.Template_Id = Convert.ToInt32(Session["TemplateId"]);
        //        if (duBO.Project_Id == 0) //New Batch
        //        {
        //            duBO.Template_Name = txtNewTemplateName.Text;
        //            duBO.Template_Created_By = _emplId;
        //        }
        //        //else
        //        //{
        //        //    duBO.Project_Name = ddlProjects.SelectedItem.ToString();
        //        //}

        //        //Only Single Document Uploads uses Session["Criteria"]
        //        if (Session["Criteria"] != null)
        //        {
        //            duBO.Criteria_Name = Session["Criteria"].ToString();
        //        }
        //        else
        //        {
        //            duBO.Criteria_Name = string.Empty;
        //        }
        //        //duBO.Upload_Criteria_Id = Convert.ToInt32(ddlUploadCriteria.SelectedValue);
        //        duBO.DOCX_File_Name = Path.GetFileName(txtDOCXFile.Text);
        //        duBO.Effective_Date = Convert.ToDateTime(txtEffDate.Text);
        //        duBO.Category = ddlCategories.SelectedValue;
        //        duBO.Subcategory = ddlSubcategories.SelectedValue;
        //        duBO.Type = ddlTypes.SelectedValue;
        //        duBO.Executive_Code = ddlExecCode.SelectedValue;
        //        duBO.Primary_Barcode = ddlPrimaryBarcode.SelectedValue;
        //        duBO.Secondary_Barcode = ddlSecondaryBarcode.SelectedValue;                
        //        duBO.Modified_By = _emplId;

        //        DocumentUploadDA duDA = new DocumentUploadDA();
        //        duDA.UpdateInsert(duBO);
        //    }
        //    catch (InternalException err)
        //    {
        //        // Display a Messagebox
        //        AlertMessage.Show(err.UserFriendlyMsg, this.Page);
        //    }
        //    catch (Exception err)
        //    {
        //        // Database Error
        //        string errMsg = string.Format("{0} - UpdateDocUploadTable Method Error - {1}",
        //                                      GetType().FullName,
        //                                      err.Message);
        //        // Log the Error 
        //        AppLogWrapper.LogError(errMsg);

        //        // Display a User Friendly Error Message 
        //        errMsg = "There was a problem Updating the Document Upload table.  If the problem persists, please contact Technical Support.";

        //        // Display a Messagebox
        //        AlertMessage.Show(errMsg, this.Page);
        //    }
        //}

    }
}
