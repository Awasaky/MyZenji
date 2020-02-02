using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
  public bool controlsNord, controlsEast, controlsSouth, controlsWest;
  public struct Connector
  {
    public bool Control;
    public bool Connected;
    public GameObject Neighbour;
  }
  public Connector Nord, East, South, West;

  void Awake()
  {
    // copying initial position of links to Connector Controls
    Nord.Control = controlsNord;
    East.Control = controlsEast;
    South.Control = controlsSouth;
    West.Control = controlsWest;
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
