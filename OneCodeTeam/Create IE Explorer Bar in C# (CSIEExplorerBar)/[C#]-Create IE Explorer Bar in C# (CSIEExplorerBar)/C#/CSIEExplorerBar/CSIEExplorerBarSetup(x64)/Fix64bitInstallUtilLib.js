// http://blogs.msdn.com/b/heaths/archive/2006/02/01/64-bit-managed-custom-actions-with-visual-studio.aspx
var msiOpenDatabaseModeTransact = 1;
var msiViewModifyUpdate = 2

var filespec = WScript.Arguments(0);
var projdir = WScript.Arguments(1);
var installer = WScript.CreateObject("WindowsInstaller.Installer");
var database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);

// Update the Binary table...
var sql = "SELECT `Name`,`Data` FROM `Binary` where `Binary`.`Name` = 'InstallUtil'";
var view = database.OpenView(sql);
view.Execute();
var record = view.Fetch();
record.SetStream(2, projdir + "InstallUtilLib.dll");
view.Modify(msiViewModifyUpdate, record);
view.Close();
database.Commit();