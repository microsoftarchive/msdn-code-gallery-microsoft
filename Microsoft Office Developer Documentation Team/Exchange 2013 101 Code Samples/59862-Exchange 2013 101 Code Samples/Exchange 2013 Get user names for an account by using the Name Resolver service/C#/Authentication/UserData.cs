using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
  public interface IUserData
  {
    ExchangeVersion Version { get; }
    string EmailAddress { get; }
    SecureString Password { get; }
    Uri AutodiscoverUrl { get; set; }
  }

  public class UserDataFromConsole : IUserData
  {
    private static UserDataFromConsole userData;

    public static IUserData GetUserData()
    {
      if (userData == null)
      {
        GetUserDataFromConsole();
      }

      return userData;
    }

    private static void GetUserDataFromConsole()
    {
      userData = new UserDataFromConsole();

      Console.Write("Enter email address: ");
      userData.EmailAddress = Console.ReadLine();

      userData.Password = new SecureString();

      Console.Write("Enter password: ");

      while (true)
      {
          ConsoleKeyInfo userInput = Console.ReadKey(true);
          if (userInput.Key == ConsoleKey.Enter)
          {
              break;
          }
          else if (userInput.Key == ConsoleKey.Escape)
          {
              return;
          }
          else if (userInput.Key == ConsoleKey.Backspace)
          {
              if (userData.Password.Length != 0)
              {
                  userData.Password.RemoveAt(userData.Password.Length - 1);
              }
          }
          else
          {
              userData.Password.AppendChar(userInput.KeyChar);
              Console.Write("*");
          }
      }

      Console.WriteLine();

      userData.Password.MakeReadOnly();
    }

    public ExchangeVersion Version { get { return ExchangeVersion.Exchange2010_SP2; } }

    public string EmailAddress
    {
        get;
        private set;
    }

    public SecureString Password
    {
        get;
        private set;
    }

    public Uri AutodiscoverUrl
    {
        get;
        set;
    }
  }
}
