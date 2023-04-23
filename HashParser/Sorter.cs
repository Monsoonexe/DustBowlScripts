using System.Collections.Generic;
using System.Linq;

namespace HashParser
{
    internal class Sorter
    {
        #region Constants

        #endregion Constants

        private readonly IReadOnlyCollection<ClothingInfo> allClothing;
        private readonly Dictionary<string, Dictionary<string, ClothingInfo[][]>> clothes_list = new Dictionary<string, Dictionary<string, ClothingInfo[][]>>();

        Dictionary<string, List<ClothingInfo>> table = new Dictionary<string, List<ClothingInfo>>();

        #region Constructors

        public Sorter(List<ClothingInfo> clothingInfos)
        {
            allClothing = clothingInfos;
        }

        #endregion Constructors
            // sex
            //  category_hashname (hat, pants, shoes)
            //      model (model 1, model 2)
            //          color variation (model 1 blue, model 2 red)
            //


        public List<List<List<List<ClothingInfo>>>> Sort()
        {
            EnsureHashnameIsFilled(new HashnameFixer());

            var femaleClothing = allClothing.Where(x => x.ped_type == "female")
                .ToList();
            var maleClothing = allClothing.Where(x => x.ped_type == "male")
                .ToList();

            return new List<List<List<List<ClothingInfo>>>>
            {
                CategorizeCategory(femaleClothing),
                CategorizeCategory(maleClothing)
            };
        }

        private void EnsureHashnameIsFilled(HashnameFixer fixer)
        {
            fixer.Fix(allClothing);
        }
        
        private List<List<List<ClothingInfo>>> CategorizeCategory(List<ClothingInfo> clothing)
        {
            var categories = new List<List<List<ClothingInfo>>>(8);

            while (clothing.Count > 0)
            {
                categories.Add(CategorizeModel(clothing));
            }

            return categories;
        }

        private List<List<ClothingInfo>> CategorizeModel(List<ClothingInfo> clothing)
        {
            string category = clothing[0].category_hashname;

            var categoryItems = clothing
                .Where(item => item.category_hashname == category)
                .ToList(); // because of removal

            foreach (var item in categoryItems)
            {
                (string Model, string Variation) = SplitHashTokens(item.hashname);
                CategorizeVariation(Model, item);
                clothing.QuickRemove(item);
            }

            var variationCollection = table.Values.ToList();
            table.Clear();

            return variationCollection;
        }

        private void CategorizeVariation(string Model, ClothingInfo clothingInfo)
        {
            if (!table.TryGetValue(Model, out List<ClothingInfo> variations))
            {
                variations = new List<ClothingInfo>(4);
                table.Add(Model, variations);
            }

            variations.Add(clothingInfo);
        }

        private static (string Model, string Variation) SplitHashTokens(string hashName)
        {
            int length = hashName.Length;
            int i = 0;
            int start = 0;

            // look for ..._001
            while (!char.IsDigit(hashName[i++]) && i < length)
                ;

            // check for no model number
            if (i >= length)
                return (hashName, string.Empty);

            // skip over 000_
            i += 3;

            // check for no variations
            if (i >= length)
                return (hashName, string.Empty);
            
            string model = hashName.Substring(start, i); // "CLOTHING_ITEM_F_BANDOLIER_000
            string variation = hashName.Substring(i, length - i); // _TINT_001

            return (model, variation);
        }

    }
}
