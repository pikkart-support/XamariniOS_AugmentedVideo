using System;
using Foundation;
using UIKit;
using CoreGraphics;
using OpenTK.Graphics.ES20;
using Pikkart.ArSdk.Recognition;


using PikkartVideoPlayerBindingLibrary;


namespace XamariniOS_AugmentedVideo
{
	public class VideoMesh : NSObject
	{
		UIViewController _parentCtrl;
		float[] mVertices= {};
    	float[] mTexCoords={};
        float[] mNormals={};
    	ushort[] mIndex={};
    	int mIndices_Number = 0;
    	int mVertices_Number = 0;
    	int mKeyframeTexture_GL_ID = 0;
    	int mIconBusyTexture_GL_ID = 0;
    	int mIconPlayTexture_GL_ID = 0;
    	int mIconErrorTexture_GL_ID = 0;
    	int mVideoTexture_GL_ID = 0;
    
        int mVideo_Program_GL_ID= 0;
    	int mKeyframe_Program_GL_ID = 0;
    	PikkartVideoPlayer mPikkartVideoPlayer;
        string mMovieUrl = "";
    	int mSeekPosition = 0;
    	bool mAutostart = false;
    
    	float keyframeAspectRatio = 1f;
    	float videoAspectRatio = 1f;
    
    	float[] mTexCoordTransformationMatrix= {};
    	float[] videoTextureCoords = {0f,1f,1f, 1f,1f, 0f,0f, 0f};
    	float[] videoTextureCoordsTransformed = {0f, 0f, 1f, 0f, 1f, 1f, 0f, 1f};
    	float[] mVideoTexCoords  = {0f,1f,1f, 1f,1f, 0f,0f, 0f};
    	
    	public PikkartVideoPlayer videoPlayer {
        	get {
            	return mPikkartVideoPlayer;
        	}
        	set {
            	mPikkartVideoPlayer = value;
        	}
    	}
    
    const string VERTEX_SHADER ="attribute vec4 vertexPosition;attribute vec2 vertexTexCoord;varying vec2 texCoord;uniform mat4 modelViewProjectionMatrix;void main() {   gl_Position = modelViewProjectionMatrix * vertexPosition;   texCoord = vertexTexCoord;}";
    
    const string KEYFRAME_FRAGMENT_SHADER ="precision mediump float;varying vec2 texCoord;uniform sampler2D texSampler2D;void main(){   gl_FragColor = texture2D(texSampler2D, texCoord);}";
    
    const string VIDEO_FRAGMENT_SHADER ="precision mediump float; varying vec2 texCoord; uniform sampler2D texSamplerOES; void main() {    gl_FragColor = texture2D(texSamplerOES, texCoord); } ";
    
    	bool GenerateMesh()  {
        	mVertices = new float[]{	0f, 0f, 0f,
                     		1f, 0f, 0f,
                     		1f, 1f, 0f,
                     		0f, 1f, 0f 
						};
        	mVertices_Number = 4;
        	mTexCoords = new float[]{0f, 1f, 1f, 1f, 1f, 0f, 0f, 0f};
        	mNormals = new float[]{ 0f, 0f, 1f,
            	         0f, 0f, 1f,
                	     0f, 0f, 1f,
                    	 0f, 0f, 1f};
        
        	mIndex = new ushort[]{0, 1, 2, 2, 3, 0};
        	mIndices_Number = 6;
        
        	return true;
    	}
    
		public VideoMesh (UIViewController parentCtrl)
		{
			_parentCtrl = parentCtrl;
		}
		
