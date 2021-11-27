using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatcherAndDispatcher;

namespace BindingValueExample
{
    public partial class Form1 : Form
    {
        private string _filePath;
        private string _fileDir;
        private string _fileName;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var model = new TestModel { Value = 50, BtnName = "绑定事件" };
            new ViewBind(this, model);

        }
    }
}
