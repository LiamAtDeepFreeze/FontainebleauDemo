using System;
using System.Collections;
using System.Collections.Generic;
using GameplayIngredients;
using NaughtyAttributes;
using UnityEngine;

public class AutoPlayCallable : MonoBehaviour
{
    [ReorderableList]
    public Callable[] callables;

    private void Start()
    {
        foreach (var callable in callables)
        {
            Callable.Call(callable, gameObject);
        }
    }
}
