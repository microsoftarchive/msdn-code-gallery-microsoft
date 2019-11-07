# How to download or upload multiple files in Windows Azure blob storage (VS2013)
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
- Windows Azure Storage
## Topics
- Azure
## Updated
- 09/22/2016
## Description

<p><img id="154166" src="154166-8171.onecodesampletopbanner.png" alt=""></p>
<p><strong>如何在 Windows Azure Blob 儲存體中下載</strong><strong>/</strong><strong>上載多個檔案</strong><strong>&nbsp;</strong></p>
<h2>簡介</h2>
<p>對於有大量非結構化資料要儲存在雲端的使用者，Blob 儲存體提供具成本效益且可擴充的解決方案，以便使用者儲存文件、社交資料、影像和文字等。&nbsp;這個範例示範如何在 Windows Azure Blob 儲存體中下載/上傳多個檔案。使用者可以選擇將多個不同種類的檔案上傳至 Blob 儲存體，或從 Blob 儲存體下載多個不同種類的檔案。</p>
<h2>執行範例</h2>
<p>&nbsp;您應該在執行程式碼範例前先執行下列步驟。</p>
<p>&nbsp;步驟 1：建立儲存體帳戶</p>
<p>&nbsp; &nbsp;&nbsp;1. 移至 <a href="https://manage.windowsazure.com/">Windows Azure 管理入口網站</a>並登入。</p>
<p>&nbsp;&nbsp;&nbsp;2. 按一下 &ldquo;New&rdquo; -&gt; &ldquo;data service&rdquo; -&gt; &ldquo;storage&rdquo;-&gt; &ldquo;quick create&rdquo;。</p>
<h1><img id="154177" src="154177-23324234324.png" alt=""></h1>
<p>3. 按一下 &ldquo;Manage Access Keys&rdquo;，並取得儲存體帳戶名稱和主要存取金鑰<strong>&nbsp;</strong><em>&nbsp;</em>Introduction</p>
<p><img id="154178" src="154178-23424324324.png" alt=""></p>
<p>&nbsp;&nbsp;&nbsp;開啟 <a href="http://home.aspx.cs/">Home.aspx.cs</a> 檔案和 <a href="http://home.aspx./">
Home.aspx.</a>vb，以您取得的儲存體帳戶名稱取代此「儲存體帳戶」，以您取得的主要存取金鑰取代此「主要存取金鑰」。<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p>步驟 2：安裝封裝</p>
<p>&nbsp;&nbsp; 1. 按一下 Manage NuGet Packages 以安裝封裝。<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154179" width="578" height="627" src="154179-1231231.png" alt=""></p>
<p>2. 安裝 Windows Azure Storage。<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154180" src="154180-2342423424.png" alt=""></p>
<p>&nbsp;</p>
<p>步驟 3：&nbsp;在 Visual Studio 2013 執行專案</p>
<p>&nbsp;&nbsp; 1. 從 Blob 儲存體下載多個檔案。</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (1) 選取含有您要下載的 Blob 的容器。</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (2) 選取您要下載的 Blob，然後按一下 &ldquo;Download&rdquo;。<strong>&nbsp;</strong><em></em></p>
<p><img id="154181" width="469" height="366" src="154181-dsfdds.png" alt=""></p>
<p>&nbsp;(3) 將檔案下載到此解決方案的實體檔案路徑中的 &ldquo;下載&rdquo; 資料夾。</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp; &nbsp; 2. 將多個檔案上傳到 Blob 儲存體</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (1) 輸入您要上傳檔案的容器名稱。</p>
<p>&nbsp;</p>
<p>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;(2) 按一下 [瀏覽]，然後選取您要上傳的檔案並按一下 [上傳]。</p>
<p>&nbsp;</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;(3) 您可以檢查您所上傳的檔案</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 選取 &ldquo;multcontainer&rdquo; 以檢查您所上傳的檔案</p>
<h2>使用程式碼</h2>
<p>&nbsp;</p>
<p>1. 撰寫程式碼來列出指定 CloudStorageAccount 的所有容器<strong></strong><em></em></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Private Sub GetContainerListByStorageAccount(storageAcount As CloudStorageAccount)

        '清除所有項目

        cbxl_container.Items.Clear()

        '取得 ViewState 的容器

        Dim lstSelectContainer As New List(Of String)()

        If ViewState(&quot;selectContainer&quot;) IsNot Nothing Then

            lstSelectContainer = CType(ViewState(&quot;selectContainer&quot;), List(Of String))

        End If

        '列出所有容器並新增至 CheckBoxList   

        Dim blobClient As CloudBlobClient = storageAcount.CreateCloudBlobClient()

        For Each container As CloudBlobContainer In blobClient.ListContainers()

            Dim item As New ListItem()

            item.Value = container.Uri.ToString()

            item.Text = container.Name

            If lstSelectContainer.Contains(container.Name) Then

                item.Selected = True

            End If

            cbxl_container.Items.Add(item)

        Next container

    End Sub</pre>
