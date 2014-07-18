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

@interface ZUMViewController ()

- (IBAction)buttonClicked:(id)sender;

@property (strong, nonatomic) QSTodoService *todoService;

@end

@implementation ZUMViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (IBAction)buttonClicked:(id)sender
{
    NSLog(@"Clicked");
    
    // Create the todoService - this creates the Mobile Service client inside the wrapped service
    self.todoService = [QSTodoService defaultService];
    
    NSDictionary *item = @{ @"text": @"Hello", @"complete":@NO };
    [self.todoService addItem:item completion:^(NSUInteger index) {
        NSLog(@"Inserted: %u", index);
    }];
}

@end
