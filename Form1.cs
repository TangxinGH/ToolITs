using HZH_Controls.Controls;
using HZH_Controls.Forms;

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
            FrmTips.ShowTipsError(this, "Error提示信息");
            FrmTips.ShowTipsInfo(this, "Info提示信息");
            FrmTips.ShowTipsSuccess(this, "Success提示信息");
            FrmTips.ShowTipsWarning(this, "Warning提示信息");
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void secondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrmDialog.ShowDialog(this, "是否再显示一个没有取消按钮的提示框？", "模式窗体测试", true) == System.Windows.Forms.DialogResult.OK)
            {
                FrmDialog.ShowDialog(this, "这是一个没有取消按钮的提示框", "模式窗体测试");
            }
        }

        private void otherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputs frm = new FrmInputs("动态多输入窗体测试",
               new string[] { "姓名", "电话", "身份证号", "住址" },
               new Dictionary<string, HZH_Controls.TextInputType>() { { "电话", HZH_Controls.TextInputType.Regex }, { "身份证号", HZH_Controls.TextInputType.Regex } },
               new Dictionary<string, string>() { { "电话", "^1\\d{10}$" }, { "身份证号", "^\\d{18}$" } },
               new Dictionary<string, KeyBoardType>() { { "电话", KeyBoardType.UCKeyBorderNum }, { "身份证号", KeyBoardType.UCKeyBorderNum } },
               new List<string>() { "姓名", "电话", "身份证号" });
            frm.ShowDialog(this);
        }
    }
}