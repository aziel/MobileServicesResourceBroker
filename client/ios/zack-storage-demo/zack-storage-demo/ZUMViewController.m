//
//  ZUMViewController.m
//  zack-storage-demo
//
//  Created by David on 7/18/14.
//  Copyright (c) 2014 ___FULLUSERNAME___. All rights reserved.
//

#import "ZUMViewController.h"
#import "QSTodoService.h"
#import <WindowsAzureMobileServices/WindowsAzureMobileServices.h>

@interface ZUMViewController () <UIImagePickerControllerDelegate,UINavigationControllerDelegate>

- (IBAction)buttonClicked:(id)sender;

@property (weak, nonatomic) IBOutlet UIImageView *imageView;
@property (strong, nonatomic) QSTodoService *todoService;
@property (strong, nonatomic) UIImagePickerController *imagePickerController;

@end

@implementation ZUMViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
    
    self.imagePickerController = [[UIImagePickerController alloc] init];
    self.imagePickerController.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    self.imagePickerController.delegate = self;
    
    self.todoService = [QSTodoService defaultService];
}

-(void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    UIImage *image = [info valueForKey:UIImagePickerControllerOriginalImage];
    self.imageView.image = image;
    self.imageView.alpha = 0;
    [UIView animateWithDuration:2.0 animations:^{
        self.imageView.alpha = 1;
    }];
    
    NSData* imageData = UIImagePNGRepresentation(image);
    
    MSClient *client = self.todoService.client;
    MSStorage *storage = [[MSStorage alloc] initWithClient:client];
    NSLog(@"Will upload image!");
    
    [storage uploadBlobWithName:@"DavidNI_blob"
                      container:@"testcontainer"
                       contents:imageData
                        apiName:@"resources"
                     completion:^(NSString *blobUri, NSError *error) {
                         if (error != nil)
                         {
                             NSLog(@"Error during upload: %@", error);
                             return;
                         }
                         NSLog(@"Upload successful: %@",blobUri);
                     }];

    
    [picker dismissViewControllerAnimated:YES completion:^{
    }];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (IBAction)buttonClicked:(id)sender
{
    NSLog(@"Clicked");
    self.imageView.image = nil;
    [self presentViewController:self.imagePickerController animated:YES completion:nil];

    /*
    // Create the todoService - this creates the Mobile Service client inside the wrapped service
    self.todoService = [QSTodoService defaultService];
    
    MSClient *client = self.todoService.client;
    MSStorage *storage = [[MSStorage alloc] initWithClient:client];

    [storage uploadBlobWithName:@"DavidNI_blob"
                      container:@"testcontainer"
                       contents:[@"Hello world!" dataUsingEncoding:NSUTF8StringEncoding]
                        apiName:@"resources"
                     completion:^(NSString *blobUri, NSError *error) {
                         if (error != nil)
                         {
                             NSLog(@"Error during upload: %@", error);
                             return;
                         }
                         NSLog(@"Upload successful: %@",blobUri);
                     }];
    
    [storage downloadBlobWithName:@"DavidNI_blob"
                        container:@"testcontainer"
                          apiName:@"resources"
                       completion:^(NSData *contents, NSError *error) {
                           if (error != nil)
                           {
                               NSLog(@"Error during download: %@", error);
                               return;
                           }
                           NSLog(@"Download successful: %@",contents);
                       }];
    */
    
    
    /*
    NSDictionary *item = @{ @"text": @"Hello", @"complete":@NO };
    [self.todoService addItem:item completion:^(NSUInteger index) {
        NSLog(@"Inserted: %u", index);
    }];
    */
}

@end
