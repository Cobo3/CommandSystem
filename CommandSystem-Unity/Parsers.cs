using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SickDev.CommandSystem.Unity {
    static class Parsers {
        [Parser(typeof(Vector2))]
        static Vector2 ParseVector2(string value) {
            float[] values = GenericParser<float, Vector2>(value, 2, 2);
            return new Vector2(values[0], values[1]);
        }

        [Parser(typeof(Vector3))]
        static Vector3 ParseVector3(string value) {
            float[] values = GenericParser<float, Vector3>(value, 2, 3);
            return new Vector3(values[0], values[1], values.Length > 2 ? values[2] : 0);
        }

        [Parser(typeof(Vector4))]
        static Vector4 ParseVector4(string value) {
            float[] values = GenericParser<float, Vector4>(value, 2, 4);
            return new Vector4(values[0], values[1], values.Length > 2 ? values[2] : 0, values.Length > 3 ? values[3] : 0);
        }

        [Parser(typeof(Quaternion))]
        static Quaternion ParseQuaternion(string value) {
            float[] values = GenericParser<float, Quaternion>(value, 4, 4);
            return new Quaternion(values[0], values[1], values[2], values[3]);
        }

        [Parser(typeof(Color))]
        static Color ParseColor(string value) {
            float[] values = GenericParser<float, Color>(value, 3, 4);
            return new Color(values[0], values[1], values[2], values.Length > 3 ? values[3] : 1);
        }

        [Parser(typeof(Color32))]
        static Color32 ParseColor32(string value) {
            byte[] values = GenericParser<byte, Color32>(value, 3, 4);
            return new Color32(values[0], values[1], values[2], values.Length > 3 ? values[3] : (byte)255);
        }

        [Parser(typeof(Hash128))]
        static Hash128 ParseHash128(string value) {
            return Hash128.Parse(value);
        }

#if UNITY_5_3_OR_NEWER
        [Parser(typeof(UnityEngine.SceneManagement.Scene))]
        static UnityEngine.SceneManagement.Scene ParseScene(string value) {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByName(value);
        }
#endif

        [Parser(typeof(Rect))]
        static Rect ParseRect(string value) {
            float[] values = GenericParser<float, Rect>(value, 4, 4);
            return new Rect(values[0], values[1], values[2], values[3]);
        }

        [Parser(typeof(GameObject))]
        static GameObject ParseGameObject(string value) {
            if(value.StartsWith("res:", StringComparison.InvariantCultureIgnoreCase))
                return Resources.Load<GameObject>(value.Substring(4).Trim());

            for(int i = 0; i < SceneManager.sceneCount; i++) {
                GameObject[] root = SceneManager.GetSceneAt(i).GetRootGameObjects();
                for(int j = 0; j < root.Length; j++) {
                    Transform[] children = root[j].GetComponentsInChildren<Transform>(true);
                    for(int k = 0; k < children.Length; k++)
                        if(children[k].name == value)
                            return children[k].gameObject;
                }
            }
            return null;
        }

        [Parser(typeof(Texture2D))]
        static Texture2D ParseTexture2D(string value) {
            return Resources.Load<Texture2D>(value);
        }

        [Parser(typeof(Sprite))]
        static Sprite ParseSprite(string value) {
            return Resources.Load<Sprite>(value);
        }

        [Parser(typeof(AudioClip))]
        static AudioClip ParseAudioClip(string value) {
            return Resources.Load<AudioClip>(value);
        }

        [Parser(typeof(Material))]
        static Material ParseMaterial(string value) {
            return Resources.Load<Material>(value);
        }

        static ArgumentType[] GenericParser<ArgumentType, ObjectType>(string value, int min, int max) where ArgumentType:IConvertible {
            string[] array = GenericSplitter<ObjectType>(value, min, max);

            ArgumentType[] values = new ArgumentType[array.Length];
            for(int i = 0; i < array.Length; i++)
                values[i] = (ArgumentType)Convert.ChangeType(array[i].Trim(), typeof(ArgumentType));
            return values;
        }

        static string[] GenericSplitter<T>(string value, int min, int max) {
            string[] array = value.Split(' ');
            if(array.Length < min || array.Length > max)
                throw new InvalidArgumentFormatException<T>(value);
            return array;
        }
    }
}