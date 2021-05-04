# ComponentFillSimplify
## 기능
* 사용자가 작성한 MonoBehaviour에서 컴포넌트의 초기화를 간단하게 만들어줍니다.

## 왜 쓸까?
Unity3D에서 게임을 만들때 컴포넌트는 반드시 필요합니다.  
사용자가 컴포넌트들을 코드로 직접 사용하기위해 아래와 같은 방법들이 자주 쓰입니다.
```C#
public class Test : MonoBehaviour
{
  // 방법1
  Collider _collider;
  public Collider collider
  {
    get
    {
      if( _collider = null )
      {
        _collider = GetComponent<Collider>();
      }
      
      return _collider;
    }
  }
  
  // 방법2
  Rigidbody _rigibdoy;
  public Rigidbody rigidbody => _rigidbody ?? ( _rigidbody = GetComponent<Rigidbody>() );
  
  
  // 방법3
  public AudioSource audio;
  void Start()
  {
    audio = GetComponent<AudioSource>();
  }
  
  // 기타 등등 내가 알지 못하는 여러가지 방법....
}
```
컴포넌트를 가져오는 맥락의 차이는 있으나 결국 컴포넌트를 사용하기 위한 코드라는 뜻은 같습니다.  
위처럼 멤버별로 컴포넌트를 직접 채워주는 방법에는 문제가 없습니다.  
하지만 멤버가 많아질수록 비슷한 코드가 점점 늘어날 것 입니다.  
ComponentFillSimplify는 이것을 간편하게 만들어줍니다.  

## 사용방법
```C#
public class Test : MonoBehaviour
{
  // GameObject 자신에게서 Rigidbody 컴포넌트를 가져옴
  [GetComponent( GetComponentScope.This )]
  public Collider collider;
  
  // GetComponentScope가 지정되지 않을 경우 GetComponentScope.This와 같음
  [GetComponent]
  public Rigidbody rigidbody;
  
  // 가장 최상위 부모 오브젝트에게서 AudioSource를 가져옴
  [GetComponent( GetComponentScope.Root )]
  public AudioSource audio { get; set; }
  
  void Start()
  {
    // GetComponent Attribute를 쓰는 모든 필드/프로퍼티에 실제 컴포넌트를 넣어줌.
    this.ComponentFill();
  }
}
```
GetComponent Attribute를 각 멤버에 넣고, this.ComponentFill()을 호출하면 됩니다.

## TODO?
* this.ComponentFill()을 쓰지 않고 자동으로 실행 되도록 변경... CustomPropertyDrawwer를 쓰지 않고 가능하긴 한걸까?
