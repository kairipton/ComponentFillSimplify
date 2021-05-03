using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class ComponentFillSimplify
{
	const BindingFlags flags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;

#if UNITY_EDITOR
	[MenuItem( "GameObject/Set Component Field n Property", false, 9 )]
	static void GetComponentThis()
	{
		var go = Selection.activeGameObject;
		if( go == null ) return;
		var monos = go.GetComponents<MonoBehaviour>();
		for(int i=0; i<monos.Length; i++) monos[i].ComponentFill();
	}
#endif

	/// <summary>
	/// MonoBehaviour 내부에서 사용중인 멤버 필드나 프로퍼티에 사용한다.
	/// 필드, 프로퍼티에 [GetComponent] Attribute를 사용하고
	/// 해당 MonoBehaviour에서 이 메소드를 호출하면 자동으로 필드와 프로퍼티가 채워진다.
	/// MonoBehaviour를 가진 GameObject는 해당하는 컴포넌트가 추가 되어 있어야 한다.
	/// </summary>
	public static void ComponentFill(this MonoBehaviour mono)
	{
		FieldInfo[] fields = mono.GetType().GetFields( flags )
			.Where( x=> x.GetCustomAttribute( typeof(GetComponentAttribute), true ) != null )
			.ToArray();
		PropertyInfo[] props = mono.GetType().GetProperties( flags )
			.Where( x=> x.GetCustomAttribute( typeof(GetComponentAttribute), true ) != null )
			.ToArray();

		// MonoBehaviour.GetComponent의 대상 오브젝트들
		GameObject[] scopeTargets = new GameObject[0];

		for(int i=0; i<fields.Length; i++)
		{
			var att = (GetComponentAttribute)fields[i].GetCustomAttribute( typeof(GetComponentAttribute), true );
			if( att != null )
			{
				GetScopeTargets( mono, ref scopeTargets, att.Scope );
				for(int j=0; j<scopeTargets.Length; j++)
				{
					SetValue( scopeTargets[j], mono, fields[i].SetValue, fields[i].FieldType );
				}
			}	
		}

		for(int i=0; i<props.Length; i++)
		{
			var att = (GetComponentAttribute)props[i].GetCustomAttribute( typeof(GetComponentAttribute), true );
			if( att != null )
			{
				GetScopeTargets( mono, ref scopeTargets, att.Scope );
				for(int j=0; j<scopeTargets.Length; j++)
				{
					SetValue( scopeTargets[j], mono, props[i].SetValue, props[i].PropertyType );
				}
			}
		}
	}

	/// <summary>
	/// GetComponentScope에 따라 GetComponent를 수행할 오브젝트들을 가져온다.
	/// </summary>
	/// <param name="mono">GetComponent Attribute를 사용하고 있는 필드를 가진 MonoBehaviour</param>
	/// <param name="scopeTargets">결과 오브젝트가 여기에 저장된다.</param>
	/// <param name="scope">GetComponent Attribute에 지정된 scope</param>
	static void GetScopeTargets(MonoBehaviour mono, ref GameObject[] scopeTargets, GetComponentScope scope)
	{
		int arraySize = scope == GetComponentScope.Childs ? mono.transform.childCount : 1;
		Array.Resize( ref scopeTargets, arraySize );

		if( scope == GetComponentScope.FirstChild )
		{
			scopeTargets[0] = mono.transform.GetChild( 0 ).gameObject;
		}

		else if( scope == GetComponentScope.LastChild )
		{
			scopeTargets[0] = mono.transform.GetChild( mono.transform.childCount-1 ).gameObject;
		}

		else if( scope == GetComponentScope.Childs )
		{
			for(int j=0; j<mono.transform.childCount; j++)
			{
				scopeTargets[j] = mono.transform.GetChild( j ).gameObject;
			}
		}

		else if( scope == GetComponentScope.Parent )
		{
			scopeTargets[0] = mono.transform.parent.gameObject;
		}

		else if( scope == GetComponentScope.Root )
		{
			scopeTargets[0] = mono.transform.root.gameObject;
		}

		else // att.Scope == GetComponentScope.This 
		{
			scopeTargets[0] = mono.gameObject;
		}
	}

	/// <summary>
	/// 멤버 Field나 Property에 실제 컴포넌트 값을 넣는다.
	/// </summary>
	/// <param name="sourceGo">GetComponent를 수행할 대상</param>
	/// <param name="destMono">GetComponent로 얻은 컴포넌트를 넣을 MonoBehaviour</param>
	static void SetValue(GameObject sourceGo, MonoBehaviour destMono, Action<object, object> setValue, Type memberType)
	{
		var comp = sourceGo.GetComponent( memberType );
		if( comp != null )
		{
			setValue( destMono, comp );
		}
		else
		{
			Debug.LogError( $"{sourceGo.name}에 {memberType.Name} 컴포넌트가 없습니다" );
		}
	}
}