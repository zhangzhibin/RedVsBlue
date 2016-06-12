using UnityEngine;
using System.Collections;
//using UnityEditor;

namespace TDGame{
	public class Tools {
		public static Vector3 GetRendererSize(GameObject obj){
			SpriteRenderer sr = obj.GetComponent<SpriteRenderer> ();
			if (sr) {
				return sr.bounds.size;
			}
			
			return new Vector3 ();
		}

		public static Texture2D CaptureScreenshot2(Rect rect, bool save=false)
		{  
			// 先创建一个的空纹理，大小可根据实现需要来设置
			Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);  

			// 读取屏幕像素信息并存储为纹理数据，  
			screenShot.ReadPixels(rect, 0, 0);  
			screenShot.Apply();  

			if(save)
			{
				// 然后将这些纹理数据，成一个png图片文件  
				byte[] bytes = screenShot.EncodeToPNG();  
				string filename = Application.dataPath + "/Screenshot.png";  
				System.IO.File.WriteAllBytes(filename, bytes);  
				Debug.Log(string.Format("截屏了一张图片: {0}", filename));  
			}

			// 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
			return screenShot;  
		}  

		public static Texture2D CaptureCamera(Camera camera, Rect rect, bool save=false)   
		{  
			// 创建一个RenderTexture对象  
			RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);  
			// 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
			camera.targetTexture = rt;  
			camera.Render();  
			//ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
			//ps: camera2.targetTexture = rt;  
			//ps: camera2.Render();  
			//ps: -------------------------------------------------------------------  

			// 激活这个rt, 并从中中读取像素。  
			RenderTexture.active = rt;  
			Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);  
			screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
			screenShot.Apply();  

			// 重置相关参数，以使用camera继续在屏幕上显示  
			camera.targetTexture = null;  
			//ps: camera2.targetTexture = null;  
			RenderTexture.active = null; // JC: added to avoid errors  
			GameObject.Destroy(rt);  

			if(save)
			{
				// 最后将这些纹理数据，成一个png图片文件  
				byte[] bytes = screenShot.EncodeToPNG();  
				string filename = Application.dataPath + "/Screenshot.png";  
				System.IO.File.WriteAllBytes(filename, bytes);  
				Debug.Log(string.Format("截屏了一张照片: {0}", filename));  
			}

			return screenShot;  
		}  
	}
}