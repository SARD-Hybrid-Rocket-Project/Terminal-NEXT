# SerialPortManagement クラス
SerialPortManagementはシリアルポートの接続や管理を行うクラスです。
名前はManagementとしていますが、実際にはインスタンスを生成して使うのであとで名前を変えるかもしれません。
## 定義
|||
|---|---|
|名前空間|FlightController.Core|
|ファイル名|SerialPortManagement.cs|
## 変数
    private SerialPort serialPort;
シリアルポートです。

    wirelessModuleType
接続する無線モジュールの形式を指定します。

## メソッド
### GetSerialPort()
    public SerialPort GetSerialPort()
SerialPortを返します
### GetSerialPortInformation()
    public static SerialPortInformation GetSerialPortInformation()
シリアルポート接続ダイアログを開いて、接続するシリアルポート情報を取得します。
### Connect(SerialPortInformation)
    public void Connect(SerialPortInformation serialPortInformation)
指定されたシリアルポートに接続します。
### Disconnect
    public void Disconnect()
シリアルポートに接続している場合、接続を終了します。