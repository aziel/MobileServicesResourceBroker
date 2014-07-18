// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

#import "MSStorage.h"


typedef void (^MSCompleteWithStringBlock)(NSString *result, NSError *error);

@interface MSStorage ()

@property (nonatomic, strong) MSClient *client;

@end


@implementation MSStorage

-(id)initWithClient:(MSClient*)client
{
    if (self = [super init])
    {
        self.client = client;
    }
    return self;
}

-(void)uploadBlobWithName:(NSString*)blobName
                container:(NSString*)containerName
                 contents:(NSData*)contents
                  apiName:(NSString*)apiName
               completion:(MSUploadBlobBlock)complete
{
    [self getSasTokenForBlob:blobName
                   container:containerName
                 permissions:@"rw"
                     apiName:apiName
                  completion:^(NSString *result, NSError *error) {
                      if (error != nil)
                      {
                          NSLog(@"upload sas Failed with error: %@", error);
                          complete(nil,error);
                          return;
                      }
                      NSLog(@"Received SAS for upload: %@",result);
                      // TODO: Upload data
                      if (![self postData:contents toUri:[NSURL URLWithString:result]])
                      {
                          complete(nil, [[NSError alloc] init]);
                          return;
                      }
                      complete(result,nil);
                  }];
}

-(void)downloadBlobWithName:(NSString*)blobName
                  container:(NSString*)containerName
                    apiName:(NSString*)apiName
                 completion:(MSDownloadBlobBlock)complete
{
    [self getSasTokenForBlob:blobName
                   container:containerName
                 permissions:@"rw"
                     apiName:apiName
                  completion:^(NSString *result, NSError *error) {
                      if (error != nil)
                      {
                          NSLog(@"download sas Failed with error: %@", error);
                          complete(nil,error);
                          return;
                      }
                      NSLog(@"Received SAS for download: %@", result);
                      NSData *data = [NSData dataWithContentsOfURL: [NSURL URLWithString:result]];
                      complete(data, nil);
                  }];
}

/// Invokes a user-defined API of the Mobile Service to get
/// a SAS token for Azure Storage access
-(void)getSasTokenForBlob:(NSString*)blobName
                container:(NSString*)containerName
              permissions:(NSString*)permissions
                  apiName:(NSString *)apiName
               completion:(MSCompleteWithStringBlock)complete
{
    NSDictionary *dict = @{ @"name":       blobName,
                            @"permissions": permissions,
                            @"expiry":     @"2015-04-23 12:43:00Z",
                            @"container":  containerName};
    
    [self.client invokeAPI:apiName
                      body:dict
                HTTPMethod:@"POST"
                parameters:@{ @"type":@"blob" }
                   headers:nil
                completion:^(id result, NSHTTPURLResponse *response, NSError *error) {
                    if (error != nil)
                    {
                        complete(nil, error);
                        return;
                    }
                    NSDictionary *res = result;
                    NSString* uriString = [res valueForKey:@"uri"];
                    complete(uriString, nil);
                }];
}

- (BOOL)postData:(NSData*)data toUri:(NSURL*)uri
{
    NSString *postLength = [NSString stringWithFormat:@"%d", [data length]];
    
    // Init and set fields of the URLRequest
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] init];
    [request setHTTPMethod:@"POST"];
    [request setURL:uri];
    [request setValue:@"image/png" forHTTPHeaderField:@"Content-Type"];
    [request setValue:@"BlockBlob" forHTTPHeaderField:@"x-ms-blob-type"];
    [request setValue:postLength forHTTPHeaderField:@"Content-Length"];
    [request setHTTPBody:data];
    
    NSURLConnection *connection = [[NSURLConnection alloc] initWithRequest:request delegate:self];
    if (!connection)
    {
        return false;
    }
    
    return true;
}

@end
