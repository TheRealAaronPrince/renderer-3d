using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace graphics
{
	public partial class render
	{
		WriteableBitmap writeableBitmap;
		public Image i = new Image();
		public int width;
		public int height;
		//RGBA value array for the pixels
		public byte[] pixelBuffer;
		public render(int ww = 320, int hh = 240)
		{
			i = new Image();
			i.Stretch = Stretch.None;
			i.HorizontalAlignment = HorizontalAlignment.Left;
			i.VerticalAlignment = VerticalAlignment.Top;
			RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(i, EdgeMode.Aliased);
			width = ww;
			height = hh;
			writeableBitmap = new WriteableBitmap(width,height,96,96,PixelFormats.Bgr32,null);
			i.Source = writeableBitmap;
			pixelBuffer = new byte[4*width*height];
		}
		//loop to set a default color for every pixel
		public void clearImg(int colR, int colG, int colB)
		{
			for( int i = 0; i < pixelBuffer.Length-1; i+=4)
			{
				pixelBuffer[i + 0] = (byte)colB;
				pixelBuffer[i + 1] = (byte)colG;
				pixelBuffer[i + 2] = (byte)colR;
			}
		}
		//converting the array to an image
		public void update()
		{
			writeableBitmap.Lock();
			// Get a pointer to the back buffer.
			IntPtr pBackBuffer = writeableBitmap.BackBuffer;
			Marshal.Copy(pixelBuffer,0,pBackBuffer,pixelBuffer.Length);
			writeableBitmap.AddDirtyRect(new Int32Rect(0,0,width,height));
			writeableBitmap.Unlock();
		}
	}
}