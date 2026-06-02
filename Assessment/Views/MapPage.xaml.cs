using Assessment.ViewModels;
using System.ComponentModel;

namespace Assessment.Views
{
    public partial class MapPage : ContentPage
    {
        private readonly MapViewModel _viewModel;
        private bool _mapInitialized;

        public MapPage() : this(ServiceHelper.GetService<MapViewModel>()) { }

        public MapPage(MapViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.LocationUpdated += OnLocationUpdated;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!_mapInitialized)
            {
                InitializeMap();
                _mapInitialized = true;
            }
        }

        private void InitializeMap()
        {
            var html = GenerateMapHtml(
                _viewModel.ShopLatitudeValue,
                _viewModel.ShopLongitudeValue,
                _viewModel.ShopNameDisplay);

            MapWebView.Source = new HtmlWebViewSource { Html = html };
        }

        private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapViewModel.HasLocation) && _viewModel.HasLocation)
            {
                await Task.Delay(500);
                await UpdateUserMarker(
                    _viewModel.CurrentLatitude,
                    _viewModel.CurrentLongitude,
                    _viewModel.CurrentAddress);
            }
        }

        private async void OnLocationUpdated(double lat, double lon, string address)
        {
            await Task.Delay(300);
            await UpdateUserMarker(lat, lon, address);
        }

        private async Task UpdateUserMarker(double lat, double lon, string address)
        {
            try
            {
                var escapedAddress = address.Replace("'", "\\'").Replace("\"", "\\\"");
                var script = $"updateUserLocation({lat}, {lon}, '{escapedAddress}');";
                await MapWebView.EvaluateJavaScriptAsync(script);
            }
            catch
            {
                // WebView JS evaluation may fail silently in some scenarios
            }
        }

        private static string GenerateMapHtml(double shopLat, double shopLon, string shopName)
        {
            return $@"<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8'/>
<meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'/>
<link rel='stylesheet' href='https://cdn.bootcdn.net/ajax/libs/leaflet/1.9.4/leaflet.min.css'/>
<script src='https://cdn.bootcdn.net/ajax/libs/leaflet/1.9.4/leaflet.min.js'></script>
<style>
*{{margin:0;padding:0}}
html,body,#map{{width:100%;height:100%}}
.leaflet-control-zoom{{margin:10px}}
.leaflet-control-zoom a{{width:36px;height:36px;line-height:36px;font-size:20px;border-radius:8px;background:#fff;box-shadow:0 2px 8px rgba(0,0,0,.2)}}
.custom-marker{{background:#E53935;color:#fff;padding:4px 10px;border-radius:20px;font-size:12px;font-weight:bold;white-space:nowrap;box-shadow:0 2px 6px rgba(0,0,0,.3);border:2px solid #fff}}
.user-marker{{background:#1E88E5;color:#fff;padding:4px 10px;border-radius:20px;font-size:12px;font-weight:bold;white-space:nowrap;box-shadow:0 2px 6px rgba(0,0,0,.3);border:2px solid #fff}}
.info-panel{{position:absolute;bottom:10px;left:10px;right:10px;z-index:1000;background:rgba(255,255,255,.95);border-radius:10px;padding:10px;box-shadow:0 2px 10px rgba(0,0,0,.15);font-size:12px;display:none}}
</style>
</head>
<body>
<div id='map'></div>
<div id='infoPanel' class='info-panel'>
    <strong>📍 Your Location</strong><br/>
    <span id='userAddr'></span><br/>
    <span style='color:#E53935;font-weight:bold' id='userDist'></span>
</div>
<script>
var map = L.map('map', {{
    center: [{shopLat}, {shopLon}],
    zoom: 15,
    zoomControl: true,
    attributionControl: false
}});

L.tileLayer('https://webrd0{{s}}.is.autonavi.com/appmaptile?lang=en&size=1&scale=1&style=8&x={{x}}&y={{y}}&z={{z}}', {{
    subdomains: ['1','2','3','4'],
    maxZoom: 18,
    minZoom: 3
}}).addTo(map);

var shopIcon = L.divIcon({{
    html: '<div class=""custom-marker"">🏪 {shopName}</div>',
    className: '',
    iconSize: null,
    iconAnchor: null
}});

L.marker([{shopLat}, {shopLon}], {{icon: shopIcon}}).addTo(map)
    .bindPopup('<b>🏪 {shopName}</b><br/>Longitude: {shopLon:F6}<br/>Latitude: {shopLat:F6}<br/>Authentic Chinese Cuisine | Fresh Delivery');

var userMarker = null;
var userCircle = null;
var infoPanel = document.getElementById('infoPanel');

function updateUserLocation(lat, lon, address) {{
    if (userMarker) {{ map.removeLayer(userMarker); }}
    if (userCircle) {{ map.removeLayer(userCircle); }}

    var userIcon = L.divIcon({{
        html: '<div class=""user-marker"">📍 My Location</div>',
        className: '',
        iconSize: null,
        iconAnchor: null
    }});

    userMarker = L.marker([lat, lon], {{icon: userIcon}}).addTo(map)
        .bindPopup('<b>📍 My Location</b><br/>' + address);

    userCircle = L.circle([lat, lon], {{
        radius: 150,
        color: '#1E88E5',
        fillColor: '#1E88E5',
        fillOpacity: 0.15,
        weight: 2
    }}).addTo(map);

    var dist = map.distance([lat, lon], [{shopLat}, {shopLon}]);
    var distText = dist < 1000 ? (dist.toFixed(0) + ' m') : ((dist/1000).toFixed(1) + ' km');

    document.getElementById('userAddr').innerText = address;
    document.getElementById('userDist').innerText = '🏪 About ' + distText + ' from store';
    infoPanel.style.display = 'block';

    var bounds = L.latLngBounds([[lat, lon], [{shopLat}, {shopLon}]]);
    map.fitBounds(bounds, {{padding: [50, 50], maxZoom: 16}});
}}
</script>
</body>
</html>";
        }
    }
}
