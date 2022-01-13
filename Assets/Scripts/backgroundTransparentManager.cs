using UnityEngine;
using System.Collections;
using OpenCvSharp;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using static OpenCvSharp.Unity;
using System.IO;

public class backgroundTransparentManager : MonoBehaviour
{
    #region variable
    DataManager DM;
    CameraManager CM;
    SettingManager Setting;
    ScannerManager SM;

    Texture2D m_texture;

    public RawImage m_Image_binarization;
    public RawImage m_image_mask;
    public RawImage m_image_backgroundTransparent;

    [HideInInspector] public Texture2D target;

    bool saveMask;

    //[HideInInspector]
    public string mask_path; //mask를 저장할 경로
    //[HideInInspector]
    public string mask_name; //mask를 저장할 이름

    public string fin_path; //최종 이미지 저장 경로

    public string fin_name; //최종 이미지 저장 이름

    public double v_thresh = 180;
    public double v_maxval = 255;
    public Texture2D texture2D;
    #endregion

    private void Start()
    {
        SM = FindObjectOfType<ScannerManager>();
        DM = FindObjectOfType<DataManager>();
        CM = FindObjectOfType<CameraManager>();
        Setting = FindObjectOfType<SettingManager>();
    }

    private void Update()
    {
        IsSaveMask();
    }

    void IsSaveMask() //세팅에서 bool값 받아오기
    {
        saveMask = Setting.saveMask;
    }

    //Mat mask;
    public void backgroundTransparent()
    {
        //m_texture = SM.target;
        //mask_name = SM.capture_name;
        //fin_name = SM.capture_name;

        m_texture = SM.target;
        mask_name = SM.capture_name;
        fin_name = SM.capture_name;

        #region load texture
        Mat origin = TextureToMat(m_texture);
        //m_image_origin.texture = MatToTexture(origin);
        #endregion

        Mat origin2 = new Mat();
        if (saveMask == false)
        {
            //Texture2D texture2D = null;
            texture2D = null;

            for (int i = 0; i < DM.QR.Count; i++)
            {
                if (CM.QRtext == DM.QR[i])
                {
                    texture2D = DM.MASK[i];
                    //texture2D = ScaleTexture(texture2D, 2048, 1024); // 사이즈조절
                    break;
                }
            }
            if (texture2D == null)
            {
                Debug.LogWarning("QR코드를 읽는데 실패했거나 QR코드값이 없습니다.");
                return;
            }

            m_texture.Resize(texture2D.width, texture2D.height);
            origin2 = TextureToMat(texture2D);
        }

        #region  Gray scale image
        Mat grayMat = new Mat();
        if (saveMask == true)
            Cv2.CvtColor(origin, grayMat, ColorConversionCodes.BGR2GRAY);
        else
            Cv2.CvtColor(origin2, grayMat, ColorConversionCodes.BGR2GRAY);
        #endregion

        #region Find Edge
        Mat thresh = new Mat();
        Cv2.Threshold(grayMat, thresh, v_thresh, v_maxval, ThresholdTypes.BinaryInv);
        m_Image_binarization.texture = MatToTexture(thresh);
        #endregion

        #region Create Mask 
        Mat Mask = TextureToMat(MatToTexture(grayMat));
        Point[][] contours;
        HierarchyIndex[] hierarchy;
        Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);
        for (int i = 0; i < contours.Length; i++)
        {
            Cv2.DrawContours(Mask, new Point[][] { contours[i] }, 0, new Scalar(0, 0, 0), -1);
        }
        Mask = Mask.CvtColor(ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(Mask, Mask, v_thresh, v_maxval, ThresholdTypes.Binary);
        m_image_mask.texture = MatToTexture(Mask);
        #endregion

        //세팅에서 mask저장 체크시 활성화
        if (saveMask == true)
            SM.SavePNG(MatToTexture(Mask), mask_path, mask_name);

        #region TransparentBackground
        Mat transparent = origin.CvtColor(ColorConversionCodes.BGR2BGRA);
        unsafe
        {
            byte* b_transparent = transparent.DataPointer;
            byte* b_mask = Mask.DataPointer;
            float pixelCount = transparent.Height * transparent.Width;

            for (int i = 0; i < pixelCount; i++)
            {
                if (b_mask[0] == 255)
                {
                    b_transparent[0] = 0;
                    b_transparent[1] = 0;
                    b_transparent[2] = 0;
                    b_transparent[3] = 0;
                }
                b_transparent = b_transparent + 4;
                b_mask = b_mask + 1;
            }
        }
        target = MatToTexture(transparent);
        target = ScaleTexture(target, 3508, 2480); //이미지를 용지 크기로 사이즈 조절
        m_image_backgroundTransparent.texture = target;
        #endregion
        SM.SavePNG(target, fin_path, fin_name);
    }

