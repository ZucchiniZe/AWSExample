# AWS Example

This project is meant to be a way to document how to interface with the AWS C# SDK.

## Authentication

This is arguably the trickest part of integrating with the API.

Here are the steps that I went through to get my project working.

1. Install package(s) through nuget
    * install AWSSDK.Core first
    * search through nuget for the sub packages
    * in this example I installed AWSSDK.S3 alongside core to allow me to interface with S3
2. Create a IAM user through the aws console.
    * go to https://console.aws.amazon.com/iam/home#/users
    * click on `add user`
    * fill in username and select programmatic access
    * click next
    * create group
    * fill in group name
    * search for "S3" in the permissions
    * select either the `AmazonS3FullAccess` or `AmazonS3ReadOnlyAccess` depending on what you are doing
    * click "create group"
    * click next
    * click "create user"
    * copy down the credentials that are created to a safe place
3. Add a AWS profile (this was the confusing part)
    * go to `C:\Users\{CURRENT_USER}\.aws` on windows and `~/.aws/credentials` on Linux/macOS
    * create a text file named `credentials`
    * fill it out like so
    ```ini
    [default]
    aws_access_key_id = <access key>
    aws_secret_access_key = <secret key>
    ```
4. Make sure your settings are correct in the config file
    * create a `settings.json` and fill it out like so
    ```json
    {
      "AWS": {
        "Profile": "default",
        "Region": "us-west-2"
      }
    }
    ```
    * change the region to the region you want to operate in
5. Start implementing code
    * generate the configuration with the ASP.NET configurationbuilder
    ```c#
    var configBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("settings.json");

    _config = configBuilder.Build();
    ```
    * create the S3 client from said config (replacing `IAmazonS3` with the interface of the service you want to use)
    ```c#
    _client = _config.GetAWSOptions().CreateServiceClient<IAmazonS3>();
    ```

6. Profit, you now have everything you need to interface with the Amazon SDK for .NET!
    
I have tried to make my code as readable as possible to allow for copying and learning.

[Program.cs](Program.cs) handles all the client initializations and connecting to the service plus the cli
[Operations.cs](Operations.cs) handles all of the actual interfacing with the S3 API.
