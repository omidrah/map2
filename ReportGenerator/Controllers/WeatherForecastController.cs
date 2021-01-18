using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReportGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task GetPng(int zoom, double lat, double lon)
        {
            var fileInfo = new FileInfo($"png/{Guid.NewGuid().ToString("N")}.png");
            var fs = System.IO.File.Create(fileInfo.FullName);
            var x = long2tilex(lon, zoom);
            var y = lat2tiley(lat, zoom);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                using var stream = await client.GetStreamAsync(new Uri($"http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png"));
                stream.CopyTo(fs);
                Bitmap img = new Bitmap(System.Drawing.Image.FromStream(fs));
                Graphics gr = Graphics.FromImage(img);
                //img.Save($"fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                img.Save($"png/fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
                fs.Dispose();
                fileInfo.Delete();
            }
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task GetPngByLine(int zoom, double lat, double lon)
        {
            var fileInfo = new FileInfo($"png/{Guid.NewGuid().ToString("N")}.png");
            var fs = System.IO.File.Create(fileInfo.FullName);
            var x = long2tilex(lon, zoom);
            var y = lat2tiley(lat, zoom);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                using var stream = await client.GetStreamAsync(new Uri($"http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png"));
                stream.CopyTo(fs);
                Bitmap img = new Bitmap(System.Drawing.Image.FromStream(fs));
                Graphics gr = Graphics.FromImage(img);
                DrawLine(gr);
                img.Save($"png/fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
                fs.Dispose();
                fileInfo.Delete();
            }
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task GetPngByCircle(int zoom, double lat, double lon)
        {
            var fileInfo = new FileInfo($"png/{Guid.NewGuid().ToString("N")}.png");
            var fs = System.IO.File.Create(fileInfo.FullName);
            var x = long2tilex(lon, zoom);
            var y = lat2tiley(lat, zoom);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                using var stream = await client.GetStreamAsync(new Uri($"http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png"));
                stream.CopyTo(fs);
                Bitmap img = new Bitmap(System.Drawing.Image.FromStream(fs));
                Graphics gr = Graphics.FromImage(img);
                DrawCircle(gr);
                img.Save($"png/fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
                fs.Dispose();
                fileInfo.Delete();
            }
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task GetPngByPoint(int zoom, double lat, double lon)
        {
            var fileInfo = new FileInfo($"png/{Guid.NewGuid().ToString("N")}.png");
            var fs = System.IO.File.Create(fileInfo.FullName);
            var x = long2tilex(lon, zoom);
            var y = lat2tiley(lat, zoom);
            changeLatLonToPixel(lat, lon, out double xx, out double yy);
            var resolution = 156543.03 * Math.Cos(lat) / Math.Pow(2, zoom);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                using var stream = await client.GetStreamAsync(new Uri($"http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png"));
                stream.CopyTo(fs);
                Bitmap img = new Bitmap(System.Drawing.Image.FromStream(fs));
                Graphics gr = Graphics.FromImage(img);
                DrawPoint2(gr, xx, yy, zoom);               
                //img.Save($"fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);                
                img.Save($"png/fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();             
            }            
               fs.Dispose();
               fileInfo.Delete();
        }
         [HttpGet]
        [Route("[Action]")]
        public async Task GetPngMultiPoint()
        {
            var fileInfo = new FileInfo($"png/{Guid.NewGuid().ToString("N")}.png");
            var fs = System.IO.File.Create(fileInfo.FullName);
            var res = new double[4,2];
            var zLe= new double[4];
            double[,] points = new double[,]  //lat,lon
                    { 
                                { 35.7193, 51.3504666666667 },
                                { 35.6618833333333, 51.3705666666667 },
                                { 35.6652666666667, 51.3690666666667 }, 
                                { 35.75765, 51.4848666666667 } 
                     };
            double minx= 51.3504666666667,miny=35.7193,maxx=51.3504666666667,maxy=35.7193;
            for (int i = 0; i < 4; i++)
            {
                changeLatLonToPixel(points[i,0], points[i,1], out double x, out double y);  
                maxy= Math.Max(points[i,0],maxy);
                maxx= Math.Max(points[i,1],maxx);
                miny= Math.Min(points[i,0],miny);
                minx= Math.Min(points[i,1],minx);
                //var resolution = 156543.03 * Math.Cos(points[i,0]) / (2 ^ zoomlevel);
                res[i,0]=x;
                res[i,1]=y;
            }
                double ry1 = Math.Log((Math.Sin(ToRadians(miny)) + 1) 
                            / Math.Cos(ToRadians(miny)));              
                double ry2 = Math.Log((Math.Sin(ToRadians(maxy)) + 1) 
                            / Math.Cos(ToRadians(maxy)));

                double ryc = (ry1 + ry2) / 2;
                double centerY = ToDeg(Math.Atan(Math.Sinh(ryc)));
                double resolutionHorizontal = Math.Abs(minx - maxx) / 600;
                double vy0 = Math.Log(Math.Tan(Math.PI*(0.25 + centerY/360)));
                double vy1 = Math.Log(Math.Tan(Math.PI*(0.25 + maxy/360)));
                double viewHeightHalf =600/2.0f;
                double zoomFactorPowered = viewHeightHalf
                    / (40.7436654315252*(vy1 - vy0));
                double resolutionVertical = 360.0 / (zoomFactorPowered * 256);

                double resolution = Math.Max(resolutionHorizontal, resolutionVertical) * 1.2;
                int zoomlevel = (int)Math.Log(360 / (resolution * 256), 2);
                // double lon = mapArea.Center.X;
                // double lat = centerY;
           
                 var lx1 = long2tilex(minx, zoomlevel);
                 var ly1 = lat2tiley(miny, zoomlevel);
                 var hx1 = long2tilex(maxx, zoomlevel);
                 var hy1 = lat2tiley(maxy, zoomlevel);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                    
                    using var stream = await client.GetStreamAsync(new Uri($"http://a.tile.openstreetmap.org/{zoomlevel}/{lx1}/{ly1}.png"));
                    stream.CopyTo(fs);
                    Bitmap img = new Bitmap(System.Drawing.Image.FromStream(fs));


                    var n = new FileInfo($"png/{Guid.NewGuid().ToString("N")}.png");
                    var fs2 = System.IO.File.Create(n.FullName);
                    using var stream2 = await client.GetStreamAsync(new Uri($"http://a.tile.openstreetmap.org/{zoomlevel}/{hx1}/{hy1}.png"));
                    stream2.CopyTo(fs2);
                    Bitmap img2 = new Bitmap(System.Drawing.Image.FromStream(fs2));
                    Bitmap[] ss = new Bitmap[2];
                    ss[0]=img;ss[1]=img2;
                    var ret =  CombineBitmap(ss);
                    ret.Save($"png/marg-{Guid.NewGuid().ToString("N")}.png");
                    Graphics gr = Graphics.FromImage(ret);    
                    changeLatLonToPixel(miny,minx, out double xx, out double yy);                     
                    DrawPoint2(gr, xx, yy, zoomlevel);
                    // for (int i = 1; i < 4; i++)
                    // {
                    //     changeLatLonToPixel(points[i,0], points[i,1], out double z, out double zz); 
                    //     DrawPoint2(gr, z,zz, zoomlevel);
                    // }
                    // ss[0].Save($"png/fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                    // ss[1].Save($"png/fi2-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
                    // ss[0].Dispose();                   
                    // ss[1].Dispose();
                    //fs2.Dispose();n.Delete();
                }
           
               //fs.Dispose();
               //fileInfo.Delete();
        }   

public  System.Drawing.Bitmap CombineBitmap(Bitmap[] files)
{
    //read all images into memory
    List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
    System.Drawing.Bitmap finalImage = null;

    try
    {
        int width = 0;
        int height = 0;

        foreach (Bitmap image in files)
        {
            
            //update the size of the final bitmap
            width += image.Width;
            height = image.Height > height ? image.Height : height;
            image.Save($"png/fi-{Guid.NewGuid().ToString("N")}.png", System.Drawing.Imaging.ImageFormat.Png);
            images.Add(image);
        }

        //create a bitmap to hold the combined image
        finalImage = new System.Drawing.Bitmap(width, height);

        //get a graphics object from the image so we can draw on it
        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
        {
            //set background color
            g.Clear(System.Drawing.Color.Black);

            //go through each image and draw it on the final image
            int offset = 0;
            foreach (System.Drawing.Bitmap image in images)
            {
                g.DrawImage(image,
                  new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                offset += image.Width;
            }
        }        
        return finalImage;
    }
    catch (Exception ex)
    {
        if (finalImage != null)
            finalImage.Dispose();

        throw ex;
    }
    finally
    {
        //clean up memory
        foreach (System.Drawing.Bitmap image in images)
        {
            image.Dispose();
        }
    }
}       
        Graphics DrawLine(Graphics gr)
        {
            var c = new PointF(0, 0);
            var b = new PointF(100, 200);
            gr.DrawLine(new Pen(Brushes.DeepSkyBlue), c, b);
            return gr;
        }
        Graphics DrawString(Graphics gr)
        {
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            gr.DrawString("Omid Rahimi", drawFont, drawBrush, new RectangleF(0, 0, 100, 100));
            return gr;
        }
        Graphics DrawCircle(Graphics gr)
        {
            var cpen = new Pen(Color.Red);
            gr.DrawEllipse(cpen, new RectangleF(50, 50, 10, 30));
            gr.DrawEllipse(cpen, 10, 10, 1, 1);
            gr.DrawEllipse(cpen, 20, 20, 1, 1);
            gr.DrawEllipse(cpen, 30, 30, 1, 1);
            return gr;
        }
        Graphics DrawPoint(Graphics gr, double x, double y, int zoom)
        {
            // gr.DrawEllipse(cpen, new RectangleF(5, 5, 5, 5));
            // gr.DrawEllipse(cpen, new Rectangle(15, 15, 5, 5));
            // gr.DrawEllipse(cpen, 20, 20, 3, 3);
            // gr.DrawEllipse(cpen, 30, 30, 3, 3);
            using (Brush b = new SolidBrush(Color.Red))
            {
                gr.FillEllipse(b, new RectangleF(55, 55, 3, 3));
            }
            return gr;
        }
        Graphics DrawPoint2(Graphics gr, double x, double y,int zoom)
        {
            //  var pf = new PointF((float)(x / Math.Pow(2, zoom + 1)), (float)(y / Math.Pow(2, zoom + 1)));
            // gr.DrawImage(Image.FromFile($"{Environment.CurrentDirectory}/assets/marker.png"), pf);

             Icon newIcon = new Icon($"{Environment.CurrentDirectory}/assets/marker.ico");
             gr.DrawIcon(newIcon, (int)(x / Math.Pow(2, zoom + 1)), (int)(y / Math.Pow(2, zoom + 1)));
            return gr;
        }
        int long2tilex(double lon, int z)
        {
            return (int)(Math.Floor((lon + 180.0) / 360.0 * (1 << z)));
        }
        int lat2tiley(double lat, int z)
        {
            return (int)Math.Floor((1 - Math.Log(Math.Tan(ToRadians(lat)) + 1 / Math.Cos(ToRadians(lat))) / Math.PI) / 2 * (1 << z));
        }
        public  double Distance(Point pt1, Point pt2)
        {
            if ((pt1 ==null || pt2==null))
                return double.NaN;
            return Math.Sqrt((pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y));
        }
        public  double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            if (double.IsNaN(lon1) || double.IsNaN(lat1) || double.IsNaN(lon2) || (double.IsNaN(lat2)))
                return 0;

            double radiusInLat = EarthRadius * Math.Cos((lat1 * Math.PI / 180 + lat2 * Math.PI / 180) / 2);
            double dx = (lon1 * Math.PI / 180 - lon2 * Math.PI / 180) * radiusInLat;
            double dy = (lat1 * Math.PI / 180 - lat2 * Math.PI / 180) * EarthRadius;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        public const double EarthRadius = 6378137;
        void changeLatLonToPixel(double Lat, double Lon, out double x, out double y)
        {
            if (!double.IsNaN(Lat) && !double.IsNaN(Lon))
            {

                var latRad = ToRadians(Lat);
                var lonRad = ToRadians(Lon);
                x = EarthRadius * lonRad;
                y = EarthRadius * Math.Log(Math.Tan(Math.PI / 4 + latRad / 2));
                //روش تبدیل زیر خروجی دیگر داشت
                // x = EarthRadius *  Math.Cos(Lat) * Math.Cos(Lon);
                // y = EarthRadius * Math.Cos(Lat) * Math.Sin(Lon);                         
            }
            else
            {
                x = 0;
                y = 0;
            }

        }

        private double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
        
        private double ToDeg(double val)
        {
            return (180* val)/Math.PI;
        }
    }
}
