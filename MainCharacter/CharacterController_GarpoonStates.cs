
using UnityEngine;

namespace Servant.Control
{
    public sealed partial class MainCharacterController
    {
        private GarpoonProjectile Projectile;
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
                    Controller.ChangeGarpoonState(ReadyState);
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
                Controller.Projectile =
                    Instantiate(GarpoonBasePrefab, Controller.gameObject.transform).
                    GetComponent<GarpoonBase>().Initialize
                    (Vector3.Normalize
                        ((Vector3)MainCameraBehavior.singltone.GetCursorPos() -
                        Controller.transform.position),
                        Registry.GarpoonSpeed,
                        Registry.GarpoonProjectileMaxDistance,
                        Registry.GarpoonMaxHookDistance);
                Controller.Projectile.OnMiss += Shoot_OnMissAction;
                Controller.Projectile.OnHit += Shoot_OnHitAction;
                Controller.Projectile.OnTurnOff += Shoot_OnMissAction;
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
                Controller.ChangeControllerState(NoneState);
                Controller.ResetVelocity();
                Controller.TurnGravity(false);
                GarpoonPull puller = Controller.gameObject.AddComponent<GarpoonPull>();
                puller.OnDonePull += Controller.Projectile.TurnHookOff;
                puller.Initialize
                    (Registry.GarpoonPullStartSpeed,
                    Registry.GarpoonPullAcceleration,
                    Vector3.Normalize
                        (Controller.Projectile.transform.position - Controller.transform.position),
                    Controller.Projectile.transform.position);
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
                Controller.ResetControllerState();
                Controller.TurnGravity(true);
                if(Controller.gameObject.TryGetComponent(out GarpoonPull puller))
                {
                    puller.CancelPull();
                }
            }
            //Events
            private static void ResetMissAndHitEvents()
            {
                Controller.Projectile.OnMiss -= Shoot_OnMissAction;
                Controller.Projectile.OnHit -= Shoot_OnHitAction;
            }
            private static void Shoot_OnHitAction()
            {
                Controller.Projectile.gameObject.AddComponent<Rigidbody2D>().isKinematic=true;
                Controller.Projectile.OnTurnOff += () =>
                    Destroy(Controller.Projectile.gameObject.GetComponent<Rigidbody2D>());
                ResetMissAndHitEvents();
                Controller.ChangeGarpoonState(HookState);
            }
            private static void Shoot_OnMissAction()
            {
                ResetMissAndHitEvents();
                Controller.Projectile.OnTurnOff-= Shoot_OnMissAction;
                Controller.ChangeGarpoonState(ReadyState);
            }

            public static readonly ControllerState ReadyState = new ControllerState
                (StateName.Garpoon_ReadyState,
                UpdateAction: ReadyUpdateAction,
                LandAction: Registry.EmptyMethod,
                FallAction: Registry.EmptyMethod,
                EnterStateAction: ReadyEnterAction,
                ExitStateAction: ReadyExitAction);
            public static readonly ControllerState ShootState = new ControllerState
                (StateName.Garpoon_ShootState,
                UpdateAction: ShootUpdateAction,
                LandAction: Registry.EmptyMethod,
                FallAction: Registry.EmptyMethod,
                EnterStateAction: ShootEnterAction,
                ExitStateAction: Registry.EmptyMethod);
            public static readonly ControllerState HookState = new ControllerState
                (StateName.Garpoon_HookState,
                UpdateAction: HookUpdateAction,
                LandAction: Registry.EmptyMethod,
                FallAction: HookFallAction,
                EnterStateAction: HookEnterAction,
                ExitStateAction: HookExitAction);
            public static readonly ControllerState PullState = new ControllerState
                (StateName.Garpoon_PullState,
                UpdateAction: PullUpdateAction,
                LandAction: Registry.EmptyMethod,
                FallAction: Registry.EmptyMethod,
                EnterStateAction: PullEnterAction,
                ExitStateAction: PullExitAction);
        }
    }
}
