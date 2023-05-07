
using System.ComponentModel;
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.Serialization
{
    public sealed class EnvironmentPrefabs:MonoBehaviour,ISingltone<EnvironmentPrefabs>
    {
        public static GameObject MainCharacterPrefab => instance_.MainCharacterPrefab_;
        public static GameObject NonePassablePlatform => instance_.NonePassablePlatform_;
        public static GameObject SimpleLocationTransit => instance_.SimpleLocationTransit_;
        public static GameObject FAKELocationTransit => instance_.FAKELocationTransit_;
        public static GameObject Wall => instance_.Wall_;
        public static GameObject MovableBox => instance_.MovableBox_;
        public static GameObject SaveInfoContainer_ => instance_.SaveInfoContainer;
        public static GameObject CheckPoint_ => instance_.CheckPoint;
        public static GameObject StartButton_ => instance_.StartButton;
        public static GameObject LoadGameButton_ => instance_.LoadGameButton;

        [SerializeField]
        private GameObject MainCharacterPrefab_;
        [SerializeField]
        private GameObject NonePassablePlatform_;
        [SerializeField]
        private GameObject SimpleLocationTransit_;
        [SerializeField]
        private GameObject FAKELocationTransit_;
        [SerializeField]
        private GameObject Wall_;
        [SerializeField]
        private GameObject MovableBox_;
        [SerializeField]
        private GameObject SaveInfoContainer;
        [SerializeField]
        private GameObject CheckPoint;
        [SerializeField]
        private GameObject StartButton;
        [SerializeField]
        private GameObject LoadGameButton;

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
        public void Awake()
        {
            Singltone.Initialization(this, () => Destroy(this), () => { });
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

