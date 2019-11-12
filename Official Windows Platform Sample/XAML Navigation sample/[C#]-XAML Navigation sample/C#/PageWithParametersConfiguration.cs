using System;

namespace Navigation
{
    //This page is used by the Scenario 2
    public class PageWithParametersConfiguration
    {
        public string Message { get; set; }
        
        private static int _Id { get; set; }
        public int Id 
        { 
            get
            {
                return _Id;
            }
            set { _Id = value; }
        }

        public PageWithParametersConfiguration()
        {
            _Id = _Id + 1;
        }
    }
}
