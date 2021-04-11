using System;
using Unigine;
using UltralightNet;
using UltralightNet.AppCore;
using ImpromptuNinjas.UltralightSharp;
using System.Text;
using View = UltralightNet.View;
using App = Unigine.App;
using Session = UltralightNet.Session;

[Component(PropertyGuid = "ac0aef82b81ce8f9db0afe048b977193c1bb49d6")]
public class UltralightImpl : Component
{
	// + UltralightNet
	UltralightNet.Renderer renderer;
	static View view;
	ULSurface surface;
	ULBitmap bitmap;
	// - UltralightNet

	// + Unigine
	Gui gui;
	WidgetSprite hud;
	int Ultralight_Gui_Id;
	Blob blob = new Blob();
	static int saved_mouse = 0;
	Texture texture = new Texture();
	static IntPtr after_render_callback_handle;
	static IntPtr before_render_callback_handle;
	// - Unigine

	void Handle_Mouse_Movement()
	{
		ULMouseEvent evt = new ULMouseEvent();
		evt.type = ULMouseEvent.Type.MouseMoved;
		evt.x = App.GetMouseX();
		evt.y = App.GetMouseY();
		evt.button = ULMouseEvent.Button.None;

		view.FireMouseEvent(evt);
	}

	int on_mouse_button_pressed(int button)
	{
		ULMouseEvent evt = new ULMouseEvent();
		evt.type = ULMouseEvent.Type.MouseDown;
		evt.x = App.GetMouseX();
		evt.y = App.GetMouseY();
		
		switch (button)
		{
			case App.BUTTON_LEFT:
				evt.button = ULMouseEvent.Button.Left;
				break;
			case App.BUTTON_RIGHT:
				evt.button = ULMouseEvent.Button.Right;
				break;
			case App.BUTTON_MIDDLE:
				evt.button = ULMouseEvent.Button.Middle;
				break;
		}

		if (mouse_is_over_unigine_widget(gui.GetChild(Ultralight_Gui_Id)))
		{
			saved_mouse = App.GetMouseButton();
			App.SetMouseButton(0);
			Gui.Get().MouseButton = 0;
		}

		view.FireMouseEvent(evt);
		return 0;
	}

	int on_mouse_button_released(int button)
	{
		ULMouseEvent evt = new ULMouseEvent();
		evt.type = ULMouseEvent.Type.MouseUp;
		evt.x = App.GetMouseX();
		evt.y = App.GetMouseY();
		
		switch (button)
		{
			case App.BUTTON_LEFT:
				evt.button = ULMouseEvent.Button.Left;
				break;
			case App.BUTTON_RIGHT:
				evt.button = ULMouseEvent.Button.Right;
				break;
			case App.BUTTON_MIDDLE:
				evt.button = ULMouseEvent.Button.Middle;
				break;
		}

		// unlock mouse state if mouse is not over hub
		if (mouse_is_over_unigine_widget(gui.GetChild(Ultralight_Gui_Id)))
		{
			App.SetMouseButton(saved_mouse);
		}

		view.FireMouseEvent(evt);		
		return 0;
	}

	int on_key_pressed(uint key)
	{
		if (key == App.KEY_ESC)
		{
			gui.Hidden = !gui.Hidden;
			hud.Hidden = gui.Hidden;
		}
		var Convertedkey = SpecialKeyToInt((int)key);

		ULKeyEvent evt = new(ULKeyEventType.RawKeyDown, 0, Convertedkey, Convertedkey, string.Empty, string.Empty, false, false, false);

		view.FireKeyEvent(evt);
		return 0;
	}

	int on_key_released(uint key)
	{
		var Convertedkey = SpecialKeyToInt((int)key);

		ULKeyEvent evt = new ULKeyEvent(ULKeyEventType.KeyUp, 0, Convertedkey, Convertedkey, string.Empty, string.Empty, false, false, false);

		view.FireKeyEvent(evt);
		return 0;
	}

	int on_unicode_key_pressed(uint key)
	{
		if (key < App.KEY_ESC || key >= App.NUM_KEYS)
		{
			var KeyToString = Encoding.ASCII.GetString(new byte[] { (byte)key });
			var ConvertedKey = (int)key;

			ULKeyEvent evt = new ULKeyEvent(ULKeyEventType.Char, 0, ConvertedKey, ConvertedKey, KeyToString, KeyToString, false, false, false);

			view.FireKeyEvent(evt);
		}
		return 0;
	}

