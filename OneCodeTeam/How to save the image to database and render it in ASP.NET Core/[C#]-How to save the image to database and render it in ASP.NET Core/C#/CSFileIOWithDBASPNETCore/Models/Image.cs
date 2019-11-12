using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSFileIOWithDBASPNETCore.Models
{
    public class Image
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }

        public int Length { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ContentType { get; set; }

    }
}
