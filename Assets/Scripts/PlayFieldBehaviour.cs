﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldBehaviour : MonoBehaviour
{
  struct InitialCell
  {
    public int Code;
    public bool Turnable;
    public int Angle;
    public bool Bonus;

    public InitialCell(int codeAssigment, bool turnAssigment, int angleAssigment, bool bonusAssigment)
    {
      Code = codeAssigment;
      Turnable = turnAssigment;
      Angle = angleAssigment;
      Bonus = bonusAssigment;
    }
  }

  struct StartPoint
  {
    public int Width;
    public int Height;
    public Color SourceColor;

    public StartPoint(int heightAssigment, int widthAssigment, Color colorAssigment)
    {
      Width = widthAssigment;
      Height = heightAssigment;
      SourceColor = colorAssigment;
    }
  }

  enum PaintFrom {Start, Nord, East, South, West};

  // public GameObject cellType0; // because zero means "empty"
  public GameObject cellType1, cellType2, cellType3, cellType4, cellType5, cellType6, cellType7; // assign different types of cell prefabs
  public GameObject underlayTurnable, underlayStatic; // layers of objects
  GameObject[] CellTypes;
  bool initialTurnAllowed;
  Color emptyColor;

  StartPoint[] StartPoints;
  int StartPointsCount;
  GameObject[,] PlayFieldMap;
  int PlayFieldHeight, PlayFieldWidth; // fieldSize stored as different variable
  GameObject[,] UnderlayFieldMap;


  void Start()
  {
    #region CellTypes Assigment
    CellTypes = new GameObject[8];
    CellTypes[0] = null;
    CellTypes[1] = cellType1;
    CellTypes[2] = cellType2;
    CellTypes[3] = cellType3;
    CellTypes[4] = cellType4;
    CellTypes[5] = cellType5;
    CellTypes[6] = cellType6;
    CellTypes[7] = cellType7;
    #endregion
    initialTurnAllowed = false;
    emptyColor = Color.white;
    StartPoints = new StartPoint[2]{
      new StartPoint( 1, 2, Color.green),
      new StartPoint(4, 3, Color.red)
    };
    StartPointsCount = 2; //  later load from another place
    PlayFieldMap = new GameObject[5, 4]; //arguments sequence - width, height
    PlayFieldHeight = 5; //  later load from another place
    PlayFieldWidth = 4; //  later load from another place
    UnderlayFieldMap = new GameObject[5, 4];

    //initial cells array - later load from another place
    InitialCell[,] FieldCode = new InitialCell[5, 4] {
      { new InitialCell(2, false, 1, false), new InitialCell(5, true, 0, false), new InitialCell(2, true, 0, false), new InitialCell(2, true, 0, false) }, // upper left corner - downer left corner
      { new InitialCell(1, true, 0, false), new InitialCell(6, true, 0, false), new InitialCell(3, true, 2, false), new InitialCell(3, true, 0, false) },
      { new InitialCell(3, true, 0, false), new InitialCell(3, false, 0, false), new InitialCell(3, true, 0, false), new InitialCell(2, true, 0, false) },
      { new InitialCell(2, true, 0, false), new InitialCell(1, true, 0, false), new InitialCell(3, true, 0, false), new InitialCell(3, true, 0, false) },
      { new InitialCell(2, false, 2, false), new InitialCell(2, true, 0, false), new InitialCell(1, true, 0, false), new InitialCell(2, true, 0, false) } // upper right corner - downer right corner
    };

    BuildGameField(FieldCode);
    StartingTurnCells(FieldCode);
    UpdateAllConnections();
    ReDrawAll();
  }

  void BuildGameField(InitialCell[,] CodeArray)
  {
    // take PlayFieldHeight, PlayFieldWidth, CellTypes, underlayTurnable, underlayStatic
    // change PlayFieldMap, UnderlayFieldMap
    for (int i = 0; i < PlayFieldHeight; i++)
    {
      for (int k = 0; k < PlayFieldWidth; k++)
      {
        if (CodeArray[i, k].Code != 0)
        {
          PlayFieldMap[i, k] = Instantiate(CellTypes[CodeArray[i, k].Code], transform);
          Vector2 CellPlace = new Vector2(i * 4, k * -4);
          PlayFieldMap[i, k].transform.position = CellPlace;
          PlayFieldMap[i, k].GetComponent<Cell>().Turnable = CodeArray[i, k].Turnable;
          if (CodeArray[i, k].Turnable)
          {
            UnderlayFieldMap[i, k] = Instantiate(underlayTurnable, transform);
          }
          else
          {
            UnderlayFieldMap[i, k] = Instantiate(underlayStatic, transform);
          }
          UnderlayFieldMap[i, k].transform.position = CellPlace;
          UnderlayFieldMap[i, k].GetComponent<Renderer>().sortingOrder = -1;
        }
      }
    }
  }

  void StartingTurnCells(InitialCell[,] CodeArray)
  {
    //take initialTurnAllowed, PlayFieldHeight, PlayFieldWidth, 
    //change initialTurnAllowed, CodeArray, PlayFieldMap
    initialTurnAllowed = true;
    for (int i = 0; i < PlayFieldHeight; i++)
    {
      for (int k = 0; k < PlayFieldWidth; k++)
      {
        CodeArray[i, k].Angle = CodeArray[i, k].Angle % 4; // only 4 directions allowed
        if (CodeArray[i, k].Angle > 0)
        {
          for (int l = 0; l < CodeArray[i, k].Angle; l++)
          {
            TurnCell(PlayFieldMap[i, k]);
          }
        }
      }
    }
    initialTurnAllowed = false;
  }

  public void TurnCell(GameObject CellObject)
  {
    //prototype method also external used
    Cell toTurn = CellObject.GetComponent<Cell>();
    if (!initialTurnAllowed && !toTurn.Turnable) return; //if false - do nothing;

    CellObject.transform.Rotate(new Vector3(0, 0, -90));
    
    // connectors turning logic
    Cell.Connector tempConnector = toTurn.Nord;
    toTurn.Nord = toTurn.West;
    toTurn.West = toTurn.South;
    toTurn.South = toTurn.East;
    toTurn.East = tempConnector;

    if (initialTurnAllowed) return; // speed up initial turning
    UpdateAllConnections();
    ReDrawAll();
  }

  void UpdateAllConnections()
  {
    for (int i = 0; i < PlayFieldHeight; i++)
    {
      for (int k = 0; k < PlayFieldWidth; k++)
      {
        UpdateConnection(i, k);
      }
    }
  }

  void UpdateConnection(int posHeight, int posWidth)
  {
    Cell CurrentCell = PlayFieldMap[posHeight, posWidth].GetComponent<Cell>();
    #region Getting neighbours from PlayFieldMap
    Cell NordNeighbour = null;
    Cell EastNeighbour = null;
    Cell SouthNeighbour = null;
    Cell WestNeighbour = null;
    if (posWidth > 0)
    {
      NordNeighbour = PlayFieldMap[posHeight, posWidth - 1].GetComponent<Cell>(); 
    }
    if (posHeight < PlayFieldHeight - 1)
    {
      EastNeighbour = PlayFieldMap[posHeight + 1, posWidth].GetComponent<Cell>(); 
    }
    if (posWidth < PlayFieldWidth - 1)
    {
      SouthNeighbour = PlayFieldMap[posHeight, posWidth + 1].GetComponent<Cell>(); 
    }
    if (posHeight > 0)
    {
      WestNeighbour = PlayFieldMap[posHeight - 1, posWidth].GetComponent<Cell>();
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
    ClearField();
    // here we start redrawings from array StartPoints.
    for (int i = 0; i < StartPointsCount; i++)
    {
      ReDrawCell(StartPoints[i].Height, StartPoints[i].Width, StartPoints[i].SourceColor, PaintFrom.Start);
    }
  }

  void ClearField()
  {
    for (int i = 0; i < PlayFieldHeight; i++)
    {
      for (int k = 0; k < PlayFieldWidth; k++)
      {
        Renderer[] CellParts = PlayFieldMap[i, k].GetComponentsInChildren<Renderer>();
        for (int m = 0; m < CellParts.Length; m++)
        {
          CellParts[m].material.color = emptyColor;
        }
      }
    }
  }

  void ReDrawCell(int posHeight, int posWidth, Color newColor, PaintFrom CallDirection)
  {
    // if new color isn't same than here position start color - stop
    for (int i = 0; i < StartPointsCount; i++)
    {
      if (StartPoints[i].SourceColor != newColor && StartPoints[i].Height == posHeight && StartPoints[i].Width == posWidth) return;
    }

    Cell CurrentCell = PlayFieldMap[posHeight, posWidth].GetComponent<Cell>();
    bool incomingLine = CurrentCell.Nord.Way; // used to draw another directions, next 3 checks reassign it if needed
    if (CallDirection == PaintFrom.East) incomingLine = CurrentCell.East.Way;
    if (CallDirection == PaintFrom.South) incomingLine = CurrentCell.South.Way;
    if (CallDirection == PaintFrom.West) incomingLine = CurrentCell.West.Way;

    Renderer RendererLineA = CurrentCell.ReturnRendererLineA();
    Renderer RendererLineB = CurrentCell.ReturnRendererLineB();

    // Paint this position cell
    if (CallDirection == PaintFrom.Start) // paint from start need'nt check empty colors
    {
      RendererLineA.material.color = newColor;
      if (RendererLineB != null) RendererLineB.material.color = newColor;
    }
    else
    {
      if (!incomingLine)
      {
        if (RendererLineA.material.color != emptyColor) return; // every actual line checks to emptycolor, if not - stop
        RendererLineA.material.color = newColor;
      }
      else
      {
        if (RendererLineB != null)
        { 
          if (RendererLineB.material.color != emptyColor) return;
          RendererLineB.material.color = newColor;
        }
      }
    }

    // check what neighbours can be drawed - very complex rules, so moved to special bool
    bool PaintNordPossible = posWidth > 0 
      && CurrentCell.Nord.Connected && CallDirection != PaintFrom.Nord 
      && (CallDirection == PaintFrom.Start || incomingLine == CurrentCell.Nord.Way);
    bool PaintEastPossible = posHeight + 1 < PlayFieldHeight 
      && CurrentCell.East.Connected && CallDirection != PaintFrom.East 
      && (CallDirection == PaintFrom.Start || incomingLine == CurrentCell.East.Way);
    bool PaintSouthPossible = posWidth + 1 < PlayFieldWidth 
      && CurrentCell.South.Connected && CallDirection != PaintFrom.South
      && (CallDirection == PaintFrom.Start || incomingLine == CurrentCell.South.Way);
    bool PaintWestPossible = posHeight > 0 && CurrentCell.West.Connected
      && CurrentCell.West.Connected && CallDirection != PaintFrom.West
      && (CallDirection == PaintFrom.Start || incomingLine == CurrentCell.West.Way);

    // Paint neighbours
    if (PaintNordPossible) ReDrawCell(posHeight, posWidth - 1, newColor, PaintFrom.South);
    if (PaintEastPossible) ReDrawCell(posHeight + 1, posWidth, newColor, PaintFrom.West);
    if (PaintSouthPossible) ReDrawCell(posHeight, posWidth + 1, newColor, PaintFrom.Nord);
    if (PaintWestPossible) ReDrawCell(posHeight - 1, posWidth, newColor, PaintFrom.East);
  }
}