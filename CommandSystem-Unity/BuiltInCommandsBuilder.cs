using System;
using UnityEngine;

namespace SickDev.CommandSystem.Unity {
    public abstract class BuiltInCommandsBuilder {
        protected CommandsManager manager { get; private set; }

        public BuiltInCommandsBuilder(CommandsManager manager) {
            this.manager = manager;
        }

        public abstract void Build();

        protected void Analytics() {
#if UNITY_ANALYTICS
            Type type = typeof(UnityEngine.Analytics.Analytics);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions(type.GetMethod("CustomEvent", new Type[] { typeof(string), typeof(Vector3) }));
            builder.methodsSettings.AddExceptions(type.GetMethod("CustomEvent", new Type[] { typeof(string), typeof(System.Collections.Generic.IDictionary<string, object>) }));
            builder.methodsSettings.AddExceptions(type.GetMethod("Transaction", new Type[] { typeof(string), typeof(decimal), typeof(string), typeof(string), typeof(string) }));
            builder.methodsSettings.AddExceptions(type.GetMethod("Transaction", new Type[] { typeof(string), typeof(decimal), typeof(string), typeof(string), typeof(string), typeof(bool) }));
            manager.Add(builder.Build());
#endif
        }

        protected void PerformanceReporting() {
#if UNITY_5_6_OR_NEWER
            CommandsBuilder builder = new CommandsBuilder(typeof(UnityEngine.Analytics.PerformanceReporting));
            builder.addClassName = true;
            manager.Add(builder.Build());
#endif
        }

        protected void AndroidInput() {
#if UNITY_ANDROID
            CommandsBuilder builder = new CommandsBuilder(typeof(AndroidInput));
            builder.addClassName = true;
            builder.methodsSettings.accesModiferBindings = CommandsBuilder.AccesModifierBindings.None;
            builder.propertiesSettings.AddExceptions("touchCountSecondary");
            manager.Add(builder.Build());
#endif
        }

