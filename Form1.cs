using ToolITs.db;
using System.Data;
using Microsoft.VisualBasic.ApplicationServices;
using System.Net;

namespace ToolITs
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void secondToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void otherToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = textBox2.Text;
            DbConnect dbConnect;
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("To can be null!");
                return;
            }
            string inset_per = $" \r\nREM INSERTING into {comboBox1.Text} \r\nSET DEFINE OFF;\r\n";
            if (!string.IsNullOrWhiteSpace(comboBox2.Text) &&
                !string.IsNullOrWhiteSpace(comboBox6.Text) &&
                !string.IsNullOrWhiteSpace(comboBox7.Text) &&
                !string.IsNullOrWhiteSpace(comboBox8.Text)
                )
            {
                string connstr = $"Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = {comboBox2.Text.Trim()})(PORT = {comboBox6.Text.Trim()}))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = xe))); user Id = {comboBox7.Text.Trim()}; Password = {comboBox8.Text.Trim()}";
                dbConnect = new DbConnect(connstr);
            }
            else
            {
                dbConnect = new DbConnect();
            }

            List<string> sql_collection = new List<string>();
            try
            {
                DataSet ds = dbConnect.ExcuteQuery(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    var columns = from c in dt.Columns.Cast<DataColumn>() select c.ColumnName;
                    List<string> k = columns.ToList();

                    foreach (DataRow item in dt.Rows)
                    {
                        List<string> v = item.ItemArray.Select(x => $"'{x}'").ToArray().ToList();
                        string insert_sql = $"INSERT INTO {comboBox1.Text} ({string.Join(",", k)}) VALUES({string.Join(",", v)}) ;";
                        sql_collection.Add(insert_sql);
                    }

                    if (sql_collection.Count > 0)
                    {
                        string clip_text = inset_per + string.Join("\n", sql_collection);
                        textBox5.Text = clip_text;
                        Clipboard.SetText(clip_text);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }



        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            string sql = textBox2.Text;
            DbConnect source;
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("To can be null!");
                return;
            }
            if (!string.IsNullOrWhiteSpace(comboBox2.Text) &&
                !string.IsNullOrWhiteSpace(comboBox6.Text) &&
                !string.IsNullOrWhiteSpace(comboBox13.Text) &&
                !string.IsNullOrWhiteSpace(comboBox7.Text) &&
                !string.IsNullOrWhiteSpace(comboBox8.Text)
                )
            {
                string connstr = $"Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = {comboBox2.Text.Trim()})(PORT = {comboBox6.Text.Trim()}))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = {comboBox13.Text.Trim()}))); user Id = {comboBox7.Text.Trim()}; Password = {comboBox8.Text.Trim()}";
                source = new DbConnect(connstr);
            }
            else
            {
                source = new DbConnect();
            }

            List<string> sql_collection = new List<string>();
            try
            {
                DataSet ds = source.ExcuteQuery(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    var columns = from c in dt.Columns.Cast<DataColumn>() select c.ColumnName;
                    List<string> k = columns.ToList();

                    foreach (DataRow item in dt.Rows)
                    {
                        List<string> v = item.ItemArray.Select(x => $"'{x}'").ToArray().ToList();
                        string insert_sql = $"INSERT INTO {comboBox1.Text} ({string.Join(",", k)}) VALUES({string.Join(",", v)}) ";
                        sql_collection.Add(insert_sql);
                    }

                    if (sql_collection.Count > 0)
                    {

                        if (!string.IsNullOrWhiteSpace(comboBox12.Text) &&
                           !string.IsNullOrWhiteSpace(comboBox11.Text) &&
                           !string.IsNullOrWhiteSpace(comboBox14.Text) &&
                           !string.IsNullOrWhiteSpace(comboBox10.Text) &&
                           !string.IsNullOrWhiteSpace(comboBox9.Text)
                           )
                        {
                            string connstr = $"Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = {comboBox12.Text.Trim()})(PORT = {comboBox11.Text.Trim()}))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = {comboBox14.Text.Trim()}))); user Id = {comboBox10.Text.Trim()}; Password = {comboBox9.Text.Trim()}";
                            DbConnect Target = new DbConnect(connstr);
                            Target.ExecuteBatchNonQuery(sql_collection);
                        }
                        else
                        {
                            MessageBox.Show("Target DB can't be null !!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
    }
}