using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace Main_Base.UseControlData
{
    public partial class NewModel : UIForm2
    {
        public NewModel()
        {
            InitializeComponent();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(uiTextBox1.Text))
            {
                UIMessageBox.ShowWarning("Model name is empty");
                return;
            }

            Global.ModelName = uiTextBox1.Text.Trim();

            this.DialogResult = DialogResult.OK;  
            this.Close();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