		public bool InitMesh(string movieUrl, string keyFrameUrl, int seekPosition, 
							 bool autoStart, PikkartVideoPlayer videoPlayerParam) {
			GenerateMesh();
        
        	if (videoPlayer == null) {
            	videoPlayer = new PikkartVideoPlayer();
        	} else {
            	videoPlayer = videoPlayerParam;
        	}
        	CGSize dims = CGSize.Empty;
            mMovieUrl = movieUrl;
        	mKeyframeTexture_GL_ID = RenderUtils.LoadTextureFromFileName(keyFrameUrl, out dims);
           	keyframeAspectRatio = (float)(dims.Height/dims.Width);
            mSeekPosition = seekPosition;
        	mAutostart = autoStart;
        
        	string mediaBundlePath=NSBundle.MainBundle.BundlePath+"/media.bundle";
        	NSBundle mediaBundle=new NSBundle(mediaBundlePath);
            mIconBusyTexture_GL_ID = RenderUtils.LoadTextureFromFileName(mediaBundle.PathForResource("busy", "png"));
        	mIconPlayTexture_GL_ID = RenderUtils.LoadTextureFromFileName(mediaBundle.PathForResource("play", "png"));
        	mIconErrorTexture_GL_ID = RenderUtils.LoadTextureFromFileName(mediaBundle.PathForResource("error", "png"));
        
        	mKeyframe_Program_GL_ID = RenderUtils.CreateProgram(VERTEX_SHADER, KEYFRAME_FRAGMENT_SHADER);
        	mVideo_Program_GL_ID = RenderUtils.CreateProgram(VERTEX_SHADER, VIDEO_FRAGMENT_SHADER);
        	mVideoTexture_GL_ID = RenderUtils.CreateVideoTexture();
        
        if (mVideoTexture_GL_ID != 0) {
            videoPlayer.TextureHandle = mVideoTexture_GL_ID;
            videoPlayer.Load(movieUrl, mAutostart, seekPosition);
        }
        
        return true;
		}
		
		public void reloadOnAppear() {
	        if (mPikkartVideoPlayer != null) {
	            if (mVideoTexture_GL_ID != 0) {
	                videoPlayer.TextureHandle = mVideoTexture_GL_ID;
	                videoPlayer.Load(mMovieUrl, mAutostart  , mSeekPosition);
	            }
	        }
	    }
    
	    public void pauseVideo() {
	        if (mPikkartVideoPlayer != null) {
	            PKTVIDEO_STATE status = mPikkartVideoPlayer.Status;
	            if (status == PKTVIDEO_STATE.PLAYING) {
	                mPikkartVideoPlayer.Pause();
	            }
	        }
	    }
    
	    public void playOrPauseVideo() {
	        if (mPikkartVideoPlayer != null) {
	            PKTVIDEO_STATE status = mPikkartVideoPlayer.Status;
	            if (status == PKTVIDEO_STATE.PLAYING) {
	                mPikkartVideoPlayer.Pause();
	            } else if (status == PKTVIDEO_STATE.REACHED_END ||
	                       status == PKTVIDEO_STATE.PAUSED ||
	                       status == PKTVIDEO_STATE.READY ||
	                       status == PKTVIDEO_STATE.STOPPED) {
	                mPikkartVideoPlayer.Play(mSeekPosition);
	            }
	        }
	    }
    
	    void setVideoDimensions(float videoWidth, float videoHeight , float[] textureCoordMatrix) {
	    
	        videoAspectRatio = (float)(videoHeight / videoWidth);
	
	        float[] mtx = textureCoordMatrix;
	        float[] tempUVMultRes;
	
	        uvMultMat4f(videoTextureCoordsTransformed[0], videoTextureCoordsTransformed[1],
	                                    videoTextureCoords[0], videoTextureCoords[1], mtx, out tempUVMultRes);
	        videoTextureCoordsTransformed[0] = tempUVMultRes[0];
	        videoTextureCoordsTransformed[1] = tempUVMultRes[1];
	
	        uvMultMat4f(videoTextureCoordsTransformed[2],  videoTextureCoordsTransformed[3],
	                                     videoTextureCoords[2],  videoTextureCoords[3],  mtx, out tempUVMultRes);
	        videoTextureCoordsTransformed[2] = tempUVMultRes[0];
	        videoTextureCoordsTransformed[3] = tempUVMultRes[1];
	
	        uvMultMat4f(videoTextureCoordsTransformed[4],  videoTextureCoordsTransformed[5],
	                                    videoTextureCoords[4], videoTextureCoords[5], mtx, out tempUVMultRes);
	        videoTextureCoordsTransformed[4] = tempUVMultRes[0];
	        videoTextureCoordsTransformed[5] = tempUVMultRes[1];
	
	        uvMultMat4f(videoTextureCoordsTransformed[6],  videoTextureCoordsTransformed[7],
	                                    videoTextureCoords[6], videoTextureCoords[7], mtx, out tempUVMultRes);
	        videoTextureCoordsTransformed[6] = tempUVMultRes[0];
	        videoTextureCoordsTransformed[7] = tempUVMultRes[1];
	    }
    
