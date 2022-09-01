﻿#nullable disable

namespace Sdcb.FFmpeg.AutoGen.Definitions
{
    internal class EnumerationItem : ICanGenerateXmlDoc
    {
        public string Name { get; init; }
        public string RawName { get; init; }
        public string Value { get; init; }
        public string XmlDocument { get; set; }
    }
}
