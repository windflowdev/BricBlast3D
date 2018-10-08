//
//  TWTRSession+Additions.h
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 11/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <TwitterKit/TwitterKit.h>

@interface TWTRSession (Additions)

// Related to conversion
- (id)toJsonObject;
- (const char *)toCString;

@end
