using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Pub.Class;
using System.IO;
using System.Collections;

namespace frmEmail {
    public partial class Form1 : Form {
        private long count = 0;
        private long newcount = 0;
        private Thread thread;
        private string filename = "";

        private int time = 0;
        private int timeH = 0;
        private int timeS = 0;
        private int timeM = 0;

        public Form1() {
            InitializeComponent();
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            button1.Enabled = false;
            timer1.Enabled = true;
            thread = new Thread(new ThreadStart(DoStart)); 
            thread.IsBackground = true;
            thread.Start();
        }

        private void DoStart(){
            string filePath = "email\\home_Txt\\".GetMapPath();

            string[] list = FileFolder.GetAllFile(filePath).ToString().Split('|');
            foreach (string info in list) {
                filename = filePath + info;

                string city = System.IO.Path.GetFileNameWithoutExtension(filename);
                string strSql = "select CityID from SS_City where CityName='{0}'".FormatWith(city);
                object value = Data.GetScalar(strSql);
                int type = object.ReferenceEquals(value,null) ? 0 : (int)value;

                StringBuilder sbEmail = new StringBuilder(); string lineText = ""; int index = 0;
                StreamReader reader = new StreamReader(filename);
                while ((lineText = reader.ReadLine()) != null){
                    //if (lineText.Length>10 && lineText.Length<50 && lineText.IsEmail()) {
                    //    strSql = "select count(0) from UC_User where Email='{0}'".FormatWith(lineText);
                    //    index = (int)Data.GetScalar(strSql);
                    //    if (index==0) {
                    //        strSql = string.Format("insert into UC_User(Email,CityID,City) values('{0}',{1},'{2}')", lineText, type, city);
                    //        Data.ExecSql(strSql);
                    //
                    //        count++;
                    //        newcount++;
                    //        InitLabelText();
                    //    }
                    //}
                    if (lineText.Length>10) {
                        try { 
                            Data.ExecSql(lineText); 
                            count++;newcount++;InitLabelText();
                        } catch { };
                    }
                }
                reader.Close();
                reader.Dispose();
                FileFolder.DelFile(filename);
            }
        }

        private void InitData(){
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            string strSql = "select count(0) from UC_User";
            count = (int)Data.GetScalar(strSql);
            InitLabelText();
        }
        private void InitLabelText(){
            lbl1.Text = count.ToString();
            lbl2.Text = newcount.ToString();
            lbl3.Text = filename;
        }
        private void Form1_Load(object sender, EventArgs e) {
            InitData();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            timeM++;
            if (timeM > 60) { timeM = 1; timeS++; }
            if (timeS > 60) { timeS = 1; timeH++; }
            lbl4.Text = string.Format("{0}:{1}:{2}", (timeH<10 ? "0" + timeH.ToString() : timeH.ToString()), (timeS<10 ? "0" + timeS.ToString() : timeS.ToString()), (timeM<10 ? "0" + timeM.ToString() : timeM.ToString()));
        }
    }
}
