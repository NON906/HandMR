using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButtonUI : MonoBehaviour
{
    public Transform[] Resetables;

    Vector3[] resetablesPosition_;
    Quaternion[] resetablesRotation_;

    void Start()
    {
        resetablesPosition_ = new Vector3[Resetables.Length];
        resetablesRotation_ = new Quaternion[Resetables.Length];
        for (int loop = 0; loop < Resetables.Length; loop++)
        {
            resetablesPosition_[loop] = Resetables[loop].position;
            resetablesRotation_[loop] = Resetables[loop].rotation;
        }
    }

    public void Click()
    {
        for (int loop = 0; loop < Resetables.Length; loop++)
        {
            Resetables[loop].position = resetablesPosition_[loop];
            Resetables[loop].rotation = resetablesRotation_[loop];
        }
    }
}
