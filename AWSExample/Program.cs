﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;

namespace AWSExample
{
    public static class Program
    {
        private static IAmazonS3 _client;
        private static IConfiguration _config;

        public static void Main(string[] args)
        {
            // get the config file and parse it
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json");

            _config = configBuilder.Build();

            _client = _config.GetAWSOptions().CreateServiceClient<IAmazonS3>();
            
            // since the aws api is all async we need to have the main method in a async and run it syncronously 
            Run(args).GetAwaiter().GetResult();
        }

        private static async Task Run(string[] args)
        {
            // get the first argument or an empty string
            var command = args.Length != 0 ? args[0] : "";

            switch (command)
            {
                case "list-owned-buckets":
                    await ListOwnedBuckets();
                    break;
                case "create":
                    await CreateFileInBucket(bucketName: args[1], key: args[2], contents: args[3]);
                    break;
                case "list-bucket":
                    await GetBucketContents(bucketName: args[1]);
                    break;
                case "rename":
                    await RenameFileInBucket(bucketName: args[1], sourceKey: args[2], destinationKey: args[3]);
                    break;
                default:
                    Console.WriteLine($"ERROR: invalid command {command}, please choose one of the following");
                    Console.WriteLine("usage:");
                    Console.WriteLine("  create\tcreates a file `create <bucket> <key> <contents>`");
                    Console.WriteLine("  rename\trenames a file `rename <bucket> <source> <destination>`");
                    Console.WriteLine("  list-bucket\tlists the contents of a bucket `list-bucket <bucket>`");
                    Console.WriteLine("  list-owned-buckets\tlists the buckets that you own `list-buckets`");
                    break;
            }
        }

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
    }
}