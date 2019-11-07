using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.BusinessData.MetadataModel;
using Microsoft.Office.Server.Search.Connector;
using Microsoft.Office.Server.Search.Connector.BDC;
using Microsoft.SharePoint.Utilities;

namespace MyFileConnector
{
    /// <summary>
    /// This is the class referenced in your model file as the 'InputUriProcessor'. Its primary responsibility is to
    /// receive an access URL from SharePoint Search, and translate it into the appropriate BCS metadata object. Here is
    /// where you would, in effect, define and enforce your access URL format.
    ///
    /// The design of your metadata model and your crawling strategy will obviously depend on the structure of the data
    /// in your repository. In the MyFileConnector sample, our "repository" is an NTFS file system so we've defined two
    /// Entities in the model: 'MyFolder',and 'MyFile' which are of course just folders and files on an NTFS file
    /// system. As a result, we have chosen to use a simple access URL format in MyFileConnector:
    /// 
    ///     myfile://host/[entity type]/path
    /// 
    /// This access URL format is simply a URL to a folder or file, using the protocol 'myfile' instead of 'file',
    /// where the first segment specifies the entity type with one of the following values:
    /// 
    ///     MyFile
    ///     MyFolder
    /// 
    /// For example:
    /// 
    ///     myfile://host/MyFile/folder1/file.txt
    ///     myfile://host/MyFolder/folder1/subfolder1
    /// 
    /// Our input URL processing then, will simply be to remove the first URL segment and derive the Entity type from
    /// it, then parse the remaining URL into a valid UNC path. We expect that all of the access URLs supplied to us by
    /// Search will resolve to a valid folder or file once translated.
    /// 
    /// For output URL processing, where we translate the BCS metadata object into both access and display URLs, see the
    /// MyFileNamingContainer class.
    /// </summary>
    class MyFileLobUri : LobUri
    {
        /// <summary>
        /// Provide the protocol to the base constructor. This is the protocol you provide to search when you register
        /// the custom connector with SharePoint Search in PowerShell. All of your access URIs must use this protocol so
        /// that Search knows to call your custom connector to crawl them.
        /// </summary>
        public MyFileLobUri()
            : base("myfile")
        {
            //
            // The base class provides access to your model file through the Catalog property. Search is able to map the
            // protocol of your custom connector to your model file, because you provide both when you register your
            // connector in PowerShell.
            //
            // Here, you're accessing the LOB system by the name you supplied in the model file. You'll use this later
            // when processing incoming URLs.
            //
            this.lobSystem = this.Catalog.GetLobSystem("MyFileSystem");
        }

