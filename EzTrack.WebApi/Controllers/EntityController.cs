using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EzTrack.Data;

namespace EzTrack.WebApi.Controllers
{
    public class EntityController : ApiController
    {
        public List<Order> GetOrders()
        {
            using (var db = new EzTrackContext())
            {
                return db.Orders.ToList();
            }
        }

        [HttpPost]
        public void PostOrder([FromBody]OrderAsset orderAsset)
        {
            var assetWithId = new Dictionary<int, string>();
            var assetAllocations = new List<AssetAllocation>();
            using (var db = new EzTrackContext())
            {
                var order = new Order
                {
                    OrderName = orderAsset.OrderName,
                    PickupDate = orderAsset.PickupDate,
                    Status = orderAsset.Status
                };
                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var item in orderAsset.ScannedAssets)
                {
                    var id = db.Assets.FirstOrDefault(x => x.BarcodeAlias == item).Id;
                    assetWithId.Add(id, item);
                }

                foreach (var item in assetWithId)
                {
                    var assetAllocation = new AssetAllocation
                    {
                        OrderId = order.Id,
                        AssetId = item.Key
                    };
                    assetAllocations.Add(assetAllocation);
                }

                db.AssetAllocations.AddRange(assetAllocations);
                db.SaveChanges();
            }
        }

        [HttpGet]
        public string LookupAsset(int barcodeValue)
        {
            using (var db = new EzTrackContext())
            {
                return db.Assets.FirstOrDefault(x => x.BarcodeValue == barcodeValue)?.BarcodeAlias;
            }
        }
    }

    public class OrderAsset
    {
        public string OrderName { get; set; }
        public DateTime PickupDate { get; set; }
        public string Status { get; set; }
        public List<string> ScannedAssets { get; set; }
    }


}
