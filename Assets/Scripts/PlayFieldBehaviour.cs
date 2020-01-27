using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldBehaviour : MonoBehaviour
{
  public GameObject cellType0;
  public GameObject cellType1;
  public GameObject cellType2;
  public GameObject cellType3;
  public GameObject cellType4;
  GameObject[,] FieldMap;

  void Start()
  {
    FieldMap = new GameObject[4, 5];
    //initial codes of cells
    int[,] FieldCode = new int[4, 5] {
      { 2, 1, 3, 1, 2 },
      { 2, 1, 3, 1, 2 },
      { 2, 1, 3, 1, 2 },
      { 2, 1, 3, 1, 2 }
    };
  }

  void Update()
  {

  }

}
