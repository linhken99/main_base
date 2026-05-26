using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
//using SymbolFactoryDotNet;
using Sunny.UI;

namespace Main_Base
{
    public class StatusDisplay
    {
        private static StatusDisplay _instance;
        private static readonly object datalock = new object();
        public static StatusDisplay Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (datalock)
                    {
                        if (_instance == null)
                        {
                            _instance = new StatusDisplay();
                        }
                    }
                }
                return _instance;
            }
        }
        public bool IsFormOpen { get; set; }
        private void SafeBegin(Control ctrl, Action action)
        {
            if (ctrl == null || ctrl.IsDisposed || IsFormOpen == false) return;

            if (ctrl.InvokeRequired)
                ctrl.BeginInvoke(new MethodInvoker(action));
            else
                action();
        }
        private void SafeInvoke(Control ctrl, Action action)
        {
            if (ctrl == null || ctrl.IsDisposed) return;

            if (ctrl.InvokeRequired)
                ctrl.Invoke(new MethodInvoker(action));
            else
                action();
        }
        //private object datalock = new object();
        // Hiển thị nhóm 3 trạng thái động cơ/bơm;/băng tải
        //public void stt_Motor(StandardControl st, int value)
        //{
        //    if (value == 1)
        //    {
        //        st.DiscreteValue1 = true;
        //        st.DiscreteValue2 = false;
        //    }
        //    if (value == 2)
        //    {
        //        st.DiscreteValue1 = false;
        //        st.DiscreteValue2 = true;
        //    }
        //    if (value != 1 & value != 2)
        //    {
        //        st.DiscreteValue1 = false;
        //        st.DiscreteValue2 = false;
        //    }
        //}
        //// Hiển thị nhóm 3 trạng thái - Van
        //public void stt_Valve(StandardControl st, int value)
        //{
        //    if (value == 1)
        //    {
        //        st.DiscreteValue1 = true;
        //        st.DiscreteValue2 = false;
        //    }
        //    if (value == 2)
        //    {
        //        st.DiscreteValue1 = false;
        //        st.DiscreteValue2 = true;
        //    }
        //    if (value != 1 & value != 2)
        //    {
        //        st.DiscreteValue1 = false;
        //        st.DiscreteValue2 = false;
        //    }
        //}

        //// Hiển thị nhóm 2 trạng thái - Cảm biến
        //public void stt_Sensor(StandardControl st, bool value)
        //{
        //    st.AnimationMode = SymbolFactoryNetEngine.AnimationModeOptions.DiscreteColorFill;
        //    st.Band1Color = System.Drawing.Color.Yellow;
        //    st.Band1Style = SymbolFactoryNetEngine.BandStyleOptions.Shaded;
        //    if (value == true)
        //    {
        //        st.DiscreteValue1 = true;
        //    }
        //    else
        //    {
        //        st.DiscreteValue1 = false;
        //    }
        //}
        //// Hiển thị nhóm 2 trạng thái - Đèn báo
        //public void stt_Lamp(StandardControl st, bool value)
        //{
        //    st.AnimationMode = SymbolFactoryNetEngine.AnimationModeOptions.DiscreteColorFill;
        //    st.Band1Color = System.Drawing.Color.Red;
        //    st.Band1Style = SymbolFactoryNetEngine.BandStyleOptions.Solid;
        //    if (value == true)
        //    {
        //        st.DiscreteValue1 = true;
        //    }
        //    else
        //    {
        //        st.DiscreteValue1 = false;
        //    }
        //}
        //public void stt_Lamp2(StandardControl st, bool value)
        //{
        //    st.AnimationMode = SymbolFactoryNetEngine.AnimationModeOptions.DiscreteColorFill;
        //    st.Band1Color = System.Drawing.Color.Yellow;
        //    st.Band1Style = SymbolFactoryNetEngine.BandStyleOptions.Solid;
        //    if (value == true)
        //    {
        //        st.DiscreteValue1 = true;
        //    }
        //    else
        //    {
        //        st.DiscreteValue1 = false;
        //    }
        //}
        //public void stt_Lamp3(StandardControl st, int value)
        //{
        //    st.AnimationMode = SymbolFactoryNetEngine.AnimationModeOptions.DiscreteColorFill;
        //    st.Band1Color = System.Drawing.Color.Yellow;
        //    st.Band1Style = SymbolFactoryNetEngine.BandStyleOptions.Solid;
        //    if (value == 1)
        //    {
        //        st.DiscreteValue1 = true;
        //    }
        //    else
        //    {
        //        st.DiscreteValue1 = false;
        //    }
        //}
        public void STT_Button_Display_Cylinder1(UIButton button1, UIButton button2, int value)
        {
            try
            {
                SafeBegin(button1, () =>
                {
                    if (value == 2)
                    {
                        button1.FillColor = Color.GreenYellow;
                        button2.FillColor = Control.DefaultBackColor;
                    }
                    else if (value == 1)
                    {
                        button1.FillColor = Control.DefaultBackColor;
                        button2.FillColor = Color.GreenYellow;
                    }
                    else
                    {
                        button1.FillColor = Control.DefaultBackColor;
                        button2.FillColor = Control.DefaultBackColor;
                    }
                });
            }
            catch { }
        }
        public void STT_Button_Display_Cylinder2(UIButton button1, UIButton button2, int value)
        {
            try
            {
                SafeBegin(button1, () =>
                {
                    if (value == 1)
                    {
                        button1.FillColor = Color.GreenYellow;
                        button2.FillColor = Control.DefaultBackColor;
                    }
                    else
                    {
                        button2.FillColor = Color.GreenYellow;
                        button1.FillColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Button_Display_Cylinder3(UIButton button, int value)
        {

            try
            {
                SafeBegin(button, () =>
                {
                    if (value == 0)
                    {
                        button.FillColor = Color.GreenYellow;
                    }
                    else
                    {
                        button.FillColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

        }
        public void STT_Button_Display_SV(UIButton button, bool value)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (value == true)
                    {
                        button.FillColor = Color.GreenYellow;
                    }
                    else
                    {
                        button.FillColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Button_Display_SV1(UIButton button, int value)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (value == 1)
                    {
                        button.FillColor = Color.GreenYellow;
                    }
                    else
                    {
                        button.FillColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Sensor(Label label, bool value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == true)
                    {
                        label.BackColor = Color.Yellow;
                    }
                    else
                    {
                        label.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }


        }
        public void STT_Sensor1(Label label, int value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == 1)
                    {
                        label.BackColor = Color.Yellow;
                    }
                    else
                    {
                        label.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_UILabel(UILabel label, bool value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == true)
                    {
                        label.BackColor = Color.Yellow;
                    }
                    else
                    {
                        label.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }     
        public void STT_Bool_UILight(UILight label, bool value)
        {

            try
            {
                SafeBegin(label, () =>
                {
                    if (value == true)
                    {
                        label.OnColor = Color.Green;
                    }
                    else
                    {
                        label.OnColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }


        }
        public void STT_Int_UILight(UILight label, int value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == 1)
                    {
                        label.OnColor = Color.Yellow;
                    }
                    else
                    {
                        label.OnColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Int_UILight1(UILight label, int value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == 1)
                    {
                        label.OnColor = Color.Red;
                    }
                    else
                    {
                        label.OnColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_SybolUILabel(UISymbolLabel label, bool value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == true)
                    {
                        label.BackColor = Color.Yellow;
                    }
                    else
                    {
                        label.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_SybolUILabel1(UISymbolLabel label, int value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == 1)
                    {
                        label.BackColor = Color.Yellow;
                    }
                    else
                    {
                        label.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Sensor2(UISymbolLabel label, bool value)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (value == true)
                    {
                        label.BackColor = Color.Yellow;
                    }
                    else
                    {
                        label.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Sensor3(UILight Light, bool value)
        {
            try
            {
                SafeBegin(Light, () =>
                {
                    if (value == true)
                    {
                        Light.BackColor = Color.Yellow;
                    }
                    else
                    {
                        Light.BackColor = Control.DefaultBackColor;
                    }
                });
            }

            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Button_Display_Control(Button button, int value)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (value == 1)
                    {
                        button.BackColor = Color.GreenYellow;
                    }
                    else
                    {
                        button.BackColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Button_Display_Control2(UIButton button, int value)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (value == 1)
                    {
                        button.FillColor = Color.GreenYellow;
                        button.FillDisableColor = Color.GreenYellow;
                    }
                    else
                    {
                        button.FillColor = Control.DefaultBackColor;
                        button.FillDisableColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Button_Display_Control3(UISymbolButton button, int value)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (value == 1)
                    {
                        button.FillColor = Color.GreenYellow;
                    }
                    else
                    {
                        button.FillColor = Control.DefaultBackColor;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void STT_Label(UILabel lab, int value)
        {
            try
            {
                SafeBegin(lab, () =>
                {
                    if (value == 1)
                    {
                        lab.BackColor = Color.Red;
                    }
                    else if (value == 2)
                    {
                        lab.BackColor = Color.GreenYellow;
                    }
                    else if (value == 3)
                    {
                        lab.BackColor = Color.PowderBlue;
                    }
                    else if (value != 1 && value != 2 && value != 3)
                    {
                        lab.BackColor = Color.Gainsboro;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void Update_Button_text(UIButton button, int value, string text)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (value == 1)
                    {
                        button.Text = text;
                    }
                    else
                    {
                        button.Text = text;
                    }
                });
            }
            catch { };
        }
        public void Update_Button_text2(UISymbolButton button, string text)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    button.Text = text;
                });
            }
            catch { };
        }
        public void Update_text(UITextBox textbox, int value)
        {
            try
            {
                SafeBegin(textbox, () =>
                {
                    textbox.Text = value.ToString();
                });
            }
            catch
            {
                MessageBox.Show("Errror");
            }
        }
        public void Update_text2(UITextBox textbox, string value)
        {
            try
            {
                SafeBegin(textbox, () =>
                {
                    textbox.Text = value;
                });
            }
            catch
            {
                MessageBox.Show("Cập nhập lỗi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Update_text3(UITextBox textbox, int value)
        {
            try
            {
                SafeBegin(textbox, () =>
                {
                    textbox.Text = (Convert.ToDouble(value) / 1000).ToString();
                });
            }
            catch
            {
                MessageBox.Show("Cập nhập lỗi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Update_IPtext(UIIPTextBox Uiiptextbox, string value)
        {
            try
            {
                SafeBegin(Uiiptextbox, () =>
                {
                    Uiiptextbox.Text = value.ToString();
                });
            }
            catch
            {
                MessageBox.Show("Errror");
            }
        }
        public void Update_text_rate(UITextBox textbox, double value)
        {
            try
            {
                SafeBegin(textbox, () =>
                {
                    if (value != 0)
                    {
                        textbox.Text = value.ToString("P");
                    }
                    else { textbox.Text = "0.00%"; }
                });
            }
            catch { }
        }
        public void Update_text_Double(UITextBox textbox, double value)
        {
            try
            {
                SafeBegin(textbox, () =>
                {
                    textbox.Text = value.ToString();

                });
            }
            catch { }
        }
        public void Update_text_toText(UITextBox textbox, UITextBox value)
        {
            try
            {
                SafeInvoke(textbox, () =>
                {
                    textbox.Text = value.Text;
                });
            }
            catch { }
        }
        public void Update_text_Button_RB(Button button, string value)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    button.Text = value;
                });
            }
            catch { }
        }
        public void Enable_Button(UIButton button, int status)
        {
            try
            {
                SafeBegin(button, () =>
                {
                    if (status == 1)
                    {
                        button.Enabled = true;
                    }
                    else
                    {
                        button.Enabled = false;
                    }
                });

            }
            catch { }
        }
        public void Enable_Panel(UIPanel _Panel, int status)
        {
            try
            {
                SafeBegin(_Panel, () =>
                {
                    if (status == 1)
                    {
                        _Panel.Enabled = true;
                    }
                    else
                    {
                        _Panel.Enabled = false;
                    }
                });
            }
            catch { }
        }
        public void Enable_TableLayoutPanelPanel(TableLayoutPanel _TableLayoutPanelPanel, int status)
        {
            try
            {
                SafeBegin(_TableLayoutPanelPanel, () =>
                {
                    foreach (Control ctl in _TableLayoutPanelPanel.Controls)
                    {
                        if (status == 1)
                        {

                            ctl.Enabled = true;
                            if (ctl is UITextBox txtbox)
                            {
                                txtbox.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                                txtbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);

                            }
                            //_TableLayoutPanelPanel.Enabled = true;
                        }
                        else
                        {
                            ctl.Enabled = false;
                            if (ctl is UITextBox txtbox)
                            {
                                txtbox.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                            }
                            //_TableLayoutPanelPanel.Enabled = false;
                        }
                    }
                });
            }
            catch { }
        }
        public void Enable_TableLayoutPanelPanel1(TableLayoutPanel _TableLayoutPanelPanel, int status)
        {
            try
            {
                SafeBegin(_TableLayoutPanelPanel, () =>
                {
                    foreach (Control ctl in _TableLayoutPanelPanel.Controls)
                    {
                        if (status == 1)
                        {
                            _TableLayoutPanelPanel.Enabled = true;
                        }
                        else
                        {
                            _TableLayoutPanelPanel.Enabled = false;
                        }
                    }
                });
            }
            catch { }
        }
        public void Enable_Group_Box(UIGroupBox groupbox, int status)
        {
            try
            {
                SafeBegin(groupbox, () =>
                {
                    if (status == 1)
                    {
                        groupbox.Enabled = true;
                    }
                    else
                    {
                        groupbox.Enabled = false;
                    }
                });
            }
            catch { }
        }
        public void Enable_Uitextbox(UITextBox Textbox_, int status)
        {
            try
            {
                SafeBegin(Textbox_, () =>
                {
                    if (status == 1)
                    {
                        Textbox_.Enabled = true;
                    }
                    else
                    {
                        Textbox_.Enabled = false;
                    }
                });
            }
            catch { }
        }
        public void Enable_UiSymbolButton(UISymbolButton btn_Syb, int status)
        {
            try
            {
                SafeBegin(btn_Syb, () =>
                {
                    if (status == 1)
                    {
                        btn_Syb.Enabled = true;
                    }
                    else
                    {
                        btn_Syb.Enabled = false;
                    }
                });
            }
            catch { }
        }
        public void Enable_Tabcontrol(UITabControl tab, int status)
        {
            try
            {
                SafeBegin(tab, () =>
                {
                    if (status == 1)
                    {
                        tab.Enabled = true;
                    }
                    else
                    {
                        tab.Enabled = false;
                    }
                });
            }
            catch { }
        }
        public void Enable_Combox(UIComboBox Combox, int status)
        {
            try
            {
                SafeBegin(Combox, () =>
                {
                    if (status == 1)
                    {
                        Combox.Enabled = true;
                    }
                    else
                    {
                        Combox.Enabled = false;
                    }
                });
            }
            catch { }
        }
        public void Update_process(UITextBox textbox, string process)
        {
            try
            {
                SafeBegin(textbox, () =>
                {
                    textbox.Text = process;
                });
            }
            catch { }
        }
        public int math_text_(TextBox textbox)
        {
            return Convert.ToInt16(textbox.Text);
        }
        public void IO_light(UILight Light, bool value)
        {
            try
            {
                SafeBegin(Light, () =>
                {
                    if (value == true)
                    {
                        Light.OnColor = Color.Yellow;
                    }
                    else
                    {
                        Light.OnColor = Color.Silver;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void IO_light1(UILight Light, int value)
        {
            try
            {
                SafeBegin(Light, () =>
                {
                    if (value == 1)
                    {
                        Light.OnColor = Color.Yellow;
                    }
                    else
                    {
                        Light.OnColor = Color.Silver;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void Get_speed_Robot(UITrackBar Trackbar, int value)
        {
            try
            {
                SafeBegin(Trackbar, () =>
                {
                    Trackbar.Value = value;
                });
            }
            catch { }
        }
        public void Get_Symbol(UISymbolButton btn, int value)
        {
            try
            {
                SafeBegin(btn, () =>
                {
                    btn.Symbol = value;
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void Update_Label(UILabel Lab, double value)
        {
            try
            {
                SafeBegin(Lab, () =>
                {
                    Lab.Text = value.ToString();
                });
            }
            catch
            {
                MessageBox.Show("Errror");
            }
        }
        public int ConvertInt(UITextBox txt)
        {
            int res = 0;
            try
            {
                SafeInvoke(txt, () =>
                {
                    res = Convert.ToInt32(txt.Text);
                });
            }
            catch
            {
                MessageBox.Show("Errror");
            }
            return res;
        }
        public void Update_UILabel2(UILabel label, string Str)
        {
            try
            {
                SafeBegin(label, () =>
                {
                    if (Str != null)
                    {
                        label.Text = Str;
                    }
                    else
                    {
                        label.Text = "Model?";
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void Update_UILabel3(UILabel label, string Str)
        {
            try
            {
                SafeInvoke(label, () =>
                {
                    if (Str != null)
                    {
                        label.Text = Str;
                    }
                    else
                    {
                        label.Text ="0";
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
    }
}
