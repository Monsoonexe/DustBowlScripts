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

        private List<List<ClothingInfo>> Categorize(List<ClothingInfo> clothing)
        {
            var variationCollection = new List<List<ClothingInfo>>();

            string category = clothing[0].category_hashname;

            var categoryItems = clothing
                .Where(item => item.category_hashname == category)
                .ToList();




            return variationCollection;
        }

        private (string Model, string Variation) SplitHashTokens(string hashName)
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
