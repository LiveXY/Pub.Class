using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using Pub.Class;
using System.Data.OleDb;
using System.Threading;
using System.Reflection;

namespace EntityTool {
	public partial class frmEntity : Form {
		private Config config = TableStructureFactory.GetConfig();
		private Thread thread;
		private StringBuilder sbSqlCode = new StringBuilder();
		private string xmlFile = string.Empty;
		public static int spCount = 0;
		public static bool isAll = false;

		public frmEntity() {
			InitializeComponent();
			//System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
			System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
		}

		private void frmEntity_Load(object sender, EventArgs e) {
			//Data.ConnString = WebConfig.GetConn("ConnString");
			textBox1.Text = config.Project;
			config.OPList = TableStructureFactory.LoadOPList(config, entity => { 
				listBox.Items.Add((entity.isView ? "* " : "") + entity.Name, entity.isView ? false : true);
			});
			if (listBox.Items.Count > 0) listBox.SelectedIndex = 0;
			this.Text = "Entity Tool [{2}] - {1}连接共有{0}个表".FormatWith(listBox.Items.Count, Data.DBType, config.TemplateName);
		}

		private void listBox_DoubleClick(object sender, EventArgs e) {
			isAll = false;
			doRun();
		}

		private void doRun() {
			if (listBox.Items.Count <= 0) return;
			string tabName = listBox.Items[listBox.SelectedIndex].ToString();
			if (string.IsNullOrEmpty(tabName)) return;
			bool isView = false;
			if (tabName.IndexOf("* ") == 0) { tabName = tabName.Substring(2); isView = true; }

			string dalCode = string.Empty; string idalCode = string.Empty; string bllCode = string.Empty;
			string sqlCode = string.Empty; string baseCode = string.Empty;

			if (!string.IsNullOrEmpty(config.ModelPath) && config.OPList[listBox.SelectedIndex].Entity && config.DesignPattern == "Model-DAL-BLL") {
				tabPage1.Text = textBox1.Text + ".Model." + tabName;
				textBox.Text = TableStructureFactory.GetTableStructCode(config, tabName, textBox1.Text, out idalCode, out dalCode, out bllCode, out sqlCode, isView);
				Pub.Class.FileDirectory.DirectoryCreate(config.ModelPath + "\\Model\\");
				Pub.Class.FileDirectory.FileDelete(config.ModelPath + "\\Model\\" + tabName + ".cs");
				FileDirectory.FileWrite(config.ModelPath + "\\Model\\" + tabName + ".cs", textBox.Text);
			}
			if (!string.IsNullOrEmpty(config.EntityPath) && config.OPList[listBox.SelectedIndex].Entity && config.DesignPattern != "Model-DAL-BLL") {
				tabPage1.Text = textBox1.Text + ".Entity." + tabName;
				textBox.Text = TableStructureFactory.GetTableStructCode(config, tabName, textBox1.Text, out baseCode, out sqlCode, isView);
				Pub.Class.FileDirectory.DirectoryCreate(config.EntityPath + "\\Entity\\");
				Pub.Class.FileDirectory.FileDelete(config.EntityPath + "\\Entity\\" + tabName + ".cs");
				FileDirectory.FileWrite(config.EntityPath + "\\Entity\\" + tabName + ".cs", textBox.Text);
			}

			if (config.DesignPattern == "Model-DAL-BLL" && !string.IsNullOrEmpty(config.DALPath) && config.OPList[listBox.SelectedIndex].Entity) {
				tabPage2.Text = textBox1.Text + "." + config.TemplateName.Split('-')[3] + "DAL." + tabName + "DAL";
				textBox2.Text = dalCode;
				Pub.Class.FileDirectory.DirectoryCreate(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\");
				Pub.Class.FileDirectory.FileDelete(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs");

				Pub.Class.FileDirectory.DirectoryCreate(config.IDALPath + "\\IDAL\\");
				Pub.Class.FileDirectory.FileDelete(config.IDALPath + "\\IDAL\\I" + tabName + "DAL.cs");

				if (!string.IsNullOrEmpty(textBox2.Text.Trim())) {
					FileDirectory.FileWrite(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs", textBox2.Text);
					FileDirectory.FileWrite(config.IDALPath + "\\IDAL\\I" + tabName + "DAL.cs", idalCode);
				} else {
					Pub.Class.FileDirectory.FileDelete(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs");
					Pub.Class.FileDirectory.FileDelete(config.IDALPath + "\\IDAL\\I" + tabName + "DAL.cs");
				}

				tabPage3.Text = textBox1.Text + ".BLL." + tabName + "BLL";
				textBox3.Text = bllCode;
				Pub.Class.FileDirectory.DirectoryCreate(config.BLLPath + "\\BLL\\");
				Pub.Class.FileDirectory.FileDelete(config.BLLPath + "\\BLL\\" + tabName + "BLL.cs");
				if (!string.IsNullOrEmpty(textBox3.Text.Trim())) {
					FileDirectory.FileWrite(config.BLLPath + "\\BLL\\" + tabName + "BLL.cs", textBox3.Text);

					sbSqlCode.AppendLine(sqlCode);
					textBox4.Text = sqlCode;
				} else {
					Pub.Class.FileDirectory.FileDelete(config.BLLPath + "\\BLL\\" + tabName + "BLL.cs");
				}

				if (btnStart.Enabled) Clipboard.SetDataObject(textBox.Text);
			}
			if (config.DesignPattern != "Model-DAL-BLL" && !string.IsNullOrEmpty(config.FactoryPath) && config.OPList[listBox.SelectedIndex].Entity) {
				tabPage2.Text = textBox1.Text + "." + config.DesignPatternExtName + "." + tabName + "" + config.DesignPatternExtName + "";
				textBox2.Text = baseCode;
				Pub.Class.FileDirectory.DirectoryCreate(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\");
				Pub.Class.FileDirectory.FileDelete(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\" + tabName + "" + config.DesignPatternExtName + ".cs");
				if (!string.IsNullOrEmpty(textBox2.Text.Trim())) {
					FileDirectory.FileWrite(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\" + tabName + "" + config.DesignPatternExtName + ".cs", textBox2.Text);

					sbSqlCode.AppendLine(sqlCode);
					textBox3.Text = sqlCode;
				} else {
					Pub.Class.FileDirectory.FileDelete(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\" + tabName + "" + config.DesignPatternExtName + ".cs");
				}
				if (btnStart.Enabled) Clipboard.SetDataObject(textBox.Text);
			}
		}

		private void frmEntity_Resize(object sender, EventArgs e) {
			listBox.Height = this.ClientSize.Height - 25;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
			switch (tabControl1.SelectedIndex) {
				case 0: Clipboard.SetDataObject(textBox.Text); break;
				case 1: Clipboard.SetDataObject(textBox2.Text); break;
				case 2: Clipboard.SetDataObject(textBox3.Text); break;
			}
		}

		private void btnStart_Click(object sender, EventArgs e) {
			isAll = true;
			textBox.Text = "";
			textBox2.Text = "";
			textBox3.Text = "";
			sbSqlCode.Remove(0, sbSqlCode.Length);
			btnStart.Enabled = false;
			thread = new Thread(new ThreadStart(doStart));
			thread.IsBackground = true;
			thread.Start();
		}

		private void doStart() {
			isAll = true;
			for (int i = 0; i < listBox.Items.Count; i++) {
				if (!listBox.GetItemChecked(i)) { isAll = false; }
			}

			spCount = 0;
			if (!string.IsNullOrEmpty(config.AdminPath) && isAll) {
				Pub.Class.FileDirectory.DirectoryCreate(config.AdminPath + "\\xml\\");
				Pub.Class.FileDirectory.FileDelete(config.AdminPath + "\\xml\\db.aspx");
				FileDirectory.FileWrite(config.AdminPath + "\\xml\\db.aspx", "<div class='MenuTitlebar' style='top: 12px; left: 12px; width: 168px; height: 25px;' title='数据库管理'><table cellspacing='0' cellpadding='0'><tbody><tr style='width: 185px; height: 25px;'><td class='MenuTitlebarLeft_Head' style='width: 13px;'/><td class='MenuTitlebarMiddle_Head' style='width: 130px;'><div class='MenuTitle_Head' style='width: 130px; height: 25px; line-height: 25px;'>数据库管理</div></td><td class='MenuTitlebarRight_Open_Head' style='width: 25px;'/></tr></tbody></table></div>\n\r<div class='MenuBody_Head' style='border-width: 0px 1px 1px; padding: 9px 0px; overflow: hidden; top: 37px; left: 12px; width: 166px; opacity: 1;'>");
			}
			bool isNull = sbSqlCode.ToString().Trim().IsNullEmpty() ? true : false;
			if (isAll) isNull = false;
			//if (Data.DBType == "SqlServer") sbSqlCode.AppendLine(TableStructureFactory.GetPagerSPCode());
			for (int i = 0; i < listBox.Items.Count; i++) {
				if (listBox.GetItemChecked(i)) {
					listBox.SelectedIndex = i;
					doRun();
				}
			}
			btnStart.Enabled = true;
			if (!string.IsNullOrEmpty(config.AdminPath) && isAll) FileDirectory.FileWrite(config.AdminPath + "\\xml\\db.aspx", "</div>");

			if (!string.IsNullOrEmpty(config.DALPath) && !isNull) {
				string extFile = Server2.GetMapPath("") + "\\ext\\Sql\\SqlCode.sql";
				string extCode = FileDirectory.FileReadAll(extFile, Encoding.UTF8).ToString();
				Pub.Class.FileDirectory.DirectoryCreate(Server2.GetMapPath("") + "\\SQLCode\\");
				textBox4.Text = sbSqlCode.ToString() + "\r\n" + extCode;
				if (textBox4.Text.Trim().Length > 10) FileDirectory.FileWrite(Server2.GetMapPath("") + "\\SQLCode\\SQLCode" + Rand.RndDateStr() + ".sql", sbSqlCode.ToString() + "\r\n" + extCode);
			}
			this.Text = "Entity Tool [{3}] - {1}连接共有{0}个表{2}".FormatWith(listBox.Items.Count, Data.DBType, spCount > 0 ? "，自动生成{0}个存储过程".FormatWith(spCount) : "", config.TemplateName);
		}

		private void chkAll_CheckedChanged(object sender, EventArgs e) {
			if (chkAll.Checked) {
				for (int i = 0; i < listBox.Items.Count; i++) listBox.SetItemChecked(i, true);
			} else {
				for (int i = 0; i < listBox.Items.Count; i++) listBox.SetItemChecked(i, false);
			}
		}

		private void listBox_SelectedIndexChanged(object sender, EventArgs e) {
			property.SelectedObject = config.OPList[listBox.SelectedIndex];
		}

		private void property_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			Xml2 xml = new Xml2(xmlFile);
			var info = config.OPList[listBox.SelectedIndex];
			xml.SetAttr("root//Table[@Name='" + info.Table + "']", "Entity|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|UpdateAndInsert", "{8}|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{9}".FormatWith(
				info.Insert.ToString().ToLower(), info.Update.ToString().ToLower(), info.DeleteByID.ToString().ToLower(),
				info.IsExistByID.ToString().ToLower(), info.SelectByID.ToString().ToLower(), info.SelectPageList.ToString().ToLower(),
				info.SelectListByFK.ToString().ToLower(), info.SelectListByAll.ToString().ToLower(), info.Entity.ToString().ToLower(),
				info.UpdateAndInsert.ToString().ToLower()
			));
			xml.Save();
			xml.Close();
		}

		private void mnuCopy_Click(object sender, EventArgs e) {
			Clipboard.SetDataObject(textBox3.Text);
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {

		}

		private void mnuRun_Click(object sender, EventArgs e) {
			if (textBox3.Text.Length < 10) return;
			bool istrue = Data.ExecuteCommandWithSplitter(textBox3.Text, "\nGO\n", true);
			MessageBox.Show(istrue ? "执行成功！" : "执行失败！");
		}
	}
}