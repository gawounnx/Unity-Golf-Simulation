# Unity-Golf-Simulation
Computer Graphics final project: Golf physics simulation using Unity and C#.
# Unity 골프 물리 시뮬레이션

## 프로젝트 소개

본 프로젝트는 컴퓨터그래픽스 기말 프로젝트로 제작한 Unity 기반 골프 물리 시뮬레이션입니다.

단순히 골프 게임을 만드는 것이 아니라, 골프공의 움직임에 영향을 주는 주요 물리 요소를 직접 구현하는 것을 목표로 하였습니다.  
구현한 주요 요소는 발사체 운동, 바람 외력, 표면별 마찰, 충돌 및 반발, 궤적 예측, 카메라 시스템입니다.

Unity의 기본 물리 컴포넌트만 사용하는 방식이 아니라, C# 스크립트에서 직접 속도, 힘, 마찰계수, 반발계수 등을 계산하여 물리 시뮬레이션을 구현하였습니다.

---

## 개발 환경

| 항목 | 내용 |
|---|---|
| Engine | Unity 6 |
| Render Pipeline | Universal Render Pipeline (URP) |
| Language | C# |
| Platform | PC |
| Project Type | Computer Graphics Final Project |

---

## 주요 구현 기능

## 1. 발사체 운동

골프공은 사용자의 입력에 따라 발사 방향과 파워가 결정됩니다.

Space 키를 누르고 있는 동안 파워가 증가하며, 키를 떼면 현재 파워 값을 이용해 공이 발사됩니다.

```csharp
Vector3 finalDirection =
    shootDirection * currentPower
    + Vector3.up * upwardPower;

rb.AddForce(finalDirection, ForceMode.Impulse);
```

### 적용 개념

- 초기 속도
- 중력
- 포물선 운동
- 수평 속도와 수직 속도 분리

### 사용 수식

```text
P = P0 + Vt + 1/2at²
```

여기서

```text
P  : 현재 위치
P0 : 초기 위치
V  : 초기 속도
a  : 가속도
t  : 시간
```

---

## 2. 바람 외력

게임 시작 시 랜덤한 바람 벡터를 생성합니다.

```csharp
windForce = new Vector3(
    Random.Range(-2f, 2f),
    0,
    Random.Range(-2f, 2f)
);
```

공이 움직이는 동안 바람 벡터를 외력으로 적용합니다.

```csharp
rb.AddForce(windManager.windForce);
```

또한 바람은 공의 실제 이동뿐 아니라 궤적 예측선에도 반영됩니다.

### UI 표시

화면에는 현재 바람의 방향과 세기가 표시됩니다.

예시:

```text
Wind : → 1.5
```

### 적용 개념

- 외력
- 벡터 합성
- 방향 벡터
- 힘의 크기

---

## 3. 표면별 마찰

골프장 표면을 Fairway, Rough, Sand로 구분하고 각 표면마다 다른 마찰계수를 적용했습니다.

| 표면 | 마찰계수 | 특징 |
|---|---:|---|
| Fairway | 0.995 | 가장 적게 감속됨 |
| Rough | 0.98 | Fairway보다 빠르게 감속됨 |
| Sand | 0.95 | 가장 빠르게 감속됨 |

공이 표면 위에 있을 때 수평 속도에 마찰계수를 곱하여 감속시켰습니다.

```csharp
horizontalVelocity *= friction;
```

### 사용 수식

```text
Vnew = Vold × μ
```

여기서

```text
Vold : 기존 속도
Vnew : 감속 후 속도
μ    : 마찰계수
```

### 구현 의도

Fairway는 잔디가 짧고 매끄러운 표면이므로 공이 오래 굴러가도록 설정했습니다.  
Rough는 잔디 저항이 더 크기 때문에 감속을 크게 적용했습니다.  
Sand는 벙커를 표현하기 위해 가장 큰 감속을 적용했습니다.

---

## 4. 충돌 및 반발

공이 벽 또는 장애물에 충돌하면 반사 벡터를 계산하고 반발계수를 적용하여 튕기도록 구현했습니다.

```csharp
Vector3 reflect =
    Vector3.Reflect(
        rb.linearVelocity,
        collision.contacts[0].normal
    );

rb.linearVelocity = reflect * 0.8f;
```

### 적용 개념

- 충돌 감지
- 법선 벡터
- 반사 벡터
- 반발계수

### 사용 수식

```text
Vafter = Reflect(Vbefore, N) × e
```

여기서

```text
Vbefore : 충돌 전 속도
N       : 충돌면의 법선 벡터
e       : 반발계수
Vafter  : 충돌 후 속도
```

### 반발계수 설정

```text
e = 0.8
```

반발계수 1.0은 에너지 손실이 거의 없는 충돌이고, 0에 가까울수록 충돌 후 속도가 크게 감소합니다.  
본 프로젝트에서는 충돌 후 속도의 80%를 유지하도록 설정하여 자연스럽게 튕기는 느낌을 구현했습니다.

---

## 5. 지면 바운스

공이 지면에 닿을 때 완전히 붙어버리지 않도록 수직 속도를 반전시키고 바운스 계수를 적용했습니다.

