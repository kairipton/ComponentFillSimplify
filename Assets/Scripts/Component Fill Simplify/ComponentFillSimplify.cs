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
	/// MonoBehaviour ���ο��� ������� ��� �ʵ峪 ������Ƽ�� ����Ѵ�.
	/// �ʵ�, ������Ƽ�� [GetComponent] Attribute�� ����ϰ�
	/// �ش� MonoBehaviour���� �� �޼ҵ带 ȣ���ϸ� �ڵ����� �ʵ�� ������Ƽ�� ä������.
	/// MonoBehaviour�� ���� GameObject�� �ش��ϴ� ������Ʈ�� �߰� �Ǿ� �־�� �Ѵ�.
	/// </summary>
	public static void ComponentFill(this MonoBehaviour mono)
	{
		FieldInfo[] fields = mono.GetType().GetFields( flags )
			.Where( x=> x.GetCustomAttribute( typeof(GetComponentAttribute), true ) != null )
			.ToArray();
		PropertyInfo[] props = mono.GetType().GetProperties( flags )
			.Where( x=> x.GetCustomAttribute( typeof(GetComponentAttribute), true ) != null )
			.ToArray();

		// MonoBehaviour.GetComponent�� ��� ������Ʈ��
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
	/// GetComponentScope�� ���� GetComponent�� ������ ������Ʈ���� �����´�.
	/// </summary>
	/// <param name="mono">GetComponent Attribute�� ����ϰ� �ִ� �ʵ带 ���� MonoBehaviour</param>
	/// <param name="scopeTargets">��� ������Ʈ�� ���⿡ ����ȴ�.</param>
	/// <param name="scope">GetComponent Attribute�� ������ scope</param>
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
	/// ��� Field�� Property�� ���� ������Ʈ ���� �ִ´�.
	/// </summary>
	/// <param name="sourceGo">GetComponent�� ������ ���</param>
	/// <param name="destMono">GetComponent�� ���� ������Ʈ�� ���� MonoBehaviour</param>
	static void SetValue(GameObject sourceGo, MonoBehaviour destMono, Action<object, object> setValue, Type memberType)
	{
		var comp = sourceGo.GetComponent( memberType );
		if( comp != null )
		{
			setValue( destMono, comp );
		}
		else
		{
			Debug.LogError( $"{sourceGo.name}�� {memberType.Name} ������Ʈ�� �����ϴ�" );
		}
	}
}