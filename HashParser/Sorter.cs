using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;

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


        public void Sort()
        {
            EnsureHashnameIsFilled(new HashnameFixer());

            var maleClothing = allClothing.Where(x => x.ped_type == "female")
                .ToList();
            var femaleClothing = allClothing.Where(x => x.ped_type == "male")
                .ToList();

            Categorize(femaleClothing);
            Categorize(maleClothing);
        }

        private void EnsureHashnameIsFilled(HashnameFixer fixer)
        {
            fixer.Fix(allClothing);
        }
        
        private List<List<List<ClothingInfo>>> Categorize(List<ClothingInfo> clothing)
        {
            var categories = new List<List<List<ClothingInfo>>>(8);

            while (clothing.Count > 0)
                categories.Add(CategorizeOnce(clothing));

            return categories;
        }

        private List<List<ClothingInfo>> CategorizeOnce(List<ClothingInfo> clothing)
        {
            string category = clothing[0].category_hashname;

            var categoryItems = clothing
                .Where(item => item.category_hashname == category);

            foreach (var item in categoryItems)
            {
                (string Model, string Variation) = SplitHashTokens(item.hashname);
                Categorize(Model, item);
                clothing.QuickRemove(item);
            }

            var variationCollection = table.Values.ToList();
            table.Clear();

            return variationCollection;
        }

        private void Categorize(string Model, ClothingInfo clothingInfo)
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
            int i = 0;
            int start = 0;

            // look for ..._001
            while (!char.IsDigit(hashName[i++]))
                ;

            // skip over 000_
            i += 3;
            
            string model = hashName.Substring(start, i); // "CLOTHING_ITEM_F_BANDOLIER_000
            string variation = hashName.Substring(i, hashName.Length - i); // _TINT_001

            return (model, variation);
        }

    }
}
