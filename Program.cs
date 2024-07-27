using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;

namespace renderer_3d
{
	class Program
	{
		static bool mouseHeld = true;
		static float deltaX,deltaY;
		static float sensitivity = 2f;
		static int width = 1280;
		static int height = 960;
		static projection projection = new projection(width,height);
		static bool faces = true;
		static bool rotObj = false;
		static float step = 0.025f;
		static Window w = new Window();
		static Application app = new Application();
		static float angleX = 0.0f, angleY = 0.0f, angleZ = 0.0f, rotateX = 0.0f, rotateY = 0.0f, rotateZ = 0.0f;
		static float posX = 0.0f, posY = 0.0f, posZ = -0.75f;
		static string obj = "";
		[STAThread]
		public static void Main(string[] args)
		{
			if(args.Length >= 1)
			{
				obj = args[0];
			}
			else
			{
				obj = "";
			}
			w.Width = width;
			w.MinWidth = width;
			w.MaxWidth = width;
			w.Height = height;
			w.MinHeight = height;
			w.MaxHeight = height;
			w.KeyDown += new KeyEventHandler(w_KeyDown);
			w.Title = "3d rendering";
			w.Content = projection.vector.render.i;
			w.Show();
			setMousePos(new Point(width/2,height/2));
			w.MouseMove += new MouseEventHandler(w_MouseMove);
			project();
			app.Run();
		}
		private static void setMousePos(Point position)
		{
			Point toWindow = w.PointToScreen(position);
			SetCursorPos(Convert.ToInt32(toWindow.X),Convert.ToInt32(toWindow.Y));
		}
		[DllImport("User32.dll")]
    	private static extern bool SetCursorPos(int X, int Y);
		private static void project()
		{
			angleX = (angleX + 720)%360;
			angleY = (angleY + 720)%360;
			angleZ = (angleZ + 720)%360;
			rotateX = (rotateX + 720)%360;
			rotateY = (rotateY + 720)%360;
			rotateZ = (rotateZ + 720)%360;
			projection.rotX = angleX;
			projection.rotY = angleY;
			projection.rotZ = angleZ;
			projection.camX = posX;
			projection.camY = posY;
			projection.camZ = posZ;
			projection.angX = rotateX;
			projection.angY = rotateY;
			projection.angZ = rotateZ;
			projection.drawObj(obj, faces);
		}
		private static void w_MouseMove(object sender, MouseEventArgs e)
		{
			if(mouseHeld)
			{
				deltaX = (float)(e.GetPosition(w).X - width/2);
				deltaY = (float)(e.GetPosition(w).Y - height/2);
				angleY -= deltaX*sensitivity*step;
				angleX += deltaY*sensitivity*step;
				setMousePos(new Point(width/2,height/2));
				project();
			}
		}
		private static void w_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					rotateX -= 2.5f;
					break;
				case Key.Down:
					rotateX += 2.5f;
					break;
				case Key.Left:
					rotateY += 2.5f;
					break;
				case Key.Right:
					rotateY -= 2.5f;
					break;
				case Key.D:
					posZ = (float)(posZ+(Math.Sin(angleY*(Math.PI/180))*step));
					posX = (float)(posX+(Math.Cos(angleY*(Math.PI/180))*step));
					break;
				case Key.A:
					posZ = (float)(posZ+(Math.Sin(angleY*(Math.PI/180))*-step));
					posX = (float)(posX+(Math.Cos(angleY*(Math.PI/180))*-step));
					break;
				case Key.S:
					posZ = (float)(posZ+(Math.Sin((angleY-90)*(Math.PI/180))*step));
					posX = (float)(posX+(Math.Cos((angleY-90)*(Math.PI/180))*step));
					break;
				case Key.W:
					posZ = (float)(posZ+(Math.Sin((angleY-90)*(Math.PI/180))*-step));
					posX = (float)(posX+(Math.Cos((angleY-90)*(Math.PI/180))*-step));
					break;
				case Key.Space:
					posY -= step;
					break;
				case Key.LeftShift:
					posY += step;
					break;
				case Key.Tab:
					faces = !faces;
					break;
				case Key.Z:
					rotObj = !rotObj;
					break;
				case Key.PageUp:
					step *=2;
					break;
				case Key.PageDown:
					step /=2;
					break;
				case Key.Escape:
					mouseHeld = !mouseHeld;
					break;
				case Key.R:
					angleX = 0.0f;
					angleY = 0.0f;
					angleZ = 0.0f;
					rotateX = 0.0f;
					rotateY = 0.0f;
					rotateZ = 0.0f;
					posX = 0.0f;
					posY = 0.0f;
					posZ = -0.75f;
					break;
			}
			project();
		}
	}
}