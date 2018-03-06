namespace EzTrack.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Asset")]
    public partial class Asset
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Asset()
        {
            AssetAllocations = new HashSet<AssetAllocation>();
        }

        public int Id { get; set; }

        public int BarcodeValue { get; set; }

        [StringLength(50)]
        public string BarcodeAlias { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssetAllocation> AssetAllocations { get; set; }
    }
}