	    void uvMultMat4f(float transformedU, float transformedV, float u, float v, float[] pMat, out float[] result) {
	        var x = pMat[0] * u + pMat[4] * v + pMat[12] * 1f;
	        var y = pMat[1] * u + pMat[5] * v + pMat[13] * 1f;
	        result = new float[]{};
	        result[0] = x;
	        result[1] = y;
	    }
    
    	void DrawKeyFrame(ref float[] mvpMatrix) {
    
	        GL.Enable(EnableCap.Blend);
	        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
	
	        GL.UseProgram(mKeyframe_Program_GL_ID);
	
	        RenderUtils.CheckGLError();
	
	        var vertexHandle = GL.GetAttribLocation(mKeyframe_Program_GL_ID, "vertexPosition");
	        var textureCoordHandle = GL.GetAttribLocation(mKeyframe_Program_GL_ID, "vertexTexCoord");
	        var mvpMatrixHandle = GL.GetUniformLocation(mKeyframe_Program_GL_ID, "modelViewProjectionMatrix");
	        var texSampler2DHandle = GL.GetUniformLocation(mKeyframe_Program_GL_ID, "texSampler2D");
	
	        RenderUtils.CheckGLError();
	
	        GL.VertexAttribPointer(vertexHandle, 3, VertexAttribPointerType.Float, false, 0, mVertices);
	        GL.VertexAttribPointer(textureCoordHandle, 2,VertexAttribPointerType.Float, false, 0, mTexCoords);
	
	
	        GL.EnableVertexAttribArray(vertexHandle);
	        GL.EnableVertexAttribArray(textureCoordHandle);
	
	        RenderUtils.CheckGLError();
	
	        GL.ActiveTexture(TextureUnit.Texture0);
	        GL.BindTexture(TextureTarget.Texture2D, mKeyframeTexture_GL_ID);
	        GL.Uniform1(texSampler2DHandle, 0);
	
	        RenderUtils.CheckGLError();
	
	        GL.Ext.PushGroupMarker(0, "Draw Pikkart KeyFrame");
	
	        GL.UniformMatrix4(mvpMatrixHandle, 1, false, mvpMatrix);
	
	        GL.DrawElements(BeginMode.Triangles, mIndices_Number, DrawElementsType.UnsignedShort, mIndex);
	
	        GL.Ext.PopGroupMarker();
	
	        RenderUtils.CheckGLError();
	
	        GL.DisableVertexAttribArray(vertexHandle);
	        GL.DisableVertexAttribArray(textureCoordHandle);
	        
	        RenderUtils.CheckGLError();
	
	        GL.UseProgram(0);
	        GL.Disable(EnableCap.Blend);
	
	        RenderUtils.CheckGLError();
	
	    }
    
    	void DrawVideo(ref float[] mvpMatrix) {
        
	        GL.UseProgram(mVideo_Program_GL_ID);
	
	        var vertexHandle = GL.GetAttribLocation(mVideo_Program_GL_ID, "vertexPosition");
	        var textureCoordHandle = GL.GetAttribLocation(mVideo_Program_GL_ID, "vertexTexCoord");
	        var mvpMatrixHandle = GL.GetUniformLocation(mVideo_Program_GL_ID, "modelViewProjectionMatrix");
	        var texSampler2DHandle = GL.GetUniformLocation(mVideo_Program_GL_ID, "texSamplerOES");
	
	        RenderUtils.CheckGLError();
	
	        GL.VertexAttribPointer(vertexHandle, 3, VertexAttribPointerType.Float, false, 0, mVertices);
	        GL.VertexAttribPointer(textureCoordHandle, 2, VertexAttribPointerType.Float, false, 0, videoTextureCoordsTransformed);
	
	        GL.EnableVertexAttribArray(vertexHandle);
	        GL.EnableVertexAttribArray(textureCoordHandle);
	
	        RenderUtils.CheckGLError();
	
	        GL.ActiveTexture(TextureUnit.Texture0);
	        GL.BindTexture(TextureTarget.Texture2D, mVideoTexture_GL_ID);
	        GL.Uniform1(texSampler2DHandle, 0);
	
	        RenderUtils.CheckGLError();
	
	        GL.UniformMatrix4(mvpMatrixHandle, 1, false, mvpMatrix);
	
	        // Render
	        GL.DrawElements(BeginMode.Triangles , mIndices_Number, DrawElementsType.UnsignedShort, mIndex);
	
	        RenderUtils.CheckGLError();
	
	        GL.UseProgram(0);
	        
	        GL.DisableVertexAttribArray(vertexHandle);
	        GL.DisableVertexAttribArray(textureCoordHandle);
	        
	        RenderUtils.CheckGLError();
	
	    }

