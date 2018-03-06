using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Android.Support.V4;
using Android.Support.V7.Widget;
using ActionBar = Android.App.ActionBar;

namespace EzTrack.Droid
{
    [Activity(Label = "EzTrack.Droid", MainLauncher = true, Icon = "@mipmap/icon",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private SwipeRefreshLayout _refresher;
        private ImageView _search;
        private ImageView _add;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            _refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _search = FindViewById<ImageView>(Resource.Id.searchImage);
            _add = FindViewById<ImageView>(Resource.Id.addImage);
            _add.Click += AddOrder;
            _refresher.Refresh += HandleRefresh;
        }


    private void AddOrder(object sender, EventArgs e)   
        {
            StartActivity(typeof(AddActivity));
        }

        //TODO: Make async await and call server to update
        public void HandleRefresh(object sender, EventArgs e)
        {
            _refresher.Refreshing = false;
        }
    }
}

