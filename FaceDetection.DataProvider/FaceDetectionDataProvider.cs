using Azure;
using Azure.AI.Vision.Face;
using Microsoft.Extensions.Logging;
using Vision.Data.Interfaces;
using Vision.Data.Models;

namespace FaceDetection.DataProvider
{
    public class FaceDetectionDataProvider(FaceClient faceClient, FaceAdministrationClient faceAdministrationClient, ILogger<FaceDetectionDataProvider> logger) : IFaceDetectionDataProvider
    {
        private readonly string _largePersonGroupId = Guid.NewGuid().ToString();

        public async Task<bool> DetectFace(VisionRequest visionRequest)
        {
            var imageBaseUrl =
                "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/Face/images/";
            // Implementation for face detection
            var recognitionModel4 = FaceRecognitionModel.Recognition04;
            // Identify - recognize a face(s) in a large person group (a large person group is created in this example).
            await IdentifyInLargePersonGroup(imageBaseUrl, recognitionModel4);
            return true;
        }


        // Detect faces from image url for recognition purposes. This is a helper method for other functions in this quickstart.
        // Parameter `returnFaceId` of `DetectAsync` must be set to `true` (by default) for recognition purposes.
        // Parameter `returnFaceAttributes` is set to include the QualityForRecognition attribute. 
        // Recognition model must be set to recognition_03 or recognition_04 as a result.
        // Result faces with insufficient quality for recognition are filtered out. 
        // The field `faceId` in returned `DetectedFace`s will be used in Verify and Identify.
        // It will expire 24 hours after the detection call.
        private async Task<List<FaceDetectionResult>> DetectFaceRecognize(string url, FaceRecognitionModel recognitionModel)
        {
            // Detect faces from image URL.
            var response = await faceClient.DetectAsync(new Uri(url), FaceDetectionModel.Detection03, recognitionModel, true, [FaceAttributeType.QualityForRecognition]);
            IReadOnlyList<FaceDetectionResult> detectedFaces = response.Value;
            List<FaceDetectionResult> sufficientQualityFaces = new List<FaceDetectionResult>();
            foreach (FaceDetectionResult detectedFace in detectedFaces)
            {
                QualityForRecognition? faceQualityForRecognition = detectedFace.FaceAttributes.QualityForRecognition;
                if (faceQualityForRecognition.HasValue && (faceQualityForRecognition.Value != QualityForRecognition.Low))
                {
                    sufficientQualityFaces.Add(detectedFace);
                }
            }
            Console.WriteLine($"{detectedFaces.Count} face(s) with {sufficientQualityFaces.Count} having sufficient quality for recognition detected from image `{Path.GetFileName(url)}`");

            return sufficientQualityFaces;
        }

