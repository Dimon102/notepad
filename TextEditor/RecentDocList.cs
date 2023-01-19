using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor
{
    internal class RecentDocList
    {
        //private readonly string filename = "E:\\Учёба\\Технологии программирования\\Лаба 7\\RecentDocList.txt";
        private readonly string filename = "RecentDocList.txt";

        public List<string> list;
        public int Count { get { return list.Count; } }

        public delegate void listUpdated(List<string> list);
        public event listUpdated ListUpdated;

        protected internal string this[int i] { 
            get
            {
                if (i >= 0 && i < list.Count)
                    return list[i];
                else
                    return "Ошибка индекса";
            }
        }

        public RecentDocList()
        {
            list = new List<string>();
            ListUpdated += SaveData;
        }

        public void SaveData(List<string> list)
        {
            File.WriteAllLines(filename, list);
        }
        public void LoadData()
        {
            if (File.Exists(filename))
                list = new List<string>(File.ReadAllLines(filename));
            else
                File.Create(filename).Close();
            if (ListUpdated != null)
                ListUpdated.Invoke(list);
        }
        public void Add(Document doc)
        {
            bool check = false;

            if (doc.path == "0")
                return;

            for (int i = 0; i < Count; i++)
                if (this[i] == doc.path)
                    check = true;

            if (check)
                list.RemoveAll(path => path == doc.path);
            list.Insert(0, doc.path);

            if (ListUpdated != null)
                ListUpdated.Invoke(list);
        }
        public void Delete(string path)
        {
            list.RemoveAll(p => p == path);

            if (ListUpdated != null)
                ListUpdated.Invoke(list);
        }

    }
}
