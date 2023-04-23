using System.Collections.Generic;
using System.Linq;

namespace HashParser
{
    /// <summary>
    /// Creates a hashname for a clothing item in case its hashname is empty.
    /// </summary>
    internal class HashnameFixer
    {
        private readonly Dictionary<string, int> quantities = new Dictionary<string, int>();

        public void Fix(IReadOnlyCollection<ClothingInfo> allClothing)
        {
            foreach (var clothing in allClothing
                .Where(c => string.IsNullOrEmpty(c.hashname)))
            {
                string category = clothing.category_hashname.ToUpper();
               
                // handle quantity
                int quantity = GetQuantity(category) + 1; // q's start at 1
                SetQuantity(category, quantity);

                // generate new hash name
                string newHashname = $"{category}_FIXED_000_VAR_{quantity}";


                // finalize
                clothing.category_hashname = newHashname;
            }

            Reset();
        }

        private int GetQuantity(string category)
        {
            if (quantities.TryGetValue(category, out int quantity))
            {
                return quantity;
            }
            else
            {
                quantities.Add(category, 0);
                return 0;
            }
        }

        private void SetQuantity(string category, int quantity)
        {
            quantities[category] = quantity;
        }

        public void Reset()
        {
            quantities.Clear();
        }
    }
}
