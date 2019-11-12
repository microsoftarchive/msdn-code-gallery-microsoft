using System.Linq;

namespace Microsoft.Exchange.Samples.Autodiscover
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure 
    // that the code meets the coding requirements of your organization. 
    class CommandLineArgs
    {
        private const int requiredArgsCount = 1;

        public bool IsValid { get; set;  }
        public string EmailAddress { get; set;  }
        public bool UseSoapAutodiscover { get; set;  }
        public string AuthenticateAsUser { get; set; }

        public CommandLineArgs(string [] arguments)
        {
            // Defaults.
            this.IsValid = true;
            this.EmailAddress = "";
            this.AuthenticateAsUser = "";
            this.UseSoapAutodiscover = true;

            if (arguments != null && arguments.Count() >= requiredArgsCount)
            {
                this.IsValid = true;
                this.EmailAddress = arguments[0];
                if (ValidateEmailAddress(this.EmailAddress))
                {
                    for (int i = 1; i < arguments.Count(); i++)
                    {
                        switch (arguments[i].ToUpper())
                        {
                            case "-SKIPSOAP":
                                this.UseSoapAutodiscover = false;
                                break;
                            case "-AUTH":
                                if (i + 1 < arguments.Count())
                                {
                                    this.AuthenticateAsUser = arguments[++i];
                                    if (!ValidateEmailAddress(this.AuthenticateAsUser))
                                    {
                                        this.IsValid = false;
                                    }
                                }
                                else
                                {
                                    this.IsValid = false;
                                }
                                break;
                            default:
                                this.IsValid = false;
                                break;
                        }
                    }
                }
                else
                {
                    this.IsValid = false;
                }
            }
            else
            {
                this.IsValid = false;
            }
        }

        // ValidateEmailAddress
        //   This function validates an email address. The address is valid if it
        //   contains an at symbol (@) and a period (.).
        //
        // Parameters:
        //   emailAddress: The email address to validate.
        // Returns:
        //   True if the email address is valid; otherwise, false.
        //
        private bool ValidateEmailAddress(string emailAddress)
        {
            // Verify that there is an at symbol (@) and a period (.).
            if (emailAddress.Contains('@') && emailAddress.Contains('.'))
                return true;

            return false;
        }
    }
}
