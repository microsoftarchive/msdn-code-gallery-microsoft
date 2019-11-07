// FileAcessPermissions.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <io.h>
//****************************** Module Header ******************************\
    //Module Name:    FileAcessPermissions.cpp
    //Project:        FileAcessPermissions
    //Copyright (c) Microsoft Corporation

    // The project illustrates how to check whether a file is in use or not.

    //This source is subject to the Microsoft Public License.
    //See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
    //All other rights reserved.

//*****************************************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <sys/types.h>
#include <sys/stat.h>
/* You can try other files and directories */
char obj[50] = "c:\\testfolder\\robots.txt";
char newobj[50] = "c:\\testfolder\\human.txt";

int _tmain(int argc, _TCHAR* argv[])
{
	int result;
       /* Check for existence */
       printf("Check the file existence...\n");
       if ((_access(obj, 0)) != -1)
             printf("%s file exists\n", obj);
       else
             printf("%s file does not exist lol!\n", obj);
       
       //Check for read / write permission 
       printf("\nCheck for read/write permission...\n");
       if ((_access(obj, 2)) != -1)
             printf("%s file has write permission\n", obj);
       if ((_access(obj, 4)) != -1)
             printf("%s file has read permission\n", obj);
       if ((_access(obj, 6)) != -1)
             printf("%s file has write and read permission\n\n", obj);

       //Make a file read - only 
       printf("\nMake file read-only...\n");
       if (_chmod(obj, _S_IREAD) == -1)
             perror("File not found lol!");
       else
       {
             printf("The file mode is changed to read-only\n");
             _chmod(obj, _S_IREAD);
       }
       if ((_access(obj, 4)) != -1)
             printf("%s file has read permission\n", obj);


       /* Change back to read/write */
       printf("\nChange back to read/write...\n");
       if (_chmod(obj, _S_IWRITE) == -1)
             perror("file not found lol!");
       else
       {
             printf("The file\'s mode is changed to read/write\n");
             _chmod(obj, _S_IWRITE);
       }
       if ((_access(obj, 2)) != -1)
             printf("%s file has write permission\n", obj);

       /* Attempt to rename the file */
       printf("\nAttempt to rename the file...\n");
       result = rename(obj, newobj);
       if (result != 0)
             printf("Could not rename %s\n", obj);
       else
             printf("%s file has been renamed to\n %s\n", obj, newobj);

       /* remove the created file */
       printf("\nRemoving the created file...\n");
       if (remove(newobj) == -1)
             printf("Could not delete %s lol!\n", newobj);
       else
             printf("Ooops! %s file has been deleted lol!\n", newobj);
       return 0;


}

