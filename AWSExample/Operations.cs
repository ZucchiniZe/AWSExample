using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace AWSExample
{
    public static partial class Program
    {
        private static async Task ListOwnedBuckets()
        {
            var response = await _client.ListBucketsAsync();
            foreach (var bucket in response.Buckets)
            {
                Console.WriteLine("You own a bucket with name: {0}", bucket.BucketName);
            }
        }

        private static async Task GetBucketContents(string bucketName)
        {
            Console.WriteLine($"contents of {bucketName}");
            var response = await _client.ListObjectsAsync(bucketName);
            foreach (var entry in response.S3Objects)
            {
                Console.WriteLine($"{entry.Key}\t{entry.LastModified}");
            }
        }

        private static async Task CreateFileInBucket(string bucketName, string key, string contents)
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

        private static async Task RenameFileInBucket(string bucketName, string sourceKey, string destinationKey)
        {
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                SourceKey = sourceKey,
                DestinationBucket = bucketName,
                DestinationKey = destinationKey
            };

            var copyResponse = await _client.CopyObjectAsync(copyRequest);

            var deleteResponse = await _client.DeleteObjectAsync(bucketName, sourceKey);

            Console.WriteLine($"copy = {copyResponse.HttpStatusCode}, delete = {deleteResponse.HttpStatusCode}");
        }

        private static async Task ReadFileInBucket(string bucketName, string key)
        {
            var file = await _client.GetObjectAsync(bucketName, key);
            var streamReader = new StreamReader(file.ResponseStream);

            Console.WriteLine($"name: {file.Key}, length: {file.ContentLength}");
            Console.WriteLine(streamReader.ReadToEnd());
        }
    }
}