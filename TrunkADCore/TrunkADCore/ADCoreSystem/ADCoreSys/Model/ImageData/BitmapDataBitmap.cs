using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class BitmapDataBitmap
    {
        Bitmap source = null;
        IntPtr ptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int size { get; private set; }
        public byte[] srcArray { get; private set; }
        public BitmapDataBitmap(Bitmap bmp)
        {
            Width = bmp.Width;
            Height = bmp.Height;
            source = bmp;
            size = Width * Height * 3;
            //缓冲区数组
            srcArray = new byte[size];
        }

        public void LockBits()
        {
            try
            {
                bitmapData = source.LockBits(
                    new Rectangle(0, 0, Width, Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);
                unsafe
                {
                    ptr = bitmapData.Scan0;
                    //把像素值复制到缓冲区
                    Marshal.Copy(ptr, srcArray, 0, size);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void UnlockBits()
        {
            try
            {
                //从缓冲区复制回BitmapData
                Marshal.Copy(srcArray, 0, ptr, size);
                //从内存中解锁
                source.UnlockBits(bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void test()
        {
            int p;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //定位像素点位置
                    p = j * Width * 3 + i * 3;
                    //计算灰度值
                    byte color = (byte)((srcArray[p] + srcArray[p + 1] + srcArray[p + 2]) / 3);
                    srcArray[p] = srcArray[p + 1] = srcArray[p + 2] = color;
                }
            }
        }

    }
}

