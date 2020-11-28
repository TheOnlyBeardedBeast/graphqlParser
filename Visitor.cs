using System;
using HotChocolate.Language;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;

namespace graphqlParser
{
    public class TypeDefinitionField
    {
        public bool NotNullable { get; set; } = false;
        public bool IsList { get; set; } = false;
        public string Type { get; set; }
        public TypeDefinitionField InnerType { get; set; }
        public string Name { get; set; }
        public List<string> Directives { get; } = new List<string>();
    }
    public class TypeDefinitionItem
    {
        public string Name { get; set; }
        public TypeDefinitionType Type { get; set; }
        public List<TypeDefinitionField> Fields { get; } = new List<TypeDefinitionField>();
        public List<string> InterfaceKeys { get; } = new List<string>();
        public List<string> Directives { get; } = new List<string>();
    }

    public enum TypeDefinitionType
    {
        Interface,
        Type,
        Enum,
        Extension
    }



    public class Visitor
        : SyntaxWalkerBase<DocumentNode, TypeDefinitionItem>
    {
        public List<TypeDefinitionItem> VisitedItems { get; set; }

        public Visitor()
        {
        }

        public override void Visit(
            DocumentNode node,
            TypeDefinitionItem context)
        {
            VisitedItems = new List<TypeDefinitionItem>();
            if (node != null)
            {
                VisitDocument(node, context);
            }
        }

        protected override void VisitDocument(
            DocumentNode node,
            TypeDefinitionItem context)
        {
            VisitMany(node.Definitions, context, VisitDefinition);
        }

        protected virtual void VisitDefinition(
            IDefinitionNode node,
            TypeDefinitionItem context)
        {

            VisitTypeDefinition(node, context);
        }


        protected void VisitTypeDefinition(
            IDefinitionNode node,
            TypeDefinitionItem context)
        {
            switch (node)
            {
                case SchemaDefinitionNode value:
                    VisitSchemaDefinition(value, context);
                    break;
                case DirectiveDefinitionNode value:
                    VisitDirectiveDefinition(value, context);
                    break;
                case ScalarTypeDefinitionNode value:
                    VisitScalarTypeDefinition(value, context);
                    break;
                case ObjectTypeDefinitionNode value:
                    VisitObjectTypeDefinition(value, context);
                    break;
                case InputObjectTypeDefinitionNode value:
                    VisitInputObjectTypeDefinition(value, context);
                    break;
                case InterfaceTypeDefinitionNode value:
                    VisitInterfaceTypeDefinition(value, context);
                    break;
                case UnionTypeDefinitionNode value:
                    VisitUnionTypeDefinition(value, context);
                    break;
                case EnumTypeDefinitionNode value:
                    VisitEnumTypeDefinition(value, context);
                    break;
                // Extension methods
                case SchemaExtensionNode value:
                    VisitSchemaExtension(value, context);
                    break;
                case ScalarTypeExtensionNode value:
                    VisitScalarTypeExtension(value, context);
                    break;
                case ObjectTypeExtensionNode value:
                    VisitObjectTypeExtension(value, context);
                    break;
                case InterfaceTypeExtensionNode value:
                    VisitInterfaceTypeExtension(value, context);
                    break;
                case UnionTypeExtensionNode value:
                    VisitUnionTypeExtension(value, context);
                    break;
                case EnumTypeExtensionNode value:
                    VisitEnumTypeExtension(value, context);
                    break;
                case InputObjectTypeExtensionNode value:
                    VisitInputObjectTypeExtension(value, context);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        protected override void VisitSchemaDefinition(
            SchemaDefinitionNode node,
            TypeDefinitionItem context)
        {
            VisitMany(
                node.Directives,
                context,
                VisitDirective);

            VisitMany(
                node.OperationTypes,
                context,
                VisitOperationTypeDefinition);
        }

        protected override void VisitSchemaExtension(
            SchemaExtensionNode node,
            TypeDefinitionItem context)
        {
            VisitMany(
                node.Directives,
                context,
                VisitDirective);

            VisitMany(
                node.OperationTypes,
                context,
                VisitOperationTypeDefinition);
        }

        protected override void VisitOperationTypeDefinition(
            OperationTypeDefinitionNode node,
            TypeDefinitionItem context)
        {
            VisitNamedType(node.Type, context);
        }

        protected override void VisitDirectiveDefinition(
            DirectiveDefinitionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Arguments, context, VisitInputValueDefinition);
            VisitMany(node.Locations, context, VisitName);
        }

        protected override void VisitScalarTypeDefinition(
            ScalarTypeDefinitionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Directives, context, VisitDirective);
        }

        protected override void VisitScalarTypeExtension(
            ScalarTypeExtensionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitMany(node.Directives, context, VisitDirective);
        }

        protected override void VisitObjectTypeDefinition(
            ObjectTypeDefinitionNode node,
            TypeDefinitionItem context)
        {
            var item = new TypeDefinitionItem { Name = node.Name.ToString(), Type = TypeDefinitionType.Type };

            using (var interfaces = node.Interfaces.GetEnumerator())
            {
                while (interfaces.MoveNext())
                {
                    item.InterfaceKeys.Add(interfaces.Current.Name.Value);
                }
            }

            using (var directives = node.Directives.GetEnumerator())
            {
                while (directives.MoveNext())
                {
                    item.Directives.Add(directives.Current.Name.Value);
                }
            }

            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Interfaces, context, VisitNamedType);
            VisitMany(node.Fields, item, VisitFieldDefinition);

            this.VisitedItems.Add(item);
        }

