using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapweaver {
    [CreateAssetMenu(fileName = "New Tile Atlas", menuName = "Tile Atlas", order = 51)]
    public class TileAtlas : ScriptableObject {
        [SerializeField]
        public List<PixelTile> tiles;

        private void OnValidate() {
            GenerateTilesPixels();
        }

        public void GenerateTilesPixels() {
            for (int i = 0; i < tiles.Count; i++) {
                int xPos = (int)tiles[i].tileCollider.rect.x, yPos = (int)tiles[i].tileCollider.rect.y;
                int width = (int)tiles[i].tileCollider.rect.width, height = (int)tiles[i].tileCollider.rect.height;
                tiles[i].tilePixels = new Color[width * height];
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        tiles[i].tilePixels[x + y * width] = tiles[i].tileCollider.texture.GetPixel(xPos + x, yPos + y);
                    }
                }
            }
        }
    }
}
