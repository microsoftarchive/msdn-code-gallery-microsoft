# A basic Windows service in VB (VBWindowsService)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- Windows Service
## Updated
- 03/01/2012
## Description

<h1>Windows サービス (VBWindowsService)</h1>
<h2>はじめに</h2>
<p class="MsoNormal">このコード サンプルでは、Visual Basic.NET で非常に基本的な Windows サービス アプリケーションを作成する方法を紹介します。このサンプル Windows サービスでは、アプリケーション イベント ログにサービスの開始と停止情報を記録し、スレッド プールのワーカー スレッドでサービスのメイン関数を実行する方法を示します。この Windows サービス スケルトンは、独自のビジネス要件に合わせて簡単に拡張できます。<span>&nbsp;
</span></p>
<h2>サンプルの実行</h2>
<p class="MsoNormal">次の手順では、Windows サービス サンプルのデモについて説明します。</p>
<p class="MsoNormal">手順 1: Visual Studio 2010 でサンプル プロジェクトを正常にビルドすると、サービス アプリケーション VBWindowsService.exe が作成されます。</p>
<p class="MsoNormal"><span><img src="52977-image.png" alt="" width="576" height="194" align="middle">
</span></p>
<p class="MsoNormal">手順 2: Visual Studio 2010 コマンド プロンプトを管理者として実行し、サンプル プロジェクトの出力フォルダーに移動してから、次のコマンドを入力してサービスをインストールします。</p>
<p class="MsoNormal"><span>&nbsp; </span><span style="background:#D9D9D9">InstallUtil.exe VBWindowsService.exe</span></p>
<p class="MsoNormal">次のような出力が表示された場合は、サービスが正常にインストールされています。</p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal"><span><img src="52978-image.png" alt="" width="576" height="676" align="middle">
</span></p>
<p class="MsoNormal">この出力が表示されない場合は、出力フォルダーで VBWindowsService.InstallLog ファイルを検索して失敗の原因を調べてください。</p>
<p class="MsoNormal">手順 3: サービス管理コンソール (services.msc) を開きます。サービスの一覧で &quot;VBWindowsService Sample Service&quot; を見つけることができます。</p>
<p class="MsoNormal"><span><img src="52979-image.png" alt="" width="576" height="338" align="middle">
</span></p>
<p class="MsoNormal">手順 4: サービス管理コンソールで VBWindowsService サービスを右クリックし、[開始] をクリックして、サービスを開始します。イベント ビューアーを開き、Windows ログ/Application に移動します。VBWindowsService では、2 つのイベントが次のイベント メッセージとともに表示されます。&quot;VBWindowsService in OnStart.&quot; と &quot;Service started successfully.&quot; です。</p>
<p class="MsoNormal"><span><img src="52980-image.png" alt="" width="576" height="259" align="middle">
</span></p>
<p class="MsoNormal">サービス管理コンソールでサービスを右クリックし、[停止] を選択して、サービスを停止します。イベント ビューアー/Windows ログ/Application に VBWindowsService の新しい 2 つのイベントが次のメッセージとともに表示されます。&quot;VBWindowsService in OnStop&quot; と &quot;Service stopped successfully&quot; です。<span>
</span></p>
<p class="MsoNormal"><span><img src="52981-image.png" alt="" width="576" height="253" align="middle">
</span><span>&nbsp;</span></p>
<p class="MsoNormal">手順 5: サービスをアンインストールするには、管理者として実行している Visual Studio 2010 のコマンド プロンプトで次のコマンドを入力します。</p>
<p class="MsoNormal"><span style="background:#D9D9D9"><span>&nbsp; </span>InstallUtil /u VBWindowsService.exe</span></p>
<p class="MsoNormal">サービスが正常に停止して削除されると、次の出力が表示されます。</p>
<h2><span><img src="52982-image.png" alt="" width="576" height="676" align="middle">
</span></h2>
<h2>セットアップと削除<span> </span></h2>
<h3>開発環境の場合</h3>
<p class="MsoNormal">A. セットアップ</p>
<p class="MsoNormal">管理者特権の Visual Studio 2010 コマンド プロンプトでコマンド &quot;Installutil.exe VBWindowsService.exe&quot; を実行します。ローカル サービス コントロール マネージャー データベースに、サービスとして VBWindowsService.exe がインストールされます。</p>
<p class="MsoNormal">B. クリーンアップ</p>
<p class="MsoNormal">管理者特権の Visual Studio 2010 コマンド プロンプトでコマンド &quot;Installutil.exe VBWindowsService.exe&quot; を実行します。VBWindowsService<span>
</span>サービスが停止され、ローカル サービス コントロール マネージャー データベースから削除されます。</p>
<h3>展開環境での手順</h3>
<p class="MsoNormal">A. セットアップ</p>
<p class="MsoNormal">VBWindowsServiceSetup(x86) セットアップ プロジェクトの出力、VBWindowsServiceSetup(x86).msi を x86 オペレーティング システムにインストールします。ターゲット プラットフォームが x64 の場合、VBWindowsServiceSetup(x64) セットアップ プロジェクトで出力された VBWindowsServiceSetup(x64).msi をインストールします。</p>
<p class="MsoNormal">B. 削除</p>
<p class="MsoNormal">VBWindowsServiceSetup(x86) セットアップ プロジェクトの出力、VBWindowsServiceSetup(x86).msi を x86 オペレーティング システムにインストールします。ターゲット プラットフォームが x64 の場合、VBWindowsServiceSetup(x64) セットアップ プロジェクトで出力された VBWindowsServiceSetup(x64).msi をアンインストールします。</p>
<h2>コードの使用</h2>
<h3>A. Windows サービスの作成</h3>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/ja-jp/library/aa983583.aspx">http://msdn.microsoft.com/ja-jp/library/aa983583.aspx</a></p>
<p class="MsoNormal">手順 1: &quot;VBWindowsService&quot; という名前の新しい Visual Basic / Windows / Windows サービス プロジェクトを Visual Studio 2010 に追加します。<a class="libraryLink" href="https://msdn.microsoft.com/ja-JP/library/System.ServiceProcess.ServiceBase.aspx" target="_blank" title="Auto generated link to System.ServiceProcess.ServiceBase">System.ServiceProcess.ServiceBase</a> から継承される &quot;Service1&quot; という名前のコンポーネント クラスがプロジェクトのテンプレートによって自動的に追加されます。</p>
<p class="MsoNormal">手順 2: 既定の Service1 の名前を &quot;SampleService&quot; に変更します。サービスをデザイナーで開き、ServiceName プロパティを VBWindowsService に設定します。</p>
<p class="MsoNormal">手順 3: カスタム イベントのログ機能をサービスに追加するために、イベント ログ コンポーネントをツールボックスからデザイン ビューにドラッグ アンド ドロップし、Log プロパティを Application に設定し、Source プロパティを VBWindowsService に設定します。イベント ログ コンポーネントはメッセージを Application ログに記録するために使用されます。</p>
<p class="MsoNormal">手順 4: サービスの開始および停止で発生する事象を定義するには、プロジェクトの作成時に自動的に上書きされているメソッドの OnStart と OnStop をコード エディターで検索し、サービスの実行を開始したときに発生する事象を特定するコードを記述します。この例では、サービスの開始と停止の情報を Applicaion イベント ログに記録し、サービスのメイン関数をスレッド プールのワーカー スレッドで実行する方法を示します。<span>&nbsp;</span>SampleService.OnStart
 はサービスの開始時に実行され、サービスの開始情報をログする EventLog.WriteEntry を呼び出します。また、これはワーカー スレッドで実行するサービスのメイン関数 (SampleService.ServiceWorkerThread) をキューに登録する ThreadPool.QueueUserWorkItem を呼び出します。</p>