    	void DrawIcon(ref float[] mvpMatrix, PikkartVideoPlayerBindingLibrary.PKTVIDEO_STATE status) {
        
	        GL.Enable(EnableCap.Blend);
	        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		
		    GL.UseProgram(mKeyframe_Program_GL_ID);
	
	        
	        RenderUtils.CheckGLError();
	
	        var vertexHandle = GL.GetAttribLocation(mKeyframe_Program_GL_ID, "vertexPosition");
	        var textureCoordHandle = GL.GetAttribLocation(mKeyframe_Program_GL_ID, "vertexTexCoord");
	        var mvpMatrixHandle = GL.GetUniformLocation(mKeyframe_Program_GL_ID, "modelViewProjectionMatrix");
	        var texSampler2DHandle = GL.GetUniformLocation(mKeyframe_Program_GL_ID, "texSampler2D");
	
	        GL.VertexAttribPointer(vertexHandle, 3, VertexAttribPointerType.Float, false, 0, mVertices);
	        GL.VertexAttribPointer(textureCoordHandle, 2, VertexAttribPointerType.Float, false, 0, mTexCoords);
	
	        RenderUtils.CheckGLError();
	
	        GL.EnableVertexAttribArray(vertexHandle);
	        GL.EnableVertexAttribArray(textureCoordHandle);
	
	        RenderUtils.CheckGLError();
	
		    GL.ActiveTexture(TextureUnit.Texture0);
	        
	        switch ((long)status) {
	            case 0://end
	                GL.BindTexture(TextureTarget.Texture2D, mIconPlayTexture_GL_ID);
	            break;
	            case 1://pasued
	                GL.BindTexture(TextureTarget.Texture2D, mIconPlayTexture_GL_ID);
	                        break;
	
	            case 2://stopped
	                GL.BindTexture(TextureTarget.Texture2D, mIconPlayTexture_GL_ID);
	                        break;
	
	            case 3://playing
	                GL.BindTexture(TextureTarget.Texture2D, mIconPlayTexture_GL_ID);
	                        break;
	
	            case 4://ready
	                GL.BindTexture(TextureTarget.Texture2D, mIconPlayTexture_GL_ID);
	                        break;
	
	            case 5://not ready
	                GL.BindTexture(TextureTarget.Texture2D, mIconBusyTexture_GL_ID);
	                        break;
	
	            case 6://buffering
	                GL.BindTexture(TextureTarget.Texture2D, mIconBusyTexture_GL_ID);
	                        break;
	
	            case 7://error
	                GL.BindTexture(TextureTarget.Texture2D, mIconErrorTexture_GL_ID);
	                        break;
	
	            default:
	                GL.BindTexture(TextureTarget.Texture2D, mIconBusyTexture_GL_ID);
	                break;
	            
	        }
	        RenderUtils.CheckGLError();
	
	        GL.Uniform1(texSampler2DHandle, 0);
	
	        GL.UniformMatrix4(mvpMatrixHandle, 1, false, mvpMatrix);
	
	        RenderUtils.CheckGLError();
	
	        GL.DrawElements(BeginMode.Triangles, mIndices_Number, DrawElementsType.UnsignedShort, mIndex);
	
	        
	        RenderUtils.CheckGLError();
	        GL.UseProgram(0);
	        
	        GL.Disable(EnableCap.Blend);
	        GL.DisableVertexAttribArray(vertexHandle);
	        GL.DisableVertexAttribArray(textureCoordHandle);
	        
	        RenderUtils.CheckGLError();
	
	    }
    
