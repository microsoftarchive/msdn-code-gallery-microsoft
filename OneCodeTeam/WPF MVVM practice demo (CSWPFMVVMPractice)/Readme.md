# WPF MVVM practice demo (CSWPFMVVMPractice)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- WPF
## Topics
- MVVM
## Updated
- 11/27/2012
## Description

<h1>How to implement the MVVM pattern in a WPF application (<span class="SpellE">CSWPFMVVMPractice</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to implement the MVVM pattern in a WPF application.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1. Build the sample project in Visual Studio 2012.</p>
<p class="MsoNormal">Step2. Click on the cells in the grid. </p>
<p class="MsoNormal">Step3. If one player has won the game, a <span class="SpellE">
messagebox</span> pops up saying &quot;XX has <span class="SpellE">won<span class="GramE">,Congratulations</span></span>!&quot;.</p>
<p class="MsoNormal">Step4. If all the cells in the grid are clicked, but no one has won, a
<span class="SpellE">messagebox</span> pops up saying &quot;No winner&quot;.</p>
<p class="MsoNormal">Step5. You can change the dimension of the game using the Game menu.</p>
<p class="MsoNormal"><span style=""><img src="71343-image.png" alt="" width="955" height="710" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">1. Model: </p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Cell class - represents a cell in the tic-tac-toe game grid</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">PlayerMove</span> class - represents a player move in the game</p>
<p class="MsoNormal">2. <span class="SpellE">ViewModel</span>:</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">TicTacToeViewModel</span> - contains game's logic and data</p>
<p class="MsoNormal">3. View:</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">MainWindow.xaml</span> - contains a Menu and an <span class="SpellE">
ItemsControl</span></p>
<p class="MsoNormal">4. Others:</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>a. Attached behavior</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">ChangeDimensionBehavior</span> - connect the <span class="SpellE">
MenuItem</span> in the View to the <span class="SpellE">TicTacToeViewModel</span> in order to change the game's dimension</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">ShutdownBehavior</span> - contains code to exit the application<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">GameOverBehavior</span> - listen to the <span class="SpellE">
GameOver</span> event on the <span class="SpellE">TicTacToeViewModel</span> and show a
<span class="SpellE">messagebox</span> reporting the game result.</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>b. <span class="SpellE">ValueConverter</span></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">IntToBoolValueConverter</span> - used to check/uncheck a
<span class="SpellE">MenItem</span> that is used to change the dimension of the game<span style="">&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">c.Command</span></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE">RelayCommand</span> - provides an implementation of the
<span class="SpellE">ICommand</span> interface<b> </b></p>
<h2>More Information</h2>
<p class="MsoNormal">WPF Apps <span class="GramE">With</span> The Model-View-<span class="SpellE">ViewModel</span> Design Pattern
</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/magazine/dd419663.aspx">http://msdn.microsoft.com/en-us/magazine/dd419663.aspx</a><span style="">
</span></p>
<p class="MsoNormal">Introduction to Attached Behaviors in WPF </p>
<p class="MsoNormal"><a href="http://www.codeproject.com/KB/WPF/AttachedBehaviors.aspx">http://www.codeproject.com/KB/WPF/AttachedBehaviors.aspx</a><span style="">
</span></p>
<p class="MsoNormal"><span style=""></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
