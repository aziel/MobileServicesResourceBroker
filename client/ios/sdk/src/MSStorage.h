// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

#import <Foundation/Foundation.h>
#import "MSClient.h"

typedef void (^MSUploadBlobBlock)(NSString *blobUri, NSError *error);
typedef void (^MSDownloadBlobBlock)(NSData *contents, NSError *error);

@interface MSStorage : NSObject

/// The client associated with this table.
@property (nonatomic, strong, readonly) MSClient *client;

-(id)initWithClient:(MSClient*)client;


-(void)uploadBlobWithName:(NSString*)blobName
                container:(NSString*)containerName
                 contents:(NSData*)contents
                  apiName:(NSString*)apiName
               completion:(MSUploadBlobBlock)complete;

-(void)downloadBlobWithName:(NSString*)blobName
                  container:(NSString*)containerName
                    apiName:(NSString*)apiName
                 completion:(MSDownloadBlobBlock)complete;


@end
