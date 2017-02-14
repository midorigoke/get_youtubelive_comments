# Get YoutubeLive Comments CUI

YoutubeLiveのコメントを取得するやつ

<small>(GUIに対応するつもりはありません)</small>

## Discription

YoutubeLiveからコメントを受信してくれる。

他の方が作られているツールに無かった

* チャンネルIDだけでなくビデオIDから放送の指定
* 棒読みちゃんのアドレスとポートの指定
* 棒読みちゃんのプレフィックスの指定
* チャンネルオーナーのコメントをデフォルトで除外(オプションで表示も可)

を実装。

## Requirement

* [.NET Framework 4.6](https://www.microsoft.com/ja-jp/download/details.aspx?id=48137)

## Installation

0. **Windows 8.1** 以前を利用している場合は[.NET Framework 4.6](https://www.microsoft.com/ja-jp/download/details.aspx?id=48137)をインストール
0. [releases](https://github.com/midorigoke/get_youtubelive_comments/releases)からダウンロード
0. 展開し任意の場所に配置

## Usage

コマンドプロンプトから

* `get_youtubelive_comments_cui <video id> <api key> [option]...`
* `get_youtubelive_comments_cui <channel id> <api key> [option]...`

### Options

* `-h` ヘルプを表示する
* `-o` チャンネルオーナーのメッセージを表示する
* `-i <interval>` 取得間隔としてinterval(float型)
を使用する
* `-b <host>:<port>` 棒読みちゃんへの送信先を指定する
* `-p <prefix>` 棒読みちゃんへ送信するプレフィックスとしてprefixを使用する

## FAQ

### Q1

なんでGUI版を作らないの?

### A1

私にとって要らないから。
必要であれば他の人が作っているのでそっちを使ってください。

### Q2

あなた前にもYoutubeLiveのコメント取得ツール作ってたよね?

### A2

あの頃はシェルスクリプトしか書けなかった。
あれはwindowsで動かすの面倒だから。

### Q3

バグがあるんだけど?

### A3

IssueかPull Requestお待ちしています。

## License

このソフトウェアは[MIT License](https://github.com/midorigoke/get_youtubelive_comments/blob/master/LICENSE)のもとで公開されています。