        protected void Animator() {
            CommandsBuilder builder = new CommandsBuilder(typeof(Animator));
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void AppleReplayKit() {
#if UNITY_5_6_OR_NEWER && UNITY_IOS
            Type type = typeof(UnityEngine.Apple.ReplayKit.ReplayKit);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("StartBroadcasting");
            manager.Add(builder.Build());

            UnityEngine.Apple.ReplayKit.ReplayKit.BroadcastStatusCallback broadcastCallback = (hasStarted, errorMessage) => {
                if(hasStarted)
                    DevConsole.singleton.Log("Broadcast started successfully");
                else
                    DevConsole.singleton.Log("Broadcast couldn't get started. Error: "+errorMessage);
            };
            Action<bool, bool> broadcastMethod = (enableMicrophone, enableCamera) => {
                UnityEngine.Apple.ReplayKit.ReplayKit.StartBroadcasting(broadcastCallback, enableMicrophone, enableCamera);
            };
            manager.Add(new ActionCommand<bool, bool>(broadcastMethod, type.Name+".StartBroadcasting"));
#endif
        }

        protected void AppleTvRemote() {
#if UNITY_5_6_OR_NEWER && UNITY_IOS
            Type type = typeof(UnityEngine.Apple.TV.Remote);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.groupPrefix = "AppleTvRemote";
            manager.Add(builder.Build());
#endif
        }

        protected void Application() {
            Type type = typeof(Application);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("RequestAdvertisingIdentifierAsync");
            manager.Add(builder.Build());
        }

        protected void AudioListener() {
            Type type = typeof(AudioListener);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("GetOutputData", "GetSpectrumData");
            manager.Add(builder.Build());
        }

        protected void AudioSettings() {
            Type type = typeof(AudioSettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("GetConfiguration", "GetDSPBufferSize", "Reset");
            manager.Add(builder.Build());
            manager.Add(new ActionCommand<int>(value => {
                AudioConfiguration config = UnityEngine.AudioSettings.GetConfiguration();
                config.dspBufferSize = value;
                UnityEngine.AudioSettings.Reset(config);
            }, type.Name + ".dspBufferSize"));
            manager.Add(new ActionCommand<int>(value => {
                AudioConfiguration config = UnityEngine.AudioSettings.GetConfiguration();
                config.numRealVoices = value;
                UnityEngine.AudioSettings.Reset(config);
            }, type.Name + ".numRealVoices"));
            manager.Add(new ActionCommand<int>(value => {
                AudioConfiguration config = UnityEngine.AudioSettings.GetConfiguration();
                config.numVirtualVoices = value;
                UnityEngine.AudioSettings.Reset(config);
            }, type.Name + ".numVirtualVoices"));
            manager.Add(new FuncCommand<int>(() => {
                return UnityEngine.AudioSettings.GetConfiguration().dspBufferSize;
            }, type.Name + ".dspBufferSize"));
            manager.Add(new FuncCommand<int>(() => {
                return UnityEngine.AudioSettings.GetConfiguration().numRealVoices;
            }, type.Name + ".numRealVoices"));
            manager.Add(new FuncCommand<int>(() => {
                return UnityEngine.AudioSettings.GetConfiguration().numVirtualVoices;
            }, type.Name + ".numVirtualVoices"));
        }

        protected void AudioSource() {
            Type type = typeof(AudioSource);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Caching() {
            Type type = typeof(Caching);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("Authorize");
            manager.Add(builder.Build());
        }

        protected void Camera() {
            Type type = typeof(Camera);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.fieldsSettings.AddExceptions("onPostRender", "onPreCull", "onPreRender");
            builder.propertiesSettings.AddExceptions("current");
            builder.methodsSettings.AddExceptions("SetupCurrent", "GetAllCameras");
            manager.Add(builder.Build());
        }

        protected void Canvas() {
            Type type = typeof(Canvas);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("GetDefaultCanvasMaterial", "GetETC1SupportedCanvasMaterial");
            manager.Add(builder.Build());
        }

        protected void Color() {
            Type type = typeof(Color);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("RGBToHSV");
            manager.Add(builder.Build());
#if UNITY_5_3_OR_NEWER
            manager.Add(new FuncCommand<Color, string>(color => {
                float h, s, v;
                UnityEngine.Color.RGBToHSV(color, out h, out s, out v);
                return string.Format("H:{0} S:{1} V:{2}", h, s, v);
            }, type.Name + ".RGBToHSV"));
#endif
        }

        protected void Color32() {
            Type type = typeof(Color32);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void ColorUtility() {
            Type type = typeof(ColorUtility);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("TryParseHtmlString");
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<string, Color>(htmlColor => {
                Color color = new Color(-1, -1, -1, -1);
                UnityEngine.ColorUtility.TryParseHtmlString(htmlColor, out color);
                return color;
            }, type.Name + ".ParseHtmlString"));
        }

        protected void CrashReport() {
            Type type = typeof(CrashReport);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void CrashReportHandler() {
#if UNITY_5_5_OR_NEWER
            Type type = typeof(UnityEngine.CrashReportHandler.CrashReportHandler);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
#endif
        }

        protected void Cursor() {
            Type type = typeof(Cursor);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Debug() {
            Type type = typeof(Debug);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("Assert", "AssertFormat", "DrawLine", "DrawRay");
            builder.propertiesSettings.AddExceptions("logger");
            manager.Add(builder.Build());
#if UNITY_5_3_OR_NEWER
            manager.Add(new ActionCommand<LogType>(value => UnityEngine.Debug.unityLogger.filterLogType = value, type.Name + ".filterLogType"));
            manager.Add(new ActionCommand<bool>(value => UnityEngine.Debug.unityLogger.logEnabled = value, type.Name + ".logEnabled"));
            manager.Add(new FuncCommand<LogType>(() => UnityEngine.Debug.unityLogger.filterLogType, type.Name + ".filterLogType"));
            manager.Add(new FuncCommand<bool>(() => UnityEngine.Debug.unityLogger.logEnabled, type.Name + ".logEnabled"));
#endif
        }

        protected void PlayerConnection() {
#if UNITY_5_4_OR_NEWER
            Type type = typeof(UnityEngine.Diagnostics.PlayerConnection);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
#endif
        }

        protected void Display() {
            Type type = typeof(Display);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.fieldsSettings.accesModiferBindings = CommandsBuilder.AccesModifierBindings.None;
            builder.propertiesSettings.accesModiferBindings = CommandsBuilder.AccesModifierBindings.None;
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<int>(() => UnityEngine.Display.displays.Length, type.Name + ".displayCount"));
#if UNITY_5_6_OR_NEWER
            manager.Add(new FuncCommand<int, string>(index => {
                Display display = UnityEngine.Display.displays[index];
                return string.Format("Active: {0}\tRenderResolution: {1}x{2}\tSystemResolution: {3}x{4}",
                    display.active, display.renderingWidth, display.renderingHeight, display.systemWidth, display.systemHeight);
            }, type.Name + ".GetDisplayInfo"));
#else
            manager.Add(new FuncCommand<int, string>(index => {
                Display display = UnityEngine.Display.displays[index];
                return string.Format("RenderResolution: {0}x{1}\tSystemResolution: {2}x{3}",
                    display.renderingWidth, display.renderingHeight, display.systemWidth, display.systemHeight);
            }, type.Name + ".GetDisplayInfo"));
#endif
        }

        protected void DynamicGI() {
            Type type = typeof(DynamicGI);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("SetEmissive");
            manager.Add(builder.Build());
        }

        protected void Font() {
            Type type = typeof(Font);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("CreateDynamicFontFromOSFont");
            manager.Add(builder.Build());
        }

        protected void GameObject() {
            Type type = typeof(GameObject);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("Find");
            manager.Add(builder.Build());
            manager.Add(new ActionCommand<GameObject>(gameObject => UnityEngine.Object.DontDestroyOnLoad(gameObject), type.Name + ".DontDestroyOnLoad"));
            manager.Add(new ActionCommand<GameObject>(gameObject => UnityEngine.Object.Instantiate(gameObject), type.Name + ".Instantiate"));
            manager.Add(new ActionCommand<GameObject, string>((gameObject, methodName) => gameObject.SendMessage(methodName), type.Name + ".SendMessage"));
            manager.Add(new ActionCommand<GameObject, bool>((gameObject, value) => gameObject.SetActive(value), type.Name + ".SetActive"));
            manager.Add(new FuncCommand<GameObject, bool>(gameObject => gameObject.activeSelf, type.Name + ".GetActive"));
        }

        protected void Handheld() {
            Type type = typeof(Handheld);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("PlayFullScreenMovie");
            manager.Add(builder.Build());
        }

        protected void Hash128() {
            Type type = typeof(Hash128);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void HumanTrait() {
            Type type = typeof(HumanTrait);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Input() {
            Type type = typeof(Input);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.propertiesSettings.AddExceptions("accelerationEvents", "compass", "gyro", "touches");
            builder.methodsSettings.AddExceptions("GetAccelerationEvent", "GetTouch");
            manager.Add(builder.Build());
        }

        protected void Compass() {
            Type type = typeof(Compass);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<bool>(() => UnityEngine.Input.compass.enabled, type.Name + ".enabled"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.compass.headingAccuracy, type.Name + ".headingAccuracy"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.compass.magneticHeading, type.Name + ".magneticHeading"));
            manager.Add(new FuncCommand<Vector3>(() => UnityEngine.Input.compass.rawVector, type.Name + ".rawVector"));
            manager.Add(new FuncCommand<double>(() => UnityEngine.Input.compass.timestamp, type.Name + ".timestamp"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.compass.trueHeading, type.Name + ".trueHeading"));
        }

        protected void Gyroscope() {
            Type type = typeof(Gyroscope);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<bool>(() => UnityEngine.Input.gyro.enabled, type.Name + ".enabled"));
            manager.Add(new FuncCommand<Quaternion>(() => UnityEngine.Input.gyro.attitude, type.Name + ".attitude"));
            manager.Add(new FuncCommand<Vector3>(() => UnityEngine.Input.gyro.gravity, type.Name + ".gravity"));
            manager.Add(new FuncCommand<Vector3>(() => UnityEngine.Input.gyro.rotationRate, type.Name + ".rotationRate"));
            manager.Add(new FuncCommand<Vector3>(() => UnityEngine.Input.gyro.rotationRateUnbiased, type.Name + ".rotationRateUnbiased"));
            manager.Add(new FuncCommand<Vector3>(() => UnityEngine.Input.gyro.userAcceleration, type.Name + ".userAcceleration"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.gyro.updateInterval, type.Name + ".updateInterval"));
        }

        protected void LocationService() {
#if DEV_CONSOLE_USE_LOCATION
            Type type = typeof(LocationService);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<bool>(() => UnityEngine.Input.location.isEnabledByUser, type.Name + ".isEnabledByUser"));
            manager.Add(new FuncCommand<LocationServiceStatus>(() => UnityEngine.Input.location.status, type.Name + ".status"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.location.lastData.altitude, type.Name + ".altitude"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.location.lastData.horizontalAccuracy, type.Name + ".horizontalAccuracy"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.location.lastData.latitude, type.Name + ".latitude"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.location.lastData.longitude, type.Name + ".longitude"));
            manager.Add(new FuncCommand<float>(() => UnityEngine.Input.location.lastData.verticalAccuracy, type.Name + ".verticalAccuracy"));
            manager.Add(new FuncCommand<double>(() => UnityEngine.Input.location.lastData.timestamp, type.Name + ".timestamp"));
#endif
        }

        protected void IOSDevice() {
#if UNITY_IOS
            Type type = typeof(UnityEngine.iOS.Device);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.groupPrefix = "IOSDevice";
            manager.Add(builder.Build());
#endif
        }

        protected void IOSNotificationServices() {
#if UNITY_IOS
            Type type = typeof(UnityEngine.iOS.NotificationServices);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.groupPrefix = "IOSNotificationServices";
            builder.propertiesSettings.AddExceptions("localNotifications", "remoteNotifications", "scheduledLocalNotifications");
            builder.methodsSettings.AddExceptions("CancelLocalNotification", "GetLocalNotification", "GetRemoteNotification", "PresentLocalNotificationNow", "ScheduleLocalNotification");
            manager.Add(builder.Build());
#endif
        }

        protected void IOSOnDemandResources() {
#if UNITY_IOS
            Type type = typeof(UnityEngine.iOS.OnDemandResources);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.groupPrefix = "IOSOnDemandResources";
            builder.methodsSettings.AddExceptions("PreloadAsync");
            manager.Add(builder.Build());
#endif
        }

        protected void LayerMask() {
            Type type = typeof(LayerMask);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void LightmapSettings() {
            Type type = typeof(LightmapSettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.propertiesSettings.AddExceptions("lightmaps", "lightProbes");
            manager.Add(builder.Build());
        }

        protected void LightProbeProxyVolume() {
#if UNITY_5_4_OR_NEWER
            Type type = typeof(LightProbeProxyVolume);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
#endif
        }

        protected void LODGroup() {
            Type type = typeof(LODGroup);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void MasterServer() {
            Type type = typeof(MasterServer);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("PollHostList");
            manager.Add(builder.Build());
        }

        protected void Mathf() {
            Type type = typeof(Mathf);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("SmoothDamp", "SmoothDampAngle");
            manager.Add(builder.Build());
        }

        protected void Microphone() {
#if DEV_CONSOLE_USE_MICROPHONE
            Type type = typeof(Microphone);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("GetDeviceCaps");
            manager.Add(builder.Build());
#endif
        }

        protected void Physics() {
            Type type = typeof(Physics);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions(
                "BoxCastAll", "BoxCastNonAlloc", "CapsuleCastAll", "CapsuleCastNonAlloc", "SphereCast", "RaycastAll", "RaycastNonAlloc", "SphereCastAll", "SphereCastNonAlloc",
                "OverlapBoxNonAlloc", "OverlapCapsuleNonAlloc", "OverlapSphereNonAlloc", "ClosestPoint", "ComputePenetration", "IgnoreCollision"
            );
            builder.methodsSettings.AddExceptions(
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType() }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(Quaternion), typeof(float) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(Quaternion), typeof(float), typeof(int) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(Quaternion), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(Quaternion) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(Quaternion), typeof(float) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(Quaternion), typeof(float), typeof(int) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(Quaternion), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(float) }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(float), typeof(int) }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit).MakeByRefType() }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float) }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int) }),
                type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("Linecast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType() }),
                type.GetMethod("Linecast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(int) }),
                type.GetMethod("Linecast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray) }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(float) }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(float), typeof(int) }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit).MakeByRefType() }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit).MakeByRefType(), typeof(float) }),
                type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType() }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int) }),
                type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float) }),
                type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int) }),
                type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("OverlapCapsule", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("CheckCapsule", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("CheckBox", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Quaternion), typeof(int), typeof(QueryTriggerInteraction) }),
                type.GetMethod("OverlapBox", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Quaternion), typeof(int), typeof(QueryTriggerInteraction) })
            );
            manager.Add(builder.Build());
        }

        protected void Physics2D() {
            Type type = typeof(Physics2D);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions(
                "BoxCast", "BoxCastAll", "BoxCastNonAlloc", "CapsuleCast", "CapsuleCastAll", "CapsuleCastNonAlloc", "CircleCast", "CircleCastAll", "CircleCastNonAlloc",
                "Distance", "GetContacts", "GetIgnoreCollision", "GetRayIntersection", "GetRayIntersectionAll", "GetRayIntersectionNonAlloc", "IgnoreCollision",
                "IsTouching", "IsTouchingLayers", "Linecast", "LinecastAll", "LinecastNonAlloc", "OverlapAreaNonAlloc", "OverlapBoxNonAlloc",
                "OverlapCapsuleNonAlloc", "OverlapCircleNonAlloc", "OverlapCollider", "OverlapPoint", "OverlapPointNonAlloc", "Raycast", "RaycastAll", "RaycastNonAlloc"
            );
            builder.methodsSettings.AddExceptions(
#if UNITY_5_5_OR_NEWER
                type.GetMethod("OverlapCapsule", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(int) }),
                type.GetMethod("OverlapCapsule", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(int), typeof(float) }),
                type.GetMethod("OverlapCapsule", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(int), typeof(float), typeof(float) }),
                type.GetMethod("OverlapCapsuleAll", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(int) }),
                type.GetMethod("OverlapCapsuleAll", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(int), typeof(float) }),
                type.GetMethod("OverlapCapsuleAll", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(int), typeof(float), typeof(float) }),
