using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Reminder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string databasepath = "todos.db";
        bool CheckDatabaseFile()
        {
            if (!File.Exists(databasepath))
            {
                var myfile = File.Create(databasepath);
                myfile.Close();
                return false;
            }
            return true;
        }
        void StartClock()
        {
            clock.Start();
            clock_Tick(null, null);
        }
        void SetGridView()
        {
            dgw.ColumnCount = 4;
            dgw.Columns[0].Name = "ID";

            dgw.Columns[1].Name = "İş";
            dgw.Columns[2].Name = "Son Tarih";
            dgw.Columns[3].Name = "Ekleme Tarihi";
            dgw.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgw.Columns[0].Width = 30;
        }
        void LoadDatabase()
        {
            var text = File.ReadAllText(databasepath);
            var todos = text.Split(';');
            foreach (var item in todos)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;

                var obj = item.Trim();
                obj = obj.Trim('[', ']');
                var id = obj.Split(',')[0];
                var todo = obj.Split(',')[1].Trim('\"');
                var deadline = obj.Split(',')[2].Trim('\"');
                var adddate = obj.Split(',')[3].Trim('\"');
                AddToTable(int.Parse(id), todo, DateTime.Parse(deadline), DateTime.Parse(adddate));
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckDatabaseFile();
            StartClock();
            SetGridView();
            LoadDatabase();
            textBox3.Focus();

        }
        void AddToTable(int id, string todo, DateTime deadline, DateTime adddate)
        {
            dgw.Rows.Add(id, todo, deadline, adddate);
        }
        void AddToDatabase(int id, string todo, DateTime deadline, DateTime adddate)
        {
            string formatted_todo = $"[{id},\"{todo}\",\"{deadline.ToString()}\",\"{adddate.ToString()}\"];";
            File.AppendAllText(databasepath, formatted_todo);
        }
        void AddNew(int id, string todo, DateTime deadline, DateTime adddate)
        {
            AddToTable(id, todo, deadline, adddate);
            AddToDatabase(id, todo, deadline, adddate);
        }

        int GetID()
        {
            int id = 0;

            while (true)
            {
                bool flag = false;
                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.Cells[0].Value == null) { break; }
                    if ((int)row.Cells[0].Value == id) flag = true;
                }
                if (!flag) return id;
                id++;
            }

        }


        private void clock_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            textBox1.Text = now.ToShortDateString();
            textBox2.Text = now.ToLongTimeString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddNew(GetID(), textBox3.Text, deadlinepicker.Value, DateTime.Now);
            textBox3.Text = string.Empty;
        }
        void RemoveFromDatabase(int id)
        {
            File.Delete(databasepath);
            CheckDatabaseFile();

            foreach (DataGridViewRow row in dgw.Rows)
            {
                if (row.Cells[0].Value == null) break;
                if (id != int.Parse(Convert.ToString(row.Cells[0].Value)))
                {
                    int rowid = int.Parse(Convert.ToString(row.Cells[0].Value));
                    string todo = Convert.ToString(row.Cells[1].Value);
                    DateTime deadline = DateTime.Parse(Convert.ToString(row.Cells[2].Value));
                    DateTime adddate = DateTime.Parse(Convert.ToString(row.Cells[3].Value));
                    AddToDatabase(rowid, todo, deadline, adddate);
                }

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dgw.SelectedRows[0].Cells[0].Value == null) return;
            int selectedid = (int)dgw.SelectedRows[0].Cells[0].Value;
            dgw.Rows.Remove(dgw.SelectedRows[0]);
            RemoveFromDatabase(selectedid);
        }
    }
}
