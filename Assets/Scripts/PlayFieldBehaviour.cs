using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldBehaviour : MonoBehaviour
{
  struct ArrayCell
  {
    public int Code;
    public bool Turnable;
    public int Angle;
    public bool Bonus;

    public ArrayCell(int codeAssigment, bool turnAssigment, int angleAssigment, bool bonusAssigment)
    {
      Code = codeAssigment;
      Turnable = turnAssigment;
      Angle = angleAssigment;
      Bonus = bonusAssigment;
    }
  }
  // public GameObject cellType0; // because zero means "null"
  public GameObject cellType1, cellType2, cellType3, cellType4, cellType5, cellType6, cellType7; // assign different types of cell prefabs
  public GameObject underlayTurnable, underlayStatic;
  GameObject[] CellTypes;
  bool initialTurn;

  int startWidth, startHeight; // energy source coordinates
  GameObject[,] FieldMap;
  GameObject[,] UnderlayMap;
  int FieldHeight, FieldWidth; // fieldSize


  void Start()
  {
    CellTypes = new GameObject[8];
    CellTypes[1] = cellType1;
    CellTypes[2] = cellType2;
    CellTypes[3] = cellType3;
    CellTypes[4] = cellType4;
    CellTypes[5] = cellType5;
    CellTypes[6] = cellType6;
    CellTypes[7] = cellType7;
    startHeight = 1;
    startWidth = 2;
    FieldMap = new GameObject[5, 4]; //width, height
    UnderlayMap = new GameObject[5, 4];
    FieldHeight = 5;
    FieldWidth = 4;   
    //initial cells array
    ArrayCell[,] FieldCode = new ArrayCell[5, 4] {
      { new ArrayCell(2, false, 1, false), new ArrayCell(1, true, 0, false), new ArrayCell(2, true, 0, false), new ArrayCell(2, true, 0, false) }, // верхний левый угол - нижний левый угол
      { new ArrayCell(1, true, 0, false), new ArrayCell(1, true, 0, false), new ArrayCell(3, true, 2, false), new ArrayCell(3, true, 0, false) },
      { new ArrayCell(3, true, 0, false), new ArrayCell(3, false, 0, false), new ArrayCell(3, true, 0, false), new ArrayCell(2, true, 0, false) },
      { new ArrayCell(2, true, 0, false), new ArrayCell(1, true, 0, false), new ArrayCell(3, true, 0, false), new ArrayCell(3, true, 0, false) },
      { new ArrayCell(2, false, 2, false), new ArrayCell(2, true, 0, false), new ArrayCell(1, true, 0, false), new ArrayCell(2, true, 0, false) } // верхний правый угол - нижний правый угол
    };

    BuildField(FieldCode);
    StartTurnCells(FieldCode);
    UpdateAllConnections();
    ReDrawAll();
  }

  void Update()
  {

  }

  void BuildField(ArrayCell[,] CodeArray)
  {
    for (int i = 0; i < FieldHeight; i++)
    {
      for (int k = 0; k < FieldWidth; k++)
      {
        if (CodeArray[i, k].Code != 0)
        {
          FieldMap[i, k] = Instantiate(CellTypes[CodeArray[i, k].Code], transform);
          Vector2 CellPlace = new Vector2(i * 4, k * -4);
          FieldMap[i, k].transform.position = CellPlace;
          FieldMap[i, k].GetComponent<Cell>().Turnable = CodeArray[i, k].Turnable;
          if (CodeArray[i, k].Turnable)
          {
            UnderlayMap[i, k] = Instantiate(underlayTurnable, transform);
          }
          else
          {
            UnderlayMap[i, k] = Instantiate(underlayStatic, transform);

          }
          UnderlayMap[i, k].transform.position = CellPlace;
          UnderlayMap[i, k].GetComponent<Renderer>().sortingOrder = -1;
        }
      }
    }
  }

  void StartTurnCells(ArrayCell[,] CodeArray)
  {
    initialTurn = true;
    for (int i = 0; i < FieldHeight; i++)
    {
      for (int k = 0; k < FieldWidth; k++)
      {
        CodeArray[i, k].Angle = CodeArray[i, k].Angle % 4;
        if (CodeArray[i, k].Angle > 0)
        {
          for (int l = 0; l < CodeArray[i, k].Angle; l++)
          {
            TurnCell(FieldMap[i, k]);
          }
        }
      }
    }
    initialTurn = false;
  }

  public void TurnCell(GameObject CellObject)
  {
    Cell toTurn = CellObject.GetComponent<Cell>();
    if (!initialTurn && !toTurn.Turnable) return; //if false - do nothing;

    CellObject.transform.Rotate(new Vector3(0, 0, -90));
    Cell.Connector tempConnector = toTurn.Nord;
    toTurn.Nord = toTurn.West;
    toTurn.West = toTurn.South;
    toTurn.South = toTurn.East;
    toTurn.East = tempConnector;

    UpdateAllConnections();
    ReDrawAll();
  }

  void UpdateAllConnections()
  {
    for (int i = 0; i < FieldHeight; i++)
    {
      for (int k = 0; k < FieldWidth; k++)
      {
        UpdateConnection(i, k);
      }
    }
  }

  void UpdateConnection(int posHeight, int posWidth)
  {
    Cell CurrentCell = FieldMap[posHeight, posWidth].GetComponent<Cell>();
    #region Assign Neighbours Links
    Cell NordNeighbour = null;
    Cell EastNeighbour = null;
    Cell SouthNeighbour = null;
    Cell WestNeighbour = null;
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
      }
      else
      {
        CurrentCell.Nord.Connected = false;
      }
    }

    if (EastNeighbour != null)
    {
      if (CurrentCell.East.Control && EastNeighbour.West.Control)
      {
        CurrentCell.East.Connected = true;
      }
      else
      {
        CurrentCell.East.Connected = false;
      }
    }

    if (SouthNeighbour != null)
    {
      if (CurrentCell.South.Control && SouthNeighbour.Nord.Control)
      {
        CurrentCell.South.Connected = true;
      }
      else
      {
        CurrentCell.South.Connected = false;
      }
    }

    if (WestNeighbour != null)
    {
      if (CurrentCell.West.Control && WestNeighbour.East.Control)
      {
        CurrentCell.West.Connected = true;
      }
      else
      {
        CurrentCell.West.Connected = false;
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
    ReDrawCell(startHeight, startWidth, Color.green);
  }

  void ReDrawCell(int posHeight, int posWidth, Color newColor)
  {
    Renderer[] CellRend = FieldMap[posHeight, posWidth].GetComponentsInChildren<Renderer>();
    for (int i = 0; i < CellRend.Length; i++)
    {
      CellRend[i].material.color = newColor;
    }

    Cell CurrentCell = FieldMap[posHeight, posWidth].GetComponent<Cell>();

    if (posWidth > 0 && CurrentCell.Nord.Connected)
    {
      Renderer[] NordCell = FieldMap[posHeight, posWidth - 1].GetComponentsInChildren<Renderer>();
      if (NordCell[0].material.color == Color.white)
      {
        ReDrawCell(posHeight, posWidth - 1, newColor);
      }
    }
    if (posHeight + 1 < FieldHeight && CurrentCell.East.Connected)
    {
      Renderer[] EastCell = FieldMap[posHeight + 1, posWidth].GetComponentsInChildren<Renderer>();
      if (EastCell[0].material.color == Color.white)
      {
        ReDrawCell(posHeight + 1, posWidth, newColor);
      }
    }
    if (posWidth + 1 < FieldWidth && CurrentCell.South.Connected)
    {
      Renderer[] SouthCell = FieldMap[posHeight, posWidth + 1].GetComponentsInChildren<Renderer>();
      if (SouthCell[0].material.color == Color.white)
      {
        ReDrawCell(posHeight, posWidth + 1, newColor);
      }
    }
    if (posHeight > 0 && CurrentCell.West.Connected)
    {
      Renderer[] WestCell = FieldMap[posHeight - 1, posWidth].GetComponentsInChildren<Renderer>();
      if (WestCell[0].material.color == Color.white)
      {
        ReDrawCell(posHeight - 1, posWidth, newColor);
      }
    }
  }
}