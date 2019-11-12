/*
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++DISCLAIMER++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
------------------------------------------------------------------------------------------------------------------------------------------------------------------------
The sample scripts are not supported under any Microsoft standard support program or service.The sample scripts are provided AS IS without warranty
of any kind.Microsoft further disclaims all implied warranties including, without limitation, any implied warranties of merchantability or of fitness for
a particular purpose.The entire risk arising out of the use or performance of the sample scripts and documentation remains with you.In no event shall
Microsoft, its authors, or anyone else involved in the creation, production, or delivery of the scripts be liable for any damages whatsoever(including,
without limitation, damages for loss of business profits, business interruption, loss of business information, or other pecuniary loss) arising out of the use
of or inability to use the sample scripts or documentation, even if Microsoft has been advised of the possibility of such damages
------------------------------------------------------------------------------------------------------------------------------------------------------------------------
*/

//Code written by:
//Suraj U Dixit
//IIS | ASP.NET team

//Filename : Ftp.cpp

#include "Ftp.h"; //Header file which inludes all the variable declaration

//This function asks for input from user reagarding ftp site name, Username, Password, and file to be Uploaded
//Then writes it into a file called commands.bin
//The file commands.bin is given as input to the ftp cli so that each line in the file acts as one command and it will bw executed
//Once all commands are executed and all files are downloaded from ftp server the file commands.bin is deleted
int main()
{
	char FtpSite[100],Pass[100],UserName[100],File_Upload[100],c;
	string Password;
	cout << "Enter ftp site :";
	cin >> FtpSite;
	cout << "Enter username :";
	cin >> UserName;
	Password = getpass("Enter password :", true); //calling function which prints '*' in place of actual password
	strcpy(Pass, Password.c_str()); //copying string to charactor array
	cout << "Enter filename to be Uploaded :";
	cin >> File_Upload;
	FILE * fp = fopen("commands.bin","wb+"); //opening a binary file commands.bin in write and read mode 
	fprintf(fp, "open %s\n", FtpSite); //writing ftp site name to commands.bin
	fprintf(fp, "%s\n", UserName); //writing username to commands.bin
	fprintf(fp, "%s\n", Pass); //writing password to commands.bin
	fprintf(fp, "put %s\n", File_Upload); //writing name of file to be uploaded to commands.bin
	fprintf(fp, "bye\n");
	rewind(fp); //Taking file pointer to the beginning of the file so that file pointer is at the start
	cout << "Contacting FTP site.....\n" << endl;
	system("ftp -s:commands.bin > logfile.txt"); //executing ftp request command and reading commands from commands.bin and writing output to logfile.txt
	fclose(fp); //closing file
	FILE * dump = fopen("logfile.txt","r"); //opening logfile in read mode
	cout << "------------------------------" << endl;
	cout << "Data from the logfile.txt" << endl;
	cout << "------------------------------" << endl;
	while ((c = getc(dump)) != EOF)
	{
		putchar(c);
	}

	cout << "------------------------------\n" << endl;
	system("del commands.bin"); //deletes commands.bin
	system("pause");
}