    //텍스쳐 사이즈 조정
    public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    Mat MakeMask()
    {
        m_texture = SM.target;

        #region load texture
        Mat origin = TextureToMat(m_texture);
        //m_image_origin.texture = MatToTexture(origin);
        #endregion

        #region  Gray scale image
        Mat grayMat = new Mat();
        Cv2.CvtColor(origin, grayMat, ColorConversionCodes.BGR2GRAY);
        #endregion

        #region Find Edge
        Mat thresh = new Mat();
        Cv2.Threshold(grayMat, thresh, v_thresh, v_maxval, ThresholdTypes.BinaryInv);
        m_Image_binarization.texture = MatToTexture(thresh);
        #endregion

        #region Create Mask 
        Mat Mask = TextureToMat(MatToTexture(grayMat));
        Point[][] contours; HierarchyIndex[] hierarchy;
        Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);
        for (int i = 0; i < contours.Length; i++)
        {
            Cv2.DrawContours(Mask, new Point[][] { contours[i] }, 0, new Scalar(0, 0, 0), -1);
        }
        Mask = Mask.CvtColor(ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(Mask, Mask, v_thresh, v_maxval, ThresholdTypes.Binary);
        m_image_mask.texture = MatToTexture(Mask);
        #endregion

        //세팅에서 mask저장 체크시 활성화
        SM.SavePNG(MatToTexture(Mask), mask_path, mask_name);

        return Mask;
    }


    Mat ComparisonQR() //QR코드 값 비교하고 mask가져오기
    {
        Texture2D texture2D = null;

        for (int i = 0; i < DM.QR.Count; i++)
        {
            if (CM.QRtext == DM.QR[i])
            {
                texture2D = DM.MASK[i];
                break;
            }
        }

        m_texture = texture2D;

        #region load texture
        Mat origin = TextureToMat(m_texture);
        //m_image_origin.texture = MatToTexture(origin);
        #endregion

        #region  Gray scale image
        Mat grayMat = new Mat();
        Cv2.CvtColor(origin, grayMat, ColorConversionCodes.BGR2GRAY);
        #endregion

        #region Find Edge
        Mat thresh = new Mat();
        Cv2.Threshold(grayMat, thresh, v_thresh, v_maxval, ThresholdTypes.BinaryInv);
        m_Image_binarization.texture = MatToTexture(thresh);
        #endregion

        #region Create Mask 
        Mat Mask = TextureToMat(MatToTexture(grayMat));
        Point[][] contours; HierarchyIndex[] hierarchy;
        Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);
        for (int i = 0; i < contours.Length; i++)
        {
            Cv2.DrawContours(Mask, new Point[][] { contours[i] }, 0, new Scalar(0, 0, 0), -1);
        }
        Mask = Mask.CvtColor(ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(Mask, Mask, v_thresh, v_maxval, ThresholdTypes.Binary);
        m_image_mask.texture = MatToTexture(Mask);
        #endregion

        return Mask;
    }
}
