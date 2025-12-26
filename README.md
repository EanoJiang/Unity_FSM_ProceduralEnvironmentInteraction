## AnimationRigging 与 FinalIK 对应关系

### 1) `TwoBoneIKConstraint`

- FinalIK ：用 `LimbIK` 或 `FABRIK`
- Animation Rigging 是 Unity 官方 C# Job + RigBuilder 管线；FinalIK 是 RootMotion 的 solver 更新逻辑
- `constraint.weight` 混合，FinalIK 对应的是 `solver.IKPositionWeight` / `IKRotationWeight`

### 2) `MultiRotationConstraint`（手腕/手掌朝向混合）

- FinalIK ：用 `RotationIK` / `AimIK` /在 `LimbIK` 内部使用 rotation weight
- `MultiRotationConstraint` 本质是“多个 source 旋转混合到 constrained bone”

### 方案：**保留状态机/检测逻辑，只把 IK 执行层替换成 FinalIK**

- **保留** `EnvironmentInteractionStateMachine/State/Context` 的大部分逻辑
- 把 `Context.CurrentIkConstraint.weight` 等操作换成：

  - `limbIK.solver.IKPositionWeight`
  - `limbIK.solver.target.position/rotation`
  <img width="263" height="48" alt="image" src="https://github.com/user-attachments/assets/c4b4e7a7-c6ce-4afd-8e05-7c31c0c700d0" />
<img width="629" height="290" alt="image" src="https://github.com/user-attachments/assets/e84c6111-29ed-46a7-8e05-21200d8e5fe5" />
<img width="628" height="315" alt="image" src="https://github.com/user-attachments/assets/ed395af1-4af6-4526-b2b3-53abc28419b4" />
