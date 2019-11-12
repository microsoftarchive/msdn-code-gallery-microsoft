using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using Microsoft.SharePoint.Phone.Application;
using Microsoft.SharePoint.Client;

namespace SPListAppForExportName
{
    [DataContract]
    public class DisplayItemViewModel : DisplayItemViewModelBase
    {
        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for databinding to Display form UI
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public override object this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                base[fieldName] = value;
            }
        }

        public string m_fileUrl;
        public string FileUrl
        {
            get
            {
                if (string.IsNullOrEmpty(m_fileUrl))
                {
                    IListDataProvider p = this.DataProvider;
                    p.LoadItem(this.ID, (LoadItemCompletedEventArgs args) =>
                         {
                             FileUrl = this.DataProvider.SiteUrl + args.Item.File.ServerRelativeUrl;
                         });
                }

                return m_fileUrl;
            }
            set
            {
                m_fileUrl = value;
                RaisePropertyChanged("FileUrl");
            }
        }

        public string m_fileName;
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(m_fileName))
                {
                    IListDataProvider p = this.DataProvider;
                    p.LoadItem(this.ID, (LoadItemCompletedEventArgs args) =>
                    {
                        FileName = args.Item.File.Name;
                    });
                }

                return m_fileName;
            }
            set
            {
                m_fileName = value;
                RaisePropertyChanged("FileName");
            }
        }

        /// <summary>
        /// Intializes the ViewModel properties
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Deletes the current ListItem from SharePoint server
        /// </summary>
        /// <param name="callback"></param>
        public override void DeleteItem()
        {
            base.DeleteItem();
        }
    }
}
