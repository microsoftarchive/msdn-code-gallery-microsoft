REM Run this script in and elevated cmd prompt to:
REM 1. Install IIS
REM 2. Install JsonWebTokenSample server site

REM Install and configure IIS
dism /Online /Enable-Feature /FeatureName:NetFx4Extended-ASPNET45 /FeatureName:IIS-ApplicationDevelopment /FeatureName:IIS-ASPNET45 /FeatureName:IIS-BasicAuthentication /FeatureName:IIS-ClientCertificateMappingAuthentication /FeatureName:IIS-CommonHttpFeatures /FeatureName:IIS-CustomLogging /FeatureName:IIS-DefaultDocument /FeatureName:IIS-DigestAuthentication /FeatureName:IIS-DirectoryBrowsing /FeatureName:IIS-FTPExtensibility /FeatureName:IIS-FTPServer /FeatureName:IIS-FTPSvc /FeatureName:IIS-HealthAndDiagnostics /FeatureName:IIS-HostableWebCore /FeatureName:IIS-HttpCompressionDynamic /FeatureName:IIS-HttpCompressionStatic /FeatureName:IIS-HttpErrors /FeatureName:IIS-HttpLogging /FeatureName:IIS-HttpRedirect /FeatureName:IIS-HttpTracing /FeatureName:IIS-IISCertificateMappingAuthentication /FeatureName:IIS-IPSecurity /FeatureName:IIS-ISAPIExtensions /FeatureName:IIS-ISAPIFilter /FeatureName:IIS-LoggingLibraries /FeatureName:IIS-ManagementConsole /FeatureName:IIS-ManagementScriptingTools /FeatureName:IIS-ManagementService /FeatureName:IIS-NetFxExtensibility45 /FeatureName:IIS-ODBCLogging /FeatureName:IIS-Performance /FeatureName:IIS-RequestFiltering /FeatureName:IIS-RequestMonitor /FeatureName:IIS-Security /FeatureName:IIS-ServerSideIncludes /FeatureName:IIS-StaticContent /FeatureName:IIS-URLAuthorization /FeatureName:IIS-WebDAV /FeatureName:IIS-WebServer /FeatureName:IIS-WebServerManagementTools /FeatureName:IIS-WebServerRole /FeatureName:IIS-WindowsAuthentication
net stop w3svc
net stop ftpsvc

pushd %windir%\system32\inetsrv

REM Enable FTP
appcmd add site /name:"ftp" /bindings:ftp/*:21: /physicalPath:"%systemdrive%\inetpub\ftproot" /ftpServer.security.ssl.controlChannelPolicy:"SslAllow" /ftpServer.security.ssl.dataChannelPolicy:"SslAllow" /ftpServer.security.authentication.anonymousAuthentication.enabled:"true" /ftpServer.security.authentication.basicAuthentication.enabled:"true"
appcmd set config "ftp" /section:system.ftpServer/security/authorization /+[accessType='Allow',permissions='Read,Write',users='*'] /commit:apphost

REM Disable caching
appcmd set config -section:system.webServer/caching -enabled:false -enableKernelCache:false

REM Allow uploads greater than 30M (bump limit to 500M)
appcmd set config -section:system.webServer/security/requestFiltering -requestLimits.maxAllowedContentLength:500000000

REM Configure JsonWebTokenSample application
appcmd add app /site.name:"Default Web Site" /path:"/JsonWebTokenSample" /physicalPath:"%systemdrive%\inetpub\wwwroot\JsonWebTokenSample"

popd

net start w3svc
net start ftpsvc

REM Firewall exceptions for network isolation
netsh advfirewall firewall add rule name="HTTP 80" dir=in protocol=TCP localport=80 action=allow
netsh advfirewall firewall add rule name="FTP 21" dir=in protocol=TCP localport=21 action=allow

REM Setup JsonWebTokenSample Website
xcopy /siey JsonWebTokenSample %SystemDrive%\inetpub\wwwroot\JsonWebTokenSample
