using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manapotion.MainMenu {
    public class OptionsButton : MainMenuButton {
        
        public override void OnHover() {
            subtitleObject.text = subtitle;
        }

        public override void OnClick() {

        }
    }
}