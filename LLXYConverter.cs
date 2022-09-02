// Memo:
// 系番号の原点: https://www.gsi.go.jp/LAW/heimencho.html
// 系番号のおおよその位置: https://www.gsi.go.jp/sokuchikijun/jpc.html
// LL2XY: https://vldb.gsi.go.jp/sokuchi/surveycalc/surveycalc/algorithm/bl2xy/bl2xy.htm
// XY2LL: https://vldb.gsi.go.jp/sokuchi/surveycalc/surveycalc/algorithm/xy2bl/xy2bl.htm

// 注意：
// 入力は10進Degに変換して用いる．
// GRS80を測地系として採用．
// 上記国土地理院のサイトには角度の混同があるため注意して実装．
using System;

namespace LLXYConverter
{
    public static class LLXYConverter
    {
        // 定義
        public static XYGroup[] group_origin =
        {
            new XYGroup(), // dummy
            new XYGroup() { No = 1, o_lon = 129d + 30d / 60d, o_lat = 33d },
            new XYGroup() { No = 2, o_lon = 131d, o_lat = 33d },
            new XYGroup() { No = 3, o_lon = 132d + 10d / 60d, o_lat = 36d },
            new XYGroup() { No = 4, o_lon = 133d + 30d / 60d, o_lat = 33d },
            new XYGroup() { No = 5, o_lon = 134d + 20d / 60d, o_lat = 36d },
            new XYGroup() { No = 6, o_lon = 136d, o_lat = 36d },
            new XYGroup() { No = 7, o_lon = 137d + 10d / 60d, o_lat = 36d },
            new XYGroup() { No = 8, o_lon = 138d + 30d / 60d, o_lat = 36d },
            new XYGroup() { No = 9, o_lon = 139d + 50d / 60d, o_lat = 36d },
            new XYGroup() { No = 10, o_lon = 140d + 50d / 60d, o_lat = 40d },
            new XYGroup() { No = 11, o_lon = 140d + 15d / 60d, o_lat = 44d },
            new XYGroup() { No = 12, o_lon = 142d + 15d / 60d, o_lat = 44d },
            new XYGroup() { No = 13, o_lon = 144d + 15d / 60d, o_lat = 44d },
            new XYGroup() { No = 14, o_lon = 142d, o_lat = 26d },
            new XYGroup() { No = 15, o_lon = 127d + 30d / 60d, o_lat = 26d },
            new XYGroup() { No = 16, o_lon = 124d, o_lat = 26d },
            new XYGroup() { No = 17, o_lon = 131d, o_lat = 26d },
            new XYGroup() { No = 18, o_lon = 136d, o_lat = 20d },
            new XYGroup() { No = 19, o_lon = 154d, o_lat = 26d}
        };

        static double a = 6378137; // 地球の長半径[km]
        static double F = 298.257222101d; // 地球の逆扁平率
        static double m_0 = 0.9999; // 縮尺係数
        static double n = 1d / (2d * F - 1d);

        // nの累乗
        static double[] np =
        {
            1,
            n,
            Math.Pow(n, 2),
            Math.Pow(n, 3),
            Math.Pow(n, 4),
            Math.Pow(n, 5),
            Math.Pow(n, 6)
        };

        // A_0 to A_5
        static double[] A =
        {
            1d + np[2] / 4d + np[4] / 64d,
            -(3d / 2d) * (n - np[3] / 8d - np[5] / 64d),
            (15d / 16d) * (np[2] - np[4] / 4d),
            -(35d / 48d) * (np[3] - (5d / 16d) * np[5]),
            (315d / 512d) * np[4],
            -(693d / 1280d) * np[5]
        };

        static double A_bar = A[0] * (m_0 * a) / (1 + n);

