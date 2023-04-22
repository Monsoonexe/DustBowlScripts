using System.Collections.Generic;

namespace HashParser
{
    internal class ClothingParser
    {
        private readonly List<string> source;
        private readonly List<ClothingInfo> clothingInfos;

        public ClothingParser(List<string> source) 
        {
            this.source = source;
            clothingInfos = new List<ClothingInfo>(source.Count);
        }

        public List<ClothingInfo> Parse()
        {
            // TODO -

            return clothingInfos;
        }
    }
}
