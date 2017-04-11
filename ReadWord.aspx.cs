using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace ReadWordDocument
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
 
        private void Form1_Load(object sender, EventArgs e)
        {
 
        }
 
        public void ReadMsWord()
 
        {
            //clear the text
            rtfText.Text = "";
 
            // variable to store file path
            string filePath = null;
 
            // open dialog box to select file
            OpenFileDialog file = new OpenFileDialog();
 
            // dilog box title name
            file.Title = "Word File";
 
            // set initial directory of computer system
            file.InitialDirectory = "c:\\";
 
            // set restore directory
            file.RestoreDirectory = true;
 
            // execute if block when dialog result box click ok button
            if (file.ShowDialog() == DialogResult.OK)
            {
                // store selected file path
                filePath = file.FileName.ToString();
            }
 
            try
            {
                // create word application
                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
 
                // create object of missing value
                object miss = System.Reflection.Missing.Value;
 
                // create object of selected file path
                object path = filePath;
 
                // set file path mode
                object readOnly = false;
 
                // open document
                Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path
                                                                                 , ref miss
                                                                                 , ref readOnly
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss
                                                                                 , ref miss);
 
                // select whole data from active window document
                docs.ActiveWindow.Selection.WholeStory();
 
                // copy the data to cllipboard
                docs.ActiveWindow.Selection.Copy();
 
                // clipboard create reference of idataobject interface which transfer the data
                IDataObject data = Clipboard.GetDataObject();
 
                //find tags in the text              
                string rtfString = data.GetData(DataFormats.Text).ToString();
                string[] separatingChars = { "<<", ">>"};
                string[] rtfArrayString = rtfString.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
                int i = 0;
                //pick alternate values that will denote the tags
                foreach (var itm in rtfArrayString)
                {
                    if ((i % 2) != 0) {
                        rtfText.Text += itm.Trim() + '\n';
                    }
                    i++;
                }
 
                // close the document
                docs.Close(ref miss
                          , ref miss
                          , ref miss);
                word.Quit(false);
            }
 
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
 
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
 
        private void btnRead_Click(object sender, EventArgs e)
        {
            ReadMsWord();
        }
    }
}