<pre class="hidden">private void GetContainerListByStorageAccount(CloudStorageAccount storageAcount)

      {

          //清除所有項目

          cbxl_container.Items.Clear();

          //取得 ViewState 的容器

          List&lt;string&gt; lstSelectContainer = new List&lt;string&gt;();

          if (ViewState[&quot;selectContainer&quot;] != null)

          {

              lstSelectContainer = (List&lt;string&gt;)ViewState[&quot;selectContainer&quot;];

          }

          //列出所有容器並新增至 CheckBoxList   

          CloudBlobClient blobClient = storageAcount.CreateCloudBlobClient();

          foreach (var container in blobClient.ListContainers())

          {

              ListItem item = new ListItem();

              item.Value = container.Uri.ToString();

              item.Text = container.Name;

              if (lstSelectContainer.Contains(container.Name))

              {

                  item.Selected = true;

              }

              cbxl_container.Items.Add(item);

          }

      }</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;GetContainerListByStorageAccount(storageAcount&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudStorageAccount)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'清除所有項目</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cbxl_container.Items.Clear()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'取得&nbsp;ViewState&nbsp;的容器</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;lstSelectContainer&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;<span class="visualBasic__keyword">String</span>)()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;ViewState(<span class="visualBasic__string">&quot;selectContainer&quot;</span>)&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lstSelectContainer&nbsp;=&nbsp;<span class="visualBasic__keyword">CType</span>(ViewState(<span class="visualBasic__string">&quot;selectContainer&quot;</span>),&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;<span class="visualBasic__keyword">String</span>))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'列出所有容器並新增至&nbsp;CheckBoxList&nbsp;&nbsp;&nbsp;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;blobClient&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudBlobClient&nbsp;=&nbsp;storageAcount.CreateCloudBlobClient()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;container&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudBlobContainer&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;blobClient.ListContainers()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;item&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ListItem()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Value&nbsp;=&nbsp;container.Uri.ToString()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Text&nbsp;=&nbsp;container.Name&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;lstSelectContainer.Contains(container.Name)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Selected&nbsp;=&nbsp;<span class="visualBasic__keyword">True</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cbxl_container.Items.Add(item)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;container&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;2. 撰寫程式碼來儲存已選取的容器名稱<strong></strong><em></em></div>
<p></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Protected Sub cbxl_container_SelectedIndexChanged(sender As Object, e As EventArgs)

      '清除先前新增的資料

      lstContainer.Clear()

      dicBBlob.Clear()

      dicPBlob.Clear()

      '取得 ViewState 的容器

      Dim lstSelectContainer As New List(Of String)()

      If ViewState(&quot;selectContainer&quot;) IsNot Nothing Then

          lstSelectContainer = CType(ViewState(&quot;selectContainer&quot;), List(Of String))

      End If

      For Each item As ListItem In cbxl_container.Items

          If item.Selected Then

              Dim strContainer As String = item.Text

              If Not String.IsNullOrEmpty(strContainer) Then

                  If Not lstContainer.Contains(strContainer) Then

                      lstContainer.Add(strContainer)

                  End If

              End If

          End If

      Next item

      '清除 Blob

      ClearBlobList(lstSelectContainer)

      For Each key As String In lstContainer

          GetBlobListByContainer(key, Csa_storageAccount)

      Next key

    

      ' 儲存已選取的容器名稱  

      If ViewState(&quot;selectContainer&quot;) IsNot Nothing Then

          ViewState(&quot;selectContainer&quot;) = lstContainer

      Else

          ViewState.Add(&quot;selectContainer&quot;, lstContainer)

      End If

  End Sub</pre>
<pre class="hidden">protected void cbxl_container_SelectedIndexChanged(object sender, EventArgs e)

       {

           //清除先前新增的資料

           lstContainer.Clear();

           dicBBlob.Clear();

           dicPBlob.Clear();

          //取得 ViewState 的容器

           List&lt;string&gt; lstSelectContainer = new List&lt;string&gt;();

           if (ViewState[&quot;selectContainer&quot;] != null)

           {

               lstSelectContainer = (List&lt;string&gt;)ViewState[&quot;selectContainer&quot;];

           }

           foreach (ListItem item in cbxl_container.Items)

           {

               if (item.Selected)

               {

                   string strContainer = item.Text;

                   if (!string.IsNullOrEmpty(strContainer))

                   {

                       if (!lstContainer.Contains(strContainer))

                       {

                           lstContainer.Add(strContainer);

                       }

                   }

               }

           }

           //清除 Blob

           ClearBlobList(lstSelectContainer);

           foreach (string key in lstContainer)

           {

               GetBlobListByContainer(key, Csa_storageAccount);

           }

          

           // 儲存已選取的容器名稱  

           if (ViewState[&quot;selectContainer&quot;] != null)

           {

               ViewState[&quot;selectContainer&quot;] = lstContainer;

           }

           else

           {

               ViewState.Add(&quot;selectContainer&quot;, lstContainer);

           }

       }</pre>
