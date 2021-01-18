using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportGenerator.Models.Interface
{
    /// <summary>
    /// شی 
    /// rectangle
    /// </summary>
    interface IRectangle
    {
        public double left { get; set; }
        public double bottom { get; set; }
        public double height { get; set; }
        public double width { get; set; }
         void getAllLanlat();
         void getMaxLatLon();
         void getMinLatLon();        
    }
}
