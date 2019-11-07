using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.Runtime;
using Microsoft.Office.Server.Search.Connector.BDC;

namespace MyFileConnector
{
    /// <summary>
    /// Both BCS and SharePoint Search have the concept of a unique identifier. In Search, that identifier is the access
    /// URL. In BCS it's the field or fields returned by your shim that are marked as 'Identifiers'.  This is the class
    /// referenced in your model file as the  'OutputUriProcessor', and its primary responsibility is to translate the
    /// BCS 'Identifiers' of BCS objects into access and display URLs that are used by SharePoint Search.
    /// 
    /// So when does this class get used? When we crawl a container-type item (a folder, in the MyFileConnector sample,
    /// or any of the BCS metadata objects like a LobSystem or Entity) we enumerate that container's child items and
    /// send information about those child items to the search gatherer so that they may be subsequently crawled. It is
    /// at this time, when we're actually crawling the parent and enumerating its children, that we use this class to
    /// determine the access and display URL of each child items.
    /// 
    /// For input URL processing, see the MyFileLobUri class.
    /// </summary>
    /// <remarks>
    /// Notes about access URL:
    /// 
    /// The phrase 'access URL' refers to a URL that can be used by your connector to uniquely identify a particular
    /// entity instance or other BCS metadata object. It does not need to be readable by anyone or anything else. The
    /// access URL will start with your custom connector's protocol. Another thing to note is that this translation
    /// (from BCS 'Identifiers' to access URL) must be the inverse of the translation performed in your ILobUri
    /// implementation. For example, this URL:
    /// 
    ///     myfile://host/MyFile/path/file.txt
    /// 
    /// would translate into a MyFileLobUri instance with the LobSystem and LobSystemInstance members populated, the
    /// Entity property containing a 'MyFile' Entity, and an Identity of '\\host\path\file.txt'. This identity would
    /// be used to obtain the EntityInstance containing the data and metadata of that file. If you were then to provide
    /// that EntityInstance to this class, the resulting access URL generated from that instance's BCS 'Identifiers'
    /// should be identical to the one originally supplied.
    /// 
    /// Notes about display URL:
    /// 
    /// The phrase 'display URL' refers to the URL that's returned in search results by SharePoint Search. It should be
    /// a URL that can be accessed in a web browser, and should retrieve some representation of the data contained in
    /// your repository object. In the MyFileConnector example, it's fairly straightforward to perform this translation.
    /// For example, this access URL:
    /// 
    ///     myfile://otherhost/MyFolder/shared
    /// 
    /// would translate to this display URL:
    /// 
    ///     file://otherhost/shared
    /// </remarks>
    class MyFileNamingContainer : INamingContainer
    {
        private Uri sourceUri;
        private Uri accessUri;

        #region INamingContainer Members

        /// <summary>
        /// This defines the crawled property category GUID of any crawled properties emitted by your connector.
        /// </summary>
        private static Guid PropertySetGuid = new Guid("{AC0E43DF-52CF-401f-97BD-912CE683FE1C}");
        public Guid PropertySet
        {
            get { return MyFileNamingContainer.PropertySetGuid; }
        }

        public Guid PartitionId
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// The Initialize method in this class can't really do any processing because it hasn't received any BCS
        /// metadata objects yet.
        /// </summary>
        public void Initialize(
            Uri uri)
        {
            this.sourceUri = uri;
        }

        //
        // This region contains methods that generate access URL and display URL for EntityInstance data objects that
        // are enumerated using association navigators. This is the only type of enumeration that the MyFileConnector
        // sample does.
        //
        #region URL translation methods we need

