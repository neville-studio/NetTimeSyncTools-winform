using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text.Json;
using System.Windows.Forms;


namespace NetTimeSyncTools_winform
{
    public static class UserDefinedGlobalData
    {
        public static List<NTPClass> globalData = new List<NTPClass>();
        public static List<NTPClass> MessageTable = new List<NTPClass>();
        public static ResourceManager resourceManager;
        static void ReadMessageTable()
        {
            int culture = System.Globalization.CultureInfo.CurrentCulture.LCID;
            string fn = @".\i18n\messageTable." + culture + ".resx";
            if (File.Exists(fn))
            {
                resourceManager = new ResourceManager("NetTimeSyncTools_winform.i18n.messageTable." + culture, typeof(Form1).Assembly);
            }
            else
            {
                resourceManager = new ResourceManager("NetTimeSyncTools_winform.i18n.messageTable", typeof(Form1).Assembly);
            }
        }
        public static void WriteGlobalNTPGlobalDataToDB()
        {
            string filePath = @".\globalData.json";
            var partialObjectsList = new List<object>();
            foreach (var item in globalData)
            {
                var partialObject = new { item.serverIdentifier, item.serverName, item.Version_Number };
                partialObjectsList.Add(partialObject);
            }
            // 一次性将整个列表序列化为 JSON 字符串
            string jsonString = JsonSerializer.Serialize(partialObjectsList);
            // 将序列化后的 JSON 字符串写入文件
            File.WriteAllText(filePath, jsonString);
        }
        public static void ReadGlobalNTPGlobalDataFromDB()
        {
            string filePath = @".\globalData.json";
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                var partialObjectsList = JsonSerializer.Deserialize<List<NTPClass>>(jsonString);
                globalData = partialObjectsList;
                Form1.updateNotify(1);
            }
        }
        static UserDefinedGlobalData()
        {
            ReadGlobalNTPGlobalDataFromDB();
            ReadMessageTable();
        }

    }

    internal static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Form1());
        }




    }
}
