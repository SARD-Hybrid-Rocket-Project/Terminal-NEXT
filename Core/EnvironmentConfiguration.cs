using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MissionController.Core
{
    public class EnvironmentConfiguration
    {
        //定数
        public static string DefaultEnvironmentDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Constants.Developer,
            Constants.ApplicationName);//環境変数の保存場所を返す変数。場所はAppData\Local\SARD\FlightControllerを想定

        public static string DefaultEnvironmentFilePath = Path.Combine(DefaultEnvironmentDirectory, "config.json");


        //無線モジュール接続情報
        public TimeSpan ConnectionTimeout { get; set; }

        //システム設定
        public string ProfileDirectory { get; set; }


        //表示
        public SystemTheme ApplicaitonTheme { get; set; }
        public enum SystemTheme
        {
            Default = 0, Light = 1, Dark = 2
        }

        public EnvironmentConfiguration()
        {
            ApplicaitonTheme = SystemTheme.Default;
            ProfileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        public static EnvironmentConfiguration ReadConfiguration()//環境変数を読み込む。なければ新たに作る。
        {
            EnvironmentConfiguration config = new EnvironmentConfiguration();//環境変数インスタンスを初期化

            if(!File.Exists(DefaultEnvironmentFilePath)) WriteConfiguration(config);//ファイルがないなら作る

            try//設定ファイルを読み込む
            {
                string jsonString = File.ReadAllText(DefaultEnvironmentFilePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ReadConfiguration", MessageBoxButton.OK);
            }

            Debug.WriteLine("環境設定を正常に読み込めました");
            return config;//configインスタンスを返す。
        }
        public static bool WriteConfiguration(EnvironmentConfiguration config)//環境設定を保存する
        {
            if(!Directory.Exists(DefaultEnvironmentDirectory)) Directory.CreateDirectory(DefaultEnvironmentDirectory);//ディレクトリがないなら
            if (!File.Exists(DefaultEnvironmentFilePath)) using (File.Create(DefaultEnvironmentFilePath)) { }

            try
            {
                string jsonConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });//jsonにシリアライズ

                File.WriteAllText(DefaultEnvironmentFilePath, jsonConfig);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "WriteConfiguration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
        public static void ShowEnvironmentalSetting()
        {

        }
    }
}
