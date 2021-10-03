using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public static class Enlargement {

    public static string Translate(this string word)
    {
        if (LanguageManager.SetLanguage == null)
            return word;

        Translation translation = LanguageManager.SetLanguage.FindTranslation(word);
        if (translation != null)
            return translation.languageWord;
        else
            return word;
    }
    public static Vector3 Vector3d(this Vector3Dfloat vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }
    public static Vector3Dfloat Vector3dfloat(this Vector3 position)
    {
        return new Vector3Dfloat((int)position.x, (int)position.y, (int)position.z);
    }
        
}
