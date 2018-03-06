namespace EzTrack.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AssetAllocation")]
    public partial class AssetAllocation
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual Order Order { get; set; }
    }
}
