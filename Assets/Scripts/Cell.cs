using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
  public bool controlsNord, controlsEast, controlsSouth, controlsWest;

  struct Controls
  {
    public static bool Nord, East, South, West;
  }

  struct Connectors
  {
    public static bool Nord, East, South, West;
  }

  struct Links
  {
    public static Cell Nord, East, South, West;
  }

  struct Connections
  {
    public static bool Nord, East, South, West;
  }

  void Start()
  {
    Controls.Nord = controlsNord;
    Controls.East = controlsEast;
    Controls.South = controlsSouth;
    Controls.West = controlsWest;

    Connectors.Nord = Controls.Nord;
    Connectors.East = Controls.East;
    Connectors.South = Controls.South;
    Connectors.West = Controls.West;
  }

  void Update()
  {

  }
}
