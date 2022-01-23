# JwwHelperの使い方
## 準備
- jwwファイルのデータ形式を[http://www.jwcad.net/jwdatafmt.txt](http://www.jwcad.net/jwdatafmt.txt)で確認しましょう。JwwHelperの図形はこのファイルで説明されているオブジェクトに準拠しています。ただし、クラス名はCDataSenをJwwSenと言うように変更しています。
- 32ビット環境ではJwwHelper_x86.dll、64ビット環境ではJwwHelper_x64.dllをプロジェクトに追加し参照を追加します。
- JwwHelperSampleのJwwViewerでは環境に応じてどちらのdllを使うか切り替えています。Program.cs内のMain関数を参照してください。


## 読み込み
- JwwReaderオブジェクトを作りReadメソッドを呼び出します。
~~~
    using var reader = new JwwHelper.JwwReader(Completed);
    reader.Read(path);
~~~
コンストラクタのCompletedは読み込み完了時に呼ばれるコールバック、Readの引数はファイルのパスです。
~~~
private void Completed(JwwHelper.JwwReader reader)
{  
    //読み込まれたオブジェクトの処理
}
~~~
JwwReaderには以下のプロパティーがあります。これらを使ってjwwファイルのデータにアクセスします。
- `JwwHeader Header`
  レイヤ名、色、設定など。図形データ本体までにあるデータをまとめてヘッダとしています。
- `List<JwwData> DataList`
  jw_cadの図形のリスト。
- `List<JwwDataList> DataListList`
  ブロック図形の実体
- `JwwImage[] Images`
  同梱画像