<p class="MsoNormal"><strong>注:</strong> サービス アプリケーションは、長時間実行できるように設計されています。そのため、通常はシステムの何かをポーリングまたは監視します。監視は OnStart メソッドで設定されます。ただし、OnStart メソッドでは、実際に監視を行いません。サービスの動作が開始された後、OnStart メソッドはオペレーティング システムに制御を返す必要があります。永遠にループすることや、ブロックすることがないようにしなければなりません。単純な監視メカニズムを設定する一般的な方法の
 1 つは、OnStart でタイマーを作成することです。タイマーはコード内でイベントを定期的に発生させます。そのときに、サービスで監視を行うことができます。他には、新しい</p>
<p class="MsoNormal">スレッドを作成して、サービスのメイン関数を実行する方法があります。このサンプル コードでは、この方法を示しています。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre class="vb" id="codePreview">Protected Overrides Sub OnStart(ByVal args() As String)
    ' Log a service start message to the Application log.
    Me.EventLog1.WriteEntry(&quot;VBWindowsService in OnStart.&quot;)


    ' Queue the main service function for execution in a worker thread.
    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ServiceWorkerThread))
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">SampleService.OnStop はサービスの停止時に実行され、サービスの停止情報をログする EventLog.WriteEntry を呼び出します。次に、メンバー変数 &quot;stopping&quot; を true に設定してサービスが停止中であることを示し、&quot;stoppedEvent&quot; イベント オブジェクトによって通知されるサービスのメイン関数が終了するのを待機します。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre class="vb" id="codePreview">Protected Overrides Sub OnStop()
    ' Log a service stop message to the Application log.
    Me.EventLog1.WriteEntry(&quot;VBWindowsService in OnStop.&quot;)


    ' Indicate that the service is stopping and wait for the finish of 
    ' the main service function (ServiceWorkerThread).
    Me.stopping = True
    Me.stoppedEvent.WaitOne()
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">SampleService.ServiceWorkerThread は、スレッド プールのワーカー スレッドで実行されます。名前付きパイプを介してクライアント アプリケーションと通信するなど、サービスのメイン関数を実行します。サービスが停止するときにメイン関数を正常に終了するには、&quot;stopping&quot; 変数を定期的に確認する必要があります。サービスが停止していることを検出すると、作業をクリーンアップし、&quot;stoppedEvent&quot; イベント オブジェクトを通知します。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre class="vb" id="codePreview">Private Sub ServiceWorkerThread(ByVal state As Object)
    ' Periodically check if the service is stopping.
    Do While Not Me.stopping
        ' Perform main service function here...


        Thread.Sleep(2000)  ' Simulate some lengthy operations.
    Loop


    ' Signal the stopped event.
    Me.stoppedEvent.Set()
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h3>B. サービスへのインストーラーの追加</h3>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/ja-jp/library/aa984263.aspx">http://msdn.microsoft.com/ja-jp/library/aa984263.aspx</a></p>
<p class="MsoNormal">手順 1: デザイナーで SampleService.vb を開き、デザイナーの背景をクリックし、内容ではなくサービス自体を選択します。<span class="GramE">目的のデザイナーを右クリックして、[インストーラーの追加] をクリックします。</span>既定では、2 つのインストーラーを持つコンポーネントのクラスがプロジェクトに追加されます。コンポーネントには &quot;ProjectInstaller&quot; という名前が付けられ、このコンポーネントが含まれるインストーラーは、サービス用とサービスの関連プロセス用です。</p>
<p class="MsoNormal">手順 2: ProjectInstaller のデザイン ビューで ServiceInstaller1 をクリックします。プロパティ ウィンドウで、ServiceName プロパティを VBWindowsService に設定します。<span class="SpellE">DisplayName</span> プロパティを
<span class="SpellE">VBWindowsService</span> サンプル サービスに設定します。デザイナーで ServiceProcessInstaller1 をクリックします。Account プロパティを LocalService に設定します。これでサービスがインストールされ、ローカル サービス</p>
<p class="MsoNormal"><span class="GramE">アカウント</span>で実行されます。</p>
<p class="MsoNormal"><strong>セキュリティ上の注意</strong>: このサンプル コードでは、サービスは、LocalSystem ではなく、<span class="SpellE">LocalService</span> として実行するように構成されています。LocalSystem アカウントは、広範なアクセス許可を持っています。LocalSystem アカウントを使用する場合は、悪意のあるソフトウェアから攻撃される危険性が高まる可能性があるため、注意してください。広範なアクセス許可を必要としないタスクの場合は、LocalService
 アカウントを使用することを検討してください。このアカウントは、ローカル コンピューター上の権限を持たないユーザーとして動作し、リモート サーバーに匿名の資&#26684;情報を提示します。</p>
