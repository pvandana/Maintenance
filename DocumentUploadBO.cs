using System;
using System.Data;

//------------------------------------------------------------------------------
//                                    DocumentUploadBO.cs
//
//      This is the Document Upload object.
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

namespace EPF.BusinessObjects
{
    public class DocumentUploadBO
    {
        #region Member Variables

        #endregion

　
        #region Constructors

        public DocumentUploadBO()
        {
            // Initialize Properties
            //Project_Id = null;
            //Project_Name = string.Empty;
            //Upload_Criteria_Id = null;
            //Upload_Criteria_Name = string.Empty;
            //Effective_Date = null;
            //Ack_Due_Date = null;
            //Upload_Date = null;
            //Primary_Barcode = string.Empty;
            //Secondary_Barcode = string.Empty;
            //Email_Notification = string.Empty;
            //Email_Reminder = string.Empty;
            //Project_Created_By = string.Empty;
            //Project_Delete_Switch = string.Empty;
            //Zip_File_Name = string.Empty;
            //Control_File_Name = string.Empty;
            //PDF_File_Name = string.Empty;
            //Criteria_Name = string.Empty;
            //Modified_Date = null;
            //Modified_By = string.Empty;

            // Initalize all variables to null
            Project_Id = null;
            Project_Name = null;
            Upload_Criteria_Id = null;
            Upload_Criteria_Name = null;
            Effective_Date = null;
            Ack_Due_Date = null;
            Upload_Date = null;
            Primary_Barcode = null;
            Secondary_Barcode = null;
            Email_Notification = null;
            Email_Reminder = null;
            Project_Created_By = null;
            Project_Delete_Switch = null;
            Zip_File_Name = null;
            Control_File_Name = null;
            PDF_File_Name = null;
            Criteria_Name = null;
            Modified_Date = null;
            Modified_By = null;
        }

        #endregion

　
        #region Enums

        #endregion

　
        #region Properties

        public Int32? Project_Id { get; set; }
        public string Project_Name { get; set; }
        public Int32? Upload_Criteria_Id { get; set; }
        public string Upload_Criteria_Name { get; set; }
        public DateTime? Effective_Date { get; set; }
        public DateTime? Ack_Due_Date { get; set; }
        public DateTime? Upload_Date { get; set; }
        public string Primary_Barcode { get; set; }
        public string Secondary_Barcode { get; set; }
        public string Email_Notification { get; set; }
        public string Email_Reminder { get; set; }
        public string Project_Created_By { get; set; }
        public string Project_Delete_Switch { get; set; }
        public string Zip_File_Name { get; set; }
        public string Control_File_Name { get; set; }
        public string PDF_File_Name { get; set; }
        public string Criteria_Name { get; set; }
        public DateTime? Modified_Date { get; set; }
        public string Modified_By { get; set; }

        #endregion

　
        #region Methods

        /// <summary>
        /// Saves the Document Upload BO
        /// </summary>
        /// <param name="dr"></param>
        internal void Save(IDataReader dr)
        {
            if (dr["Project_Id"] != DBNull.Value)
            {
                Project_Id = Convert.ToInt32(dr["Project_Id"]);
            }
            Project_Name = dr["Project_Name"].ToString();

            if (dr["Upload_Criteria_Id"] != DBNull.Value)
            {
                Upload_Criteria_Id = Convert.ToInt32(dr["Upload_Criteria_Id"]);
            }
            Upload_Criteria_Name = dr["Upload_Criteria_Name"].ToString();

            if (dr["Effective_Date"] != DBNull.Value)
            {
                Effective_Date = (DateTime)dr["Effective_Date"];
            }

            if (dr["Ack_Due_Date"] != DBNull.Value)
            {
                Ack_Due_Date = (DateTime)dr["Ack_Due_Date"];
            }

            if (dr["Upload_Date"] != DBNull.Value)
            {
                Upload_Date = (DateTime)dr["Upload_Date"];
            }

            Primary_Barcode = dr["Primary_Barcode"].ToString();
            Secondary_Barcode = dr["Secondary_Barcode"].ToString();
            Email_Notification = dr["Email_Notification"].ToString();
            Email_Reminder = dr["Email_Reminder"].ToString();
            Project_Created_By = dr["Project_Created_By"].ToString();
            Project_Delete_Switch = dr["Project_Delete_Switch"].ToString();
            Zip_File_Name = dr["Zip_File_Name"].ToString();
            Control_File_Name = dr["Control_File_Name"].ToString();
            PDF_File_Name = dr["PDF_File_Name"].ToString();

            Criteria_Name = dr["Single_Document_Selection_Criteria_Name"].ToString();

            if (dr["Modified_Date"] != DBNull.Value)
            {
                Modified_Date = (DateTime)dr["Modified_Date"];
            }

            Modified_By = dr["Modified_By"].ToString();
        }

        #endregion
    }
}
