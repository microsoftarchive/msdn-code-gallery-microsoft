using System;

namespace LinqToTerraServerProvider
{
    [Serializable]
    public class InvalidQueryException : System.Exception
    {
        private string message;

        public InvalidQueryException() { }

        public InvalidQueryException(string message)
        {
            this.message = message + " ";
        }

        public override string Message
        {
            get
            {
                return "The client query is invalid: " + message;
            }
        }
    }
}
