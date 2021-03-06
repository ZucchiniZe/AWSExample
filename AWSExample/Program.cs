﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.S3;

namespace AWSExample
{
    public static partial class Program
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
                case "read":
                    await ReadFileInBucket(bucketName: args[1], key: args[2]);
                    break;
                case "search":
                    await SearchBucketContents(bucketName: args[1], prefix: args[2]);
                    break;
                case "md5":
                    await DownloadAllAndMd5(bucketName: args[1], prefix: args[2]);
                    break;
                default:
                    Console.WriteLine("usage:");
                    Console.WriteLine("  md5\t\t\tdownloads a bucket to temp dir and computes md5 for every file `md5 <bucke> <prefix>`");
                    Console.WriteLine("  read\t\t\treads a file `read <bucket> <key>`");
                    Console.WriteLine("  create\t\tcreates a file `create <bucket> <key> <contents>`");
                    Console.WriteLine("  search\t\tsearches through a bucket with a specified prefix `search <bucket> <prefix>`");
                    Console.WriteLine("  rename\t\trenames a file `rename <bucket> <source> <destination>`");
                    Console.WriteLine("  list-bucket\t\tlists the contents of a bucket `list-bucket <bucket>`");
                    Console.WriteLine("  list-owned-buckets\tlists the buckets that you own `list-buckets`");
                    break;
            }
        }
    }
}