	int SpecialKeyToInt(int key)
	{
		int ConvertedKey = 0;

		switch (key)
		{
			case App.KEY_ESC:
				ConvertedKey = ULKeyCodes.GK_ESCAPE;
				break;
			case App.KEY_TAB:
				ConvertedKey = ULKeyCodes.GK_TAB;
				break;
			case App.KEY_BACKSPACE:
				ConvertedKey = ULKeyCodes.GK_BACK;
				break;
			case App.KEY_RETURN:
				ConvertedKey = ULKeyCodes.GK_RETURN;
				break;
			case App.KEY_DELETE:
				ConvertedKey = ULKeyCodes.GK_DELETE;
				break;
			case App.KEY_INSERT:
				ConvertedKey = ULKeyCodes.GK_INSERT;
				break;
			case App.KEY_HOME:
				ConvertedKey = ULKeyCodes.GK_HOME;
				break;
			case App.KEY_END:
				ConvertedKey = ULKeyCodes.GK_END;
				break;
			case App.KEY_PGUP:
				ConvertedKey = ULKeyCodes.GK_PRIOR;
				break;
			case App.KEY_PGDOWN:
				ConvertedKey = ULKeyCodes.GK_NEXT;
				break;
			case App.KEY_LEFT:
				ConvertedKey = ULKeyCodes.GK_LEFT;
				break;
			case App.KEY_RIGHT:
				ConvertedKey = ULKeyCodes.GK_RIGHT;
				break;
			case App.KEY_UP:
				ConvertedKey = ULKeyCodes.GK_UP;
				break;
			case App.KEY_DOWN:
				ConvertedKey = ULKeyCodes.GK_DOWN;
				break;
			case App.KEY_SHIFT:
				ConvertedKey = ULKeyCodes.GK_SHIFT;
				break;
			case App.KEY_CTRL:
				ConvertedKey = ULKeyCodes.GK_CONTROL;
				break;
			case App.KEY_CMD:
				ConvertedKey = ULKeyCodes.GK_LWIN;
				break;
			case App.KEY_SCROLL:
				ConvertedKey = ULKeyCodes.GK_SCROLL;
				break;
			case App.KEY_CAPS:
				ConvertedKey = ULKeyCodes.GK_CAPITAL;
				break;
			case App.KEY_NUM:
				ConvertedKey = ULKeyCodes.GK_NUMLOCK;
				break;
			case App.KEY_F1:
				ConvertedKey = ULKeyCodes.GK_F1;
				break;
			case App.KEY_F2:
				ConvertedKey = ULKeyCodes.GK_F2;
				break;
			case App.KEY_F3:
				ConvertedKey = ULKeyCodes.GK_F3;
				break;
			case App.KEY_F4:
				ConvertedKey = ULKeyCodes.GK_F4;
				break;
			case App.KEY_F5:
				ConvertedKey = ULKeyCodes.GK_F5;
				break;
			case App.KEY_F6:
				ConvertedKey = ULKeyCodes.GK_F6;
				break;
			case App.KEY_F7:
				ConvertedKey = ULKeyCodes.GK_F7;
				break;
			case App.KEY_F8:
				ConvertedKey = ULKeyCodes.GK_F8;
				break;
			case App.KEY_F9:
				ConvertedKey = ULKeyCodes.GK_F9;
				break;
			case App.KEY_F10:
				ConvertedKey = ULKeyCodes.GK_F10;
				break;
			case App.KEY_F11:
				ConvertedKey = ULKeyCodes.GK_F11;
				break;
			case App.KEY_F12:
				ConvertedKey = ULKeyCodes.GK_F12;
				break;
			case App.NUM_KEYS:
				ConvertedKey = ULKeyCodes.GK_NUMPAD0;
				break;
		}	
		return ConvertedKey;
	}