        protected override void VisitObjectTypeExtension(
            ObjectTypeExtensionNode node,
            TypeDefinitionItem context)
        {
            var item = new TypeDefinitionItem { Name = node.Name.ToString(), Type = TypeDefinitionType.Extension };

            List<string> intfaces = new List<string>();
            using (var interfaces = node.Interfaces.GetEnumerator())
            {
                while (interfaces.MoveNext())
                {
                    intfaces.Add(interfaces.Current.Name.Value);
                }
            }

            item.InterfaceKeys.AddRange(intfaces);
            VisitName(node.Name, context);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Interfaces, context, VisitNamedType);
            VisitMany(node.Fields, item, VisitFieldDefinition);
            this.VisitedItems.Add(item);
        }

        protected override void VisitFieldDefinition(
            FieldDefinitionNode node,
            TypeDefinitionItem context)
        {
            var field = this.TransformTypeField(node.Type, null);
            field.Name = node.Name.ToString();

            context.Fields.Add(field);
            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Arguments, context, VisitInputValueDefinition);
            VisitType(node.Type, context);
            VisitMany(node.Directives, context, VisitDirective);
        }

        protected override void VisitInputObjectTypeDefinition(
            InputObjectTypeDefinitionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Fields, context, VisitInputValueDefinition);
        }

        protected override void VisitInputObjectTypeExtension(
            InputObjectTypeExtensionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Fields, context, VisitInputValueDefinition);
        }

        protected override void VisitInterfaceTypeDefinition(
           InterfaceTypeDefinitionNode node,
           TypeDefinitionItem context)
        {
            var item = new TypeDefinitionItem { Name = node.Name.ToString(), Type = TypeDefinitionType.Interface };
            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Fields, item, VisitFieldDefinition);
            this.VisitedItems.Add(item);
        }

        protected override void VisitInterfaceTypeExtension(
            InterfaceTypeExtensionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Fields, context, VisitFieldDefinition);
        }

        protected override void VisitUnionTypeDefinition(
            UnionTypeDefinitionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Types, context, VisitNamedType);
        }

        protected override void VisitUnionTypeExtension(
            UnionTypeExtensionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Types, context, VisitNamedType);
        }

        protected override void VisitEnumTypeDefinition(
            EnumTypeDefinitionNode node,
            TypeDefinitionItem context)
        {
            var item = new TypeDefinitionItem();
            item.Name = node.Name.ToString();
            item.Type = TypeDefinitionType.Enum;

            VisitName(node.Name, context);
            VisitIfNotNull(node.Description, context, VisitStringValue);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Values, item, VisitEnumValueDefinition);

            VisitedItems.Add(item);
        }

        protected override void VisitEnumValueDefinition(
            EnumValueDefinitionNode node,
            TypeDefinitionItem context)
        {
            context.Fields.Add(new TypeDefinitionField { Name = node.Name.ToString() });
        }

        protected override void VisitEnumTypeExtension(
            EnumTypeExtensionNode node,
            TypeDefinitionItem context)
        {
            VisitName(node.Name, context);
            VisitMany(node.Directives, context, VisitDirective);
            VisitMany(node.Values, context, VisitEnumValueDefinition);
        }

        private static void VisitIfNotNull<T>(
            T node,
            TypeDefinitionItem context,
            Action<T, TypeDefinitionItem> visitor)
            where T : class
        {
            if (node != null)
            {
                visitor(node, context);
            }
        }

        // helpers
        protected TypeDefinitionField TransformTypeField(ITypeNode type, TypeDefinitionField context, bool nullable = false)
        {
            var ctx = context ?? new TypeDefinitionField();

            if (type.IsNonNullType())
            {
                ctx.NotNullable = true;
                return TransformTypeField(type.InnerType(), ctx);
            }

            if (type.IsListType())
            {
                ctx.IsList = true;
                ctx.InnerType = TransformTypeField(type.InnerType(), null);
                return ctx;

            }

            ctx.Type = type.ToString();
            return ctx;
        }
    }


    public class CodeRewriter
    {
        public Dictionary<string, string> ScalarMap = new Dictionary<string, string>(){
            {"ID","Guid"},
            {"String","string"},
            {"Boolean","bool"},
            {"Int","int"},
            {"Float","float"},
            {"Byte","byte"},
            {"Short","short"},
            {"Long","long"} ,
            {"Decimal","decimal"},
            {"Url","Uri"},
            {"DateTime","DateTime"},
            {"Date","DateTime"},
            {"Uuid","Guid"},
            {"Any","object"}
        };

        public List<TypeDefinitionItem> VisitedItems { get; set; }

        public CodeRewriter(List<TypeDefinitionItem> visitedItems)
        {
            this.VisitedItems = visitedItems;
            this.ImplementVisitedItems();
        }

        protected void ImplementVisitedItems()
        {
            VisitedItems.ForEach(item =>
            {
                if ((item.Type == TypeDefinitionType.Type) && item.InterfaceKeys.Count > 0)
                {
                    var fieldsToImplement = VisitedItems.Where(e => item.InterfaceKeys.Contains(e.Name)).SelectMany(e => e.Fields).Where(e => !item.Fields.Any(f => f.Name == e.Name));
                    item.Fields.AddRange(fieldsToImplement);
                }

                if ((item.Type == TypeDefinitionType.Extension) && item.InterfaceKeys.Count > 0)
                {
                    var fieldsToImplement = VisitedItems.Where(e => item.InterfaceKeys.Contains(e.Name)).SelectMany(e => e.Fields).Where(e => !item.Fields.Any(f => f.Name == e.Name));
                    item.Fields.AddRange(fieldsToImplement);

                    var itemToUpdate = VisitedItems.FirstOrDefault(e => e.Name == item.Name && e.Type != item.Type);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.InterfaceKeys.AddRange(item.InterfaceKeys);
                    }
                }

                if (item.Type == TypeDefinitionType.Extension)
                {
                    var itemToUpdate = VisitedItems.FirstOrDefault(e => e.Name == item.Name && e.Type != item.Type);
                    if (itemToUpdate != null)
                    {

                        var fieldsToImplement = item.Fields.Where(e => !itemToUpdate.Fields.Any(f => f.Name == e.Name));
                        itemToUpdate.Fields.AddRange(fieldsToImplement);
                    }
                    else
                    {
                        item.Type = TypeDefinitionType.Type;
                    }

                }

            });
        }

        public string ScopeStart = "{";

        public string ScopeEnd = "}";

        public string Indent = "    ";


        public string InterfaceSuffix = "{ get; set; };";

        public string classSuffix = "{ get; set; };";

        public string EnumSuffix = ",";

        public string Transform()
        {
            var sb = new StringBuilder();

            this.VisitedItems.ForEach(item =>
            {
                switch (item.Type)
                {
                    case TypeDefinitionType.Enum:
                        {
                            this.RenderEnum(sb, item);
                            break;
                        }
                    case TypeDefinitionType.Interface:
                        {
                            this.RenderInterface(sb, item);
                            break;
                        }
                    case TypeDefinitionType.Type:
                        {
                            this.RenderType(sb, item);
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }
                sb.AppendLine();
            });

            this.RenderDbcontext(sb, this.VisitedItems.Where(e => e.Directives.Contains("entity") && e.Type == TypeDefinitionType.Type));

            return sb.ToString();
        }

        protected string RenderFieldType(TypeDefinitionField field)
        {
            return field.IsList ? $"List<{this.RenderFieldType(field.InnerType)}>" : CodeBuilder.ScalarMap.FirstOrDefault(e => e.Key == field.Type).Value ?? field.Type;
        }

        // #region Enums
        protected void RenderEnum(StringBuilder sb, TypeDefinitionItem item)
        {
            sb.AppendLine($"public enum {item.Name}");
            sb.AppendLine(this.ScopeStart);
            item.Fields.ForEach(field => this.RenderEnumField(sb, field));
            sb.AppendLine(this.ScopeEnd);
        }

        protected void RenderEnumField(StringBuilder sb, TypeDefinitionField field)
        {
            sb.AppendLine($"{this.Indent}{CodeBuilder.Capitalize(field.Name)}{this.EnumSuffix}");
        }
        // #endregion Enums
        // #region interfaces
        protected void RenderInterface(StringBuilder sb, TypeDefinitionItem item)
        {
            sb.AppendLine($"public interface {item.Name}");
            sb.AppendLine(this.ScopeStart);
            item.Fields.ForEach(field => this.RenderInterfaceField(sb, field));
            sb.AppendLine(this.ScopeEnd);
        }

        protected void RenderInterfaceField(StringBuilder sb, TypeDefinitionField field)
        {
            sb.AppendLine($"{this.Indent}{this.RenderFieldType(field)} {CodeBuilder.Capitalize(field.Name)} {InterfaceSuffix}");
        }
        // #endregion interfaces
        // #region Type
        protected void RenderType(StringBuilder sb, TypeDefinitionItem item)
        {
            sb.Append($"public class {item.Name}");
            RenderTypeImplements(sb, item);
            sb.AppendLine(this.ScopeStart);
            item.Fields.ForEach(field => this.RenderTypeField(sb, field));
            sb.AppendLine(this.ScopeEnd);
        }

        protected void RenderTypeImplements(StringBuilder sb, TypeDefinitionItem item)
        {
            if (item.InterfaceKeys.Count > 0)
            {
                sb.Append($" : {string.Join(", ", item.InterfaceKeys)}");
            }
            sb.AppendLine();
        }

        protected void RenderTypeField(StringBuilder sb, TypeDefinitionField field)
        {
            sb.AppendLine($"{this.Indent}public {this.RenderFieldType(field)} {CodeBuilder.Capitalize(field.Name)} {InterfaceSuffix}");
        }
        // #endregion
        // #region dbcontext
        protected void RenderDbcontext(StringBuilder sb, IEnumerable<TypeDefinitionItem> items)
        {
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine();

            sb.AppendLine($"public class AppDBContext : DbContext");
            sb.AppendLine(this.ScopeStart);
            foreach(var item in items)
            {
                RenderDbcontextSet(sb, item);
            }
            sb.AppendLine(this.ScopeEnd);
        }

        protected void RenderDbcontextSet(StringBuilder sb, TypeDefinitionItem item)
        {
            sb.AppendLine($"  public DbSet<{item.Name}> {item.Name.Pluralize(true)} {{ get; set; }}");
        }
        // #endregion
    }
    public static class CodeBuilder
    {
        public static Dictionary<string, string> ScalarMap = new Dictionary<string, string>(){
            {"ID","Guid"},
            {"String","string"},
            {"Boolean","bool"},
            {"Int","int"},
            {"Float","float"},
            {"Byte","byte"},
            {"Short","short"},
            {"Long","long"} ,
            {"Decimal","decimal"},
            {"Url","Uri"},
            {"DateTime","DateTime"},
            {"Date","DateTime"},
            {"Uuid","Guid"},
            {"Any","object"}
        };

        public static string Capitalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if(text.Length == 1)
            {
                return char.ToUpper(text[0]).ToString();
            }

            return char.ToUpper(text[0]) + text.Substring(1);
        }



    }
}