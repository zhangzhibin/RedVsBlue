using UnityEngine;
using System.Collections;

public delegate bool FuncBool(GameObject obj);

public delegate IEnumerator FuncRoutine();
public delegate WaitForSeconds FuncDelay(float delay);

public delegate void FuncCallback0();
public delegate void FuncCallback1(Object arg0);


public delegate void FuncCallback<T>(T arg0);
public delegate void FuncCallback<T0, T1>(T0 arg0, T1 arg1);
public delegate void FuncCallback<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

//public static class Delegates {
//
//}
