using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using static OpenCvSharp.Unity;
using UnityEngine.UI;
using System.IO;

public class ScannerManager : MonoBehaviour
{
    backgroundTransparentManager BTM;

    public RawImage grayRawImage;
    public RawImage blackRawImage;
    public RawImage cannyRawImage;
    public RawImage targetRawImage;

    public string capture_path;
    public string capture_name;

    public Texture2D baseTexture;
    public Texture2D target;

    [Tooltip("Mask와 크기가 같아야함. 2048, 1024")]
    public int width;
    public int height;

    [Range(0, 0.15f), Tooltip("정밀도")]
    public float epsilon;

    private void Start()
    {
        BTM = FindObjectOfType<backgroundTransparentManager>();
    }

    public void SetTexture2D(Texture2D _baseTexture)
    {
        baseTexture = GetTexture2D(_baseTexture);
        Mat ori = TextureToMat(_baseTexture);
        Mat grayMat = GrayMat(ori);
        Mat blackMat = BlackMat(grayMat);
        Mat cannyMat = CannyMat(blackMat);
        Point[] corners;
        findRect(cannyMat, out corners);
        Mat targetMat = TransFormImage(ori, corners);

        MakeTexture(grayMat, grayRawImage);
        MakeTexture(blackMat, blackRawImage);
        MakeTexture(cannyMat, cannyRawImage);
        //MakeTexture(targetMat, targetRawImage);
        target = MakeTexture(targetMat, targetRawImage);
        target = BTM.ScaleTexture(target, width, height);
        SavePNG(target, capture_path, capture_name);
    }

    Texture2D MakeTexture(Mat mat, RawImage raw) //Mat를 texture2d롤 변환
    {
        if (mat == null)
        {
            Debug.Log($"{mat}가 없습니다!");
            return null;
        }

        Texture2D texture2D = MatToTexture(mat);
        raw.texture = texture2D;

        return texture2D;
    }

    Mat GrayMat(Texture2D _baseTexture)
    {
        Mat grayMat = new Mat();
        grayMat = TextureToMat(_baseTexture);

        Cv2.CvtColor(grayMat, grayMat, ColorConversionCodes.BGR2GRAY);

        return grayMat;
    }

    Mat GrayMat(Mat _ori)
    {
        Mat grayMat = new Mat();

        Cv2.CvtColor(_ori, grayMat, ColorConversionCodes.BGR2GRAY);

        return grayMat;
    }

    Mat BlackMat(Mat _gray)
    {
        Mat blackMat = new Mat();
        blackMat = _gray.Threshold(100, 255, ThresholdTypes.Otsu);

        Cv2.BitwiseNot(blackMat, blackMat);

        return blackMat;
    }

    Mat CannyMat(Mat _black)
    {
        Mat cannyMat = new Mat();

        Cv2.Canny(_black, cannyMat, 50, 50);

        return cannyMat;
    }

    void findRect(Mat _canny, out Point[] corners)
    {
        corners = null;

        Point[][] contours;
        HierarchyIndex[] h;

        _canny.FindContours(out contours, out h, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        double maxArea = 0;
        for(int i = 0; i < contours.Length; i++)
        {
            double length = Cv2.ArcLength(contours[i], true);

            Point[] tmp = Cv2.ApproxPolyDP(contours[i], length * epsilon, true);

            double area = Cv2.ContourArea(contours[i]);
            if(tmp.Length == 4 && area > maxArea)
            {
                maxArea = area;
                corners = tmp;
            }
        }

        //if(corners != null)
        //{
        //    _ori.DrawContours(new Point[][] { corners }, 0, Scalar.Red, 5);

        //    for(int i = 0; i < corners.Length; i++)
        //    {
        //        _ori.Circle(corners[i], 20, Scalar.Blue, 5);
        //    }
        //}
    }

    Mat TransFormImage(Mat _ori, Point[] corners)
    {
        Mat target = new Mat();

        if (corners == null)
            return null;

        SwapCorners(corners);

        Point2f[] input = { corners[0], corners[1], corners[2], corners[3] };

        Point2f[] square = { new Point2f(0, 0), new Point2f(0, 255), new Point2f(255, 255), new Point2f(255, 0) };

        Mat transform = Cv2.GetPerspectiveTransform(input, square);

        Cv2.WarpPerspective(_ori, target, transform, new Size(256, 256));

        //int s = (int)(256 * 0.05);
        //int w = (int)(256 * 0.9);
        //OpenCvSharp.Rect innerRect = new OpenCvSharp.Rect(s, s, w, w);
        //target = target[innerRect];

        return target;
    }

    void SwapCorners(Point[] corners) // 위치 바꾸기(회전방지)
    {
        System.Array.Sort(corners, (a, b) => a.X.CompareTo(b.X));
        if (corners[0].Y > corners[1].Y)
        {
            Point i = corners[0];
            corners[0] = corners[1];
            corners[1] = i;
        }
        if (corners[3].Y > corners[2].Y)
        {
            Point i = corners[2];
            corners[2] = corners[3];
            corners[3] = i;
        }
    }

    Texture2D GetTexture2D(Texture2D _baseTexture)
    {
        Texture2D baseTexture = _baseTexture;
        capture_name = _baseTexture.name;

        return baseTexture;
    }

    public void SavePNG(Texture2D texture2D, string path, string _name)
    {
        if (texture2D == null)
        {
            Debug.Log($"{texture2D}가 없습니다!");
            return;
        }

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("저장경로가 비었습니다!");
            return;
        }

        if (Directory.Exists(path) == false)
        {
            Debug.Log("경로가 없습니다. 생성합니다");

            Directory.CreateDirectory(path);
        }

        texture2D.Apply();

        byte[] texture2DPNGbyte = texture2D.EncodeToPNG();

        string filepath = path + _name + ".png";

        File.WriteAllBytes(filepath, texture2DPNGbyte);
    }
}