	public unsafe void Init()
	{
		// + [Unigine] 

		// mouse callbacks
		App.SetButtonPressFunc(on_mouse_button_pressed);
		App.SetButtonReleaseFunc(on_mouse_button_released);

		// keyboard callbacks (characters)
		App.SetKeyPressUnicodeFunc(on_unicode_key_pressed);

		// keyboard callbacks (Special keys)
		App.SetKeyPressFunc(on_key_pressed);
		App.SetKeyReleaseFunc(on_key_released);

		// render callbacks
		before_render_callback_handle = Engine.AddCallback(Engine.CALLBACK_INDEX.BEGIN_RENDER, before_render_callback);
		after_render_callback_handle = Engine.AddCallback(Engine.CALLBACK_INDEX.END_RENDER, after_render_callback);

		createHUDWidgetSprite();
		// - [Unigine] 

		// + [Ultralight] 
		AppCoreMethods.ulEnablePlatformFontLoader();

		AppCoreMethods.ulEnablePlatformFileSystem("./resources");		

		// Create config, used for specifying resources folder (used for URL loading)
		ULConfig config = new()
		{
			ResourcePath = "./resources" // Requires "UltralightNet.Resources"
		};

		// Create Renderer
		renderer = new(config);

		// Create View
		// Session.DefaultSession - get default renderer session (used for saving cookies etc)
		view = new(renderer, (uint)App.GetWidth(), (uint)App.GetHeight(), true, Session.DefaultSession(renderer), false);		

		view.Focus();

		surface = view.Surface;

		//view.URL = "https://github.com";
		//view.URL = "file:///sample5.html";
		//view.URL = "file:///x.html";
		//view.URL = "file:///index.html";
		//view.HTML= Test();
		//view.HTML= Sample1(); // Requires "UltralightNet.Resources"
		//view.HTML = Sample2(); // Requires "UltralightNet.Resources"
		view.HTML = Sample4(); // Requires "UltralightNet.Resources"

		view.SetChangeCursorCallback((user_data, caller, frame_id) =>
		{
			//Unigine.Console.WriteLine("Dom is ready");
		});

		view.SetDOMReadyCallback((user_data, caller, frame_id, is_main_frame, url) =>
		{
			Unigine.Console.WriteLine("Dom is ready");

			string exception = "Oops!";
			view.EvaluateScript("ShowMessage('Howdy!')", out exception);

			var context = view.LockJSContext();
			var ctx = (JsContext*)context;

			JsValue* globalObj = JavaScriptCore.ContextGetGlobalObject(ctx);

			// [Ultralight] Register Callback OnButtonClick that excecutes js on demand
			JsString* name1 = JsString.Create("OnButtonClick");
			var func1 = JavaScriptCore.JsObjectMakeFunctionWithCallback(ctx, name1, new FnPtr<ObjectCallAsFunctionCallback>(OnButtonClick));
			JavaScriptCore.JsObjectSetProperty(ctx, globalObj, name1, func1, 0, null);

			// [Ultralight] Register Callback GetMessage that returns a value
			JsString* name2 = JsString.Create("GetMessage");
			var func2 = JavaScriptCore.JsObjectMakeFunctionWithCallback(ctx, name2, new FnPtr<ObjectCallAsFunctionCallback>(GetMessage));			
			JavaScriptCore.JsObjectSetProperty(ctx, globalObj, name2, func2, 0, null);
		});
		// - [Ultralight] 
	}

	public unsafe JsValue* OnButtonClick(JsContext* ctx, JsValue* function, JsValue* thisObject, UIntPtr argumentCount, JsValue** arguments, JsValue** exception)
    {
		var str = JsString.Create("document.getElementById('result').innerText = 'Ultralight rocks!'");
		var script = JavaScriptCore.StringRetain(str);

		JavaScriptCore.EvaluateScript(ctx, script, thisObject, null, 0, exception);
		JavaScriptCore.StringRelease(script);

		return JavaScriptCore.JsValueMakeNull(ctx);
	}

	public unsafe JsValue* GetMessage(JsContext* ctx, JsValue* function, JsValue* thisObject, UIntPtr argumentCount, JsValue** arguments, JsValue** exception)
	{
		var str = JsString.Create("Hello from C#!<br/>Ultralight rocks!");
		return JavaScriptCore.JsValueMakeString(ctx, str);
	}

	void createHUDWidgetSprite() 
	{
		var width = App.GetWidth();
		var height = App.GetHeight();

		gui = Gui.Get();

		hud = new WidgetSprite(gui);

		hud.SetPosition(0, 0);
		hud.Width = width;
		hud.Height = height;
		hud.SetLayerBlendFunc(0, Gui.BLEND_ONE, Gui.BLEND_ONE_MINUS_SRC_ALPHA);

		texture.Create2D(width, height, Texture.FORMAT_RGBA8, Texture.DEFAULT_FLAGS);

		Widget window = hud;

		gui.AddChild(window, Gui.ALIGN_OVERLAP);

		Ultralight_Gui_Id = gui.NumChildren - 1;
	}

