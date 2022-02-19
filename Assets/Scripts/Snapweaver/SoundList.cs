using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapweaver {
    [CreateAssetMenu(fileName = "New Sound List", menuName = "Sound List", order = 52)]
    public class SoundList : ScriptableObject {
        [SerializeField]
        public List<SoundFxClip> soundList;
    }
}