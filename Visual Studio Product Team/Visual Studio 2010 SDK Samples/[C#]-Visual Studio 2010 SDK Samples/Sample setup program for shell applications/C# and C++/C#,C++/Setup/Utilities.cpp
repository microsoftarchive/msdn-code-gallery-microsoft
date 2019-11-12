//******************************************************************
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// This code is licensed under the Visual Studio SDK license terms.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//******************************************************************
#include "stdafx.h"
#include "Setup.h"
#include "SetupDlg.h"
#include "CSetupWatcher.h"

//----------------------------------------------------------------------------
// AddSetupToStartup()
//
// Find out if the process is running with elevated priviledges.
//----------------------------------------------------------------------------
HRESULT AddSetupToStartup()
{
    HRESULT hr = S_OK;
    
    HKEY hkRoot = HKEY_LOCAL_MACHINE;
    HKEY hk = NULL;
    LONG lRet = ERROR_SUCCESS;
    
    TCHAR file[MAX_PATH];
    GetModuleFileName(NULL, file, MAX_PATH * sizeof(TCHAR));

    CString runOncePath(MAKEINTRESOURCE(IDS_KEY_RUNONCE_ROOT));
    CString name(MAKEINTRESOURCE(IDS_KEY_RUNONCE_SETUP));
    CString value;
    value.Format(_T("\"%s\" /restart"), file);
    
    lRet = RegCreateKeyEx(HKEY_LOCAL_MACHINE, runOncePath, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_SET_VALUE, NULL, &hk, NULL);
    if ( lRet == ERROR_SUCCESS )
    {
        lRet = RegSetValueEx(hk, name, 0, REG_SZ, (const BYTE*)(LPCTSTR)value, value.GetLength() * sizeof(TCHAR));
        if (lRet != ERROR_SUCCESS)
        {
            hr = HRESULT_FROM_WIN32(lRet);
        }

        RegCloseKey(hk);
    }
    else
    {
        hr = HRESULT_FROM_WIN32(lRet);
    }
            
    return hr;
}
//----------------------------------------------------------------------------
// GetSetupPath()
//
// Gets the path, including trailing backslash, to the setup process.
//----------------------------------------------------------------------------
CString GetSetupPath()
{
    TCHAR file[MAX_PATH];
    GetModuleFileName(NULL, file, MAX_PATH * sizeof(TCHAR));
    CString path(file);
    int fpos = path.ReverseFind('\\');
    if (fpos != -1)
    {
        path = path.Left(fpos + 1);
    }

    return path;
}
    
//----------------------------------------------------------------------------
// ExecCmd(const CString cmd, const CString currentDirectory)
//
// Executes the supplied command.  Provides ability to override the default working directory of the process.
//----------------------------------------------------------------------------
DWORD ExecCmd(const CString cmd, const BOOL setCurrentDirectory)
{
    BOOL  bReturnVal   = false ;
    STARTUPINFO  si ;
    DWORD  dwExitCode = ERROR_NOT_SUPPORTED;
    SECURITY_ATTRIBUTES saProcess, saThread ;
    PROCESS_INFORMATION process_info ;

    ZeroMemory(&si, sizeof(si)) ;
    si.cb = sizeof(si) ;

    saProcess.nLength = sizeof(saProcess) ;
    saProcess.lpSecurityDescriptor = NULL ;
    saProcess.bInheritHandle = TRUE ;

    saThread.nLength = sizeof(saThread) ;
    saThread.lpSecurityDescriptor = NULL ;
    saThread.bInheritHandle = FALSE ;

    LPCWSTR currentDirectory = NULL;
    
    if (setCurrentDirectory == TRUE)
    {
        currentDirectory = GetSetupPath();
    }
    
    bReturnVal = CreateProcess(NULL, 
                                (LPTSTR)(LPCTSTR)cmd,
                                &saProcess, 
                                &saThread, 
                                FALSE, 
                                DETACHED_PROCESS, 
                                NULL,
                                currentDirectory, 
                                &si, 
                                &process_info) ;

    if (bReturnVal)
    {
        CloseHandle( process_info.hThread ) ;
        WaitForSingleObject( process_info.hProcess, INFINITE ) ;
        GetExitCodeProcess( process_info.hProcess, &dwExitCode ) ;
        CloseHandle( process_info.hProcess ) ;
    }

    return dwExitCode;
}

//----------------------------------------------------------------------------
// IsUserAdministrator()
//
// Find out if the process is running with elevated priviledges.
//----------------------------------------------------------------------------
BOOL IsUserAdministrator()
{
    BOOL bRet = FALSE;
    SID_IDENTIFIER_AUTHORITY siaNtAuthority = SECURITY_NT_AUTHORITY;
    PSID psidRidGroup; 

    bRet = AllocateAndInitializeSid(&siaNtAuthority, 2, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS, 0, 0, 0, 0, 0, 0, &psidRidGroup);
    if (bRet)
    {
        if (!CheckTokenMembership(NULL, psidRidGroup, &bRet)) 
        {
            bRet = FALSE;
        } 
        FreeSid(psidRidGroup); 
    }

    return (BOOL)bRet;
}
//----------------------------------------------------------------------------
// RebootMachine()
//
// Reboots the user's machine.  This also deletes the SFX's setup resume data since that should not be run 
// for the shell SFX scenarios.
//----------------------------------------------------------------------------
BOOL RebootMachine(void)
{
    //We need to delete the SFX's resume post-reboot keys so that we can start over correctly and manage the process in our outter setup
    CString resumeSetupPath(MAKEINTRESOURCE(IDS_KEY_SETUP_RESUME));
    RegDeleteKey(HKEY_LOCAL_MACHINE, resumeSetupPath);
    
    BOOL bExitWindows = FALSE;
    OSVERSIONINFO ov;
    ov.dwOSVersionInfoSize = sizeof(ov);

    if ( GetVersionEx(&ov) )
    {
        if(ov.dwPlatformId == VER_PLATFORM_WIN32_NT)
        {
            //NT platform
            HANDLE htoken ;
            //Ask for the SE_SHUTDOWN_NAME token as this is needed by the thread calling for a system shutdown.
            if (OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &htoken))
            {
                LUID luid ;
                LookupPrivilegeValue(NULL, SE_SHUTDOWN_NAME, &luid) ;
                TOKEN_PRIVILEGES    privs ;
                privs.Privileges[0].Luid = luid ;
                privs.PrivilegeCount = 1 ;
                privs.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED ;
                AdjustTokenPrivileges(htoken, FALSE, &privs, 0, (PTOKEN_PRIVILEGES) NULL, 0) ;
            } 

            //on Whistler and future NT use InitiateSystemShutdownEx to avoid unexpected reboot message box
            try
            {            
                if ( (ov.dwMajorVersion > 5) || ( (ov.dwMajorVersion == 5) && (ov.dwMinorVersion  > 0) ))
                {
                    bExitWindows = InitiateSystemShutdownEx(0, _T(""), 0, TRUE, TRUE, REASON_PLANNED_FLAG);
                }
                else
                {
#pragma prefast(suppress:380,"ignore warning about legacy api")
                    bExitWindows = InitiateSystemShutdown(0, _T(""), 0, TRUE, TRUE);
                }
            }
            catch(...)
            {
                //advapi32.dll call not available! Will not reboot!
            }
        }
        else
        {
            //9x system - use ExitWindowsEx
#pragma prefast(suppress:380,"ignore warning about legacy api")
            return ExitWindowsEx(EWX_REBOOT, 0);
        }
    }

    //TODO: Potentially prompt users if you are unable to get the proper shutdown privelege

    return bExitWindows;
}
//----------------------------------------------------------------------------
// GetInstallStatusFromReg( key )
//
// Gets the install status from the provided registry key.
//
// returns TRUE if installed and FALSE if not installed
//----------------------------------------------------------------------------
BOOL GetInstallStatusFromReg(CString key)
{
	HKEY hkey = NULL;
	LONG lResult;
	
    lResult = RegOpenKeyEx( HKEY_LOCAL_MACHINE,       // handle to open key
                            key,                     // name of subkey to open
                            NULL,
                            KEY_READ,
                            &hkey                    // handle to open key
                            );

    // we don't proceed unless the call above succeeds
    if (ERROR_SUCCESS != lResult && ERROR_FILE_NOT_FOUND != lResult)
    {
        throw HRESULT_FROM_WIN32(lResult);
    }

	// If getting the registry key was succesful, let's check the Install value
    if (ERROR_SUCCESS == lResult)
    {
        DWORD buffer;
        DWORD len = sizeof(buffer);
		CString value("Install");

        lResult = RegQueryValueEx( hkey,
                                value,
                                NULL,
                                NULL,
                                (LPBYTE)(char*)(&buffer),
                                &len);

        if (ERROR_SUCCESS == lResult)
        {
            RegCloseKey(hkey);
			if(buffer==1)
			{
				return TRUE;
			}
        }
        else if (ERROR_FILE_NOT_FOUND != lResult)
        {
            RegCloseKey(hkey);
            throw HRESULT_FROM_WIN32(lResult);
        }
    }
    return FALSE;
}
//----------------------------------------------------------------------------
// GetProductDirFromReg()
//
// Gets the shell's install directory on the machine.
//----------------------------------------------------------------------------
CString GetProductDirFromReg( )
{
    LONG lResult;
    HKEY hkey = NULL;
	CString key(MAKEINTRESOURCE(IDS_KEY_INSTALLLOCATION));
    CString productDirectory(_T(""));
    
    //TODO: IDS_KEY_SHELL_LCID if you are targeting a partivular LCID, you
	//      should also check for LCID specific targets
    lResult = RegOpenKeyEx( HKEY_LOCAL_MACHINE,       // handle to open key
                            key,                     // name of subkey to open
                            NULL,
                            KEY_READ,
                            &hkey                    // handle to open key
                            );


    // we don't proceed unless the call above succeeds
    if (ERROR_SUCCESS != lResult && ERROR_FILE_NOT_FOUND != lResult)
    {
        throw HRESULT_FROM_WIN32(lResult);
    }

    // If getting the registry key was succesful, verify that the product directory indeed exists on the machine to
    // protect against any orphaned state
    if (ERROR_SUCCESS == lResult)
    {
		CString value("ShellFolder");
        TCHAR buffer[MAX_PATH];
        DWORD len = sizeof(buffer);

        lResult = RegQueryValueEx( hkey,
                                value,
                                NULL,
                                NULL,
                                (LPBYTE)buffer,
                                &len);

        if (ERROR_SUCCESS == lResult)
        {
            productDirectory = buffer;
            DWORD dwResult = GetFileAttributes(productDirectory);

            if (dwResult == INVALID_FILE_ATTRIBUTES || 
                !(dwResult & FILE_ATTRIBUTE_DIRECTORY))
            {   
                productDirectory = _T("");
            }
        }
        else if (ERROR_FILE_NOT_FOUND != lResult)
        {
            RegCloseKey(hkey);
            throw HRESULT_FROM_WIN32(lResult);
        }

        RegCloseKey(hkey);
    }

    return productDirectory;
}
//----------------------------------------------------------------------------
// BoutiqueInstallRequired()
//
// Finds if the boutique application has already been installed.
//----------------------------------------------------------------------------
BOOL BoutiqueInstallRequired()
{
    BOOL bRequired = TRUE;

    try 
    {
        LONG lResult;
        HKEY hkey = NULL;

        //TODO: Change the value of IDS_KEY_MSIROOT in the string table to the key set by your application's install
        CString detectionKey(MAKEINTRESOURCE(IDS_KEY_MSIROOT));
    
        lResult = RegOpenKeyEx( HKEY_LOCAL_MACHINE, detectionKey, NULL, KEY_READ, &hkey);
        
        // we don't proceed unless the call above succeeds
        if (ERROR_FILE_NOT_FOUND != lResult)
        {
            bRequired = FALSE;
        }
    }
    catch(...)
    {
        CString message(MAKEINTRESOURCE(IDS_FAIL_DETECTMSI_MSG));
        setupApp.OnInstallError(message);
    }

    return bRequired;
}
//----------------------------------------------------------------------------
// ShellIsoInstallRequired()
//
// Finds if the shell (Isolated) SFX has already been installed.
//----------------------------------------------------------------------------
BOOL ShellIsoInstallRequired()
{
    BOOL bRequired = TRUE;

    try 
    {
		if(GetInstallStatusFromReg(CString(MAKEINTRESOURCE(IDS_KEY_ISOSHELLDETECT))))
        {
            bRequired = FALSE;
        }
    }
    catch(...)
    {
        CString message(MAKEINTRESOURCE(IDS_FAIL_DETECT_MSG));
        setupApp.OnInstallError(message);
    }

    return bRequired;
}

