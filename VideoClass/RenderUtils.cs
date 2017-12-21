using System;
using System.Runtime.InteropServices;
using System.Text;
using UIKit;
using Foundation;
using OpenTK.Graphics.ES20;
using CoreGraphics;

namespace XamariniOS_AugmentedVideo
{
	public class RenderUtils : NSObject
	{
		private RenderUtils ()
		{
		}
		
		static string GetGLErrorString(ErrorCode error)
		{
			string str;
			switch( error )
			{
				case ErrorCode.NoError:
					str = "GL_NO_ERROR";
				break;
				case ErrorCode.InvalidEnum:
					str = "GL_INVALID_ENUM";
				break;
				case ErrorCode.InvalidValue:
					str = "GL_INVALID_VALUE";
				break;
				case ErrorCode.InvalidOperation:
					str = "GL_INVALID_OPERATION";
				break;
				default:
					str = "(ERROR: Unknown Error Enum)";
				break;
			}
			return str;
		}

		public static void CheckGLError()									
		{														
    		ErrorCode err = GL.GetErrorCode();					
    		while (err != ErrorCode.NoError) {					
        		Console.WriteLine("GLError {0}\n", GetGLErrorString(err));
        		err = GL.GetErrorCode();
    		}													
		}
		
		public class demoImage : NSObject
		{	
			public byte[] data;
			public int size;
			public nfloat width;
			public nfloat height;
			public PixelFormat format;
			public PixelType type;
			public int rowByteSize;
		}

        public static void ImgLoadImage(string filepathname, int flipVertical, out demoImage image) {
        
        	UIImage imageClass = new UIImage(filepathname);
			CGImage cgImage = imageClass.CGImage;
			if (cgImage == null)
			{
				image=null;
				return;
			}
	
			image = new demoImage();
			image.width = imageClass.Size.Width;
			image.height = imageClass.Size.Height;
			image.rowByteSize = (int)image.width * 4;
			image.data = new byte[(int)(image.height * image.rowByteSize)];
			image.format = PixelFormat.Rgba;
			image.type = PixelType.UnsignedByte;
			CGContext context = new CGBitmapContext(image.data, (nint)image.width, (nint)image.height, 8, 
													image.rowByteSize, cgImage.ColorSpace, (CGImageAlphaInfo)(CGBitmapFlags.AlphaInfoMask & 
																						   					  CGBitmapFlags.NoneSkipLast));
			context.SetBlendMode(CGBlendMode.Copy);
			if (flipVertical != 0) {
				context.TranslateCTM(0f,image.height);
				context.ScaleCTM(1f,-1f);
			}
			context.DrawImage(new CGRect(0,0,image.width, image.height), cgImage);
			if (image.data == null) {
				ImgDestroyImage(ref image);
			}																			   	
		}	

		public static void ImgDestroyImage(ref demoImage image) {
			image = null;
		}

		public static int MatrixMultiply(int rows1, int cols1, ref float[] mat1, int rows2, int cols2, 
        							     ref float[] mat2, ref float[] result) 
		{
			if( cols1 != rows2 )
    		{
        		return 0;
    		} else
    		{
        		float tempResult;
        		for (int i=0;i<rows1;i++)
        		{
            		for(int j=0;j<cols2;j++)
            		{
                		tempResult = 0;
                		for(int k=0;k<rows2;k++)
                		{
                    		tempResult += mat1[i*cols1 + k] * mat2[k*cols2 + j];
                		}
                		result[i*cols2 + j] = tempResult;
            		}
        		}
    		}
    		return 1;
		}
		
		public static void MtxLoadIdentity(ref float[] mtx) {
		
			mtx[ 0] = mtx[ 5] = mtx[10] = mtx[15] = 1f;
    
    		mtx[ 1] = mtx[ 2] = mtx[ 3] = mtx[ 4] =
    		mtx[ 6] = mtx[ 7] = mtx[ 8] = mtx[ 9] =
    		mtx[11] = mtx[12] = mtx[13] = mtx[14] = 0f;
		}
		
		public static void MtxTranspose(ref float[] m_in, ref float[] m_out) {
			for (int i = 0; i < 4; i++)
        		for (int j = 0; j < 4; j++)
           			 m_out[i * 4 + j] = m_in[i + 4 * j];
		}
		
