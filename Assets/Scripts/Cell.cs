using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
  public bool controlsNord, controlsEast, controlsSouth, controlsWest;
  public bool controlsNordWay, controlsEastWay, controlsSouthWay, controlsWestWay;
  public bool Turnable;
  public struct Connector
  {
    public bool Control;
    public bool Way;
    public bool Connected;
  }
  public Connector Nord, East, South, West;
  public GameObject LineA;
  public GameObject LineB;

  void Awake()
  {
    // copying initial position of links to Connector Controls
    Nord.Control = controlsNord;
    East.Control = controlsEast;
    South.Control = controlsSouth;
    West.Control = controlsWest;

    Nord.Way = controlsNordWay;
    East.Way = controlsEastWay;
    South.Way = controlsSouthWay;
    West.Way = controlsWestWay;
  }

  void Update()
  {

  }

  void OnMouseDown()
  {
    // prototype method - on click call playfield method to turn cell inside playfield
    PlayFieldBehaviour callTarget = transform.parent.gameObject.GetComponent<PlayFieldBehaviour>();
    callTarget.TurnCell(gameObject);
  }
}
