using System;

[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, Inherited=true, AllowMultiple=false )]
public class GetComponentAttribute : Attribute 
{ 
	public GetComponentScope Scope { get; private set; }
	public GetComponentAttribute(GetComponentScope scope = GetComponentScope.This) => Scope = scope;
}

/// <summary>
/// GetComponent로 가져올 컴포넌트의 위치를 지정한다.
/// </summary>
public enum GetComponentScope : byte
{
	/// <summary>
	/// GameObject 자신에게서 컴포넌트를 가져온다.
	/// </summary>
	This = 0,

	/// <summary>
	/// GameObject 자신의 하위 자식들에게서 가져온다.
	/// Hierarchy상에서 가장 상단에 있는 오브젝트를 가져온다.
	/// </summary>
	FirstChild,

	/// <summary>
	/// GameObject 자신의 하위 자식들에게서 가져온다.
	/// Hierarchy상에서 가장 상단에 있는 오브젝트를 가져온다.
	/// </summary>
	LastChild,

	/// <summary>
	/// GameObject 자신의 하위 자식들에게서 가져온다.
	/// 이 Attribute를 가진 멤버가 배열일 경우 배열로 가져오고,
	/// 아닐 경우 GetComponentInChildren으로 가져온다.
	/// </summary>
	Childs,

	/// <summary>
	/// GameObject 자신의 바로 위 부모 오브젝트에게서 가져온다.
	/// </summary>
	Parent,

	/// <summary>
	/// GameObject 자신의 Hierarchy상의 Root에서 가져온다.
	/// </summary>
	Root,
}
