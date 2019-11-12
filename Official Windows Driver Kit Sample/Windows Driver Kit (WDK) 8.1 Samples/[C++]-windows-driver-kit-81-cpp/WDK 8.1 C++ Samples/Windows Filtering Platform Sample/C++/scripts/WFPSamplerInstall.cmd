@ECHO OFF

:SoF

   IF /I "%1"=="?"  goto :HELP
   IF /I "%1"=="-?" goto :HELP
   IF /I "%1"=="/?" goto :HELP

   IF /I "%1"=="r"  goto :UNINSTALL
   IF /I "%1"=="-r" goto :UNINSTALL
   IF /I "%1"=="/r" goto :UNINSTALL

   IF /I "%1"=="" (
      SET BIN_PATH=.
   ) ELSE (
      SET BIN_PATH=%1
   )

:INSTALL

   ECHO.
   ECHO Installing WFPSampler

   ECHO.
   ECHO Attempting to sign WFPSampler.Exe
      SignTool.exe Sign -A -V WFPSampler.Exe

   ECHO.
   ECHO Attempting to sign WFPSampler.Exe
      SignTool.exe Sign -A -V WFPSamplerService.Exe

   ECHO.
   ECHO Copying WFPSamplerCalloutDriver Bins from %BIN_PATH%\ to %WinDir%\System32\Drivers\
      Copy %BIN_PATH%\WFPSamplerCalloutDriver.cat %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.cat
      Copy %BIN_PATH%\WFPSamplerCalloutDriver.inf %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.inf
      Copy %BIN_PATH%\WFPSamplerCalloutDriver.sys %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.sys

   ECHO.
   ECHO Copying WFPSampler application binaries from %1\ to %WinDir%\System32\
      Copy %BIN_PATH%\WFPSampler.exe %WinDir%\System32\WFPSampler.exe
      Copy %BIN_PATH%\WFPSamplerService.exe %WinDir%\System32\WFPSamplerService.exe

   IF EXIST %WinDir%\System32\WFPSamplerService.Exe (
      ECHO.
      ECHO Registering the WFPSampler Service
         %WinDir%\System32\WFPSamplerService.Exe -i
         Net Start WFPSampler
   )

   IF EXIST %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Inf (
      IF EXIST %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Sys (
         ECHO.
         ECHO Registering the WFPSampler Callout Driver
            RunDLL32.Exe syssetup,SetupInfObjectInstallAction DefaultInstall 131 %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Inf
            Net Start WFPSamplerCallouts
      )
   )

   SET BIN_PATH=

   goto :EoF

:UNINSTALL

   ECHO.
   ECHO Uninstalling WFPSampler

   IF EXIST %WinDir%\System32\WFPSampler.Exe (
      ECHO.
      ECHO Removing policy
         WFPSampler.exe -clean
   )

   ECHO.
   ECHO Stopping the WFPSampler service
      Net Stop WFPSampler

   ECHO.
   ECHO Stopping the WFPSamplerCallouts service
      Net Stop WFPSamplerCallouts

   IF EXIST %WinDir%\System32\WFPSamplerService.Exe (
      ECHO.
      ECHO Unregistering the WFPSampler Service
         %WinDir%\System32\WFPSamplerService.Exe -u
   )

   IF EXIST %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Inf (
      IF EXIST %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Sys (
         ECHO.
         ECHO Unregistering the WFPSampler Callout Driver
            RunDLL32.Exe SETUPAPI.DLL,InstallHinfSection DefaultUninstall 132 %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.Inf
      )
   )

   ECHO.
   ECHO Deleting WFPSampler application binaries from %WinDir%\System32\
      Erase %WinDir%\System32\WFPSampler.exe
      Erase %WinDir%\System32\WFPSamplerService.exe

   ECHO.
   ECHO Deleting WFPSamplerCalloutDriver binaries from %WinDir%\System32\Drivers\
      Erase %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.cat
      Erase %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.inf
      Erase %WinDir%\System32\Drivers\WFPSamplerCalloutDriver.sys

   SET BIN_PATH=

   goto :EoF

:HELP

      ECHO.
      ECHO WFPSamplerInstall.cmd [%%PATH%% ^| -r]
      ECHO.
      ECHO      %%PATH%%     Copies binaries from specified path and installs the WFPSampler
      ECHO                      (default is .)
      ECHO.
      ECHO      -r         Uninstalls the WFPSampler and removes binaries
      ECHO.
      ECHO    WFPSampler.cmd %ProgramFiles(x86)%\Windows Kits\8.1\src\network\trans\WFPSampler\x64\Win8Debug\Package
      ECHO.

:EoF