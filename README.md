# ComponentFillSimplify
## 기능
* 사용자가 작성한 MonoBehaviour에서 특정 컴포넌트의 초기화를 간단하게 만들어줍니다.

## 사용방법
```C#
public class Test : MonoBehaviour
{
  // GameObject 자신에게서 Transform 컴포넌트를 가져옴
  [GetComponent( GetComponentScope.This )]
  public Transform  trs;
  
  // GetComponentScope가 지정되지 않을 경우 GetComponentScope.This와 같음
  [GetComponent]
  public Collider col;
  
  // 가장 최상위 부모 오브젝트에게서 Rigidbody를 가져옴
  [GetComponent( GetComponentScope.Root )]
  public Rigidbody rootRb { get; set; }
  
  void Start()
  {
    this.ComponentFill();
  }
}
```

## TODO
* childs 배열 지원
* this.ComponentFill()을 쓰지 않고 자동으로 실행 되도록 변경
