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
        private string strSql = string.Empty;
        public static string ModelPath = string.IsNullOrEmpty(WebConfig.GetApp("ModelPath")) ? "" : WebConfig.GetApp("ModelPath").TrimEnd('/') + "/";
        public static string DALPath = string.IsNullOrEmpty(WebConfig.GetApp("DALPath")) ? "" : WebConfig.GetApp("DALPath").TrimEnd('/') + "/";
        public static string IDALPath = string.IsNullOrEmpty(WebConfig.GetApp("IDALPath")) ? "" : WebConfig.GetApp("IDALPath").TrimEnd('/') + "/";
        public static string BLLPath = string.IsNullOrEmpty(WebConfig.GetApp("BLLPath")) ? "" : WebConfig.GetApp("BLLPath").TrimEnd('/') + "/";
        public static string EntityPath = string.IsNullOrEmpty(WebConfig.GetApp("EntityPath")) ? "" : WebConfig.GetApp("EntityPath").TrimEnd('/') + "/";
        public static string FactoryPath = string.IsNullOrEmpty(WebConfig.GetApp("FactoryPath")) ? "" : WebConfig.GetApp("FactoryPath").TrimEnd('/') + "/";
        public static string AdminPath = string.IsNullOrEmpty(WebConfig.GetApp("AdminPath")) ? "" : WebConfig.GetApp("AdminPath").TrimEnd('/') + "/";
        public static string Author = string.IsNullOrEmpty(WebConfig.GetApp("Author")) ? "LiveXY" : WebConfig.GetApp("Author");
        public static string CopyRight = string.IsNullOrEmpty(WebConfig.GetApp("CopyRight")) ? "LiveXY" : WebConfig.GetApp("CopyRight");
        public static string TemplateName = string.IsNullOrEmpty(WebConfig.GetApp("TemplateName")) ? "Model-DAL-BLL-SqlServer-Text" : WebConfig.GetApp("TemplateName");
        public static string DesignPattern = string.IsNullOrEmpty(WebConfig.GetApp("DesignPattern")) ? "Static" : WebConfig.GetApp("DesignPattern");
        public static string DesignPatternExtName = string.IsNullOrEmpty(WebConfig.GetApp("DesignPatternExtName")) ? "Helper" : WebConfig.GetApp("DesignPatternExtName");
        public static bool UseOneProject = string.IsNullOrEmpty(WebConfig.GetApp("UseOneProject")) ? false : WebConfig.GetApp("UseOneProject").ToBool(false);
        public static int CacheTime = (string.IsNullOrEmpty(WebConfig.GetApp("CacheTime")) ? "0" : WebConfig.GetApp("CacheTime")).ToInt();
        public static string PageSize = (string.IsNullOrEmpty(WebConfig.GetApp("PageSize"))) ? "15" : WebConfig.GetApp("PageSize");
        public static string PagerSqlEnum = (string.IsNullOrEmpty(WebConfig.GetApp("PagerSqlEnum"))) ? "Base.PagerSqlEnum" : WebConfig.GetApp("PagerSqlEnum");
        public static string ProjectStartDate = string.IsNullOrEmpty(WebConfig.GetApp("ProjectStartDate")) ? DateTime.Now.ToString("yyyy-MM-dd") : WebConfig.GetApp("ProjectStartDate");

        private Thread thread;
        private StringBuilder sbSqlCode = new StringBuilder();
        public static IList<TableOperator> OPList = new List<TableOperator>();
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
            textBox1.Text = string.IsNullOrEmpty(WebConfig.GetApp("Project")) ? "Test" : WebConfig.GetApp("Project");
            xmlFile = "".GetMapPath() + textBox1.Text + ".xml";
            bool xmlExist = FileDirectory.FileExists(xmlFile);
            IList<TableEntity> list = TableFactory.GetTable();
            foreach (TableEntity entity in list) {
                listBox.Items.Add((entity.isView ? "* " : "") + entity.Name, entity.isView ? false : true);
                if (!xmlExist) OPList.Add(new TableOperator() { Table = entity.Name });
                else {
                    Xml2 xml = new Xml2(xmlFile);
                    string[] attrs = xml.GetAttr("root//Table[@Name='" + entity.Name + "']", "Name|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|Entity|UpdateAndInsert").Split('|');
                    if (attrs[0].IsNullEmpty()) {
                        OPList.Add(new TableOperator() { Table = entity.Name });
                        var info = OPList[OPList.Count - 1];
                        xml.AddNode("root", "Table", "Name|Entity|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|UpdateAndInsert", "{0}|{9}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{10}".FormatWith(
                            info.Table, info.Insert.ToString().ToLower(), info.Update.ToString().ToLower(), info.DeleteByID.ToString().ToLower(),
                            info.IsExistByID.ToString().ToLower(), info.SelectByID.ToString().ToLower(), info.SelectPageList.ToString().ToLower(),
                            info.SelectListByFK.ToString().ToLower(), info.SelectListByAll.ToString().ToLower(), info.Entity.ToString().ToLower(),
                            info.UpdateAndInsert.ToString().ToLower()
                        ));
                        xml.Save();
                    } else {
                        OPList.Add(new TableOperator() {
                            Table = entity.Name,
                            Insert = attrs[1] == "true" ? true : false,
                            Update = attrs[2] == "true" ? true : false,
                            DeleteByID = attrs[3] == "true" ? true : false,
                            IsExistByID = attrs[4] == "true" ? true : false,
                            SelectByID = attrs[5] == "true" ? true : false,
                            SelectPageList = attrs[6] == "true" ? true : false,
                            SelectListByFK = attrs[7] == "true" ? true : false,
                            SelectListByAll = attrs[8] == "true" ? true : false,
                            Entity = attrs[9] == "true" ? true : false,
                            UpdateAndInsert = attrs[10] == "true" ? true : false,
                        });
                    }
                    xml.Close();
                }
            }
            if (!xmlExist) CreateXML(xmlFile);
            if (listBox.Items.Count > 0) listBox.SelectedIndex = 0;
            this.Text = "Entity Tool [{2}] - {1}连接共有{0}个表".FormatWith(listBox.Items.Count, Data.DBType, TemplateName);
        }

        private void CreateXML(string fileName) {
            Xml2.Create(fileName, "", "", "utf-8", "<root></root>");
            Xml2 xml = new Xml2(fileName);
            foreach (var info in OPList) {
                xml.AddNode("root", "Table", "Name|Entity|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|UpdateAndInsert", "{0}|{9}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{10}".FormatWith(
                    info.Table, info.Insert.ToString().ToLower(), info.Update.ToString().ToLower(), info.DeleteByID.ToString().ToLower(),
                    info.IsExistByID.ToString().ToLower(), info.SelectByID.ToString().ToLower(), info.SelectPageList.ToString().ToLower(),
                    info.SelectListByFK.ToString().ToLower(), info.SelectListByAll.ToString().ToLower(), info.Entity.ToString().ToLower(),
                    info.UpdateAndInsert.ToString().ToLower()
                ));
            }
            xml.Save();
            xml.Close();
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

            string dalCode = string.Empty;string idalCode = string.Empty;string bllCode = string.Empty; 
            string sqlCode = string.Empty;string baseCode = string.Empty;

            if (!string.IsNullOrEmpty(ModelPath) && OPList[listBox.SelectedIndex].Entity && DesignPattern == "Model-DAL-BLL") {
                    tabPage1.Text = textBox1.Text + ".Model." + tabName;
                    textBox.Text = TableStructureFactory.GetTableStructCode(tabName, textBox1.Text, out idalCode, out dalCode, out bllCode, out sqlCode, isView);
                    Pub.Class.FileDirectory.DirectoryCreate(ModelPath + "\\Model\\");
                    Pub.Class.FileDirectory.FileDelete(ModelPath + "\\Model\\" + tabName + ".cs");
                    FileDirectory.FileWrite(ModelPath + "\\Model\\" + tabName + ".cs", textBox.Text);
            }
            if (!string.IsNullOrEmpty(EntityPath) && OPList[listBox.SelectedIndex].Entity && DesignPattern != "Model-DAL-BLL") {
                tabPage1.Text = textBox1.Text + ".Entity." + tabName;
                textBox.Text = TableStructureFactory.GetTableStructCode(tabName, textBox1.Text, out baseCode, out sqlCode, isView);
                Pub.Class.FileDirectory.DirectoryCreate(EntityPath + "\\Entity\\");
                Pub.Class.FileDirectory.FileDelete(EntityPath + "\\Entity\\" + tabName + ".cs");
                FileDirectory.FileWrite(EntityPath + "\\Entity\\" + tabName + ".cs", textBox.Text);
            }

            if (DesignPattern == "Model-DAL-BLL" && !string.IsNullOrEmpty(DALPath) && OPList[listBox.SelectedIndex].Entity) {
                tabPage2.Text = textBox1.Text + "." + TemplateName.Split('-')[3] + "DAL." + tabName + "DAL";
                textBox2.Text = dalCode;
                Pub.Class.FileDirectory.DirectoryCreate(DALPath + "\\" + TemplateName.Split('-')[3] + "DAL" + "\\");
                Pub.Class.FileDirectory.FileDelete(DALPath + "\\" + TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs");

                Pub.Class.FileDirectory.DirectoryCreate(IDALPath + "\\IDAL\\");
                Pub.Class.FileDirectory.FileDelete(IDALPath + "\\IDAL\\I" + tabName + "DAL.cs");

                if (!string.IsNullOrEmpty(textBox2.Text.Trim())) {
                    FileDirectory.FileWrite(DALPath + "\\" + TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs", textBox2.Text);
                    FileDirectory.FileWrite(IDALPath + "\\IDAL\\I" + tabName + "DAL.cs", idalCode);
                } else {
                    Pub.Class.FileDirectory.FileDelete(DALPath + "\\" + TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs");
                    Pub.Class.FileDirectory.FileDelete(IDALPath + "\\IDAL\\I" + tabName + "DAL.cs");
                }

                tabPage3.Text = textBox1.Text + ".BLL." + tabName + "BLL";
                textBox3.Text = bllCode;
                Pub.Class.FileDirectory.DirectoryCreate(BLLPath + "\\BLL\\");
                Pub.Class.FileDirectory.FileDelete(BLLPath + "\\BLL\\" + tabName + "BLL.cs");
                if (!string.IsNullOrEmpty(textBox3.Text.Trim())) {
                    FileDirectory.FileWrite(BLLPath + "\\BLL\\" + tabName + "BLL.cs", textBox3.Text);

                    
                    sbSqlCode.AppendLine(sqlCode);
                    textBox4.Text = sqlCode;
                } else {
                    Pub.Class.FileDirectory.FileDelete(BLLPath + "\\BLL\\" + tabName + "BLL.cs");
                }

                if (btnStart.Enabled) Clipboard.SetDataObject(textBox.Text);
            }
            if (DesignPattern != "Model-DAL-BLL" && !string.IsNullOrEmpty(FactoryPath) && OPList[listBox.SelectedIndex].Entity) {
                tabPage2.Text = textBox1.Text + "." + DesignPatternExtName + "." + tabName + "" + DesignPatternExtName + "";
                textBox2.Text = baseCode;
                Pub.Class.FileDirectory.DirectoryCreate(FactoryPath + "\\" + DesignPatternExtName + "\\");
                Pub.Class.FileDirectory.FileDelete(FactoryPath + "\\" + DesignPatternExtName + "\\" + tabName + "" + DesignPatternExtName + ".cs");
                if (!string.IsNullOrEmpty(textBox2.Text.Trim())) {
                    FileDirectory.FileWrite(FactoryPath + "\\" + DesignPatternExtName + "\\" + tabName + "" + DesignPatternExtName + ".cs", textBox2.Text);

                    sbSqlCode.AppendLine(sqlCode);
                    textBox3.Text = sqlCode;
                } else {
                    Pub.Class.FileDirectory.FileDelete(FactoryPath + "\\" + DesignPatternExtName + "\\" + tabName + "" + DesignPatternExtName + ".cs");
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
            if (!string.IsNullOrEmpty(AdminPath) && isAll) {
                Pub.Class.FileDirectory.DirectoryCreate(AdminPath + "\\xml\\");
                Pub.Class.FileDirectory.FileDelete(AdminPath + "\\xml\\db.aspx");
                FileDirectory.FileWrite(AdminPath + "\\xml\\db.aspx", "<div class='MenuTitlebar' style='top: 12px; left: 12px; width: 168px; height: 25px;' title='数据库管理'><table cellspacing='0' cellpadding='0'><tbody><tr style='width: 185px; height: 25px;'><td class='MenuTitlebarLeft_Head' style='width: 13px;'/><td class='MenuTitlebarMiddle_Head' style='width: 130px;'><div class='MenuTitle_Head' style='width: 130px; height: 25px; line-height: 25px;'>数据库管理</div></td><td class='MenuTitlebarRight_Open_Head' style='width: 25px;'/></tr></tbody></table></div>\n\r<div class='MenuBody_Head' style='border-width: 0px 1px 1px; padding: 9px 0px; overflow: hidden; top: 37px; left: 12px; width: 166px; opacity: 1;'>");
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
            if (!string.IsNullOrEmpty(AdminPath) && isAll) FileDirectory.FileWrite(AdminPath + "\\xml\\db.aspx", "</div>");

            if (!string.IsNullOrEmpty(DALPath) && !isNull) {
                string extFile = Server2.GetMapPath("") + "\\ext\\Sql\\SqlCode.sql";
                string extCode = FileDirectory.FileReadAll(extFile, Encoding.UTF8).ToString();
                Pub.Class.FileDirectory.DirectoryCreate(Server2.GetMapPath("") + "\\SQLCode\\");
                textBox4.Text = sbSqlCode.ToString() + "\r\n" + extCode;
                if (textBox4.Text.Trim().Length > 10) FileDirectory.FileWrite(Server2.GetMapPath("") + "\\SQLCode\\SQLCode" + Rand.RndDateStr() + ".sql", sbSqlCode.ToString() + "\r\n" + extCode);
            }
            this.Text = "Entity Tool [{3}] - {1}连接共有{0}个表{2}".FormatWith(listBox.Items.Count, Data.DBType, spCount > 0 ? "，自动生成{0}个存储过程".FormatWith(spCount) : "", TemplateName);
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e) {
            if (chkAll.Checked) {
                for (int i = 0; i < listBox.Items.Count; i++) listBox.SetItemChecked(i, true);
            } else {
                for (int i = 0; i < listBox.Items.Count; i++) listBox.SetItemChecked(i, false);
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e) {
            property.SelectedObject = OPList[listBox.SelectedIndex];
        }

        private void property_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            Xml2 xml = new Xml2(xmlFile);
            var info = OPList[listBox.SelectedIndex];
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