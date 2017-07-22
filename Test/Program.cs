using System;
using SickDev.CommandSystem;
using UnityEngine;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            Config.AddAssemblyWithCommands("Test");

            CommandsManager manager = new CommandsManager();
            manager.Load();

            Type type = typeof(Physics);
            CommandsBuilder builder = new CommandsBuilder(type);
            builder.addClassName = true;
            builder.methodsSettings.AddExceptions(
                "BoxCastAll", "BoxCastNonAlloc", "CapsuleCastAll", "CapsuleCastNonAlloc", "SphereCast",
                "RaycastAll", "RaycastNonAlloc", "SphereCastAll", "SphereCastNonAlloc",
                "OverlapBoxNonAlloc", "OverlapCapsuleNonAlloc", "OverlapSphereNonAlloc",
                "ClosestPoint", "ComputePenetration", "IgnoreCollision"
            );
            builder.methodsSettings.AddExceptions(
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(Quaternion) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(Quaternion), typeof(float) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(Quaternion), typeof(float), typeof(int) }),
                type.GetMethod("BoxCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(Quaternion), typeof(float), typeof(int), typeof(QueryTriggerInteraction) })
            //type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit) }),
            //type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit), typeof(float) }),
            //type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit), typeof(float), typeof(int) }),
            //type.GetMethod("CapsuleCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Vector3), typeof(RaycastHit), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
            //type.GetMethod("LineCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit) }),
            //type.GetMethod("LineCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(int) }),
            //type.GetMethod("LineCast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(int), typeof(QueryTriggerInteraction) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(float) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(float), typeof(int) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit), typeof(float) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit), typeof(float), typeof(int) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(float) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Ray), typeof(RaycastHit), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }),
            //type.GetMethod("Raycast", new Type[] { typeof(Vector3), typeof(Vector3), typeof(RaycastHit), typeof(float), typeof(int) })
            );
            manager.Add(builder.Build());
        }
    }
}