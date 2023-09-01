using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD
    {
        public event Action GoingToBackGroundEvent = delegate { };
        public event Action ReturnToCharactersLayerEvent=delegate { };
        public bool IsInBackGroundLayer_ => gameObject.layer == Registry.BackGroundCharactersLayer;
        private void ChangeLayerToBackGround()
        {
            if(!IsInBackGroundLayer_)
            {
                gameObject.layer = Registry.BackGroundCharactersLayer;
                GoingToBackGroundEvent();
            }    
        }
        private void ChangeLayerToCharacters()
        {
            if (IsInBackGroundLayer_)
            {
                gameObject.layer = Registry.CharactersLayer;
                ReturnToCharactersLayerEvent();
            }
        }
    }
}
