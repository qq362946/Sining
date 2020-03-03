namespace ExcelToCS
{
    public class TemplateConst
    {
        public const string ConfigCsTemplateHead =
            "using MongoDB.Bson;\nusing MongoDB.Bson.Serialization.Attributes;\n\nnamespace Sining.Config\n{";
        public const string ConfigDataTemplateHead =
            "namespace Sining.Config\n{";

        //public const string ConfigCsClassBsonKnownTypes = "[BsonKnownTypes(typeof(IConfig))]";
        public const string ConfigCsClassName = "\tpublic partial class {0} : IConfig, IObject\n\t{{";
        public const string ConfigCsClassId = "\t\tpublic int Id { get; set; }";
        public const string FieldByFloat = "\t\t[BsonRepresentation(BsonType.Double, AllowTruncation = true)]";
        public const string FieldByBsonDefaultValue = "\t\t[BsonDefaultValue({0})]\n";
        public const string FieldByPublic = "\t\tpublic {0} {1} {{ get; set; }}\n";

        public const string ConfigCsCategoryAttribute = "\t[Config]";
        public const string ConfigCsCategoryClassName = "\tpublic partial class {0}Data : AConfig<{0}>";

        public const string ConfigCsCategoryInstanceName =
            "\t\tpublic static {0}Data Instance {{ get; private set; }}\n\n\t\tpublic {0}Data()\n\t\t{{\n\t\t\tInstance = this;\n\t\t}}";
    }
}