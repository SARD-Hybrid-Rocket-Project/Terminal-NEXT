using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace FlightController.Core
{
    public class EnvironmentConfiguration
    {
        //定数
        public static string DefaultEnvironmentDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Constants.ProjectName,
            Constants.ApplicationName);//環境変数の保存場所を返す変数。場所はAppData\Local\SARD\FlightControllerを想定

        public static string DefaultEnvironmentFilePath = Path.Combine(DefaultEnvironmentDirectory, "config.json");


        //無線モジュール接続情報
        public TimeSpan ConnectionTimeout { get; set; }

        //システム設定
        public string EnvironmentName { get; set; }
        public string filePath { get; set; }

        public EnvironmentConfiguration()
        {
            EnvironmentName = string.Empty;
            filePath = string.Empty;
        }


        public static EnvironmentConfiguration ReadConfiguration()//環境変数を読み込む。なければ新たに作る。
        {
            EnvironmentConfiguration config = new EnvironmentConfiguration();//環境変数インスタンスを初期化

            try//設定ファイルを読み込む
            {
                string jsonString = File.ReadAllText(Path.Combine(DefaultEnvironmentFilePath));
            }
            catch (DirectoryNotFoundException ex)//そもそもディレクトリが無かった場合にディレクトリを作って、初期設定を保存する。
            {
                try//ディレクトリを作成してファイルを保存する
                {
                    Directory.CreateDirectory(DefaultEnvironmentDirectory);//ディレクトリ作成
                    File.Create(DefaultEnvironmentFilePath);//ファイル作成

                    string jsonConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });//jsonにシリアライズ

                    File.WriteAllText(Path.Combine(DefaultEnvironmentFilePath), jsonConfig);//書き込み
                }
                catch (Exception e)//エラーが起きたら、エラーメッセージを出す。
                {
                    MessageBox.Show(e.ToString(), Constants.ApplicationName, MessageBoxButton.OK);
                }
            }
            catch (FileNotFoundException ex)//ファイルが見つからない場合
            {
                File.Create(DefaultEnvironmentFilePath);//ファイル作成

                string jsonConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });//jsonにシリアライズ

                File.WriteAllText(Path.Combine(DefaultEnvironmentFilePath), jsonConfig);//書き込み
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), Constants.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return config;//configインスタンスを返す。
        }
        public static bool WriteConfiguration(EnvironmentConfiguration config)
        {
            return true;
        }

    }
}