<h3>C. サービスで展開を簡単にするためのセットアップ プロジェクトの作成</h3>
<p class="MsoNormal">手順 1: <span class="SpellE">VBWindowsServiceSetup</span> という名前の他のプロジェクト タイプ/セットアップと展開プロジェクト/セットアップ プロジェクトを新たに追加します。</p>
<p class="MsoNormal">手順 2: セットアップ プロジェクトに VBWindowsService.exe を追加するには、<span class="SpellE">VBWindowsServiceSetup</span> を右クリックして、[追加] をポイントし、[プロジェクト出力] をクリックします。[プロジェクト] ボックスで
<span class="SpellE">VBWindowsService</span> を選び、リストの [プロジェクト出力] をクリックします。VBWindowsService のプライマリ出力のプロジェクト項目がセットアップ プロジェクトに追加されます。</p>
<p class="MsoNormal">手順 3: 既定では、セットアップ プロジェクトのターゲット プラットフォームは x86 です。x64 プラットフォームをターゲットにするセットアップをビルドする場合、セットアップ プロジェクトをクリックし、[プロパティ] ダイアログで、&quot;x64&quot; を TargetPlatform として選択します。</p>
<p class="MsoNormal">手順 4: 次に、VBWindowsService.exe ファイルをインストールするカスタム動作を追加します。ソリューション エクスプローラーでセットアップ プロジェクトを右クリックし、[ビュー] をポイントし、[カスタム動作] を<span class="GramE">クリック</span>します。カスタム動作エディターでカスタム 動作ノードを右クリックし、[カスタム動作の追加] をクリックします。一覧で [アプリケーション] フォルダーをダブルクリックして開き、<span class="SpellE">VBWindowsService</span>
 (アクティブ) で [プライマリ出力] を選択し、[OK] をクリックします。インストール、確定、ロールバック、アンインストールの 4 つのカスタム動作ノードすべてにプライマリ出力が追加されます。ソリューション エクスプローラーで、<span class="SpellE">VBWindowsServiceSetup</span> プロジェクトを右クリックし、[ビルド] をクリックします。<span>
</span></p>
<h2>詳細情報</h2>
<p class="MsoListParagraph"><span style="font-family:Symbol"><span><span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/ja-jp/library/y817hyb6.aspx">MSDN: Windows サービス アプリケーション</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