        /*
        * IDENTIFY FACES
        * To identify faces, you need to create and define a large person group.
        * The Identify operation takes one or several face IDs from DetectedFace or PersistedFace and a LargePersonGroup and returns 
        * a list of Person objects that each face might belong to. Returned Person objects are wrapped as Candidate objects, 
        * which have a prediction confidence value.
        */
        public async Task IdentifyInLargePersonGroup( string url, FaceRecognitionModel recognitionModel)
        {
            logger.LogInformation("========IDENTIFY FACES========");

            // Create a dictionary for all your images, grouping similar ones under the same key.
            Dictionary<string, string[]> personDictionary =
                new Dictionary<string, string[]>
                    { { "Family1-Dad", ["Family1-Dad1.jpg", "Family1-Dad2.jpg"] },
                      { "Family1-Mom", ["Family1-Mom1.jpg", "Family1-Mom2.jpg"] },
                      { "Family1-Son", ["Family1-Son1.jpg", "Family1-Son2.jpg"] }
                    };
            // A group photo that includes some of the persons you seek to identify from your dictionary.
            var sourceImageFileName = "identification1.jpg";

            // Create a large person group.
            logger.LogInformation($"Create a person group ({_largePersonGroupId}).");
            var largePersonGroupClient = faceAdministrationClient.GetLargePersonGroupClient(_largePersonGroupId);
            await largePersonGroupClient.CreateAsync(_largePersonGroupId, recognitionModel: recognitionModel);
            // The similar faces will be grouped into a single large person group person.
            foreach (var groupedFace in personDictionary.Keys)
            {
                // Limit TPS
                await Task.Delay(250);
                var createPersonResponse = await largePersonGroupClient.CreatePersonAsync(groupedFace);
                var personId = createPersonResponse.Value.PersonId;
                logger.LogInformation($"Create a person group person '{groupedFace}'.");

                // Add face to the large person group person.
                foreach (var similarImage in personDictionary[groupedFace])
                {
                    logger.LogInformation($"Check whether image is of sufficient quality for recognition");
                    var detectResponse = await faceClient.DetectAsync(new Uri($"{url}{similarImage}"), FaceDetectionModel.Detection03, recognitionModel, false, [FaceAttributeType.QualityForRecognition]);
                    IReadOnlyList<FaceDetectionResult> facesInImage = detectResponse.Value;
                    var sufficientQuality = true;
                    foreach (var face in facesInImage)
                    {
                        var faceQualityForRecognition = face.FaceAttributes.QualityForRecognition;
                        //  Only "high" quality images are recommended for person enrollment
                        if (!faceQualityForRecognition.HasValue ||
                            (faceQualityForRecognition.Value == QualityForRecognition.High)) continue;
                        sufficientQuality = false;
                        break;
                    }

                    if (!sufficientQuality)
                    {
                        logger.LogInformation($"Image `{similarImage}` does not have sufficient quality for recognition.");
                        continue;
                    }

                    if (facesInImage.Count != 1)
                    {
                        logger.LogInformation($"Image `{similarImage}` does not contain exactly one face.");
                        continue;
                    }

                    // add face to the large person group
                    logger.LogInformation($"Add face to the person group person({groupedFace}) from image `{similarImage}`");
                    await largePersonGroupClient.AddFaceAsync(personId, new Uri($"{url}{similarImage}"), detectionModel: FaceDetectionModel.Detection03);
                }
            }

            // Start to train the large person group.
            logger.LogInformation("");
            logger.LogInformation($"Train person group {_largePersonGroupId}.");
            Operation operation = await largePersonGroupClient.TrainAsync(WaitUntil.Completed);

            // Wait until the training is completed.
            await operation.WaitForCompletionResponseAsync();
            logger.LogInformation("Training status: succeeded.");
            logger.LogInformation("----");

            logger.LogInformation("Pausing for 60 seconds to avoid triggering rate limit on free account...");
            await Task.Delay(60000);

            List<Guid> sourceFaceIds = new List<Guid>();
            // Detect faces from source image url.
            List<FaceDetectionResult> detectedFaces = await DetectFaceRecognize($"{url}{sourceImageFileName}", recognitionModel);

            // Add detected faceId to sourceFaceIds.
            foreach (FaceDetectionResult detectedFace in detectedFaces)
            {
                if (detectedFace.FaceId != null) sourceFaceIds.Add(detectedFace.FaceId.Value);
            }

            // Identify the faces in a large person group.
            var identifyResponse = await faceClient.IdentifyFromLargePersonGroupAsync(sourceFaceIds, _largePersonGroupId);
            IReadOnlyList<FaceIdentificationResult> identifyResults = identifyResponse.Value;
            foreach (FaceIdentificationResult identifyResult in identifyResults)
            {
                if (identifyResult.Candidates.Count == 0)
                {
                    logger.LogInformation($"No person is identified for the face in: {sourceImageFileName} - {identifyResult.FaceId},");
                    continue;
                }

                FaceIdentificationCandidate candidate = identifyResult.Candidates.First();
                var getPersonResponse = await largePersonGroupClient.GetPersonAsync(candidate.PersonId);
                string personName = getPersonResponse.Value.Name;
                logger.LogInformation($"Person '{personName}' is identified for the face in: {sourceImageFileName} - {identifyResult.FaceId}," + $" confidence: {candidate.Confidence}.");

                var verifyResponse = await faceClient.VerifyFromLargePersonGroupAsync(identifyResult.FaceId, _largePersonGroupId, candidate.PersonId);
                FaceVerificationResult verifyResult = verifyResponse.Value;
                logger.LogInformation($"Verification result: is a match? {verifyResult.IsIdentical}. confidence: {verifyResult.Confidence}");
            }
            logger.LogInformation("----");

            // Delete large person group.
            logger.LogInformation("========DELETE PERSON GROUP========");
            logger.LogInformation("----");
            await largePersonGroupClient.DeleteAsync();
            logger.LogInformation($"Deleted the person group {_largePersonGroupId}.");
            logger.LogInformation("----");
        }
    }
}
