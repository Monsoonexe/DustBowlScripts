
namespace HashParser
{
    internal class ClothingInfo
    {
        public long category_hash;
        public long category_hash_dec_signed;
        public string category_hashname;
        public long hash;
        public long hash_dec_signed;
        public string hashname;
        public bool is_multiplayer;
        public string ped_type;

        #region Constructors

        public ClothingInfo()
        {
            // exists
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ClothingInfo (ClothingInfo src)
        {
            // copy constructor
            this.category_hash = src.category_hash;
            this.category_hash_dec_signed = src.category_hash_dec_signed;
            this.category_hashname = src.category_hashname;
            this.hash = src.hash;
            this.hash_dec_signed = src.hash_dec_signed;
            this.hashname = src.hashname;
            this.is_multiplayer = src.is_multiplayer;
            this.ped_type = src.ped_type;
        }

        #endregion Constructors

        public ClothingInfo Clone() => new ClothingInfo(this);
    }
}
