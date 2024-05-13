using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class ScreenShot : MonoBehaviour
{
    public GameObject pic;
    public GameObject pic2;
    private bool isCoolingDown = false;

    public void TakeScreenshot()
    {
        if (!isCoolingDown)
        {
            StartCoroutine(CaptureScreenshotCoroutine());
        }
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        // 1. 隐藏鼠标光标和 pic
        isCoolingDown = true;
        Cursor.visible = false;
        pic.SetActive(false);
        pic2.SetActive(false);

        // 2. 等待下一帧渲染完成
        yield return null; // 或者 yield return new WaitForEndOfFrame();

        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string folderName = "三界宙CodeNames";
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, folderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filename = $"三界宙CodesName_{timestamp}.png";
        string filePath = Path.Combine(folderPath, filename);
        ScreenCapture.CaptureScreenshot(filePath);

        yield return new WaitForSeconds(0.1f); // 等待一小段时间确保文件写入完成
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + filePath))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Application.OpenURL(filePath); // 打开系统默认程序查看图片
            }
        }

        yield return null;
        // 4. 显示鼠标光标和 pic
        Cursor.visible = true;
        pic.SetActive(true);
        pic2.SetActive(true);
        // Debug.Log($"Screenshot saved to: {filePath}");
        yield return new WaitForSeconds(1f); // 1 秒冷却时间，可自行调整
        isCoolingDown = false;


    }
}