        /// <summary>
        /// This method does the processing of incoming URLs. The intent here is to receive a URL via this context
        /// object, parse it to determine what metadata object it refers to, and then populate members of this class
        /// with this information.
        /// </summary>
        /// <param name="context">
        /// Supplies the connection context. The context contains (most importantly) the URL of the item being crawled,
        /// and also other information about the current crawl.
        /// </param>
        public override void Initialize(
            Microsoft.Office.Server.Search.Connector.IConnectionContext context)
        {

            Uri sourceUri = context.Path;

            //
            // A URL can point to a LobSystem, a LobSystemInstance, an Entity, or a specific instance of an Entity.
            // A specific instance of an Entity for MyFileConnector would be, of course, an actual file or folder on
            // disk. The other BCS metadata objects exist to provide you more flexibility in designing your connector
            // by providing slightly different crawling behaviors (which we'll detail below).
            //
            // Your job in this method is to parse the URL and decide what it's pointing to. Then, populate the
            // appropriate class members to indicate what it's pointing to.
            //
            // To indicate that the supplied URL refers to a(n):         Populate only these class members:
            //
            //                                         LobSystem         this.LobSystem
            // 
            //                                 LobSystemInstance         this.LobSystem
            //                                                           this.LobSystemInstance
            //
            //                                            Entity         this.LobSystem
            //                                                           this.LobSystemInstance
            //                                                           this.Entity
            //
            //                                   Entity instance         this.LobSystem
            //                                                           this.LobSystemInstance
            //                                                           this.Entity
            //                                                           this.Identity
            //
            // What happens when these BCS metadata objects are crawled? Obviously, if you've got an entity instance,
            // you're crawling an actual item in your repository. But what about the others? In general, there's no real
            // data or metadata associated with any of them until you get down to an actual Entity instance (i.e., a
            // file or folder on disk, in the case of this connector). So the only real thing that happens when these
            // objects are crawled is that they emit their children, which will then subsequently be crawled. So here's
            // what the "children" of each of these metadata objects are:
            // 
            //  Metadata object type    Children that are emitted when crawled
            //
            //  LobSystem               All LobSystemInstances in the model file that are marked with the
            //                          'ShowInSearchUI' property.
            //
            //  LobSystemInstance       If at least one Entity defined in this LobSystemInstance has a Finder method
            //                          with the 'RootFinder' property set, the LobSystemInstance will emit all Entities
            //                          that have Finder methods marked with the 'RootFinder' property. If an Entity
            //                          does not have a RootFinder, it will not be emitted.
            //
            //                          If no Entities in the LobSystemInstance with a Finder marked with the
            //                          'RootFinder' property, the LobSystemInstance will emit any Entities that have
            //                          both IdEnumerator and SpecificFinder methods defined in the model file. Note: If
            //                          you define only one of the two (either IdEnumerator or SpecificFinder) in this
            //                          case, an exception will be thrown at crawl time. You must define both to be
            //                          crawled in this manner.
            //
            // (Up to this point, with the previous two metadata objects, we had not yet called into the custom
            // connector shim. Instead, we were able to figure out what to emit from the model file alone. Starting with
            // the Entity, we will be calling into the custom connector shim, and also the full and incremental crawl
            // behavior can be different...)
            //
            // Metadata object type     Children that are emitted when crawled
            //
            // Entity                   Full crawl:
            //                          If the Entity has a Finder with the 'RootFinder' property set, that method is
            //                          called. Whatever entity instances your shim emits are the child items of the
            //                          Entity. If there is no Finder with the 'RootFinder' property set, but there
            //                          is an IdEnumerator defined for the entity, that method will be called, and
            //                          whatever entity instances your shim emits are the child items of the Entity.
            //
            //                          Incremental crawl:
            //                          If the Entity is the source for any AssociationNavigator methods that are marked
            //                          with the 'DirectoryLink' property, the behavior is the same as in the full
            //                          crawl. If there are no AssociationNavigator methods where the Entity is the
            //                          source, marked with the 'DirectoryLink' property, and the Entity has both a
            //                          ChangedIdEnumerator and DeletedIdEnumerator defined, then both of those methods
            //                          will be called in an incremental crawl. If none of the above is true, the
            //                          incremental crawl behavior is the same as full crawl.
            //
            // (Now, for an actual Entity instance (e.g., a file or folder on disk, in this case), it will be helpful to
            // define the concept of 'container'-type Entities. A 'container' type Entity is an Entity that is defined
            // in your model file as the SourceEntity of any AssociationNavigator marked with the 'DirectoryLink'
            // property.)
            //
            // Metadata object type     Children that are emitted when crawled
            //
            // Container-type           Full crawl:
            //  Entity instance         The association navigators for which the Entity type is the SourceEntity are
            //                          called.
            //
            //                          Incremental crawl:
            //                          This is the same as in a full crawl, unless several things are configured to
            //                          enable your shim to more intelligently decide what to emit. Here's what needs
            //                          to be configured to enable this behavior:
            //
            //                          1.  The SpecificFinder of the Entity has to return a field containing the number
            //                              of deleted direct child items in that particular container and this field
            //                              must be identified in the model file by defining the 'DeletedCountField'
            //                              property on the SpecificFinder method instance, where its value must be the
            //                              name of the field that returns the delete count.
            //
            //                          2.  The AssociationNavigator for which this Entity is the SourceEntity must
            //                              include in its return type descriptor a last modified time DateTime field
            //                              and this field must be identified in the model file by defining the
            //                              'LastModifiedTimeStamp' property on the AssociationNavigator method
            //                              instance, where its value must be the name of the fiels that returns the
            //                              last modified DateTime.
            //
            //                          3.  The AssociationNavigator for which this Entity is the SourceEntity must
            //                              have an input filter defined, and that input filter must have the
            //                              'CrawlStartTime' string property defined:
            //
            //                                  <FilterDescriptor Name="LastModifiedFilter" Type="Input">
            //                                      <Properties>
            //                                          <Property Name="CrawlStartTime" Type="String">x</Property>
            //                                      </Properties>
            //                                  </FilterDescriptor>
            //
            //                          4.  The AssociationNavigator for this this Entity is the SourceEntity must
            //                              take a DateTime input parameter, and that parameter must be associated with
            //                              the filter described in #3:
            //
            //                                  <Parameter Name="lastModifiedTime" Direction="In">
            //                                      <TypeDescriptor
            //                                          Name="lastModifiedTime"
            //                                          TypeName="System.DateTime"
            //                                          AssociatedFilter="LastModifiedFilter" />
            //                                  </Parameter>
            //
            //                          If this is the case, then the association navigator is called, and your shim
            //                          will be provided the last modified time to determine what items to return.
            // 
            // Non-container-type       Full and incremental crawl behavior is the same. The SpecificFinder of the
            //  Entity instance         non-container Entity is called. You can also enable caching behavior, if the
            //                          method that emitted this Entity instance has the 'UseClientCachingForSearch'
            //                          property defined on its method instance. If caching is enabled in this way,
            //                          the SpecificFinder is not called, and only the data returned by the method
            //                          that originally emitted this Entity instance (i.e., an AssociationNavigator
            //                          or IdEnumerator, etc.) will be used to index the item. Note: If you return a
            //                          security descriptor in the return type descriptor, identified by the
            //                          'WindowsSecurityDescriptorField' defined on the method instance, then items
            //                          will *not* be cached, regardless of if you set the 'UseClientCachingForSearch'
            //                          property or not. The reason is that the SharePoint Search crawler has a limited
            //                          size for caching an individual item, and security descriptors can regularly
            //                          exceed that size.
            //

            //
            // As we mentioned above, we expect that every URL will resolve to a valid folder or file once we
            // translate it. Because of this, we won't really need to crawl any of the BCS metadata objects (LobSystem,
            // LobSystemInstance or Entity) - we'll only be crawling Entity instances. So based on the above, we know
            // that this method must end up populating all four LobUri properties.
            //

            //
            // We already populated this.lobSystem in the constructor. Now here's this.lobSystemInstance.
            // LobSystemInstances are statically defined in your model file. In this sample, we've only defined one.
            //
            this.lobSystemInstance = this.lobSystem.GetLobSystemInstances()[0].Value;

            //
            // Next, we need to figure out what Entity to which the URL refers to populate this.Entity. So for starters,
            // let's read the first segment to see what entity type we should assign to this.Entity.
            //
            String entityType = sourceUri.Segments[1].Replace("/", "");
            if(entityType.Equals("MyFolder", StringComparison.OrdinalIgnoreCase))
            {
                this.entity = this.Catalog.GetEntity("MyFileConnector", "MyFolder");
            }
            else if (entityType.Equals("MyFile", StringComparison.OrdinalIgnoreCase))
            {
                this.entity = this.Catalog.GetEntity("MyFileConnector", "MyFile");
            }
            else
            {
                throw new Microsoft.BusinessData.Runtime.RuntimeException(String.Format(
                    "Invalid entity type {0} specified in URL {1}",
                    entityType,
                    sourceUri.ToString()));
            }

            //
            // Finally, populate this.identity. Generally, the 'identity' is whatever your repository needs to uniquely
            // identify an Entity instance. In the case of our repository, an NTFS file system, that would conveniently
            // just be the path to the file. So reconstruct the URL as a UNC path, minus the first segment.
            //
            StringBuilder path = new StringBuilder(@"\\");
            path.Append(sourceUri.Host);
            path.Append(@"\");
            for (int i = 2; i < sourceUri.Segments.Length; ++i)
            {
                path.Append(SPHttpUtility.UrlPathDecode(sourceUri.Segments[i].Replace('/', '\\'), false));
            }

            this.identity = new Microsoft.BusinessData.Runtime.Identity(path.ToString());
        }

        private ILobSystem lobSystem;
        public override Microsoft.BusinessData.MetadataModel.ILobSystem LobSystem
        {
            get { return this.lobSystem; }
        }

        private ILobSystemInstance lobSystemInstance;
        public override Microsoft.BusinessData.MetadataModel.ILobSystemInstance LobSystemInstance
        {
            get { return this.lobSystemInstance; }
        }

        private IEntity entity;
        public override Microsoft.BusinessData.MetadataModel.IEntity Entity
        {
            get { return this.entity; }
        }

        private Microsoft.BusinessData.Runtime.Identity identity;
        public override Microsoft.BusinessData.Runtime.Identity Identity
        {
            get { return this.identity; }
        }

        public override Guid PartitionId
        {
            get { throw new NotImplementedException(); }
        }

        private Uri sourceUri;
        public override Uri SourceUri
        {
            get
            {
                return this.sourceUri;
            }
            set
            {
                this.sourceUri = value;
            }
        }
    }
}