```csharp
if (collision.contacts[0].normal.y > 0.5f)
{
    Vector3 velocity = rb.linearVelocity;

    if (velocity.y < -1f)
    {
        velocity.y = -velocity.y * 0.3f;
        rb.linearVelocity = velocity;
    }
}
```

### 사용 수식

```text
Vy_after = -Vy_before × e
```

### 바운스 계수

```text
e = 0.3
```

골프공이 과도하게 튀지 않고 한두 번 자연스럽게 튄 후 굴러가도록 낮은 값을 사용했습니다.

---

## 6. 궤적 예측

발사 전 사용자가 예상 경로를 확인할 수 있도록 LineRenderer를 이용하여 궤적 예측선을 구현했습니다.

```csharp
Vector3 point =
    startPos
    + velocity * t
    + 0.5f * Physics.gravity * t * t
    + 0.5f * windManager.windForce * t * t;

trajectoryLine.SetPosition(i, point);
```

### 사용 수식

```text
P = P0 + Vt + 1/2at²
```

본 프로젝트의 궤적 예측은 중력뿐만 아니라 바람의 영향도 함께 반영합니다.

### 구현 특징

- Space 키를 누르는 동안 실시간 갱신
- 파워 변화 반영
- 발사 방향 반영
- 중력 반영
- 바람 반영

---

## 7. 카메라 시스템

카메라는 두 가지 모드로 구성했습니다.

| 카메라 | 역할 |
|---|---|
| Aim Camera | 발사 전 조준 화면 |
| Follow Camera | 발사 후 공 추적 |

발사 전에는 Aim Camera를 사용하고, 공이 발사되면 Follow Camera로 전환됩니다.  
공이 멈추면 다시 Aim Camera로 복귀합니다.

```csharp
aimCamera.gameObject.SetActive(false);
followCamera.gameObject.SetActive(true);
```

공이 정지하면 다음과 같이 복귀합니다.

```csharp
aimCamera.gameObject.SetActive(true);
followCamera.gameObject.SetActive(false);
```

### 구현 목적

- 발사 전 조준 편의성 제공
- 발사 후 공의 이동 경로 확인
- 자연스러운 시점 전환 구현

---

## 8. 게임 요소

### Stroke 카운트

공을 발사할 때마다 Stroke 수가 증가합니다.

```csharp
stroke++;
```

### Hole In

공이 홀컵 Trigger에 들어가면 Hole In 판정을 수행합니다.

```csharp
if (other.CompareTag("GolfBall"))
{
    Debug.Log("HOLE IN!");
    holeInText.gameObject.SetActive(true);
}
```

Hole In이 발생하면 공의 속도를 0으로 만들고 추가 입력을 막아 게임 종료 상태로 처리합니다.

### Water Hazard

공이 물에 빠질 경우 Water Hazard로 처리합니다.

```text
Water Hazard
+1 Stroke
Return to Start Position
```

### Out Of Bounds

공이 코스 밖으로 떨어질 경우 Out Of Bounds로 처리합니다.

```text
Out Of Bounds
+1 Stroke
Return to Start Position
```

---

## 사용된 주요 파라미터

| 항목 | 값 | 설정 이유 |
|---|---:|---|
| maxPower | 25 | 과도하게 멀리 날아가지 않도록 제한 |
| upwardPower | 3 | 적절한 포물선 높이 구현 |
| windRange | -2 ~ 2 | 바람 영향이 보이되 과하지 않도록 설정 |
| Fairway friction | 0.995 | 가장 오래 굴러가는 표면 |
| Rough friction | 0.98 | 중간 정도 감속 |
| Sand friction | 0.95 | 가장 빠른 감속 |
| restitution | 0.8 | 충돌 후 자연스러운 반발 |
| ground bounce | 0.3 | 지면 충돌 시 약한 바운스 |

---

## 조작 방법

| 입력 | 기능 |
|---|---|
| A | 조준 방향 왼쪽 회전 |
| D | 조준 방향 오른쪽 회전 |
| Space 누르기 | 파워 충전 |
| Space 떼기 | 공 발사 |

---

## 프로젝트 구조

```text
Assets
├── GolfBallController.cs
├── WindManager.cs
├── WindUI.cs
├── CameraFollow.cs
├── HoleTrigger.cs
├── StrokeUI.cs
├── PowerUI.cs
├── Scenes
├── Settings
└── 3D Golf Hole Terrain Pack
```

---

## 핵심 스크립트 설명

| 스크립트 | 역할 |
|---|---|
| GolfBallController.cs | 공 발사, 마찰, 충돌, 바람, OOB 처리 |
| WindManager.cs | 랜덤 바람 생성 |
| WindUI.cs | 바람 방향 및 세기 UI 표시 |
| CameraFollow.cs | 공 추적 카메라 제어 |
| HoleTrigger.cs | Hole In 판정 |
| StrokeUI.cs | 타수 UI 표시 |
| PowerUI.cs | 파워 게이지 표시 |

---

## 구현 완료 항목

- [x] 발사체 운동
- [x] 바람 외력
- [x] 표면별 마찰
- [x] 충돌 및 반발
- [x] 지면 바운스
- [x] 궤적 예측
- [x] 카메라 전환
- [x] Stroke 카운트
- [x] Hole In
- [x] Water Hazard
- [x] Out Of Bounds

---
