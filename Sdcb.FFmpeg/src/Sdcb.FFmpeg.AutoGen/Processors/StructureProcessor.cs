using System;
using System.Collections.Generic;
using System.Linq;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using Sdcb.FFmpeg.AutoGen.Definitions;
using Type = CppSharp.AST.Type;

#nullable disable

namespace Sdcb.FFmpeg.AutoGen.Processors
{
    internal class StructureProcessor
    {
        private readonly ASTProcessor _context;

        public StructureProcessor(ASTProcessor context) => _context = context;

        public void Process(TranslationUnit translationUnit)
        {
            foreach (var typedef in translationUnit.Typedefs)
            {
                if (!typedef.Type.TryGetClass(out var @class))
                    continue;

                if (@class.Comment == null && typedef.Comment != null)
                    @class.Comment = typedef.Comment;

                var className = @class.Name;
                MakeDefinition(@class, className);
            }
        }

        private void MakeDefinition(Class @class, string name)
        {
            name = string.IsNullOrEmpty(@class.Name) ? name : @class.Name;

            var definition = _context.Units.OfType<StructureDefinition>().FirstOrDefault(x => x.Name == name);

            if (definition == null)
            {
                definition = new StructureDefinition
                {
                    Name = name,
                    IsUnion = @class.IsUnion,
                    Obsoletion = ObsoletionHelper.CreateObsoletion(@class)
                };
                _context.AddUnit(definition);
            }

            if (@class.Comment != null)
                definition.XmlDocument = @class.Comment?.BriefText;

            if (@class.IsIncomplete || definition.IsComplete) return;

            definition.IsComplete = true;

            var bitFieldNames = new List<string>();
            var bitFieldComments = new List<string>();
            long bitCounter = 0;
            var fields = new List<StructureField>();

            foreach (var field in @class.Fields)
            {
                if (field.IsBitField)
                {
                    bitFieldNames.Add($"{field.Name}{field.BitWidth}");
                    bitFieldComments.Add(field.Comment?.BriefText ?? string.Empty);
                    bitCounter += field.BitWidth;

                    if (bitCounter % 8 == 0)
                    {
                        fields.Add(GetBitField(bitFieldNames, bitCounter, bitFieldComments));
                        bitFieldNames.Clear();
                        bitFieldComments.Clear();
                        bitCounter = 0;
                    }

                    continue;
                }

                var typeName = $"{field.Class.Name}_{field.Name}";
                fields.Add(new StructureField
                {
                    Name = field.Name,
                    FieldType = GetTypeDefinition(field.Type, typeName),
                    XmlDocument = field.Comment?.BriefText,
                    Obsoletion = ObsoletionHelper.CreateObsoletion(field)
                });
            }

            if (bitFieldNames.Any() || bitCounter > 0) throw new InvalidOperationException();

            definition.Fields = fields.ToArray();
        }

        internal TypeDefinition GetTypeDefinition(Type type, string name = null, bool useWrapperForFixedArray = true)
        {
            return type switch
            {
                TypedefType declaration => GetTypeDefinition(declaration.Declaration.Type, name),
                ArrayType { SizeType: ArrayType.ArraySize.Constant } arrayType => useWrapperForFixedArray
                    ? GetFieldTypeForFixedArray(arrayType)
                    : new TypeDefinition() { Name = TypeHelper.GetTypeName(new PointerType(arrayType.QualifiedType)) },
                TagType tagType => GetFieldTypeForNestedDeclaration(tagType.Declaration, name),
                PointerType pointerType => GetTypeDefinition(pointerType, name),
                _ => new TypeDefinition
                {
                    Name = TypeHelper.GetTypeName(type)
                }
            };
        }

        private static StructureField GetBitField(IEnumerable<string> names, long bitCounter, List<string> comments)
        {
            var fieldName = string.Join("_", names);

            var fieldType = bitCounter switch
            {
                8 => "byte",
                16 => "short",
                32 => "int",
                64 => "long",
                _ => throw new NotSupportedException()
            };

            return new StructureField
            {
                Name = fieldName,
                FieldType = new TypeDefinition { Name = fieldType },
                XmlDocument = string.Join(" ", comments.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            };
        }

        private TypeDefinition GetTypeDefinition(PointerType pointerType, string name)
        {
            var pointee = pointerType.Pointee;

            if (pointee is TypedefType typedefType)
                pointee = typedefType.Declaration.Type;

            if (pointee is FunctionType functionType)
                return _context.FunctionProcessor.GetDelegateType(functionType, name);

            var pointerTypeDefinition = GetTypeDefinition(pointee, name);
            return new TypeDefinition { Name = $"{pointerTypeDefinition.Name}*" };
        }

        private TypeDefinition GetFieldTypeForNestedDeclaration(Declaration declaration, string name)
        {
            var typeName = string.IsNullOrEmpty(declaration.Name) ? name : declaration.Name;

            switch (declaration)
            {
                case Class @class:
                    MakeDefinition(@class, typeName);
                    return new TypeDefinition { Name = typeName };
                case Enumeration @enum:
                    _context.EnumerationProcessor.MakeDefinition(@enum, typeName);
                    return new TypeDefinition { Name = typeName };
                default:
                    throw new NotSupportedException();
            }
        }


        private TypeDefinition GetFieldTypeForFixedArray(ArrayType arrayType)
        {
            var elementType = arrayType.Type;
            var elementTypeDefinition = GetTypeDefinition(elementType);

            var fixedSize = (int) arrayType.Size;

            var name = $"{elementTypeDefinition.Name}_array{fixedSize}";

            if (elementType.IsPointer())
                name = $"{TypeHelper.GetTypeName(elementType.GetPointee())}_ptrArray{fixedSize}";

            if (elementType is ArrayType elementArrayType)
            {
                if (elementArrayType.SizeType == ArrayType.ArraySize.Constant)
                {
                    fixedSize /= (int) elementArrayType.Size;
                    name = $"{TypeHelper.GetTypeName(elementArrayType.Type)}_array{fixedSize}x{elementArrayType.Size}";
                }
                else
                    name = $"{TypeHelper.GetTypeName(elementArrayType.Type)}_arrayOfArray{fixedSize}";
            }

            if (_context.IsKnownUnitName(name))
                return new TypeDefinition { Name = name, ByReference = !arrayType.QualifiedType.Qualifiers.IsConst };

            var fixedArray = new FixedArrayDefinition
            {
                Name = name,
                Size = fixedSize,
                ElementType = elementTypeDefinition,
                IsPrimitive = elementType.IsPrimitiveType()
            };
            _context.AddUnit(fixedArray);
            return fixedArray;
        }
    }
}