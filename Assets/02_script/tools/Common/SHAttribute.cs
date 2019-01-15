using UnityEngine;

using System;
using System.Collections;

// 특성 : Mono를 상속받은 클래스내 함수를 인스펙터에 버튼형태로 노출합니다.
[AttributeUsage(AttributeTargets.Method)]
public class FuncButton : Attribute
{
}

// 특성 : Mono를 상속받은 클래스내 필드를 인스펙터에 읽기전용으로 노출합니다.
public class ReadOnlyField : PropertyAttribute
{
}

// 특성 : 인스펙터에 같은 타입의 자식오브젝트를 콤보박스 형태로 노출합니다.
public class SelectOnChildren : PropertyAttribute
{
}
