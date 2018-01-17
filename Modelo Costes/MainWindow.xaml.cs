using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Utilities;
using Mapsui.Styles;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace Modelo_Costes
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyMapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            var layer = GenerateIconLayer();
            MyMapControl.Map.Layers.Add(layer);
            MyMapControl.Map.InfoLayers.Add(layer);
            MyMapControl.Map.Info += (sender, args) =>
            {
                var layername = args.Layer?.Name;
                var featureLabel = args.Feature?["Label"]?.ToString();
                var featureType = args.Feature?["Type"]?.ToString();

                Debug.WriteLine("Info Event was invoked.");
                Debug.WriteLine("Layername: " + layername);
                Debug.WriteLine("Feature Label: " + featureLabel);
                Debug.WriteLine("Feature Type: " + featureType);

                Debug.WriteLine("World Postion: {0:F4} , {1:F4}", args.WorldPosition?.X, args.WorldPosition?.Y);
                Debug.WriteLine("Screen Postion: {0:F4} , {1:F4}", args.ScreenPosition?.X, args.ScreenPosition?.Y);
            };



            //var centerOfLondonOntario = new Mapsui.Geometries.Point(-6.3714, 39.4768);
            // // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            // var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfLondonOntario.X, centerOfLondonOntario.Y);
            // MyMapControl.Map.NavigateTo(sphericalMercatorCoordinate);
            // MyMapControl.Map.NavigateTo(MyMapControl.Map.Resolutions[9]);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyMapControl.ZoomIn();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MyMapControl.ZoomOut();
            var centerOfCC = new Mapsui.Geometries.Point(-6.3714, 39.4768);
            var center = GetCoordenates();
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(center.X, center.Y);
            MyMapControl.Map.NavigateTo(sphericalMercatorCoordinate);


            MyMapControl.Map.NavigateTo(MyMapControl.Map.Resolutions[15]);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MyMapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()));
            //MyMapControl.Map.Layers.Add(new )
          

          
        
            //var centerOfLondonOntario = new Mapsui.Geometries.Point(-73.94, 40.67);
            //// OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            //var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfLondonOntario.X, centerOfLondonOntario.Y);
            //MyMapControl.Map.NavigateTo(sphericalMercatorCoordinate);


            // MyMapControl.Map.NavigateTo(MyMapControl.Map.Resolutions[9]);
        }

        private ILayer GenerateIconLayer()
        {
            var layername = "My Local Layer";
            return new Layer(layername)
            {
                Name = layername,
                DataSource = new MemoryProvider(GetIconFeatures()),
                Style = new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Brush(Color.Red),
                    Outline = { Color = Color.Black, Width = 1 }
                }
            };
        }

        private Features GetIconFeatures()
        {
            var features = new Features();
            var feature = new Feature
            {
                Geometry = new Polygon(new LinearRing(new[]
            {
            new Mapsui.Geometries.Point(1066689.6851, 6892508.8652),
            new Mapsui.Geometries.Point(1005540.0624, 6987290.7802),
            new Mapsui.Geometries.Point(1107659.9322, 7056389.8538),
            new Mapsui.Geometries.Point(1066689.6851, 6892508.8652)
            })),
                ["Label"] = "My Feature Label",
                ["Type"] = "My Feature Type"
            };

            features.Add(feature);
            return features;
        }

        private Mapsui.Geometries.Point GetCoordenates()
        {
            Double latNumber =0;
            Double longNumber =0;
            var httpClient = new HttpClient();
            HttpResponseMessage httpResult =  httpClient.GetAsync( "http://nominatim.openstreetmap.org/search?q=135+pilkington+avenue,+birmingham&format=json&polygon=1&addressdetails=1").Result;
            if (httpResult.IsSuccessStatusCode) {
                var result = httpResult.Content.ReadAsStringAsync().Result;
                 var r = (JArray)JsonConvert.DeserializeObject(result);
                var latString = ((JValue)r[0]["lat"]).Value as string;
                var longString = ((JValue)r[0]["lon"]).Value as string;

                Debug.WriteLine("Latitud: " + latString);
                Debug.WriteLine("Longitud: " + longString);
                latNumber = Convertcoordinates(latString);
                longNumber = Convertcoordinates(longString);
              
              
            }
            return new Mapsui.Geometries.Point(longNumber,latNumber);
        }

        private Double Convertcoordinates(string coordinate)
        {
            NumberStyles style;
            CultureInfo culture;
            double number;

            style = NumberStyles.Number;
            culture = CultureInfo.InvariantCulture;
            if (Double.TryParse(coordinate, style, culture, out number))
                Debug.WriteLine("Converted '{0}' to {1}.", coordinate, number);
            else
                Debug.WriteLine("Unable to convert '{0}'.", coordinate);
            return number;
        }
    }
}
