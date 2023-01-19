using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TextEditor
{
    internal class Editor : TabControl
    {
        private List<Document> documents;

        private RecentDocList recentDocList;

        private readonly OpenFileDialog openFileDialog;
        private readonly SaveFileDialog saveFileDialog;

        private int TabCounter;
        private int TabNameCounter;

        public delegate DialogResult sendMes(string mes, string caption, MessageBoxButtons buttons); // Потом убрать
        public event sendMes SendMes;

        public delegate void listUpdated(List<string> list);
        public event listUpdated ListUpdated;

        public Editor()
        {
            documents = new List<Document>();

            TabCounter = 0;
            TabNameCounter = 0;

            /// рецент
            this.recentDocList = new RecentDocList();
            //this.recentDocList.ListUpdated += ListUpdated.Invoke;
            this.recentDocList.ListUpdated += ListUpdatedInvoke;
            //this.recentDocList.LoadData();

            /// стиль
            //this.Controls.Add(New((TabCounter + 1).ToString() + ".txt"));
            New();
            this.Dock = DockStyle.Fill;
            this.HotTrack = true;
            this.Location = new System.Drawing.Point(0, 24);
            this.Name = "tabControl1";
            this.SelectedIndex = 0;
            this.Size = new System.Drawing.Size(984, 587);
            this.TabIndex = 0;

            this.SelectTab(TabCounter - 1);

            /// диалоги
            this.openFileDialog = new OpenFileDialog();
            this.saveFileDialog = new SaveFileDialog();

            this.saveFileDialog.AddExtension = true;
            this.saveFileDialog.DefaultExt = ".txt";
            this.Selected += TabSelected;
        }

        public void TabSelected(object sender, TabControlEventArgs e)
        {
            if (this.TabCounter != 0)
                ((Document) this.SelectedTab).textbox.Focus();
        }

        public void ListUpdatedInvoke(List<string> list) ///
        {
            ListUpdated.Invoke(list);
        }

        public void RecentLoad()
        {
            this.recentDocList.LoadData();
        }

        public bool Quit()
        {
            Document doc;
            int count = this.documents.Count;
            for (int i = 0; i < count; i++)
            {
                //doc = DocFinder(this.documents.Count - 1);
                //this.SelectTab(doc);
                doc = documents[0];
                this.SelectTab(doc);
                if (CloseDoc(doc))
                    return false;
                /*
                this.SelectTab(this.documents.Count - 1 - i);
                doc = (Document)this.SelectedTab;
                CloseDoc(doc);
                */
            }
            return true;
        }

        public bool CloseDoc(Document doc) /// Нужно ли удаление объекта
        {
            //this.SelectTab(doc);
            if (doc.modified == true)
            {
                DialogResult result = SendMes.Invoke("Сохранить файл?", "Сохранение файла", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                    Save(doc);
                else if (result == DialogResult.Cancel)
                    return true;
            }
            TabCounter--;
            if (this.SelectedIndex > 0)
                this.SelectedIndex--;
            ///
            this.Controls.Remove(doc);
            doc.Dispose();
            documents.Remove(doc); // не удалять
            return false;
        }

        public Document New()
        {
            string name = (TabNameCounter + 1).ToString() + ".txt";
            TabCounter++;
            TabNameCounter++;

            Document document = new Document("0", name);
            document.SaveEvent += Save;

            this.Controls.Add(document);
            this.SelectTab(TabCounter - 1);

            recentDocList.Add(document);

            documents.Add(document);


            return document;
        }

        public Document Open(string recpath)
        {
            Document doc = null;
            bool check = true;
            string path;
            string name;
            if (recpath != String.Empty)
                path = recpath;
            else if (openFileDialog.ShowDialog() == DialogResult.OK)
                path = this.openFileDialog.FileName;
            else
                return null;

            foreach (Document d in documents)
                if (d.path == path)
                {
                    check = false;
                    doc = d;
                }

            if (check)
            {
                TabCounter++;
                TabNameCounter++;
                name = Path.GetFileName(path);

                doc = new Document(path, name);
                doc.Open();
                doc.SaveEvent += Save;
                this.Controls.Add(doc);
                this.SelectTab(TabCounter - 1);
                documents.Add(doc);

                recentDocList.Add(doc);

                return doc;
            }
            else
            {
                this.SelectTab(doc);
                return null;
            }
        }

        public Document Save(TabPage tab)
        {
            Document doc = (Document) tab;
            if (doc != null) {
                if (doc.path != "0")
                    doc.Save();
                else
                    SaveAs(tab);
            }
            return doc;
        }

        public Document SaveAs(TabPage tab)
        {
            Document doc = (Document) tab;
            //SendMes.Invoke(doc.ToString());
            this.saveFileDialog.FileName = ((Document)this.SelectedTab).name;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = this.saveFileDialog.FileName;
                string fname = Path.GetFileName(path);
                //SendMes.Invoke(path + "\nsadfa\n" + fname);
                doc.path = path;
                doc.name = fname;
                recentDocList.Add(doc);

                doc.SaveAs(path);
            }
            return doc;
        }

        public void OpenRecent(object sender, EventArgs e)
        {
            string path = ((ToolStripMenuItem)sender).Text;
            if (File.Exists(path))
                Open(path);
            else
                recentDocList.Delete(path);

        }
    }
}
