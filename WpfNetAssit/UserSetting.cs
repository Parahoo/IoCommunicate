using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WpfNetAssit.Communicate;
using WpfNetAssit.LogicalAction.ControlAction;

namespace WpfNetAssit
{
    [Serializable]
    public class UserSetting
    {
        /// <summary>
        /// 端口信息的配置
        /// </summary>
        public IoConnect.IosSetting IosSetting { get; set; } = new IoConnect.IosSetting();

        public CommunicatePageModel CommunicatePageModel { get; set; } = new CommunicatePageModel();

        public ControlActionBuilder RootBuilder { get; set; } = new ControlActionBuilder();

        public static UserSetting Default { get; set; } = new UserSetting();

        private const string settingFilename = "usersetting.xml";
        public void Save()
        {
            using (StreamWriter fs = new StreamWriter(settingFilename, false, Encoding.UTF8))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserSetting));
                xmlSerializer.Serialize(fs, this);
            }
        }

        public static UserSetting Load()
        {
            try
            {
                using (StreamReader fs = new StreamReader(settingFilename, Encoding.UTF8))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserSetting));
                    return xmlSerializer.Deserialize(fs) as UserSetting;
                }
            }
            catch
            {
                return new UserSetting();
            }

        }
    }
}