	unsafe void CopyBitmapToTexture()
	{
		bitmap = surface.Bitmap;

		bitmap.SwapRedBlueChannels();

		IntPtr pixels = bitmap.LockPixels();

		byte* pixelBytes = (byte*)pixels;

		CreateTexture(hud, pixelBytes, bitmap.Width, bitmap.Height, bitmap.RowBytes);
		
		bitmap.UnlockPixels();

		bitmap.SwapRedBlueChannels();
	}

	unsafe void CreateTexture(WidgetSprite hud, byte* pixels, uint width, uint height, uint stride)
    {
		Span<byte> pixelSpan = new(pixels, (int)bitmap.RowBytes * (int)bitmap.Height);

		var lol = pixelSpan.ToArray();

		blob.SetData(lol, stride);
		texture.SetBlob(blob);
		blob.SetData(null, 0);

		hud.SetRender(texture);
    }

	string Test()
	{
		string page = 
			@"
			<html>
			  <head>
			  </head>
			  <body>
				<button onclick=""OnButtonClick();"">Click Me</button>
				<div id=""result""></div>
			  </body>
			</html>
			";
		return page;
	}

	string Sample1()
	{
		string xxxx =
			@"
			<html>
				<head>
					<style type=""text/css"">
						body {
							margin: 0;
							padding: 0;
							overflow: hidden;
							color: black;
							font-family: Arial;
							background: linear-gradient(-45deg, #acb4ff, #f5d4e2);
							display: flex;
							justify-content: center;
							align-items: center;
						}
						div {
							width: 350px;
							height: 350px;
							text-align: center;
							border-radius: 25px;
							background: linear-gradient(-45deg, #e5eaf9, #f9eaf6);
							box-shadow: 0 7px 18px -6px #8f8ae1;
						}
						h1 {
							padding: 1em;
						}
						p {
							background: white;
							padding: 2em;
							margin: 40px;
							border-radius: 25px;
						}
					</style>
				</head>
				<body>
					<div>
						<h1>Hello World!</h1>
						<p>Welcome to Ultralight!</p>
					</div>
				</body>
			</html>
			";
		return xxxx;
	}

	string Sample2()
	{
		string xxxx =
			@"
			<html>
			  <head>
				<style type=""text/css"">
				* { -webkit-user-select: none; }
				body { 
				  overflow: hidden;
				  margin: 0;
				  padding: 0;
				  background-color: #e0e3ed;
				  background: linear-gradient(-45deg, #e0e3ed, #f7f9fc);
				  width: 900px;
				  height: 600px;
				  font-family: -apple-system, 'Segoe UI', Ubuntu, Arial, sans-serif;
				}
				h2, h3 {
				  margin: 0;
				}
				div {
				  padding: 35px;
				  margin: 10px;
				  height: 510px;
				  width: 360px;
				}
				p, li { 
				  font-size: 1em;
				}
				#leftPane {
				  float: left;
				  color: #858998;
				  padding: 85px 65px;
				  height: 410px;
				  width: 300px;
				}
				#leftPane p {
				  color: #858998;
				}
				#rightPane {
				  border-radius: 15px;
				  background-color: white;
				  float: right;
				  color: #22283d;
				  box-shadow: 0 7px 24px -6px #aaacb8;
				}
				#rightPane li, #rightPane p {
				  color: #7e7f8e;
				  font-size: 0.9em;
				}
				#rightPane li {
				  list-style-type: none;
				  padding: 0.6em 0;
				  border-radius: 20px;
				  margin: 0;
				  padding-left: 1em;
				  cursor: pointer;
				}
				#rightPane li:hover {
				  background-color: #f4f6fb;
				}
				li:before {
				  content: '';
				  display:inline-block; 
				  height: 18; 
				  width: 18;
				  margin-bottom: -5px; 
				  margin-right: 1em;
				  background-image: url(""data:image/svg+xml;utf8,<svg xmlns=\
			'http://www.w3.org/2000/svg' width='18' height='18' viewBox='-2 -2 27 27'>\
			<path stroke='%23dbe2e7' stroke-width='2' fill='white' d='M12 0c-6.627 0-12 \
			5.373-12 12s5.373 12 12 12 12-5.373 12-12-5.373-12-12-12z'/></svg>"");
				}
				li.checked:before {
				  background-image: url(""data:image/svg+xml;utf8,<svg xmlns=\
			'http://www.w3.org/2000/svg' width='18' height='18' viewBox='0 0 24 24'><path \
			fill='%2334d7d6' d='M12 0c-6.627 0-12 5.373-12 12s5.373 12 12 12 12-5.373 \
			12-12-5.373-12-12-12zm-1.25 17.292l-4.5-4.364 1.857-1.858 2.643 2.506 \
			5.643-5.784 1.857 1.857-7.5 7.643z'/></svg>"");
				}
				#rightPane h5 {
				  border-bottom: 1px solid #eceef0;
				  padding-bottom: 9px;
				  margin-bottom: 1em;
				  margin-top: 3em;
				}
				#rightPane h5 {
				  padding-left: 1em;
				}
				#rightPane ul {
				  padding-left: 0;
				}
				</style>
				<script>
				  window.onload = function() {
					var listItems = document.getElementsByTagName('li');
					for(var i = 0; i < listItems.length; i++) {
					  listItems[i].onclick = function() {
						this.classList.toggle('checked');
					  }
					}
				  }
			  </script>
			  </head>
			  <body>
				<div id=""leftPane"">
				  <h2>My Planner App</h2>
				  <p>Welcome to Ultralight Tutorial 2!</p>
				</div>
				<div id=""rightPane"">
				  <h3>Upcoming Tasks</h3>
				  <p>Click a task to mark it as completed.</p>
				  <h5>Today</h5>
				  <ul>
					<li class=""checked"">Create layout for initial mockup</li>
					<li class=""checked"">Select icons for mobile interface</li>
					<li class=""checked"">Discussions regarding new sorting algorithm</li>
					<li class=""checked"">Call with automotive clients</li>
					<li>Create quote for the Tesla remodel</li>
				  </ul>
				  <h5>Upcoming</h5>
				  <ul>
					<li>Plan itinerary for conference</li>
					<li>Discuss desktop workflow optimizations</li>
					<li>Performance improvement analysis</li>
				  </ul>
				</div>
			  </body>
			</html>
			";
		return xxxx;
	}
	
	string Sample4()
	{
		string xxxx =
			@"
			<html>
				<head>
				<style type=""text/css"">
					* { -webkit-user-select: none; }
					body { 
					font-family: -apple-system, 'Segoe UI', Ubuntu, Arial, sans-serif; 
					text-align: center;
					background: linear-gradient(#FFF, #DDD);
					padding: 2em;
					}
					body.rainbow {
					background: linear-gradient(90deg, #ff2363, #fff175, #68ff9d, 
														#45dce0, #6c6eff, #9e23ff, #ff3091);
					background-size: 1000% 1000%;
					animation: ScrollGradient 10s ease infinite;
					}
					@keyframes ScrollGradient {
					0%   { background-position:0% 50%; }
					50%  { background-position:100% 50%; }
					100% { background-position:0% 50%; }
					}
					#message {
					padding-top: 2em;
					color: white;
					font-weight: bold;
					font-size: 24px;
					text-shadow: 1px 1px rgba(0, 0, 0, 0.4);
					}
				</style>
				<script type=""text/javascript"">
				function HandleButton(evt) {
					// Call our C++ callback 'GetMessage'
					var message = GetMessage();
      
					// Display the result in our 'message' div element and apply the
					// rainbow effect to our document's body.
					document.getElementById('message').innerHTML = message;
					document.body.classList.add('rainbow');
				}
				</script>
				</head>
				<body>
				<button onclick=""HandleButton(event);"">Get the Secret Message!</button>
				<div id=""message""></div>
				</body>
			</html>
			";
		return xxxx;
	}
	
	private void Update()
	{
		renderer.Update();

		renderer.Render();

		Handle_Mouse_Movement();

		surface = view.Surface;

		if (!surface.DirtyBounds.IsEmpty)
        {
			CopyBitmapToTexture();
			surface.ClearDirtyBounds();
		}
	}

	public bool mouse_is_over_unigine_widget(Widget w)
	{
		if (w.Hidden)
			return false;

		bool r = ( (gui.MouseX > 0) && (gui.MouseY > 0) && (gui.MouseX < hud.Width) && (gui.MouseY < hud.Height));
        {
			hud.Hidden = r ? false : true;
			return r;
		}
	}

	void before_render_callback()
	{
	}

	void after_render_callback()
	{
	}
}