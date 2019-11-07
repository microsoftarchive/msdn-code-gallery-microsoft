// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

namespace Doto.Model
{
    /// <summary>
    /// A class used to store information about users in Windows Azure Mobile Service
    /// </summary>
    [DataContract(Name = "profiles")]
    public class Profile : ViewModel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "userId")]
        public string UserId {get; set;}

        [DataMember(Name = "created")]
        public string CreatedDate {get; set;}
            
        [DataMember(Name = "imageUri")]
        public string ImageUri {get; set;}

        // Name, City & State are part of the two way binding hence raising the property changed event.
        private string name;

        [DataMember(Name = "name")]
        public string Name
        {
            get { return name; }
            set { SetValue(ref name, value); }
        }

        private string city;

        [DataMember(Name = "city")]
        public string City
        {
            get { return city; }
            set { SetValue(ref city, value); }
        }

        private string state;

        [DataMember(Name = "state")]
        public string State
        {
            get { return state; }
            set { SetValue(ref state, value); }
        }
    }
}
