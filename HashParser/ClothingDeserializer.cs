using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashParser
{
    internal class ClothingDeserializer
    {
        public ClothingInfo Deserialize(string line)
        {
            // consume {
            // consume 'hashname=
            // consume string value
            // consume comma

            // consume 'category_hashname=
            // consume string value
            // consume comma

            // consume 'ped_type=
            // consume string value
            // consume comma

            // consume 'is_multiplayer=
            // consume bool value
            // consume comma

            // consume 'category_hash=
            // consume hex value
            // consume comma

            // consume 'hash=
            // consume hex value
            // consume comma

            // consume 'hash_dec_signed=
            // consume long value
            // consume comma

            // consume 'category_hash_dec_signed=
            // consume long value
            // consume }

            // consume comma

            return null;
        }

        public string Serialize(ClothingInfo info)
        {
            return null;
        }
    }
}
