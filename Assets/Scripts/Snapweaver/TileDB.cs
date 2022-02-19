using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapweaver {
    [CreateAssetMenu(fileName = "New Tile DB", menuName = "Tile DB", order = 52)]
    public class TileDB : ScriptableObject {
        [SerializeField]
        private Sprite[] sprites;
    }
}
