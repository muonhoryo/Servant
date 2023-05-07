
using UnityEngine;

namespace Servant.Control
{
    public sealed partial class MainCharacterController
    {
        private GarpoonBase Garpoon;
        private GarpoonProjectile Projectile => Garpoon.Projectile;
        public static GameObject GarpoonBasePrefab;
        private static class GarpoonStates
        {
            //UpdateAction
            private static void ReadyUpdateAction()
            {
                if (Input.GetButtonDown(Input_Shoot))
                {
                    Controller.ChangeGarpoonState(ShootState);
                }
            }
            private static void ShootUpdateAction()
            {
                if (Input.GetButtonUp(Input_Shoot))
                {
                    Controller.Projectile.TurnHookOff();
                }
            }
            private static void HookUpdateAction()
            {
                if (Input.GetButtonUp(Input_Shoot))
                {
                    Controller.Projectile.TurnHookOff();
                    Controller.ChangeGarpoonState(ReadyState);
                }
                else if (Input.GetButtonDown(Input_GarpoonPull))
                {
                    Controller.ChangeGarpoonState(PullState);
                }
            }
            private static void PullUpdateAction()
            {
                if (Input.GetButtonDown(Input_GarpoonPull))
                {
                    PullState.Puller.CancelPull();
                }
            }
            //FallAction
            private static void HookFallAction()
            {
                Controller.ChangeControllerState(RockingState);
            }
            //EnterAction
            private static void ReadyEnterAction()
            {
                Controller.SetGarpoonAnimation(false);
            }
            private static void ShootEnterAction()
            {
                Controller.Garpoon =
                    Instantiate(GarpoonBasePrefab, Controller.gameObject.transform).
                    GetComponent<GarpoonBase>();
                Controller.Garpoon.Initialize
                    (Vector3.Normalize
                        ((Vector3)MainCameraBehavior.singltone.GetCursorPos() -
                        Controller.transform.position),
                        Registry.GarpoonSpeed,
                        Registry.GarpoonProjectileMaxDistance,
                        Registry.GarpoonMaxHookDistance);
                Controller.Projectile.ProjectileMissEvent += Shoot_OnMissAction;
                Controller.Projectile.ProjectileHitEvent += Shoot_OnHitAction;
                Controller.Projectile.HookTurnOffEvent += Shoot_OnMissAction;
            }
            private static void HookEnterAction()
            {
                if (Controller.isFall)
                {
                    HookFallAction();
                }
            }
            private static void PullEnterAction()
            {
                if (Controller.Garpoon.HitObject.layer == Registry.GroundLayer)
                {
                    GarpoonGroundPull puller= Controller.gameObject.AddComponent<GarpoonGroundPull>();
                    PullState.Puller = puller;
                    void CancelPullState()
                    {
                        Controller.ResetControllerState();
                        Controller.Projectile.HookTurnOffEvent -= CancelPullState;
                    }
                    puller.PullDoneEvent += Controller.Projectile.TurnHookOff;
                    Controller.Projectile.HookTurnOffEvent += CancelPullState;
                    puller.Initialize
                        (Registry.GarpoonGroundPullStartSpeed,
                        Registry.GarpoonGroundPullAcceleration,
                        Controller.Projectile.transform.position,
                        Vector3.Normalize
                            (Controller.Projectile.transform.position - Controller.transform.position));
                    Controller.ChangeControllerState(MainCharacterController.PullState);
                }
                else if (Controller.Garpoon.HitObject.layer == Registry.MovableItemLayer)
                {
                    GarpoonItemPull puller = Controller.Garpoon.HitObject.AddComponent<GarpoonItemPull>();
                    PullState.Puller = puller;
                    puller.PullDoneEvent += Controller.Projectile.TurnHookOff;
                    puller.Initialize(Registry.GarpoonItemPullStartSpeed, Registry.GarpoonItemPullAcceleration,
                        Controller.transform,Controller.Projectile.transform);
                }
            }
            //ExitAction
            private static void ReadyExitAction() 
            {
                Controller.SetGarpoonAnimation(true);
            }
            private static void HookExitAction()
            {
                if (Controller.isFall) Controller.ChangeControllerState(FallState);
            }
            private static void PullExitAction()
            {
                if (PullState.Puller != null) Destroy(PullState.Puller);
            }
            //Events
            private static void ResetMissAndHitEvents()
            {
                Controller.Projectile.ProjectileMissEvent -= Shoot_OnMissAction;
                Controller.Projectile.ProjectileHitEvent -= Shoot_OnHitAction;
            }
            private static void Shoot_OnHitAction(GameObject a)
            {
                Controller.Projectile.gameObject.AddComponent<Rigidbody2D>().isKinematic=true;
                Controller.Projectile.HookTurnOffEvent += () =>
                    Destroy(Controller.Projectile.gameObject.GetComponent<Rigidbody2D>());
                ResetMissAndHitEvents();
                Controller.ChangeGarpoonState(HookState);
            }
            private static void Shoot_OnMissAction()
            {
                ResetMissAndHitEvents();
                Controller.Projectile.HookTurnOffEvent-= Shoot_OnMissAction;
                Controller.ChangeGarpoonState(ReadyState);
            }

            public static readonly ControllerState ReadyState = new ControllerState
                (StateName.Garpoon_ReadyState,
                UpdateAction: ReadyUpdateAction,
                LandAction: EmptyAction,
                FallAction: EmptyAction,
                EnterStateAction: ReadyEnterAction,
                ExitStateAction: ReadyExitAction);
            public static readonly ControllerState ShootState = new ControllerState
                (StateName.Garpoon_ShootState,
                UpdateAction: ShootUpdateAction,
                LandAction: EmptyAction,
                FallAction: EmptyAction,
                EnterStateAction: ShootEnterAction,
                ExitStateAction: EmptyAction);
            public static readonly ControllerState HookState = new ControllerState
                (StateName.Garpoon_HookState,
                UpdateAction: HookUpdateAction,
                LandAction: EmptyAction,
                FallAction: HookFallAction,
                EnterStateAction: HookEnterAction,
                ExitStateAction: HookExitAction);
            public static readonly GarpoonPullControllerState PullState =  new GarpoonPullControllerState
                (StateName.Garpoon_PullState,
                updateAction: PullUpdateAction,
                landAction: EmptyAction,
                fallAction: EmptyAction,
                enterStateAction: PullEnterAction,
                exitStateAction: PullExitAction);
        }
    }
}
