using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using UnityEngine;


/*
*	杖の先端の色を取得し、指定のmmapに書き込むための関数
*/
public class UpdateBallColor : MonoBehaviour
{
	public string mmapPath = "C:/Users/{ユーザー名}/color.txt"; // パスは適宜変更する。

	// mmap に書き込むバッファサイズ（3つの double = 24 バイト）
	private const long MmapSize = sizeof(double) * 3;

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StartCoroutine(CaptureColorAndWrite());
		}
	}

	private System.Collections.IEnumerator CaptureColorAndWrite()
	{
		yield return new WaitForEndOfFrame();

		Vector2 mousePos = Input.mousePosition;
		int x = Mathf.FloorToInt(mousePos.x);
		int y = Mathf.FloorToInt(mousePos.y);

		Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
		tex.Apply();
		Color pixelColor = tex.GetPixel(0, 0);
		Destroy(tex);

		double r = Math.Round(pixelColor.r * 255.0);
		double g = Math.Round(pixelColor.g * 255.0);
		double b = Math.Round(pixelColor.b * 255.0);

		// B, G, R の順番で書き込む（OpenCV 等と同じ BGR 系に合わせる想定）
		WriteColorToMmap(b, g, r);

		Debug.Log($"クリック色 (Unity Color) = R:{pixelColor.r:F3}, G:{pixelColor.g:F3}, B:{pixelColor.b:F3}");
		Debug.Log($"mmap に書き込んだ色 (0-255) = B:{b}, G:{g}, R:{r}");
	}

	private void WriteColorToMmap(double b, double g, double r)
	{
		try
		{
			using (var mmf = MemoryMappedFile.CreateFromFile(mmapPath, FileMode.Open, null, MmapSize))
			{
				using (var accessor = mmf.CreateViewAccessor(0, MmapSize, MemoryMappedFileAccess.Write))
				{
					// 先頭から 3 つの double を順に書き込む
					accessor.Write(0 * sizeof(double), b);
					accessor.Write(1 * sizeof(double), g);
					accessor.Write(2 * sizeof(double), r);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError($"mmap への書き込みに失敗: {ex.Message}");
		}
	}
}
