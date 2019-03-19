//
//  PikkartVideoPlayer.h
//  PikkartVideoPlayer
//
//  Created by Roberto Avanzi on 14/03/2019.
//  Copyright © 2019 Roberto Avanzi. All rights reserved.
//
#import <UIKit/UIKit.h>

typedef enum : NSUInteger {
    REACHED_END = 0,
    PAUSED,
    STOPPED,
    PLAYING,
    READY,
    NOT_READY,
    ERROR
} PKTVIDEO_STATE;

@interface PikkartVideoPlayer : NSObject

-(bool)loadWithFilename:(NSString *)filename
        playImmediately:(bool)playImmediately
           seekPosition:(float)seek;
-(bool)playWithSeekposition:(float)seekPosition;
-(bool)pause;
-(bool)stop;
-(bool)seekToPosition:(float)position;
-(void)updateVideoData;
-(int)videoTextureHandle;
-(void)setVideoTextureHandle:(int)textureId;
-(int)videoStatus;
-(CGSize)videoSize;
@end
