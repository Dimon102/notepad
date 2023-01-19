using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        private Editor editor;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            editor = new Editor();
            editor.ListUpdated += RecentDocListUpdate;

            editor.RecentLoad();
            this.panel1.Controls.Add(editor);


            editor.SendMes += MessageBox.Show; // Потом убрать
            //MessageBox mbox = new MessageBox();


        }

        private void RecentDocListUpdate(List<string> list)
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem() {
                    Size = new Size(80, 22),
                    Text = list[i]
                };
                toolStripMenuItem.Click += editor.OpenRecent;
                this.recentToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.New();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Open("");
        }
        
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) // Save
        {
            editor.Save(editor.SelectedTab);
            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.SaveAs(editor.SelectedTab);
            
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editor.SelectedTab != null)
                editor.CloseDoc((Document) editor.SelectedTab);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CloseApp())
                e.Cancel = true;
        }

        private bool CloseApp()
        {
            if (editor.Quit())
                this.Dispose();
            else
                return false;
            return true;
        }

    }
}
