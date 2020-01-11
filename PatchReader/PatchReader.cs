using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatchReader
{
    public partial class PatchReader : Form
    {
        StringBuilder files;
        List<string> filesChanged;
        public PatchReader()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            opnDlgFileReader = new OpenFileDialog()
            {
                InitialDirectory = @"C:\",
                Title = "Browse Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "patch",
                Filter = "patch files (*.patch)|*.patch",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true,
                Multiselect = true
            };

            if (opnDlgFileReader.ShowDialog() == DialogResult.OK)
            {
                lstFiles.Items.Clear();
                txtOutput.Clear();

                files = new StringBuilder();
                filesChanged = new List<string>();
                
                foreach (String file in opnDlgFileReader.FileNames)
                {

                    lstFiles.Items.Add(file);
                    string line = file;
                    var type = line.GetType();

                    string result = ReadFile(line);
                }

                if (!string.IsNullOrEmpty(files.ToString()))
                {
                    txtOutput.Text = files.ToString().Trim();
                }
                else
                {
                    MessageBox.Show("File is Empty");
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
            txtOutput.ResetText();
        }

        private void txtCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                Clipboard.SetText(txtOutput.Text);
                MessageBox.Show("Copied");
            }
            else
            {
                MessageBox.Show("Text Box is Empty");
            }
        }

        #region Custom Methods
        public string ReadFile(string filePath)
        {
            try
            {
                using (StreamReader file = new StreamReader(filePath))
                {
                    string ln;
                    while ((ln = file.ReadLine()) != null)
                    {
                        if (ln.Contains("(working copy)"))
                        {
                            ln = ln.Replace("(working copy)", "").Replace("+++ ", "").Trim();

                            if (!files.ToString().Contains(ln) && !filesChanged.Any(ln.Contains))
                            {
                                files.AppendLine(ln);
                                filesChanged.Add(ln);
                            }
                        }
                    }
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return files.ToString();
        }
        #endregion Custom Methods
    }
}
