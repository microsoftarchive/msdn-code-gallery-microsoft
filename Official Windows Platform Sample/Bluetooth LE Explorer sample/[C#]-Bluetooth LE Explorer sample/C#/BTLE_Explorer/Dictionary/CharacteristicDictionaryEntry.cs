using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

using BTLE_Explorer.Dictionary.DataParser; 

namespace BTLE_Explorer.Dictionary
{
    // Contains extra information about a characteristic. Used with the CharacteristicDictionary class. 
    public class CharacteristicDictionaryEntry
    {
        #region ------------------------ Properties ------------------------
        // Name for the service. "Unknown Characteristic" by default. 
        public string Name;
        public const string CHARACTERISTIC_MISSING_STRING = "Unknown Characteristic";
        // Uuid. 
        public Guid Uuid;
        // Whether or not this characteristic is meant to be changable or not. 
        public bool IsDefault;
        private bool _changed;
        #endregion // properties

        #region ------------------------ Constructor/Initialize ------------------------
        public void Initialize(Guid uuid, string name = CHARACTERISTIC_MISSING_STRING, bool isDefault = false)
        {
            Uuid = uuid;
            Name = name;
            IsDefault = isDefault;

            // Get the appropriate parser for this Characteristic's Uuid. If it does not exist, will 
            // automatically parse as some type of unknown. 
            var parserLookupTable = BTLE_Explorer.GlobalSettings.ParserLookupTable; 
            if (parserLookupTable.ContainsKey(Uuid))
            {
                ParseBuffer = parserLookupTable[Uuid];
                ReadUnknownAs = ReadUnknownAsEnum.NOT_UNKNOWN; 
                IsParserTypeKnown = true; 
            }
            else
            {
                ReadUnknownAs = ReadUnknownAsEnum.UINT8;
                IsParserTypeKnown = false; 
            }
        }
        #endregion // Constructor/Initialize

        #region ------------------------ Change properties ------------------------
        public void ChangeFriendlyName(string newName)
        {
            if (IsDefault)
            {
                throw new InvalidOperationException("Cannot change friendly name of a default service.");
            }
            Name = newName;
            _changed = true;
        }

        public void ChangeBufferUnknownType(ReadUnknownAsEnum type)
        {
            if (IsParserTypeKnown)
            {
                throw new ArgumentException("Cannot change parser type of a known parse characteristic.");
            }
            ReadUnknownAs = type;
            _changed = true;
        }

        public bool HasChanged()
        {
            bool result = _changed;
            _changed = false;
            return result;
        }
        #endregion // Change Properties

        #region ------------------------ Reading from buffer ------------------------
        public bool IsParserTypeKnown;
        private Func<IBuffer, string> KnownBufferParser; 
        public enum ReadUnknownAsEnum
        {
            HEX,
            UINT8,
            STRING,
            NOT_UNKNOWN
        };
        public ReadUnknownAsEnum ReadUnknownAs;
        public string DisplayReadBufferType
        {
            get
            {
                switch ((uint)ReadUnknownAs)
                {
                    case ((uint)ReadUnknownAsEnum.UINT8):
                        return "UINT8";
                    case ((uint)ReadUnknownAsEnum.HEX):
                        return "HEX";
                    case ((uint)ReadUnknownAsEnum.STRING):
                        return "STRING";
                    case ((uint)ReadUnknownAsEnum.NOT_UNKNOWN):
                        return "SOME KNOWN TYPE"; // UI will end up showing this as collapsed, but will still access the string. 
                    default:
                        throw new ArgumentException("Unknown read type for characteristic.");
                }
            }
        }

        private Func<IBuffer, string> ParseBuffer
        {
            get
            {
                if (IsParserTypeKnown)
                {
                    return KnownBufferParser; 
                }
                switch ((uint)ReadUnknownAs)
                {
                    case ((uint)ReadUnknownAsEnum.UINT8):
                        return BasicParsers.ParseUInt8Multi;
                    case ((uint)ReadUnknownAsEnum.HEX):
                        return BasicParsers.ParseUInt16Multi;
                    case ((uint)ReadUnknownAsEnum.STRING):
                        return BasicParsers.ParseString;
                    case ((uint)ReadUnknownAsEnum.NOT_UNKNOWN):
                        throw new InvalidOperationException("Read type of this characteristic should be known.");
                    default:
                        throw new InvalidOperationException("Unknown read type for characteristic.");
                }
            }
            set
            {
                KnownBufferParser = value; 
            }
        }
        
        public string ParseReadValue(IBuffer inputstream) 
        {
            try
            {
                return ParseBuffer(inputstream);
            }
            catch (Exception ex)   
            {
                Utilities.OnException(ex);
                return ""; 
            }
        }
        #endregion // Reading from Buffer
    }
}
