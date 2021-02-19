using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Amazon.S3;
using System;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NET5ContainerImage
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and returns both the upper and lower case version of the string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            //var s = new SortedSet<int>();
            //for (int n = 0; n < 100_000; n++)
            //{
            //    s.Add(n);
            //}

            //for (int i = 0; i < 10_000_000; i++)
            //{
            //    var tmpVar = s.Min;
            //}

            //return new APIGatewayProxyResponse
            //{
            //    Body = "Done",
            //    StatusCode = 200
            //};

            var bucketName = "net-benchmark-test"; //update the bucket name
            var objectKey = "d6a15c89-48ae-4bca-9001-d4486fc66bae.png"; //update the object key
            try
            {
                Stream imageStream = new MemoryStream();
                using (var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.EUCentral1))
                {
                    var getObjectResponse = s3Client.GetObjectAsync(new Amazon.S3.Model.GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = objectKey
                    });

                    using (Stream responseStream = getObjectResponse.Result.ResponseStream)
                    {
                        using (var image = Image.Load(responseStream))
                        {
                            // Create B&W thumbnail
                            image.Mutate(ctx => ctx.Grayscale().Resize(200, 200));
                            image.Save(imageStream, new JpegEncoder());
                            imageStream.Seek(0, SeekOrigin.Begin);
                        }
                    }


                    var thumbnailObjectKey = "thumbnails/" + objectKey;

                    LambdaLogger.Log("Thumbnail file Key: " + thumbnailObjectKey);

                    var putObjectResponse = s3Client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = thumbnailObjectKey,
                        InputStream = imageStream
                    });
                }

                return new APIGatewayProxyResponse
                {
                    Body = "Done",
                    StatusCode = 200
                };
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"Make sure object and bucket exist and your bucket is in the same region as this function");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    Body = "Error",
                    StatusCode = 500
                };
            }
        }
    }
}

