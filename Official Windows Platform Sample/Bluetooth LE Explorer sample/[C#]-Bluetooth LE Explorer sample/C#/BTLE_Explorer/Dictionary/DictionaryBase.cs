using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary
{
    public abstract class DictionaryBase<TValue> : Dictionary<Guid, TValue>
    {
        public abstract Task LoadDictionaryAsync();

        public abstract Task SaveDictionaryAsync();
        
        public abstract void AddLoadedEntry(TValue input);

        public TValue Get(Guid guid)
        {
            return this[guid]; 
        }

        public async Task ClearDictionaryAsync()
        {
            Clear();
            await SaveDictionaryAsync();
        }

        protected async Task SerializeAndWriteFileAsync(string filename)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (Stream fileStream = await file.OpenStreamForWriteAsync())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<TValue>));

                serializer.Serialize(fileStream, this.Values.ToList());
            }
        }

        protected async Task ReadFileAndDeserializeIfExistsAsync(string filename)
        {
            List<TValue> list;

            // Try to open up our file, if it exists. 
            try
            {
                using (Stream fileStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename))
                {
                    // Serialize the info from the file.
                    XmlSerializer serializer = new XmlSerializer(typeof(List<TValue>));
                    list = (List<TValue>)serializer.Deserialize(fileStream);
                }
            }
            catch (FileNotFoundException)
            {
                // If this is the first time the application has been run, we may have not initialized any unknown
                // characteristics/services, so dictionaries (and thus, their files) may not have been created yet.
                return; 
            }

            // Load the entry. 
            foreach (TValue item in list)
            {
                AddLoadedEntry(item);
            }
        }
    }
}