        /// <summary>
        /// Generate the access URL for an entity instance. This method is used when the instance is being enumerated
        /// by an association navigator (and will therefore have a "parent" entity instance).
        /// </summary>
        /// <param name="entityInstance">
        /// Supplies the entity instance.
        /// </param>
        /// <param name="parentEntityInstance">
        /// Supplies the entity instance that contained the entity instance in whose access URL we're interested.
        /// </param>
        /// <returns>
        /// Returns a Uri containing the access Url.
        /// </returns>
        public Uri GetAccessUri(IEntityInstance entityInstance, IEntityInstance parentEntityInstance)
        {
            //
            // The identifier of an Entity instance is defined in the model file at the Entity level:
            //
            //      <Entity Name="MyFolder" Namespace="MyFileConnector" Version="1.0.0.1">
            //          <Identifiers>
            //              <Identifier Name="PathID" TypeName="System.String" />
            //          </Identifiers>
            //      ...
            //
            // And every method defined for that Entity must mark the field or fields in the return type descriptor that
            // contain the identifier(s). In the MyFileConnector example, the identifier is just the path to the file,
            // which is a field in the return type descriptor of every method defined in the model file for the MyFolder
            // Entity. Here is an example of how it's marked as an 'Identifier' in the return type descriptor of the 
            // GetSubFolders method:
            //
            //      <TypeDescriptor
            //          Name="Path"
            //          TypeName="System.String"
            //          IdentifierEntityNamespace="MyFileConnector"
            //          IdentifierEntityName="MyFolder"
            //          IdentifierName="PathID" />
            //

            //
            // Sanity check that we have an Entity name that we expect.
            //
            if (!entityInstance.Entity.Name.Equals("MyFolder", StringComparison.OrdinalIgnoreCase) &&
                !entityInstance.Entity.Name.Equals("MyFile", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(String.Format(
                    "Invalid Entity type: {0}",
                    entityInstance.Entity.Name));
            }

            //
            // For both the 'MyFile' and 'MyFolder' Entities, the 'Identifier' is a UNC path to the file or folder. Our
            // access URL format is the UNC path in URL format, with the first segment (either 'MyFolder' or 'MyFile')
            // to indicate entity type. So first, read the path from the 'Identifiers'. There's only one Identifier
            // defined in our model file so we can just access the first element of the Identifier values array. Had
            // there been more than one, this array would be in the order in which the identifiers are defined in the
            // 'Identifiers' node of an Entity.
            //
            object[] ids = entityInstance.GetIdentity().GetIdentifierValues();
            String path = ids[0].ToString();

            //
            // Now generate a Uri from the path, and insert the appropriate entity type as the first segment.
            //
            Uri pathUri = new Uri(path);
            StringBuilder pathWithEntityType = new StringBuilder();
            pathWithEntityType.AppendFormat("myfile://{0}/{1}", pathUri.Host, entityInstance.Entity.Name);
            foreach (String segment in pathUri.Segments)
            {
                pathWithEntityType.Append(segment);
            }

            this.accessUri = new Uri(pathWithEntityType.ToString());

            //
            // (Side note and general comment on this class: It's really convenient that our external repository's BCS
            // Identifier happens to be a UNC path, which is something that's easily translated into a URL. However,
            // your BCS Identifier(s) can be anything at all. You have to decide on the mapping from that set of
            // Identifiers (which is what BCS understands) to an access URL (which is what the SharePoint Search
            // gatherer works with) and implement it in this class.)
            //
            return this.accessUri;
        }

        /// <summary>
        /// Generate a display URL from an EntityInstance.
        /// </summary>
        /// <param name="entityInstance">
        /// Supplies the EntityInstance data object.
        /// </param>
        /// <param name="computedDisplayUri">
        /// Supplies the display URL provided by the repository if there was one.
        /// </param>
        /// <returns>
        /// Returns a Uri containing the display URL.
        /// </returns>
        public Uri GetDisplayUri(IEntityInstance entityInstance, string computedDisplayUri)
        {
            //
            // The only time we expect to receive a computed display URL is when the shim itself returns a String field
            // in the return type descriptor containing a display URL. In order to cause the connector framework to use
            // this String field, the 'DisplayUriField' property must be defined on the method instance and the value of
            // the 'DisplayUriField' property must be the name of the String field in the return type descriptor that
            // contains the display URL.
            //
            // However, in this case our shim does not return a display URL, so we always expect this to be empty.
            //
            if (!String.IsNullOrEmpty(computedDisplayUri))
            {
                throw new ArgumentException(String.Format(
                    "Unexpectedly received a computed display URL: {0}",
                    computedDisplayUri));
            }

            //
            // In this case, the 'Path' identifier is simply the UNC path to the file or folder. So return the URL
            // representation of that.
            //
            object[] ids = entityInstance.GetIdentity().GetIdentifierValues();
            String path = ids[0].ToString();

            return new Uri(path);
        }

        #endregion

        //
        // This region contains methods for generating access and display URLs for child items that are enumerated from
        // BCS metadata objects that we don't expect to crawl (e.g. LobSystem, LobSystemInstance, Entity). As a result,
        // they all throw NotImplementedException. If you were to change the crawl strategy of this connector to crawl
        // different BCS metadata objects you would need to implement the GetAccessUri and GetDisplayUri for those
        // metadata object types.
        //
        #region Unused URL translation methods

        /// <summary>
        /// Generate the access URL for an EntityInstance. This method is called when the instance is being enumerated
        /// from the finder method of an entity (i.e., the RootFinder).
        /// </summary>
        public Uri GetAccessUri(IEntityInstance entityInstance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri GetDisplayUri(IEntityInstance entityInstance, IEntityInstance parentEntityInstance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates access URL for an Entity.
        /// </summary>
        public Uri GetAccessUri(IEntity entity, ILobSystemInstance lobSystemInstance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates display URL for an Entity.
        /// </summary>
        public Uri GetDisplayUri(IEntity entity, ILobSystemInstance lobSystemInstance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates access URL for a LobSystemInstance.
        /// </summary>
        public Uri GetAccessUri(ILobSystemInstance lobSystemInstance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates display URL for a LobSystemInstance.
        /// </summary>
        public Uri GetDisplayUri(ILobSystemInstance lobSystemInstance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates access URL for a LobSystem.
        /// </summary>
        public Uri GetAccessUri(ILobSystem lobSystem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates display URL for a LobSystem.
        /// </summary>
        public Uri GetDisplayUri(ILobSystem lobSystem)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


    }
}
