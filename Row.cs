using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
  // Array holding tiles in row
  public Tile[] tiles { get; private set; }

  // Form word from tile letters
  public string word {
    get {
        string word = "";

        for (int i = 0; i < tiles.Length; i++) {
            word += tiles[i].letter;
        }
        return word;
    }
  }

  private void Awake() {
    // Initialize tile array by getting Tile components
    tiles = GetComponentsInChildren<Tile>();
  }
}
