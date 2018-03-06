using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EzTrack.Droid.Model
{
    public class OrderAsset
    {
        public string OrderName { get; set; }
        public DateTime PickupDate { get; set; }
        public string Status { get; set; }
        public List<string> ScannedAssets { get; set; }
    }
}