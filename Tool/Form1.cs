
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using Xceed.Words.NET;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using System;

namespace PDFTOWORD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowser_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.Filter = "Archivos PDF|*.pdf|Archivos WORD|*.docx";    
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
        }

        private void btnConvert_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Por favor seleccione su documento a convertir", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (radioButtonPDFTOWORD.Checked == false && radioButtonWORDTOPDF.Checked == false)
            {
                MessageBox.Show("Por favor seleccione el tipo de conversión!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (radioButtonPDFTOWORD.Checked == true)
            {
                PDDocument doc = PDDocument.load(txtFileName.Text);
                PDFTextStripper stripper = new PDFTextStripper();
                richTextBox1.Text = (stripper.getText(doc));
                string newtext = richTextBox1.Text.Replace("Powered by TCPDF (www.tcpdf.org)", "").Trim();
                var docName = Path.GetFileNameWithoutExtension(txtFileName.Text) + ".docx";
                var worddoc = DocX.Create(docName);                
                worddoc.InsertParagraph(newtext);
                worddoc.Save();
                Process.Start(docName);
                return;
            }
            if (radioButtonWORDTOPDF.Checked == true)
            {
                // Create a new Microsoft Word application object
                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();

                // C# doesn't have optional arguments so we'll need a dummy value
                object oMissing = System.Reflection.Missing.Value;

                // Get list of Word files in specified directory
                DirectoryInfo dirInfo = new DirectoryInfo(txtFileName.Text);

                FileInfo[] wordFiles = dirInfo.GetFiles("*.docx");

                word.Visible = false;
                word.ScreenUpdating = false;

                foreach (FileInfo wordFile in wordFiles)
                {
                    // Cast as Object for word Open method
                    Object filename = (Object)wordFile.FullName;

                    // Use the dummy value as a placeholder for optional arguments
                    Document doc = word.Documents.Open(ref filename, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                    doc.Activate();

                    richTextBox1.Text = doc.ToString();

                    object outputFileName = wordFile.FullName.Replace(".docx", ".pdf");
                    object fileFormat = WdSaveFormat.wdFormatPDF;

                    // Save document into PDF Format
                    doc.SaveAs(ref outputFileName,
                        ref fileFormat, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    // Close the Word document, but leave the Word application open.
                    // doc has to be cast to type _Document so that it will find the
                    // correct Close method.                
                    object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                    ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
                    doc = null;
                }
                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
                word = null;
                return;
            }
        }
    }
}
