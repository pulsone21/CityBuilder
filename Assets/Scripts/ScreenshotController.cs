using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenshotController : MonoBehaviour
{
    [SerializeField] private Camera myCamera;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Taking Screenshot");
            StartCoroutine(CorutineScreenshot());
        }
    }

    private IEnumerator CorutineScreenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Rect rect = new Rect(0, 0, width, height);
        screenshotTexture.ReadPixels(rect, 0, 0);
        screenshotTexture.Apply();

        byte[] byteArray = screenshotTexture.EncodeToPNG();

        string imageCount = CountImages();

        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/Screenshot" + imageCount + ".png", byteArray);
    }

    private string CountImages()
    {
        //Application.dataPath + "/Screenshots";
        DirectoryInfo d = new DirectoryInfo(Application.dataPath + "/Screenshots");
        int count = 0;

        FileInfo[] fis = d.GetFiles();

        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Contains("png"))
            {
                count++;
            }
        }
        string outString = count.ToString();
        if (outString.Length < 2)
        {
            outString = "0" + outString;
        }
        return outString;
    }

}
