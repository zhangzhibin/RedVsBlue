using UnityEngine;
using System.Collections;

public interface IEditorSupport {
	void OnBindParts ();
}

public interface IEditorSupportInitInEditor
{
	void OnInitInEditor ();
}