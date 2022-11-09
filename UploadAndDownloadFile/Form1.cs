using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UploadAndDownloadFile
{
    public partial class Form1 : Form
    {
        int seqCode = -1;
        public Form1()
        {
            InitializeComponent();

            LoadData();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All File (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Pilih file terlebih dahulu");
            }
            else
            {
                try
                {
                    FileStream fs = new FileStream(textBox1.Text, FileMode.Open);
                    string[] strPath = textBox1.Text.Split(Convert.ToChar(@"\"));
                    string[] strPath2 = textBox1.Text.Split(Convert.ToChar(@"."));
                    long fileLength = Convert.ToInt64(fs.Length) / 1024; //byte ke KB

                    byte[] filedata = new byte[fs.Length];
                    fs.Read(filedata, 0, (int)fs.Length);
                    fs.Close();

                    Data.Connect();
                    Data.Command("INSERT INTO FileUpload(FileName,FileType,FileSize,FileData) VALUES(@0,@1,@2,@3)",
                        new object[] { strPath[strPath.Length - 1], strPath2[strPath2.Length - 1], fileLength, filedata });

                    MessageBox.Show("Upload data berhasil!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "GAGAL");

                }
                finally
                {
                    Data.Disconnect();
                }
            }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Data.Connect();
                DataTable dt = Data.SelectDataTable("SELECt SeqCode,FileName,FileType,FileSize FROM FileUpload", null);

                listView1.Items.Clear();
                listView1.Columns.Clear();
                listView1.Columns.Add("SeqCode", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("FileName", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("FileType", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("FileSize", 60, HorizontalAlignment.Left);

                listView1.GridLines = true;
                listView1.FullRowSelect = true;
                listView1.MultiSelect = false;
                listView1.View = View.Details;

                if(dt!=null)
                {
                    if(dt.Rows.Count>0)
                    {
                        foreach(DataRow row in dt.Rows)
                        {
                            ListViewItem item = new ListViewItem(row[0].ToString());
                            for(int i=0; i<dt.Columns.Count; i++)
                            {
                                item.SubItems.Add(row[i].ToString());
                            }
                            listView1.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "GAGAL");

            }
            finally
            {
                Data.Disconnect();
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if(seqCode<0)
            {
                MessageBox.Show("Pilih file pad list terlebih dahulu!");
            }
            else
            {
                try
                {
                    Data.Connect();
                    DataTable dt = Data.SelectDataTable("SELECT FileData,FileType From FileUpload WHERE SeqCode=@0", new object[]{ seqCode });
                    if(dt!=null)
                    {
                        if(dt.Rows.Count>0)
                        {
                            byte[] objData = (byte[])dt.Rows[0][0];

                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Filter = "Format|*." + dt.Rows[0][1].ToString();
                            if(sfd.ShowDialog() == DialogResult.OK)
                            {
                                string strFileToSave = sfd.FileName;
                                FileStream objFileStream = new FileStream(strFileToSave, FileMode.Create, FileAccess.Write);
                                objFileStream.Write(objData, 0, objData.Length);
                                objFileStream.Close();

                                if(MessageBox.Show("Download berhasil, Apakah kamu ingin membuka file?","",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                                {
                                    Process.Start(sfd.FileName);
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "GAGAL");

                }
                finally
                {
                    Data.Disconnect();
                }
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(e.IsSelected)
            {
                try
                {
                    seqCode = Convert.ToInt32(listView1.SelectedItems[0].Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
