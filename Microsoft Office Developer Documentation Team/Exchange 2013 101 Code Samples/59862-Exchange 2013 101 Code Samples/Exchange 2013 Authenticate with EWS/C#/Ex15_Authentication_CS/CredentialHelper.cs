using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only.
    // Before you run this sample, make sure that the code meets the
    // coding requirements of your organization.

    static class CredentialHelper
    {
        // References:
        // http://msdn.microsoft.com/en-us/library/aa480470.aspx

        /// <summary>
        /// Accepts a user credentials if they haven't been saved or if they have changed, adds them to the credential cache, and makes
        /// a call to EWS to verify that the credentials work. If the credentials work, then a confirmation is sent to the credential
        /// manager and the credentials are provided back to the application to be used to make EWS calls.
        /// </summary>
        internal static void AppLogin(ref ExchangeService service)
        {
            try
            {
                // Create the credential prompt that will handle the user credentials. 
                CredPrompt prompt = new CredPrompt("Exchange Web Services");
                DialogResult result;

                // Show the prompt. This will either prompt for the user credentials or, if they 
                // already exist on the system, it will load credentials from the cache in the CredDialog object.
                result = prompt.ShowPrompt();
                if (result == DialogResult.OK)
                {
                    // Authenticate the credentials and if they work, confirm the credentials and provide the 
                    // ExchangeService object back to the application.  
                    if (Authenticate(prompt.Name, prompt.Password, ref service))
                    {
                        // The credentials were authenticated. Confirm that we want the 
                        // credential manager to save our credentials.
                        if (prompt.SaveChecked)
                            prompt.Confirm(true);
                    }

                    // The credentials were not authenticated.
                    else
                    {
                        try
                        {
                            prompt.Confirm(false);
                        }
                       
                        catch (ApplicationException)
                        {
                            // Prompt user for credentials again. We may need to provide a way to break 
                            Console.Write("Would you like to retry connecting to EWS? [Y/N]: ");

                            while (true)
                            {
                                ConsoleKeyInfo userInput = Console.ReadKey();
                                Console.WriteLine();
                                if (userInput.Key == ConsoleKey.Y)
                                    AppLogin(ref service);
                                else if (userInput.Key == ConsoleKey.N)
                                {
                                    Environment.Exit(0);   
                                }
                                else
                                    Console.WriteLine("Please press 'Y' or 'N'");
                            }
                        }
                    }
                }
                
                else if (result != DialogResult.Cancel)
                {
                    throw new ApplicationException("Unhandled condition.");
                }
            }

            // This will capture errors stemming from CredUICmdLinePromptForCredentials.
            catch(ApplicationException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }


        /// <summary>
        /// Authenticates using the credentials provided to the credential manager. It also 
        /// populates the ExchangeService object used by the application.
        /// </summary>
        /// <param name="userprincipalname">The user's name for accessing Exchange</param>
        /// <param name="password">The user's password.</param>
        /// <param name="service">The ExchangeService object for this application.</param>
        /// <returns>A value of true indicates that the client successfully authenticated with EWS.</returns>
        private static bool Authenticate(string userprincipalname, SecureString password, ref ExchangeService service) 
        {
            bool authenticated = false;
            try
            {
                service.Credentials = new NetworkCredential(userprincipalname, password);

                // Check if we have the service URL.
                if (service.Url == null)
                {
                    Console.WriteLine("Using Autodiscover to find EWS URL for {0}. Please wait... ", userprincipalname);
                    service.AutodiscoverUrl(userprincipalname, Service.RedirectionUrlValidationCallback);
                    Console.WriteLine("Autodiscover complete.");
                }

                // Once we have the URL, try a ConvertId operation to check if we can access the service. We expect that
                // the user will be authenticated and that we will get an error code due to the invalid format. Expect a
                // ServiceResponseException.
                Console.WriteLine("Attempting to connect to EWS...");
                AlternateIdBase response = service.ConvertId(new AlternateId(IdFormat.EwsId, "Placeholder", userprincipalname), IdFormat.EwsId);
            }

            // The user principal name is in a bad format.
            catch (FormatException fe)
            {
                Console.WriteLine("Error: {0} Please enter your credentials in UPN format.", fe.Message);
                //service = null;
            }

            catch (AutodiscoverLocalException ale)
            {
                Console.WriteLine("Error: {0}", ale.Message);
                //service = null;
            }

            // The credentials were authenticated. We expect this exception since we are providing intentional bad data for ConvertId
            catch (ServiceResponseException)
            {
                Console.WriteLine("Successfully connected to EWS.");
                authenticated = true;
            }

            // The credentials were not authenticated.
            catch (ServiceRequestException)
            {
                throw new ApplicationException("The credentials were not authenticated.");
                //service = null;
            }

           return authenticated;

        }
    }

    /// <summary>Encapsulates the Credential Management API functionality for the the command line.</summary>
    public sealed class CredPrompt
    {
        public CredPrompt(string target)
        {
            this.Target = target;
        }

        private string _name = String.Empty;
        private bool _saveChecked = false;
        private SecureString _password = new SecureString();
        private string _target = "Exchange Web Services";

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > CREDMGMTUI.MAX_USERNAME_LENGTH)
                    {
                        string message = String.Format(
                            Thread.CurrentThread.CurrentUICulture,
                            "The name has a maximum length of {0} characters.",
                            CREDMGMTUI.MAX_USERNAME_LENGTH);
                        throw new ArgumentException(message, "Name");
                    }
                }
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the credentials.
        /// </summary> 
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > CREDMGMTUI.MAX_PASSWORD_LENGTH)
                    {
                        string message = String.Format(
                            Thread.CurrentThread.CurrentUICulture,
                            "The password has a maximum length of {0} characters.",
                            CREDMGMTUI.MAX_PASSWORD_LENGTH);
                        throw new ArgumentException(message, "Password");
                    }
                }
                _password = value;
            }
        }

        /// <summary>
        /// Gets or sets if the save checkbox status.
        /// </summary>
        public bool SaveChecked
        {
            get
            {
                return _saveChecked;
            }
            set
            {
                _saveChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the target for the credentials.
        /// </summary>
        public string Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("The target cannot be a null value.", "Target");
                }
                else if (value.Length > CREDMGMTUI.MAX_GENERIC_TARGET_LENGTH)
                {
                    string message = String.Format(
                        Thread.CurrentThread.CurrentUICulture,
                        "The target has a maximum length of {0} characters.",
                        CREDMGMTUI.MAX_GENERIC_TARGET_LENGTH);
                    throw new ArgumentException(message, "Target");
                }
                _target = value;
            }
        }

        /// <summary>
        /// Calls the CredUIConfirmCredentials function which confirms that the credentials
        /// provided for the target should be kept in the case they are persisted.</summary>
        /// <param name="value">A value of true if the credentials should be persisted.</param>
        public void Confirm(bool value)
        {
            switch (CREDMGMTUI.ConfirmCredentials(this.Target, value))
            {
                case CREDMGMTUI.ReturnCodes.NO_ERROR:
                    break;

                case CREDMGMTUI.ReturnCodes.ERROR_INVALID_PARAMETER:
                    break;

                default:
                    throw new ApplicationException("Credential confirmation failed.");
            }
        }

        /// <summary>
        /// Calls the CredUICmdLinePromptForCredentials function that will either prompt for
        /// credentials or get the stored credentials for the specified target.
        /// </summary>
        /// <returns>The DialogResult that indicates the results of using CredUI.</returns>
        /// <remarks>
        /// Sets the user name, password and persistence state of the dialog.
        /// </remarks>
        internal DialogResult ShowPrompt()
        {
            StringBuilder name = new StringBuilder(CREDMGMTUI.MAX_USERNAME_LENGTH);
            StringBuilder pwd = new StringBuilder(CREDMGMTUI.MAX_PASSWORD_LENGTH);
            int saveChecked = Convert.ToInt32(this.SaveChecked);
            CREDMGMTUI.FLAGS flags = GetFlags();

            Console.WriteLine("Enter your Exchange Online user principal name for the Exchange Web Services stored credential (user@domain.com)...");

            lock (this)
            {
                // Call CredUi.dll to prompt for credentials. If the credentials for the target have already
                // been saved in the credential manager, then this method will return the credentials 
                // and the UI will not be displayed. 
                CREDMGMTUI.ReturnCodes code = CREDMGMTUI.CredUICmdLinePromptForCredentials(
                    this.Target,
                    IntPtr.Zero, 0,
                    name, CREDMGMTUI.MAX_USERNAME_LENGTH,
                    pwd, CREDMGMTUI.MAX_PASSWORD_LENGTH,
                    ref saveChecked,
                    flags
                    );

                // Convert the password returned by the credential manager to a secure string. 
                this.Password = ConvertToSecureString(pwd);

                // Get the user name stored in the credential manager.
                this.Name = name.ToString();

                // Get the value that indicates whether the credentials are persisted 
                // in the credential manager.
                this.SaveChecked = Convert.ToBoolean(saveChecked);

                return GetCredPromptResult(code);
            }
        }

        /// <summary>
        /// Converts a string to a SecureString object.
        /// </summary>
        /// <param name="password">The unprotected password returned by CredUI.</param>
        /// <returns>The password as a SecureString object.</returns>
        private static SecureString ConvertToSecureString(StringBuilder password)
        {
            SecureString securePassword;

            if (password == null)
                throw new ArgumentNullException("password");

            securePassword = new SecureString();
            for (int i = 0; i < password.Length; i++)
            {
                securePassword.AppendChar(password[i]);
                password[i] = '\x0000';
            }

            securePassword.MakeReadOnly();
            password.Clear();
            return securePassword;
        }

        /// <summary>
        /// Provides the flags that specify how the Windows credential manager handles the credentials.
        /// </summary>
        private CREDMGMTUI.FLAGS GetFlags()
        {
            CREDMGMTUI.FLAGS flags = CREDMGMTUI.FLAGS.GENERIC_CREDENTIALS |
                                     CREDMGMTUI.FLAGS.EXPECT_CONFIRMATION |
                                     CREDMGMTUI.FLAGS.EXCLUDE_CERTIFICATES;
            return flags;
        }

        /// <summary>
        /// Returns a DialogResult based on the code returned by the credential manager.
        /// </summary>
        /// <param name="code">The credential return code provided by the credential manager.</param>
        private DialogResult GetCredPromptResult(CREDMGMTUI.ReturnCodes code)
        {
            DialogResult result;
            switch (code)
            {
                case CREDMGMTUI.ReturnCodes.NO_ERROR:
                    result = DialogResult.OK;
                    break;

                case CREDMGMTUI.ReturnCodes.ERROR_CANCELLED:
                    result = DialogResult.Cancel;
                    break;

                case CREDMGMTUI.ReturnCodes.ERROR_NO_SUCH_LOGON_SESSION:
                    throw new ApplicationException("The credential manager cannot be used.");

                case CREDMGMTUI.ReturnCodes.ERROR_INVALID_PARAMETER:
                    throw new ApplicationException(@"Invalid parameter. See http://msdn.microsoft.com/en-us/library/windows/desktop/aa375177(v=vs.85).aspx");

                case CREDMGMTUI.ReturnCodes.ERROR_INVALID_FLAGS:
                    throw new ApplicationException("Invalid flags.");

                default:
                    throw new ApplicationException("The credential manager returned an unknown return code.");
            }
            return result;
        }
    }


    public sealed class CREDMGMTUI
    {
        private CREDMGMTUI() { }

        public const int MAX_USERNAME_LENGTH = 100;
        public const int MAX_PASSWORD_LENGTH = 100;
        public const int MAX_GENERIC_TARGET_LENGTH = 100;

        /// <summary>
        /// Provides the flags that specify how the Windows credential manager handles the credentials.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa375177(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum FLAGS
        {
            EXCLUDE_CERTIFICATES = 0x8,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
        }

        /// <summary>
        /// Provides the return code provided by the credential manager.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa375177(v=vs.85).aspx
        /// </summary>
        public enum ReturnCodes
        {
            NO_ERROR = 0,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
        }

        /// <summary>
        /// Confirms that the credentials harvested by CredUICmdLinePromptForCredentialsW are valid and should be persisted.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa375173(v=vs.85).aspx
        /// </summary>
        [DllImport("credui.dll", EntryPoint = "CredUIConfirmCredentialsW", CharSet = CharSet.Unicode)]
        public static extern ReturnCodes ConfirmCredentials(
            string targetName,
            bool confirm
            );

        /// <summary>
        /// Provides access to the Windows command line interface for storing user credentials.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa375171(v=vs.85).aspx
        /// </summary>
        [DllImport("credui.dll", EntryPoint = "CredUICmdLinePromptForCredentialsW", CharSet = CharSet.Unicode)] 
        public static extern ReturnCodes CredUICmdLinePromptForCredentials(
            string targetName,
            IntPtr reserved1,
            int iError,
            StringBuilder userName,
            int maxUserName,
            StringBuilder password,
            int maxPassword,
            ref int iSave,
            FLAGS flags
            );
    }
}
