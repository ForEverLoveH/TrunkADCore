using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrunkADCore.ADCoreSystem
{
    public class CommandData
    {

        public int str2int(string str)
        {
            int i = 0;
            if (null == str)
                return 0;
            int.TryParse(str, out i);
            return i;
        }

        public System.Drawing.Point XYString2Point(string str)
        {
            string[] strg = str.Split(',');
            if (null == strg)
            {
                return new System.Drawing.Point(0, 0);
            }

            if (strg.Length == 1)
            {
                return new System.Drawing.Point(str2int(strg[0]), 0);
            }

            System.Drawing.Point p = new System.Drawing.Point(str2int(strg[0]), str2int(strg[1]));
            return p;
        }

       public  void drawPointCross(Graphics g, System.Drawing.Point markerTop1, Pen pen)
        {

            g.DrawLine(pen, markerTop1.X - 15, markerTop1.Y, markerTop1.X + 15, markerTop1.Y);
            g.DrawLine(pen, markerTop1.X, markerTop1.Y - 15, markerTop1.X, markerTop1.Y + 15);
        }


       public  void drawPointText(Graphics g, String text, System.Drawing.Point point, Font drawFont, SolidBrush drawBrush,
            int directionx, int distancex, int directiony, int distancey)
        {
            //directiony 0 1 上下
            //directionx 0 1 左右

            int x = point.X;
            int y = point.Y;
            switch (directionx)
            {
                case 0:
                    x -= distancex;
                    break;
                case 1:
                    x += distancex;
                    break;
                default:
                    break;
            }

            switch (directiony)
            {
                case 0:
                    y -= distancey;
                    break;
                case 1:
                    y += distancey;
                    break;

                default:
                    break;
            }

            g.DrawString(text, drawFont, drawBrush, x, y);
        }
        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="cen">中心点</param>
        /// <param name="first">第一个点</param>
        /// <param name="second">第二个点</param>
        /// <returns></returns>
        public double Angle(System.Drawing.Point cen, System.Drawing.Point first, System.Drawing.Point second)
        {
            const double M_PI = 3.1415926535897;
            double ma_x = first.X - cen.X;
            double ma_y = first.Y - cen.Y;
            double mb_x = second.X - cen.X;
            double mb_y = second.Y - cen.Y;

            double v1 = (ma_x * mb_x) + (ma_y * mb_y);

            double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y);
            double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y);
            double cosM = v1 / (ma_val * mb_val);
            double angleAMB = Math.Acos(cosM) * 180 / M_PI;

            return angleAMB;
        }
        /// <summary>
        /// 根据长度,求两坐标之间的点坐标
        /// </summary>
        /// <param name="len">场地</param>
        /// <param name="baseStartP">第一个点</param>
        /// <param name="baseEndP">第二个点</param>
        /// <returns></returns>
        public System.Drawing.Point Len2xy(double len, System.Drawing.Point baseStartP, System.Drawing.Point baseEndP)
        {
            System.Drawing.Point pout = new System.Drawing.Point();

            double dy = baseEndP.Y - baseStartP.Y;
            double dx = baseEndP.X - baseStartP.X;

            // double top1k = dy / dx;
            double len1 = len;// topLength1;
            double xt1 = Math.Sqrt(len1 * len1 / (dy * dy / (dx * dx) + 1));
            double yt1 = dy * xt1 / dx;

            pout.X = baseStartP.X + (int)(xt1 + 0.5);
            pout.Y = baseStartP.Y + (int)(yt1 + 0.5);

            return pout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="len">长度</param>
        /// <param name="baseStartP">第一个点</param>
        /// <param name="baseEndP">第二个点</param>
        /// <returns></returns>
        public System.Drawing.Point len2yx(double len, System.Drawing.Point baseStartP, System.Drawing.Point baseEndP)
        {
            System.Drawing.Point pout = new System.Drawing.Point();

            double dx = baseEndP.X - baseStartP.X;
            double dy = baseEndP.Y - baseStartP.Y;
            double len1 = len;// topLength1;

            double yt1 = len1 / Math.Sqrt((dx * dx) / (dy * dy) + 1);
            double xt1 = (dx * yt1) / dy;


            pout.X = baseStartP.X + (int)(xt1 + 0.5);
            pout.Y = baseStartP.Y + (int)(yt1 + 0.5);

            return pout;
        }
        /// <summary>
        /// 求两点长度
        /// 
        /// </summary>
        /// <param name="StartP">第一个点</param>
        /// <param name="EndP">第二个点</param>
        /// <returns></returns>
        public double getLenForm2Point(System.Drawing.Point StartP, System.Drawing.Point EndP)
        {
            double len = Math.Sqrt((StartP.X - EndP.X) * (StartP.X - EndP.X) + (StartP.Y - EndP.Y) * (StartP.Y - EndP.Y));
            return len;
        }
        /// <summary>
        /// 判断点是否在俩点之间范围
        /// </summary>
        /// <param name="P1">第一个点</param>
        /// <param name="P2">第二个点</param>
        /// <param name="point">判断的点</param>
        /// <returns></returns>
        public int judgeSide(System.Drawing.Point P1, System.Drawing.Point P2, System.Drawing.Point point)
        {
            return ((P2.Y - P1.Y) * point.X + (P1.X - P2.X) * point.Y + (P2.X * P1.Y - P1.X * P2.Y));
        }
        int falseR0 = 0;
        /// <summary>
        /// 垂直距离
        /// </summary>
        /// <param name="gPnts"></param>
        /// <param name="mousePoint"></param>
        /// <param name="outTopP"></param>
        /// <param name="outBottomP"></param>
        /// <param name="outLen"></param>
        public void VerticalDistance(List<System.Drawing.Point[]> gPnts, System.Drawing.Point mousePoint,
            ref System.Drawing.Point outTopP, ref System.Drawing.Point outBottomP, ref double outLen, List<int[]> gfencePntsDisValue)
        {
            int nLen = gPnts.Count;
            System.Drawing.Point topStartP = gPnts[nLen - 1][1];
            System.Drawing.Point topEndP = gPnts[nLen - 1][3];
            System.Drawing.Point bottomStartP = gPnts[0][0];
            System.Drawing.Point bottomEndP = gPnts[0][2];
            //两点距离
            double fullLenPix = (gfencePntsDisValue[0][1] - gfencePntsDisValue[0][0]) * 10;
            //最小距离
            double baseLen = 0;
            //计算上标点和下标点到鼠标点的角度,180度成直线,
            //上下长度不同应该走不同的step长度
            //超过区域边界长度就break
            //或者上下两点的水平x值都超过鼠标点的xbreak
            double lenq = 0;
            double nowLen = 0;
            double div1 = 0.0f;

            System.Drawing.Point AngleMaxtopP = new System.Drawing.Point();
            System.Drawing.Point AngleMaxbotP = new System.Drawing.Point();
            //顶部长度
            double topLength1a = getLenForm2Point(topStartP, topEndP);
            //底部长度
            double bottomLength1a = getLenForm2Point(bottomStartP, bottomEndP);
            //实际长度和像素长度比值
            fullLenPix /= bottomLength1a;
            double AngleMin = 0;//最小角度
            double AngleMax = 180;
            double divStep = 0.00001f;//粗算
            double angleP12MTemp = 0;

            while (!((AngleMaxtopP.Y >= mousePoint.Y) && (AngleMaxbotP.Y >= mousePoint.Y)) && div1 <= 1)
            {

                try
                {
                    double angleP12M = 0;
                    int angleDecrementCount = 0;
                    div1 = div1 + divStep;
                    //上下标定线不停增加0.1f，直到角度>=0度
                    double topLen = topLength1a * div1;
                    double bottomLen = bottomLength1a * div1;
                    //求出顶部x，y值
                    System.Drawing.Point topP = len2yx(topLen, topStartP, topEndP);
                    //求出底部xy
                    System.Drawing.Point botP = len2yx(bottomLen, bottomStartP, bottomEndP);

                    angleP12M = Angle(mousePoint, topP, botP);

                    if (angleP12M < 120)
                    {
                        divStep = 0.05f;//精算
                    }
                    else if (angleP12M < 160)
                    {
                        divStep = 0.005f;//精算
                    }
                    else if (angleP12M < 170)
                    {
                        divStep = 0.001f;//精算
                    }
                    else if (angleP12M < 180)
                    {
                        divStep = 0.0001f;//精算
                    }
                    if (angleP12MTemp > angleP12M)
                    {
                        angleDecrementCount++;
                        if (angleDecrementCount > 10)
                        {
                            break;
                        }
                    }
                    else
                    {
                        angleDecrementCount = 0;
                    }
                    angleP12MTemp = angleP12M;
                    if (angleP12M <= AngleMax)
                    {
                        if (angleP12M > AngleMin)
                        {
                            AngleMin = angleP12M;
                            AngleMaxtopP = topP;
                            AngleMaxbotP = botP;
                        }
                    }
                    if (179 <= AngleMin && AngleMin <= 180)
                    {
                        outTopP = AngleMaxtopP;
                        outBottomP = AngleMaxbotP;
                        //计算距离
                        lenq = getLenForm2Point(bottomStartP, outBottomP);
                        nowLen = lenq * fullLenPix;
                        nowLen += (baseLen * 10);

                        outLen = nowLen;//测量长度
                                        //pictureBox1.Refresh();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }

            }

            falseR0++;
            outTopP = AngleMaxtopP;
            outBottomP = AngleMaxbotP;
            //计算距离
            lenq = getLenForm2Point(bottomStartP, outBottomP);
            nowLen = lenq * fullLenPix;
            nowLen += (baseLen * 10);
            outLen = nowLen;//测量长度

        }
        /// <summary>
        /// 测距新方法
        /// </summary>
        /// <param name="mousePoint"></param>
        /// <param name="topStartP"></param>
        /// <param name="topEndP"></param>
        /// <param name="bottomStartP"></param>
        /// <param name="bottomEndP"></param>
        /// <param name="outTopP"></param>
        /// <param name="outBottomP"></param>
        /// <param name="baseLen"></param>
        /// <param name="fullLenPix"></param>
        /// <param name="outLen"></param>
        public void DrawMousePointLine1(System.Drawing.Point mousePoint, System.Drawing.Point topStartP, System.Drawing.Point topEndP, System.Drawing.Point bottomStartP, System.Drawing.Point bottomEndP,
        ref System.Drawing.Point outTopP, ref System.Drawing.Point outBottomP, double baseLen, double fullLenPix, ref double outLen)
        {
            //计算上标点和下标点到鼠标点的角度,180度成直线,
            //上下长度不同应该走不同的step长度
            //超过区域边界长度就break
            //或者上下两点的水平x值都超过鼠标点的xbreak
            double lenq = 0;
            double nowLen = 0;
            double div1 = 0.0f;

            System.Drawing.Point AngleMaxtopP = new System.Drawing.Point();
            System.Drawing.Point AngleMaxbotP = new System.Drawing.Point();
            //顶部长度
            double topLength1a = getLenForm2Point(topStartP, topEndP);
            //底部长度
            double bottomLength1a = getLenForm2Point(bottomStartP, bottomEndP);
            //实际长度和像素长度比值
            fullLenPix /= bottomLength1a;
            double AngleMin = 0;//最小角度
            double AngleMax = 180;
            double divStep = 0.00001f;//粗算
            double angleP12MTemp = 0;
            while (!((AngleMaxtopP.X >= mousePoint.X) && (AngleMaxbotP.X >= mousePoint.X)) && div1 <= 1)
            {

                try
                {
                    double angleP12M = 0;
                    int angleDecrementCount = 0;
                    div1 = div1 + divStep;
                    //上下标定线不停增加0.1f，直到角度>=0度
                    double topLen = topLength1a * div1;
                    double bottomLen = bottomLength1a * div1;
                    //求出顶部x，y值
                    System.Drawing.Point topP = Len2xy(topLen, topStartP, topEndP);
                    //求出底部xy
                    System.Drawing.Point botP = Len2xy(bottomLen, bottomStartP, bottomEndP);

                    angleP12M = Angle(mousePoint, topP, botP);

                    if (angleP12M < 120)
                    {
                        divStep = 0.05f;//精算
                    }
                    else if (angleP12M < 160)
                    {
                        divStep = 0.005f;//精算
                    }
                    else if (angleP12M < 170)
                    {
                        divStep = 0.001f;//精算
                    }
                    else if (angleP12M < 180)
                    {
                        divStep = 0.0001f;//精算
                    }
                    if (angleP12MTemp > angleP12M)
                    {
                        angleDecrementCount++;
                        if (angleDecrementCount > 10)
                        {
                            break;
                        }
                    }
                    else
                    {
                        angleDecrementCount = 0;
                    }
                    angleP12MTemp = angleP12M;
                    if (angleP12M <= AngleMax)
                    {
                        if (angleP12M > AngleMin)
                        {
                            AngleMin = angleP12M;
                            AngleMaxtopP = topP;
                            AngleMaxbotP = botP;
                        }
                    }
                    if (179 <= AngleMin && AngleMin <= 180)
                    {
                        outTopP = AngleMaxtopP;
                        outBottomP = AngleMaxbotP;
                        //计算距离
                        lenq = getLenForm2Point(bottomStartP, outBottomP);
                        nowLen = lenq * fullLenPix;
                        nowLen += (baseLen * 10);
                        outLen = nowLen;//测量长度

                        return;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);

                }

            }

            falseR0++;
            outTopP = AngleMaxtopP;
            outBottomP = AngleMaxbotP;
            //计算距离
            lenq = getLenForm2Point(bottomStartP, outBottomP);
            nowLen = lenq * fullLenPix;
            nowLen += (baseLen * 10);
            outLen = nowLen;//测量长度
        }
    }
}

