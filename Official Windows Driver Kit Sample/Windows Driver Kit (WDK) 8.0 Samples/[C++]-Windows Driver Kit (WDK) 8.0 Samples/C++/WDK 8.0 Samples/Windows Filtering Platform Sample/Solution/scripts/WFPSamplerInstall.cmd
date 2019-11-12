@ECHO OFF

:SoF

   IF /I "%1"==""   goto :HELP
   IF /I "%1"=="?"  goto :HELP
   IF /I "%1"=="-?" goto :HELP
   IF /I "%1"=="/?" goto :HELP

   IF /I "%1"=="r"  goto :UNINSTALL
   IF /I "%1"=="-r" goto :UNINSTALL
   IF /I "%1"=="/r" goto :UNINSTALL

:INSTALL

   ECHO.
   ECHO Installing WFPSampler
   ECHO.

   ECHO Copying WFPSampler Bins from %1\ to %WinDir%\System32\Drivers\
   copy %1\WFPSamplerCalloutDriver.cat %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.cat
   copy %1\WFPSamplerCalloutDriver.inf %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.inf
   copy %1\WFPSamplerCalloutDriver.sys %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.sys

   ECHO Copying WFPSampler Bins to  %WinDir%\System32
   copy %1\WFPSampler.exe %WinDir%\System32\WFPSampler.exe
   copy %1\WFPSamplerService.exe %WinDir%\System32\WFPSamplerService.exe

   ECHO Registering the WFPSampler Service
   %WinDir%\System32\WFPSamplerService.Exe -i

   ECHO Registering the WFPSampler Callout Driver
   RunDLL32.Exe syssetup,SetupInfObjectInstallAction DefaultInstall 131 %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Inf

   SET BIN_PATH=

   goto :EoF

:UNINSTALL

   ECHO.
   ECHO Uninstalling WFPSampler
   ECHO.

   Net Stop WFPSampler

   Net Stop WFPSamplerCallouts

   ECHO Unregistering the WFPSampler Service
   %WinDir%\System32\WFPSamplerService.Exe -i      

   ECHO Unregistering the WFPSampler Callout Driver
   RunDLL32.Exe SETUPAPI.DLL,InstallHinfSection DefaultUninstall 132 %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Inf

   ECHO Deleting WFPSampler Bins from %WinDir%\System32\
   del %WinDir%\System32\WFPSampler.exe
   del %WinDir%\System32\WFPSamplerService.exe

   ECHO Deleting WFPSampler Bins from %WinDir%\System32\Drivers\
   del %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.cat
   del %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.inf
   del %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.sys

   SET BIN_PATH=

   goto :EoF

:HELP

      ECHO.
      ECHO WFPSamplerInstall.cmd [%%PATH%% ^| -r]
      ECHO.
      ECHO      %%PATH%%     Copies binaries from specified path and installs the WFPSampler
      ECHO      -r           Uninstalls the WFPSampler and removes binaries
      ECHO.
      ECHO    WFPSampler.cmd C:\SampleBinaries
      ECHO.

:EoF