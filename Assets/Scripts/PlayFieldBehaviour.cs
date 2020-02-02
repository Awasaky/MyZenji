using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldBehaviour : MonoBehaviour
{
  // public GameObject cellType0; // because zero means "null"
  public GameObject cellType1, cellType2, cellType3, cellType4; // assign different types of cell prefabs
  GameObject[] CellTypes;

  int startWidth, startHeight; // energy source coordinates
  GameObject[,] FieldMap;
  int FieldHeight, FieldWidth; // fieldSize

  void Start()
  {
    CellTypes = new GameObject[5];
    CellTypes[1] = cellType1;
    CellTypes[2] = cellType2;
    CellTypes[3] = cellType3;
    CellTypes[4] = cellType4;
    startWidth = 1;
    startHeight = 2;
    FieldMap = new GameObject[5, 4];
    FieldHeight = FieldMap.GetLength(0);
    FieldWidth = FieldMap.GetLength(1);
    //initial cells array
    int[,] FieldCode = new int[5, 4] {
      { 2, 1, 2, 3 }, // x = 1
      { 1, 1, 3, 1 }, // x = 2
      { 3, 3, 2, 1 }, // x = 3
      { 2, 1, 3, 3 }, // x = 4
      { 2, 2, 3, 1 } // x = 5
    };

    BuildField(FieldCode);
    CheckAllConnections();
    ReDrawAll();
  }

  void Update()
  {
    
  }

  void BuildField(int[,] CodeArray)
  {
    for (int i = 0; i < FieldHeight; i++)
    {
      for (int k = 0; k < FieldWidth; k++)
      {
        FieldMap[i, k] = Instantiate(CellTypes[CodeArray[i, k]], transform);
        FieldMap[i, k].transform.position = new Vector2(i * 4, k * -4);
        Cell CurrentCell = FieldMap[i, k].GetComponent<Cell>();
        #region Assign Neighbours
        if (k > 0)
        {
          CurrentCell.Nord.Neighbour = FieldMap[i, k - 1];
        }
        if (i < FieldHeight - 1)
        {
          CurrentCell.East.Neighbour = FieldMap[i + 1, k];
        }
        if (k < FieldWidth - 1)
        {
          CurrentCell.South.Neighbour = FieldMap[i, k + 1];
        }
        if (i > 0)
        {
          CurrentCell.West.Neighbour = FieldMap[i - 1, k];
        }
        #endregion
      }
    }
  }

  public void TurnCell(GameObject CellObject)
  {
    CellObject.transform.Rotate(new Vector3(0, 0, -90));
    Cell toTurn = CellObject.GetComponent<Cell>();

    // Update Neighbours
    GameObject tempNeighbour = toTurn.Nord.Neighbour;
    toTurn.Nord.Neighbour = toTurn.West.Neighbour;
    toTurn.West.Neighbour = toTurn.South.Neighbour;
    toTurn.South.Neighbour = toTurn.East.Neighbour;
    toTurn.East.Neighbour = tempNeighbour;

    // Update Controls
    bool tempControl = toTurn.Nord.Control;
    toTurn.Nord.Control = toTurn.West.Control;
    toTurn.West.Control = toTurn.South.Control;
    toTurn.South.Control = toTurn.East.Control;
    toTurn.East.Control = tempControl;

    CheckAllConnections();
    ReDrawAll();
  }

  void CheckAllConnections()
  {
    for (int i = 0; i < FieldHeight; i++)
    {
      for (int k = 0; k < FieldWidth; k++)
      {
        CheckConnection(i, k);
      }
    }
  }

  void CheckConnection(int posHeight, int posWidth)
  {
    Cell CurrentCell = FieldMap[posHeight, posWidth].GetComponent<Cell>();
    Cell NordNeighbour = null;
    Cell EastNeighbour = null;
    Cell SouthNeighbour = null;
    Cell WestNeighbour = null;
    #region Assign Neighbours Links
    if (posWidth > 0)
    {
      NordNeighbour = FieldMap[posHeight, posWidth - 1].GetComponent<Cell>();
    }
    if (posHeight < FieldHeight - 1)
    {
      EastNeighbour = FieldMap[posHeight + 1, posWidth].GetComponent<Cell>();
    }
    if (posWidth < FieldWidth - 1)
    {
      SouthNeighbour = FieldMap[posHeight, posWidth + 1].GetComponent<Cell>();
    }
    if (posHeight > 0)
    {
      WestNeighbour = FieldMap[posHeight - 1, posWidth].GetComponent<Cell>();
    }
    #endregion

    #region Check Connected
    if (NordNeighbour != null)
    {
      if (CurrentCell.Nord.Control && NordNeighbour.South.Control)
      {
        CurrentCell.Nord.Connected = true;
        NordNeighbour.South.Connected = true;
      }
      else
      {
        CurrentCell.Nord.Connected = false;
        NordNeighbour.South.Connected = false;
      }
    }

    if (EastNeighbour != null)
    {
      if (CurrentCell.East.Control && EastNeighbour.West.Control)
      {
        CurrentCell.East.Connected = true;
        EastNeighbour.West.Connected = true;
      }
      else
      {
        CurrentCell.East.Connected = false;
        EastNeighbour.West.Connected = false;
      }
    }

    if (SouthNeighbour != null)
    {
      if (CurrentCell.South.Control && SouthNeighbour.Nord.Control)
      {
        CurrentCell.South.Connected = true;
        SouthNeighbour.Nord.Connected = true;
      }
      else
      {
        CurrentCell.South.Connected = false;
        SouthNeighbour.Nord.Connected = false;
      }
    }

    if (WestNeighbour != null)
    {
      if (CurrentCell.West.Control && WestNeighbour.East.Control)
      {
        CurrentCell.West.Connected = true;
        WestNeighbour.East.Connected = true;
      }
      else
      {
        CurrentCell.West.Connected = false;
        WestNeighbour.East.Connected = false;
      }
    }
    #endregion
  }

  void ReDrawAll()
  {
    for (int i = 0; i < FieldHeight; i++)
    {
      for (int k = 0; k < FieldWidth; k++)
      {
        Renderer[] toWhite = FieldMap[i, k].GetComponentsInChildren<Renderer>();
        for (int m = 0; m < toWhite.Length; m++)
        { 
          toWhite[m].material.color = Color.white;
        }
      }
    }
    ReGreenCell(startWidth, startHeight);
  }

  void ReGreenCell(int posWidth, int posHeight)
  {
    Renderer[] GreenRend = FieldMap[posWidth, posHeight].GetComponentsInChildren<Renderer>();
    for (int i = 0; i < GreenRend.Length; i++)
    {
      GreenRend[i].material.color = Color.green;
    }

    Cell CurrentCell = FieldMap[posWidth, posHeight].GetComponent<Cell>();
    if (CurrentCell.Nord.Connected)
    {
      Renderer[] NordCell = FieldMap[posWidth, posHeight - 1].GetComponentsInChildren<Renderer>();
      if (NordCell[0].material.color == Color.white)
      {
        ReGreenCell(posWidth, posHeight - 1);
      }
    }
    if (CurrentCell.East.Connected)
    {
      Renderer[] EastCell = FieldMap[posWidth + 1, posHeight].GetComponentsInChildren<Renderer>();
      if (EastCell[0].material.color == Color.white)
      {
        ReGreenCell(posWidth + 1, posHeight);
      }
    }
    if (CurrentCell.South.Connected)
    {
      Renderer[] SouthCell = FieldMap[posWidth, posHeight + 1].GetComponentsInChildren<Renderer>();
      if (SouthCell[0].material.color == Color.white)
      {
        ReGreenCell(posWidth, posHeight + 1);
      }
    }
    if (CurrentCell.West.Connected)
    {
      Renderer[] WestCell = FieldMap[posWidth - 1, posHeight].GetComponentsInChildren<Renderer>();
      if (WestCell[0].material.color == Color.white)
      { 
        ReGreenCell(posWidth - 1, posHeight);
      }
    }
  }
}