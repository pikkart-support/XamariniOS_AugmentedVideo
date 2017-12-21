using System;

namespace PikkartVideoPlayerBindingLibrary
{
	public enum PKTVIDEO_STATE:int {
        REACHED_END = 0,
        PAUSED,
        STOPPED,
        PLAYING,
        READY,
        NOT_READY,
        ERROR,
    }
}