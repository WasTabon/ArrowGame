using UnityEngine;
using System.Collections.Generic;

namespace ArrowGame.Skins
{
    [CreateAssetMenu(fileName = "SkinsConfig", menuName = "ArrowGame/Skins Config")]
    public class SkinsConfig : ScriptableObject
    {
        public List<NeedleSkinDefinition> skins = new List<NeedleSkinDefinition>();
        public NeedleSkinDefinition defaultSkin;

        public NeedleSkinDefinition GetById(string id)
        {
            return skins.Find(s => s.id == id);
        }

        public NeedleSkinDefinition GetDefault()
        {
            if (defaultSkin != null) return defaultSkin;
            if (skins.Count > 0) return skins[0];
            return null;
        }
    }
}
