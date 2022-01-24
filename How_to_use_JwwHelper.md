# JwwHelperの使い方
## 準備
- jwwファイルのデータ形式を[http://www.jwcad.net/jwdatafmt.txt](http://www.jwcad.net/jwdatafmt.txt)で確認しましょう。JwwHelperの図形はこのファイルの説明に準拠しています。ただし、クラス名はCDataSenをJwwSenと言うように変更しています。また、座標などの構造体メンバは`p.x`のようなメンバを`p_x`のように変数に分解しています（C++/CLIで構造体のプロパティ公開の方法がわからなかったため）。
- 32ビット環境ではJwwHelper_x86.dll、64ビット環境ではJwwHelper_x64.dllをプロジェクトに追加し参照を追加します。


## 読み込み
JwwReaderオブジェクトを作りReadメソッドを呼び出します。
  ~~~
    using var reader = new JwwHelper.JwwReader();
    reader.Read(path, Completed);
  ~~~
Readの引数のpathはファイルのパス、次のCompletedは読み込み完了時に呼ばれるコールバックです。このコールバックの中で図形情報などの取得を行います。
~~~
private void Completed(JwwHelper.JwwReader reader)
{  
  //読み込まれたオブジェクトの処理
  foreach (var jwwData in reader.DataList)
  {
    switch(jwData){
      case JwwSen sen:
      //線
        ...
      break;
      case JwwEnko enko:
      //楕円、楕円弧
        ...
      break;
      default:
        ...
      break
    }
    .....
  }
}
~~~
JwwReaderには以下のプロパティーがあります。
- `JwwHeader Header`<a id="JwwHeader"></a>
  レイヤ名、色、設定など。図形データ本体までにあるデータをまとめてヘッダとしています。プロパティはほぼ[http://www.jwcad.net/jwdatafmt.txt](http://www.jwcad.net/jwdatafmt.txt)の名称に合わせました。
- `List<JwwData> DataList`
  jw_cadの図形のリスト。
- `List<JwwDataList> DataListList`
  ブロック図形の実体
- `JwwImage[] Images`
  同梱画像の配列。
####ブロック図形実体の読み込みについて
`JwwDataList`がブロック図形の実体です。図形の参照には`EnumerateDataList`メソッドを使います。
~~~
foreach (var dataList in reader.DataListList)
{
    dataList.EnumerateDataList(jwwData =>
    {
      switch(jwwData){
        case JwwSen sen:
        //線
          ...
        break;
        ...
      }
      return true;    //trueで処理継続
    });
}
~~~
#### 同梱画像の読み込みについて
`JwwImage`オブジェクトが同梱画像の実体です。主なプロパティーは
- `string ImageName`
  画像ファイル名。
- `byte[] Buffer`
  画像のデータです。画像には圧縮と非圧縮形式があります。JwwHelperでは圧縮の展開は行いません。使用するアプリ側で対応してください。JwwHelperSampleでは圧縮形式の展開を行っているので参考にしてください（非圧縮形式は見たことがないのでJwwHelperSampleでは実装していません）。


## 書き込み
まず、JwwWriterオブジェクトを作りヘッダーを初期化します。
~~~
  var w = new JwwHelper.JwwWriter();
  w.InitHeader(templatePath);
~~~ 
`InitHeader(templatePath)`の`templatePath`は初期化に使うテンプレートファイルのパスです。テンプレートファイルは任意のjwwファイルでこれによりヘッダーを初期化します。
テンプレートファイルを使わずにヘッダーの個々のプロパティに値を代入してもかまいません（項目が多くて大変と思います）。JwwHelperSampleのJwwViewerではリソースにテンプレートファイルを入れています。
ヘッダーは`JwwWriter`の`Header`プロパティでアクセスできます。初期化後にヘッダーのプロパティーに値を設定してください。
図形は`AddData`メソッドで図形オブジェクトを追加します。
ブロック図形実体は`AddDataList`メソッドで追加、同梱画像は`AddImage`メソッドです。
最後に書き込みを行います。
~~~
w.Write(path);
~~~
`path`は保存先のパスです。

## 注意点
- JwwHelperSampleのJwwViewerでは環境に応じて32bitか64bitのどちらのdllを使うか切り替えています。Program.cs内のMain関数を参照してください。
- Jw_cad Version 8.20から（と思った）いくつかの情報が文字図形データ（CDataMoji）として保存されています。そのため、図形が何も書かれていないファイルでも文字図形が存在します。例えば、線しか記入されていないjwwファイルを読み込む時に図形は線しかないと思い込むと思わぬミスを招きます。ライブラリを使うのでしたらそれほど問題ないのですが、ライブラリ開発中はかなり悩みました。
- jw_cadの文字コードはsjisですがJwwHelperでunicodeの変換を行っています。
