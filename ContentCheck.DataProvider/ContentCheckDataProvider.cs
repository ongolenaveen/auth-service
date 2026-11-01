using Azure;
using Azure.AI.ContentSafety;
using Vision.Data.Interfaces;
using Vision.Data.Models;

namespace ContentCheck.DataProvider
{
    public class ContentCheckDataProvider(ContentSafetyClient contentSafetyClient) : IContentCheckDataProvider
    {
        public async Task<ContentCheckResult> CheckImageContentAsync(ImageContent model)
        {
            var image = new ContentSafetyImageData(BinaryData.FromBytes(model.Picture));
            var request = new AnalyzeImageOptions(image);

            Response<AnalyzeImageResult> response = await contentSafetyClient.AnalyzeImageAsync(request);

            return new ContentCheckResult
            {
                HateSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == ImageCategory.Hate)?.Severity ?? 0,
                SelfHarmSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == ImageCategory.SelfHarm)?.Severity ?? 0,
                SexualSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == ImageCategory.Sexual)?.Severity ?? 0,
                ViolenceSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == ImageCategory.Violence)?.Severity ?? 0
            };
        }
    }
}
