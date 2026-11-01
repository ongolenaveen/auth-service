using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Logging;
using Vision.Data.Interfaces;
using Vision.Data.Models;

namespace Ocr.DataProvider
{
    public class OcrDataProvider(ImageAnalysisClient imageAnalysisClient, ILogger<OcrDataProvider> logger) : IOcrDataProvider
    {
        public async Task<List<OcrResult>> CheckImageContentAsync(VisionRequest model)
        {
            var results = new List<OcrResult>();

            logger.LogInformation("Finding text in the received images.");

            ImageAnalysisResult result = await imageAnalysisClient.AnalyzeAsync(BinaryData.FromBytes(model.Picture),
                VisualFeatures.Read ,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            if (result.Read?.Blocks == null || result.Read.Blocks.Count <= 0)
                return results;

            var ocrResult = new OcrResult
            {
                CaptionText = result.Caption?.Text,
                Confidence = result.Caption?.Confidence.ToString("F4")
            };

            Console.WriteLine("Image analysis results:");
            Console.WriteLine(" Caption:");
            Console.WriteLine($"'{result.Caption?.Text}', Confidence {result.Caption?.Confidence:F4}");

            Console.WriteLine(" Read:");
            foreach (var block in result.Read.Blocks)
            {
                foreach (var line in block.Lines)
                {
                    Console.WriteLine($"Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
                    ocrResult.LineText = line.Text;
                    foreach (var word in line.Words)
                    {
                        Console.WriteLine($"Word: '{word.Text}', Confidence {word.Confidence:#.####}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
                        var ocrWordResult = new OcrWordResult
                        {
                            Text = word.Text,
                            Confidence = word.Confidence.ToString("F4"),
                            X = word.BoundingPolygon[0].X,
                            Y = word.BoundingPolygon[1].Y
                        };
                        ocrResult.Words.Add(ocrWordResult);
                    }
                }
            }
            results.Add(ocrResult);
            return results;
        }
    }
}
