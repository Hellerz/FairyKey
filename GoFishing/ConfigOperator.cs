using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GoFishing
{
    public class ConfigOperator
    {

        private static readonly AppSettingsSection AppSettings;
        private static readonly Configuration Config;

        static ConfigOperator()
        {
            Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettings = Config.AppSettings;
        }

        #region 从配置文件获取Value

        /// <summary>
        /// 从配置文件获取Value
        /// </summary>
        /// <param name="key">配置文件中key字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string AchiveValue(string key, string defaultValue = null)
        {
            if (AppSettings.Settings[key] != null)
            {
                return AppSettings.Settings[key].Value;
            }
            if (defaultValue!=null)
            {
                AddValue(key, defaultValue);
            }
            return defaultValue;
        }

        /// <summary>
        /// 从配置文件获取Value
        /// </summary>
        /// <param name="key">配置文件中key字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetValue(string key, string defaultValue = null)
        {
            if (AppSettings.Settings[key] != null)
            {
                return AppSettings.Settings[key].Value;
            }

            return defaultValue;
        }

        public static Dictionary<string, string> GetAllValues()
        {
            if (AppSettings.Settings != null && AppSettings.Settings.Count > 0)
            {
                return AppSettings.Settings.Cast<object>().ToDictionary(setting => ((KeyValueConfigurationElement)setting).Key, setting => ((KeyValueConfigurationElement)setting).Value);
            }
            return null;
        }

        #endregion

        #region 设置配置文件

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="key">配置文件中key字符串</param>
        /// <param name="value">配置文件中value字符串</param>
        /// <returns></returns>
        public static bool AddValue(string key, string value)
        {
            if (AppSettings.Settings[key] != null)
            {
                throw new ConfigurationErrorsException("配置键已存在");
            }
            try
            {
                AppSettings.Settings.Add(key, value);
                Config.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="key">配置文件中key字符串</param>
        /// <param name="value">配置文件中value字符串</param>
        /// <returns></returns>
        public static bool SetValue(string key, string value)
        {
            if (AppSettings.Settings[key] == null)
            {
                throw new ConfigurationErrorsException("配置键不存在");
            }
            try
            {
                AppSettings.Settings[key].Value = value;
                Config.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="key">配置文件中key字符串</param>
        /// <param name="value">配置文件中value字符串</param>
        /// <returns></returns>
        public static bool CoverValue(string key, string value)
        {
            try
            {
                if (AppSettings.Settings[key] != null)
                {
                    AppSettings.Settings[key].Value = value;
                }
                else
                {
                    AppSettings.Settings.Add(key, value);
                }
                Config.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteValue(string key)
        {
            try
            {
                if (AppSettings.Settings[key] != null)
                {
                    AppSettings.Settings.Remove(key);
                    Config.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion
    }
}
