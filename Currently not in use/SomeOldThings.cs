using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace bot
{
    class SomeOldThings
    {
        static void F(Mat result)
        {
            var res = result;
            Cv2.FindContours(res, out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchyIndexes, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

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

            var biggestContourRect = Cv2.BoundingRect(contours[biggestContourNo]);

            Cv2.CvtColor(res, res, ColorConversionCodes.GRAY2BGR);

            Cv2.Rectangle(res,
                new OpenCvSharp.Point(biggestContourRect.X - 10, biggestContourRect.Y - 10),
                new OpenCvSharp.Point(biggestContourRect.X + biggestContourRect.Width + 10, biggestContourRect.Y + biggestContourRect.Height + 10),
                new Scalar(0, 255, 0), 2);
        }

        System.Boolean IsMatchWithTemplate(System.Drawing.Bitmap monsterRef, System.Drawing.Bitmap monsterTemplate)
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
                Cv2.MinMaxLoc(result, out _, out double maxval, out _, out Point maxloc);

                if (maxval >= threshold)
                {
                    #region Отрисовка найденого сходства (дебажная функция)
                    
                    //Setup the rectangle to draw
                    Rect r = new Rect(new Point(maxloc.X, maxloc.Y), new Size(template.Width, template.Height));
                    //Draw a rectangle of the matching area
                    Cv2.Rectangle(reference, r, Scalar.LimeGreen, 2);

                    //Fill in the result Mat so you don't find the same area again in the MinMaxLoc
                    //Rect outRect;
                    //Cv2.FloodFill(result, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0), FloodFillFlags.Link4);

                    Cv2.ImShow("Matches", reference);
                    Cv2.WaitKey();

                    #endregion
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static void GetDiffAndPrintImg(string mainImage, string templateImage)
        {
            Mat img1 = new Mat(mainImage);
            Mat img2 = new Mat(templateImage);
            Mat differenceBetweenImages = new Mat();
            Cv2.Absdiff(img1, img2, differenceBetweenImages);

            // Get the mask if difference greater than threshold
            Mat mask = new Mat(img1.Size(), MatType.CV_8UC1);
            int threshold = 80;  // 0
            Vec3b vectorOfColorsDifference;
            int curDifferenceLvl;

            for (int j = 0; j < differenceBetweenImages.Rows; ++j)
            {
                for (int i = 0; i < differenceBetweenImages.Cols; ++i)
                {
                    vectorOfColorsDifference = differenceBetweenImages.At<Vec3b>(j, i);
                    curDifferenceLvl = (vectorOfColorsDifference[0] + vectorOfColorsDifference[1] + vectorOfColorsDifference[2]);
                    if (curDifferenceLvl > threshold)
                    {
                        mask.Set<int>(j, i, 255);
                    }
                }
            }

            Mat res = new Mat();

            Cv2.BitwiseAnd(img2, img2, res, mask);
            Cv2.Threshold(res, res, 50, 255, ThresholdTypes.Binary);
            Cv2.CvtColor(res, res, ColorConversionCodes.BGR2GRAY);
            Cv2.FindContours(res, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            int biggestContourNo = 0;
            int ContourLength = 0;
            for (int i = 0; i < contours[i].Length; i++)
            {
                if (ContourLength < contours[i].Length)
                {
                    biggestContourNo = i;
                    ContourLength = contours[i].Length;
                }
            }

            var biggestContourRect = Cv2.BoundingRect(contours[biggestContourNo]);

            Cv2.CvtColor(res, res, ColorConversionCodes.GRAY2BGR);

            Cv2.Rectangle(res,
                new OpenCvSharp.Point(biggestContourRect.X - 10, biggestContourRect.Y - 10),
               new OpenCvSharp.Point(biggestContourRect.X + biggestContourRect.Width + 10, biggestContourRect.Y + biggestContourRect.Height + 10),
              new Scalar(0, 255, 0), 2);
            Cv2.ImShow("res", res);
            Cv2.WaitKey();
        }
    }
}
