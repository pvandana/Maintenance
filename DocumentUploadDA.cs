using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web;
using EPF.BusinessObjects;
using EPF.Utilities;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

//------------------------------------------------------------------------------
//                                    DocumentUploadDA.cs
//
//      This class handles all requests for the Document Upload Table
// 
//------------------------------------------------------------------------------
// 
//                          Modification Control Log                           
//                                                                             
//    Date     By                 Description                                
//  --------  ---  -------------------------------------------------------------
//  04-28-15  JV   Initial creation of program                                                          
// 
//------------------------------------------------------------------------------

namespace EPF.DataAccess
{
    public class DocumentUploadDA
    {
        #region Member Variables

        // Variables
        private SqlDatabase _DB = null;

        #endregion

　
        #region Constructors

        public DocumentUploadDA()
        {
            // Initialize Variables
            ErrMsg = string.Empty;

            try
            {
                // Create an instance of the SQL Database class
                _DB = new SqlDatabase(ConfigurationManager.ConnectionStrings["PROCESS360"].ToString());
            }
            catch (Exception err)
            {
                // Database Error
                ErrMsg = string.Format("{0} - Connecting to Database Error - {1}",
                                       GetType().FullName,
                                       err.Message);

                // Save the Error
                CommonDA.SaveError(ErrMsg, err);
            }
        }

        #endregion

　
        #region Enums

        #endregion

　
        #region Properties

        public string ErrMsg { get; set; }

        #endregion

　
        #region Methods

        /// <summary>
        /// Retrieves the Document Upload record for an Project Id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>documentUploadBO</returns>
        public DocumentUploadBO GetByProjectId(Int32 projectId)
        {
            // Define Constants
            const string sp = "dbo.Get_DocUpload_By_Project_Id";

            // Set Stored Procedure
            DbCommand dbCommand = _DB.GetStoredProcCommand(sp);

            // Define Variables
            DocumentUploadBO duBO = new DocumentUploadBO();

            try
            {
                // Set Input Parameters
                _DB.AddInParameter(dbCommand, "Project_Id", DbType.Int32, projectId);

                using (IDataReader dr = _DB.ExecuteReader(dbCommand))
                {
                    // Process all the records
                    while (dr.Read())
                    {
                        // Save the DocUpload record
                        duBO.Save(dr);
                    }
                }
            }
            catch (InternalException err)
            {
                // Re-Throw the Internal Exception
                throw;
            }
            catch (Exception err)
            {
                // Format the Parameters
                string parms = CommonDA.FormatParmsSQL(dbCommand);

                // Database Error
                ErrMsg = string.Format("{0} - Get DocUpload By Project Id Error - Parameters = {1} - {2}",
                                       GetType().FullName,
                                       parms,
                                       err.Message);

                // Save the Error
                CommonDA.SaveError(ErrMsg, err);
            }

            return duBO;
        }

　
        /// <summary>
        /// UpdateInsert the Document Upload record 
        /// </summary>
        /// <param name="DocumentUploadBO"></param>
        /// <returns>int</returns>
        public void UpdateInsert(DocumentUploadBO duBO)
        {
            // Define Constants
            const string sp = "dbo.Update_Insert_Document_Upload";

            // Set Stored Procedure
            DbCommand dbCommand = _DB.GetStoredProcCommand(sp);

            try
            {
                _DB.AddInParameter(dbCommand, "Project_Id", DbType.Int32, duBO.Project_Id);
                _DB.AddInParameter(dbCommand, "Project_Name", DbType.String, duBO.Project_Name);
                _DB.AddInParameter(dbCommand, "Upload_Criteria_Id", DbType.Int32, duBO.Upload_Criteria_Id);
                _DB.AddInParameter(dbCommand, "Effective_Date", DbType.DateTime, duBO.Effective_Date);
                _DB.AddInParameter(dbCommand, "Ack_Due_Date", DbType.DateTime, duBO.Ack_Due_Date);
                _DB.AddInParameter(dbCommand, "Primary_Barcode", DbType.String, duBO.Primary_Barcode);
                _DB.AddInParameter(dbCommand, "Secondary_Barcode", DbType.String, duBO.Secondary_Barcode);
                _DB.AddInParameter(dbCommand, "Email_Notification", DbType.String, duBO.Email_Notification);
                _DB.AddInParameter(dbCommand, "Email_Reminder", DbType.String, duBO.Email_Reminder);
                _DB.AddInParameter(dbCommand, "Zip_File_Name", DbType.String, duBO.Zip_File_Name);
                _DB.AddInParameter(dbCommand, "Control_File_Name", DbType.String, duBO.Control_File_Name);
                _DB.AddInParameter(dbCommand, "PDF_File_Name", DbType.String, duBO.PDF_File_Name);
                _DB.AddInParameter(dbCommand, "Criteria_Name", DbType.String, duBO.Criteria_Name);
                _DB.AddInParameter(dbCommand, "Project_Created_By", DbType.String, duBO.Project_Created_By);
                _DB.AddInParameter(dbCommand, "Modified_By", DbType.String, duBO.Modified_By);

                _DB.AddOutParameter(dbCommand, "Id", DbType.Int32, 0);
                _DB.ExecuteNonQuery(dbCommand);

                if (duBO.Project_Id == 0)
                {
                    HttpContext.Current.Session["ProjectId"] = Convert.ToInt32(_DB.GetParameterValue(dbCommand, "@Id"));
                }
            }
            catch (InternalException err)
            {
                // Re-Throw the Internal Exception
                throw;
            }
            catch (Exception err)
            {
                // Format the Parameters
                string parms = CommonDA.FormatParmsSQL(dbCommand);

                // Database Error
                ErrMsg = string.Format("{0} - UpdateInsert Document Upload Error - Parameters = {1} - {2}",
                                       GetType().FullName,
                                       parms,
                                       err.Message);

                // Save the Error
                CommonDA.SaveError(ErrMsg, err);
            }
        }

        /// <summary>
        /// Delete the Document Upload records for multiple row project data 
        /// </summary>
        /// <param name="DocumentUploadBO"></param>
        /// <returns>int</returns>
        public void Delete(Int32 projectId)
        {
            // Define Constants
            const string sp = "dbo.Delete_Document_Project_Id";

            // Set Stored Procedure
            DbCommand dbCommand = _DB.GetStoredProcCommand(sp);

            try
            {
                _DB.AddInParameter(dbCommand, "Project_Id", DbType.Int32, projectId);
                _DB.ExecuteNonQuery(dbCommand);
            }
            catch (InternalException err)
            {
                // Re-Throw the Internal Exception
                throw;
            }
            catch (Exception err)
            {
                // Format the Parameters
                string parms = CommonDA.FormatParmsSQL(dbCommand);

                // Database Error
                ErrMsg = string.Format("{0} - Delete Document Upload Project Id Error - Parameters = {1} - {2}",
                                       GetType().FullName,
                                       parms,
                                       err.Message);

                // Save the Error
                CommonDA.SaveError(ErrMsg, err);
            }
        }

        #endregion
    }
}