<div class="preview">
<pre class="js">Protected&nbsp;Sub&nbsp;cbxl_container_SelectedIndexChanged(sender&nbsp;As&nbsp;<span class="js__object">Object</span>,&nbsp;e&nbsp;As&nbsp;EventArgs)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'清除先前新增的資料&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lstContainer.Clear()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dicBBlob.Clear()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dicPBlob.Clear()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'取得&nbsp;ViewState&nbsp;的容器&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;lstSelectContainer&nbsp;As&nbsp;New&nbsp;List(Of&nbsp;<span class="js__object">String</span>)()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;ViewState(<span class="js__string">&quot;selectContainer&quot;</span>)&nbsp;IsNot&nbsp;Nothing&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lstSelectContainer&nbsp;=&nbsp;CType(ViewState(<span class="js__string">&quot;selectContainer&quot;</span>),&nbsp;List(Of&nbsp;<span class="js__object">String</span>))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For&nbsp;Each&nbsp;item&nbsp;As&nbsp;ListItem&nbsp;In&nbsp;cbxl_container.Items&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;item.Selected&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;strContainer&nbsp;As&nbsp;<span class="js__object">String</span>&nbsp;=&nbsp;item.Text&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;Not&nbsp;<span class="js__object">String</span>.IsNullOrEmpty(strContainer)&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;Not&nbsp;lstContainer.Contains(strContainer)&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lstContainer.Add(strContainer)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Next&nbsp;item&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'清除&nbsp;Blob&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ClearBlobList(lstSelectContainer)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For&nbsp;Each&nbsp;key&nbsp;As&nbsp;<span class="js__object">String</span>&nbsp;In&nbsp;lstContainer&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GetBlobListByContainer(key,&nbsp;Csa_storageAccount)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Next&nbsp;key&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'&nbsp;儲存已選取的容器名稱&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;ViewState(<span class="js__string">&quot;selectContainer&quot;</span>)&nbsp;IsNot&nbsp;Nothing&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ViewState(<span class="js__string">&quot;selectContainer&quot;</span>)&nbsp;=&nbsp;lstContainer&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Else&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ViewState.Add(<span class="js__string">&quot;selectContainer&quot;</span>,&nbsp;lstContainer)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;End&nbsp;Sub</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;3. 撰寫程式碼來顯示已選取的容器 Blob<strong></strong><em></em></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Private Sub GetBlobListByContainer(strContainerName As String, storageAcount As CloudStorageAccount)

     '將容器名稱新增為表&#26684;標題

     Dim celltitle As New TableCell()

     celltitle.Text = strContainerName

     tbl_blobList.Rows(0).Cells.Add(celltitle)

     Dim blobClient As CloudBlobClient = storageAcount.CreateCloudBlobClient()

     Dim blobContainer As CloudBlobContainer = blobClient.GetContainerReference(strContainerName)

     If blobContainer.Exists() Then

         Dim cell As New TableCell()

         Dim chkl_blobList As New CheckBoxList()

         chkl_blobList.ID = strContainerName

         chkl_blobList.AutoPostBack = False

         chkl_blobList.EnableViewState = True

         '列出所有 Blob 並新增至 CheckBoxList

         For Each Blob As IListBlobItem In blobContainer.ListBlobs(Nothing, True)

             Dim item As New ListItem()

             Dim strUrl As String = Blob.Uri.ToString()

             Dim strUrlArr() As String = strUrl.Split(&quot;/&quot;c)

             If strUrlArr.Length &gt; 0 Then

                 Dim intIndex As Integer = 0

                 For i As Integer = 0 To strUrlArr.Length - 1

                     If strUrlArr(i) = strContainerName Then

                         intIndex = i

                         Exit For

                     End If

                 Next i

                 '以 ContainerName&#43;BlobName &#26684;式產生新名稱

                 Dim strBlobName As String = &quot;&quot;

                 For i As Integer = intIndex &#43; 1 To strUrlArr.Length - 1

                     strBlobName &amp;= strUrlArr(i) &amp; &quot;/&quot;

                 Next i

                 If Not String.IsNullOrEmpty(strBlobName) Then

                     strBlobName = strBlobName.Substring(0, strBlobName.Length - 1)

                     item.Text = strBlobName

                     item.Value = Blob.Uri.ToString()

                     chkl_blobList.Items.Add(item)

                     If TypeOf Blob Is CloudBlockBlob AndAlso (Not dicBBlob.ContainsKey(strBlobName)) Then

                         dicBBlob.Add(strBlobName, CType(Blob, CloudBlockBlob))

                     ElseIf TypeOf Blob Is CloudPageBlob AndAlso (Not dicPBlob.ContainsKey(strBlobName)) Then

                         dicPBlob.Add(strBlobName, CType(Blob, CloudPageBlob))

                     End If

                 End If

             End If

         Next Blob

         '將 CheckBoxList 新增至表&#26684;

         cell.Controls.Add(chkl_blobList)

         tbl_blobList.Rows(1).Cells.Add(cell)

     End If

 End Sub</pre>