        // alpha_1 to alpha_5
        static double[] alpha =
        {
            0, // dummy
            (1d / 2d) * n - (2d / 3d) * np[2] + (5d / 16d) * np[3] + (41d / 180d) * np[4] - (127d / 288d) * np[5],
            (13d / 48d) * np[2] - (3d / 5d) * np[3] + (557d / 1440d) * np[4] + (281d / 630d) * np[5],
            (61d / 240d) * np[3] - (103d / 140d) * np[4] + (15061d / 26880d) * np[5],
            (49561d / 161280d) * np[4] - (179d / 168d) * np[5],
            (34729d / 80640d) * np[5]
        };

        // beta_1 to beta_5
        static double[] beta =
        {
            0, // dummy
            (1d / 2d) * n - (2d / 3d) * np[2] + (37d / 96d) * np[3] - (1d / 360d) * np[4] - (81d / 512d) * np[5],
            (1d / 48d) * np[2] + (1d / 15d) * np[3] - (437d / 1440d) * np[4] + (46d / 105d) * np[5],
            (17d / 480d) * np[3] - (37d / 840d) * np[4] - (209d / 4480d) * np[5],
            (4397d / 161280d) * np[4] - (11d / 504d) * np[5],
            (4583d / 161280d) * np[5]
        };

        // delta_1 to delta_6
        static double[] delta =
        {
            0, // dummy
            2 * n - (2d / 3d) * np[2] - 2d * np[3] + (116d / 45d) * np[4] + (26d / 45d) * np[5] - (2854d / 675d) * np[6],
            (7d / 3d) * np[2] - (8d / 5d) * np[3] - (227d / 45d) * np[4] + (2704d / 315d) * np[5] + (2323d / 945d) * np[6],
            (56d / 15d) * np[3] - (136d / 35d) * np[4] - (1262d / 105d) * np[5] + (73814d / 2835d) * np[6],
            (4279d / 630d) * np[4] - (332d / 35d) * np[5] - (399572d / 14175d) * np[6],
            (4174d / 315d) * np[5] - (144838d / 6237d) * np[6],
            (601676d / 22275d) * np[6]
        };


        /// <summary>
        /// 緯度経度 to 平面直角座標
        /// </summary>
        /// <param name="lon">経度(deg)</param>
        /// <param name="lat">緯度(deg)</param>
        /// <param name="gNo">系番号</param>
        /// <returns></returns>
        public static LonLatXY LL2XY(double lon, double lat, int gNo)
        {
            var group = group_origin[gNo];
            var res = new LonLatXY() { group = group, lon = lon, lat = lat };
            double t = DegSinh(DegAtanh(DegSin(lat)) - ((2 * Math.Sqrt(n)) / (1 + n)) * DegAtanh(DegSin(lat) * (2 * Math.Sqrt(n)) / (1 + n)));
            double t_bar = Math.Sqrt(1 + Math.Pow(t, 2));
            double lon_c = DegCos(lon - group.o_lon);
            double lon_s = DegSin(lon - group.o_lon);
            double xi_prime = Math.Atan(t / lon_c);
            double eta_prime = Math.Atanh(lon_s / t_bar);
            double X = xi_prime;
            double Y = eta_prime;
            double S_phi0_bar = Deg2Rad(A[0] * group.o_lat);
            for (int i = 1; i <= 5; i++)
            {
                X += alpha[i] * Math.Sin(2 * i * xi_prime) * Math.Cosh(2 * i * eta_prime);
                Y += alpha[i] * Math.Cos(2 * i * xi_prime) * Math.Sinh(2 * i * eta_prime);
                S_phi0_bar += A[i] * DegSin(2 * i * group.o_lat);
            }
            S_phi0_bar *= m_0 * a / (1 + n);
            X *= A_bar;
            X -= S_phi0_bar;
            Y *= A_bar;
            res.X = X;
            res.Y = Y;
            return res;
        }

