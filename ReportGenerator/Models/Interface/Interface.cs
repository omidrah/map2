using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ReportGenerator.Models.Interface
{
    /// <summary>
    /// ساخت عکس از روی شی
    /// Rectangel
    /// </summary>
    internal  abstract  class MapMaker
    {        
        /// <summary>
        /// زمینه نقشه را ایجاد کند
        /// </summary>
        /// <param name="rectangle"></param>
        public void GetRawImageFromRectange(IRectangle rectangle)
        {
            //ایجاد زمینه پس ازخواندن نقاط جغرافیایی
            //و پیداکردن ابعاد نقشه با استفاده 
            //می نی مم و ماکسیمم نفاط دریافتی

        }
        /// <summary>
        /// ایجاد مارکرها روی زمینه
        /// </summary>
        public void MakeMarkerOnRectangle()
        {
            //لیست نقاط را خوانده و براساس طول و عرض جغرافیایی نقطه ایجا کنیم
            //سپس خروجی حاصل از آنرا روی زمینه قرار دهیم
            //عکس حاصله را ذخیره نماییم
        }
        

    }
}
