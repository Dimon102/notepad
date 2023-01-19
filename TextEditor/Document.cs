using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TextEditor
{
    internal class Document : TabPage
    {
        public bool modified;
        public string path;
        public string name;
        public TextBox textbox;

        public delegate Document saveEvent(Document doc);
        public event saveEvent SaveEvent;

        public delegate void sendMes(string mes); // Потом убрать
        public event sendMes SendMes;

        public Document(string path, string name)
        {
            modified = false;
            this.path = path;
            this.name = name;

            this.SuspendLayout();

            // стили
            this.Location = new System.Drawing.Point(4, 22);
            this.Padding = new Padding(3);
            this.TabIndex = 0;
            //this.Text = "*" + name;
            this.Text = name;
            this.UseVisualStyleBackColor = true;
            
            // TextBox
            textbox = new TextBox();

            textbox.AcceptsReturn = true;
            textbox.AcceptsTab = true;
            textbox.AllowDrop = true;
            textbox.Dock = DockStyle.Fill;
            //textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            textbox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            textbox.Location = new System.Drawing.Point(3, 3);
            textbox.MaxLength = 0;
            textbox.Multiline = true;
            textbox.ScrollBars = ScrollBars.Vertical;
            //textbox.Text = this.name; /// убрать текст потом
            textbox.Text = "";
            textbox.DeselectAll();
            //textbox.Select(0, 0);
            textbox.Focus();

            textbox.TextChanged += Modified;

            this.Controls.Add(textbox);

            //textbox.TextChanged += Eve;
            this.ResumeLayout(true);

            textbox.KeyDown += Textbox_KeyDown;
        }

        public void Eve(object sender, EventArgs e)
        {
            this.textbox.Dock = DockStyle.Fill;
            Random rnd = new Random();

            textbox.Size = new System.Drawing.Size((int)(rnd.NextDouble() * 1000), (int)(rnd.NextDouble() * 1000));
        }

        private void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if ( (e.Control) && (e.KeyCode == Keys.S) )
            {
                SaveEvent.Invoke(this);
            }
        }

        public void Modified(object sender, EventArgs e)
        {
            this.modified = true;
            if (this.Text == this.name)
                this.Text = "*" + this.Text;
        }

        public void UnModified()
        {
            this.modified = false;
            this.Text = this.name;
        }

        public void Open()
        {
            this.textbox.Text = File.ReadAllText(this.path);
            textbox.DeselectAll();
            UnModified();
        }

        public void Save()
        {
            File.WriteAllText(this.path, this.textbox.Text);
            UnModified();
        }

        public void SaveAs(string path)
        {
            File.WriteAllText(path, this.textbox.Text);
            UnModified();
        }
    }
}
