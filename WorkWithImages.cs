using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScreenshotCaptureWithMouse.ScreenCapture;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace bot
{
    class WorkWithImages
    {
        public static System.Drawing.Bitmap BringProcessToFrontAndCaptureD3DWindow(Process[] process)
        {
            WorkWithProcess.BringProcessWindowToFront(process[0]);
            var capturedImage = Direct3DCapture.CaptureWindow(process[0].MainWindowHandle);
            return capturedImage;
        }

        public static System.Drawing.Bitmap BringProcessToFrontAndCaptureGDIWindow(Process[] process)
        {
            WorkWithProcess.BringProcessWindowToFront(process[0]);
            var capturedImage = CaptureScreen.CaptureWindow(process[0].MainWindowHandle);
            return capturedImage;
        }


        public static OpenCvSharp.Mat GetDiffInTwoImages(System.Drawing.Bitmap firstState, System.Drawing.Bitmap secondState)
        {

            Mat img1 = firstState.ToMat();
            Mat img2 = secondState.ToMat();
            Mat differenceBetweenImages = new Mat();
            Cv2.Absdiff(img1, img2, differenceBetweenImages);

            // Get the mask if difference greater than threshold
            Mat mask = new Mat(img1.Size(), MatType.CV_8UC1);

            int threshold = 70;
            Vec3b vectorOfColorsDifference;
            int curDifferenceLvl;
            
            Parallel.For(60, differenceBetweenImages.Rows - 200,
                   j =>
                   {
                       Parallel.For(30, differenceBetweenImages.Cols - 30,
                            i =>
                            {
                                vectorOfColorsDifference = differenceBetweenImages.At<Vec3b>(j, i);
                                curDifferenceLvl = (vectorOfColorsDifference[0] + vectorOfColorsDifference[1] + vectorOfColorsDifference[2]);
                                if (curDifferenceLvl > threshold)
                                {
                                    mask.Set<int>(j, i, 255);
                                }
                            });
                   });

            return mask;

            /*
            Mat result = new Mat();
            Cv2.BitwiseAnd(img2, img2, result, mask);
            Cv2.Threshold(result, result, 50, 255, ThresholdTypes.Binary);
            Cv2.CvtColor(result, result, ColorConversionCodes.BGR2GRAY);
            //GC.Collect();
            #region debug ImShow("res", res)
            //Cv2.ImShow("res", res);
            //Cv2.WaitKey(); 
            #endregion
            return result;
            */
        }

        public static OpenCvSharp.Point[][] FindCountoursAtImage(Mat image)
        {
            Cv2.FindContours(image, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            return contours;
        }

        public static Mat GetDiffInTwoImagesWithCustomBorders(System.Drawing.Bitmap firstState, System.Drawing.Bitmap secondState, int xBorder, int yBorder)
        {
            Mat img1 = firstState.ToMat();
            Mat img2 = secondState.ToMat();
            Mat differenceBetweenImages = new Mat();
            Cv2.Absdiff(img1, img2, differenceBetweenImages);

            // Get the mask if difference greater than threshold
            Mat mask = new Mat(img1.Size(), MatType.CV_8UC1);
            int threshold = 70;
            Vec3b vectorOfColorsDifference;
            int curDifferenceLvl;

            Parallel.For(xBorder, differenceBetweenImages.Rows - xBorder,
                   j =>
                   {
                       Parallel.For(yBorder, differenceBetweenImages.Cols - yBorder,
                            i =>
                            {
                                vectorOfColorsDifference = differenceBetweenImages.At<Vec3b>(j, i);
                                curDifferenceLvl = (vectorOfColorsDifference[0] + vectorOfColorsDifference[1] + vectorOfColorsDifference[2]);
                                if (curDifferenceLvl > threshold)
                                {
                                    mask.Set<int>(j, i, 255);
                                }
                            });
                   });

            Mat res = new Mat();

            Cv2.BitwiseAnd(img2, img2, res, mask);
            Cv2.Threshold(res, res, 50, 255, ThresholdTypes.Binary);
            Cv2.CvtColor(res, res, ColorConversionCodes.BGR2GRAY);

            #region debug ImShow("res", res)
            //Cv2.ImShow("res", res);
            //Cv2.WaitKey(); 
            #endregion
            return res;
        }

        public static Boolean IsImageMatchWithTemplate(System.Drawing.Bitmap monsterRef, System.Drawing.Bitmap monsterTemplate)
        {
            Mat reference = monsterRef.ToMat();
            Mat template = monsterTemplate.ToMat();
            Mat result = new Mat(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_32FC1);
            {
                //Convert input images to gray
                Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

                double threshold = 0.7;
                Cv2.MatchTemplate(gref, gtpl, result, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(result, result, threshold, 1.0, ThresholdTypes.Tozero);
                Cv2.MinMaxLoc(result, out _, out double maxval, out _, out _);

                if (maxval >= threshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static Point GetBiggestCountourCoordinates(OpenCvSharp.Point[][] pointsOfCountours)
        {
            Point mobCoordinate = new Point(622, 401);


            if (pointsOfCountours.Length > 0)
            {
                var biggestContour = pointsOfCountours[GetNoOfBiggestContour(pointsOfCountours)];
                var sortPointsOfContour = biggestContour.OrderByDescending(p => p.X).ToList();
                var arrayOfPoints = sortPointsOfContour.ToArray();
                mobCoordinate = arrayOfPoints[arrayOfPoints.Length / 2];
            }
            return mobCoordinate;

        }

        public static int GetNoOfBiggestContour(OpenCvSharp.Point[][] contours)
        {
            int biggestContourNo = 0;
            int ContourLength = 0;
            int count = 0;
            foreach (var contour in contours)
            {

                if (ContourLength < contours[count].Length)
                {
                    biggestContourNo = count;
                    ContourLength = contours[count].Length;
                }
                count++;
            }
            return biggestContourNo;
        }

    }
}
