//
// Auto-generated from generator.cs, do not edit
//
// We keep references to objects, so warning 414 is expected

#pragma warning disable 414

using System;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using UIKit;
using GLKit;
using Metal;
using MapKit;
using Photos;
using ModelIO;
using SceneKit;
using Contacts;
using Security;
using Messages;
using AudioUnit;
using CoreVideo;
using CoreMedia;
using QuickLook;
using CoreImage;
using SpriteKit;
using Foundation;
using CoreMotion;
using ObjCRuntime;
using AddressBook;
using MediaPlayer;
using GameplayKit;
using CoreGraphics;
using CoreLocation;
using AVFoundation;
using NewsstandKit;
using FileProvider;
using CoreAnimation;
using CoreFoundation;

namespace PikkartVideoPlayerBindingLibrary {
	[Register("_TtC18PikkartVideoPlayer18PikkartVideoPlayer", true)]
	public unsafe partial class PikkartVideoPlayer : NSObject {
		
		[CompilerGenerated]
		static readonly IntPtr class_ptr = Class.GetHandle ("_TtC18PikkartVideoPlayer18PikkartVideoPlayer");
		
		public override IntPtr ClassHandle { get { return class_ptr; } }
		
		[CompilerGenerated]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		public PikkartVideoPlayer () : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType ().Assembly == global::ApiDefinition.Messaging.this_assembly;
			if (IsDirectBinding) {
				InitializeHandle (global::ApiDefinition.Messaging.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.Selector.GetHandle ("init")), "init");
			} else {
				InitializeHandle (global::ApiDefinition.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.Selector.GetHandle ("init")), "init");
			}
		}

		[CompilerGenerated]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected PikkartVideoPlayer (NSObjectFlag t) : base (t)
		{
			IsDirectBinding = GetType ().Assembly == global::ApiDefinition.Messaging.this_assembly;
		}

		[CompilerGenerated]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected internal PikkartVideoPlayer (IntPtr handle) : base (handle)
		{
			IsDirectBinding = GetType ().Assembly == global::ApiDefinition.Messaging.this_assembly;
		}

		[Export ("loadWithFilename:playImmediately:seekPosition:")]
		[CompilerGenerated]
		public virtual bool Load (string filename, bool playImmediately, float seekPosition)
		{
			if (filename == null)
				throw new ArgumentNullException ("filename");
			var nsfilename = NSString.CreateNative (filename);
			
			bool ret;
			if (IsDirectBinding) {
				ret = global::ApiDefinition.Messaging.bool_objc_msgSend_IntPtr_bool_float (this.Handle, Selector.GetHandle ("loadWithFilename:playImmediately:seekPosition:"), nsfilename, playImmediately, seekPosition);
			} else {
				ret = global::ApiDefinition.Messaging.bool_objc_msgSendSuper_IntPtr_bool_float (this.SuperHandle, Selector.GetHandle ("loadWithFilename:playImmediately:seekPosition:"), nsfilename, playImmediately, seekPosition);
			}
			NSString.ReleaseNative (nsfilename);
			
			return ret;
		}
		
		[Export ("pause")]
		[CompilerGenerated]
		public virtual bool Pause ()
		{
			if (IsDirectBinding) {
				return global::ApiDefinition.Messaging.bool_objc_msgSend (this.Handle, Selector.GetHandle ("pause"));
			} else {
				return global::ApiDefinition.Messaging.bool_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("pause"));
			}
		}
		
		[Export ("playWithSeekposition:")]
		[CompilerGenerated]
		public virtual bool Play (float seekPosition)
		{
			if (IsDirectBinding) {
				return global::ApiDefinition.Messaging.bool_objc_msgSend_float (this.Handle, Selector.GetHandle ("playWithSeekposition:"), seekPosition);
			} else {
				return global::ApiDefinition.Messaging.bool_objc_msgSendSuper_float (this.SuperHandle, Selector.GetHandle ("playWithSeekposition:"), seekPosition);
			}
		}
		
		[Export ("seekTo:")]
		[CompilerGenerated]
		public virtual bool Seek (float position)
		{
			if (IsDirectBinding) {
				return global::ApiDefinition.Messaging.bool_objc_msgSend_float (this.Handle, Selector.GetHandle ("seekTo:"), position);
			} else {
				return global::ApiDefinition.Messaging.bool_objc_msgSendSuper_float (this.SuperHandle, Selector.GetHandle ("seekTo:"), position);
			}
		}
		
		[Export ("updateVideoData")]
		[CompilerGenerated]
		public virtual void UpdateVideoData ()
		{
			if (IsDirectBinding) {
				global::ApiDefinition.Messaging.void_objc_msgSend (this.Handle, Selector.GetHandle ("updateVideoData"));
			} else {
				global::ApiDefinition.Messaging.void_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("updateVideoData"));
			}
		}
		
		[CompilerGenerated]
		public virtual CGSize Size {
			[Export ("videoSize")]
			get {
				CGSize ret;
				if (IsDirectBinding) {
					if (Runtime.Arch == Arch.DEVICE) {
						if (IntPtr.Size == 8) {
							ret = global::ApiDefinition.Messaging.CGSize_objc_msgSend (this.Handle, Selector.GetHandle ("videoSize"));
						} else {
							global::ApiDefinition.Messaging.CGSize_objc_msgSend_stret (out ret, this.Handle, Selector.GetHandle ("videoSize"));
						}
					} else if (IntPtr.Size == 8) {
						ret = global::ApiDefinition.Messaging.CGSize_objc_msgSend (this.Handle, Selector.GetHandle ("videoSize"));
					} else {
						ret = global::ApiDefinition.Messaging.CGSize_objc_msgSend (this.Handle, Selector.GetHandle ("videoSize"));
					}
				} else {
					if (Runtime.Arch == Arch.DEVICE) {
						if (IntPtr.Size == 8) {
							ret = global::ApiDefinition.Messaging.CGSize_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("videoSize"));
						} else {
							global::ApiDefinition.Messaging.CGSize_objc_msgSendSuper_stret (out ret, this.SuperHandle, Selector.GetHandle ("videoSize"));
						}
					} else if (IntPtr.Size == 8) {
						ret = global::ApiDefinition.Messaging.CGSize_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("videoSize"));
					} else {
						ret = global::ApiDefinition.Messaging.CGSize_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("videoSize"));
					}
				}
				return ret;
			}
			
		}
		
		[CompilerGenerated]
		public virtual PKTVIDEO_STATE Status {
			[Export ("videoStatus")]
			get {
				if (IsDirectBinding) {
					return (PKTVIDEO_STATE) global::ApiDefinition.Messaging.int_objc_msgSend (this.Handle, Selector.GetHandle ("videoStatus"));
				} else {
					return (PKTVIDEO_STATE) global::ApiDefinition.Messaging.int_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("videoStatus"));
				}
			}
			
		}
		
		[CompilerGenerated]
		public virtual int TextureHandle {
			[Export ("videoTextureHandle")]
			get {
				if (IsDirectBinding) {
					return global::ApiDefinition.Messaging.int_objc_msgSend (this.Handle, Selector.GetHandle ("videoTextureHandle"));
				} else {
					return global::ApiDefinition.Messaging.int_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("videoTextureHandle"));
				}
			}
			
			[Export ("setVideoTextureHandle:")]
			set {
				if (IsDirectBinding) {
					global::ApiDefinition.Messaging.void_objc_msgSend_int (this.Handle, Selector.GetHandle ("setVideoTextureHandle:"), value);
				} else {
					global::ApiDefinition.Messaging.void_objc_msgSendSuper_int (this.SuperHandle, Selector.GetHandle ("setVideoTextureHandle:"), value);
				}
			}
		}
		
	} /* class PikkartVideoPlayer */
}
