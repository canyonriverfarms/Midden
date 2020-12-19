using Caf.Midden.Components.Common;
using Darnton.Blazor.Leaflet.LeafletMap;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caf.Midden.Components
{
    public partial class GeoJsonMap : ComponentBase
    {
        public Map PositionMap;

        protected TileLayer OpenStreetMapsTileLayer;

        [Parameter]
        public string GeoJsonString { get; set; }

        public string DebugString { get; set; }

        public async Task LoadCoords()
        {

            JsonDocument jd = JsonDocument.Parse(GeoJsonString);
            string type = jd.RootElement.GetProperty("type").GetString();

            var geometry = JsonSerializer.Deserialize<GeometryPolygon>(GeoJsonString);

            if (type == "Point")
            {
                // set point

            }
            if (type == "Polygon")
            {

                string coordString =
                    JsonSerializer.Serialize(jd.RootElement.GetProperty("coordinates"));

                DebugString = coordString;

                var coords = JsonSerializer
                    .Deserialize<List<List<List<double>>>>(coordString);

                DebugString += "\nCoords Count:" + coords.Count;

                foreach (var coord in coords)
                {
                    List<LatLng> latLngs = new List<LatLng>();

                    foreach (var point in coord)
                    {
                        DebugString += "\nPoint: " + point[1] + "," + point[0];

                        latLngs.Add(new LatLng(point[1], point[0]));
                    }

                    var poly = new Polyline(latLngs, new PolylineOptions());
                    await poly.AddTo(this.PositionMap);
                }
            }
            else
            {

            }
        }
        protected override void OnInitialized()
        {
            PositionMap = new Map("testMap", new MapOptions
            {
                Center = new LatLng(46.780523, -117.079884),
                Zoom = 12 // TODO: This should scale to size of poly
            });

            OpenStreetMapsTileLayer = new TileLayer(
                "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                new TileLayerOptions
                {
                    Attribution = @"Map data &copy; <a href=""https://www.openstreetmap.org/"">OpenStreetMap</a> contributors, " +
                        @"<a href=""https://creativecommons.org/licenses/by-sa/2.0/"">CC-BY-SA</a>"
                }
            );

            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await LoadCoords();
        }
    }
}
