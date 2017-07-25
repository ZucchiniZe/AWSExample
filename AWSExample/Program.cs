using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace AWSExample
{
    public class Program
    {
        private static IAmazonS3 _client;
        private static IConfiguration _config;

        public static void Main(string[] args)
        {
            // get the config file
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json");

            _config = configBuilder.Build();

            _client = _config.GetAWSOptions().CreateServiceClient<IAmazonS3>();

            var bucketName = _config["bucketName"];

            ListBuckets().GetAwaiter().GetResult();
            CreateFileInBucket("test/file.txt", "this is a text file", bucketName).GetAwaiter().GetResult();
            GetBucketContents(bucketName).GetAwaiter().GetResult();
        }

        private static async Task ListBuckets()
        {
            var response = await _client.ListBucketsAsync();
            foreach (var bucket in response.Buckets)
            {
                Console.WriteLine("You own a bucket with name: {0}", bucket.BucketName);
            }
        }

        private static async Task GetBucketContents(string bucketName)
        {
            Console.WriteLine($"trying to read the contents of {bucketName}");
            var response = await _client.ListObjectsAsync(bucketName);
            foreach (var entry in response.S3Objects)
            {
                Console.WriteLine($"{entry.Key}\t{entry.LastModified}");
            }
        }

        private static async Task CreateFileInBucket(string key, string contents, string bucketName)
        {
            var request = new PutObjectRequest
            {
                ContentBody = contents,
                BucketName = bucketName,
                Key = key
            };
            var response = await _client.PutObjectAsync(request);
            Console.WriteLine($"response statuscode: {response.HttpStatusCode}");
        }
    }
}