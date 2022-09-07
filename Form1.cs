using ToolITs.db;
using System.Data;
using Microsoft.VisualBasic.ApplicationServices;
using System.Net;
using System.Collections;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ToolITs
{
    public partial class Form1 : Form
    {
        List<string> history_sql = new List<string>();
        int history_current = -1;
        List<string> history_tranfer = new List<string>();
        int tranfer_current = -1;

        private MyService service;
        private List<tns_oracle> tns;
        public Form1()
        {
            InitializeComponent();
            initConfig();
            initdropdownlist();
        }

        private void initdropdownlist()
        {

            comboBoxFromTns.DataSource = tns;
            comboBoxFromTns.DisplayMember = "Name";
            comboBoxFromTns.ValueMember = "Connectionstring";

            comboBoxToTns.DataSource = tns;
            comboBoxToTns.DisplayMember = "Name";
            comboBoxToTns.ValueMember = "Connectionstring";
        }

        private void initConfig()
        {
            #region config
            // 1. crete a service collection for DI
            var serviceCollection = new ServiceCollection();
            // 2. Build a configuration 
            IConfiguration configuration;
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile("appsettings.json").Build(); ;
            //3. Add the configuration to the service collection 
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton<MyService>();

            // start service
            var serviceProvider = serviceCollection.BuildServiceProvider();
            service = serviceProvider.GetRequiredService<MyService>();
            tns = service.GetTns();

            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = "";
            IDataObject dataObject = Clipboard.GetDataObject();
            try
            {
                if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    text = dataObject.GetData(DataFormats.Text).ToString();
                }
            }
            catch
            {
                MessageBox.Show("剪貼簿的資料空白或格式不正確！！");
                return;
            }
            string text2 = "";
            while (text.IndexOf("\r\n") > -1)
            {
                text2 = string.Format("{0}'{1}',", text2, text.Substring(0, text.IndexOf("\r\n")).Trim());
                text = text.Substring(text.IndexOf("\r\n") + 2);
            }
            if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
            {
                text2 = $"({text2.Substring(0, text2.Length - 1)})";
                textBox1.Text = text2;
            }
            else if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
            {
                text2 = $"({text2}'{text.Trim()}')";
                textBox1.Text = text2;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                textBox1.Text = text;
            }
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

        private void buttonhadle_confirm_Click(object sender, EventArgs e)
        {
            string sql = textBox2.Text;
            history_sql.Add(sql);

            DbConnect dbConnect;
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("To can be null!");
                return;
            }
            string inset_per = $" \r\nREM INSERTING into {comboBox1.Text} \r\nSET DEFINE OFF;\r\n";

            string connstr = comboBoxFromTns.SelectedValue.ToString(); 
                dbConnect = new DbConnect(connstr);

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


        private void buttonMigration_Click(object sender, EventArgs e)
        {

            string sql = textBox2.Text;
            DbConnect source;
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("To Table can be null!");
                return;
            }

            string connstr = comboBoxFromTns.SelectedValue.ToString();
                source = new DbConnect(connstr);
            

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

                            string Target_connstr = comboBoxToTns.SelectedValue.ToString();
                            DbConnect Target = new DbConnect(Target_connstr);
                            Target.ExecuteBatchNonQuery(sql_collection);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void button_hadling_next_Click(object sender, EventArgs e)
        {
            if (history_sql.Count > 0)
            {
                if (history_current + 1 < history_sql.Count)
                {
                    history_current += 1;
                    textBox2.Text = history_sql[history_current].ToString();
                }
            }
        }

        private void button_hadle_pre_click(object sender, EventArgs e)
        {
            if (history_sql.Count>0)
            {
                if (history_current>1)
                {
                    history_current -= 1;
                    textBox2.Text = history_sql[history_current].ToString();
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (history_tranfer.Count > 0)
            {
                if (tranfer_current > 1)
                {
                    tranfer_current -= 1;
                    textBox1.Text = history_tranfer[tranfer_current].ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox1.Text);
            string data = textBox1.Text;
            history_tranfer.Add(data);

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (history_tranfer.Count > 0)
            {
                if (tranfer_current + 1 < history_tranfer.Count)
                {
                    tranfer_current += 1;
                    textBox1.Text = history_tranfer[tranfer_current].ToString();
                }
            }
        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}