using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;

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
            var s = new SortedSet<int>();
            for (int n = 0; n < 100_000; n++)
            {
                s.Add(n);
            }

            for (int i = 0; i < 10_000_000; i++)
            {
                var tmpVar = s.Min;
            }

            return new APIGatewayProxyResponse
            {
                Body = "Done",
                StatusCode = 200
            };
        }
    }
}

