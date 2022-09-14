using ToolITs.db;
using System.Data;
using Microsoft.VisualBasic.ApplicationServices;
using System.Net;
using System.Collections;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

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

            #region From
            comboBoxFromTns.DataSource = tns;
            comboBoxFromTns.DisplayMember = "Name";
            comboBoxFromTns.ValueMember = "Connectionstring";
            #endregion

            #region TO
            tns_oracle[] s = new tns_oracle[tns.Count];
            tns.CopyTo(s);
            comboBoxToTns.DataSource = s.ToList();
            comboBoxToTns.DisplayMember = "Name";
            comboBoxToTns.ValueMember = "Connectionstring";
            #endregion

            #region dcsmrp
            tns_oracle[] t = new tns_oracle[tns.Count];
            tns.CopyTo(t);
            comboBoxdb_tns.DataSource = t.ToList();
            comboBoxdb_tns.DisplayMember = "Name";
            comboBoxdb_tns.ValueMember = "Connectionstring";
            #endregion

            #region OpenFileAfterExport Checked
            OpenFileAfterExport.Checked = (bool)Settings1.Default["OpenFile"];
            #endregion
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
            if (history_sql.Count > 0)
            {
                if (history_current > 1)
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

        private void comboBoxToTns_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonDCSMR_MRP_Export_Click(object sender, EventArgs e)
        {
            string Min_Ver = comboBoxMINVER.Text;
            string Max_Ver = comboBoxMAXVER.Text;
            string Line_Id = textBoxLine_id.Text;
            string Track = textBoxTrack.Text;
            string? tns = comboBoxdb_tns.SelectedValue.ToString();
            DCS_MRP_Reoprt(Min_Ver, Max_Ver, Line_Id, Track, tns);
        }

        private void DCS_MRP_Reoprt(string min_Ver, string max_Ver, string line_Id, string track, string? tns)
        {

            string mr_ver_group = $"select distinct MR_VER from dcsmr_wo where(mr_ver between  '{min_Ver}' and  '{max_Ver}') ";

            DbConnect Target = new DbConnect(tns);
            DataSet dcs_mrp = Target.ExcuteQuery(mr_ver_group);

            DataTable dt = new DataTable();
            if (dcs_mrp != null && dcs_mrp.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in dcs_mrp.Tables[0].Rows)
                {
                    string mr_ver = item["MR_VER"].ToString();
                    #region sql calucation
                    string sql = @$" 

  WITH G
        AS(SELECT PLANT,
                     MIN(MAT) KEEP(DENSE_RANK FIRST ORDER BY WO ASC)-- 同plant, line_id, track, starttime ，wo ?, min mat_group
                        MAT_GROUP,
                     LINE_ID,
                     TRACK,
                     START_TIME
                FROM(SELECT DISTINCT DD.PLANT,
                                      DD.WO,
                                      DD.MAT,
                                      DD.BLU_PROCESS,
                                      DD.LINE_ID,
                                      DD.TRACK,
                                      DD.MR_VER,
                                      DD.START_TIME
                        FROM DCSMR_MRP DD
                       WHERE DD.CREATE_DATE >= TRUNC(SYSDATE) - 7)
               WHERE MR_VER = '{mr_ver}'--(SELECT MAX(MR_VER) FROM DCSMR_MRP)
            GROUP BY PLANT,
                     LINE_ID,
                     TRACK,
                     START_TIME
              HAVING COUNT(MAT) > 1),--?
       L

       AS(SELECT ROW_NUMBER()

                  OVER(PARTITION BY PLANT, LINE_ID, TRACK

                        ORDER BY START_TIME, WO ASC)

                     RN,
                  A.*
             FROM(SELECT DISTINCT DD.MR_VER,
                                   DD.PLANT,
                                   DD.WO,
                                   DD.MAT,
                                   NVL(G.MAT_GROUP, DD.MAT) MAT_GROUP,
                                   DD.BLU_PROCESS,
                                   DD.LINE_ID,
                                   DD.TRACK,
                                   DD.START_TIME,
                                   DD.S_TRACK_GI_QTY,
                                   DD.S_TRACK_PASS_QTY,
                                   DD.S_TRACK_OUT_QTY,
                                   DD.S_TRACK_IN_QTY,
                                   DD.SAFE_QTY

                     FROM DCSMR_MRP DD

                          LEFT JOIN G

                             ON     DD.PLANT = G.PLANT

                                AND DD.LINE_ID = G.LINE_ID

                                AND DD.TRACK = G.TRACK

                                AND DD.START_TIME = G.START_TIME

                    WHERE     DD.CREATE_DATE >= TRUNC(SYSDATE) - 7

                          AND DD.MR_VER = '{mr_ver}'
                                --(SELECT MAX(MR_VER) FROM DCSMR_MRP)
                                 ) A),
        SS(RN,
            MR_VER,
            PLANT,
            WO,
            MAT,
            MAT_GROUP,
            BLU_PROCESS,
            LINE_ID,
            TRACK,
            START_TIME,
            S_TRACK_GI_QTY,
            S_TRACK_PASS_QTY,
            S_TRACK_OUT_QTY,
            S_TRACK_IN_QTY,
            SAFE_QTY,
            FLAG)
        AS(SELECT L.*, 'Y' FLAG
              FROM L
             WHERE RN = 1
            UNION ALL
            SELECT L.RN,
                   L.MR_VER,
                   L.PLANT,
                   L.WO,
                   L.MAT,
                   L.MAT_GROUP,
                   L.BLU_PROCESS,
                   L.LINE_ID,
                   L.TRACK,
                   L.START_TIME,
                   L.S_TRACK_GI_QTY,
                   L.S_TRACK_PASS_QTY,
                   L.S_TRACK_OUT_QTY,
                   L.S_TRACK_IN_QTY,
                   L.SAFE_QTY,
                   CASE
                      WHEN    L.MAT_GROUP = SS.MAT_GROUP
                           OR L.START_TIME = SS.START_TIME
                      THEN
                         'Y'
                      ELSE
                         'N'
                   END
                      FLAG
              FROM SS
                   INNER JOIN L
                      ON     SS.RN + 1 = L.RN
                         AND SS.TRACK = L.TRACK
                         AND SS.LINE_ID = L.LINE_ID
                         AND SS.PLANT = L.PLANT),
  DCSMR_MRP_V as (SELECT RN,
          MR_VER,
          PLANT,
          WO,
          MAT,
          MAT_GROUP,
          BLU_PROCESS,
          LINE_ID,
          TRACK,
          START_TIME,
          S_TRACK_GI_QTY,
          S_TRACK_PASS_QTY,
          S_TRACK_OUT_QTY,
          S_TRACK_IN_QTY,
          SAFE_QTY,
          FLAG
     FROM SS)
 
  SELECT '{mr_ver}' MR_VER, A.BLU_PROCESS,A.LINE_ID,A.TRACK,A.A_TRACK_GI_QTY,A.A_TRACK_OUT_QTY,A.A_TRACK_IN_QTY,A.SAFE_QTY, C.A_TRACK_PASS_QTY
 ,A_TRACK_GI_QTY 軌道總已發量, A_TRACK_PASS_QTY 軌道總耗用量 ,SAFE_QTY 軌道安全庫存,
A_TRACK_IN_QTY 軌道轉入量,   A_TRACK_OUT_QTY 軌道轉出量
, A.A_TRACK_GI_QTY + A.A_TRACK_IN_QTY - C.A_TRACK_PASS_QTY - A.A_TRACK_OUT_QTY 線上庫存
                              FROM(SELECT LINE_ID,
                                             TRACK,
                                             BLU_PROCESS,
                                             SUM(S_TRACK_GI_QTY) A_TRACK_GI_QTY,
                                             SUM(S_TRACK_OUT_QTY) A_TRACK_OUT_QTY,
                                             SUM(S_TRACK_IN_QTY) A_TRACK_IN_QTY,
                                             MAX(SAFE_QTY) SAFE_QTY
                                        FROM DCSMR_MRP_V
                                       WHERE FLAG = 'Y'
                                    GROUP BY LINE_ID, TRACK, BLU_PROCESS) A,                            			
                                   (SELECT B.LINE_ID, B.TRACK, SUM(B.S_TRACK_PASS_QTY) A_TRACK_PASS_QTY
                                        FROM(SELECT DISTINCT LINE_ID,
                                                                TRACK,
                                                                WO,
                                                                S_TRACK_PASS_QTY
                                                  FROM DCSMR_MRP_V
                                                 WHERE FLAG = 'Y'
                                              GROUP BY LINE_ID,
                                                       TRACK,
                                                       WO,
                                                       S_TRACK_PASS_QTY) B
                                    GROUP BY B.LINE_ID, B.TRACK) C
                             WHERE     A.LINE_ID = C.LINE_ID
                                   AND A.TRACK = C.TRACK
                                   and a.track = '{track}'
                                   and A.LINE_ID = '{line_Id}'
--                                   AND A.A_TRACK_GI_QTY + A.A_TRACK_IN_QTY - C.A_TRACK_PASS_QTY - A.A_TRACK_OUT_QTY < A.SAFE_QTY
                                   ORDER BY A.LINE_ID,A.TRACK
";
                    #endregion
                    DataSet ds = Target.ExcuteQuery(sql);
                    DataTable temp = ds.Tables[0];
                    // append table to result
                    dt.Merge(temp);

                }
                string filename = "ToolITs.xlsx";
                SaveToExeclFile(Path.Combine(Application.StartupPath, filename), dt);
                //string currentDirectory = Environment.CurrentDirectory;
                //Console.WriteLine($"当前目录{currentDirectory}");
                string currentDirectory = OpenFileAfterExport.Checked ?  Path.Combine(Application.StartupPath, filename) : Application.StartupPath ;
                System.Diagnostics.Process.Start("explorer.exe ", currentDirectory);
            }
            else
            {
                MessageBox.Show("no mr_ver");
            }


        }
      private  void SaveToExeclFile(string fileName,DataTable dataTable)
        {
            try
            {
                //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetest1");//创建worksheet
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();//先删除文件，以免增加sheet时 重名错误
                }
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                    package.Save();

                    //// 文件头
                    //worksheet.InsertRow(1/*开始插入的位置*/, 1/*行数*/);
                    //worksheet.InsertColumn(1/*开始插入的位置*/, 3/*列数*/);
                    //worksheet.
                    //worksheet.Column(1).Width = 20;
                    //worksheet.Cells[1, 1].Value = "sn";

                    //worksheet.Column(2).Width = 20;
                    //worksheet.Cells[1, 2].Value = "iccid";

                    //worksheet.Column(3).Width = 20;
                    //worksheet.Cells[1, 3].Value = "name";

                    try
                    {
                        package.Save();//保存excel
                    }
                    catch (System.InvalidOperationException)
                    {
                        MessageBox.Show("文件已被打开,请关闭后点击[确定]");
                        bool fileIsNotClosed = true;
                        while (fileIsNotClosed)
                        {
                            try
                            {
                                package.Save();//保存excel
                            }
                            catch (System.InvalidOperationException)
                            {
                                MessageBox.Show("文件已被打开,请关闭后点击[确定]");
                                continue;
                            }
                            fileIsNotClosed = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"SaveToExeclFile: {e.ToString()}");
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            //string currentDirectory = Environment.CurrentDirectory;
            //Console.WriteLine($"当前目录{currentDirectory}");
            //System.Diagnostics.Process.Start("explorer.exe ", Application.StartupPath);

            //DataTable dataTable0 = new DataTable();

            //DataTable dataTable = new DataTable();
            //dataTable.Columns.Add("TEST");

            //dataTable.Rows.Add("s");

            //DataTable dataTable2 = new DataTable();
            //dataTable2.Columns.Add("TEST");

            //dataTable2.Rows.Add("ds");

            //dataTable0.Merge(dataTable);
            string? tns = comboBoxdb_tns.SelectedValue.ToString();
            DbConnect Target = new DbConnect(tns);
            DataSet dcs_mrp = Target.ExcuteQuery("SELECT * from CUSTOMERS WHERE CUSTOMER_ID < 10 ");
            DataSet dcs_mrp2= Target.ExcuteQuery("SELECT * from CUSTOMERS WHERE CUSTOMER_ID > 10 ");

            dcs_mrp.Merge(dcs_mrp2);
            SaveToExeclFile("test.xlsx", dcs_mrp.Tables[0]);
            string currentDirectory = OpenFileAfterExport.Checked ? Path.Combine(Application.StartupPath, "test.xlsx") : Application.StartupPath;
            System.Diagnostics.Process.Start("explorer.exe ", currentDirectory);
        }

        private void OpenFileAfterExport_CheckedChanged(object sender, EventArgs e)
        {
            ToolITs.Settings1.Default["OpenFile"] = OpenFileAfterExport.Checked;
            ToolITs.Settings1.Default.Save(); // Saves settings in application configuration file
        }
    }
}