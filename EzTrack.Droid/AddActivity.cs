using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using EzTrack.Droid.Model;
using ZXing.Mobile;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace EzTrack.Droid
{
    [Activity(Label = "AddActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddActivity : AppCompatActivity
    {
        private ImageView _closeActivity;
        private ImageView _cameraImage;
        private ImageView _addOrder;
        private MediaPlayer _player;
        private AutoCompleteTextView _autoCompleteTextView;
        private EditText _datePicker;
        private ListView _scanList;
        private readonly List<string> _scannedItems = new List<string>();
        private const int RequestCameraPermisionId = 1001;
        private readonly string[] _permissionsLocation = {Manifest.Permission.Camera};
        private MobileBarcodeScanner _scanner;
        private MobileBarcodeScanningOptions _option;
        private Spinner _statusSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Add);

            RequestPermissions(_permissionsLocation, RequestCameraPermisionId);

            _closeActivity = FindViewById<ImageView>(Resource.Id.closeActivity);
            _cameraImage = FindViewById<ImageView>(Resource.Id.cameraImage);
            _autoCompleteTextView = FindViewById<AutoCompleteTextView>(Resource.Id.addresses);
            _player = MediaPlayer.Create(this, Resource.Raw.beep);
            _player.SetVolume(100, 100);
            _datePicker = FindViewById<EditText>(Resource.Id.datepicker);
            _scanList = FindViewById<ListView>(Resource.Id.scanList);
            _statusSpinner = FindViewById<Spinner>(Resource.Id.orderStatus);
            _addOrder = FindViewById<ImageView>(Resource.Id.addOrder);
            var today = DateTime.Today;
            var dateDialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
            dateDialog.DatePicker.MinDate = today.Millisecond;
            _datePicker.Touch += delegate { dateDialog.Show(); };
            _closeActivity.Click += CloseActivity;
            _cameraImage.Click += LaunchScanner;
            _autoCompleteTextView.TextChanged += GetAddresses;
            _scanList.ItemClick += RemoveScannedItem;
            _statusSpinner.Adapter = FillStatusSpinner();
            _addOrder.Click += HandleOrderSubmit;
        }

        private void HandleOrderSubmit(object sender, EventArgs e)
        {
            var order = new OrderAsset
            {
                PickupDate = Convert.ToDateTime(_datePicker.Text),
                OrderName = _autoCompleteTextView.Text,
                Status = _statusSpinner.SelectedItem.ToString(),
                ScannedAssets = _scannedItems
            };
            Http.PostOrder(order);
            Finish();
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            _datePicker.Text = e.Date.ToString("D");
        }

        private void CloseActivity(object sender, EventArgs e) => Finish();

        private void LaunchScanner(object sender, EventArgs e)
        {
            MobileBarcodeScanner.Initialize(Application);
            _scanner = new MobileBarcodeScanner
            {
                FlashButtonText = "Flash",
                TopText = "Hold camera up to barcode to scan",
                BottomText = "Barcode will automatically scan"
            };

            _option = new MobileBarcodeScanningOptions { DelayBetweenContinuousScans = 1000 };
            _scanner.ScanContinuously(_option, HandleScanResult);
        }

        private async void HandleScanResult(ZXing.Result result)
        {
            if (result == null) return;
            _player.Start();

            var lookupResult = await Http.GetAssetName(result.Text);
            lookupResult = lookupResult.Replace("\"", "");
            RunOnUiThread(() =>
            {
                if (_scannedItems.Contains(lookupResult))
                {
                    _scanner.Cancel();

                    var alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Duplicate Scan");
                    alert.SetMessage($"Are you sure you want to scan {lookupResult} again?");
                    alert.SetPositiveButton("Yes", (senderAlert, args) =>
                    {
                        _scannedItems.Add(lookupResult);
                        UpdateScanListView();
                        _scanner.ScanContinuously(_option, HandleScanResult);
                    });

                    alert.SetNegativeButton("No", (senderAlert, args) => { _scanner.ScanContinuously(_option, HandleScanResult); });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {
                    _scannedItems.Add(lookupResult);
                    UpdateScanListView();
                }

            });
        }

        private void GetAddresses(object sender, EventArgs e)
        {
            var autoCompleteOptions = new[] { "Hello", "Hey", "Heja", "Hi", "Hola", "Bonjour", "Gday", "Goodbye", "Sayonara", "Farewell", "Adios" };
            var autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, autoCompleteOptions);
            _autoCompleteTextView.Adapter = autoCompleteAdapter;
        }

        private void RemoveScannedItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            var itemToDelete = _scannedItems[e.Position];

            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Confirm delete");
            alert.SetMessage($"Are you sure you want to remove {itemToDelete}  from the list?");
            alert.SetPositiveButton("Delete", (senderAlert, args) =>
            {
                _scannedItems.RemoveAt(e.Position);
                UpdateScanListView();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => { });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void UpdateScanListView()
        {
            var listAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _scannedItems.ToArray());
            listAdapter.NotifyDataSetChanged();
            _scanList.Adapter = listAdapter;
        }

        private ISpinnerAdapter FillStatusSpinner()
        {
            var statuses = new[] {"On Truck", "At Residence", "Returned"};
            var spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, statuses);
            return spinnerAdapter;
        }
    }
}  