using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using System.Xml.Serialization;

namespace com.simplesoft.bpworkspaces
{   
    public partial class frmMain : Form
    {
   
        private EnvDTE80.DTE2 dte2;
        private BreakPointWorkspace _bpw = new BreakPointWorkspace();        
        //EnvDTE90.Debugger3 dte3;
        //ArrayList al = new ArrayList();     

        public frmMain()
        {
            InitializeComponent();
        }

        #region Helpers
        private void SaveToFile2()
        {
            FileStream file = null;
            // Create a new stream to write to the file
            StreamWriter sw = null;

            try
            {
                if (!File.Exists(txtFilePath.Text))
                {
                    folderBrowserDialog1.ShowDialog();
                    string str = folderBrowserDialog1.SelectedPath + "\\bpworkspace.txt";
                    //file = new FileStream(str, FileMode.Create, FileAccess.ReadWrite);
                    file = new FileStream(str, FileMode.OpenOrCreate, FileAccess.Write);
                    txtFilePath.Text = str;
                }
                else
                {
                    file = new FileStream(txtFilePath.Text, FileMode.Truncate, FileAccess.Write);
                }

                //File.Delete(textBox1.Text);
                sw = new StreamWriter(file);
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(_bpw.GetType());
                x.Serialize(sw, _bpw);                
                   

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                sw.Close();

                // Close file
                file.Close();
            }

        }

        private void EnableForm()
        {
            txtFilePath.Enabled = true;
            lstGroups.Enabled = true;
            btnDelete.Enabled = true;
            btnLoad.Enabled = true;
        }
    
        private void AddBreakPoint(EnvDTE80.DTE2 dte, string filename, int line)
        {
            try
            {
                //EnvDTE80.DTE2 dte2;
                // dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.
                //GetActiveObject("VisualStudio.DTE.8.0");
                dte2.Debugger.Breakpoints.Add("", filename, line, 1, "", EnvDTE.dbgBreakpointConditionType.dbgBreakpointConditionTypeWhenTrue,
                         "", "", 0, "", 0, EnvDTE.dbgHitCountType.dbgHitCountTypeNone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
            }
           // dte2 = null;
        }        

        private void LoadBreakPointGroup()
        {
            try
            {
                string groupname = lstGroups.SelectedItem.ToString();

                foreach (BreakPointGroup bpg in _bpw.BreakPointGroups)
                {
                    if (bpg.GroupName.Equals(groupname))
                    { 
                        //add breakpoints
                        foreach(BPoint bPoint in bpg.Breakpoints)
                        {
                            AddBreakPoint(dte2, bPoint.FileName, bPoint.LineNumber);
                        }
                    }
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
            }
        
        }        
        #endregion

        #region Events
        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {

                string gpname = txtNewGroupName.Text;

                if (!txtNewGroupName.Text.Equals(""))
                {
                    BreakPointGroup bpg = new BreakPointGroup();
                    bpg.GroupName = gpname;

                    List<BPoint> bps = new List<BPoint>();
                    foreach (EnvDTE80.Breakpoint2 bp in dte2.Debugger.Breakpoints)
                    {
                        bps.Add(new BPoint { FileName=bp.File, LineNumber=bp.FileLine });
                    }
                    bpg.Breakpoints = bps;
                    if (_bpw.BreakPointGroups == null)
                    {
                        _bpw.BreakPointGroups = new List<BreakPointGroup>();
                    }
                    _bpw.BreakPointGroups.Add(bpg);

                    SaveToFile2();

                    lstGroups.Items.Add(gpname);

                    EnableForm();
                   
                }
                else
                {
                    MessageBox.Show("You must enter a group name to save a new group.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstGroups.SelectedItem != null)
                {

                    if (!lstGroups.SelectedItem.Equals(""))
                    {
                        //delete breakpoints
                        foreach (EnvDTE80.Breakpoint2 bp in dte2.Debugger.Breakpoints)
                        {
                            bp.Delete();
                        }
                        //load breakpoints from group

                        LoadBreakPointGroup();
                    }
                    else
                    {
                        MessageBox.Show("Nothing to load.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
            }
        }

        private void btnGetFile_Click(object sender, EventArgs e)
        {
            //show dialog
            this.openFileDialog1.ShowDialog();
            EnableForm();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                //display path in textbox
                txtFilePath.Text = openFileDialog1.FileName;
                //al = new ArrayList();
                if (File.Exists(txtFilePath.Text))
                {
                    this.lstGroups.Items.Clear();
                    // *** Read from file ***

                    // Specify file, instructions, and privelegdes
                    FileStream file = new FileStream(txtFilePath.Text, FileMode.OpenOrCreate, FileAccess.Read);

                    // Create a new stream to read from a file
                    StreamReader sr = new StreamReader(file);

                    //deserilize file and load names
                    XmlSerializer serializer = new XmlSerializer(typeof(BreakPointWorkspace));


                    // Call the Deserialize method to restore the object's state.
                    _bpw = (BreakPointWorkspace)serializer.Deserialize(sr);

                    foreach(BreakPointGroup  bpg in _bpw.BreakPointGroups)
                    {
                        lstGroups.Items.Add(bpg.GroupName);
                    }
                   
                    // Close StreamReader
                    sr.Close();

                    // Close file
                    file.Close();

                    //cleanup
                    sr = null;
                    file = null;

                    

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
            }
        }
               
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstGroups.SelectedItem != null)
                {
                    string groupname = lstGroups.SelectedItem.ToString();

                    lstGroups.Items.Remove(groupname);

                    BreakPointGroup bpToRemove = null;

                    foreach (BreakPointGroup bpg in _bpw.BreakPointGroups)
                    {
                        if (bpg.GroupName.Equals(groupname))
                        {
                            bpToRemove = bpg;
                            break;
                           
                        }

                    }
                    if (bpToRemove != null) _bpw.BreakPointGroups.Remove(bpToRemove);
                 
                    SaveToFile2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.
                GetActiveObject("VisualStudio.DTE.9.0");

                //dte3 =(EnvDTE90.Debugger3)dte2.Debugger;

                linkLabel1.Links.Add(0, 32, "mailto:rick_schott2002@yahoo.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
            }

        }
        #endregion 
        
    }    

}
