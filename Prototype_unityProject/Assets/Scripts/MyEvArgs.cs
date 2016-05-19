using UnityEngine;
using System.Collections;
using System;

public class MyEvArgs<T> : EventArgs
{

	public T data;

	public MyEvArgs (T _data)
	{
		this.data = _data;
	}

}
