using System.ComponentModel;
using Raven.Server.Config.Attributes;
using Raven.Server.Config.Settings;

namespace Raven.Server.Config.Categories
{
    public class PerformanceHintsConfiguration : ConfigurationCategory
    {
        [Description("The size of a document after which it will get into the huge documents collection")]
        [DefaultValue(5)]
        [SizeUnit(SizeUnit.Megabytes)]
        [ConfigurationEntry("Raven/PerformanceHints/Documents/HugeDocumentSize")]
        public Size HugeDocumentSize { get; set; }

        [Description("The maximum size of the huge documents collection")]
        [DefaultValue(100)]
        [ConfigurationEntry("Raven/PerformanceHints/Documents/HugeDocumentsCollectionSize")]
        public int HugeDocumentsCollectionSize { get; set; }

        [Description("The maximum amount of index outputs per document after which we send a performance hint")]
        [DefaultValue(1024)]
        [ConfigurationEntry("Raven/PerformanceHints/Indexing/MaxIndexOutputsPerDocument")]
        public int MaxWarnIndexOutputsPerDocument { get; set; }
    }
}