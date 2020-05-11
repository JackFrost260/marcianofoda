using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticDeposit : MonoBehaviour
{
  public void Deposit()
    {
        PlasticProduction.instance.Deposit();
    }
}