#if UNITY_5_6_OR_NEWER
                type.GetMethod("OverlapArea", new Type[] { typeof(Vector2), typeof(Vector2), typeof(ContactFilter2D), typeof(Collider2D[]) }),
                type.GetMethod("OverlapBox", new Type[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(ContactFilter2D), typeof(Collider2D[]) }),
                type.GetMethod("OverlapCapsule", new Type[] { typeof(Vector2), typeof(Vector2), typeof(CapsuleDirection2D), typeof(float), typeof(ContactFilter2D), typeof(Collider2D[]) }),
                type.GetMethod("OverlapCircle", new Type[] { typeof(Vector2), typeof(float), typeof(ContactFilter2D), typeof(Collider2D[]) }),
                type.GetMethod("OverlapPoint", new Type[] { typeof(Vector2), typeof(ContactFilter2D), typeof(Collider2D[]) }),
#endif
#endif
                type.GetMethod("OverlapArea", new Type[] { typeof(Vector2), typeof(Vector2), typeof(int), typeof(float), typeof(float) }),
                type.GetMethod("OverlapAreaAll", new Type[] { typeof(Vector2), typeof(Vector2), typeof(int), typeof(float), typeof(float) }),
                type.GetMethod("OverlapBox", new Type[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float) }),
                type.GetMethod("OverlapBox", new Type[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(float) }),
                type.GetMethod("OverlapBoxAll", new Type[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float) }),
                type.GetMethod("OverlapBoxAll", new Type[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(float) }),
                type.GetMethod("OverlapCircle", new Type[] { typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(float) }),
                type.GetMethod("OverlapCircleAll", new Type[] { typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(float) })
            );
            manager.Add(builder.Build());
        }

        protected void PlayerPrefs() {
            Type type = typeof(PlayerPrefs);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
            manager.Add(new ActionCommand<string, bool>((key, value) => UnityEngine.PlayerPrefs.SetInt(key, value ? 1 : 0), type.Name + ".SetBool"));
            manager.Add(new FuncCommand<string, bool>(key => UnityEngine.PlayerPrefs.GetInt(key) != 0, type.Name + ".GetBool"));
            manager.Add(new FuncCommand<string, bool, bool>((key, defaultValue) => UnityEngine.PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0, type.Name + ".GetBool"));
        }

        protected void ProceduralMaterial() {
            Type type = typeof(ProceduralMaterial);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Profiler() {
            Type type = typeof(
#if UNITY_5_5_OR_NEWER
                UnityEngine.Profiling.Profiler
#else
                UnityEngine.Profiler
#endif
            );
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("GetRuntimeMemorySizeLong");
            builder.methodsSettings.AddExceptions(type.GetMethod("BeginSample", new Type[] { typeof(string), typeof(UnityEngine.Object) }));
            manager.Add(builder.Build());
        }

        protected void QualitySettings() {
            Type type = typeof(QualitySettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Quaternion() {
            Type type = typeof(Quaternion);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Random() {
            Type type = typeof(UnityEngine.Random);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.propertiesSettings.AddExceptions("state");
            builder.methodsSettings.AddExceptions(type.GetMethod("ColorHSV", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float) }));
            builder.methodsSettings.AddExceptions(type.GetMethod("ColorHSV", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float) }));
            builder.methodsSettings.AddExceptions(type.GetMethod("Range", new Type[] { typeof(int), typeof(int) }));
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<int, int, int>((min, max) => UnityEngine.Random.Range(min, max), type.Name + ".RangeInt"));
        }

        protected void Rect() {
            Type type = typeof(Rect);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void ReflectionProbe() {
            Type type = typeof(ReflectionProbe);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void RemoteSettings() {
#if UNITY_5_5_OR_NEWER
            Type type = typeof(RemoteSettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
#endif
        }

        protected void GraphicsSettings() {
#if UNITY_5_3_OR_NEWER
            Type type = typeof(UnityEngine.Rendering.GraphicsSettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.propertiesSettings.AddExceptions("renderPipelineAsset");
            builder.methodsSettings.AddExceptions("GetCustomShader", "SetCustomShader");
            manager.Add(builder.Build());
#endif
        }

        protected void RenderSettings() {
            Type type = typeof(RenderSettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.propertiesSettings.AddExceptions("ambientProbe", "customReflection", "skybox", "sun");
            builder.methodsSettings.AddExceptions("GetCustomShader", "SetCustomShader");
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<Material>(() => UnityEngine.RenderSettings.skybox, type.Name + ".skybox"));
#if UNITY_5_5_OR_NEWER
            manager.Add(new FuncCommand<Light>(() => UnityEngine.RenderSettings.sun, type.Name + ".sun"));
#endif
        }

        protected void SamsungTV() {
            Type type = typeof(SamsungTV);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void SceneManager() {
#if UNITY_5_3_OR_NEWER
            Type type = typeof(UnityEngine.SceneManagement.SceneManager);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("CreateScene", "GetActiveScene", "GetSceneAt", "GetSceneByBuildIndex", "GetSceneByName",
                "GetSceneByPath", "LoadSceneAsync", "UnloadSceneAsync");
            manager.Add(builder.Build());
            manager.Add(new FuncCommand<string>(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, type.Name + ".GetActiveScene"));
            manager.Add(new FuncCommand<int, string>((index) => UnityEngine.SceneManagement.SceneManager.GetSceneAt(index).name, type.Name + ".GetSceneAt"));
#if UNITY_5_5_OR_NEWER
            manager.Add(new FuncCommand<int, string>((buildIndex) => UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(buildIndex).name, type.Name + ".GetSceneByBuildIndex"));
#endif
#endif
        }

        protected void SceneUtility() {
#if UNITY_5_5_OR_NEWER
            Type type = typeof(UnityEngine.SceneManagement.SceneUtility);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
#endif
        }

        protected void Screen() {
            Type type = typeof(Screen);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Shader() {
            Type type = typeof(Shader);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("GetGlobalMatrix", "GetGlobalMatrixArray", "SetGlobalBuffer", "SetGlobalMatrix", "SetGlobalMatrixArray");
            builder.methodsSettings.AddExceptions(
                type.GetMethod("GetGlobalFloatArray", new Type[] { typeof(string), typeof(System.Collections.Generic.List<float>) }),
                type.GetMethod("GetGlobalFloatArray", new Type[] { typeof(int), typeof(System.Collections.Generic.List<float>) }),
                type.GetMethod("GetGlobalVectorArray", new Type[] { typeof(string), typeof(System.Collections.Generic.List<Vector4>) }),
                type.GetMethod("GetGlobalVectorArray", new Type[] { typeof(int), typeof(System.Collections.Generic.List<Vector4>) }),
                type.GetMethod("SetGlobalFloatArray", new Type[] { typeof(string), typeof(System.Collections.Generic.List<float>) }),
                type.GetMethod("SetGlobalFloatArray", new Type[] { typeof(int), typeof(System.Collections.Generic.List<float>) }),
                type.GetMethod("SetGlobalVectorArray", new Type[] { typeof(string), typeof(System.Collections.Generic.List<Vector4>) }),
                type.GetMethod("SetGlobalVectorArray", new Type[] { typeof(int), typeof(System.Collections.Generic.List<Vector4>) })
            );
            manager.Add(builder.Build());
        }

        protected void SortingLayer() {
            Type type = typeof(SortingLayer);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.propertiesSettings.AddExceptions("layers");
            manager.Add(builder.Build());
        }

        protected void SystemInfo() {
            Type type = typeof(SystemInfo);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Texture() {
            Type type = typeof(Texture);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Time() {
            Type type = typeof(Time);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void TouchScreenKeyboard() {
            Type type = typeof(TouchScreenKeyboard);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.methodsSettings.AddExceptions("Open");
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void Vector2() {
            Type type = typeof(Vector2);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("SmoothDamp");
            manager.Add(builder.Build());
        }

        protected void Vector3() {
            Type type = typeof(Vector3);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions("SmoothDamp");
            builder.methodsSettings.AddExceptions("OrthoNormalize");
            manager.Add(builder.Build());
        }

        protected void Vector4() {
            Type type = typeof(Vector4);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void VRInputTracking() {
            Type type = typeof(UnityEngine.VR.InputTracking);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void VRDevice() {
            Type type = typeof(UnityEngine.VR.VRDevice);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }

        protected void VRSettings() {
            Type type = typeof(UnityEngine.VR.VRSettings);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            manager.Add(builder.Build());
        }
    }
}