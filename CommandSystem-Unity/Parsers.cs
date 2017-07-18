using UnityEngine;

namespace SickDev.CommandSystem {
    static class Parsers {
        [Parser(typeof(Vector2))]
        static Vector2 ParseVector2(string value) {
            string[] array = value.Split(' ');
            if(array.Length != 2)
                throw new InvalidArgumentFormatException<Vector2>(value);
            float[] values = new float[array.Length];
            for(int i = 0; i < array.Length; i++) {
                if(!float.TryParse(array[i].Trim(), out values[i]))
                    throw new InvalidArgumentFormatException<Vector2>(value);
            }
            return new Vector2(values[0], values[1]);
        }

        [Parser(typeof(Vector3))]
        static Vector3 ParseVector3(string value) {
            string[] array = value.Split(' ');
            if(array.Length < 2 || array.Length > 3)
                throw new InvalidArgumentFormatException<Vector3>(value);
            float[] values = new float[array.Length];
            for(int i = 0; i < array.Length; i++) {
                if(!float.TryParse(array[i].Trim(), out values[i]))
                    throw new InvalidArgumentFormatException<Vector3>(value);
            }
            return new Vector3(values[0], values[1], array.Length > 2 ? values[2] : 0);
        }

        [Parser(typeof(Vector4))]
        static Vector4 ParseVector4(string value) {
            string[] array = value.Split(' ');
            if(array.Length < 2 || array.Length > 4)
                throw new InvalidArgumentFormatException<Vector4>(value);
            float[] values = new float[array.Length];
            for(int i = 0; i < array.Length; i++) {
                if(!float.TryParse(array[i].Trim(), out values[i]))
                    throw new InvalidArgumentFormatException<Vector4>(value);
            }
            return new Vector4(values[0], values[1], array.Length > 2 ? values[2] : 0, array.Length > 3 ? values[3] : 0);
        }

        [Parser(typeof(Quaternion))]
        static Quaternion ParseQuaternion(string value) {
            string[] array = value.Split(' ');
            if(array.Length != 4)
                throw new InvalidArgumentFormatException<Quaternion>(value);
            float[] values = new float[array.Length];
            for(int i = 0; i < array.Length; i++) {
                if(!float.TryParse(array[i].Trim(), out values[i]))
                    throw new InvalidArgumentFormatException<Quaternion>(value);
            }
            return new Quaternion(values[0], values[1], values[2], values[3]);
        }

        [Parser(typeof(Color))]
        static Color ParseColor(string value) {
            string[] array = value.Split(' ');
            if(array.Length < 3 || array.Length > 4)
                throw new InvalidArgumentFormatException<Color>(value);
            float[] values = new float[array.Length];
            for(int i = 0; i < array.Length; i++) {
                if(!float.TryParse(array[i].Trim(), out values[i]))
                    throw new InvalidArgumentFormatException<Color>(value);
            }
            return new Color(values[0], values[1], values[2], array.Length > 3 ? values[3] : 1);
        }

        [Parser(typeof(Rect))]
        static Rect ParseRect(string value) {
            string[] array = value.Split(' ');
            if(array.Length != 4)
                throw new InvalidArgumentFormatException<Rect>(value);
            float[] values = new float[array.Length];
            for(int i = 0; i < array.Length; i++) {
                if(!float.TryParse(array[i].Trim(), out values[i]))
                    throw new InvalidArgumentFormatException<Rect>(value);
            }
            return new Rect(values[0], values[1], values[2], values[3]);
        }

        [Parser(typeof(GameObject))]
        static GameObject ParseGameObject(string value) {
            if(value.StartsWith("res:"))
                return Resources.Load<GameObject>(value.Substring(4).Trim());
            else
                return GameObject.Find(value);
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
    }
}