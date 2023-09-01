
using MuonhoryoLibrary;
using System.ComponentModel;
using UnityEngine;

namespace Servant.Serialization
{
    public sealed class EnvironmentPrefabs:MonoBehaviour,ISingltone<EnvironmentPrefabs>
    {
        public static GameObject HumanCharacterPrefab_ => instance_.MainCharacterPrefab;
        public static GameObject NonePassablePlatform_ => instance_.NonePassablePlatform;
        public static GameObject SimpleLocationTransit_ => instance_.SimpleLocationTransit;
        public static GameObject FAKELocationTransit_ => instance_.FAKELocationTransit;
        public static GameObject Wall_ => instance_.Wall;
        public static GameObject MovableBox_ => instance_.MovableBox;
        public static GameObject SaveInfoContainer_ => instance_.SaveInfoContainer;
        public static GameObject CheckPoint_ => instance_.CheckPoint;
        public static GameObject StartButton_ => instance_.StartButton;
        public static GameObject LoadGameButton_ => instance_.LoadGameButton;
        public static GameObject ClinePlatform_ => instance_.ClinePlatform;
        public static GameObject RadialRockingPoint_ => instance_.RadialRockingPoint;
        public static GameObject DeathArea_ => instance_.DeathArea;
        public static GameObject GuardAndroid_ => instance_.GuardAndroid;

        [SerializeField]
        private GameObject MainCharacterPrefab;
        [SerializeField]
        private GameObject NonePassablePlatform;
        [SerializeField]
        private GameObject SimpleLocationTransit;
        [SerializeField]
        private GameObject FAKELocationTransit;
        [SerializeField]
        private GameObject Wall;
        [SerializeField]
        private GameObject MovableBox;
        [SerializeField]
        private GameObject SaveInfoContainer;
        [SerializeField]
        private GameObject CheckPoint;
        [SerializeField]
        private GameObject StartButton;
        [SerializeField]
        private GameObject LoadGameButton;
        [SerializeField]
        private GameObject ClinePlatform;
        [SerializeField]
        private GameObject RadialRockingPoint;
        [SerializeField]
        private GameObject DeathArea;
        [SerializeField]
        private GameObject GuardAndroid;

        public static EnvironmentPrefabs instance_
        {
            get
            {
                if(instance==null)
                    throw new ServantRequaringCompDeletingException("EnvironmentPrefabs");
                return instance;
            }
            private set=>instance = value;
        }
        private static EnvironmentPrefabs instance;
        EnvironmentPrefabs ISingltone<EnvironmentPrefabs>.Singltone { get => instance; set => instance = value; }
        void ISingltone<EnvironmentPrefabs>.Destroy() =>
            Destroy(this);
        public void Awake()
        {
            this.ValidateSingltone();
        }
        public void OnDestroy()
        {
            if (!Application.isEditor)
            {
                throw new ServantRequaringCompDeletingException(TypeDescriptor.GetClassName(this));
            }
        }
    }
}