        public static LonLatXY XY2LL(double X, double Y, int gNo)
        {
            var group = group_origin[gNo];
            var res = new LonLatXY() { group = group, X = X, Y = Y };
            double S_phi0_bar = Deg2Rad(A[0] * group.o_lat);
            for (int i = 1; i <= 5; i++)
            {
                S_phi0_bar += A[i] * DegSin(2 * i * group.o_lat);
            }
            S_phi0_bar *= m_0 * a / (1 + n);
            double xi = (X + S_phi0_bar) / A_bar;
            double eta = Y / A_bar;
            double xi_prime = xi;
            double eta_prime = eta;
            for (int i = 1; i <= 5; i++)
            {
                xi_prime -= beta[i] * Math.Sin(2 * i * xi) * Math.Cosh(2 * i * eta);
                eta_prime -= beta[i] * Math.Cos(2 * i * xi) * Math.Sinh(2 * i * eta);
            }
            double khi = Math.Asin(Math.Sin(xi_prime) / Math.Cosh(eta_prime));
            res.lon = Deg2Rad(group.o_lon) + Math.Atan(Math.Sinh(eta_prime) / Math.Cos(xi_prime));
            res.lat = khi;
            for (int i = 1; i<= 6; i++)
            {
                res.lat += delta[i] * Math.Sin(2 * i * khi);
            }
            res.lon = Rad2Deg(res.lon);
            res.lat = Rad2Deg(res.lat);
            return res;
        }

        #region 三角関数

        /// <summary>
        /// sinをdeg単位で計算
        /// </summary>
        /// <param name="t">角度(deg)</param>
        /// <returns></returns>
        private static double DegSin(double t)
        {
            return Math.Sin(Deg2Rad(t));
        }

        /// <summary>
        /// sinhをdeg単位で計算
        /// </summary>
        /// <param name="t">角度(deg)</param>
        /// <returns></returns>
        private static double DegSinh(double t)
        {
            return Math.Sinh(Deg2Rad(t));
        }

        /// <summary>
        /// Asinをdegで返す
        /// </summary>
        /// <param name="sin">sine</param>
        /// <returns></returns>
        private static double DegAsin(double sin)
        {
            return Rad2Deg(Math.Asin(sin));
        }

        /// <summary>
        /// cosをdeg単位で計算
        /// </summary>
        /// <param name="t">角度(deg)</param>
        /// <returns></returns>
        private static double DegCos(double t)
        {
            return Math.Cos(Deg2Rad(t));
        }

        /// <summary>
        /// coshをdeg単位で計算
        /// </summary>
        /// <param name="t">角度(deg)</param>
        /// <returns></returns>
        private static double DegCosh(double t)
        {
            return Math.Cosh(Deg2Rad(t));
        }

        /// <summary>
        /// Atanをdegで返す
        /// </summary>
        /// <param name="tan">tangent</param>
        /// <returns></returns>
        private static double DegAtan(double tan)
        {
            return Rad2Deg(Math.Atan(tan));
        }

        /// <summary>
        /// Atanhをdegで返す
        /// </summary>
        /// <param name="tanh">hyperbolic tangent</param>
        /// <returns></returns>
        private static double DegAtanh(double tanh)
        {
            return Rad2Deg(Math.Atanh(tanh));
        }


        private static double Deg2Rad(double deg)
        {
            return Math.PI * deg / 180d;
        }

        private static double Rad2Deg(double rad)
        {
            return 180d * rad / Math.PI;
        }
        #endregion
    }

    public class LonLatXY 
    {
        /// <summary>
        /// 経度(deg)
        /// </summary>
        public double lon { get; set; }

        /// <summary>
        /// 緯度(deg)
        /// </summary>
        public double lat { get; set; }

        /// <summary>
        /// 平面直角座標系 X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 平面直角座標系 Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 平面直角座標の系情報
        /// </summary>
        public XYGroup group { get; set; }
    }

    public class XYGroup {

        /// <summary>
        /// 系番号
        /// </summary>
        public double No { get; set; }

        /// <summary>
        /// 系の経度原点(deg)
        /// </summary>
        public double o_lon { get; set; }

        /// <summary>
        /// 系の緯度原点
        /// </summary>
        public double o_lat { get; set; }
    }
}