		public static int InitShader(ShaderType shaderType, string source) {
			
	        int logLength;
	        int status = 0;
	        int shader =  GL.CreateShader(shaderType);
			
	        if (shader != 0) {
	        	unsafe {
			        GL.ShaderSource(shader,source);
			        GL.CompileShader(shader);
			        GL.GetShader(shader,  ShaderParameter.InfoLogLength,&logLength);
			        
			        if (logLength > 0) {
			            StringBuilder log = new StringBuilder();
			            GL.GetShaderInfoLog(shader, logLength, (int *)null, log);
			            Console.WriteLine("Vtx Shader compile log: {0}\n",log);
			        }
			        GL.GetShader(shader, ShaderParameter.CompileStatus, &status);
			        if (status == 0) {
			            Console.WriteLine("Failed to compile vtx shader: {0}\n",source);
			    	}
			    }
	        }
	        RenderUtils.CheckGLError();
	        
	        return shader;
    	}
    	
    	public static bool ValidateProgram(int prog) {
        
		    int logLength = 0, status = 0;
		    
		    unsafe {
			    GL.ValidateProgram(prog);
			    GL.GetProgram(prog, ProgramParameter.InfoLogLength, &logLength);
			    if (logLength > 0)
			    {
					StringBuilder log = new StringBuilder();
			        GL.GetProgramInfoLog(prog, logLength, &logLength, log);
			        Console.WriteLine("Program validate log:{0}\n",log);
			    }
			    
			    GL.GetProgram(prog, ProgramParameter.ValidateStatus, &status);
			}
		    if (status == 0) {return false;}
		    
		    return true;
    	}
    	
    	public static int CreateProgram(string vertexShaderSrc, string fragmentShaderSrc) {
        
	        int vertShader = InitShader(ShaderType.VertexShader, vertexShaderSrc);
	        int fragShader = InitShader(ShaderType.FragmentShader, fragmentShaderSrc);
	        
	         if (vertShader == 0 || fragShader == 0)
	         {
	         	return 0;
	         }
	        
	         int programStatus = 0, logLength = 0;
	         int program = GL.CreateProgram();
	         
	         if (program != 0) {
	         	unsafe  {
			        GL.AttachShader(program, vertShader);
			        GL.AttachShader(program, fragShader);
			        GL.LinkProgram(program);
			         GL.GetProgram(program, ProgramParameter.InfoLogLength, &logLength);
			         if (logLength > 0)
			         {
					 	StringBuilder log = new StringBuilder(512);
			         	GL.GetProgramInfoLog(program, 512, &logLength,  log);
			         	Console.WriteLine("Program link log:\n{0}\n",log);
			         }
			         GL.GetProgram(program, ProgramParameter.LinkStatus, &programStatus);
			         if (programStatus == 0)
			         {
			         	Console.WriteLine("Failed to link program:\n)");
			         	return 0;
			         }
			     }
	         }
	         return program;
    	}
    	
    	internal static int BuildTexture(demoImage image)  {
        	int  texName = 0;
        
        	// Create a texture object to apply to model
        	unsafe  {
        		GL.GenTextures(1, &texName);
        		GL.BindTexture(TextureTarget.Texture2D, texName);
        
       		 	// Set up filter and wrap modes for this texture object
        		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)All.Nearest);

        
        		// Allocate and load image data into texture
        		GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)image.format, 
							  (int)image.width,(int)image.height, 0,
                     		   image.format, image.type, Marshal.UnsafeAddrOfPinnedArrayElement(image.data,0));        
        		RenderUtils.CheckGLError();
        	}
        
        	return texName;
        }
        
        public static int LoadTextureFromFileName(string filePathName) {
        
        	demoImage image;
        
			ImgLoadImage(filePathName,1,out image);
        
        
        	return BuildTexture(image);
    	}
    
    	public static int LoadTextureFromFileName(string filePathName, out CGSize dims) {
            demoImage image;

			ImgLoadImage(filePathName,1,out image);
        	dims = new CGSize(image.width, image.height);
        
        	return BuildTexture(image);
    	}
    	
    	public static int CreateVideoTexture()  {
	        int textureID = 0;
	        
	        unsafe {
	        	GL.GenTextures(1, &textureID);
	        	GL.BindTexture(TextureTarget.Texture2D, textureID);
	        	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)All.Linear);
	        	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)All.Linear);
	        	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)All.ClampToEdge);
	        	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)All.ClampToEdge);
	        	GL.BindTexture(TextureTarget.Texture2D, 0);
	        }
	        
	        return textureID;
    	}
	}
}
	