<pre class="hidden">private void GetBlobListByContainer(string strContainerName, CloudStorageAccount storageAcount)

       {

           //將容器名稱新增為表&#26684;標題

           TableCell celltitle = new TableCell();

           celltitle.Text = strContainerName;

           tbl_blobList.Rows[0].Cells.Add(celltitle);

           CloudBlobClient blobClient = storageAcount.CreateCloudBlobClient();

           CloudBlobContainer blobContainer = blobClient.GetContainerReference(strContainerName);

           if (blobContainer.Exists())

           {

               TableCell cell = new TableCell();

               CheckBoxList chkl_blobList = new CheckBoxList();

               chkl_blobList.ID = strContainerName;

               chkl_blobList.AutoPostBack = false;

               chkl_blobList.EnableViewState = true;

               //列出所有 Blob 並新增至 CheckBoxList

               foreach (var blob in blobContainer.ListBlobs(null, true))

               {

                   ListItem item = new ListItem();

                   string strUrl = blob.Uri.ToString();

                   string[] strUrlArr = strUrl.Split('/');

                   if (strUrlArr.Length &gt; 0)

                   {

                       int intIndex = 0;

                       for (int i = 0; i &lt; strUrlArr.Length; i&#43;&#43;)

                       {

                           if (strUrlArr[i] == strContainerName)

                           {

                               intIndex = i;

                               break;

                           }

                       }

                       //以 ContainerName&#43;BlobName &#26684;式產生新名稱

                       string strBlobName = &quot;&quot;;

                       for (int i = intIndex &#43; 1; i &lt; strUrlArr.Length; i&#43;&#43;)

                       {

                           strBlobName &#43;= strUrlArr[i] &#43; &quot;/&quot;;

                       }

                       if (!string.IsNullOrEmpty(strBlobName))

                       {

                           strBlobName = strBlobName.Substring(0, strBlobName.Length - 1);

                           item.Text = strBlobName;

                           item.Value = blob.Uri.ToString();                         

                           chkl_blobList.Items.Add(item);

                           if (blob is CloudBlockBlob &amp;&amp; !dicBBlob.ContainsKey(strBlobName))

                           {

                               dicBBlob.Add(strBlobName, (CloudBlockBlob)blob);

                           }

                           else if (blob is CloudPageBlob &amp;&amp; !dicPBlob.ContainsKey(strBlobName))

                           {

                               dicPBlob.Add(strBlobName, (CloudPageBlob)blob);

                           }

                       }

                   }

               }

               //將 CheckBoxList 新增至表&#26684;

               cell.Controls.Add(chkl_blobList);

               tbl_blobList.Rows[1].Cells.Add(cell);

           }

       }</pre>
<div class="preview">
<pre class="js">Private&nbsp;Sub&nbsp;GetBlobListByContainer(strContainerName&nbsp;As&nbsp;<span class="js__object">String</span>,&nbsp;storageAcount&nbsp;As&nbsp;CloudStorageAccount)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'將容器名稱新增為表&#26684;標題&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;celltitle&nbsp;As&nbsp;New&nbsp;TableCell()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;celltitle.Text&nbsp;=&nbsp;strContainerName&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbl_blobList.Rows(<span class="js__num">0</span>).Cells.Add(celltitle)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;blobClient&nbsp;As&nbsp;CloudBlobClient&nbsp;=&nbsp;storageAcount.CreateCloudBlobClient()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;blobContainer&nbsp;As&nbsp;CloudBlobContainer&nbsp;=&nbsp;blobClient.GetContainerReference(strContainerName)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;blobContainer.Exists()&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;cell&nbsp;As&nbsp;New&nbsp;TableCell()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;chkl_blobList&nbsp;As&nbsp;New&nbsp;CheckBoxList()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chkl_blobList.ID&nbsp;=&nbsp;strContainerName&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chkl_blobList.AutoPostBack&nbsp;=&nbsp;False&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chkl_blobList.EnableViewState&nbsp;=&nbsp;True&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'列出所有&nbsp;Blob&nbsp;並新增至&nbsp;CheckBoxList&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For&nbsp;Each&nbsp;Blob&nbsp;As&nbsp;IListBlobItem&nbsp;In&nbsp;blobContainer.ListBlobs(Nothing,&nbsp;True)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;item&nbsp;As&nbsp;New&nbsp;ListItem()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;strUrl&nbsp;As&nbsp;<span class="js__object">String</span>&nbsp;=&nbsp;Blob.Uri.ToString()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;strUrlArr()&nbsp;As&nbsp;<span class="js__object">String</span>&nbsp;=&nbsp;strUrl.Split(<span class="js__string">&quot;/&quot;</span>c)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;strUrlArr.Length&nbsp;&gt;&nbsp;<span class="js__num">0</span>&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;intIndex&nbsp;As&nbsp;Integer&nbsp;=&nbsp;<span class="js__num">0</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For&nbsp;i&nbsp;As&nbsp;Integer&nbsp;=&nbsp;<span class="js__num">0</span>&nbsp;To&nbsp;strUrlArr.Length&nbsp;-&nbsp;<span class="js__num">1</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;strUrlArr(i)&nbsp;=&nbsp;strContainerName&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;intIndex&nbsp;=&nbsp;i&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Exit&nbsp;For&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Next&nbsp;i&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'以&nbsp;ContainerName&#43;BlobName&nbsp;&#26684;式產生新名稱&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dim&nbsp;strBlobName&nbsp;As&nbsp;<span class="js__object">String</span>&nbsp;=&nbsp;<span class="js__string">&quot;&quot;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For&nbsp;i&nbsp;As&nbsp;Integer&nbsp;=&nbsp;intIndex&nbsp;&#43;&nbsp;<span class="js__num">1</span>&nbsp;To&nbsp;strUrlArr.Length&nbsp;-&nbsp;<span class="js__num">1</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strBlobName&nbsp;&amp;=&nbsp;strUrlArr(i)&nbsp;&amp;&nbsp;<span class="js__string">&quot;/&quot;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Next&nbsp;i&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;Not&nbsp;<span class="js__object">String</span>.IsNullOrEmpty(strBlobName)&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strBlobName&nbsp;=&nbsp;strBlobName.Substring(<span class="js__num">0</span>,&nbsp;strBlobName.Length&nbsp;-&nbsp;<span class="js__num">1</span>)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Text&nbsp;=&nbsp;strBlobName&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Value&nbsp;=&nbsp;Blob.Uri.ToString()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chkl_blobList.Items.Add(item)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;If&nbsp;TypeOf&nbsp;Blob&nbsp;Is&nbsp;CloudBlockBlob&nbsp;AndAlso&nbsp;(Not&nbsp;dicBBlob.ContainsKey(strBlobName))&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dicBBlob.Add(strBlobName,&nbsp;CType(Blob,&nbsp;CloudBlockBlob))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ElseIf&nbsp;TypeOf&nbsp;Blob&nbsp;Is&nbsp;CloudPageBlob&nbsp;AndAlso&nbsp;(Not&nbsp;dicPBlob.ContainsKey(strBlobName))&nbsp;Then&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dicPBlob.Add(strBlobName,&nbsp;CType(Blob,&nbsp;CloudPageBlob))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Next&nbsp;Blob&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'將&nbsp;CheckBoxList&nbsp;新增至表&#26684;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cell.Controls.Add(chkl_blobList)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbl_blobList.Rows(<span class="js__num">1</span>).Cells.Add(cell)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End&nbsp;If&nbsp;
&nbsp;
&nbsp;End&nbsp;Sub</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
4. 撰寫程式碼以根據指定的 Blob 名稱取得 Blob 並下載到本機磁碟<strong></strong><em></em>
<p></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Protected Sub btn_Downlaod_Click(sender As Object, e As EventArgs)
       If ViewState(&quot;selectContainer&quot;) IsNot Nothing Then
           Dim lst As List(Of String) = CType(ViewState(&quot;selectContainer&quot;), List(Of String))
           '取得已選取的 Blob 
           GetSelectedBlob(lst)
           If dicSelectedBlob.Count &gt; 0 Then
               For Each strContainer As String In lst
                   If dicSelectedBlob.ContainsKey(strContainer) Then
                       Dim lstBlob As New List(Of String)()
                       lstBlob = dicSelectedBlob(strContainer)
                       For Each KeyName As String In lstBlob
                           DownLoadBlobByBlobName(KeyName, strContainer)
                       Next KeyName
                   End If
               Next strContainer
               Response.Write(&quot;&lt;script&gt;alert('Files Successfully downloaded！');&lt;/script&gt;&quot;)
           Else
               Response.Write(&quot;&lt;script&gt;alert('Select the blobs you want to download！');&lt;/script&gt;&quot;)
           End If
       Else
           Response.Write(&quot;&lt;script&gt;alert('Select the container which contains the blobs you want to download！');&lt;/script&gt;&quot;)
       End If
   End Sub
Private Sub DownLoadBlobByBlobName(strBlobName As String, strContainer As String)
       Dim filePath As String = Server.MapPath(&quot;DownLoad/&quot;)
       If Directory.Exists(filePath) = False Then
           Directory.CreateDirectory(filePath)
       End If
       '產生新名稱
       Dim strName() As String = strBlobName.Split(&quot;/&quot;c)
       Dim strNewName As String = &quot;&quot;
       If strName.Length &gt; 0 Then
           For i As Integer = 0 To strName.Length - 1
               strNewName &amp;= strName(i) &amp; &quot;_&quot;
           Next i
       End If
       strNewName = strNewName.Substring(0, strNewName.Length - 1)
       strNewName = strContainer &amp; &quot;_&quot; &amp; strNewName
       Try
           '下載 Blob
           If dicBBlob.ContainsKey(strBlobName) Then
               Dim bblob As CloudBlockBlob = dicBBlob(strBlobName)
               bblob.DownloadToFile(filePath &amp; strNewName, FileMode.Create)
           End If
           If dicPBlob.ContainsKey(strBlobName) Then
               Dim bblob As CloudPageBlob = dicPBlob(strBlobName)
               bblob.DownloadToFile(filePath &amp; strNewName, FileMode.Create)
           End If
       Catch
       End Try
   End Sub
</pre>
<pre class="hidden">protected void btn_Downlaod_Click(object sender, EventArgs e)
       {
           if (ViewState[&quot;selectContainer&quot;] != null)
           {
               List&lt;string&gt; lst = (List&lt;string&gt;)ViewState[&quot;selectContainer&quot;];
               //取得已選取的 Blob 
               GetSelectedBlob(lst);
               if(dicSelectedBlob.Count&gt;0)
               {
                   foreach (string strContainer in lst)
                   {
                       if (dicSelectedBlob.ContainsKey(strContainer))
                       {
                           List&lt;string&gt; lstBlob = new List&lt;string&gt;();
                           lstBlob = dicSelectedBlob[strContainer];
                           foreach (string KeyName in lstBlob)
                           {
                               DownLoadBlobByBlobName(KeyName, strContainer);
                           }
                       }
                   }
                   Response.Write(&quot;&lt;script&gt;alert('Files Successfully downloaded！');&lt;/script&gt;&quot;);
               }
               else
               {
                   Response.Write(&quot;&lt;script&gt;alert('Select the blobs you want to download！');&lt;/script&gt;&quot;);
               }                         
           }
           else
           {
               Response.Write(&quot;&lt;script&gt;alert('Select the container which contains the blobs you want to download！');&lt;/script&gt;&quot;);              
           }
       }
private void DownLoadBlobByBlobName(string strBlobName,string strContainer)
       {
           string filePath = Server.MapPath(&quot;DownLoad/&quot;);
           if (Directory.Exists(filePath) == false) 
           {
               Directory.CreateDirectory(filePath);
           }    
    
           //產生新名稱
           string[] strName = strBlobName.Split('/');
           string strNewName = &quot;&quot;;
           if (strName.Length &gt; 0)
           {
               for (int i = 0; i &lt; strName.Length; i&#43;&#43;)
               {
                   strNewName &#43;= strName[i]&#43;&quot;_&quot;;
               }
           }
           strNewName = strNewName.Substring(0, strNewName.Length - 1);
           strNewName = strContainer &#43; &quot;_&quot; &#43; strNewName;
           try
           {
               //下載 Blob
               if (dicBBlob.ContainsKey(strBlobName))
               {
                   CloudBlockBlob bblob = dicBBlob[strBlobName];
                   bblob.DownloadToFile(filePath &#43; strNewName, FileMode.Create);
               }
               if (dicPBlob.ContainsKey(strBlobName))
               {
                   CloudPageBlob bblob = dicPBlob[strBlobName];
                   bblob.DownloadToFile(filePath &#43; strNewName, FileMode.Create);
               }
           }
           catch
           {
           }
       }
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Protected</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;btn_Downlaod_Click(sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>,&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;EventArgs)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;ViewState(<span class="visualBasic__string">&quot;selectContainer&quot;</span>)&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;lst&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;=&nbsp;<span class="visualBasic__keyword">CType</span>(ViewState(<span class="visualBasic__string">&quot;selectContainer&quot;</span>),&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;<span class="visualBasic__keyword">String</span>))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'取得已選取的&nbsp;Blob&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GetSelectedBlob(lst)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;dicSelectedBlob.Count&nbsp;&gt;&nbsp;<span class="visualBasic__number">0</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;strContainer&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;lst&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;dicSelectedBlob.ContainsKey(strContainer)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;lstBlob&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;<span class="visualBasic__keyword">String</span>)()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lstBlob&nbsp;=&nbsp;dicSelectedBlob(strContainer)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;KeyName&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;lstBlob&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DownLoadBlobByBlobName(KeyName,&nbsp;strContainer)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;KeyName&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;strContainer&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Write(<span class="visualBasic__string">&quot;&lt;script&gt;alert('Files&nbsp;Successfully&nbsp;downloaded！');&lt;/script&gt;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Write(<span class="visualBasic__string">&quot;&lt;script&gt;alert('Select&nbsp;the&nbsp;blobs&nbsp;you&nbsp;want&nbsp;to&nbsp;download！');&lt;/script&gt;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Write(<span class="visualBasic__string">&quot;&lt;script&gt;alert('Select&nbsp;the&nbsp;container&nbsp;which&nbsp;contains&nbsp;the&nbsp;blobs&nbsp;you&nbsp;want&nbsp;to&nbsp;download！');&lt;/script&gt;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;DownLoadBlobByBlobName(strBlobName&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>,&nbsp;strContainer&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;filePath&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;Server.MapPath(<span class="visualBasic__string">&quot;DownLoad/&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;Directory.Exists(filePath)&nbsp;=&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Directory.CreateDirectory(filePath)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'產生新名稱</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;strName()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;strBlobName.Split(<span class="visualBasic__string">&quot;/&quot;</span>c)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;strNewName&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;<span class="visualBasic__string">&quot;&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;strName.Length&nbsp;&gt;&nbsp;<span class="visualBasic__number">0</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;=&nbsp;<span class="visualBasic__number">0</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;strName.Length&nbsp;-&nbsp;<span class="visualBasic__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strNewName&nbsp;&amp;=&nbsp;strName(i)&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;_&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strNewName&nbsp;=&nbsp;strNewName.Substring(<span class="visualBasic__number">0</span>,&nbsp;strNewName.Length&nbsp;-&nbsp;<span class="visualBasic__number">1</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strNewName&nbsp;=&nbsp;strContainer&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;_&quot;</span>&nbsp;&amp;&nbsp;strNewName&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'下載&nbsp;Blob</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;dicBBlob.ContainsKey(strBlobName)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;bblob&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudBlockBlob&nbsp;=&nbsp;dicBBlob(strBlobName)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;bblob.DownloadToFile(filePath&nbsp;&amp;&nbsp;strNewName,&nbsp;FileMode.Create)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;dicPBlob.ContainsKey(strBlobName)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;bblob&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudPageBlob&nbsp;=&nbsp;dicPBlob(strBlobName)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;bblob.DownloadToFile(filePath&nbsp;&amp;&nbsp;strNewName,&nbsp;FileMode.Create)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
5. 撰寫程式碼以將檔案上傳到 Blob 儲存體<strong></strong><em></em>
<p></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Protected Sub btn_Upload_Click(sender As Object, e As EventArgs)

        '產生容器名稱

        Dim strContainerName As String = txt_container.Text

        Try

            Dim blobClient As CloudBlobClient = Csa_storageAccount.CreateCloudBlobClient()

            Dim blobContainer As CloudBlobContainer = blobClient.GetContainerReference(strContainerName)

            blobContainer.CreateIfNotExists()

            blobContainer.SetPermissions(New BlobContainerPermissions With {.PublicAccess = BlobContainerPublicAccessType.Blob})

            '上傳檔案

            Dim httpFiles As HttpFileCollection = Request.Files

            If httpFiles IsNot Nothing Then

                For i As Integer = 0 To httpFiles.Count - 1

                    Dim file As HttpPostedFile = httpFiles(i)

                    '產生 blobName

                    Dim blockName As String = file.FileName

                    Dim strName() As String = file.FileName.Split(&quot;\&quot;c)

                    If strName.Length &gt; 0 Then

                        blockName = strName(strName.Length - 1)

                    End If

                    If Not String.IsNullOrEmpty(blockName) Then

                        Dim blob As CloudBlockBlob = blobContainer.GetBlockBlobReference(blockName)

                        '上傳檔案

                        blob.UploadFromStream(file.InputStream)

                    End If

                Next i

            End If

        Catch

        End Try

        '重新讀取容器和 Blob 

        GetContainerListByStorageAccount(Csa_storageAccount)

        cbxl_container_SelectedIndexChanged(cbxl_container, Nothing)

    End Sub</pre>
<pre class="hidden">protected void btn_Upload_Click(object sender, EventArgs e)
       {
           //產生容器名稱
           string strContainerName = txt_container.Text;
           try
           {
               CloudBlobClient blobClient = Csa_storageAccount.CreateCloudBlobClient();
               CloudBlobContainer blobContainer = blobClient.GetContainerReference(strContainerName);
               blobContainer.CreateIfNotExists();
               blobContainer.SetPermissions(new BlobContainerPermissions
               {
                   PublicAccess = BlobContainerPublicAccessType.Blob
               });
               //上傳檔案
               HttpFileCollection httpFiles = Request.Files;
               if (httpFiles != null)
               {
                   for (int i = 0; i &lt; httpFiles.Count; i&#43;&#43;)
                   {
                       HttpPostedFile file = httpFiles[i];
                       //產生 blobName
                       string blockName = file.FileName;
                       string[] strName = file.FileName.Split('\\');
                       if (strName.Length &gt; 0)
                       {
                           blockName = strName[strName.Length - 1];
                       }
                       if (!string.IsNullOrEmpty(blockName))
                       {
                           CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blockName);
                           //上傳檔案
                           blob.UploadFromStream(file.InputStream);
                       }
                   }
               }
           }
           catch
           {
           }
           //重新讀取容器和 Blob  
           GetContainerListByStorageAccount(Csa_storageAccount);
           cbxl_container_SelectedIndexChanged(cbxl_container, null);
       }
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Protected</span><span class="visualBasic__keyword">Sub</span>&nbsp;btn_Upload_Click(sender&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>,&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;EventArgs)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'產生容器名稱</span><span class="visualBasic__keyword">Dim</span>&nbsp;strContainerName&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;txt_container.Text&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span><span class="visualBasic__keyword">Dim</span>&nbsp;blobClient&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudBlobClient&nbsp;=&nbsp;Csa_storageAccount.CreateCloudBlobClient()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;blobContainer&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudBlobContainer&nbsp;=&nbsp;blobClient.GetContainerReference(strContainerName)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;blobContainer.CreateIfNotExists()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;blobContainer.SetPermissions(<span class="visualBasic__keyword">New</span>&nbsp;BlobContainerPermissions&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;{.PublicAccess&nbsp;=&nbsp;BlobContainerPublicAccessType.Blob})&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'上傳檔案</span><span class="visualBasic__keyword">Dim</span>&nbsp;httpFiles&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;HttpFileCollection&nbsp;=&nbsp;Request.Files&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;httpFiles&nbsp;<span class="visualBasic__keyword">IsNot</span><span class="visualBasic__keyword">Nothing</span><span class="visualBasic__keyword">Then</span><span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Integer</span>&nbsp;=&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">To</span>&nbsp;httpFiles.Count&nbsp;-&nbsp;<span class="visualBasic__number">1</span><span class="visualBasic__keyword">Dim</span>&nbsp;file&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;HttpPostedFile&nbsp;=&nbsp;httpFiles(i)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'產生&nbsp;blobName</span><span class="visualBasic__keyword">Dim</span>&nbsp;blockName&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;file.FileName&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;strName()&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;file.FileName.Split(<span class="visualBasic__string">&quot;\&quot;</span>c)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;strName.Length&nbsp;&gt;&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;blockName&nbsp;=&nbsp;strName(strName.Length&nbsp;-&nbsp;<span class="visualBasic__number">1</span>)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span><span class="visualBasic__keyword">If</span><span class="visualBasic__keyword">Not</span><span class="visualBasic__keyword">String</span>.IsNullOrEmpty(blockName)&nbsp;<span class="visualBasic__keyword">Then</span><span class="visualBasic__keyword">Dim</span>&nbsp;blob&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CloudBlockBlob&nbsp;=&nbsp;blobContainer.GetBlockBlobReference(blockName)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'上傳檔案</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;blob.UploadFromStream(file.InputStream)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span><span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span><span class="visualBasic__keyword">Catch</span><span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">Try</span><span class="visualBasic__com">'重新讀取容器和&nbsp;Blob&nbsp;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GetContainerListByStorageAccount(Csa_storageAccount)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cbxl_container_SelectedIndexChanged(cbxl_container,&nbsp;<span class="visualBasic__keyword">Nothing</span>)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<p></p>
<h2>其他相關資訊</h2>
<p>&nbsp;</p>
<p>CloudStorageAccount 類別</p>
<p>&nbsp; <a href="http://msdn.microsoft.com/zh-tw/library/microsoft.windowsazure.cloudstorageaccount.aspx">
http://msdn.microsoft.com/zh-tw/library/microsoft.windowsazure.cloudstorageaccount.aspx</a></p>
<p>&nbsp;</p>
<p>CloudBlobClient 類別</p>
<p>&nbsp; <a href="http://msdn.microsoft.com/zh-tw/library/azure/microsoft.windowsazure.storage.blob.cloudblobclient.aspx">
http://msdn.microsoft.com/zh-tw/library/azure/microsoft.windowsazure.storage.blob.cloudblobclient.aspx</a></p>
<p>&nbsp;</p>
<p>CloudBlobContainer 類別</p>
<p>&nbsp;&nbsp; <a href="http://msdn.microsoft.com/zh-tw/library/azure/microsoft.windowsazure.storage.blob.cloudblobcontainer.aspx">
http://msdn.microsoft.com/zh-tw/library/azure/microsoft.windowsazure.storage.blob.cloudblobcontainer.aspx</a></p>
<p>&nbsp;</p>
<p>CloudBlob.DownloadToFile 方法 (字串)</p>
<p>&nbsp;&nbsp; <a href="http://msdn.microsoft.com/zh-tw/library/azure/ee772800(v=azure.95).aspx">
http://msdn.microsoft.com/zh-tw/library/azure/ee772800(v=azure.95).aspx</a></p>
<p>&nbsp;</p>
<p>CloudBlob.UploadFromStream 方法 (資料流)</p>
<p>&nbsp;&nbsp; <a href="http://msdn.microsoft.com/zh-tw/library/azure/ee772826(v=azure.95).aspx">
http://msdn.microsoft.com/zh-tw/library/azure/ee772826(v=azure.95).aspx</a><strong></strong><em></em></p>
<p>&nbsp;</p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><strong></strong><em></em></p>
<p><strong></strong><em></em></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><em><br>
</em></p>
