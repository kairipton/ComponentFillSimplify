using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
	[GetComponent( GetComponentScope.This )]	// scope가 없을 경우 기본으로 This
	public Collider		col;
	
	[GetComponent]
	public Transform	trs;

	[GetComponent( GetComponentScope.Root )]
	public Rigidbody	root;

	[GetComponent( GetComponentScope.Parent )]
	public MeshFilter	mf;

	[GetComponent( GetComponentScope.FirstChild )]
	public AudioSource	first;
	
	[GetComponent( GetComponentScope.LastChild )]
	public AudioSource	last;

	void Start()
	{
		this.ComponentFill();
	}
}