    	public void DrawMesh(ref float [] modelView, ref float[] projection)
    	{
	        var viewCtrl = _parentCtrl as RecognitionViewController;
	        
	        if (viewCtrl != null) {
	            var currentStatus = PKTVIDEO_STATE.NOT_READY;
	            if(mPikkartVideoPlayer != null) {
	                currentStatus = mPikkartVideoPlayer.Status;
	                if (currentStatus == PKTVIDEO_STATE.PLAYING) {
	                    mPikkartVideoPlayer.UpdateVideoData();
	                    videoAspectRatio = (float)(mPikkartVideoPlayer.Size.Height/mPikkartVideoPlayer.Size.Width);
	                }
	            }
	            
	            PKTMarker currentMarker = viewCtrl.CurrentMarker;
	            if (currentMarker != null) {
	                var markerSize = new CGSize(currentMarker.Width,currentMarker.Height);
	                GL.Enable(EnableCap.DepthTest);
	                GL.Disable(EnableCap.CullFace);
	                
	                if ((currentStatus == PKTVIDEO_STATE.READY)
	                    || (currentStatus == PKTVIDEO_STATE.REACHED_END)
	                    || (currentStatus == PKTVIDEO_STATE.NOT_READY)
	                    || (currentStatus == PKTVIDEO_STATE.ERROR)) {
	                    
	                    float[] scaleMatrix = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    RenderUtils.MtxLoadIdentity(ref scaleMatrix);
	                    scaleMatrix[0]=(float)markerSize.Width;
	                    scaleMatrix[5]=(float)(markerSize.Width * keyframeAspectRatio);
	                    scaleMatrix[10]=(float)markerSize.Width;
	                    
	                    float[] temp_mv = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    RenderUtils.MatrixMultiply(4,  4, ref modelView,  4,  4, ref scaleMatrix, ref temp_mv);
	                    
	                    float[] temp_mvp = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    RenderUtils.MatrixMultiply(4,  4, ref projection,  4,  4, ref temp_mv, ref temp_mvp);
	
	                    float[] mvpMatrix = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    
	                    RenderUtils.MtxTranspose(ref temp_mvp,
	                                              ref mvpMatrix);
	
	                    DrawKeyFrame(ref mvpMatrix);
	                } else
	                {
	                    
	                    float[] scaleMatrix = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    RenderUtils.MtxLoadIdentity(ref scaleMatrix);
	                    scaleMatrix[0]=(float)(markerSize.Width);
	                    scaleMatrix[5]=(float)(markerSize.Width * videoAspectRatio);
	                    scaleMatrix[10]=(float)(markerSize.Width);
	                    
	                    
	                    float[] temp_mv = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	
	                    RenderUtils.MatrixMultiply(4,  4, ref modelView,  4,  4, ref scaleMatrix, ref temp_mv);
	                    
	                    
	                    float[] temp_mvp = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    RenderUtils.MatrixMultiply(4,  4, ref projection,  4,  4, ref temp_mv, ref temp_mvp);
	                    
	                    float[] mvpMatrix = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	                    
	                    RenderUtils.MtxTranspose(ref temp_mvp,
	                                              ref mvpMatrix);
	                    
	                    DrawVideo(ref mvpMatrix);
	                }
	                
	                if ((currentStatus == PKTVIDEO_STATE.READY)
	                    || (currentStatus == PKTVIDEO_STATE.REACHED_END)
	                    || (currentStatus == PKTVIDEO_STATE.PAUSED)
	                    || (currentStatus == PKTVIDEO_STATE.NOT_READY)
	                    || (currentStatus == PKTVIDEO_STATE.ERROR)) {
	                    
	                    float[] translateMatrix = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	
	                    RenderUtils.MtxLoadIdentity(ref translateMatrix);
	                    //scale a bit
	                    translateMatrix[0] = 0.4f;
	                    translateMatrix[5] = 0.4f;
	                    translateMatrix[10] = 0.4f;
	                    //translate a bit
	                    translateMatrix[3] = 0;
	                    translateMatrix[7] = 0.45f;
	                    translateMatrix[11] = -0.05f;
	                    
	                    float[]temp_mv = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	
	                    RenderUtils.MatrixMultiply(4,  4, ref modelView,  4,  4, ref translateMatrix, ref temp_mv);
	                    
	                    float[] temp_mvp = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	
	                    RenderUtils.MatrixMultiply(4,  4, ref projection, 4, 4, ref temp_mv,ref temp_mvp);
	                    
	                    float[] mvpMatrix = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	
	                    RenderUtils.MtxTranspose(ref temp_mvp,
	                                              ref mvpMatrix);
	                    
	                    DrawIcon(ref mvpMatrix, currentStatus);
	                }
	                RenderUtils.CheckGLError();
	            }
	
	        }
	    }
    
    	public bool isPlaying()  {
        
	        if (mPikkartVideoPlayer != null) {
	            if(mPikkartVideoPlayer.Status == PKTVIDEO_STATE.PLAYING ||
	                mPikkartVideoPlayer.Status == PKTVIDEO_STATE.PAUSED){
	                return true;
	            }
	        }
	        return false;
	    }

	}
}
