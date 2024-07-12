using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable

namespace Sdcb.FFmpeg.AutoGen.Definitions
{
    internal record FunctionDefinitionBase : IDefinition, ICanGenerateXmlDoc, IObsoletionAware
    {
        public TypeDefinition ReturnType { get; set; }
        public FunctionParameter[] Parameters { get; set; }
        public string ReturnComment { get; set; }
        public string XmlDocument { get; set; }
        public string Name { get; set; }
        public Obsoletion Obsoletion { get; set; }

        public virtual bool Equals(FunctionDefinitionBase other) =>
            other is not null
            && EqualityComparer<TypeDefinition>.Default.Equals(ReturnType, other.ReturnType)
            && Parameters.SequenceEqual(other.Parameters)
            && EqualityComparer<string>.Default.Equals(ReturnComment, other.ReturnComment)
            && EqualityComparer<string>.Default.Equals(XmlDocument, other.XmlDocument)
            && EqualityComparer<string>.Default.Equals(Name, other.Name)
            && EqualityComparer<Obsoletion>.Default.Equals(Obsoletion, other.Obsoletion);

        public override int GetHashCode()
        {
            HashCode hashcode = new();
            hashcode.Add(ReturnType);
            foreach (var item in Parameters) hashcode.Add(item);
            hashcode.Add(ReturnComment);
            hashcode.Add(XmlDocument);
            hashcode.Add(Name);
            hashcode.Add(Obsoletion);
            hashcode.Add(base.GetHashCode());

            return hashcode.ToHashCode();
        }
    }
}