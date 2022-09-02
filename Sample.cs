using System;

namespace LLXYConverter
{
    class Sample
    {
        static void Main(string[] args)
        {
            // 東京都庁
            var lon = 139.6917337d;
            var lat = 35.6895014d;
            var res = LLXYConverter.LL2XY(lon, lat, 9);
            Console.WriteLine($"X={res.X}, Y={res.Y}");
            var res2 = LLXYConverter.XY2LL(res.X, res.Y, 9);
            Console.WriteLine($"lon={res2.lon}, lat={res2.lat}");
        }
    }
}