//----------------------------------------------------------------------------
// ShellIntInstallRequired()
//
// Finds if the shell (Integrated) SFX has already been installed.
//----------------------------------------------------------------------------
BOOL ShellIntInstallRequired()
{
    BOOL bRequired = TRUE;

    try 
    {
		if(GetInstallStatusFromReg(CString(MAKEINTRESOURCE(IDS_KEY_INTSHELLDETECT))))
        {
            bRequired = FALSE;
        }
    }
    catch(...)
    {
        CString message(MAKEINTRESOURCE(IDS_FAIL_DETECT_MSG));
        setupApp.OnInstallError(message);
    }

    return bRequired;
}

//----------------------------------------------------------------------------
// Install(LPVOID pParam)
//
// Installs the required components.  This method is designed to be called on the background
// worker thread so that it does not hang the setup UI.
//----------------------------------------------------------------------------
UINT Install(LPVOID pParam)
{    
    BOOL bInstallIsoSfx = ShellIsoInstallRequired();
    BOOL bInstallIntSfx = ShellIntInstallRequired();
	BOOL bInstallBoutique = BoutiqueInstallRequired();
    int ToInstall = 0;
    if(bInstallIsoSfx) ToInstall++;
    if(bInstallIntSfx) ToInstall++;
	if(bInstallBoutique) ToInstall++;

    while(ToInstall>0)
    {
        DWORD dwResult = ERROR_SUCCESS;
    
        if (bInstallIsoSfx == TRUE)
        {
            dwResult = InstallIsoShellSfx();
            bInstallIsoSfx = FALSE;
        }
        else if (bInstallIntSfx == TRUE)
        {
            dwResult = InstallIntShellSfx();
            bInstallIntSfx = FALSE;
        }
        else
        {
            dwResult = InstallShellBoutique();
        }
    
        switch (dwResult)
        {
            case (IMMEDIDATE_REBOOT_REQUIRED):
            case (ERROR_SUCCESS_REBOOT_REQUIRED):
            {
                AddSetupToStartup();
                setupApp.GetProgressPage().OnRebootRequired();
				ToInstall = 0; // Don't install anything else right now.
                break;
            }
            case (ERROR_SUCCESS):
            {
                ToInstall--;
                if(ToInstall==0)
                {
                    setupApp.GetProgressPage().OnInstallFinished();
                }
                break;
            }
            default :
            {
                setupApp.OnInstallError(dwResult);
				ToInstall = 0; // Don't attempt to install anything else.
                break;
            }
            break;
        }
    }
    return 0;
}
//----------------------------------------------------------------------------
// InstallShellBoutique()
//
// Installs the boutique application's MSI.
//----------------------------------------------------------------------------
DWORD InstallShellBoutique()
{
    CString actionTemplate(MAKEINTRESOURCE(IDS_PROGRESS_CURRACTION));
    CString details(MAKEINTRESOURCE(IDS_PROGRESS_INSTALL));
    CString currentAction;
    currentAction.Format(actionTemplate, details);
    setupApp.GetProgressPage().SetCurrentComponentText(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    setupApp.GetProgressPage().SetCurrentActionText(currentAction);

    DWORD dwResults[2];

    TCHAR temp[MAX_PATH];
    GetTempPath(MAX_PATH, temp);

    CString boutiqueInstallCmd, msi, log;
    CString cmdLine(MAKEINTRESOURCE(IDS_CMDLINE_MSIEXEC));
    CString name(MAKEINTRESOURCE(IDS_BOUTIQUE_MSI));
    log.Format(_T("\"%s%s.log\""), temp, name);
    msi.Format(_T("\"%s%s\""), GetSetupPath(), name);
    boutiqueInstallCmd.Format(cmdLine, msi, log);

    //TODO: You can use MSI API to gather and present install progress feedback from your MSI.
    dwResults[0] = ExecCmd(boutiqueInstallCmd, FALSE);

	//TODO: result should not be ignored
    CString SlashSetup = GetProductDirFromReg();
	SlashSetup = SlashSetup + CString("Common7\\IDE\\devenv /Setup");
	dwResults[1] = ExecCmd(SlashSetup, FALSE);

	if(dwResults[0]==ERROR_SUCCESS)
	{
		return dwResults[1];
	}
    return dwResults[0];
}
//----------------------------------------------------------------------------
// InstallIsoShellSfx()
//
// Installs the Shell (Isolate) SFX.  A pipe is created between the SFX process and the setup process so that progress can be reported to the user.
//----------------------------------------------------------------------------
DWORD InstallIsoShellSfx()
{
    setupApp.GetProgressPage().SetCurrentComponentText(MAKEINTRESOURCE(IDS_ISOSHELL_NAME));

    DWORD dwResult;

    CString sfx(MAKEINTRESOURCE(IDS_ISOSHELL_SFX));
    CString cmdLine(MAKEINTRESOURCE(IDS_CMDLINE_SHELLSFX));
    CString shellInstallCmd;
    shellInstallCmd.Format(_T("\"%s\" %s"), sfx, cmdLine);

    //we need to set our current directory for the new process to the directory that the file exists, not the working directory
    //this is due to an issue where the SFX can lose the command line arguments.
    dwResult = ExecCmd(shellInstallCmd, TRUE);

    return dwResult;
}
//----------------------------------------------------------------------------
// InstallIntShellSfx()
//
// Installs the shell (Integrated) SFX.  A pipe is created between the SFX process and the setup process so that progress can be reported to the user.
//----------------------------------------------------------------------------
DWORD InstallIntShellSfx()
{
    setupApp.GetProgressPage().SetCurrentComponentText(MAKEINTRESOURCE(IDS_INTSHELL_NAME));

    // This sets up the pipe used to report progress from the SFX

    DWORD dwResult;

    CString sfx(MAKEINTRESOURCE(IDS_INTSHELL_SFX));
    CString cmdLine(MAKEINTRESOURCE(IDS_CMDLINE_SHELLSFX));
    CString fullCmdLind, shellInstallCmd;
    shellInstallCmd.Format(_T("\"%s\" %s"), sfx, fullCmdLind);

    //we need to set our current directory for the new process to the directory that the file exists, not the working directory
    //this is due to an issue where the SFX can lose the command line arguments.
    dwResult = ExecCmd(shellInstallCmd, TRUE);

    return dwResult;
}
//----------------------------------------------------------------------------
// SetupUIHandler()
//
// The callback used by the pipe between the SFX and Setup so that progress can be reported during install.
//----------------------------------------------------------------------------
int CALLBACK SetupUIHandler(void* pvContext, UINT iMessageType, MSIHANDLE hrecMsg)
{
    if (setupApp.IsCancelRequested() && !setupApp.IsCancelInitiated())
    {
        setupApp.SetCancelInitiated();
        return IDCANCEL;
    }

    if (iMessageType == INSTALLMESSAGE_SUITEPROGRESS)
    {
        DWORD downloadedKb = 0;
        DWORD totalKb = 0;
        DWORD downloadRate = 0;
        DWORD percentComplete = 0;
        CString currentAction;
        CString actionTemplate(MAKEINTRESOURCE(IDS_PROGRESS_CURRACTION));
        CString details(MAKEINTRESOURCE(IDS_PROGRESS_INIT));
        switch(MsiRecordGetInteger(hrecMsg, 1))
        {
        case 1: //Initializing
            percentComplete = MsiRecordGetInteger(hrecMsg, 2);
            if (percentComplete != setupApp.GetProgressPage().GetProgressBarPosition())
            {
                details.LoadStringW(IDS_PROGRESS_INIT);
                currentAction.Format(actionTemplate, details);
                setupApp.GetProgressPage().SetCurrentActionText(currentAction);
                setupApp.GetProgressPage().SetProgressBarPosition(percentComplete);
            }
            break;
        case 2: //Downloading
            /*
            Field 2: Integer indicating the number of KB downloaded so far.
            Field 3: Integer indicating the total number of KB to be downloaded.
            Field 4: Integer indicating the current transfer rate in KB/s.
            */
            downloadedKb = MsiRecordGetInteger(hrecMsg, 2);
            totalKb = MsiRecordGetInteger(hrecMsg, 3);
            downloadRate = MsiRecordGetInteger(hrecMsg, 4);
            percentComplete = (int) 100.0f * downloadedKb / totalKb;            
            if (percentComplete != setupApp.GetProgressPage().GetProgressBarPosition())
            {
                details.LoadStringW(IDS_PROGRESS_DOWN);
                details.Format(details, downloadedKb, totalKb, downloadRate);
                currentAction.Format(actionTemplate, details);
                setupApp.GetProgressPage().SetCurrentActionText(currentAction);
                setupApp.GetProgressPage().SetProgressBarPosition(percentComplete);
            }
            break;
        case 3: //Installing
            details.LoadStringW(IDS_PROGRESS_INSTALL);
            currentAction.Format(actionTemplate, details);
            setupApp.GetProgressPage().SetCurrentActionText(currentAction);
            setupApp.GetProgressPage().StepProgressBar();
            break;
        case 4: //Rollback
            details.LoadStringW(IDS_PROGRESS_ROLLBACK);
            currentAction.Format(actionTemplate, details);
            setupApp.GetProgressPage().SetCurrentActionText(currentAction);
            setupApp.GetProgressPage().SetProgressBarPosition(setupApp.GetProgressPage().GetProgressBarPosition() - 1);
            break;
        };
        
        return IDOK;
    }

    return 0;
}

//Detecting If Terminal Services is Installed
// code is taken from  
// http://msdn.microsoft.com/en-us/library/cc644951.aspx


/* -------------------------------------------------------------
   Note that the ValidateProductSuite and IsTerminalServices
   functions use ANSI versions of Win32 functions to maintain
   compatibility with Windows 95/98.
   ------------------------------------------------------------- */
void SetTSInInstallMode()
{
    if (IsTerminalServicesEnabled())
    {
        ExecCmd(_T("change user /INSTALL"), FALSE);
    }
}
BOOL IsTerminalServicesEnabled() 
{
  BOOL    bResult = FALSE;
  DWORD   dwVersion;
  OSVERSIONINFOEXA osVersion;
  DWORDLONG dwlCondition = 0;
  HMODULE hmodK32 = NULL;
  HMODULE hmodNtDll = NULL;
  typedef ULONGLONG (WINAPI *PFnVerSetCondition) (ULONGLONG, ULONG, UCHAR);
  typedef BOOL (WINAPI *PFnVerifyVersionA) (POSVERSIONINFOEXA, DWORD, DWORDLONG);
  PFnVerSetCondition pfnVerSetCondition;
  PFnVerifyVersionA pfnVerifyVersionA;

  dwVersion = GetVersion();

  // Are we running Windows NT?

  if (!(dwVersion & 0x80000000)) 
  {
    // Is it Windows 2000 or greater?
    
    if (LOBYTE(LOWORD(dwVersion)) > 4) 
    {
      // In Windows 2000, use the VerifyVersionInfo and 
      // VerSetConditionMask functions. Don't static link because 
      // it won't load on earlier systems.

      hmodNtDll = GetModuleHandleA( "ntdll.dll" );
      if (hmodNtDll) 
      {
        pfnVerSetCondition = (PFnVerSetCondition) GetProcAddress( 
            hmodNtDll, "VerSetConditionMask");
        if (pfnVerSetCondition != NULL) 
        {
          dwlCondition = (*pfnVerSetCondition) (dwlCondition, 
              VER_SUITENAME, VER_AND);

          // Get a VerifyVersionInfo pointer.

          hmodK32 = GetModuleHandleA( "KERNEL32.DLL" );
          if (hmodK32 != NULL) 
          {
            pfnVerifyVersionA = (PFnVerifyVersionA) GetProcAddress(
               hmodK32, "VerifyVersionInfoA") ;
            if (pfnVerifyVersionA != NULL) 
            {
              ZeroMemory(&osVersion, sizeof(osVersion));
              osVersion.dwOSVersionInfoSize = sizeof(osVersion);
              osVersion.wSuiteMask = VER_SUITE_TERMINAL;
              bResult = (*pfnVerifyVersionA) (&osVersion,
                  VER_SUITENAME, dwlCondition);
            }
          }
        }
      }
    }
    else  // This is Windows NT 4.0 or earlier.
    {

      bResult = ValidateProductSuite( "Terminal Server" );
    }
  }

  return bResult;
}


// ==========================================================================
// ValidateProductSuite()
//
// Purpose:
//  Terminal Services detection code for systems running
//  Windows NT 4.0 and earlier.
// ==========================================================================
BOOL ValidateProductSuite (LPSTR lpszSuiteToValidate) 
{
  BOOL fValidated = FALSE;
  LONG lResult;
  HKEY hKey = NULL;
  DWORD dwType = 0;
  DWORD dwSize = 0;
  LPSTR lpszProductSuites = NULL;
  LPSTR lpszSuite;

  // Open the ProductOptions key.

  lResult = RegOpenKeyA(
      HKEY_LOCAL_MACHINE,
      "System\\CurrentControlSet\\Control\\ProductOptions",
      &hKey
  );
  if (lResult != ERROR_SUCCESS)
  {
      goto exit;
  }

  // Determine required size of ProductSuite buffer.

  lResult = RegQueryValueExA( hKey, "ProductSuite", NULL, &dwType, 
      NULL, &dwSize );
  if (lResult != ERROR_SUCCESS || !dwSize)
  {
      goto exit;
  }

  // Allocate buffer.

  lpszProductSuites = (LPSTR) LocalAlloc( LPTR, dwSize );
  if (!lpszProductSuites)
  {
      goto exit;
  }

  // Retrieve array of product suite strings.

  lResult = RegQueryValueExA( hKey, "ProductSuite", NULL, &dwType,
      (LPBYTE) lpszProductSuites, &dwSize );
  if (lResult != ERROR_SUCCESS || dwType != REG_MULTI_SZ)
  {
      goto exit;
  }

  // Search for suite name in array of strings.

  lpszSuite = lpszProductSuites;
  while (*lpszSuite) 
  {
      if (lstrcmpA( lpszSuite, lpszSuiteToValidate ) == 0) 
      {
          fValidated = TRUE;
          break;
      }
      lpszSuite += (lstrlenA( lpszSuite ) + 1);
  }

exit:
  if (lpszProductSuites)
  {
      LocalFree( lpszProductSuites );
  }

  if (hKey)
  {
      RegCloseKey( hKey );
  }

  return fValidated;
}