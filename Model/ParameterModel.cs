using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.Model
{
    public class ParameterModel
    {
        // tên model
        public string Name { get; set; }
        public ParameterMain parameterMain { get; set; } = new ParameterMain();
        public ParameterRobot_Teaching parameterRobot_Teaching { get; set; } = new ParameterRobot_Teaching();
        public ParameterRobot_Setup parameterRobot_Setup { get; set; } = new ParameterRobot_Setup();
        public ParameterRobot_SetupTray parameterRobot_SetupTray { get; set; } = new ParameterRobot_SetupTray();
        public ParameterPLC_Loading parameterPLC_Loading { get; set; } = new ParameterPLC_Loading();
        public ParameterPLC_Vision parameterPLC_Vision { get; set; } = new ParameterPLC_Vision();
    }
}

