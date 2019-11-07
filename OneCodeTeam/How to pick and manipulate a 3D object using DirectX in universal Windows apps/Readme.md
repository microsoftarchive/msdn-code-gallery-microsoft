# How to pick and manipulate a 3D object using DirectX in universal Windows apps
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- DirectX
- Windows
- Direct3D
- Windows Phone
- Windows 8
- Windows Phone 8
- Windows Store app Development
- Windows 8.1
- Windows Phone 8.1
- Graphics and Gaming
- Windows Phone App Development
## Topics
- DirectX
- universal app
- rotate object
- picking
## Updated
- 09/21/2016
## Description

<h1><em><img id="154434" src="154434-8171.onecodesampletopbanner.png" alt="" width="696" height="58"></em></h1>
<h1><span><span>ユニバーサル アプリで DirectX を使用して 3D オブジェクトを選択および操作する方法</span></span></h1>
<h2><span><span>はじめに</span></span></h2>
<p><span><span>このサンプルでは、回転、拡大/縮小、および移動など、3D オブジェクトを選択して操作する方法を示します。</span><span>また、このコードは、</span><a name="_GoBack"></a><span>境界ボックスと境界錐台の間の交点テストも示します。</span></span></p>
<h2><span><span>サンプルの実行</span></span></h2>
<p><span><a name="OLE_LINK3"></a><span>Visual Studio 2013 でこのサンプルをビルドし、実行します。</span></span></p>
<p><span><span>[</span><span>Rotate</span><span>] ボタンをクリックすると、ポインターのドラッグと共にキューブが回転します。</span></span></p>
<p><span><span><img id="154435" src="154435-image.png" alt=""></span></span></p>
<p><span><span><span><span>[</span><span>Translate</span><span>] ボタンをクリックすると、ポインターのドラッグに応じてオブジェクトがある程度の距離だけ移動します。</span></span><strong>&nbsp;</strong><em>&nbsp;</em></span></span></p>
<p><span><span><img id="154436" src="154436-01b9bd70-1ba2-45a7-a305-7c0922e55015image.png" alt=""></span></span></p>
<p><span><span>[</span><span>Scale</span><span>&rdquo;</span><span>] ボタンをクリックすると、ポインターのドラッグに応じてキューブが拡大/縮小します</span><span>。</span></span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154437" src="154437-27392741-4f93-4fc8-ba58-778f703b2cd4image.png" alt="" width="1088" height="664"></p>
<p>&nbsp;</p>
<h2><span><span>コードの使用</span></span></h2>
<p><span>&bull;&nbsp; <span>まず、2D 画面の座標を</span><span>横向き</span><span>を基準にした空間に変換する必要があります。これは、Windows 8 タッチ デバイスには複数の方向があるためです。</span></span><strong>&nbsp;</strong><em></em></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>
<pre class="hidden">//現在の点の座標を原点の画面領域に変換する 
Point CubeRenderer::TransformToOrientation(Point point, bool dipsToPixels) 
{ 
Point returnValue; 
 
switch (m_orientation) 
{ 
case DisplayOrientations::Landscape: 
returnValue = point; 
break; 
case DisplayOrientations::Portrait: 
returnValue = Point(point.Y, m_windowBounds.Width - point.X); 
break; 
case DisplayOrientations::PortraitFlipped: 
returnValue = Point(m_windowBounds.Height - point.Y, point.X); 
break; 
case DisplayOrientations::LandscapeFlipped: 
returnValue = Point(m_windowBounds.Width - point.X, m_windowBounds.Height - point.Y); 
break; 
default: 
throw ref new Platform::FailureException(); 
break; 
} 
// DIP をピクセルに変換するかどうか 
return dipsToPixels ? Point(ConvertDipsToPixels(returnValue.X), 
ConvertDipsToPixels(returnValue.Y)) 
: returnValue; 
}
</pre>
<div class="preview">
<pre class="cplusplus"><span class="cpp__com">//現在の点の座標を原点の画面領域に変換する&nbsp;</span>&nbsp;
Point&nbsp;CubeRenderer::TransformToOrientation(Point&nbsp;point,&nbsp;<span class="cpp__datatype">bool</span>&nbsp;dipsToPixels)&nbsp;&nbsp;
{&nbsp;&nbsp;
Point&nbsp;returnValue;&nbsp;&nbsp;
&nbsp;&nbsp;
<span class="cpp__keyword">switch</span>&nbsp;(m_orientation)&nbsp;&nbsp;
{&nbsp;&nbsp;
<span class="cpp__keyword">case</span>&nbsp;DisplayOrientations::Landscape:&nbsp;&nbsp;
returnValue&nbsp;=&nbsp;point;&nbsp;&nbsp;
<span class="cpp__keyword">break</span>;&nbsp;&nbsp;
<span class="cpp__keyword">case</span>&nbsp;DisplayOrientations::Portrait:&nbsp;&nbsp;
returnValue&nbsp;=&nbsp;Point(point.Y,&nbsp;m_windowBounds.Width&nbsp;-&nbsp;point.X);&nbsp;&nbsp;
<span class="cpp__keyword">break</span>;&nbsp;&nbsp;
<span class="cpp__keyword">case</span>&nbsp;DisplayOrientations::PortraitFlipped:&nbsp;&nbsp;
returnValue&nbsp;=&nbsp;Point(m_windowBounds.Height&nbsp;-&nbsp;point.Y,&nbsp;point.X);&nbsp;&nbsp;
<span class="cpp__keyword">break</span>;&nbsp;&nbsp;
<span class="cpp__keyword">case</span>&nbsp;DisplayOrientations::LandscapeFlipped:&nbsp;&nbsp;
returnValue&nbsp;=&nbsp;Point(m_windowBounds.Width&nbsp;-&nbsp;point.X,&nbsp;m_windowBounds.Height&nbsp;-&nbsp;point.Y);&nbsp;&nbsp;
<span class="cpp__keyword">break</span>;&nbsp;&nbsp;
<span class="cpp__keyword">default</span>:&nbsp;&nbsp;
<span class="cpp__keyword">throw</span>&nbsp;ref&nbsp;<span class="cpp__keyword">new</span>&nbsp;Platform::FailureException();&nbsp;&nbsp;
<span class="cpp__keyword">break</span>;&nbsp;&nbsp;
}&nbsp;&nbsp;
<span class="cpp__com">//&nbsp;DIP&nbsp;をピクセルに変換するかどうか&nbsp;</span><span class="cpp__keyword">return</span>&nbsp;dipsToPixels&nbsp;?&nbsp;Point(ConvertDipsToPixels(returnValue.X),&nbsp;&nbsp;
ConvertDipsToPixels(returnValue.Y))&nbsp;&nbsp;
:&nbsp;returnValue;&nbsp;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span>&bull;&nbsp; <span>続いて、ポインターの 2D 領域の座標を表示領域に変換します。</span><span>以下に示す数学関連の資料を参照してください。</span></span></div>
<div class="endscriptcode"><span><span>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>
<pre class="hidden">void CubeRenderer::ScreenToView( 
_In_ float sx, 
_In_ float sy, 
_Outptr_ float * vx, 
_Outptr_ float * vy 
) 
{ 
*vx = (2.0f * sx / m_d3dRenderTargetSize.Width - 1.0f) / m_cbMVPData.projection._11; 
*vy = (-2.0f * sy / m_d3dRenderTargetSize.Height &#43; 1.0f) / m_cbMVPData.projection._22; 
} 
void CubeRenderer::VectorToLocal( 
_In_ XMVECTOR inVec, 
_Outptr_ XMVECTOR * outVec 
) 
{ 
XMMATRIX viewMx = XMLoadFloat4x4(&amp;m_cbMVPData.view); 
XMMATRIX modelMx = XMLoadFloat4x4(&amp;m_cbMVPData.model); 
XMMATRIX invView = XMMatrixInverse(&amp;XMMatrixDeterminant(viewMx), viewMx); 
XMMATRIX invModel = XMMatrixInverse(&amp;XMMatrixDeterminant(modelMx), modelMx); 
XMMATRIX toLocal = invView * invModel; 
XMFLOAT4 inVecF; 
XMStoreFloat4(&amp;inVecF, inVec); 
if(1.0f == inVecF.w)//point vector 
{ 
*outVec = XMVector3TransformCoord(inVec, toLocal); 
} 
else 
{ 
*outVec = XMVector3TransformNormal(inVec, toLocal); 
*outVec = XMVector3Normalize(*outVec); 
} 
 
}
</pre>
<div class="preview">
<pre class="cplusplus"><span class="cpp__keyword">void</span>&nbsp;CubeRenderer::ScreenToView(&nbsp;&nbsp;
_In_&nbsp;<span class="cpp__datatype">float</span>&nbsp;sx,&nbsp;&nbsp;
_In_&nbsp;<span class="cpp__datatype">float</span>&nbsp;sy,&nbsp;&nbsp;
_Outptr_&nbsp;<span class="cpp__datatype">float</span>&nbsp;*&nbsp;vx,&nbsp;&nbsp;
_Outptr_&nbsp;<span class="cpp__datatype">float</span>&nbsp;*&nbsp;vy&nbsp;&nbsp;
)&nbsp;&nbsp;
{&nbsp;&nbsp;
*vx&nbsp;=&nbsp;(<span class="cpp__number">2</span>.0f&nbsp;*&nbsp;sx&nbsp;/&nbsp;m_d3dRenderTargetSize.Width&nbsp;-&nbsp;<span class="cpp__number">1</span>.0f)&nbsp;/&nbsp;m_cbMVPData.projection._11;&nbsp;&nbsp;
*vy&nbsp;=&nbsp;(-<span class="cpp__number">2</span>.0f&nbsp;*&nbsp;sy&nbsp;/&nbsp;m_d3dRenderTargetSize.Height&nbsp;&#43;&nbsp;<span class="cpp__number">1</span>.0f)&nbsp;/&nbsp;m_cbMVPData.projection._22;&nbsp;&nbsp;
}&nbsp;&nbsp;
<span class="cpp__keyword">void</span>&nbsp;CubeRenderer::VectorToLocal(&nbsp;&nbsp;
_In_&nbsp;XMVECTOR&nbsp;inVec,&nbsp;&nbsp;
_Outptr_&nbsp;XMVECTOR&nbsp;*&nbsp;outVec&nbsp;&nbsp;
)&nbsp;&nbsp;
{&nbsp;&nbsp;
XMMATRIX&nbsp;viewMx&nbsp;=&nbsp;XMLoadFloat4x4(&amp;m_cbMVPData.view);&nbsp;&nbsp;
XMMATRIX&nbsp;modelMx&nbsp;=&nbsp;XMLoadFloat4x4(&amp;m_cbMVPData.model);&nbsp;&nbsp;
XMMATRIX&nbsp;invView&nbsp;=&nbsp;XMMatrixInverse(&amp;XMMatrixDeterminant(viewMx),&nbsp;viewMx);&nbsp;&nbsp;
XMMATRIX&nbsp;invModel&nbsp;=&nbsp;XMMatrixInverse(&amp;XMMatrixDeterminant(modelMx),&nbsp;modelMx);&nbsp;&nbsp;
XMMATRIX&nbsp;toLocal&nbsp;=&nbsp;invView&nbsp;*&nbsp;invModel;&nbsp;&nbsp;
XMFLOAT4&nbsp;inVecF;&nbsp;&nbsp;
XMStoreFloat4(&amp;inVecF,&nbsp;inVec);&nbsp;&nbsp;
<span class="cpp__keyword">if</span>(<span class="cpp__number">1</span>.0f&nbsp;==&nbsp;inVecF.w)<span class="cpp__com">//point&nbsp;vector&nbsp;</span>&nbsp;
{&nbsp;&nbsp;
*outVec&nbsp;=&nbsp;XMVector3TransformCoord(inVec,&nbsp;toLocal);&nbsp;&nbsp;
}&nbsp;&nbsp;
<span class="cpp__keyword">else</span>&nbsp;&nbsp;
{&nbsp;&nbsp;
*outVec&nbsp;=&nbsp;XMVector3TransformNormal(inVec,&nbsp;toLocal);&nbsp;&nbsp;
*outVec&nbsp;=&nbsp;XMVector3Normalize(*outVec);&nbsp;&nbsp;
}&nbsp;&nbsp;
&nbsp;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
</span></span></div>
<p></p>
<p><span><span>ただし、ここでは単に、DirectXMath の </span><a href="http://msdn.microsoft.com/en-us/library/microsoft.directx_sdk.transformation.xmvector3unproject(v=vs.85).aspx"><span>XMVector3Unproject</span></a><span> に由来するメソッドを使用します。</span></span></p>
<p><span><span>x と y の値が同じで、z の値が異なる画面領域から 2 つの点を計算します。</span><span>&nbsp; </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>
<pre class="hidden">XMVECTOR vector1 = DirectX::XMVector3Unproject( 
XMVectorSet(sx, sy, 0.0f, 1.0f), 
0.0f, 
0.0f, 
m_d3dRenderTargetSize.Width, 
m_d3dRenderTargetSize.Height, 
0.0f, 
1.0f, 
XMLoadFloat4x4(&amp;m_cbMVPData.projection), 
XMLoadFloat4x4(&amp;m_cbMVPData.view), 
XMLoadFloat4x4(&amp;m_cbMVPData.model) 
); 
XMVECTOR vector2 = DirectX::XMVector3Unproject( 
XMVectorSet(sx, sy, 1.0f, 1.0f), 
0.0f, 
0.0f, 
m_d3dRenderTargetSize.Width, 
m_d3dRenderTargetSize.Height, 
0.0f, 
1.0f, 
XMLoadFloat4x4(&amp;m_cbMVPData.projection), 
XMLoadFloat4x4(&amp;m_cbMVPData.view), 
XMLoadFloat4x4(&amp;m_cbMVPData.model) 
); 
</pre>
<div class="preview">
<pre class="cplusplus">XMVECTOR&nbsp;vector1&nbsp;=&nbsp;DirectX::XMVector3Unproject(&nbsp;&nbsp;
XMVectorSet(sx,&nbsp;sy,&nbsp;<span class="cpp__number">0</span>.0f,&nbsp;<span class="cpp__number">1</span>.0f),&nbsp;&nbsp;
<span class="cpp__number">0</span>.0f,&nbsp;&nbsp;
<span class="cpp__number">0</span>.0f,&nbsp;&nbsp;
m_d3dRenderTargetSize.Width,&nbsp;&nbsp;
m_d3dRenderTargetSize.Height,&nbsp;&nbsp;
<span class="cpp__number">0</span>.0f,&nbsp;&nbsp;
<span class="cpp__number">1</span>.0f,&nbsp;&nbsp;
XMLoadFloat4x4(&amp;m_cbMVPData.projection),&nbsp;&nbsp;
XMLoadFloat4x4(&amp;m_cbMVPData.view),&nbsp;&nbsp;
XMLoadFloat4x4(&amp;m_cbMVPData.model)&nbsp;&nbsp;
);&nbsp;&nbsp;
XMVECTOR&nbsp;vector2&nbsp;=&nbsp;DirectX::XMVector3Unproject(&nbsp;&nbsp;
XMVectorSet(sx,&nbsp;sy,&nbsp;<span class="cpp__number">1</span>.0f,&nbsp;<span class="cpp__number">1</span>.0f),&nbsp;&nbsp;
<span class="cpp__number">0</span>.0f,&nbsp;&nbsp;
<span class="cpp__number">0</span>.0f,&nbsp;&nbsp;
m_d3dRenderTargetSize.Width,&nbsp;&nbsp;
m_d3dRenderTargetSize.Height,&nbsp;&nbsp;
<span class="cpp__number">0</span>.0f,&nbsp;&nbsp;
<span class="cpp__number">1</span>.0f,&nbsp;&nbsp;
XMLoadFloat4x4(&amp;m_cbMVPData.projection),&nbsp;&nbsp;
XMLoadFloat4x4(&amp;m_cbMVPData.view),&nbsp;&nbsp;
XMLoadFloat4x4(&amp;m_cbMVPData.model)&nbsp;&nbsp;
);&nbsp;&nbsp;
</pre>
</div>
</div>
</div>
</span></span>
<p></p>
<p><span><span>続いて、原点 </span><span>(vector1) および方向ベクトル (vector2 &ndash; vector1) を取得できます。これらは、XMVector3Unproject メッソドに設定されます。方向ベクトルを正規化することに注意してください。</span></span></p>
<p><span>これで、3D オブジェクトを繰り返し処理し、選択線がオブジェクトと交差するかどうかをテストできます。DirectXMath に新しく導入された</span><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh855922(v=vs.85).aspx"><span>Intersects(&hellip;)</span></a><span> メソッドを使用します。</span></p>
<p><span>交差をテストできれば、ポインターをドラッグしてオブジェクトを変換します。</span></p>
<p><span>回転を処理する場合は、オブジェクトを円弧のボールに変換するため、スムーズに回転させることができます。</span><strong></strong><em></em></p>
<p></p>
<div class="endscriptcode"><span><span>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>
<pre class="hidden">void CubeRenderer::ScreenToArcBall( 
_In_ float sx,  
_In_ float sy,  
_Outptr_ DirectX::XMFLOAT3 &amp;vec 
) 
{ 
float width = m_windowBounds.Width; 
float height = m_windowBounds.Height; 
float x = ( sx  - width / 2 ) / ( width / 2 ); 
float y = -( sy - height / 2 ) / ( height / 2 ); 
 
float z = 0.0f; 
float mag =  x * x &#43; y * y; 
 
if( mag &gt; 1.0f ) 
{ 
float scale = 1.0f / sqrtf( mag ); 
x *= scale; 
y *= scale; 
} 
else 
z = -(sqrtf( 1.0f - mag )); 
vec = XMFLOAT3(x, y, z); 
} 
</pre>
<div class="preview">
<pre class="cplusplus"><span class="cpp__keyword">void</span>&nbsp;CubeRenderer::ScreenToArcBall(&nbsp;&nbsp;
_In_&nbsp;<span class="cpp__datatype">float</span>&nbsp;sx,&nbsp;&nbsp;&nbsp;
_In_&nbsp;<span class="cpp__datatype">float</span>&nbsp;sy,&nbsp;&nbsp;&nbsp;
_Outptr_&nbsp;DirectX::XMFLOAT3&nbsp;&amp;vec&nbsp;&nbsp;
)&nbsp;&nbsp;
{&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;width&nbsp;=&nbsp;m_windowBounds.Width;&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;height&nbsp;=&nbsp;m_windowBounds.Height;&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;x&nbsp;=&nbsp;(&nbsp;sx&nbsp;&nbsp;-&nbsp;width&nbsp;/&nbsp;<span class="cpp__number">2</span>&nbsp;)&nbsp;/&nbsp;(&nbsp;width&nbsp;/&nbsp;<span class="cpp__number">2</span>&nbsp;);&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;y&nbsp;=&nbsp;-(&nbsp;sy&nbsp;-&nbsp;height&nbsp;/&nbsp;<span class="cpp__number">2</span>&nbsp;)&nbsp;/&nbsp;(&nbsp;height&nbsp;/&nbsp;<span class="cpp__number">2</span>&nbsp;);&nbsp;&nbsp;
&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;z&nbsp;=&nbsp;<span class="cpp__number">0</span>.0f;&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;mag&nbsp;=&nbsp;&nbsp;x&nbsp;*&nbsp;x&nbsp;&#43;&nbsp;y&nbsp;*&nbsp;y;&nbsp;&nbsp;
&nbsp;&nbsp;
<span class="cpp__keyword">if</span>(&nbsp;mag&nbsp;&gt;&nbsp;<span class="cpp__number">1</span>.0f&nbsp;)&nbsp;&nbsp;
{&nbsp;&nbsp;
<span class="cpp__datatype">float</span>&nbsp;scale&nbsp;=&nbsp;<span class="cpp__number">1</span>.0f&nbsp;/&nbsp;sqrtf(&nbsp;mag&nbsp;);&nbsp;&nbsp;
x&nbsp;*=&nbsp;scale;&nbsp;&nbsp;
y&nbsp;*=&nbsp;scale;&nbsp;&nbsp;
}&nbsp;&nbsp;
<span class="cpp__keyword">else</span>&nbsp;&nbsp;
z&nbsp;=&nbsp;-(sqrtf(&nbsp;<span class="cpp__number">1</span>.0f&nbsp;-&nbsp;mag&nbsp;));&nbsp;&nbsp;
vec&nbsp;=&nbsp;XMFLOAT3(x,&nbsp;y,&nbsp;z);&nbsp;&nbsp;
}&nbsp;&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span>移動を実行する場合、座標のオフセットを定数で除算します。</span></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>
<pre class="hidden">transform = XMMatrixTranslation((x2 - x1) / 200, (y1 - y2) / 200, 0.0f);
</pre>
<div class="preview">
<pre class="cplusplus">transform&nbsp;=&nbsp;XMMatrixTranslation((x2&nbsp;-&nbsp;x1)&nbsp;/&nbsp;<span class="cpp__number">200</span>,&nbsp;(y1&nbsp;-&nbsp;y2)&nbsp;/&nbsp;<span class="cpp__number">200</span>,&nbsp;<span class="cpp__number">0</span>.0f);&nbsp;
</pre>
</div>
</div>
</div>
</div>
</span></span></div>
<p></p>
<div class="endscriptcode">&nbsp;<span>拡大/縮小を実行する場合も、円弧のボールを使用してボールの周囲におけるポインター オフセットの距離を計算します。その後、データを
</span><a href="http://msdn.microsoft.com/en-us/library/microsoft.directx_sdk.matrix.xmmatrixscaling(v=vs.85).aspx"><span>XMMatrixScaling(&hellip;)</span></a><span> に設定します。</span></div>
<h2><span><span>詳細</span></span></h2>
<p><span><span>&bull;&nbsp;</span><a href="http://www.braynzarsoft.net/index.php?p=D3D11PICKING"><span>Direct3D 11 の選択</span></a></span><strong></strong><em></em></p>
<p></p>
<div class="endscriptcode"><span><span>
<div class="endscriptcode"></div>
</span></span></div>
<p></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><span style="color:#ffffff"><img id="154439" src="154439-e61fed22-d54f-413d-844a-0acf6159f342image.png" alt="" width="341" height="57"><br>
</span></p>
<p><em><br>
</em></p>
